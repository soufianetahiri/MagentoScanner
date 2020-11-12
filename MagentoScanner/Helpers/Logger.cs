using System;

namespace MagentoScanner.Helpers
{
    public static class Logger
    {
        public static void Log(Importance importance, string data, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(string.Format("[{0}] [{1}] {2}", DateTime.Now.ToString("HH:mm:ss"), importance.ToString(), data));
            Console.ResetColor();
        }
    }
}
