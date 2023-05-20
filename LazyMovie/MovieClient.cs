using Microsoft.Extensions.Logging;
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

    public interface IMovieClient 
    { 
        Task<MovieSeriesInfo?> SearchByTitle(
            string title, 
            string availabilityCountry, 
            string? showType = "all", 
            string? lang = "en"
        );
    }


    public class MovieClient : IMovieClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MovieClient> _logger;

        public MovieClient(IHttpClientFactory httpClientFactory, ILogger<MovieClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<MovieSeriesInfo?> SearchByTitle(
            string title, 
            string availabilityCountry,
            string? showType = "all", 
            string? lang = "en"
        )
        {
            var client = _httpClientFactory.CreateClient("streamingAPI");
            var request = new HttpRequestMessage() 
            { 
                Method = HttpMethod.Get, 
                RequestUri = new Uri($"https://streaming-availability.p.rapidapi.com/v2/search/title" +
                    $"?title={title}&country={availabilityCountry}" +
                    $"&show_type={showType}&output_language={lang}"),
                Headers =
                {
                    {"X-RapidAPI-Key", "d5be745721mshe3e915078cff247p1dd875jsn9d63c10dc80a"},
                    {"X-RapidAPI-Host", "streaming-availability.p.rapidapi.com"}
                }
            };
            using var response = await client.SendAsync( request );
            //_logger.Log(LogLevel.Debug,
            //response.StatusCode.ToString());
            Result? resultsList = new Result();
            if (response.IsSuccessStatusCode)
            {
                var stringResults = await response.Content.ReadAsStringAsync();
                resultsList = JsonSerializer.Deserialize<Result>(stringResults);
            }
            var result = resultsList!.Results.FirstOrDefault()!;
            Console.WriteLine(result);
            return result;
        }
    }
}
