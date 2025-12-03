using Entities.Models;
using Newtonsoft.Json;
using Repositories.Common.Interfaces;
using System.Text;

namespace FPTTrackingSystem.Services.Common.Gemini
{
    public class GeminiService : IGeminiService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAISettingsCache _cache;
        private readonly HttpClient _httpClient;
        private const string Model = "gemini-2.5-flash";
        private const string BaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/{0}:generateContent?key={1}";

        private Aisetting settings => _cache.Settings;

        public GeminiService(IServiceProvider serviceProvider, IAISettingsCache cache, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> AskGeminiAsync(string prompt)
        {
            if (settings == null || string.IsNullOrWhiteSpace(settings.SecretKey))
                throw new Exception("Gemini API key not configured.");

            var url = string.Format(BaseUrl, Model, settings.SecretKey);

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(payload);
            var request = new StringContent(json, Encoding.UTF8, "application/json");

            string responseText = await CallApiWithRetry(url, request);

            var response = JsonConvert.DeserializeObject<GeminiResponse>(responseText);
            return response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "";
        }

        private async Task<string> CallApiWithRetry(string url, HttpContent content)
        {
            int maxRetries = 3;

            for (int retry = 0; retry <= maxRetries; retry++)
            {
                try
                {
                    var response = await _httpClient.PostAsync(url, content);
                    var body = await response.Content.ReadAsStringAsync();

                    // ✅ Xử lý 429 TRƯỚC KHI throw exception
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        if (retry >= maxRetries)
                        {
                            throw new Exception($"Rate limit exceeded. Please try again in a few minutes. Response: {body}");
                        }

                        // Exponential backoff: 5s, 10s, 20s
                        int delaySeconds = 5 * (int)Math.Pow(2, retry);
                        Console.WriteLine($"⚠️ Rate limit (429). Waiting {delaySeconds}s before retry {retry + 1}/{maxRetries}...");
                        await System.Threading.Tasks.Task.Delay(delaySeconds * 1000);
                        continue; 
                    }

                    // ✅ Xử lý lỗi khác
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Gemini API error ({response.StatusCode}): {body}");
                    }

                    // ✅ Thành công → return
                    return body;
                }
                catch (HttpRequestException ex) // ✅ Chỉ catch network errors
                {
                    if (retry >= maxRetries)
                        throw new Exception($"Network error after {maxRetries} retries: {ex.Message}", ex);

                    Console.WriteLine($"⚠️ Network error. Retry {retry + 1}/{maxRetries}...");
                    await System.Threading.Tasks.Task.Delay(2000 * (retry + 1));
                }
                // ✅ KHÔNG catch Exception chung → để lỗi khác throw ra ngoài
            }

            throw new Exception("Max retries exceeded");
        }
    }

    public class GeminiResponse
    {
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
    }

    public class Content
    {
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        public string Text { get; set; }
    }
}