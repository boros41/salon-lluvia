using Mvc.Dto.Calendly.AvailableTimes;
using Mvc.Dto.Calendly.CreateAppointment;
using Mvc.Dto.Calendly.EventType;
using Mvc.Dto.Calendly.User;
using Mvc.Models.ViewModels;
using System.Net.Http.Headers;
using System.Text.Json;
using JsonException = Newtonsoft.Json.JsonException;

namespace Mvc.Integrations.Calendly;

public sealed class CalendlyClient
{
    private readonly string _token;

    public CalendlyClient(IConfiguration config)
    {
        _token = config["Calendly:PAT"] ?? throw new InvalidOperationException("Missing Calendly:PAT secret");
    }

    public async Task<HashSet<string>> GetAvailableDays()
    {
        string userUri = await GetUserUri();
        string eventTypeUri = await GetUserEventTypeUri(userUri);
        HashSet<string> availableDays = await GetEventTypeAvailableTimes(eventTypeUri);

        return availableDays;
    }

    private async Task<string> GetUserUri()
    {
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api.calendly.com/users/me"),
            Headers =
            {
                {
                    "Authorization",
                    "Bearer " + _token
                },
            },
        };

        using HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        UserResponse userResponse = JsonSerializer.Deserialize<UserResponse>(body) ?? throw new JsonException("Failed to deserialize Calendly user.");

        return userResponse.Resource.Uri;
    }

    private async Task<string> GetUserEventTypeUri(string userUri)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api.calendly.com/event_types?user=" + Uri.EscapeDataString(userUri)),
            Headers =
            {
                {
                    "Authorization",
                    "Bearer " + _token
                },
            },
        };
        using HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        EventTypeResponse eventTypeResponse = JsonSerializer.Deserialize<EventTypeResponse>(body) ??
                                      throw new JsonException("Failed to deserialize Calendly user's event types.");

        // Calendly account is configured to have only one event type
        return eventTypeResponse.Collection[0].Uri;
    }

    private async Task<HashSet<string>> GetEventTypeAvailableTimes(string eventTypeUri)
    {
        // Will not contain weekends nor any day that already has at least one appointment. Calendly account is configured that way.
        HashSet<string> availableDays = [];

        DateTime start = DateTime.UtcNow.AddDays(1);
        DateTime end = start.AddDays(7);

        await FillDays(start, end);

        return availableDays;

        async Task FillDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException($"Start date \"{startDate}\" must be before end date \"{endDate}\"");
            }

            TimeSpan duration = endDate - startDate;

            if (duration.Days != 7)
            {
                throw new ArgumentException($"Date range can be no greater than 1 week (7 days). \"{endDate}\" - \"{startDate}\" == {duration.Days}");
            }

            DateTime today = DateTime.UtcNow;
            TimeSpan twoMonths = startDate - today;

            // BASE CASE: Can only book up to two months in advance. Really, 9 weeks: 7 * 9 = 63 days
            if (twoMonths.Days >= 60)
            {
                return;
            }

            string startTime = startDate.ToString("O");
            string endTime = endDate.ToString("O");

            string requestUri = $"https://api.calendly.com/event_type_available_times?event_type={Uri.EscapeDataString(eventTypeUri)}&start_time={Uri.EscapeDataString(startTime)}&end_time={Uri.EscapeDataString(endTime)}";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(requestUri),
                Headers =
                {
                    {
                        "Authorization",
                        "Bearer " + _token
                    },
                },
            };
            using HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();

            AvailableTimesResponse availableTimesResponse = JsonSerializer.Deserialize<AvailableTimesResponse>(body) ??
                                                            throw new JsonException("Failed to deserialize Calendly event type available times.");

            foreach (AvailableTime availableTime in availableTimesResponse.Collection)
            {
                // Since a day is considered available if it has 0 bookings, we don't care about the time part, hence the hashset for no duplicates
                const string dateFormat = "yyyy-MM-dd";
                availableDays.Add(availableTime.StartTime.ToString(dateFormat));
            }

            // Recursive step towards base case, Calendly API only allows date ranges of 1 week at a time
            await FillDays(startDate: endDate, endDate: endDate.AddDays(7));
        }
    }

    public async Task<string> CreateAppointment(AppointmentViewModel model)
    {
        DateTime date = (DateTime)model.Date!; // model.Date has a [Required] attribute which requires a nullable for value types
        CreateAppointmentRequest appointmentRequest = new CreateAppointmentRequest
        {
            EventTypeUri = await GetUserEventTypeUri(await GetUserUri()),
            StartTime = new DateTime(date.Year, date.Month, date.Day, 16, 0, 0, DateTimeKind.Utc),
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

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.calendly.com/invitees"),
            Headers =
            {
                {
                    "Authorization",
                    "Bearer " + _token
                }
            },
            Content = new StringContent(jsonContent)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            }
        };
        using HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        return body;
    }
}