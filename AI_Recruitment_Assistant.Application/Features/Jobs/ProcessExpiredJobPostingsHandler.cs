using AI_Recruitment_Assistant.Application.Abstractions.Messaging;
using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Domain.Entities;
using AI_Recruitment_Assistant.Domain.Repositories;
using AI_Recruitment_Assistant.Domain.Shared;

namespace AI_Recruitment_Assistant.Application.Features.Jobs;

internal sealed class ProcessExpiredJobPostingsHandler(
    IJobRepository jobRepository,
    IRepository repository,
    ICalendarService calendarService,
    IEmailService emailService)
    : ICommandHandler<ProcessExpiredJobPostingsCommand>
{
    private readonly IJobRepository _jobRepository = jobRepository;
    private readonly ICalendarService _calendarService = calendarService;
    private readonly IEmailService _emailService = emailService;
    private readonly IRepository _repository = repository;

    public async Task<Result> Handle(
        ProcessExpiredJobPostingsCommand request,
        CancellationToken cancellationToken)
    {
        var expiredJobs = await _jobRepository.GetExpiredJobPostingsAsync(cancellationToken);

        foreach (var job in expiredJobs)
        {
            var topApplications = job.Applications
                .Where(a => a.MatchScore >= job.MatchThreshold)
                .OrderByDescending(a => a.MatchScore)
                .Take(job.TopCandidatesCount)
                .ToList();

            var interviewTime = DateTime.UtcNow.AddDays(1).Date.AddHours(9); // Start at 9 AM next day

            foreach (var application in topApplications)
            {
                try
                {
                    var meetLink = await _calendarService.ScheduleInterviewAsync(
                        candidateEmail: application.Candidate.Email,
                        jobTitle: job.Title,
                        interviewTime: interviewTime,
                        durationMinutes: 60
                    );

                    await _repository.CreateAsync(new Interview
                    {
                        JobApplicationId = application.Id,
                        InterviewDate = interviewTime,
                        MeetingLink = meetLink,
                    }, cancellationToken);

                    await _emailService.SendScheduleEmailAsync(
                        application.Candidate.Email,
                        interviewTime,
                        meetLink
                    );

                    interviewTime = interviewTime.AddHours(1); 
                }
                catch (Exception ex)
                {
                    
                }
            }

            job.IsProcessed = true;
        }

        return Result.Success();
    }
}