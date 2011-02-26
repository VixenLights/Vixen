using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module.Output {
    public interface IOutput {
        // As defined by the owning controller.
        void SetOutputCount(int outputCount);
        // One command for each output as defined by the owning controller.
        void UpdateState(CommandData[] outputStates);
    }
}
