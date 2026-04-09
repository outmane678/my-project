using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Refit;
using user_account_service.Client;
using user_account_service.Services.Implementations; // Pour AuthService etc.
using user_account_service.Config; // Pour PasswordService, JwtTokenService
using user_account_service.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// =======================
// Services
// =======================

// Controllers + API Explorer + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])
            ),
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

// =======================
// Middleware
// =======================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// JWT Middlewares (ordre IMPORTANT)
app.UseAuthentication();
app.UseAuthorization();

// =======================
// Mapping des Controllers
// =======================
app.MapControllers(); // ✅ Active les controllers (AuthController, etc.)

app.Run();