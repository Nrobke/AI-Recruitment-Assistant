
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AI_Recruitment_Assistant.Application.Abstractions.Authentication;
using AI_Recruitment_Assistant.Application.DTOs.EntityDtos;
using Microsoft.AspNetCore.Http;

namespace MerchantAppBackend.Infrastructure.Authentication;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUser? GetCurrentUserInfo()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            return null;
        }

        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return new CurrentUser(userId);
    }
}
