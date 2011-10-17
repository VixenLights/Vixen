using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Execution;

namespace Vixen.Sys {
    public interface IHardwareModule : IModuleInstance, IExecutionControl {
		bool IsRunning { get; }
        bool Setup();
    }
}
