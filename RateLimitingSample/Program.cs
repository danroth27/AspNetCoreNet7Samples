using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("token", options =>
    {
        options.TokenLimit = 2;
        options.TokensPerPeriod = 1;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
    });
    
});

var app = builder.Build();

app.UseRateLimiter();

app.MapGet("/", () => "Hello world");
app.MapGet("/limit", () => "Hello world").RequireRateLimiting("token");

app.MapControllers();

app.Run();