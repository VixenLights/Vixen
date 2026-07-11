using System.Windows.Forms.Integration;
using System.Windows.Interop;
using Common.Controls;
using Vixen.Rule;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using VixenModules.Property.State.Setup.Views;

namespace VixenModules.Property.State
{
	public class StateSetupHelper: IElementSetupHelper
	{
		#region Implementation of IElementSetupHelper

		public string HelperName { get { return "State Mapping"; } }
		
		public bool Perform(IEnumerable<IElementNode> nodes)
		{
			var selectedNodes = nodes.Take(2).ToList();
			if (selectedNodes.Count != 1)
			{
				MessageBoxForm mb = new MessageBoxForm("State Mapping can only be applied to one Element Node",
					"Too many nodes selected", MessageBoxButtons.OK, SystemIcons.Information);
				mb.ShowDialog();
				return false;
			}

			var data = new StateData();
			var node = selectedNodes[0];
			if (node.Properties.Contains(StateDescriptor.ModuleId))
			{
				if (node.Properties.Get(StateDescriptor.ModuleId)?.ModuleData is StateData existingData)
				{
					data = existingData;
				}
			}
			var vm = new StateMapperViewModel(node, data, new StateColorPickerService());
			var mapper = new StateMapperView(vm);
			if (Form.ActiveForm != null)
			{
				new WindowInteropHelper(mapper).Owner = Form.ActiveForm.Handle;
			}

			ElementHost.EnableModelessKeyboardInterop(mapper);
			var response = mapper.ShowDialog();
			if (response == true)
			{
				var sm = node.Properties.Add(StateDescriptor.ModuleId) as StateModule;
				sm?.ModuleData = data;
			}
			
			return response == true;
		}

		#endregion
	}
}