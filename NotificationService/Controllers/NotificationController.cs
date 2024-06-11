using Contracts.NotificationService;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Services;

namespace NotificationService.Controllers;

[ApiController]
[Route("notificationservice/[controller]")]
public class NotificationController(INotificationService notificationService, ILogger<NotificationController> logger) : ControllerBase
{
    [HttpGet("all/{userId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IList<Notification>), StatusCodes.Status200OK)]
    public ActionResult<IList<Notification>> GetAllByUserBy(int userId)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Id}' was called", nameof(GetAllByUserBy), userId);

        var notifications = notificationService.GetAllByAccountId(userId);
        if (!notifications.Any())
        {
            return NoContent();
        }

        return Ok(notifications);
    }


    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(Notification), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult GetLastByUserId(int userId)
    {
        logger.LogInformation("'{MethodName}' with parameter '{Id}' was called", nameof(GetLastByUserId), userId);

        var notification = notificationService.GetLastByAccountId(userId);
        if (notification == null)
        {
            return NoContent();
        }

        return Ok(notification);
    }
}
