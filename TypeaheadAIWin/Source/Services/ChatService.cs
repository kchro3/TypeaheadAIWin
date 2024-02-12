using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeaheadAIWin.Source.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public event EventHandler<ChatResponse> OnChatResponseReceived;

        public ChatService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializerOptions = new JsonSerializerOptions
            {
                Converters = { new ChatMessageJsonConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };
        }

        public async Task StreamChatAsync(ChatRequest chatRequest, CancellationToken cancellationToken)
        {
            using var response = _httpClient.PostAsStreamAsync(
                AppConfig.GetApiBaseUrl() + "/v5/wstream",
                chatRequest,
                _serializerOptions,
                cancellationToken
            );

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    var chatResponse = JsonSerializer.Deserialize<ChatResponse>(line, _serializerOptions);
                    if (chatResponse != null)
                    {
                        OnChatResponseReceived?.Invoke(this, chatResponse);
                    }
                }
            }
        }
    }
}
