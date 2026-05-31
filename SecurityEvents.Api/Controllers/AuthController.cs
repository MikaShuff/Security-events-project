// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AdAuthService _ad;
    private readonly bool _windowsAuthEnabled;

    public AuthController(AdAuthService ad, IConfiguration cfg)
    {
        _ad = ad;
        _windowsAuthEnabled = cfg.GetValue<bool>("WindowsAuth:Enabled");
    }

    public record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest();

        var (ok, displayName, groups) = _ad.Validate(req.Username, req.Password);
        if (!ok)
            return Unauthorized(new { error = "Invalid username or password" });

        var role = MapRole(groups);
        if (role is null)
            return Forbid(); // cleaner than StatusCode(403, ...)

        await SignInCookie(req.Username, displayName ?? req.Username, role);
        return Ok(new { username = req.Username, role });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
        => Ok(new
        {
            username = User.Identity?.Name,
            role = User.FindFirstValue(ClaimTypes.Role),
            displayName = User.FindFirstValue("displayName")
        });

    // Windows SSO endpoint (only works when Negotiate is enabled on this host)
    [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    [HttpPost("windows-login")]
    public async Task<IActionResult> WindowsLogin()
    {
        if (!_windowsAuthEnabled)
            return StatusCode(501, new { error = "Windows SSO is not enabled on this host" });

        if (!(User?.Identity?.IsAuthenticated ?? false))
            return Unauthorized();

        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized();

        var (displayName, groups) = _ad.LookupByWindowsIdentity(username);
        var role = MapRole(groups) ?? "User";

        await SignInCookie(username, displayName ?? username, role);
        return Ok(new { username, role });
    }

    private async Task SignInCookie(string username, string displayName, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("displayName", displayName),
            new(ClaimTypes.Role, role),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private static string? MapRole(IEnumerable<string> groups)
    {
        var set = new HashSet<string>(groups ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

        if (set.Contains("Security_Events_Admins")) return "Admin";
        if (set.Contains("Security_Events_Users")) return "User";
        if (set.Contains("Security_Events_Viewers")) return "Viewer";
        return null;
    }

}