using Telegram.Bot;
using UtilityBot.Models;

namespace UtilityBot.Services
{
    internal class CountHandler : IMessageHandler<CountHandler>
    {
        private readonly ITelegramBotClient _telegramBotClient;
        public CountHandler(ITelegramBotClient telegramBotClient) 
        {
            _telegramBotClient = telegramBotClient;
        }

        public string Process(string message)
        {
            return $"В вашем сообщении: {message.Trim().Length} символов";
        }
    }
}
