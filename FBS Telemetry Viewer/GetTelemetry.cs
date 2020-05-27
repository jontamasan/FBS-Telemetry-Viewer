using System.Net.Http;
using System.Threading.Tasks;

namespace FBS_Telemetry_Viewer
{
    class GetTelemetry
    {
        private static readonly HttpClient _client;

        static GetTelemetry()
        {
            _client = new HttpClient();
        }

        // Connect to Telemetry using HttpClient.
        public async Task<HttpResponseMessage> GetTelemetryAsync(string uri)
        {
            try
            {
                // Connect to telemetry
                // Added a "ConfigureAwait(false)" to avoid deadlock.
                HttpResponseMessage response = await _client.GetAsync(uri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return response;

                // If we don't need headers, just this one line.
                //return await client.GetStringAsync(uri).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
        }
    }
}
