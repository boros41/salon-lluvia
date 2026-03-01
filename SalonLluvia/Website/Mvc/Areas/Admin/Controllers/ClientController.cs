using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;
using Mvc.Utilities;

namespace Mvc.Areas.Admin.Controllers;

[Area("Admin")]
public class ClientController : Controller
{
    private readonly SalonContext _context;
    public ClientController(SalonContext ctx) => _context = ctx;

    public IActionResult List()
    {
        List<Client> clients = _context.Clients.ToList();

        //TODO: toast if no clients found

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

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Client? client = _context.Clients.Find(id);

        if (client is null)
        {
            // TODO: move these into Utilities.Tags method
            TempData[Tags.ToastHeader] = "Client";
            TempData[Tags.ToastMessage] = $"No Client with ID \"{id}\" to delete";
            TempData[Tags.IsSuccess] = false;

            return RedirectToAction("List");
        }

        return View(client);
    }

    [HttpPost]
    public IActionResult Delete(Client client)
    {
        Client? clientToDelete = _context.Clients.Find(client.Id);

        if (clientToDelete is null)
        {
            return RedirectToAction("List");
        }

        _context.Clients.Remove(clientToDelete);
        _context.SaveChanges();
        return RedirectToAction("List");
    }
}
