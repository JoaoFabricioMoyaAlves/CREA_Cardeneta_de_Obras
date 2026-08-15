using Microsoft.AspNetCore.Mvc;

namespace CadernetaObras.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "ok", timestampUtc = DateTime.UtcNow });
}
