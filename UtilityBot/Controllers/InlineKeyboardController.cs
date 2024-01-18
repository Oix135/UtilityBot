using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using UtilityBot.Services;

namespace UtilityBot.Controllers
{
    internal class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;
            _memoryStorage.GetSession(callbackQuery.From.Id).Operation = callbackQuery.Data;
            string operation = callbackQuery.Data switch
            {
                "count" => " Количество символов",
                "sum" => " Сумма чисел",
                _ => string.Empty
            };
            await _telegramClient.SendTextMessageAsync(callbackQuery.From.Id,
                $"<b>Выбранная операция - {operation}.{Environment.NewLine}</b>" +
                $"{Environment.NewLine}Можно поменять в главном меню.", cancellationToken: ct, parseMode: ParseMode.Html);
        }
    }
}
