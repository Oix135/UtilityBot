using UtilityBot.Models;

namespace UtilityBot.Services
{
    internal interface IStorage
    {
        Session GetSession(long chatId);
    }
}
