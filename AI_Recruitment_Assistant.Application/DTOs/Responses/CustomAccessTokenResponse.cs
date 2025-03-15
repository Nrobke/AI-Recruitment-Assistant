
namespace AI_Recruitment_Assistant.Application.DTOs.Responses;

public sealed class CustomAccessTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
