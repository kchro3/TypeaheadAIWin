using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TypeaheadAIWin.Source.Service
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly Supabase.Client _supabaseClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public event EventHandler<ChatResponse> OnChatResponseReceived;

        public ChatService(HttpClient httpClient, Supabase.Client supabaseClient) {
            _httpClient = httpClient;
            _supabaseClient = supabaseClient;

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
            var authorizationToken = _supabaseClient.Auth.CurrentSession?.AccessToken ?? throw new InvalidOperationException("User is not authenticated");
            using var response = _httpClient.PostAsStreamAsync(
                AppConfig.GetApiBaseUrl() + "/v5/wstream",
                chatRequest,
                authorizationToken,
                _serializerOptions,
                cancellationToken
            );

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
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
