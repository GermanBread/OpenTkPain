// System
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace uf.Utility.Logging
{
    public static class Logger
    {
        private static async Task LogAsync(LogMessage message) {
            // Get the output stream
            var cout = Console.Out;
            
            // Determine the color to use
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Debug:
                    // Only return if this app does not run in a debugging context
                    #if !DEBUG
                    return;
                    #endif
                // This was put below Debug so that I can let the case fall through
                case LogSeverity.Verbose:
                    // I had to flip DEBUG and VERBOSE
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await cout.WriteAsync($"[{message.Severity, 8}] ");
            Console.ResetColor();

            // Source
            await cout.WriteAsync(message.Source);

            // Spacer
            await cout.WriteAsync(": ");

            // Message
            await cout.WriteLineAsync(message.Message);

            // This exception gets displayed on a newline
            if (message.Exception != null) await cout.WriteLineAsync(message.Exception.ToString());

            cout.Close();
            await cout.DisposeAsync();
        }
        
        public static void Log(LogMessage message) {
            LogAsync(message).Wait();
        }
    }
}