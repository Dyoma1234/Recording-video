using System;

namespace LoggerApp
{
    public class ConsoleLoger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }

        public void Error(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }
    }
}
