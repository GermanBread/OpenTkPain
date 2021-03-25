using System;

namespace nb.Game.Utility.Logging
{
    public struct LogMessage
    {
        public string source;
        public LogSeverity severity;
        public string message;
        public Exception exception;
        
        public LogMessage(LogSeverity severity, string source, string message, Exception exception) {
            this.severity = severity;
            this.source = source;
            this.message = message;
            this.exception = exception;
        }
        public LogMessage(LogSeverity severity, string source, string message) {
            this.severity = severity;
            this.source = source;
            this.message = message;
            this.exception = null;
        }
    }
}