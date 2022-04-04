using System;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot;
namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class Usr
    {
        public long TelegramId { get; set; }
        public long AccountId { get; set; }
        private Timer SessionInvoker { get; }
        public MessageEventArgs Message { get; }
        public SessionEventArgs Session { get; private set; }
        public NavigationManager Navigation { get; }

        public event EventHandler<SessionEventArgs> SessionStarted;
        public event EventHandler<SessionEventArgs> SessionStopped;
        public event EventHandler<SessionEventArgs> SessionExpired;
        private event EventHandler<SessionEventArgs> SessionActivity;
        /// <summary>
        /// Reised if the user recieved a message
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageRecieved;

        public event EventHandler<MessageEventArgs> IncomingMessageUpdated;

        public Usr()
        {
            SessionInvoker = new Timer(5000);
            SessionInvoker.Elapsed += new ElapsedEventHandler(OnSessionActivity);
            Message = new MessageEventArgs { OutboxManager = new OutboxManager() };
            Navigation = new NavigationManager(this);

        }

        protected virtual void OnSessionStartedAsync(float duration)
        {
            try
            {
                if (SessionStarted != null)
                {
                    Session = CreateSession(duration);
                    SessionStarted?.Invoke(this, Session);
                    if (SessionActivity == null)
                        SessionActivity += User_SessionActivity;

                }
            }
            catch (Exception)
            {
            }
        }
        protected virtual void OnSessionExpired()
        {
            SessionExpired?.Invoke(this, Session);
        }
        protected virtual void OnSessionStopped()
        {
            SessionStopped?.Invoke(this, Session);
        }
        protected virtual void OnSessionActivity(object sender, EventArgs e)
        {
            SessionActivity?.Invoke(this, Session);
        }

        protected virtual void OnIncommingMessageUpdated()
        {
            IncomingMessageUpdated?.Invoke(this, Message);
        }

        protected virtual void OnMessageRecieved()
        {
            MessageRecieved?.Invoke(this, Message);
        }
        private void User_SessionActivity(object sender, SessionEventArgs e)
        {
            try
            {
                if (DateTime.Now.Subtract(e.LastActivity).TotalMinutes > e.Duration)
                {
                    OnSessionExpired();
                    SessionExpired += User_SessionExpiredAsync;
                }
                else
                    Session.ActivityStatus = true;
            }
            catch (Exception)
            {
            }
        }

        private async void User_SessionExpiredAsync(object sender, SessionEventArgs e)
        {
            try
            {
                // Check if the user is not playing a game
                await RemoveHistory();
                Session.DestroyedDate = DateTime.Now;
                Session.ActivityStatus = false;
                SessionInvoker.Stop();
                if (SessionActivity != null)
                    this.SessionActivity -= User_SessionActivity;
                MessageSetting.UsersList.Remove(TelegramId);
            }
            catch (Exception)
            {
            }

        }

        public void StopSession()
        {
            OnSessionStopped();
            if (SessionActivity != null)
                SessionActivity -= User_SessionActivity;
            Session.ActivityStatus = false;
            Session.DestroyedDate = DateTime.Now;
            SessionInvoker.Stop();
        }
        public void StartSessionAsync(float duration)
        {
            OnSessionStartedAsync(duration);
            SessionInvoker.Start();
        }
        public void StartUpdatingMessage()
        {
            OnMessageRecieved();
            OnIncommingMessageUpdated();
        }
        public void RecieveMessage()
        {
            OnMessageRecieved();
        }
        private SessionEventArgs CreateSession(float duration)
        {
            SessionEventArgs session = new SessionEventArgs()
            {
                ActivityStatus = true,
                CreatedDate = DateTime.Now,
                Duration = duration,
                LastActivity = DateTime.Now.AddMilliseconds(-5)
            };
            return session;
        }
        private async Task RemoveHistory()
        {
            try
            {
                if (Message.OutboxManager.OutBox.Message != null && Message.OutboxManager.OutBox.Message.MessageId >= 1)
                    await MessageSetting.Bot.DeleteMessageAsync(TelegramId, Message.OutboxManager.OutBox.Message.MessageId);
            }
            catch (Exception)
            {
            }
        }
    }
}
