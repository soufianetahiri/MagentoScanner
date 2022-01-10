using MagentoScanner.Helpers;
using MagentoScanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagentoScanner.Core
{
    public static class MageReport
    {
        private const string refresh = "&refresh=1";
        private static int scanRetry = 0;


        public static async Task TestMegaReportAsync(TargetOptions targetOptions, HttpClient client)
        {
            Logger.Log(Importance.Log, " Scanning Patches... ", ConsoleColor.White);
            await Scan(targetOptions, client);
        }

        private static async Task Scan(TargetOptions targetOptions, HttpClient client)
        {
            HttpResponseMessage result = await client.GetAsync(new Uri(uriString: string.Concat("https://www.magereport.com/scan/result/?s=",
                string.Concat(Helper.AddSlash(targetOptions), refresh))),
                HttpCompletionOption.ResponseHeadersRead);
            if (!result.IsSuccessStatusCode)
            {
                scanRetry++;
                if (scanRetry < 2)
                {
                    Logger.Log(Importance.Warning, "Unable to connect. Retrying...", ConsoleColor.DarkYellow);
                    result = await client.GetAsync(new Uri(string.Concat("https://www.magereport.com/scan/result/?s=",
           string.Concat(Helper.AddSlash(targetOptions)))),
           HttpCompletionOption.ResponseHeadersRead);
                    if (result.IsSuccessStatusCode)
                    {
                        ParseResponse(await result.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        Logger.Log(Importance.Warning, "Unable to scan for patches. Are you sure it's a Magento ?", ConsoleColor.DarkYellow);
                    }
                }
            }
            else
            {
                ParseResponse(await result.Content.ReadAsStringAsync());
            }
            scanRetry = 0;
        }

        private static void ParseResponse(string mixedString)
        {
            List<string> output = mixedString.Replace("'", string.Empty).Split('{', '}').Where(x => !string.IsNullOrEmpty(x)).ToList();
            bool SecPatchFound = false;
            for (int i = 0; i < output.Count; i++)
            {
                if (output[i].Trim().Contains("security.supee"))
                {
                    SecPatchFound = true;
                    string idPatch = output[i].Split("security.supee")[1].Replace(":", string.Empty).Trim();
                    string infoUrl = string.Format("https://magentary.com/glossary/supee-{0}/", idPatch);
                    Enum.TryParse(ExtractValues(output[i + 1].Trim().Replace("\"", string.Empty), "resultString:"), true, out PatchStats patchStats);
                    string msg = string.Format("Security Patch {0} ({1}). More info @ {2}", idPatch, patchStats, infoUrl);
                    Enum.TryParse(ExtractValues(output[i + 1].Trim().Replace("\"", string.Empty), "riskRating:"), true, out Criticity criticity);
                    PrintPatches(patchStats, msg);
                    PrintRisk(criticity);
                }
            }

            if (!SecPatchFound)
            {
                Logger.Log(Importance.Critical, "Security Patches could not be scanned. Retrying...", ConsoleColor.DarkRed);
            }

            for (int i = 0; i < output.Count; i++)
            {
                if (output[i].Trim().Replace("\"", string.Empty).Contains("security.magversion:"))
                {
                    Logger.Log(Importance.Info, "Confirming Magento version : " + ExtractValues(output[i + 1].Trim().Replace("\"", string.Empty), "resultString:"), ConsoleColor.Green);
                    Enum.TryParse(ExtractValues(output[i + 1].Trim().Replace("\"", string.Empty), "riskRating:"), true, out Criticity criticity);
                    PrintRisk(criticity);
                }
            }
        }

        private static string ExtractValues(string re, string key)
        {
            //dirty lazy way because of why not.
            try
            {
                return re.Split(new string[] { key }, StringSplitOptions.None)[1]
       .Split(',')[0].Replace(" ", string.Empty);
            }
            catch
            {
                return re.Split(new string[] { key }, StringSplitOptions.None)[0]
           .Split(',')[0].Replace(" ", string.Empty);
            }
        }
        private static void PrintPatches(PatchStats patchStats, string data)
        {
            switch (patchStats)
            {
                case PatchStats.NotPatched:
                    PrintPatch(ConsoleColor.Red, data);
                    break;
                case PatchStats.Installed:
                case PatchStats.Patched:
                    PrintPatch(ConsoleColor.Green, data);
                    break;
                case PatchStats.Unknown:
                default:
                    PrintPatch(ConsoleColor.DarkGray, data);
                    break;
            }
        }
        private static void PrintRisk(Criticity criticity)
        {
            switch (criticity)
            {

                case Criticity.Low:
                    Print(Criticity.Low, ConsoleColor.Yellow);
                    break;
                case Criticity.High:
                    Print(Criticity.High, ConsoleColor.Red);
                    break;
                case Criticity.Unknown:
                default:
                    Print(Criticity.Unknown, ConsoleColor.DarkGray);
                    break;
            }
        }

        private static void PrintPatch(ConsoleColor consoleColor, string data)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("\t\t  " + data);
            Console.ResetColor();
        }
        private static void Print(Criticity criticity, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("\t\t  Risk Rating: " + criticity);
            Console.ResetColor();
        }

    }
}
