using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;

namespace Mvc.Areas.Admin.Controllers;

[Area("Admin")]
public class AppointmentController : Controller
{
    private readonly SalonContext _context;
    public AppointmentController(SalonContext ctx) => _context = ctx;

    [HttpGet]
    public ViewResult List()
    {
        List<Appointment> appointments = _context.Appointments.Include(a => a.Client).ToList();

        //TODO: Handle if no appointments are found

        return View(appointments);
    }

    [HttpGet]
    public IActionResult Edit(int appointmentId)
    {
        Appointment? appointment = _context.Appointments
                                           .Include(a => a.Client)
                                           .FirstOrDefault(a => a.Id == appointmentId);

        if (appointment is null)
        {
            return RedirectToAction("List");
        }

        return View(appointment);
    }

    [HttpPost]
    public IActionResult Edit(Appointment editedAppointment)
    {
        Appointment? appointment = _context.Appointments
                                           .Include(a => a.Client)
                                           .FirstOrDefault(a => a.Id == editedAppointment.Id);

        if (appointment is null) // client probably edited the id to a non-existent one
        {
            return RedirectToAction("List");
        }

        if (!ModelState.IsValid)
        {

            return View(editedAppointment);
        }

        appointment.Date = editedAppointment.Date;
        appointment.DesiredService = editedAppointment.DesiredService;

        _context.SaveChanges();

        return RedirectToAction("List");
    }
}
