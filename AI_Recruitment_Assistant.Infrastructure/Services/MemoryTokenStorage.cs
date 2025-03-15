using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using Microsoft.Extensions.Caching.Memory;

namespace AI_Recruitment_Assistant.Infrastructure.Services;

public class MemoryTokenStorage : ITokenStorageService
{
    private const string CacheKey = "calendar_t";
    private readonly IMemoryCache _cache;

    public MemoryTokenStorage(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task StoreTokensAsync(TokenResponse tokens)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokens.ExpiresIn - 60)
        };

        _cache.Set(CacheKey, tokens, cacheOptions);
        return Task.CompletedTask;
    }

    public Task<TokenResponse?> GetTokensAsync()
    {
        _cache.TryGetValue<TokenResponse>(CacheKey, out var tokens);
        return Task.FromResult(tokens);
    }

    public Task ClearTokensAsync()
    {
        _cache.Remove(CacheKey);
        return Task.CompletedTask;
    }
}