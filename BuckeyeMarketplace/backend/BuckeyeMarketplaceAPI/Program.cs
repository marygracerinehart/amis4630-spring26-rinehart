using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BuckeyeMarketplaceAPI.Data;
using BuckeyeMarketplaceAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add CORS with production security
var frontendUrl = builder.Configuration["FrontendUrl"]
    ?? (builder.Environment.IsDevelopment() ? "http://localhost:3000" : null);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsBuilder =>
    {
        if (string.IsNullOrEmpty(frontendUrl))
        {
            corsBuilder.SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
        else
        {
            corsBuilder.WithOrigins(frontendUrl)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
        }
    });
});

// Add Entity Framework with Azure SQL (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection is not configured. Run: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;\"");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is not configured. Run: dotnet user-secrets set \"Jwt:Key\" \"<your-256-bit-secret>\"");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        // Apply any pending migrations
        await context.Database.MigrateAsync();
        
        // Seed initial data
        await DbSeeder.SeedAsync(context, scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Use CORS - must be before auth and after routing
app.UseCors("AllowAll");

// Enforce HTTPS in production
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// ── Security Headers ────────────────────────────────────────────────
// CI/CD Pipeline: Automatic deployment enabled
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    // Prevent MIME-type sniffing
    headers["X-Content-Type-Options"] = "nosniff";

    // Block clickjacking — only allow same-origin framing
    headers["X-Frame-Options"] = "DENY";

    // Opt out of the legacy XSS auditor (modern browsers ignore it;
    // setting to "0" avoids unexpected filter side-effects)
    headers["X-XSS-Protection"] = "0";

    // Only send the origin as the referrer to cross-origin requests
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

    // Restrict browser features the API doesn't need
    headers["Permissions-Policy"] =
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()";

    // Content-Security-Policy — frame-ancestors only; no default-src to avoid blocking API consumers
    headers["Content-Security-Policy"] = "frame-ancestors 'none'";

    // Prevent caching of authenticated responses
    headers["Cache-Control"] = "no-store";
    headers["Pragma"] = "no-cache";

    await next();
});

// Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class accessible to the integration-test project
public partial class Program { }