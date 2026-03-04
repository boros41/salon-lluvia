using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mvc.Utilities;

public static class Tags
{
    public record ToastValues(string Header, string Message, bool IsSuccess);

    public const string UsPhoneRegEx = @"^(\+?1[-. ]?)?\(?([2-9][0-8]\d)\)?[-. ]?([2-9][0-9]{2})[-. ]?([0-9]{4})$";

    public static void ToastMessage(ITempDataDictionary tempData, ToastValues values)
    {
        tempData["toast-header"] = values.Header;
        tempData["toast-message"] = values.Message;
        tempData["is-success"] = values.IsSuccess;
    }
}
