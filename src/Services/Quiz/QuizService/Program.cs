using System.Text;
using System.Text.Json;
using GeminiLib;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/api/v1/quiz/generate", async (PromptRequest req, IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var apiKey = "key";
    var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    var requestBody = new
    {
        contents = new[]
        {
            new {
                parts = new[]
                {
                    new { text = req.Prompt }
                }
            }
        }
    };

    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Add("X-goog-api-key", apiKey);

    var response = await httpClient.PostAsync(url, content);
    var result = await response.Content.ReadAsStringAsync();

    return Results.Content(result, "application/json");

    if (!response.IsSuccessStatusCode)
        throw new HttpRequestException($"Gemini API error: {result}");

    using var doc = JsonDocument.Parse(result);

    var finishReason = doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("finishReason")
        .GetString();

    if(finishReason != "STOP_SEQUENCE")
        throw new Exception("The response was not completed successfully.");

    var responseContent = doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString();

    var responseId = doc.RootElement
        .GetProperty("responseId")
        .GetString();

    return Results.Ok(new PromptResponse
    {
        ResponseContent = responseContent,
        ResponseId = responseId,
    });
});

app.Run();

public record PromptRequest(string Prompt);
public record PromptResponse(string ResponseContent, string ResponseId, string FinishReason);