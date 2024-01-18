using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using UtilityBot.Controllers;

namespace UtilityBot
{
    internal class Bot : BackgroundService
    {
        private ITelegramBotClient _telegramClient;

        private DefaultMessageController _defaultMessageController;
        private TextMessageController _textMessageController;
        private InlineKeyboardController _inlineKeyboardController;


        public Bot(ITelegramBotClient telegramClient, 
            DefaultMessageController defaultMessageController,
            TextMessageController textMessageController,
            InlineKeyboardController inlineKeyboardController)
        {
            _telegramClient = telegramClient;
            _defaultMessageController = defaultMessageController;
            _textMessageController = textMessageController;
            _inlineKeyboardController = inlineKeyboardController;
        }
        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Выводим в консоль информацию об ошибке
            Console.WriteLine(errorMessage);

            // Задержка перед повторным подключением
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");

            //а значит это заменяем на awaitor, ну чтобы был какой-то смысл в 
            await Task.Delay(10000, cancellationToken);
            
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //  Обрабатываем нажатия на кнопки  из Telegram Bot API: https://core.telegram.org/bots/api#callbackquery
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _inlineKeyboardController.Handle(update.CallbackQuery, cancellationToken);
                return;
            }

            // Обрабатываем входящие сообщения из Telegram Bot API: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                //Что такое !.  никогда раньше не встречал
                switch (update.Message!.Type)
                {
                    case MessageType.Text:
                        await _textMessageController.Handle(update.Message, cancellationToken);
                        return;
                    default:
                        await _defaultMessageController.Handle(update.Message, cancellationToken);
                        return;
                }
            }
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Factory.StartNew(() =>
            {
                _telegramClient.StartReceiving(
                    HandleUpdateAsync, HandleErrorAsync,
                    new ReceiverOptions { AllowedUpdates = { } },
                    cancellationToken: stoppingToken);
                Console.WriteLine("Бот запущен!");
            }, stoppingToken);
        }
    }
}
