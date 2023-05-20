using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace LazyMovie
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly ITelegramMovieClient _movieClient;

        public Worker(
            ILogger<Worker> logger, 
            ITelegramBotClient botClient, 
            ITelegramMovieClient movieClient
        )
        {
            _logger = logger;
            _botClient = botClient;
            _movieClient = movieClient;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            ReceiverOptions receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            _botClient.StartReceiving(
                updateHandler: _movieClient.HandleUpdateAsync,
                pollingErrorHandler: _movieClient.HandlingPollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken
            );
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        
        }
    }
}