using NetflixSelenium.TelegramBot.Services.Settings;

namespace NetflixSelenium.TelegramBot.Services
{
    public class NavigationManager
    {
        public Usr User { get; set; }
        Registration registration;
        public NavigationManager(Usr user)
        {
            User = user;
            user.MessageRecieved += User_MessageRecieved;
        }

        private async void User_MessageRecieved(object sender, MessageEventArgs e)
        {
            if (registration == null)
                registration = new Registration(User);
            await registration.Navigate(e.In);
            //InlineKeyboardButton proceed = new InlineKeyboardButton("")
            //{
            //    CallbackData = "Registration/Start",
            //    Text = "Proceed ⏭️"
            //};
            //List<InlineKeyboardButton> raw1 = new List<InlineKeyboardButton>();
            //raw1.Add(personalInfo);
            //Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup inlineKeyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[] {
            //        //raw1
            //    });

        }
    }
}
