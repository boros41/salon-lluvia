using Azure;
using Azure.Storage.Blobs.Models;

namespace Mvc.Integrations.AzureBlobStorage.Interfaces;

public interface IAzureBlobStorageImages
{
    Task<HashSet<Dictionary<string, string>>> GetImageUrlsAsync();
    Task<Response<BlobContentInfo>> PostImageAsync(string filename, IFormFile file);
}