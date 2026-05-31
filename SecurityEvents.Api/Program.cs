// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<AdAuthService>();

var corsPolicyName = "AllowSecurityFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("https://security.shufersal.co.il", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
  .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
      options.Cookie.Name = "sec_auth";
      options.Cookie.Path = "/";
      options.Cookie.HttpOnly = true;
      options.Cookie.SameSite = SameSiteMode.None;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

      // IMPORTANT: don't set Domain on localhost
      if (!builder.Environment.IsDevelopment())
      {
          options.Cookie.Domain = ".security.shufersal.co.il";
      }

      options.SlidingExpiration = true;
      options.ExpireTimeSpan = TimeSpan.FromHours(8);

      options.Events = new CookieAuthenticationEvents
      {
          OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; },
          OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; },
      };
  });

builder.Services.AddAuthorization();


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//  CORS פעם אחת, עם policy אחת
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
