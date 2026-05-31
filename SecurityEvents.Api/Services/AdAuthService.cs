using System.DirectoryServices.Protocols;
using System.Net;

public sealed class AdAuthService
{
    private readonly string _host;
    private readonly int _port;
    private readonly bool _useSsl;
    private readonly bool _skipCertValidation;
    private readonly string _baseDn;
    private readonly string _upnSuffix;

    public AdAuthService(IConfiguration cfg, IWebHostEnvironment env)
    {
        _host = cfg["Ldap:Host"] ?? throw new InvalidOperationException("Missing config: Ldap:Host");
        _baseDn = cfg["Ldap:BaseDn"] ?? throw new InvalidOperationException("Missing config: Ldap:BaseDn");
        _upnSuffix = cfg["Ldap:UpnSuffix"] ?? throw new InvalidOperationException("Missing config: Ldap:UpnSuffix");

        _port = int.TryParse(cfg["Ldap:Port"], out var p) ? p : 636;
        _useSsl = !bool.TryParse(cfg["Ldap:UseSsl"], out var useSsl) || useSsl;

        // Dev-only escape hatch for self-signed/untrusted LDAPS certs
        _skipCertValidation =
            env.IsDevelopment() &&
            bool.TryParse(cfg["Ldap:SkipCertValidation"], out var skip) &&
            skip;
    }

    /// <summary>
    /// Validates username+password by binding to LDAP with those credentials,
    /// then returns displayName + group CNs.
    /// </summary>
    public (bool ok, string? displayName, List<string> groupCns) Validate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, null, new());

        var sam = ExtractSam(username);
        var bindUserUpn = username.Contains('@') ? username : $"{sam}@{_upnSuffix}";

        using var conn = CreateConnection(
            credential: new NetworkCredential(bindUserUpn, password),
            authType: AuthType.Negotiate);

        try
        {
            conn.Bind(); // validates password
        }
        catch
        {
            return (false, null, new());
        }

        return QueryUser(conn, sam);
    }

    /// <summary>
    /// Looks up displayName + group CNs using the current process identity
    /// (IIS app pool / service account). Used for Windows SSO.
    /// </summary>
    public (string? displayName, List<string> groupCns) LookupByWindowsIdentity(string windowsIdentityName)
    {
        var sam = ExtractSam(windowsIdentityName);

        using var conn = CreateConnection(
            credential: null,              // use process identity
            authType: AuthType.Negotiate); // works with Windows identity

        conn.Bind();

        var (ok, displayName, groups) = QueryUser(conn, sam);
        return ok ? (displayName, groups) : (null, new());
    }

    // -------------------- internals --------------------

    private LdapConnection CreateConnection(NetworkCredential? credential, AuthType authType)
    {
        var conn = new LdapConnection(new LdapDirectoryIdentifier(_host, _port))
        {
            AuthType = authType
        };

        if (credential != null)
            conn.Credential = credential;

        conn.SessionOptions.ProtocolVersion = 3;

        if (_useSsl)
            conn.SessionOptions.SecureSocketLayer = true;

        if (_skipCertValidation)
            conn.SessionOptions.VerifyServerCertificate += (_, __) => true;

        return conn;
    }

    private (bool ok, string? displayName, List<string> groupCns) QueryUser(LdapConnection conn, string sam)
    {
        // Good default filter for AD user objects
        var filter =
            "(&" +
            "(objectCategory=person)" +
            "(objectClass=user)" +
            $"(sAMAccountName={EscapeLdapFilterValue(sam)})" +
            ")";

        var req = new SearchRequest(
            _baseDn,
            filter,
            SearchScope.Subtree,
            "displayName",
            "memberOf");

        var resp = (SearchResponse)conn.SendRequest(req);
        var entry = resp.Entries.Count > 0 ? resp.Entries[0] : null;

        if (entry == null)
            return (false, null, new());

        var displayName = GetFirstString(entry, "displayName");
        var groupDns = GetAllStrings(entry, "memberOf");

        var groupCns = groupDns
            .Select(ParseCnFromDn)
            .Where(cn => !string.IsNullOrWhiteSpace(cn))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()!;

        return (true, displayName, groupCns);
    }

    private static string? GetFirstString(SearchResultEntry entry, string attributeName)
        => GetAllStrings(entry, attributeName).FirstOrDefault();

    private static IEnumerable<string> GetAllStrings(SearchResultEntry entry, string attributeName)
    {
        var attr = entry.Attributes[attributeName];
        if (attr == null || attr.Count == 0)
            return Array.Empty<string>();

        // Cleanest way to get AD attribute values as strings without manual encoding guesses
        return attr.GetValues(typeof(string)).Cast<string>();
    }

    private static string ExtractSam(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // DOMAIN\user
        var idx = input.IndexOf('\\');
        if (idx >= 0 && idx < input.Length - 1)
            return input[(idx + 1)..];

        // user@domain
        idx = input.IndexOf('@');
        if (idx > 0)
            return input[..idx];

        // user
        return input;
    }

    private static string EscapeLdapFilterValue(string value)
        => value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");

    private static string? ParseCnFromDn(string? dn)
    {
        if (string.IsNullOrWhiteSpace(dn)) return null;

        // DN format: CN=GroupName,OU=...,DC=...
        var first = dn.Split(',').FirstOrDefault();
        if (first != null && first.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
            return first.Substring(3);

        return null;
    }
}