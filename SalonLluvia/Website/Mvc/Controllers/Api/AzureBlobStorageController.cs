using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Integrations.AzureBlobStorage.Interfaces;

namespace Mvc.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class AzureBlobStorageController : ControllerBase
{
    private readonly IAzureBlobStorageImages _blobStorageImages;
    private readonly IMemoryCache _memoryCache;

    public AzureBlobStorageController(IAzureBlobStorageImages blobStorageImages, IMemoryCache memoryCache)
    {
        _blobStorageImages = blobStorageImages;
        _memoryCache = memoryCache;
    }

    [HttpGet("image-url")]
    public async Task<IActionResult> Get()
    {
        string imageUrl = await _blobStorageImages.GetImageUrl();

        return Ok(imageUrl);
    }
}
