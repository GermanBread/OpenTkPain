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
        public static async Task LogAsync(LogMessage Message) {
            // Get the output stream
            TextWriter cout = Console.Out;
            
            // Determine the color to use
            switch (Message.Severity)
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
            }
            await cout.WriteAsync($"[{Message.Severity, 8}] ");
            Console.ResetColor();

            // Source
            await cout.WriteAsync(Message.Source);

            // Spacer
            await cout.WriteAsync(": ");

            // Message
            await cout.WriteLineAsync(Message.Message);

            // This exception gets displayed on a newline
            if (Message.Exception != null) await cout.WriteLineAsync(Message.Exception.ToString());

            cout.Close();
            cout.Dispose();
        }
        
        public static void Log(LogMessage Message) {
            LogAsync(Message).Wait();
        }
    }
}