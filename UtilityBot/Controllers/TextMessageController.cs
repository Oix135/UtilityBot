using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using UtilityBot.Models;
using UtilityBot.Services;

namespace UtilityBot.Controllers
{
    internal class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IMessageHandler<CountHandler> _countHandler;
        private readonly IMessageHandler<SumHandler> _sumHandler;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient,
            IMessageHandler<CountHandler> countHandler,
            IMessageHandler<SumHandler> sumHandler,
            IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _countHandler = countHandler;
            _sumHandler = sumHandler;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            switch (message.Text)
            {
                case "/start":
                    {
                        // Объект, представляющий кноки
                        var buttons = new List<InlineKeyboardButton[]>
                        {
                            new[]
                            {
                                 InlineKeyboardButton.WithCallbackData($" Количество символов" , $"count"),
                                 InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"sum")
                            }
                        };

                        // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Бот может вычислять количество символов в строке и сумму чисел.</b> {Environment.NewLine}" +
                            $"{Environment.NewLine}Вы убедитесь, настолько это бывает необходимо, и как вы раньше без этого скучно жили.{Environment.NewLine}",
                            cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                        break;
                    }
                default:
                    {
                        string operation = _memoryStorage.GetSession(message.Chat.Id).Operation;
                        switch (operation)
                        {
                            case "count":
                                {
                                    var mess = message.Text.Trim();
                                    var result = _countHandler.Process(mess);
                                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, result, cancellationToken: ct);
                                    break;
                                }
                            case "sum":
                                {
                                    var mess = message.Text.Trim();
                                    var result = _sumHandler.Process(mess);
                                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, result, cancellationToken: ct);
                                    break;
                                }
                            default:
                                {
                                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Отправьте сообщение.", cancellationToken: ct);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
