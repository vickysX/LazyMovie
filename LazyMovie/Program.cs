using Telegram.Bot;

namespace LazyMovie
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var botClient = new TelegramBotClient(GetEnvironmentVariable("TELEGRAM_API_TOKEN"));
                    services.AddSingleton(typeof(ITelegramBotClient), botClient);
                    services.AddHttpClient();
                    services.AddHostedService<Worker>();
                    //services.AddSingleton<ITelegramBotClient, Worker>();
                })
                .Build();

            host.Run();
        }
    }
}