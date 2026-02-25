using Microsoft.AspNetCore.Mvc;
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
        return View(clients);
    }
}
