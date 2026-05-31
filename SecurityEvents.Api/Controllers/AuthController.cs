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
public class AuthController : ControllerBase
{
    private readonly AdAuthService _ad;
    private readonly IConfiguration _cfg;

    public AuthController(AdAuthService ad, IConfiguration cfg)
    {
        _ad = ad;
        _cfg = cfg;
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
            return StatusCode(403, new { groups });

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, req.Username),
            new("displayName", displayName ?? req.Username),
            new(ClaimTypes.Role, role),
        };

        await SignInCookie(claims);
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
    {
        return Ok(new
        {
            username = User.Identity?.Name,
            role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
            displayName = User.Claims.FirstOrDefault(c => c.Type == "displayName")?.Value
        });
    }

    // Windows SSO endpoint (works only when Negotiate is enabled on this host)
    [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme)]
    [HttpPost("windows-login")]
    public async Task<IActionResult> WindowsLogin()
    {
        // If Negotiate is not enabled on this host, this endpoint won't authenticate.
        // But in that case we prefer a clear message:
        var windowsEnabled = _cfg.GetValue<bool>("WindowsAuth:Enabled");
        if (!windowsEnabled)
            return StatusCode(501, new { error = "Windows SSO is not enabled on this host" });

        if (!(User?.Identity?.IsAuthenticated ?? false))
            return Unauthorized();

        var username = User.Identity?.Name ?? "unknown";

        // Optional: lookup displayName + groups using AD without password
        var (displayName, groups) = _ad.LookupByWindowsIdentity(username);
        var role = MapRole(groups) ?? "User";

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("displayName", displayName ?? username),
            new(ClaimTypes.Role, role),
        };

        await SignInCookie(claims);
        return Ok(new { username, role });
    }

    private async Task SignInCookie(List<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private static string? MapRole(List<string> groups)
    {
        if (groups.Contains("Security_Events_Admins")) return "Admin";
        if (groups.Contains("Security_Events_Users")) return "User";
        if (groups.Contains("Security_Events_Viewers")) return "Viewer";
        return null;
    }

    [AllowAnonymous]
    [HttpGet("debug/windows-auth")]
    public IActionResult DebugWindowsAuth([FromServices] IConfiguration cfg)
    {
        return Ok(new
        {
            windowsAuthEnabled = cfg.GetValue<bool>("WindowsAuth:Enabled"),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            host = Request.Host.Value
        });
    }
}