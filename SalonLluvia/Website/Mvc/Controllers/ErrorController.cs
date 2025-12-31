using Microsoft.AspNetCore.Mvc;

namespace Mvc.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            ViewBag.Status = statusCode;

            switch (statusCode)
            {
                case 404:
                    ViewBag.Error = "Not Found";
                    ViewBag.Message = "We’re sorry, the page you have looked for does not exist in our website! " +
                                    "Maybe go to our home page or try to use a search?";
                    break;
            }

            return View("NotFound");
        }
    }
}
