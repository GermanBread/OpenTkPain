// System
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace nb.Game.Utility.Logging
{
    public struct LogMessage
    {
        public string Source;
        public LogSeverity Severity;
        public string Message;
        public Exception Exception;
        
        public LogMessage(LogSeverity Severity, string Message, Exception Exception, [CallerFilePath] string Source = "unknown") {
            this.Severity = Severity;
            this.Source = Path.GetFileName(Source);
            this.Message = Message;
            this.Exception = Exception;
        }
        public LogMessage(LogSeverity Severity, string Message, [CallerFilePath] string Source = "unknown") {
            this.Severity = Severity;
            this.Source = Path.GetFileName(Source);
            this.Message = Message;
            this.Exception = null;
        }
    }
}