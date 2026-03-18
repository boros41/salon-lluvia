using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Mvc.Controllers;
using Mvc.Data.Repository;
using Mvc.Integrations.Calendly;
using Mvc.Models;
using Mvc.Models.ViewModels;

namespace Mvc.Tests;

public class HomeControllerTests
{
    [Fact]
    public async Task Appointment_POST_ReturnsViewResultIfModelIsNotValid()
    {
        // arrange
        var appointmentRepo = new Mock<IRepository<Appointment>>();
        var clientRepo = new Mock<IRepository<Client>>();
        var calendlyAppointment = new Mock<ICalendlyAppointment>();
        var memoryCache = new Mock<IMemoryCache>();
        var appointmentViewModel = new AppointmentViewModel();
        var controller = new HomeController(appointmentRepo.Object, clientRepo.Object, calendlyAppointment.Object, memoryCache.Object);
        controller.ModelState.AddModelError("", "Test Error Message");

        // act
        var result = await controller.Appointment(appointmentViewModel);

        // assert
        Assert.IsType<ViewResult>(result);
    }
}