using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Refit;
using user_account_service.Client;
using user_account_service.Services.Implementations; // Pour AuthService etc.
using user_account_service.Config; // Pour PasswordService, JwtTokenService
using user_account_service.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// =======================
// Services
// =======================

// Controllers + API Explorer + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("Jwt:Secret manquant : ajoutez-le dans appsettings ou variables d'environnement.");
}

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true
        };
    });

// Authorization
builder.Services.AddAuthorization();

// DbContext (si vous utilisez Entity Framework)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));
// =======================
// Injection des dépendances (Services)
// =======================
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthService>();

// Refit Client
builder.Services
    .AddRefitClient<IEmployeeApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("http://localhost:5063"); // employee-service
    });

var app = builder.Build();

app.UseForwardedHeaders();

var pathBase = app.Configuration["PathBase"];
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

// =======================
// Middleware
// =======================

var enableSwagger = app.Environment.IsDevelopment()
    || app.Configuration.GetValue("EnableSwagger", false);
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "user-account-service v1");
    });
}

if (!app.Configuration.GetValue("DisableHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

// JWT Middlewares (ordre IMPORTANT)
app.UseAuthentication();
app.UseAuthorization();

// =======================
// Mapping des Controllers
// =======================
app.MapGet("/health", () => Results.Text("user-account-service OK", "text/plain"))
    .ExcludeFromDescription();

if (enableSwagger)
{
    app.MapGet("/", (HttpContext ctx) =>
    {
        var prefix = ctx.Request.PathBase.HasValue ? ctx.Request.PathBase.Value! : string.Empty;
        var target = string.IsNullOrEmpty(prefix) ? "/swagger" : $"{prefix.TrimEnd('/')}/swagger";
        return Results.Redirect(target);
    }).ExcludeFromDescription();
}

app.MapControllers(); // ✅ Active les controllers (AuthController, etc.)

app.Run();