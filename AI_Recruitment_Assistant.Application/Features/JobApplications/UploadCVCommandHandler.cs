
using AI_Recruitment_Assistant.Application.Abstractions.Messaging;
using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Domain.Constants;
using AI_Recruitment_Assistant.Domain.Entities;
using AI_Recruitment_Assistant.Domain.Repositories;
using AI_Recruitment_Assistant.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace AI_Recruitment_Assistant.Application.Features.JobApplications;

public class UploadCVCommandHandler(
    IGeminiService geminiService, 
    IRepository repository,
     IFileStorageService fileStorage) : ICommandHandler<UploadCVCommand, int>
{

    public async Task<Result<int>> Handle(UploadCVCommand request, CancellationToken cancellationToken)
    {
        var parsedData = await geminiService.ParseCV(request.CVFile);

        var job = await repository.GetByIdAsync<JobPosting>(request.JobPostingId, cancellationToken);

        var score = await geminiService.CalculateMatchScore(
            parsedData.Summary,
            job.Description
        );

        var application = new JobApplication
        {
            CandidateId = request.UserId,
            JobPostingId = request.JobPostingId,
            CvFilePath = await fileStorage.SaveFile(request.CVFile),
            ParsedSkills = string.Join(",", parsedData.Skills),
            MatchScore = score,
            AppliedDate = DateTime.UtcNow,
            StatusId = ApplicationConstants.ApplicationStatusTypePending 
        };

        await repository.CreateAsync(application, cancellationToken);

        return application.Id;
    }
}
