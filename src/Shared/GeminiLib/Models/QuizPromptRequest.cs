namespace GeminiLib.Models;

public class QuizPromptRequest
{
    public string Question { get; set; } = string.Empty;
    public string[] Options { get; set; } = Array.Empty<string>();
}
