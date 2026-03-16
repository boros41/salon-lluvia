using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Mvc.Integrations.Calendly;
using System.Text.Json;

namespace Mvc.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class CalendlyController : ControllerBase
{
    private readonly CalendlyAvailableDays _availableDays;
    private readonly IMemoryCache _memoryCache;

    public CalendlyController(CalendlyAvailableDays availableDays, IMemoryCache memoryCache)
    {
        _availableDays = availableDays;
        _memoryCache = memoryCache;
    }

    [HttpGet("available-days")]
    public async Task<IActionResult> Get()
    {
        StringValues authorization = Request.Headers.Authorization;

        try
        {
            const string key = "available-days";
            HashSet<string> availableDays = await _memoryCache.GetOrCreateAsync(key, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return _availableDays.Get();
            }) ?? throw new InvalidOperationException($"Value for memory cache \"{key}\" was null");

            return Ok(availableDays);
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            int statusCode = (int)e.StatusCode.Value;
            return StatusCode(statusCode);
        }
        catch (JsonException e)
        {
            return StatusCode(StatusCodes.Status502BadGateway);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
