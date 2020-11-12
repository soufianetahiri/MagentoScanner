using System;
using MagentoScanner.Models;
using MatthiWare.CommandLine;

namespace MagentoScanner.Helpers
{
    public static class ArgsHelper
    {
        public static TargetOptions ParseArgs(string[] args)
        {
            var options = new CommandLineParserOptions {
                AppName = "MagentoScanner",
               };

            var parser = new CommandLineParser<TargetOptions>(options);
            var res = parser.Parse(args);
            if(res.HasErrors)
            {
                Console.ReadKey();
                Environment.Exit(-1);
            }
            return res.Result;

        }
    }
}
