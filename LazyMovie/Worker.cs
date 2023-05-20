using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using static LazyMovie.TelegramClient;

namespace LazyMovie
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramBotClient _botClient;

        public Worker(ILogger<Worker> logger, ITelegramBotClient botClient)
        {
            _logger = logger;
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlingPollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }
    }
}