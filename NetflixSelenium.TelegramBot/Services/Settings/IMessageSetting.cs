using Telegram.Bot;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public interface IMessageSetting
    {
        ITelegramBotClient Bot { get; }
    }
}