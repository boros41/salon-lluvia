using Microsoft.AspNetCore.Mvc;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.ViewModels;
using Mvc.Utilities;
using System.Diagnostics;

namespace Mvc.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly SalonContext _context;
    private readonly CalendlyClient _calendly;

    public HomeController(SalonContext ctx, CalendlyClient calendly)
    {
        _context = ctx;
        _calendly = calendly;
    }

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Appointment()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Appointment(AppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Client? client = _context.Clients.FirstOrDefault(c => c.PhoneNumber == model.PhoneNumber);
        if (client is null)
        {
            client = new Client()
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };

            // must save to increment client's id before appointment can relate to it
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        Appointment appointment = new Appointment()
        {
            ClientId = client.Id,
            Client = client,
            Date = model.Date,
            DesiredService = model.DesiredService
        };

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", "Thank you for your appointment! We will reach out to you soon to confirm.", true));

        return RedirectToAction("Appointment");
    }
    public IActionResult Service()
    {
        return View();
    }
    public IActionResult Pricing()
    {
        return View();
    }
    public IActionResult Gallery()
    {
        return View();
    }
    public IActionResult Team()
    {
        return View();
    }
    public IActionResult Testimonial()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
