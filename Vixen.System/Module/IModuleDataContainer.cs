using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module {
	public interface IModuleDataContainer {
		IModuleDataSet ModuleDataSet { get; }
	}
}
