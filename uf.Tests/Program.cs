// System
using System;

// OpenTK
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using uf.Utility.Globals;
using uf.Utility.Logging;

namespace uf
{
    class Entry
    {
        static void Main(string[] args)
        {
            EngineGlobals.Args = args;
            
            using Game window = new(new GameWindowSettings {
                UpdateFrequency = 120,
            }, new NativeWindowSettings {
                Title = "Unsigned Framework Dev.",
                Profile = ContextProfile.Core
            });
            window.VSync = VSyncMode.On;
            try {
                window.Run();
            }
            catch (Exception ex) {
                Logger.Log(new LogMessage(LogSeverity.Critical, "Uh huh. That's wierd.", ex));
            }
        }
    }
}
