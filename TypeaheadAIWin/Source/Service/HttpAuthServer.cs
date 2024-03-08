using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TypeaheadAIWin.Source.Service
{
    public class HttpAuthServer
    {
        public static string CallbackUrl = "http://localhost:35640/";

        private readonly HttpListener _listener = new HttpListener();
        private TaskCompletionSource<string> _tcs;

        public HttpAuthServer()
        {
            _listener.Prefixes.Add(CallbackUrl);
        }

        public async Task<string> StartAndWaitForAuthorizationAsync()
        {
            _listener.Start();
            _tcs = new TaskCompletionSource<string>();

            try
            {
                // Asynchronously wait for a request.
                var context = await _listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // Process the request.
                var callbackUrl = request.Url.ToString();
                Trace.WriteLine($"Received request for {callbackUrl}");

                // Extract the authorization code from the query parameters.
                var query = request.Url.Query;
                var queryParameters = HttpUtility.ParseQueryString(query);
                var authCode = queryParameters["code"]; // Assuming 'code' is the query parameter name for the authorization code
                if (string.IsNullOrEmpty(authCode))
                {
                    throw new InvalidOperationException("Authorization code not found in the request.");
                }

                Trace.WriteLine($"Received authorization code: {authCode}");

                // Respond to the request.
                string responseString = "<html><body>Authentication successful. You can close this window.</body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                // Complete the task with the auth code.
                _tcs.SetResult(authCode);
            }
            catch (Exception ex)
            {
                // If an error occurs, set the exception on the task.
                _tcs.SetException(ex);
            }
            finally
            {
                Stop();
            }

            // Wait for the task to complete and return the result.
            return await _tcs.Task;
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
