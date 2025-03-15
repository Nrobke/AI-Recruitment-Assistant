
using AI_Recruitment_Assistant.Application.DTOs.Responses;

namespace AI_Recruitment_Assistant.Application.Abstractions.Services;

public interface ITokenStorageService
{
    Task StoreTokensAsync(TokenResponse tokens);
    Task<TokenResponse> GetTokensAsync();
    Task ClearTokensAsync();
}
