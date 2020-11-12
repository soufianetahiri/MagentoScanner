using MagentoScanner.Helpers;
using MagentoScanner.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MagentoScanner.Core
{
   public static class RobotsSitemap
    {
        public static async System.Threading.Tasks.Task GetSitemaps(TargetOptions targetOptions, HttpClient client)
        {
            HttpResponseMessage result = await client.GetAsync(new Uri(string.Concat(Helper.AddSlash(targetOptions), "robots.txt")), HttpCompletionOption.ResponseHeadersRead);
            if (result.IsSuccessStatusCode)
            {
                var rTxt = await result.Content.ReadAsStringAsync();
                Logger.Log(Importance.Log, " Robots.txt found ", ConsoleColor.White);
                if (rTxt.Contains("Sitemap",StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] sitemaps = rTxt.Split("Sitemap:", StringSplitOptions.RemoveEmptyEntries);
                    Logger.Log(Importance.Log, " Sitemap found: " , ConsoleColor.White);
                    for (int i = 1; i < sitemaps.Length ; i++)
                    {
                        if (!string.IsNullOrEmpty(sitemaps[i]))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\t\t " + sitemaps[i]);
                            Console.ResetColor();
                        }
                      
                    }
                }
                else
                {
                    Logger.Log(Importance.Warning, "Sitemap is not declared in robots.txt", ConsoleColor.DarkYellow);
                }
            }
            else
            {
                Logger.Log(Importance.Warning, "No robots.txt found", ConsoleColor.DarkYellow);
            }
        }

    }
}
