namespace Mvc.Integrations.AzureBlobStorage.Interfaces;

public interface IAzureBlobStorageImages
{
    Task<string> GetImageUrl();
}