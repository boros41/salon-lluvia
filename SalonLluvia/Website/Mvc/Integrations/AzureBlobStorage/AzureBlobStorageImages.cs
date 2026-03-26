using Mvc.Integrations.AzureBlobStorage.Interfaces;

namespace Mvc.Integrations.AzureBlobStorage;

public class AzureBlobStorageImages : IAzureBlobStorageImages
{
    public Task<string> GetImageUrl()
    {
        return Task.Run(() => "test from server");
    }
}