using System.Net.Http.Json;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace LazyMovie
{
    public interface ITelegramMovieClient
    {

        Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken
        );

        Task HandlingPollingErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        );

    }


    public class TelegramMovieClient : ITelegramMovieClient
    {

        private readonly IMovieClient _movieClient;
        private readonly ILogger<TelegramMovieClient> _logger;

        public TelegramMovieClient(IMovieClient movieClient, ILogger<TelegramMovieClient> logger)
        {
            _movieClient = movieClient;
            _logger = logger;
        }


        public async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken
        )
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(botClient, update.Message!, cancellationToken);
                    break;
                case UpdateType.InlineQuery:
                    await HandleInlineQuery(botClient, update.InlineQuery!);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleInlineQuery(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            await botClient.AnswerInlineQueryAsync(inlineQuery.Id, new List<InlineQueryResult>());
        }

        public Task HandlingPollingErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(
            ITelegramBotClient botClient,
            Message message,
            CancellationToken cancellationToken
            )
        {
            var chatId = message.Chat.Id;
            var text = message.Text ?? "";
            //_logger.LogDebug($"User wrote {text}");
            if (text.StartsWith('/'))
            {
                await HandleBotCommands(
                    botClient: botClient, 
                    chatId: chatId, 
                    text: text, 
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                (string title, string country) = HandleUserSearch(text);
                MovieSeriesInfo movieSeriesInfo = await _movieClient.SearchByTitle(title, country); 
                    //HandleStreamingApiResponse(title, country);
                Console.WriteLine(movieSeriesInfo.ToString());
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: movieSeriesInfo.ToString(), /*$"You wrote {message.Text}",*/
                    cancellationToken: cancellationToken
                );
            } 
        }

        private async Task HandleBotCommands(
            ITelegramBotClient botClient,
            long chatId,
            string text,
            CancellationToken cancellationToken
        )
        {
            var answer = text switch
            {
                "/start" => "Hi, I'm LazyMovie, ...",
                "/help" => "Here's how I work and how to use me...",
                _ => ""
            };
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: answer,
                cancellationToken: cancellationToken
            );
        }

        private (string title, string country) HandleUserSearch(string text)
        {
            var searchItems = text.Split(' ');
            return (searchItems[0], searchItems[1]);
        }

        private static async Task<MovieSeriesInfo> HandleStreamingApiResponse(
            string title, 
            string country,
            string? showType = "all",
            string? lang = "en"
        )
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://streaming-availability.p.rapidapi.com/v2/search/title" +
                    $"?title={title}&country={country}" +
                    $"&show_type={showType}&output_language={lang}"),
                Headers =
                {
                    {"X-RapidAPI-Key", "d5be745721mshe3e915078cff247p1dd875jsn9d63c10dc80a"},
                    {"X-RapidAPI-Host", "streaming-availability.p.rapidapi.com"}
                },
                
            };
            using var response = await httpClient.SendAsync(request);
            Result? resultsList = new Result();
            if (response.IsSuccessStatusCode)
            {
                var stringResults = await response.Content.ReadAsStringAsync();
                resultsList = JsonSerializer.Deserialize<Result>(stringResults);
            }
            var result = resultsList!.Results.FirstOrDefault()!;
            Console.WriteLine( result );
            return result;
        }
    }
}
