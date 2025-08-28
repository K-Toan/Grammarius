namespace GeminiLib;

public class GeminiConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = "gemini-1.5-flash";
    public Dictionary<string, GeminiModelConfig> Models { get; set; } = new();
}

public static class GeminiConfigurations
{
    public const string GeminiPro = "gemini-2.0-pro";
    public const string Gemini2Flash = "gemini-2.0-flash";
    public const string Gemini2FlashLite = "gemini-2.0-flash-lite";
}