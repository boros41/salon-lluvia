using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using Mvc.Models.ViewModels;
using System.Diagnostics;

namespace Mvc.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly SalonContext _context;
    public HomeController(SalonContext ctx) => _context = ctx;

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

        // TODO: refactor & dont create new clients for the same person, maybe unique constraint phone number
        Client client = new Client()
        {
            Name = model.Name,
            PhoneNumber = model.PhoneNumber
        };

        // must save to increment client's id before appointment can relate to it
        _context.Clients.Add(client);
        _context.SaveChanges();

        Appointment appointment = new Appointment()
        {
            ClientId = client.Id,
            Client = client,
            Date = model.Date,
            DesiredService = model.DesiredService
        };

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        return View();
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
