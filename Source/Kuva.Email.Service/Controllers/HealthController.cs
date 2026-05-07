using Microsoft.AspNetCore.Mvc;

namespace Kuva.Email.Service.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
        => Ok(new
        {
            service = "Kuva.Email",
            status = "Healthy",
            utc = DateTime.UtcNow
        });
}
