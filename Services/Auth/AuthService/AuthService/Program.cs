using AuthService.Configuration;
using AuthService.Data;
using AuthService.Mappers;
using AuthService.Repositories;
using AuthService.Security;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using AuthService.ErrorHandling;
using AuthService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ? OpenAPI natif (.NET 9) — compatible Scalar
builder.Services.AddOpenApi();

// ? CORS (Front Next.js en dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --- DI
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<PasswordPolicy>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthentificationService>();
builder.Services.AddScoped<IAuthMapper, AuthMapper>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddSingleton<IExceptionMapper, ExceptionMapper>();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- JWT options (Options pattern)
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// --- JWT service (génération token)
builder.Services.AddSingleton<IJwtService, JwtService>();

// --- JWT authentication (validation token)
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Missing Jwt configuration section.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),

            ClockSkew = TimeSpan.Zero
        };
    });

// ? Authorization (roles / policies)
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    // OpenAPI + Scalar
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseHttpsRedirection();

// ? Ordre important
app.UseCors("FrontDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();