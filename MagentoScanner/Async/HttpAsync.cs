using MagentoScanner.Core;
using MagentoScanner.Helpers;
using MagentoScanner.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace MagentoScanner.Async
{
    public class HttpAsync
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient client;
        public HttpAsync(IHttpClientFactory httpClientFactory, TargetOptions targetOptions)
        {
            _httpClientFactory = httpClientFactory;
            if (targetOptions.IgnoreSSl)
            {
                HttpClientHandler ignoreSSLHandler = new HttpClientHandler()
                {
#pragma warning disable SCS0004 // Certificate Validation has been disabled
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        return true;
                    }
#pragma warning restore SCS0004 // Certificate Validation has been disabled
                };
                client = new HttpClient(ignoreSSLHandler);
            }
            else
            {
                client = Helper.InithttpClient(_httpClientFactory, targetOptions);
            }
        }
        public async Task<string> Start(TargetOptions targetOptions)
        {
            try
            {
                if ((await InitClientAsync(targetOptions)).IsSuccessStatusCode || 
                    (await InitClientAsync(targetOptions)).StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ResponseHeaders.PrintResponseHeaders(await InitClientAsync(targetOptions));
                    await RobotsSitemap.GetSitemaps(targetOptions, client);
                    await VersionIdentifier.TryToDetectVersion(targetOptions, client);
                    if (targetOptions.MageReport)
                    {
                        await MageReport.TestMegaReportAsync(targetOptions, client);
                    }
                 
                    if (targetOptions.DiscoverContent)
                    {
                        await DiscoverContent.StartDiscoveryAsync(targetOptions, client);
                    }
                }
                else
                {
                    Logger.Log(Importance.Critical, string.Concat(targetOptions.Url) + " responded " + (await InitClientAsync(targetOptions)).StatusCode, ConsoleColor.Red);
                }
                Logger.Log(Importance.Log, " DONE, press any key to exit.", ConsoleColor.White);
                Console.ReadKey();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Log(Importance.Critical, "Something went wrong: " + ex.Message, ConsoleColor.Red);
                Logger.Log(Importance.Critical, ex.Source + "\n" + ex.StackTrace, ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }
            return null;
        }
        private async Task<HttpResponseMessage> InitClientAsync(TargetOptions targetOptions)
        {
            try
            {
                return await client.GetAsync(new Uri(Helper.AddSlash(targetOptions)), HttpCompletionOption.ResponseHeadersRead);

            }
            catch (Exception ex)
            {
                if (ex.InnerException is System.Security.Authentication.AuthenticationException)
                {
                    Logger.Log(Importance.Critical, "The remote certificate is invalid according to the validation procedure. Try turn on  --ignore-ssl flag", ConsoleColor.Red);
                }
                else
                {
                    Logger.Log(Importance.Critical, ex.InnerException.Message, ConsoleColor.Red);
                }
                Console.ReadKey();
                Environment.Exit(0);
            }
                return null;
        }
    }
}
