using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("orderservice/[controller]")]
    public class HealthController(ILogger<HealthController> logger) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            logger.LogInformation($"{nameof(OrderService)}->{nameof(GetHealth)}' was called");
            return Ok(new { status = $"{nameof(OrderService)}: OK" });
        }
    }
}
