using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("notificationservice/[controller]")]
    public class HealthController(ILogger<HealthController> logger) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            logger.LogInformation($"{nameof(NotificationService)}->{nameof(GetHealth)}' was called");
            return Ok(new { status = $"{nameof(NotificationService)}: OK" });
        }
    }
}
