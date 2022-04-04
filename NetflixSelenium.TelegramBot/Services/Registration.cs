using NetflixSelenium.TelegramBot.Services.Settings;
using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace NetflixSelenium.TelegramBot.Services
{
    public class Registration
    {
        public Usr User { get; set; }
        public bool IsNew { get; set; } = true;
        AlertMessage alertMessage;
        Selenium.Models.AccountInformation AccountInformation;
        public Registration(Usr user)
        {
            User = user;
            AccountInformation = new Selenium.Models.AccountInformation();
        }

        /// <summary>
        /// Sends welcome message to the client
        /// </summary>
        private void SendWelcomeMessage()
        {
            if (IsNew)
            {
                SendGreetingSticker();
                IsNew = false;
            }
            var result = MessageSetting.Bot.SendTextMessageAsync(User.TelegramId, "<b>Welcome Please Input your information as follows to create your Netflix® Account</b>\n\n" +
                                                                                  "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV"
                        , ParseMode.Html).Result;
            User.Message.OutboxManager.LogMessage(result, this.GetType());
        }
        private async Task SendErrorMessage()
        {
            await SendErrorStickerAsync();
            await Task.Delay(100);
            var result = MessageSetting.Bot.EditMessageTextAsync(User.TelegramId, User.Message.OutboxManager.OutBox.Message.MessageId, "⚠️<b>Command Not Found</b>\n\n" +
                                                                                  "I couldn't understand what you are trying to say...\n\n" +
                                                                                  "I'm here to serve you if you use the following commands:\n\n" +
                                                                                  "/start - to start process from the beginnig\n" +
                                                                                  "/new - to create a new <b>Netflix</b> account"
                        , ParseMode.Html).Result;
            User.Message.OutboxManager.LogMessage(result, this.GetType());
        }
        private void SendGreetingSticker()
        {
            var result = MessageSetting.Bot.SendStickerAsync(User.TelegramId, "CAACAgIAAxkBAAMiYkcktHqm2jwatcSMRqlyyZpzsNwAAlgAAw220hmdwnGZGduoKSME").Result;
            AlertMessage alert = new AlertMessage();
            alert.InvokeAlert(5, result.MessageId, result.Chat.Id);
        }
        private async Task SendErrorMessage(string message)
        {
            await MessageSetting.Bot.DeleteMessageAsync(User.TelegramId, User.Message.OutboxManager.OutBox.Message.MessageId);
            await SendErrorStickerAsync();
            var result = await MessageSetting.Bot.SendTextMessageAsync(User.TelegramId, message
                        , ParseMode.Html);
            User.Message.OutboxManager.LogMessage(result, this.GetType());
        }
        private async Task SendErrorStickerAsync()
        {
            var result = await MessageSetting.Bot.SendStickerAsync(User.TelegramId, "CAACAgIAAxkBAAM7YkcvzxTW8ZEzl1XGpoAr9mES2yMAAlcAAw220hnIl_fbDyhepSME");
            AlertMessage alert = new AlertMessage();
            alert.InvokeAlert(8, result.MessageId, result.Chat.Id);
        }
        private void SendMessage(string message)
        {
            if (User.Message.OutboxManager.OutBox.Message != null)
            {
                var result = MessageSetting.Bot.EditMessageTextAsync(User.TelegramId, User.Message.OutboxManager.OutBox.Message.MessageId, message
                        , ParseMode.Html).Result;
                User.Message.OutboxManager.LogMessage(result, this.GetType());
            }
            else
            {
                var result = MessageSetting.Bot.SendTextMessageAsync(User.TelegramId, message
                        , ParseMode.Html).Result;
                User.Message.OutboxManager.LogMessage(result, this.GetType());
            }
        }
        /// <summary>
        /// Sends loading sticker message to the client
        /// </summary>
        private void SendLoadingSticker()
        {
            var result = MessageSetting.Bot.SendStickerAsync(User.TelegramId, "CAACAgIAAxkBAANKYkczz4zhaZmK76YZYMXhcmaSlJsAAn0AA0QNzxfyoncKN_7mLSME").Result;
            alertMessage = new AlertMessage();
            alertMessage.InvokeAlert(result.MessageId, result.Chat.Id);
        }
        private void SendSuccessSticker()
        {
            var result = MessageSetting.Bot.SendStickerAsync(User.TelegramId, "CAACAgIAAxkBAAIBf2JIJa7kST_vw4d8AhMHuu74D0p2AAJtAAMNttIZetD8_glwm1UjBA").Result;
            var alert = new AlertMessage();
            alert.InvokeAlert(10, result.MessageId, result.Chat.Id);
        }
        private async void TranslateRegistrationResponse(string response)
        {
            var rawResponse = response.Split('|');
            if (rawResponse.Length == 6)
            {
                //correct format
                if (IsValidEmail(rawResponse[0].TrimEnd().TrimStart()))
                {
                    AccountInformation.EmailAddress = rawResponse[0].TrimEnd().TrimStart();
                    if (IsValidPassword(rawResponse[1].TrimEnd().TrimStart()))
                    {
                        AccountInformation.Password = rawResponse[1].TrimEnd().TrimStart();
                        if (await IsValidCardNumber(rawResponse[2].TrimEnd().TrimStart()))
                        {
                            AccountInformation.CardNumber = rawResponse[2].TrimEnd().TrimStart();
                            if (await IsValidMonth(rawResponse[3].TrimEnd().TrimStart()))
                            {
                                AccountInformation.Month = rawResponse[3].TrimEnd().TrimStart();
                                if (await IsValidYear(rawResponse[4].TrimEnd().TrimStart()))
                                {
                                    AccountInformation.Year = rawResponse[4].TrimEnd().TrimStart();
                                    if (await IsValidCvv(rawResponse[5].TrimEnd().TrimStart()))
                                    {
                                        AccountInformation.CVV = rawResponse[5].TrimEnd().TrimStart();
                                        await Task.Run(async () =>
                                        {
                                            await CreateAccount();
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        await SendErrorMessage("⚠️<b>Invalid Password!</b>\n" +
                                      "Password length must be > 6 !\n" +
                                      "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                      "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                      "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                      $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
                    }
                }
                else
                {
                    await SendErrorMessage("⚠️<b>Invalid Email Address!</b>\n" +
                                     "Please use a correct email address!\n" +
                                     "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                     "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                     "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                     $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
                }
            }
            else
            {
                // Incorrect Format
                var result = MessageSetting.Bot.EditMessageTextAsync(User.TelegramId, User.Message.OutboxManager.OutBox.Message.MessageId, "⚠️<b>Incorrect Format!</b>\n" +
                       "Please use the following format to create your account:\n" +
                       "-----------<b>ℹ️ Format </b>-----------\n\n" +
                       "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                       "-----------<b>ℹ️ Example </b>-----------\n\n" +
                       $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522"
                       , ParseMode.Html).Result;
                User.Message.OutboxManager.LogMessage(result, this.GetType());
            }
        }
        private async Task<bool> IsValidCardNumber(string card)
        {
            var isNumber = Regex.IsMatch(card, @"^\d+$");
            if (isNumber)
                return true;
            else
            {
                await SendErrorMessage("⚠️<b>Invalid Card Number!</b>\n" +
                                 "Please use only number!\n" +
                                 "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                 "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                 "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                 $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
            }
            return false;
        }
        private async Task<bool> IsValidCvv(string cvv)
        {
            var isNumber = int.TryParse(cvv, out int n);
            if (cvv.Length >= 2)
            {
                if (isNumber)
                    return true;
                else
                {
                    await SendErrorMessage("⚠️<b>Invalid CVV!</b>\n" +
                                     "Please use only number!\n" +
                                     "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                     "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                     "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                     $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
                }
            }
            else
            {
                await SendErrorMessage("⚠️<b>Invalid CVV!</b>\n" +
                                     "CVV must me >2 digit number!\n" +
                                     "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                     "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                     "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                     $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
            }
            return false;
        }
        private async Task<bool> IsValidMonth(string month)
        {
            var isNumber = int.TryParse(month, out int n);
            if (month.Length == 2)
            {
                if (isNumber)
                    return true;
                else
                {
                    await SendErrorMessage("⚠️<b>Invalid Month!</b>\n" +
                                      "Please use only number!\n" +
                                      "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                      "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                      "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                      $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
                }
            }
            else
            {
                await SendErrorMessage("⚠️<b>Invalid Month!</b>\n" +
                                      "Month must me 2 digit number!\n" +
                                      "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                      "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                      "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                      $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
            }
            return false;
        }
        private async Task<bool> IsValidYear(string year)
        {
            var isNumber = int.TryParse(year, out int n);
            if (year.Length == 2)
            {
                if (isNumber)
                    return true;
                else
                {
                    await SendErrorMessage("⚠️<b>Invalid Year!</b>\n" +
                                      "Please use only number!\n" +
                                      "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                      "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                      "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                      $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
                }
            }
            else
            {
                await SendErrorMessage("⚠️<b>Invalid Year!</b>\n" +
                                     "Year must me 2 digit number!\n" +
                                     "-----------<b>ℹ️ Format Info</b>-----------\n\n" +
                                     "EMAIL|PASSWORD|CC_NUMBER|CC_MONTH|CC_YEAR|CVV\n\n" +
                                     "-----------<b>ℹ️ Example </b>-----------\n\n" +
                                     $"abcde@email.com|password|123456678992|02|{DateTime.Now.Year}|522");
            }
            return false;
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length > 6)
                return true;
            return false;
        }
        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        private async Task CreateAccount()
        {
            SendLoadingSticker();
            SendMessage("I'm Processing your request Please wait...");
            Selenium.Automation automation = new Selenium.Automation();
            var result = automation.CreateAccount(AccountInformation);
            if (result.ErrorMessage == "Success")
            {
                /// created
                alertMessage.StopAlert();
                SendSuccessSticker();
                SendMessage("<b>✅ Account successfuly created!</b>");
            }
            else if (result.ErrorMessage == "Account Already Exist")
            {
                alertMessage.StopAlert();
                await SendErrorMessage("<b>Account Already Exist!</b>");
            }
            else
            {
                alertMessage.StopAlert();
                await SendErrorMessage("<b>Something went wrong, please try again!</b>");
            }
        }
        public async Task Navigate(Telegram.Bot.Types.Update e)
        {
            if (e != null)
            {
                if (e.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
                {
                    // for regular messages
                    if (e.Message.Text.Contains("|") || e.Message.Text.Contains("@"))
                    {
                        // registration
                        TranslateRegistrationResponse(e.Message.Text);
                    }
                    else if (e.Message.Text == "/start" || e.Message.Text == "/new")
                    {
                        // new
                        SendWelcomeMessage();
                    }
                    else
                    {
                        await SendErrorMessage();
                    }
                }
                else if (e.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    // for buttons
                }
            }
        }

    }
}
