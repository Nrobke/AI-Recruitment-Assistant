
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using AI_Recruitment_Assistant.Domain.Entities;

namespace AI_Recruitment_Assistant.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    Task<CustomAccessTokenResponse> Create(User user, IList<string> roles);
}
