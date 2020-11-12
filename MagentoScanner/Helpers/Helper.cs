using MagentoScanner.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MagentoScanner.Helpers
{
   public static class Helper
    {
        private static string GetUri(this string s)
        {
            return new UriBuilder(s).Uri.ToString();
        }
        public static string AddSlash(TargetOptions targetOptions) {

            if (!string.IsNullOrEmpty( targetOptions?.Url) )
            {
                targetOptions.Url = GetUri(targetOptions.Url);
                    if (targetOptions.Url.EndsWith("/"))
                {
                    return targetOptions.Url;
                }
                else
                {
                    return targetOptions.Url + "/";
                }
            }
            return null;
        }

        public static HttpClient InithttpClient(IHttpClientFactory  httpClientFactory , TargetOptions targetOptions)
        {
            HttpClient client = httpClientFactory.CreateClient("MagentoScanner");
          
            //Add CustomHeader
            if (targetOptions.CustomHeaders != null)
            {
                foreach (DictionaryEntry header in targetOptions.CustomHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key.ToString(), header.Value.ToString());
                }
            }
            //Add CustomCookies
            if (!string.IsNullOrEmpty(targetOptions.Cookies))
            {
                AddHeader(client, "Cookie", targetOptions.Cookies);
              
            }
            return client;
        }

        private static HttpClient AddHeader(HttpClient client, string headerName, string headerValue)
        {
            client.DefaultRequestHeaders.Add(headerName,headerValue );
            return client;
        }
    }
}
