// Program.cs

using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var corsPolicyName = "AllowSecurityFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.WithOrigins("http://security.shufersal.co.il")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();


// HTTPS redirect only outside development (כמו אצלך)

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


//  CORS פעם אחת, עם policy אחת

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
