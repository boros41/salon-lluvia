using Microsoft.AspNetCore.Mvc;
using Mvc.Integrations.Calendly;

namespace Mvc.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class CalendlyController : ControllerBase
{
    private readonly CalendlyClient _calendly;

    public CalendlyController(CalendlyClient calendly)
    {
        _calendly = calendly;
    }

    [HttpGet("available-days")]
    public async Task<OkObjectResult> Get()
    {
        HashSet<string> availableDays = await _calendly.GetAvailableDays();

        return Ok(availableDays);
    }
}
