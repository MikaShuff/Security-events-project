//Controllers/AuthController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AdAuthService _ad;

    public AuthController(AdAuthService ad) => _ad = ad;

    public record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest();

        try
        {
            var (ok, displayName, groups) = _ad.Validate(req.Username, req.Password);

            if (!ok)
                return Unauthorized(new { error = "Invalid username or password" });

            // AD group -> role mapping (by CN)
            string? role =
                groups.Contains("Security_Events_Admins") ? "Admin" :
                groups.Contains("Security_Events_Users") ? "User" :
                groups.Contains("Security_Events_Viewers") ? "Viewer" :
                null;

            if (role is null)
                return StatusCode(403, new { groups });

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, req.Username),
                new("displayName", displayName ?? req.Username),
                new(ClaimTypes.Role, role),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { username = req.Username, role });
        }
        catch (Exception ex)
        {
             return Unauthorized(new { error = "Invalid username or password" });
        }
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
            role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
        });
    }

    [Authorize]
    [HttpPost("windows-login")]
    public async Task<IActionResult> WindowsLogin()
    {
        // This requires Windows Authentication to be enabled (for this endpoint),
        // so HttpContext.User is a Windows principal.
        if (!(User?.Identity?.IsAuthenticated ?? false))
            return Unauthorized();

        var username = User.Identity!.Name ?? "unknown";

        // TODO: map AD groups -> role (reuse your AdAuthService lookup-by-username, no password)
        // For now, at least issue a cookie:
        var claims = new List<Claim>
    {
        new(ClaimTypes.Name, username),
        new(ClaimTypes.Role, "User"),
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Ok(new { username });
    }
}