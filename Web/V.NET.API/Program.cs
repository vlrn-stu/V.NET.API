using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using V.NET.API.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

app.UseHttpsRedirection();

// Enable Rate Limiting Middleware globally
app.UseRateLimiter();

// Authorization Middleware
app.UseAuthorization();

// Map controllers and apply rate limiting policy
app.MapControllers().RequireRateLimiting("GlobalRateLimit");

app.Run();
