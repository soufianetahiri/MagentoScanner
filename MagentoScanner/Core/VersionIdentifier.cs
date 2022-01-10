using MagentoScanner.Helpers;
using MagentoScanner.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MagentoScanner.Core
{
    public static class VersionIdentifier
    {
        public static async Task TryToDetectVersion(TargetOptions targetOptions, HttpClient client)
        {
            Logger.Log(Importance.Log, " Identifying Magento version...", ConsoleColor.White);

            bool versionIdentified = false;
            //Try direct access Magento 2.x
            //https://magento.stackexchange.com/questions/60476/determine-magento-version-without-access-to-code-base
            string u = string.Concat(Helper.AddSlash(targetOptions), "magento_version");
            HttpResponseMessage result = await client.GetAsync(new Uri(u), HttpCompletionOption.ResponseHeadersRead);
            if (result.IsSuccessStatusCode && result.RequestMessage.RequestUri == new Uri(u))
            {
                string tmpRslt = await result.Content.ReadAsStringAsync();
                if (!tmpRslt.Contains("</body>"))
                {
                    Logger.Log(Importance.Info, "Magento version used: " + tmpRslt, ConsoleColor.Green);
                    versionIdentified = true;
                }
            }
            //Redirection?
            else if (result.IsSuccessStatusCode && result.RequestMessage.RequestUri != new Uri(u))
            {
                HttpResponseMessage result2 = await client.GetAsync(
                    string.Concat(result.RequestMessage.RequestUri.ToString(), "magento_version"),
                    HttpCompletionOption.ResponseHeadersRead);

                string tmpRslt = await result2.Content.ReadAsStringAsync();
                if (!tmpRslt.Contains("</body>"))
                {
                    Logger.Log(Importance.Info, "Magento version used: " + tmpRslt, ConsoleColor.Green);
                    versionIdentified = true;
                }
            }

            try
            {
                if (!versionIdentified)
                {
                    //start version identification
                    JObject versionHashs = LoadVersionHash();
                    foreach (KeyValuePair<string, JToken> jToken in versionHashs)
                    {
                        result = await client.GetAsync(
                            new Uri(string.Concat(Helper.AddSlash(targetOptions), jToken.Key)),
                            HttpCompletionOption.ResponseHeadersRead);
                        if (result.IsSuccessStatusCode)
                        {
                            string fileHash = HashCalculator.CalculateMD5(await result.Content.ReadAsByteArrayAsync())
                                .ToLower();
                            if (jToken.Value.SelectToken(fileHash) != null)
                            {
                                Logger.Log(Importance.Info,
                                    "Magento version used: " + jToken.Value.SelectToken(fileHash).Value<string>(),
                                    ConsoleColor.Green);
                                versionIdentified = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Importance.Warning, "Error while determining Magento version", ConsoleColor.Red);
                Logger.Log(Importance.Warning, ex.Message + ex.Source + ex.StackTrace, ConsoleColor.Red);
            }

            if (!versionIdentified)
            {
                Logger.Log(Importance.Warning, "Unable to determine Magento version", ConsoleColor.DarkYellow);
            }
        }

        private static JObject LoadVersionHash()
        {
            string version_hashes =
                Path
                    .Combine(AppDomain.CurrentDomain.BaseDirectory.CurrentProjectFolder(),
                        "../Resources/version_hashes.json");

            if (File.Exists(version_hashes))
            {
                using (StreamReader file = File.OpenText(version_hashes))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject) JToken.ReadFrom(reader);
                }
            }

            Logger.Log(Importance.Critical, "version_hashes.json file not found!", ConsoleColor.Red);
            return null;
        }
    }
}