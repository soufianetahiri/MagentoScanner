using MagentoScanner.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace MagentoScanner.Core
{
    public static class ResponseHeaders
    {
        public static void PrintResponseHeaders(HttpResponseMessage result)
        {
            Logger.Log(Importance.Log, " Response Headers:" , ConsoleColor.White);
            foreach (KeyValuePair<string, IEnumerable<string>> headerItem in result.Headers)
            {
                IEnumerable<string> values;
                string HeaderItemValue = "";
                values = result.Headers.GetValues(headerItem.Key);
                if (values != null)
                {
                    foreach (string valueItem in values)
                    {
                        HeaderItemValue = HeaderItemValue + valueItem;
                    }
                }
                Console.WriteLine("\t\t  "+headerItem.Key + " : " + HeaderItemValue);
            }
        }
    }
}
