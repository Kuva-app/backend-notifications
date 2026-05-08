using Microsoft.AspNetCore.Mvc;

namespace Kuva.Notifications.Service.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
        => Ok(new
        {
            service = "Kuva.Notifications",
            status = "Healthy",
            utc = DateTime.UtcNow
        });
}
