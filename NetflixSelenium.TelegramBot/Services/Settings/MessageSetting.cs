using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class MessageSetting
    {
        private static Telegram.Bot.TelegramBotClient _client;
        public static Telegram.Bot.ITelegramBotClient Bot
        {
            get
            {
                return _client;
            }
        }
        public static Dictionary<long, Usr> UsersList { get; set; }
        public static async Task<Telegram.Bot.TelegramBotClient> Get()
        {
            if (_client != null)
                return _client;
            _client = new Telegram.Bot.TelegramBotClient(AppSettings.Key);
            UsersList = new Dictionary<long, Usr>();
            var hook = AppSettings.Url;
            await _client.SetWebhookAsync(hook, maxConnections: 100);
            return _client;
        }
    }
}
