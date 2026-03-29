using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mvc.Utilities;

public static class Tags
{
    public const string BusinessName = "salonlluvia";
    public const int SHA256HashHexLength = 64;
    public const string ImagePurpose = "gallery";
    public const string ImageVariant = "original"; // og image uploaded or processed .webp image, etc
    public const int GalleryImageNameLength = 93; // {imageHash}-{Tags.BusinessName}-{purpose}-{variant}
    public const string GalleryImagesCacheKey = "gallery-image-urls";
    public const string AvailableDaysCacheKey = "available-days";

    public record ToastValues(string Header, string Message, bool IsSuccess);

    public static void ToastMessage(ITempDataDictionary tempData, ToastValues values)
    {
        tempData["toast-header"] = values.Header;
        tempData["toast-message"] = values.Message;
        tempData["is-success"] = values.IsSuccess;
    }
}
