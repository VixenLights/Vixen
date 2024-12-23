using Vixen.Module.OutputFilter;
using Vixen.Sys;

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
		void UnpatchControllers();

		Control SetupPatchingControl { get; }
		DisplaySetup MasterForm { get; set; }
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
