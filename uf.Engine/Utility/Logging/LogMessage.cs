// System

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace uf.Utility.Logging
{
    public struct LogMessage
    {
        public readonly string Source;
        public readonly LogSeverity Severity;
        public readonly string Message;
        public readonly Exception Exception;
        /// <summary>
        /// Create a log message from scratch
        /// </summary>
        /// <param name="severity">Severity</param>
        /// <param name="message">The message (should be meaningful)</param>
        /// <param name="exception">The caught exception</param>
        /// <param name="source">Do not set this, it is automatically set by the .NET runtime!</param>
        public LogMessage(LogSeverity severity, string message, Exception exception, [CallerFilePath] string source = "unknown") {
            Severity = severity;
            Source = Path.GetFileName(source);
            Message = message;
            Exception = exception;
        }
        /// <summary>
        /// Create a log message from scratch
        /// </summary>
        /// <param name="severity">Severity</param>
        /// <param name="message">The message (should be meaningful)</param>
        /// <param name="source">Do not set this, it is automatically set by the .NET runtime!</param>
        public LogMessage(LogSeverity severity, string message, [CallerFilePath] string source = "unknown") {
            Severity = severity;
            Source = Path.GetFileName(source);
            Message = message;
            Exception = null;
        }
    }
}