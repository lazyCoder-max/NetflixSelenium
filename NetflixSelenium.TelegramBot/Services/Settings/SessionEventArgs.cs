using System;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class SessionEventArgs
    {
        /// <summary>
        /// Total minuites that the user can spent without interaction
        /// Default= "2 Min."
        /// </summary>
        public float Duration { get; set; }
        /// <summary>
        /// Session Status
        /// </summary>
        public bool ActivityStatus { get; set; }
        /// <summary>
        /// Session created date and time
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Session stopped date and time
        /// </summary>
        public DateTime DestroyedDate { get; set; }
        public DateTime LastActivity { get; set; }
        public SessionLog Log { get; set; }
    }
}