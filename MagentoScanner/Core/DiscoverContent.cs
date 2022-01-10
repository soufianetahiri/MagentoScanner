using MagentoScanner.Helpers;
using MagentoScanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagentoScanner.Core
{

    public static class DiscoverContent

    {
        private static readonly string[] admin_locations = { "admin", "index.php/admin", "administrator", "index.php/administrator", "backend","adminMage" };
        private static readonly string[] customer_locations = { "customer/account/create/", "customer/account/login/", "customer/account/forgotpassword/"};
        private static HttpClient _client = new HttpClient();


        public static async Task StartDiscoveryAsync(TargetOptions targetOptions, HttpClient client)
        {
            await IdentifyEasyHits(targetOptions, client);

            Logger.Log(Importance.Log, " Starting content discovery (warming up) ", ConsoleColor.White);

            HttpResponseMessage[] results = await LoadContentAsync(targetOptions);
            try
            {
                foreach (var result in results)
                {
                    if (result != null && (int) result.StatusCode != 404)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t\t  " + "[" + (int) result.StatusCode + "] " +
                                          result.RequestMessage.RequestUri.ToString());
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Importance.Warning, "Error during content discovery", ConsoleColor.Red);
                Logger.Log(Importance.Warning, ex.Message + ex.Source + ex.StackTrace, ConsoleColor.Red);
            }
        }

        private static async Task IdentifyEasyHits(TargetOptions targetOptions, HttpClient client)
        {
            Logger.Log(Importance.Log, " Identifying admin pannel... ", ConsoleColor.White);
            bool isAdminPathIdentified = false;
            for (int i = 0; i < admin_locations.Length; i++)
            {
                HttpResponseMessage result = await client.GetAsync(new Uri(string.Concat(Helper.AddSlash(targetOptions), admin_locations[i])),
                    completionOption: HttpCompletionOption.ResponseHeadersRead);
                if (result.IsSuccessStatusCode)
                {
                    Logger.Log(Importance.Info, "[" + (int)result.StatusCode + "] " +
                        string.Concat(Helper.AddSlash(targetOptions), admin_locations[i]), ConsoleColor.Green);
                    isAdminPathIdentified = true;
                }
            }
            if (!isAdminPathIdentified)
            {
                Logger.Log(Importance.Warning, "No admin panel identified.", ConsoleColor.DarkYellow);

            }

            Logger.Log(Importance.Log, " Identifying customer related screens... ", ConsoleColor.White);
            bool isCustomerPathIdentified = false;
            for (int i = 0; i < customer_locations.Length; i++)
            {
                HttpResponseMessage result = await client.GetAsync(new Uri(string.Concat(Helper.AddSlash(targetOptions), customer_locations[i])),
                    HttpCompletionOption.ResponseHeadersRead);
                if (result.IsSuccessStatusCode)
                {
                    Logger.Log(Importance.Info, "[" + (int)result.StatusCode + "] " +
                        string.Concat(Helper.AddSlash(targetOptions), customer_locations[i]), ConsoleColor.Green);
                    isCustomerPathIdentified = true;
                }
            }
            if (!isCustomerPathIdentified)
            {
                Logger.Log(Importance.Warning, "No customer related screen identified.", ConsoleColor.DarkYellow);

            }
        }


        //Please choose a better name than this
        private static async Task<HttpResponseMessage> GetData(string url)
        {
            //Handling stupidly stupid Unrecognized Response  
            try
            {
                return await _client.GetAsync(url);
            }
            catch
            {
                return null;
            }  
        }

        private static async Task<HttpResponseMessage[]> LoadContentAsync(TargetOptions targetOptions)
        {
            string content = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.CurrentProjectFolder(), "../Resources/default_content.txt");
            if (File.Exists(content))
            {
                string[] paths = File.ReadAllLines(content);
                var tasks = new List<Task<HttpResponseMessage>>();
                for (int i = 0; i < paths.Length; i++)
                { 
                    tasks.Add(GetData(string.Concat(Helper.AddSlash(targetOptions), paths[i])));
                    Console.Write("\r[{0}] Preparing {1} requests", DateTime.Now.ToString("HH:mm:ss"), i);
                }
                Console.WriteLine("\n[{0}] {1}", DateTime.Now.ToString("HH:mm:ss"),"Analyzing responses, it may takes sometime so PLEASE WAIT :-)");
                return await Task.WhenAll(tasks);
            }
            return null;
        }
    }
}

