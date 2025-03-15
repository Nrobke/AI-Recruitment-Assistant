
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace AI_Recruitment_Assistant.Application.Abstractions.Services;

public interface IGeminiService
{
    Task<CVParseResult> ParseCV(IFormFile cvFile);
    Task<float> CalculateMatchScore(string cvText, string jobDescription);
}
