
using AI_Recruitment_Assistant.Application.Features.Jobs;
using MediatR;
using Quartz;

namespace AI_Recruitment_Assistant.Infrastructure.Services.Background;

public class ProcessExpiredJobsJob(IMediator mediator) : IJob
{

    public async Task Execute(IJobExecutionContext context)
        => await mediator.Send(new ProcessExpiredJobPostingsCommand());
}
