using System;
using System.Timers;
using Telegram.Bot;
namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class AlertMessage
    {
        private AlertEventArgs AlertEvent { get; }
        private Timer AlertTimer { get; }

        private event EventHandler<AlertEventArgs> AlertActivity;
        public event EventHandler<AlertEventArgs> AlertExpired;
        public AlertMessage()
        {
            AlertEvent = new AlertEventArgs();
            AlertTimer = new Timer(1000);
            AlertTimer.Elapsed += new ElapsedEventHandler(OnAlertActivity);
        }
        protected virtual void OnAlertActivity(object sender, EventArgs e)
        {
            AlertActivity?.Invoke(this, AlertEvent);
        }
        protected virtual void OnAlertExpired()
        {
            AlertExpired?.Invoke(this, AlertEvent);
        }
        private void AlertMessage_AlertActivity(object sender, AlertEventArgs e)
        {
            if (DateTime.Now.Subtract(e.CreatedDate).TotalSeconds > e.Duration)
            {
                StopAlert();
            }
        }
        public void InvokeAlert(int duration, int messageId, long chatId)
        {
            AlertEvent.MessageId = messageId;
            AlertEvent.ChatId = chatId;
            AlertTimer.Start();
            AlertEvent.Duration = duration;
            AlertEvent.CreatedDate = DateTime.Now.AddMilliseconds(-5);
            AlertActivity += AlertMessage_AlertActivity;
        }
        public void InvokeAlert(int messageId, long chatId)
        {
            AlertEvent.MessageId = messageId;
            AlertEvent.ChatId = chatId;
            AlertTimer.Start();
            AlertEvent.Duration = 80;
            AlertEvent.CreatedDate = DateTime.Now.AddMilliseconds(-5);
            AlertActivity += AlertMessage_AlertActivity;
        }
        public void StopAlert()
        {
            DeleteMessage();
            AlertActivity -= AlertMessage_AlertActivity;
            AlertTimer.Stop();
            OnAlertExpired();
        }
        private void DeleteMessage()
        {
            try
            {
                MessageSetting.Bot.DeleteMessageAsync(AlertEvent.ChatId, AlertEvent.MessageId);
            }
            catch (Exception)
            {
            }
        }
    }
}
