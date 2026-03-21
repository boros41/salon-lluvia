using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;
using Mvc.Utilities;

namespace Mvc.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class ClientController : Controller
{
    private readonly SalonContext _context;
    public ClientController(SalonContext ctx) => _context = ctx;

    public IActionResult List()
    {
        List<Client> clients = _context.Clients.ToList();

        if (clients.Count == 0)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Client", "No clients found", false));
        }

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
    public IActionResult Edit(int id)
    {
        Client? client = _context.Clients.Find(id);

        if (client is null)
        {
            return RedirectToAction("List");
        }

        return View(client);
    }

    [HttpPost]
    public IActionResult Edit(Client model)
    {
        Client? client = _context.Clients.Find(model.Id);

        if (client is null)
        {
            return RedirectToAction("List");
        }

        if (!ModelState.IsValid)
        {

            return View(model);
        }

        client.PhoneNumber = model.PhoneNumber;

        int propertiesEdited = _context.SaveChanges();
        if (propertiesEdited > 0)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Client Edit", $"Successfully edited {client.Name}\'s phone number", true));
            return RedirectToAction("List");
        }
        else
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Client Edit", "No changes were made", false));
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        Client? client = _context.Clients.Find(id);

        if (client is null)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Client", $"No Client with ID \"{id}\" to delete", false));
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
