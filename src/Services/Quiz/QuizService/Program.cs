using GeminiLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GeminiConfig>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddHttpClient<GeminiClient>();

var app = builder.Build();

app.MapPost("/prompt", async (PromptRequest req, GeminiClient gemini) =>
{
    var text = await gemini.GenerateAsync(req.Prompt);
    return Results.Ok(new { output = text });
});

app.Run();

public record PromptRequest(string Prompt);
