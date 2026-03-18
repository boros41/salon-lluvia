using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Mvc.Controllers;
using Mvc.Data.Repository;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.ViewModels;
using System.Net;

namespace Mvc.Tests;

public class HomeControllerTests
{
    private readonly Mock<IRepository<Appointment>> _appointmentRepo = new();
    private readonly Mock<IRepository<Client>> _clientRepo = new();
    private readonly Mock<ICalendlyAppointment> _calendlyAppointment = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<ITempDataDictionary> _tempData = new();

    #region Appointment_POST
    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfModelIsValid()
    {
        // arrange
        _clientRepo.Setup(m => m.List(It.IsAny<QueryOptions<Client>>()))
                   .Returns(new List<Client>()
                   {
                       new Client() { PhoneNumber = "213-373-4253" } // simulate user in DbSet<Client>
                   });

        _memoryCache.Set("available-days", new HashSet<string>() { "2026-03-18" }); // simulates cache created in CalendlyController.Get()

        var appointmentViewModel = new AppointmentViewModel()
        {
            Name = "boros41",
            PhoneNumber = "213-373-4253",
            Date = new DateTime(2026, 3, 18),
            Email = "testemail@gmail.com",
            DesiredService = "haircut"
        };

        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsViewResultIfModelIsNotValid()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel();
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache);
        controller.ModelState.AddModelError("", "Test Error Message");

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfMemoryCacheKeyDNE()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel();
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfMemoryCacheValueIsNull()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel();
        _memoryCache.Set<HashSet<string>?>("available-days", null);
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfDateIsNotInMemoryCache()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel()
        {
            Date = new DateTime(2026, 02, 20)
        };
        _memoryCache.Set("available-days", new HashSet<string>() { "2026-03-18" });
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfHttpRequestExceptionWithNoStatusCode()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel()
        {
            Date = new DateTime(2026, 3, 18)
        };
        _memoryCache.Set("available-days", new HashSet<string>() { "2026-03-18" });
        _calendlyAppointment.Setup(m => m.CreateAppointment(It.IsAny<AppointmentViewModel>()))
                            .ThrowsAsync(new HttpRequestException("Thrown from HomeControllerTests test class."));
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfHttpRequestExceptionWithStatusCode()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel()
        {
            Date = new DateTime(2026, 3, 18)
        };
        _memoryCache.Set("available-days", new HashSet<string>() { "2026-03-18" });
        _calendlyAppointment.Setup(m => m.CreateAppointment(It.IsAny<AppointmentViewModel>()))
                            .ThrowsAsync(new HttpRequestException(message: "Thrown from HomeControllerTests test class", inner: null, HttpStatusCode.InternalServerError));
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }

    [Fact]
    public async Task Appointment_POST_ReturnsRedirectToActionResultIfException()
    {
        // arrange
        var appointmentViewModel = new AppointmentViewModel()
        {
            Date = new DateTime(2026, 3, 18)
        };
        _memoryCache.Set("available-days", new HashSet<string>() { "2026-03-18" });
        _calendlyAppointment.Setup(m => m.CreateAppointment(It.IsAny<AppointmentViewModel>()))
                            .ThrowsAsync(new Exception("Thrown from HomeControllerTests test class."));
        var controller = new HomeController(_appointmentRepo.Object, _clientRepo.Object, _calendlyAppointment.Object, _memoryCache)
        {
            TempData = _tempData.Object
        };

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<RedirectToActionResult>(result);
    }
    #endregion
}