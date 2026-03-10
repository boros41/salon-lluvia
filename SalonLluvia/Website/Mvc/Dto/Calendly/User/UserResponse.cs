using System.Text.Json.Serialization;

namespace Mvc.Dto.Calendly.User;

internal sealed record UserResponse
{
    [JsonPropertyName("resource")]
    public required UserResource Resource { get; init; }
}

internal sealed record UserResource
{
    [JsonPropertyName("uri")]
    public required string Uri { get; init; }
}