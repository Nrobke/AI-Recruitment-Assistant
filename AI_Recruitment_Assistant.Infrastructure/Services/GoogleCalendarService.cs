
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Application.DTOs.Requests;
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AI_Recruitment_Assistant.Infrastructure.Services;

public class GoogleCalendarService : ICalendarService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenStorageService _tokenStorage;
    private readonly IConfiguration _config;
    private readonly ILogger<GoogleCalendarService> _logger;
    public GoogleCalendarService(
      IHttpClientFactory httpClientFactory,
      ITokenStorageService tokenStorage,
      IConfiguration config,
      ILogger<GoogleCalendarService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenStorage = tokenStorage;
        _config = config;
        _logger = logger;
    }
    public async Task<string> ScheduleInterviewAsync(string candidateEmail, string jobTitle, DateTime interviewTime, int durationMinutes)
    {
        try
        {
            // Get access token for current user
            var tokens = await _tokenStorage.GetTokensAsync();
            if (tokens == null) throw new InvalidOperationException("User not authenticated");

            // Create Google Calendar event
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var eventId = await CreateCalendarEvent(client, new CalendarEventRequest(
                candidateEmail,
                jobTitle,
                interviewTime,
                durationMinutes
            ));

            return eventId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule interview");
            throw;
        }
    }

    private static async Task<string> CreateCalendarEvent(
        HttpClient client,
        CalendarEventRequest request)
    {
        var response = await client.PostAsJsonAsync(
            "https://www.googleapis.com/calendar/v3/calendars/primary/events",
            new
            {
                summary = $"Interview for {request.JobTitle}",
                description = "Scheduled by AI Recruitment Assistant",
                start = new { dateTime = request.InterviewTime.ToString("o") },
                end = new { dateTime = request.InterviewTime.AddMinutes(request.DurationMinutes).ToString("o") },
                attendees = new[] { new { email = request.CandidateEmail } },
                conferenceData = new
                {
                    createRequest = new
                    {
                        requestId = Guid.NewGuid().ToString(),
                        conferenceSolutionKey = new { type = "hangoutsMeet" }
                    }
                }
            });

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<GoogleCalendarEventResponse>();
        return content.Id;
    }
}
