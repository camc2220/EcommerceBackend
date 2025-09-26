using EcommerceBackend.Data;
using EcommerceBackend.Services;
using EcommerceBackend.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });
});
builder.Services.AddScoped<TokenService>();

// DB - Connection string from env: ConnectionStrings__DefaultConnection or DATABASE_URL
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(conn))
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        conn = BuildConnectionStringFromUrl(databaseUrl);
    }
}

if (string.IsNullOrWhiteSpace(conn))
{
    // fallback local for development
    conn = "Host=localhost;Port=5432;Database=ecommercedb;Username=postgres;Password=postgres";
}
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]
              ?? Environment.GetEnvironmentVariable("Jwt__Key")
              ?? "development-secret-key-change-me";

if (jwtKey.Length < 16)
{
    throw new InvalidOperationException(
        "JWT key must be at least 16 characters long. Set Jwt:Key/Jwt__Key to a secure value.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();

// CORS - allow all for demo (restrict in production)
builder.Services.AddCors(p => p.AddDefaultPolicy(pb => pb.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI(c => 
{ 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1"); 
    c.RoutePrefix = string.Empty; 
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure the latest migrations are applied automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var usersWithoutNormalizedEmail = await db.Users
        .Where(u => string.IsNullOrWhiteSpace(u.NormalizedEmail) && !string.IsNullOrWhiteSpace(u.Email))
        .ToListAsync();

    if (usersWithoutNormalizedEmail.Count > 0)
    {
        foreach (var user in usersWithoutNormalizedEmail)
        {
            user.NormalizedEmail = user.Email.ToLowerInvariant();
        }

        await db.SaveChangesAsync();
    }
}

app.Run();

static string BuildConnectionStringFromUrl(string databaseUrl)
{
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
    {
        return databaseUrl;
    }

    var host = uri.Host;
    var port = uri.IsDefaultPort ? 5432 : uri.Port;
    var database = uri.AbsolutePath.Trim('/');

    var userInfo = uri.UserInfo.Split(':', 2);
    var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

    var sslMode = uri.Scheme.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)
        ? "Require"
        : "Disable";

    return $"Host={host};Port={port};Database={database};Username={username};Password={password};Ssl Mode={sslMode};Trust Server Certificate=true";
}
