using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace RateLimitingSample;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("token")]
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello world, from controller";
}
