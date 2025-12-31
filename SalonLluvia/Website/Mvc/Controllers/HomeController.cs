using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mvc.Models;

namespace Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "About Us";
            ViewBag.Action = "About"; 
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Appointment()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Appointment";
            ViewBag.Action = "Appointment";
            return View();
        }
        public IActionResult Service()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Services";
            ViewBag.Action = "Service";
            return View();
        }
        public IActionResult Pricing()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Prices";
            ViewBag.Action = "Pricing";
            return View();
        }
        public IActionResult Gallery()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Gallery";
            ViewBag.Action = "Gallery";
            return View();
        }
        public IActionResult Team()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Meet the Team";
            ViewBag.Action = "Team";
            return View();
        }
        public IActionResult Testimonial()
        {
            // used in _breadcrumbPartial
            ViewBag.Header = "Testimonial";
            ViewBag.Action = "Testimonial";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
