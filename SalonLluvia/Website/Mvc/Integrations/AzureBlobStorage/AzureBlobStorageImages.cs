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

    public async Task<HashSet<string>> GetImageUrlsAsync()
    {
        AsyncPageable<BlobItem> allBlobItems = _blobContainerClient.GetBlobsAsync();

        HashSet<string> blobUris = [];
        await foreach (BlobItem currentBlobItem in allBlobItems)
        {
            string blobUri = _blobContainerClient.GetBlobClient(currentBlobItem.Name).Uri.ToString();
            blobUris.Add(blobUri);
        }

        return blobUris;
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