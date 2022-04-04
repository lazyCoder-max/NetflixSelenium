using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NetflixSelenium.TelegramBot.Services.Settings
{
    public class UserManager
    {
        public static bool IsUserAvailableAsync(Update e)
        {
            bool isExist = false;
            try
            {
                if (MessageSetting.UsersList == null)
                    MessageSetting.UsersList = new Dictionary<long, Usr>();

                if (e.Type == UpdateType.CallbackQuery)
                    isExist = MessageSetting.UsersList.ContainsKey(e.CallbackQuery.Message.Chat.Id);
                else if (e.Type == UpdateType.Message)
                    isExist = MessageSetting.UsersList.ContainsKey(e.Message.Chat.Id);
                else if (e.Type == UpdateType.ChosenInlineResult)
                    isExist = MessageSetting.UsersList.ContainsKey(e.ChosenInlineResult.From.Id);
                else if (e.Type == UpdateType.EditedMessage)
                    isExist = MessageSetting.UsersList.ContainsKey(e.EditedMessage.Chat.Id);
                else if (e.Type == UpdateType.PollAnswer)
                    isExist = MessageSetting.UsersList.ContainsKey(e.PollAnswer.User.Id);
            }
            catch (Exception)
            {
            }
            return isExist;
        }
        public static int GetMessageId(Update e)
        {
            int id = 0;
            try
            {
                if (e.Type == UpdateType.Message)
                {
                    id = e.Message.MessageId;
                }
                else if (e.Type == UpdateType.CallbackQuery)
                {
                    id = e.CallbackQuery.Message.MessageId;
                }
                else if (e.Type == UpdateType.ChosenInlineResult)
                {
                    id = int.Parse(e.ChosenInlineResult.InlineMessageId);
                }
                else if (e.Type == UpdateType.EditedMessage)
                {
                    id = e.EditedMessage.MessageId;
                }
                else if (e.Type == UpdateType.PollAnswer)
                {
                    id = int.Parse(e.PollAnswer.PollId);
                }
            }
            catch (Exception)
            {
            }
            return id;
        }
        public static void AddNewUser(Update e)
        {
            try
            {
                Usr tempUser = new Usr();
                tempUser.Message.In = e;
                tempUser.TelegramId = GetTelegramId(e);
                if (MessageSetting.UsersList == null)
                    MessageSetting.UsersList = new Dictionary<long, Usr>();
                if (!MessageSetting.UsersList.ContainsKey(tempUser.TelegramId))
                    MessageSetting.UsersList.Add(tempUser.TelegramId, tempUser);
            }
            catch (Exception)
            {
            }
        }
        public static long GetTelegramId(Update e)
        {
            long id = 0;
            try
            {
                if (e.Type == UpdateType.Message)
                {
                    id = e.Message.From.Id;
                }
                else if (e.Type == UpdateType.CallbackQuery)
                {
                    id = e.CallbackQuery.From.Id;
                }
                else if (e.Type == UpdateType.EditedMessage)
                {
                    id = e.EditedMessage.Chat.Id;
                }
                else if (e.Type == UpdateType.ChosenInlineResult)
                {
                    id = e.ChosenInlineResult.From.Id;
                }
                else if (e.Type == UpdateType.InlineQuery)
                {
                    id = e.InlineQuery.From.Id;
                }
                else if (e.Type == UpdateType.PollAnswer)
                {
                    id = e.PollAnswer.User.Id;
                }
            }
            catch (Exception)
            {
            }
            return id;
        }

        public static Usr GetUserByIdAsync(long telegramId)
        {
            Usr user = null;
            try
            {
                if (MessageSetting.UsersList != null)
                    user = MessageSetting.UsersList[telegramId];
            }
            catch (Exception)
            {
            }
            return user;
        }
        private static Usr GetUserByIdAsync(Update e)
        {
            Usr user = null;
            try
            {
                if (MessageSetting.UsersList != null)
                {
                    if (e.Type == UpdateType.Message)
                    {
                        user = MessageSetting.UsersList[e.Message.From.Id];
                    }
                    else if (e.Type == UpdateType.CallbackQuery)
                    {
                        user = MessageSetting.UsersList[e.CallbackQuery.From.Id];
                    }
                    else if (e.Type == UpdateType.EditedMessage)
                    {
                        user = MessageSetting.UsersList[e.EditedMessage.From.Id];
                    }
                    else if (e.Type == UpdateType.ChosenInlineResult)
                    {
                        user = MessageSetting.UsersList[e.ChosenInlineResult.From.Id];
                    }
                    else if (e.Type == UpdateType.InlineQuery)
                    {
                        user = MessageSetting.UsersList[e.InlineQuery.From.Id];
                    }
                    else if (e.Type == UpdateType.PollAnswer)
                    {
                        user = MessageSetting.UsersList[e.PollAnswer.User.Id];
                    }
                }
            }
            catch (Exception)
            {
            }
            return user;
        }

        public static void StartNewSessionAsync(Update e)
        {
            try
            {
                var user = GetUserByIdAsync(e);
                if (user != null)
                {
                    user.SessionStarted += User_SessionStarted;
                    user.StartSessionAsync(5);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void User_SessionStarted(object sender, SessionEventArgs e)
        {
            try
            {
                var user = ((Usr)sender);
                user.RecieveMessage();
            }
            catch (Exception)
            {
            }

        }

        public static void UpdateIncomingMessageAsync(Update e)
        {
            try
            {
                var TelegramId = GetTelegramId(e);
                MessageSetting.UsersList[GetTelegramId(e)].Message.In = e;
                MessageSetting.UsersList[TelegramId].StartUpdatingMessage();
                MessageSetting.UsersList[TelegramId].IncomingMessageUpdated += User_IncomingMessageUpdated;
            }
            catch (Exception)
            {
            }
        }
        private static void User_IncomingMessageUpdated(object sender, MessageEventArgs e)
        {
            try
            {
                var user = ((Usr)sender);
                user.Session.LastActivity = DateTime.Now;
            }
            catch (Exception)
            {
            }
        }
    }
}
