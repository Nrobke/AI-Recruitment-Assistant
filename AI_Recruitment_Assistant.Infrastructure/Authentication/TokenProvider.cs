
using System.Data;
using System.Security.Claims;
using System.Text;
using AI_Recruitment_Assistant.Application.Abstractions.Authentication;
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using AI_Recruitment_Assistant.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CustomerAppDashboard.Infrastructure.Authentication;

public sealed class TokenProvider(IConfiguration _configuration) : ITokenProvider
{
    public async Task<CustomAccessTokenResponse> Create(User user, IList<string> roles)
    {
        string secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return new CustomAccessTokenResponse
        {
            AccessToken = token,
            ExpiresAt = expiration,
        };
    }
}
