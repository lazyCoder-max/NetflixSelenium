using System;
using Telegram.Bot.Types;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public partial class OutboxManager : IOutboxManager
    {
        public OutBoxEventArgs OutBox { get; set; }
        public OutboxManager()
        {
            OutBox = new OutBoxEventArgs();
        }
        public void LogMessage(Message message, object sender)
        {
            OutBox.Message = message;
            OutBox.MessageType = message.Type;
            OutBox.Sender = sender;
            OutBox.SentDate = DateTime.Now;
        }
    }
}
