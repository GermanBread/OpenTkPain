// System
using System;
using System.Runtime.InteropServices;

// OpenTK
using OpenTK.Graphics.OpenGL;

using uf.Utility.Logging;

namespace uf.Utility.Debugging
{
    public static class GlCallback
    {
        public static void Init() {
            GCHandle.Alloc(debugProcCallback);

            GL.DebugMessageCallback(debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
        private static readonly DebugProc debugProcCallback = DebugCallback;
        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) {
            var _messageString = Marshal.PtrToStringAnsi(message, length);
            var _messageSource = source.ToString().Remove(0, 11);

            switch (severity) {
                case DebugSeverity.DebugSeverityHigh:
                    Logger.Log(new LogMessage(LogSeverity.Error, _messageString, null, _messageSource));
                    break;
                case DebugSeverity.DebugSeverityMedium:
                    Logger.Log(new LogMessage(LogSeverity.Error, _messageString, null, _messageSource));
                    break;
                case DebugSeverity.DebugSeverityLow:
                    Logger.Log(new LogMessage(LogSeverity.Warning, _messageString, null, _messageSource));
                    break;
                case DebugSeverity.DebugSeverityNotification:
                case DebugSeverity.DontCare:
                    Logger.Log(new LogMessage(LogSeverity.Info, _messageString, null, _messageSource));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }
    }
}