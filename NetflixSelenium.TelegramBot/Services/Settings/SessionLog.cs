using System;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public partial class SessionLog
    {
        public long Id { get; set; }
        public long? ClientId { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? EndedDate { get; set; }
        public decimal? Duration { get; set; }
        public DateTime? LogDate { get; set; }

    }
}