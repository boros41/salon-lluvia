using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;

namespace Mvc.Areas.Admin.Controllers;

[Area("Admin")]
public class ClientController : Controller
{
    private readonly SalonContext _context;
    public ClientController(SalonContext ctx) => _context = ctx;

    public IActionResult List()
    {
        List<Client> clients = _context.Clients.ToList();

        //TODO: handle if no clients are found

        return View(clients);
    }

    [HttpGet]
    public ViewResult Appointments(int id)
    {
        // TODO: download ReSharper to format lambdas
        List<Appointment> appointments = _context.Appointments.Where(a => a.ClientId == id).Include(a => a.Client).ToList();

        if (appointments.Count != 0)
        {
            string? clientName = _context.Clients.Find(id)?.Name ?? "Deleted client";

            ViewBag.Header = $"{clientName}\'s Appointments ({appointments.Count} Found)";
        }
        else
        {
            ViewBag.Header = $"No appointments found for client with ID: \"{id}\"";
        }

        return View(appointments);
    }
}
