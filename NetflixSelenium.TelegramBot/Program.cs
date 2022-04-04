using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NetflixSelenium.TelegramBot.Services.Settings;
using System;
using System.Threading.Tasks;

namespace NetflixSelenium.TelegramBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await MessageSetting.Get();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception)
            {
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
