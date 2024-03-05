using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Headers;

namespace TypeaheadAIWin.Source
{
    internal static class HttpClientExtensions
    {
        public static HttpResponseMessage PostAsStreamAsync(
            this HttpClient client,
            string uri,
            object requestModel,
            String authorizationToken,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };
            }

            var content = JsonContent.Create(requestModel, null, options);

            using var request = CreatePostEventStreamRequest(uri, authorizationToken, content);

            try
            {
                return client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            }
            catch (PlatformNotSupportedException)
            {
                using var newRequest = CreatePostEventStreamRequest(uri, authorizationToken, content);
                var responseTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var response = responseTask.GetAwaiter().GetResult();
                return response;
            }
        }

        private static HttpRequestMessage CreatePostEventStreamRequest(string uri, string authorizationToken, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new("text/event-stream"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);
            request.Content = content;

            return request;
        }
    }
}
