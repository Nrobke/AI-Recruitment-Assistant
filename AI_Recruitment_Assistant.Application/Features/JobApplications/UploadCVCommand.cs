
using AI_Recruitment_Assistant.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace AI_Recruitment_Assistant.Application.Features.JobApplications;

public record UploadCVCommand : ICommand<int>
{
    public IFormFile CVFile { get; set; }
    public int JobPostingId { get; set; }
    public int UserId { get; set; }
}
