
namespace AI_Recruitment_Assistant.Application.DTOs.Requests;

public record CalendarEventRequest(
    string CandidateEmail,
    string JobTitle,
    DateTime InterviewTime,
    int DurationMinutes
);
