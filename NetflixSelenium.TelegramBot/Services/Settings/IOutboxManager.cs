using Telegram.Bot.Types;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public interface IOutboxManager
    {
        OutboxManager.OutBoxEventArgs OutBox { get; set; }

        void LogMessage(Message message, object sender);
    }
}