using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Mvc.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ViewBag.Status = statusCode;
        ViewBag.Reason = ReasonPhrases.GetReasonPhrase(statusCode);

        switch (statusCode)
        {
            case 404:
                ViewBag.Message = "We’re sorry, the page you have looked for does not exist in our website! " +
                                "Maybe go to our home page or try to use a search?";
                break;
        }

        // TODO: change this view name to generic error name
        return View("NotFound");
    }
}
