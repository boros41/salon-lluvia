namespace Mvc.Integrations.Calendly;

public class CalendlyAvailableDays : CalendlyClient
{
    public CalendlyAvailableDays(IConfiguration config) : base(config) { }

    public async Task<HashSet<string>> Get()
    {
        string userUri = await GetUserUri();
        string eventTypeUri = await GetUserEventTypeUri(userUri);
        HashSet<string> availableDays = await GetEventTypeAvailableTimes(eventTypeUri);

        return availableDays;
    }
}