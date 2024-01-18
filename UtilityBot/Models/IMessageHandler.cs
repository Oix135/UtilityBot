namespace UtilityBot.Models
{
    internal interface IMessageHandler<in T>
    {
        string Process(string message);
    }
}
