using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Integrations.AzureBlobStorage.Interfaces;
using Mvc.Utilities;

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
        try
        {
            HashSet<string> imageUrls = await _memoryCache.GetOrCreateAsync<HashSet<string>>(Tags.GalleryImagesCacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                return _blobStorageImages.GetImageUrlsAsync();
            }) ?? throw new InvalidOperationException($"Value for memory cache \"{Tags.GalleryImagesCacheKey}\" was null");

            return new JsonResult(imageUrls);
        }
        catch (RequestFailedException e)
        {
            return StatusCode(e.Status);
        }
        catch (AggregateException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (InvalidOperationException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}