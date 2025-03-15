
using AI_Recruitment_Assistant.Application.Features.InterviewReminder;
using MediatR;
using Quartz;

namespace AI_Recruitment_Assistant.Infrastructure.Services.Background;

public class InterviewReminderJob(IMediator mediator) : IJob
{
    private readonly IMediator _mediator = mediator;

    public async Task Execute(IJobExecutionContext context)
        => await _mediator.Send(new SendReminderCommand());
}
