using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace TypeaheadAIWin
{
    internal static class HttpClientExtensions
    {
        public static HttpResponseMessage PostAsStreamAsync(this HttpClient client, string uri, object requestModel, CancellationToken cancellationToken = default)
        {
            var settings = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            var content = JsonContent.Create(requestModel, null, settings);

            using var request = CreatePostEventStreamRequest(uri, content);

            try
            {
                return client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            }
            catch (PlatformNotSupportedException)
            {
                using var newRequest = CreatePostEventStreamRequest(uri, content);
                var responseTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var response = responseTask.GetAwaiter().GetResult();
                return response;
            }
        }

        private static HttpRequestMessage CreatePostEventStreamRequest(string uri, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Accept.Add(new("text/event-stream"));
            request.Content = content;

            return request;
        }
    }
}
