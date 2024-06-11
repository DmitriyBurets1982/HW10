using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("userservice/[controller]")]
    public class HealthController(ILogger<HealthController> logger) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            logger.LogInformation($"{nameof(UserService)}->{nameof(GetHealth)}' was called");
            return Ok(new { status = $"{nameof(UserService)}: OK" });
        }
    }
}
