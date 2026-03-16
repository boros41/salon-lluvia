using Mvc.Dto.Calendly.CreateAppointment;
using Mvc.Models.ViewModels;
using System.Text.Json;

namespace Mvc.Integrations.Calendly;

public class CalendlyAppointment : CalendlyClient
{
    public CalendlyAppointment(IConfiguration config) : base(config) { }

    public async Task Create(AppointmentViewModel model)
    {
        DateTime date = (DateTime)model.Date!; // model.Date has a [Required] attribute which requires a nullable for value types
        CreateAppointmentRequest appointmentRequest = new CreateAppointmentRequest
        {
            EventTypeUri = await GetUserEventTypeUri(await GetUserUri()),
            StartTime = new DateTime(date.Year, date.Month, date.Day, 16, 0, 0, DateTimeKind.Utc), // hour is within Calendly user's working hours
            Invitee = new Invitee { Name = model.Name, Email = model.Email },
            QuestionsAndAnswers = new List<QuestionsAndAnswers>()
            {
                {new QuestionsAndAnswers() {Question = "Teléfono", Answer = model.PhoneNumber, Position = 0}},
                {new QuestionsAndAnswers()
                {
                    Question = "Cuéntenos algo que nos ayude a prepararnos para la reunión.", Answer = model.DesiredService, Position = 1
                }}
            }
        };

        string jsonContent = JsonSerializer.Serialize(appointmentRequest);

        await CreateAppointment(jsonContent);
    }
}