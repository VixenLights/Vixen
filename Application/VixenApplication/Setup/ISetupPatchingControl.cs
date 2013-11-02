using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Setup
{
	interface ISetupPatchingControl
	{
		event EventHandler<FiltersEventArgs> FiltersAdded;
		event EventHandler PatchingUpdated;

		void UpdateElementSelection(IEnumerable<ElementNode> nodes);
		void UpdateElementDetails(IEnumerable<ElementNode> nodes);
		void UpdateControllerSelection(ControllersAndOutputsSet controllersAndOutputs);
		void UpdateControllerDetails(ControllersAndOutputsSet controllersAndOutputs);

		Control SetupPatchingControl { get; }
	}

	public class FiltersEventArgs : EventArgs
	{
		public List<IOutputFilterModuleInstance> Filters;

		public FiltersEventArgs(IEnumerable<IOutputFilterModuleInstance> filters)
		{
			Filters = new List<IOutputFilterModuleInstance>(filters);
		}
	}

}
