namespace ClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestApi().Wait();
        }

        static async Task TestApi()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://streaming-availability.p.rapidapi.com/v2/search/title?title=batman&country=us&show_type=movie&output_language=en"),
                Headers =
                {
                    { "X-RapidAPI-Key", "d5be745721mshe3e915078cff247p1dd875jsn9d63c10dc80a" },
                    { "X-RapidAPI-Host", "streaming-availability.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }
        }
    }
}