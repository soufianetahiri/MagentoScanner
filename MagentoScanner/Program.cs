using MagentoScanner.Async;
using MagentoScanner.Helpers;
using MagentoScanner.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MagentoScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(
                    Math.Min(140, Console.LargestWindowWidth),
                    Math.Min(50, Console.LargestWindowHeight));
                Console.Title = "MagentoScanner v0.1";
            }

            string banner = @"
__   __  _______  _______  _______  __    _  _______  _______  _______  _______  _______  __    _  __    _  _______  ______   
|  |_|  ||   _   ||       ||       ||  |  | ||       ||       ||       ||       ||   _   ||  |  | ||  |  | ||       ||    _ |  
|       ||  |_|  ||    ___||    ___||   |_| ||_     _||   _   ||  _____||       ||  |_|  ||   |_| ||   |_| ||    ___||   | ||  
|       ||       ||   | __ |   |___ |       |  |   |  |  | |  || |_____ |       ||       ||       ||       ||   |___ |   |_||_ 
|       ||       ||   ||  ||    ___||  _    |  |   |  |  |_|  ||_____  ||      _||       ||  _    ||  _    ||    ___||    __  |
| ||_|| ||   _   ||   |_| ||   |___ | | |   |  |   |  |       | _____| ||     |_ |   _   || | |   || | |   ||   |___ |   |  | |
|_|   |_||__| |__||_______||_______||_|  |__|  |___|  |_______||_______||_______||__| |__||_|  |__||_|  |__||_v_0.1_||___|  |_|
";

            Console.WriteLine(banner);
            ServiceProvider serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            TargetOptions targetOptions = ArgsHelper.ParseArgs(args);
            Logger.Log(Importance.Log, " Starting scan @ " + DateTime.Now, ConsoleColor.White);
            Logger.Log(Importance.Log, " Target set to " + targetOptions.Url, ConsoleColor.White);
            Console.WriteLine(string.Empty);
            await Task.Run(() => new HttpAsync(httpClientFactory, targetOptions).Start(targetOptions));
        }
    }
}