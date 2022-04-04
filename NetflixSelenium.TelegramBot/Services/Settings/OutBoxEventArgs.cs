using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public partial class OutboxManager
    {
        public class OutBoxEventArgs
        {
            public Message Message { get; set; }
            public MessageType MessageType { get; set; }
            public DateTime SentDate { get; set; }
            public object Sender { get; set; }
        }
    }
}
