using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using Vixen.Sys;
using VixenModules.Property.State.Setup.ViewModels;
using VixenModules.Property.State.Setup.Views;

namespace VixenModules.Property.State.Setup.Services;

/// <summary>
/// Displays the WPF State mapper dialog.
/// </summary>
internal sealed class StateMapperDialogService : IStateMapperDialogService
{
	/// <inheritdoc />
	public bool Show(IElementNode node, StateData data)
	{
		var viewModel = new StateMapperViewModel(node, data, new StateColorPickerService());
		var mapper = new StateMapperView(viewModel);
		if (Form.ActiveForm != null)
		{
			new WindowInteropHelper(mapper).Owner = Form.ActiveForm.Handle;
		}

		ElementHost.EnableModelessKeyboardInterop(mapper);
		return mapper.ShowDialog() == true;
	}
}
