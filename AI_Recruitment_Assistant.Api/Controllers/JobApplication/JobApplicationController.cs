using AI_Recruitment_Assistant.Application.Features.JobApplications;
using AI_Recruitment_Assistant.Infrastructure.Persistence;
using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AI_Recruitment_Assistant.Application.Abstractions.Authentication;
using AI_Recruitment_Assistant.Application.DTOs.EntityDtos;

namespace AI_Recruitment_Assistant.Api.Controllers.JobApplication;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationController(IMediator mediator, IUserContext userContext) : ControllerBase
{

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCV([FromForm] IFormFile cvFile, [FromForm] int jobPostingId)
    {
        var currentUser =  userContext.GetCurrentUserInfo();
        if (currentUser?.Id == null)
            return Unauthorized();

        var command = new UploadCVCommand
        {
            CVFile = cvFile,
            JobPostingId = jobPostingId,
            UserId = int.Parse(currentUser.Id)
        };

        var result = await mediator.Send(command);
        return Ok(new { ApplicationId = result });
    }
}
