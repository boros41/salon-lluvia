using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;

namespace Mvc.Areas.Admin.Controllers;

[Area("Admin")]
public class AppointmentController : Controller
{
    private readonly SalonContext _context;
    public AppointmentController(SalonContext ctx) => _context = ctx;

    public ViewResult List()
    {
        List<Appointment> appointments = _context.Appointments.Include(a => a.Client).ToList();
        return View(appointments);
    }
}
