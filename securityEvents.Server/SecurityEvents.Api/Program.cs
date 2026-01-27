// Program.cs

using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors(p => p.
    AllowAnyOrigin().
    AllowAnyHeader().
    AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
