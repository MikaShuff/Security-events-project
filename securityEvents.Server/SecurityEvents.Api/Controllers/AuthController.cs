// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AdAuthService _ad;

    public AuthController(AdAuthService ad)
    {
        _ad = ad;
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
            return Forbid();

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