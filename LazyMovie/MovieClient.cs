using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LazyMovie
{
    public class MovieClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MovieClient> _logger;

        public MovieClient(IHttpClientFactory httpClientFactory, ILogger<MovieClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieSeriesInfo>?> SearchByTitle(
            string title, 
            string availabilityCountry,
            string showType = "all", 
            string lang = "en"
        )
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage() 
            { 
                Method = HttpMethod.Get, 
                RequestUri = new Uri($"https://streaming-availability.p.rapidapi.com/v2/search/title" +
                    $"?title={title}&country={availabilityCountry}" +
                    $"&show_type={showType}&output_language={lang}"),
                Headers =
                {
                    {"X-RapidAPI-Key", GetEnvironmentVariable("STREAMING_AVAILABILITY_TOKEN")},
                    {"X-RapidAPIHost", "streaming-availability.p.rapidapi.com"}
                }
            };
            var response = await client.SendAsync( request );
            return await response.Content.ReadFromJsonAsync<IEnumerable<MovieSeriesInfo>>();
        }
    }
}
