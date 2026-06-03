using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TeamsApi.Middleware;
using TeamsApi.Repositories;
using TeamsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Entity Framework + PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// JWT Authentication (shared with api-users)
var jwtSecret = builder.Configuration["Jwt:SecretKey"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtSecret))
    throw new InvalidOperationException("Jwt:SecretKey configuration is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Repositories & Services
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<ISquadRepository, SquadRepository>();
builder.Services.AddScoped<IProfessionalService, ProfessionalService>();
builder.Services.AddScoped<ISquadService, SquadService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Pipeline
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.Title = "Kiro Teams API"; });
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database tables exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

    try
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS professionals (
                "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
                "AwsUserId" varchar(100) NOT NULL,
                "Name" varchar(255) NOT NULL,
                "Email" varchar(255) NOT NULL,
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(),
                "UpdatedAt" timestamp with time zone,
                CONSTRAINT "UQ_professionals_AwsUserId" UNIQUE ("AwsUserId"),
                CONSTRAINT "UQ_professionals_Email" UNIQUE ("Email")
            );

            CREATE TABLE IF NOT EXISTS squads (
                "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
                "Name" varchar(255) NOT NULL,
                "Description" varchar(500),
                "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(),
                "UpdatedAt" timestamp with time zone,
                CONSTRAINT "UQ_squads_Name" UNIQUE ("Name")
            );

            CREATE TABLE IF NOT EXISTS squad_professionals (
                "SquadId" uuid NOT NULL REFERENCES squads("Id") ON DELETE CASCADE,
                "ProfessionalId" uuid NOT NULL REFERENCES professionals("Id") ON DELETE CASCADE,
                "AssignedAt" timestamp with time zone NOT NULL DEFAULT now(),
                PRIMARY KEY ("SquadId", "ProfessionalId")
            );
        """);

        logger.LogInformation("Teams tables ensured.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to ensure teams tables.");
        throw;
    }
}

app.Run();

public partial class Program { }
