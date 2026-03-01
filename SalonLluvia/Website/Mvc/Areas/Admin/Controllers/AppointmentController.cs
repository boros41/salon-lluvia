using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mvc.Models;
using Mvc.Utilities;

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

        if (appointments.Count == 0)
        {
            TempData[Tags.ToastHeader] = "Appointment";
            TempData[Tags.ToastMessage] = "No appointments found";
            TempData[Tags.IsSuccess] = false;
        }

        return View(appointments);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        int appointmentId = id;
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

        TempData[Tags.ToastHeader] = "Appointment Edit";

        appointment.Date = editedAppointment.Date;
        appointment.DesiredService = editedAppointment.DesiredService;

        int propertiesEdited = _context.SaveChanges();
        if (propertiesEdited > 0)
        {
            TempData[Tags.ToastMessage] = $"Successfully edited {appointment.Client.Name}\'s appointment";
            TempData[Tags.IsSuccess] = true;
            return RedirectToAction("List");
        }
        else
        {
            TempData[Tags.ToastMessage] = "No changes were made";
            TempData[Tags.IsSuccess] = false;
            return View(editedAppointment);
        }
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        int appointmentId = id;
        Appointment? appointment = _context.Appointments
                                           .Include(a => a.Client)
                                           .FirstOrDefault(a => a.Id == appointmentId);

        if (appointment is null)
        {
            RedirectToAction("List");
        }

        return View(appointment);
    }

    [HttpPost]
    public IActionResult Delete(Appointment appointment)
    {
        Appointment? appointmentToDelete = _context.Appointments.Find(appointment.Id);

        if (appointmentToDelete is null)
        {
            return RedirectToAction("List");
        }

        _context.Appointments.Remove(appointmentToDelete);
        _context.SaveChanges();
        return RedirectToAction("List");
    }
}
