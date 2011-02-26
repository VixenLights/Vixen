using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;

namespace Vixen.Hardware {
    public interface IHardwareModule : IModuleInstance {
        void Startup();
        void Shutdown();
        void Pause();
        void Resume();
		bool IsRunning { get; }
        bool Setup();
    }
}
