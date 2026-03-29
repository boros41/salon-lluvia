using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Integrations.AzureBlobStorage.Interfaces;

namespace Mvc.Integrations.AzureBlobStorage;

public class AzureBlobStorageImages : IAzureBlobStorageImages
{
    private readonly BlobContainerClient _blobContainerClient;
    private readonly IMemoryCache _memoryCache;

    public AzureBlobStorageImages(BlobContainerClient blobContainerClient, IMemoryCache memoryCache)
    {
        _blobContainerClient = blobContainerClient;
        _memoryCache = memoryCache;
    }

    public async Task<HashSet<Dictionary<string, string>>> GetImageUrlsAsync()
    {
        AsyncPageable<BlobItem> allBlobItems = _blobContainerClient.GetBlobsAsync();

        // the filename key will be used to query the DB for the image's metadata such as description to later display in the gallery page
        HashSet<Dictionary<string, string>> blobUriMappings = [];
        await foreach (BlobItem currentBlobItem in allBlobItems)
        {
            string blobUri = _blobContainerClient.GetBlobClient(currentBlobItem.Name).Uri.ToString();
            Dictionary<string, string> blobUriByFileName = new()
            {
                {currentBlobItem.Name, blobUri}
            };

            blobUriMappings.Add(blobUriByFileName);
        }

        return blobUriMappings;
    }

    [NonAction]
    public async Task<Response<BlobContentInfo>> PostImageAsync(string filename, IFormFile file)
    {

        await using Stream imageStream = file.OpenReadStream();

        BlobUploadOptions uploadOptions = new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders() { ContentType = file.ContentType },
            Conditions = new BlobRequestConditions() { IfNoneMatch = ETag.All } // prevent overriding an existing blob
        };
        return await _blobContainerClient.GetBlobClient(filename).UploadAsync(imageStream, uploadOptions);

    }
}