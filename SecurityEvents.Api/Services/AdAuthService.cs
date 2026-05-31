//Services/AdAuthService.cs
using System.DirectoryServices.Protocols;
using System.Net;

public class AdAuthService
{
    private readonly string _ldapHost;
    private readonly int _ldapPort = 636;
    private readonly string _baseDn;
    private readonly string _upnSuffix;

    private static string DecodeLdapString(byte[] bytes)
    {
        // AD commonly uses UTF-8 for LDAP string values; if you see garbage,
        // try Encoding.Unicode.
        var s = System.Text.Encoding.UTF8.GetString(bytes);

        // If it doesn't look like a DN, try Unicode as fallback
        if (!s.Contains("CN=", StringComparison.OrdinalIgnoreCase) && bytes.Length % 2 == 0)
        {
            var s2 = System.Text.Encoding.Unicode.GetString(bytes);
            if (s2.Contains("CN=", StringComparison.OrdinalIgnoreCase))
                return s2;
        }

        return s;
    }

    public AdAuthService(IConfiguration cfg)
    {
        _ldapHost = cfg["Ldap:Host"]!;
        _baseDn = cfg["Ldap:BaseDn"]!;
        _upnSuffix = cfg["Ldap:UpnSuffix"]!;
    }

    public (bool ok, string? displayName, List<string> groupCns) Validate(string username, string password)
    {
        var sam = username.Contains('\\') ? username.Split('\\')[1]
        : username.Contains('@') ? username.Split('@')[0]
        : username;

        // bind using UPN (no NetBIOS needed)
        var bindUser = username.Contains("@") ? username : $"{sam}@{_upnSuffix}";

        // 1) Connect + bind (validates password)
        using var conn = new LdapConnection(new LdapDirectoryIdentifier(_ldapHost, _ldapPort))
        {
            AuthType = AuthType.Negotiate,
            Credential = new NetworkCredential(bindUser, password),
        };

        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = true;

        // IMPORTANT: in production you should validate the certificate properly.
        // If you don’t have a trusted cert chain yet, you can temporarily allow it (dev only):
        // conn.SessionOptions.VerifyServerCertificate += (_, __) => true;

        try
        {
            conn.Bind();
        }
        catch (Exception ex)
        {
            throw new Exception("LDAP bind failed: " + ex.Message, ex);
        }

        // 2) Search user by sAMAccountName
        var filter = $"(sAMAccountName={EscapeLdapFilterValue(sam)})";



        var request = new SearchRequest(
            _baseDn,
            filter,
            SearchScope.Subtree,
            new[] { "displayName", "memberOf"});

        var response = (SearchResponse)conn.SendRequest(request);

        var entry = response.Entries.Count > 0 ? response.Entries[0] : null;
        if (entry == null)
            return (false, null, new List<string>());

        string? displayName = entry.Attributes["displayName"]?.Count > 0
            ? entry.Attributes["displayName"][0]?.ToString()
            : null;

        var groupCns = new List<string>();

        foreach (string attrName in entry.Attributes.AttributeNames.Cast<string>())
        {
            if (!attrName.StartsWith("memberOf", StringComparison.OrdinalIgnoreCase))
                continue;

            var memberOf = entry.Attributes[attrName];
            if (memberOf == null) continue;

            foreach (var g in memberOf)
            {
                string? dn = g switch
                {
                    string s => s,
                    byte[] bytes => DecodeLdapString(bytes),
                    _ => g?.ToString()
                };

                var cn = ParseCnFromDn(dn);
                if (!string.IsNullOrWhiteSpace(cn))
                    groupCns.Add(cn);
            }
        }

        return (true, displayName, groupCns);
    }



    private static string EscapeLdapFilterValue(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }

    private static string? ParseCnFromDn(string? dn)
    {
        if (string.IsNullOrWhiteSpace(dn)) return null;
        var part = dn.Split(',').FirstOrDefault();
        if (part != null && part.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
            return part.Substring(3);
        return null;
    }


    public (string? displayName, List<string> groupCns) LookupByWindowsIdentity(string windowsIdentityName)
    {
        // windowsIdentityName can be:
        // - DOMAIN\user
        // - user@domain
        // - user
        var sam =
            windowsIdentityName.Contains('\\') ? windowsIdentityName.Split('\\')[1] :
            windowsIdentityName.Contains('@') ? windowsIdentityName.Split('@')[0] :
            windowsIdentityName;

        using var conn = new LdapConnection(new LdapDirectoryIdentifier(_ldapHost, _ldapPort))
        {
            // IMPORTANT:
            // This uses the process identity (AppPool / service account) to query LDAP.
            // Make sure that identity has permission to read user attributes & memberOf.
            AuthType = AuthType.Negotiate,
        };

        conn.SessionOptions.ProtocolVersion = 3;
        conn.SessionOptions.SecureSocketLayer = true;

        // If you need this in dev only (self-signed cert), you can uncomment:
        // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        //     conn.SessionOptions.VerifyServerCertificate += (_, __) => true;

        conn.Bind();

        var filter = $"(sAMAccountName={EscapeLdapFilterValue(sam)})";
        var request = new SearchRequest(
            _baseDn,
            filter,
            SearchScope.Subtree,
            new[] { "displayName", "memberOf" });

        var response = (SearchResponse)conn.SendRequest(request);

        var entry = response.Entries.Count > 0 ? response.Entries[0] : null;
        if (entry == null)
            return (null, new List<string>());

        string? displayName = entry.Attributes["displayName"]?.Count > 0
            ? entry.Attributes["displayName"][0]?.ToString()
            : null;

        var groupCns = new List<string>();

        foreach (string attrName in entry.Attributes.AttributeNames.Cast<string>())
        {
            if (!attrName.StartsWith("memberOf", StringComparison.OrdinalIgnoreCase))
                continue;

            var memberOf = entry.Attributes[attrName];
            if (memberOf == null) continue;

            foreach (var g in memberOf)
            {
                string? dn = g switch
                {
                    string s => s,
                    byte[] bytes => DecodeLdapString(bytes),
                    _ => g?.ToString()
                };

                var cn = ParseCnFromDn(dn);
                if (!string.IsNullOrWhiteSpace(cn))
                    groupCns.Add(cn);
            }
        }

        return (displayName, groupCns);
    }
}