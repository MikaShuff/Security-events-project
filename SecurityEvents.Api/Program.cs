// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Share cookie encryption keys between api.security.* and win.security.*
// so a cookie issued by WIN can be read by API.
var keysPath = builder.Configuration["DataProtection:KeysPath"] ?? @"C:\inetpub\dpkeys\SecurityEvents";
var appName = builder.Configuration["DataProtection:ApplicationName"] ?? "SecurityEvents";

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName(appName);

builder.Services.AddControllers();
builder.Services.AddSingleton<AdAuthService>();

const string corsPolicyName = "AllowSecurityFrontend";

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins(
                "https://security.shufersal.co.il",
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Toggle Windows Auth (Negotiate) by configuration
// - Dev: false by default
// - Win site in prod: set WindowsAuth:Enabled=true
var enableWindowsAuth = builder.Configuration.GetValue<bool>("WindowsAuth:Enabled");

// Authentication: Cookie always + Negotiate only when enabled
var authBuilder = builder.Services
    .AddAuthentication(options =>
    {
        // Most endpoints use cookies (after login)
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "sec_auth";
        options.Cookie.Path = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // IMPORTANT: only set Domain in production (never on localhost)
        if (!builder.Environment.IsDevelopment())
        {
            options.Cookie.Domain = ".security.shufersal.co.il";
        }

        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // For APIs return 401/403 instead of redirect
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; },
            OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; },
        };
    });

if (enableWindowsAuth)
{
    // Needed only on the WIN host behind IIS Windows Authentication
    authBuilder.AddNegotiate();
}

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS must be before auth
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();