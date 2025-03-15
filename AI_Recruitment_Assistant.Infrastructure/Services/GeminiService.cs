using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization; // Added for JsonPropertyName

namespace AI_Recruitment_Assistant.Infrastructure.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<GeminiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("GeminiClient");
        _config = config;
        _logger = logger;
    }

    public async Task<CVParseResult> ParseCV(IFormFile cvFile)
    {
        try
        {
            var apiKey = _config["Gemini:ApiKey"];
            var modelId = _config["Gemini:ModelId"];

            await using var stream = new MemoryStream();
            await cvFile.CopyToAsync(stream);
            var base64Content = Convert.ToBase64String(stream.ToArray());

            var request = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                inlineData = new // changed to inlineData
                                {
                                    mimeType = cvFile.ContentType,
                                    data = base64Content // changed to data
                                }
                            },
                            new
                            {
                                text = """
                                Analyze this resume and return JSON with:
                                {
                                    "skills": ["skill1", "skill2"],
                                    "education": "Highest Degree",
                                    "experience": "Work Experience Summary",
                                    "summary": "Professional Summary"
                                }
                                Use English only.
                                """
                            }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"models/{modelId}:generateContent?key={apiKey}",
                request
            );

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            return ParseResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini API error");
            throw;
        }
    }

    public async Task<float> CalculateMatchScore(string cvText, string jobDescription)
    {
        try
        {
            var apiKey = _config["Gemini:ApiKey"];
            var modelId = _config["Gemini:ModelId"];

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $$"""
                                Calculate match score (0-1) between CV and Job Description.
                                CV: {{cvText}}
                                Job Description: {{jobDescription}}
                                Return JSON format: { "score": 0.85 }
                                """
                            }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"models/{modelId}:generateContent?key={apiKey}",
                requestBody
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            return ParseScoreResponse(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Match score calculation failed");
            throw;
        }
    }

    private static CVParseResult ParseResponse(GeminiResponse response)
    {
        try
        {
            var json = response.Candidates[0].Content.Parts[0].Text;

            // Validate JSON structure first
            if (string.IsNullOrWhiteSpace(json))
                throw new ApplicationException("Empty response from Gemini API");

            // Handle potential markdown code blocks
            json = json.Trim()
                      .Replace("```json", "")
                      .Replace("```", "")
                      .Trim();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var result = JsonSerializer.Deserialize<CVParseResult>(json, options)
                ?? throw new ApplicationException("Failed to deserialize CV parse result");

            // Validate required fields
            if (result.Skills == null || result.Education == null)
                throw new ApplicationException("Invalid CV parse result structure");

            return result;
        }
        catch (JsonException ex)
        {
            throw new ApplicationException($"Invalid JSON format: {ex.Message}");
        }
        catch (Exception ex) when (
            ex is ApplicationException ||
            ex is ArgumentNullException ||
            ex is NotSupportedException)
        {
            throw new ApplicationException($"CV parsing failed: {ex.Message}");
        }
    }

    private static float ParseScoreResponse(GeminiResponse response)
    {
        try
        {
            var json = response.Candidates[0].Content.Parts[0].Text;

            // Clean the JSON response
            var sanitizedJson = json.Trim()
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // Handle possible JSONL or multiple JSON objects
            if (sanitizedJson.Contains('\n'))
            {
                sanitizedJson = sanitizedJson.Split('\n').FirstOrDefault() ?? string.Empty;
            }

            // Validate JSON structure
            if (string.IsNullOrWhiteSpace(sanitizedJson))
            {
                throw new ApplicationException("Empty score response from Gemini API");
            }

            using var doc = JsonDocument.Parse(sanitizedJson, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            // Handle different response formats
            if (doc.RootElement.TryGetProperty("score", out var scoreElement))
            {
                return scoreElement.GetSingle();
            }

            if (doc.RootElement.TryGetProperty("match_score", out scoreElement))
            {
                return scoreElement.GetSingle();
            }

            throw new ApplicationException("Score property not found in response");
        }
        catch (JsonException ex)
        {
            throw new ApplicationException($"Invalid JSON format: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            throw new ApplicationException($"Missing required property: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to parse match score: {ex.Message}");
        }
    }

    public static void ConfigureHttpClient(IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("GeminiClient", client =>
        {
            client.BaseAddress = new Uri(config["Gemini:BaseUrl"]!);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });
    }

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();

        public class Candidate
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; } = new();
        }

        public class Content
        {
            [JsonPropertyName("parts")]
            public List<Part> Parts { get; set; } = new();
        }

        public class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }
    }
}