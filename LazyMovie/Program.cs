using Telegram.Bot;
using System.Net.Http.Headers;

namespace LazyMovie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    ITelegramBotClient botClient =
                        new TelegramBotClient(GetEnvironmentVariable("TELEGRAM_API_TOKEN")!);
                    services.AddSingleton<IMovieClient, MovieClient>();
                    services.AddSingleton<ITelegramMovieClient, TelegramMovieClient>();
                    services.AddSingleton(typeof(ITelegramBotClient), botClient);
                    services.AddHostedService<Worker>();
                    services.AddHttpClient(
                        name: "streamingAPI", 
                        configureClient: options =>
                        {
                            options.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue(
                                    mediaType: "application/json", quality: 1.0
                                )
                            );
                        }
                    );
                })
                .Build();

            host.Run();
        }
    }
}