using Azure;
using Azure.Storage.Blobs.Models;

namespace Mvc.Integrations.AzureBlobStorage.Interfaces;

public interface IAzureBlobStorageImages
{
    Task<string> GetImageUrl();
    Task<Response<BlobContentInfo>> PostImageAsync(string filename, IFormFile file);
}