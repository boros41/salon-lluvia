using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Data.Repository;
using Mvc.Dto.AzureBlobStorage.ImageUrls;
using Mvc.Integrations.AzureBlobStorage.Interfaces;
using Mvc.Models.Gallery;
using Mvc.Utilities;

namespace Mvc.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class AzureBlobStorageController : ControllerBase
{
    private readonly IAzureBlobStorageImages _blobStorageImages;
    private readonly IMemoryCache _memoryCache;
    private readonly IRepository<Image> _imageRepo;

    public AzureBlobStorageController(IAzureBlobStorageImages blobStorageImages, IMemoryCache memoryCache, IRepository<Image> imageRepo)
    {
        _blobStorageImages = blobStorageImages;
        _memoryCache = memoryCache;
        _imageRepo = imageRepo;
    }

    [HttpGet("image-url")]
    public async Task<IActionResult> Get()
    {
        try
        {
            HashSet<Dictionary<string, string>> imageUrlMappings = await _memoryCache.GetOrCreateAsync(Tags.GalleryImagesCacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);

                return _blobStorageImages.GetImageUrlsAsync();
            }) ?? throw new InvalidOperationException($"Value for memory cache \"{Tags.GalleryImagesCacheKey}\" was null");

            List<ImageResponse> imagesResponse = [];
            foreach (Dictionary<string, string> imageUrlByName in imageUrlMappings)
            {
                foreach ((string imageNameInAzure, string imageUrl) in imageUrlByName) // imageName:imageUrl kvp
                {
                    // name has the image's hash code so it will be unique to safely query
                    List<Image> imagesInDb = _imageRepo.List(new QueryOptions<Image>() { Includes = "HairProfile", ThenIncludes = "HairStyles, HairColors" }).ToList();

                    List<string> imageNamesInDb = imagesInDb.Select(image => image.Name).ToList();

                    // since imageName came from Azure Blob Storage, it may not be stored in the DB if uploaded through Azure & not the web app
                    if (!imageNamesInDb.Contains(imageNameInAzure))
                    {
                        continue;
                    }

                    Image imageInDb = imagesInDb.First(image => image.Name == imageNameInAzure);

                    string imageDescription = imageInDb.Description ?? string.Empty; // null if there was no description set when uploaded

                    List<HairStyleResponse> hairStyles = imageInDb.HairProfile
                                                              .HairStyles
                                                              .Select(hairstyle => new HairStyleResponse() { Style = hairstyle.Style })
                                                              .ToList();

                    List<HairColorResponse> hairColors = imageInDb.HairProfile
                                                              .HairColors
                                                              .Select(hairColor => new HairColorResponse() { Color = hairColor.Color })
                                                              .ToList();

                    ImageResponse imageResponse = new ImageResponse()
                    {
                        Url = imageUrl,
                        Description = imageDescription,
                        Hairstyles = hairStyles,
                        HairColors = hairColors
                    };

                    imagesResponse.Add(imageResponse);
                }
            }

            ImageUrlsResponse response = new ImageUrlsResponse() { Images = imagesResponse };

            return new JsonResult(response);
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