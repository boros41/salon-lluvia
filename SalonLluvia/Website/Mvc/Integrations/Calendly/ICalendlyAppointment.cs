using Mvc.Models.ViewModels;

namespace Mvc.Integrations.Calendly;

public interface ICalendlyAppointment
{
    Task CreateAppointment(AppointmentViewModel model); // https://developer.calendly.com/api-docs/p3ghrxrwbl8kqe-create-event-invitee
}