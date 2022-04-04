using Telegram.Bot.Types;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class MessageEventArgs
    {
        public IncomingMessageType MessageType { get; set; }
        public Update In { get; set; }
        public IOutboxManager OutboxManager { get; set; }
        public CallbackQuery Callback { get; set; }
        public ChosenInlineResult ChosenInlineResult { get; set; }
        public Message Msg { get; set; }
        public Poll Poll { get; set; }
        public PollAnswer PollAnswer { get; set; }
        public InlineQuery InlineQuery { get; set; }
        public enum IncomingMessageType
        {
            Callback,
            PollAnswer,
            InvitationLink,
            Message
        }
    }
}
