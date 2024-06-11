using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        logger.LogInformation($"{nameof(ApiGateway)}->{nameof(GetHealth)}' was called");
        return Ok(new { status = $"{nameof(ApiGateway)}: OK" });
    }
}
