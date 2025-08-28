using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace GeminiLib;

public class GeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly GeminiConfig _config;

    public GeminiClient(HttpClient httpClient, IOptions<GeminiConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var model = _config.DefaultModel;

        var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent";

        var requestBody = new
        {
            contents = new[]
            {
                new {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = _config.Models[model]
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _config.ApiKey);

        var response = await _httpClient.PostAsync(url, content);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Gemini API error: {result}");

        using var doc = JsonDocument.Parse(result);
        var text = doc
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text ?? string.Empty;
    }
}
