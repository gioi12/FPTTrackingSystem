namespace FPTTrackingSystem.Services.Common.Gemini
{
    public interface IGeminiService
    {
        Task<string> AskGeminiAsync(string prompt);
    }
}
