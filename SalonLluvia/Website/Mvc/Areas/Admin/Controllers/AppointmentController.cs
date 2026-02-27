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

        //TODO: Handle if no appoinments are found

        return View(appointments);
    }

    [HttpGet]
    public ViewResult Edit(int id)
    {
        Appointment? appointment = _context.Appointments
                                          .Include(a => a.Client)
                                          .FirstOrDefault(a => a.Id == id);

        return View(appointment);
    }

    [HttpPost]
    public ViewResult Edit(Appointment editedAppointment)
    {
        _context.Appointments.Update(editedAppointment);
        _context.SaveChanges();

        return View(editedAppointment);
    }
}
