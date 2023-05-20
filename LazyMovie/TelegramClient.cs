using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace LazyMovie
{
    public static class TelegramClient
    {
        public static async Task HandleUpdateAsync(
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

        private static async Task HandleInlineQuery(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            await botClient.AnswerInlineQueryAsync(inlineQuery.Id, new List<InlineQueryResult>());
        }

        public static Task HandlingPollingErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            return Task.CompletedTask;
        }

        private static async Task HandleMessageAsync(
            ITelegramBotClient botClient,
            Message message,
            CancellationToken cancellationToken
            )
        {
            var chatId = message.Chat.Id;
            var text = message.Text ?? "";
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
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"You wrote {message.Text}",
                    cancellationToken: cancellationToken
                );
            } 
        }

        private static async Task HandleBotCommands(
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
    }
}
