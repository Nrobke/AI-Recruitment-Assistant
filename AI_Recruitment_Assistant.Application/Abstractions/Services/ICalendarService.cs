
namespace AI_Recruitment_Assistant.Application.Abstractions.Services;

public interface ICalendarService
{
    Task<string> ScheduleInterviewAsync(
       string candidateEmail,
       string jobTitle,
       DateTime interviewTime,
       int durationMinutes);
}
