
using System.Text.Json.Serialization;

namespace AI_Recruitment_Assistant.Application.DTOs.Responses;

public class GoogleCalendarEventResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("hangoutLink")]
    public string HangoutLink { get; set; }
}
