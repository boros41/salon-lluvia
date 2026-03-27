using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mvc.Integrations.AzureBlobStorage.Interfaces;

namespace Mvc.Integrations.AzureBlobStorage;

public class AzureBlobStorageImages : IAzureBlobStorageImages
{
    private readonly BlobContainerClient _blobContainerClient;
    public AzureBlobStorageImages(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }

    public Task<string> GetImageUrl()
    {
        return Task.Run(() => "test from server");
    }

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