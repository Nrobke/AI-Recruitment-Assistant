using AI_Recruitment_Assistant.Application.Abstractions.Services;
using AI_Recruitment_Assistant.Application.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI_Recruitment_Assistant.Api.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class OauthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenStorageService _tokenStorage;

    public OauthController(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ITokenStorageService tokenStorage)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _tokenStorage = tokenStorage;
    }

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var state = Guid.NewGuid().ToString();
        Response.Cookies.Append("oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={_configuration["ClientId"]}" +
            $"&redirect_uri={Uri.EscapeDataString(_configuration["RedirectUri"]!)}" +
            $"&response_type=code" +
            $"&scope={Uri.EscapeDataString(_configuration["Scope"]!)}" +
            $"&access_type=offline" +
            $"&state={state}" +
            "&prompt=consent";

        return Redirect(url);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            // Verify state parameter
            var originalState = Request.Cookies["oauth_state"];
            if (state != originalState)
            {
                return BadRequest("Invalid state parameter");
            }

            // Exchange authorization code for tokens
            var tokenResponse = await ExchangeCodeForTokens(code);

            // Store tokens securely
            await _tokenStorage.StoreTokensAsync(tokenResponse);

            // You can redirect to a success page or return JSON
            return Ok(new
            {
                message = "Authorization successful",
                access_token = tokenResponse.AccessToken,
                expires_in = tokenResponse.ExpiresIn
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Authorization failed: {ex.Message}");
        }
    }

    private async Task<TokenResponse> ExchangeCodeForTokens(string code)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _configuration["ClientId"]!,
            ["client_secret"] = _configuration["ClientSecret"]!,
            ["redirect_uri"] = _configuration["RedirectUri"]!,
            ["grant_type"] = "authorization_code"
        });

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(content)!;
    }
}
