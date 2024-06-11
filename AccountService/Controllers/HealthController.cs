using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("accountservice/[controller]")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        logger.LogInformation($"{nameof(AccountService)}->{nameof(GetHealth)}' was called");
        return Ok(new { status = $"{nameof(AccountService)}: OK" });
    }
}
