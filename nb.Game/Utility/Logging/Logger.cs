using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nb.Game.Utility.Logging
{
    public static class Logger
    {
        public static async Task LogAsync(LogMessage message) {
            // Get the output stream
            TextWriter cout = Console.Out;
            
            // Determine the color to use
            switch (message.severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Normal:
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            await cout.WriteAsync($"[{message.severity.ToString(), 8}] ");
            Console.ResetColor();

            // Source
            await cout.WriteAsync(message.source);

            // Spacer
            await cout.WriteAsync(": ");

            // Message
            await cout.WriteLineAsync(message.message);

            // This exception gets displayed on a newline
            if (message.exception != null) await cout.WriteLineAsync(message.exception.ToString());

            cout.Close();
            cout.Dispose();
        }
        
        public static void Log(LogMessage message) {
            LogAsync(message).Wait();
        }
    }
}