using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mvc.Utilities;

public static class Tags
{
    public record ToastValues(string Header, string Message, bool IsSuccess);

    public static void ToastMessage(ITempDataDictionary tempData, ToastValues values)
    {
        tempData["toast-header"] = values.Header;
        tempData["toast-message"] = values.Message;
        tempData["is-success"] = values.IsSuccess;
    }
}
