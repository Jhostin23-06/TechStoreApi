using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Interfaces;
using TechStore.Infrastructure.Data;
using TechStore.Infrastructure.Repositories;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";

if (!string.IsNullOrEmpty(connectionString) && !connectionString.EndsWith(";"))
{
    connectionString += ";";
}
connectionString += $"User={dbUser};Password={dbPassword};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register application services
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, TechStore.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<TechStore.Domain.Interfaces.IClientRepository, TechStore.Infrastructure.Repositories.ClientRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();