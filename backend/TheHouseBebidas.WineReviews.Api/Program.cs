using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using TheHouseBebidas.WineReviews.Application;
using TheHouseBebidas.WineReviews.Api.Middleware;
using TheHouseBebidas.WineReviews.Infrastructure;
using TheHouseBebidas.WineReviews.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer configuration is required.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience configuration is required.");
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key configuration is required.");

if (string.Equals(jwtKey, "__SET_VIA_USER_SECRETS_OR_ENV__", StringComparison.Ordinal)
    || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must come from user-secrets or environment variables and be at least 32 characters long.");
}

builder.Services.AddControllers();
builder.Services.AddHttpClient("ImageProxyClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("TheHouseBebidasImageProxy/1.0");
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4173",
                "http://127.0.0.1:4173",
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = static async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                type = "https://httpstatuses.com/429",
                title = "Too Many Requests",
                status = StatusCodes.Status429TooManyRequests,
                detail = "Too many review attempts. Please retry later.",
                instance = context.HttpContext.Request.Path.Value
            },
            cancellationToken);
    };

    options.AddPolicy("PublicReviewCreatePolicy", httpContext =>
    {
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
        var host = httpContext.Request.Host.Host;
        var partitionKey = !string.IsNullOrWhiteSpace(remoteIp)
            ? remoteIp
            : (!string.IsNullOrWhiteSpace(host) ? host : "unknown-client");

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            static _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("FrontendClient");
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    var adminUserSeeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
    await adminUserSeeder.SeedAsync();

    if (app.Environment.IsDevelopment())
    {
        var developmentSampleDataSeeder = scope.ServiceProvider.GetRequiredService<DevelopmentSampleDataSeeder>();
        await developmentSampleDataSeeder.SeedAsync();
    }
}

app.Run();
