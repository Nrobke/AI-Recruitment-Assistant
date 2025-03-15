
namespace AI_Recruitment_Assistant.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendScheduleEmailAsync(string email, DateTime interviewTime, string meetLink);
}
