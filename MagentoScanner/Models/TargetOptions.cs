using System.Collections.Specialized;
using System.Text.RegularExpressions;
using MatthiWare.CommandLine.Core.Attributes;

namespace MagentoScanner.Models
{
   public class TargetOptions
    {
        [Required, Name("u", "url"), Description("Target URL (e.g. 'http://www.site.com/')"), Required]
        public string Url { get; set; }

        [Name("ssl", "ignore-ssl"), Description("Ignore SSL errors")]

        public bool IgnoreSSl { get; set; }
        [Name("mr", "mage-report"), Description("Scan target using https://www.magereport.com/ ")]
        public bool MageReport { get; set; }
        [Name("d", "discover"), Description("Search for some default locations (admin panels, customer panels...)")]

        public bool DiscoverContent { get; set; }
        
        [Name("ua", "user-agent"), Description("HTTP User-Agent header value")]
        public string UserAgent
        {
            get { return string.Empty; }
            set
            {
                if (this.CustomHeaders != null)
                    if (!CustomHeaders.Contains("user-agent"))
                        CustomHeaders.Add("user-agent", value);
            }
        }

        [Name("H" ,"headers"), Description("List of headers (e.g. \"Accept-Language: fr|ETag: 123\")")]
        public string Headers 
        {
            get { return string.Empty; } // CommandeLineParser need a getter
            set 
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    var dics = new ListDictionary();
                    var regex = new Regex("(.+):(.+)");
                    if (value.Contains(@"|"))
                    {
                        foreach (var line in value.Split(@"|"))
                        {
                            CustomHeaderBuilder(dics, regex, line);
                        }
                    }
                    else // have one line
                    {
                        CustomHeaderBuilder(dics, regex, value);
                    }
                    this.CustomHeaders = dics;
                }
            } 
        }

        [Name("c", "cookie"), Description("HTTP cookie header")]
        public string Cookies { get;  set; }

        internal ListDictionary CustomHeaders
        { 
            get; set;
        }

        private void CustomHeaderBuilder(ListDictionary dics, Regex regex, string line)
        {
            if (regex == null || string.IsNullOrWhiteSpace(line))
                return;
            var match = regex.Match(line);
            if(match.Success)
                dics.Add(match.Groups[1].Value.Trim().ToLower(), match.Groups[2].Value.Trim());
        }
    }
}
