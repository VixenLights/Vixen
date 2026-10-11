using System.Linq;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using VixenModules.App.Modeling.ViewModels;
using WinForms = System.Windows.Forms;

namespace VixenModules.App.Modeling.Views;

/// <summary>
/// Displays the shared wire diagram export options dialog.
/// </summary>
public partial class WireDiagramExportOptionsWindow
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WireDiagramExportOptionsWindow" /> class.
	/// </summary>
	public WireDiagramExportOptionsWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Shows the export options dialog modally and returns the selected options.
	/// </summary>
	/// <returns>The accepted options, or <see langword="null" /> when the dialog is cancelled or closed.</returns>
	public static WireDiagramExportOptions ShowAsDialog()
	{
		var window = new WireDiagramExportOptionsWindow();
		AssignActiveOwner(window);
		ElementHost.EnableModelessKeyboardInterop(window);

		if (window.ShowDialog() != true)
		{
			return null;
		}

		return window.DataContext is WireDiagramExportOptionsViewModel viewModel
			? viewModel.Options
			: null;
	}

	private static void AssignActiveOwner(Window window)
	{
		var activeWpfOwner = System.Windows.Application.Current?.Windows
			.Cast<Window>()
			.FirstOrDefault(candidate => candidate.IsActive);
		if (activeWpfOwner is not null)
		{
			window.Owner = activeWpfOwner;
			return;
		}

		var activeWinFormsOwner = WinForms.Form.ActiveForm;
		if (activeWinFormsOwner is { IsDisposed: false })
		{
			new WindowInteropHelper(window).Owner = activeWinFormsOwner.Handle;
		}
	}
}
