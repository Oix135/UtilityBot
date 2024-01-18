using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Text;
using Telegram.Bot;
using UtilityBot.Controllers;
using UtilityBot.Models;
using UtilityBot.Services;

namespace UtilityBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.Unicode;
            //Ну конечно....
            Console.OutputEncoding = Encoding.UTF8;

            // Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var token =  ConfigurationManager.AppSettings["token"]!.ToString();

            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<IMessageHandler<CountHandler>, CountHandler>();
            services.AddSingleton<IMessageHandler<SumHandler>, SumHandler>();

            services.AddTransient<DefaultMessageController>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();

            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(token));
            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }
    }
}
