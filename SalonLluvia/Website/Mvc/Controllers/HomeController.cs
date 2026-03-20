using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mvc.Data.Repository;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.ViewModels;
using Mvc.Utilities;
using System.Diagnostics;

namespace Mvc.Controllers;

[Route("[action]")]
public class HomeController : Controller
{
    private readonly IRepository<Appointment> _appointmentRepo;
    private readonly IRepository<Client> _clientRepo;
    private readonly ICalendlyAppointment _calendlyAppointment;
    private readonly IMemoryCache _memoryCache;

    public HomeController(IRepository<Appointment> appointmentRepo, IRepository<Client> clientRepo, ICalendlyAppointment appointment, IMemoryCache memoryCache)
    {
        _appointmentRepo = appointmentRepo;
        _clientRepo = clientRepo;
        _calendlyAppointment = appointment;
        _memoryCache = memoryCache;
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
    public async Task<IActionResult> Appointment(AppointmentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_memoryCache.TryGetValue("available-days", out HashSet<string>? availableDays))
        {
            if (availableDays is null)
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: \"cache was empty\"", false));

                return RedirectToAction("Appointment");
            }

            DateTime date = (DateTime)model.Date!; // model.Date has a [Required] attribute which requires a nullable for value types
            if (!availableDays.Contains(date.ToString("yyyy-MM-dd")))
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", "The date you tried to reserve is no longer available, please try again.", false));

                _memoryCache.Remove("available-days");

                return RedirectToAction("Appointment");
            }
        }
        else
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: \"cache key did not exist\"", false));

            return RedirectToAction("Appointment");
        }

        Client? client = _clientRepo.List(new QueryOptions<Client>())
                                    .FirstOrDefault(c => c.PhoneNumber == model.PhoneNumber);
        if (client is null)
        {
            client = new Client()
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };

            // must save to increment client's id before appointment can relate to it
            _clientRepo.Insert(client);
            _clientRepo.Save();
        }

        Appointment appointment = new Appointment()
        {
            ClientId = client.Id,
            Client = client,
            Date = model.Date,
            DesiredService = model.DesiredService
        };

        try
        {
            await _calendlyAppointment.CreateAppointment(model);
            _memoryCache.Remove("available-days"); // the date the user just booked is no longer available
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode is null)
            {
                Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support with this message: {e.Message}", false));

                return RedirectToAction("Appointment");
            }

            int statusCode = (int)e.StatusCode.Value;
            string message;

            switch (statusCode)
            {
                case StatusCodes.Status403Forbidden:
                    // Access to the "/invitees" endpoint is limited to Calendly users on paid plans (Standard and above). Users on the Free plan will receive a 403 Forbidden response.
                    message = $"Unfortunately, the booking service rejected and did not create the appointment. If this continues, please contact support with this code: {statusCode}";
                    break;
                default:
                    message = $"Unfortunately, an error occured when trying to book your appointment. If this continues, please contact support with this code: {statusCode}";
                    break;
            }

            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", message, false));

            return RedirectToAction("Appointment");
        }
        catch (Exception e)
        {
            Tags.ToastMessage(TempData, new Tags.ToastValues("Appointment", $"Unfortunately, An error occured when trying to book your appointment. If this continues, please contact support.", false));

            return RedirectToAction("Appointment");
        }

        _appointmentRepo.Insert(appointment);
        _appointmentRepo.Save();

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