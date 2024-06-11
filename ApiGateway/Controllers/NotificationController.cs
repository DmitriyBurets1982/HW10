using Contracts.NotificationService;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController(HttpClient httpClient, IConfiguration configuration, ILogger<NotificationController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int userId)
    {
        var url = $"{configuration["NotificationService"]}/notificationservice/notification/all/{userId}";
        logger.LogInformation("NotificationService url: '{Url}'", url);

        using var response = await httpClient.GetAsync(url);
        var notifications = await response.Content.ReadFromJsonAsync<IEnumerable<Notification>>();

        return Ok(notifications);
    }
}
