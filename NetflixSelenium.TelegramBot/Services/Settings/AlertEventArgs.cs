using System;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class AlertEventArgs
    {
        public int MessageId { get; set; }
        public long ChatId { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}