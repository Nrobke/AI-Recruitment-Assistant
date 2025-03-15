
using AI_Recruitment_Assistant.Application.Abstractions.Messaging;
using AI_Recruitment_Assistant.Domain.Shared;

namespace AI_Recruitment_Assistant.Application.Features.InterviewReminder;

public class SendReminderCommandHandler : ICommandHandler<SendReminderCommand>
{
    public Task<Result> Handle(SendReminderCommand request, CancellationToken cancellationToken)
    {
        // logic to send reminder
        throw new NotImplementedException();
    }
}
