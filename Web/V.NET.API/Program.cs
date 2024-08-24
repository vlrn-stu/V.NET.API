using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using V.NET.API.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", builder =>
    {
        builder.WithOrigins("https://valrina.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("GlobalRateLimit", policyOptions =>
    {
        policyOptions.PermitLimit = 100;
        policyOptions.Window = TimeSpan.FromMinutes(60);
        policyOptions.QueueLimit = 2;
        policyOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

// Configure Forwarded Headers to handle X-Forwarded-For
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Clear default known networks and proxies to allow all
    options.KnownNetworks.Clear(); // Only loopback proxies are allowed by default
    options.KnownProxies.Clear();  // Clear the default known proxies (none in this case)
});

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts(); // Enable HSTS in production
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

// Apply forwarded headers middleware before authentication and other middlewares
app.UseForwardedHeaders();

// Enable Rate Limiting Middleware globally
app.UseRateLimiter();

// Authorization Middleware
app.UseAuthorization();

// Map controllers and apply rate limiting policy
app.MapControllers().RequireRateLimiting("GlobalRateLimit");

app.Run();
