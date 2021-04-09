// System
using System;
using System.Runtime.InteropServices;

// OpenTK
using OpenTK.Graphics.OpenGL4;

using nb.Game.Utility.Logging;

namespace nb.Game.Utility.Debugging
{
    public class GL_Callback
    {
        public static void Init() {
            debugProcCallbackHandle = GCHandle.Alloc(debugProcCallback);

            GL.DebugMessageCallback(debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
        private static DebugProc debugProcCallback = debugCallback;
        private static GCHandle debugProcCallbackHandle;
        private static void debugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam) {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            if (severity is DebugSeverity.DebugSeverityMedium)
                Logger.Log(new LogMessage(LogSeverity.Warning, messageString));
            if (severity is DebugSeverity.DebugSeverityHigh)
                Logger.Log(new LogMessage(LogSeverity.Error, messageString));
        }
    }
}