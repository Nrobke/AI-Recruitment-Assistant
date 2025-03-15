
using AI_Recruitment_Assistant.Application.DTOs.EntityDtos;

namespace AI_Recruitment_Assistant.Application.Abstractions.Authentication;

public interface IUserContext
{
    CurrentUser? GetCurrentUserInfo();
}
