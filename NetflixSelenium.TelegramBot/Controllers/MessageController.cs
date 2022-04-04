using Microsoft.AspNetCore.Mvc;
using NetflixSelenium.TelegramBot.Services.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace NetflixSelenium.TelegramBot.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        [Route("api/message/post")]
        [HttpPost]
        public IActionResult Post([FromBody] Update update)
        {
            if (UserManager.IsUserAvailableAsync(update))
            {
                // update
                UserManager.UpdateIncomingMessageAsync(update);
            }
            else
            {
                // create new
                UserManager.AddNewUser(update);
                UserManager.StartNewSessionAsync(update);
            }
            DeleteMessage(update);
            return Ok();
        }
        private void DeleteMessage(Update update)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                MessageSetting.Bot.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId);
            }
        }
    }
}
