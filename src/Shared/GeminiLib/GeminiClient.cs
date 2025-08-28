using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeminiLib;

public class GeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model = "gemini-2.0-flash";

    public GeminiClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

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
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _apiKey);

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
