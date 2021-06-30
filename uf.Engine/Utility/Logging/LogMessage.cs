// System
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace uf.Utility.Logging
{
    public struct LogMessage
    {
        public string Source;
        public LogSeverity Severity;
        public string Message;
        public Exception Exception;
        /// <summary>
        /// Create a log message from scratch
        /// </summary>
        /// <param name="Severity">Severity</param>
        /// <param name="Message">The message (should be meaningful)</param>
        /// <param name="Exception">The caught exception</param>
        /// <param name="Source">Do not set this, it is automatically set by the .NET runtime!</param>
        public LogMessage(LogSeverity Severity, string Message, Exception Exception, [CallerFilePath] string Source = "unknown") {
            this.Severity = Severity;
            this.Source = Path.GetFileName(Source);
            this.Message = Message;
            this.Exception = Exception;
        }
        /// <summary>
        /// Create a log message from scratch
        /// </summary>
        /// <param name="Severity">Severity</param>
        /// <param name="Message">The message (should be meaningful)</param>
        /// <param name="Source">Do not set this, it is automatically set by the .NET runtime!</param>
        public LogMessage(LogSeverity Severity, string Message, [CallerFilePath] string Source = "unknown") {
            this.Severity = Severity;
            this.Source = Path.GetFileName(Source);
            this.Message = Message;
            this.Exception = null;
        }
    }
}