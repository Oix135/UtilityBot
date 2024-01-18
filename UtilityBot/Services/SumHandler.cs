using System.Text.RegularExpressions;
using Telegram.Bot;
using UtilityBot.Models;

namespace UtilityBot.Services
{
    internal partial class SumHandler : IMessageHandler<SumHandler>
    {
        private readonly ITelegramBotClient _telegramBotClient;
        public SumHandler(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
        }

        [GeneratedRegex("[1-9 ]+$")]
        private static partial Regex DigitRegex();

        public string Process(string message)
        {
            if (DigitRegex().IsMatch(message))
            {
                int sum = 0;
                var mas = message.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var digit in mas)
                {
                    _ = int.TryParse(digit, out int s);
                    sum += s;
                }
                return $"Cумма чисел: {sum}";
            }
            return "В строке должны быть только целые числа!\nДля более сложных вычислений обратитесь в Академию Наук.";
        }
    }
}
