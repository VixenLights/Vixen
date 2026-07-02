using System.Windows;
using System.Windows.Forms.Integration;
using Common.Controls.ColorManagement.ColorModels;
using Common.ElementTagColorEditor.ViewModels;
using Vixen.Sys;
using Color = System.Drawing.Color;
using ColorPicker = Common.Controls.ColorManagement.ColorPicker.ColorPicker;

namespace Common.ElementTagColorEditor.Views
{
	/// <summary>
	/// A small standalone dialog for editing the <see cref="ElementTagDefinition.DisplayColor"/> of the built-in
	/// element tags (<c>Deprecated</c>, <c>Hidden</c>, <c>Prop</c>).
	/// </summary>
	/// <remarks>
	/// Whether this window persists its own changes on Save is decided entirely by the caller via
	/// <paramref name="saveOnClose"/> passed to the constructor - the window itself has no opinion on which host
	/// (Display Setup, Preview Setup, the Sequencer, or any future caller) opened it. A caller with its own
	/// OK/Cancel-gated save (Display Setup, Preview Setup) should pass <see langword="false"/> and let its own
	/// save path capture or discard the change; a caller with no other save path (the Sequencer) should pass
	/// <see langword="true"/>. Most callers should use the <see cref="ShowAsDialog"/> static helper rather than
	/// constructing this window directly, since it also keeps the WinForms/WPF interop plumbing
	/// (<see cref="ElementHost.EnableModelessKeyboardInterop"/>) out of the caller, letting a purely
	/// WinForms-hosting project (one without <c>UseWPF</c> set) open this window without needing any WPF
	/// assembly references of its own.
	/// </remarks>
	public partial class ElementTagColorEditorWindow : Window
	{
		private readonly bool _saveOnClose;
		private readonly ElementTagColorEditorViewModel _viewModel;

		public ElementTagColorEditorWindow(bool saveOnClose)
		{
			InitializeComponent();
			_saveOnClose = saveOnClose;
			_viewModel = new ElementTagColorEditorViewModel();
			DataContext = _viewModel;
		}

		/// <summary>
		/// Opens a modal <see cref="ElementTagColorEditorWindow"/> and returns whether the user saved.
		/// </summary>
		/// <param name="saveOnClose">
		/// <see langword="true"/> if the window itself should persist the change via
		/// <see cref="VixenSystem.SaveSystemConfigAsync"/> when Save is clicked (for a caller with no other
		/// save path, e.g. the Sequencer); <see langword="false"/> if the caller has its own OK/Cancel-gated
		/// save that will capture or discard the in-memory change itself (Display Setup, Preview Setup).
		/// </param>
		/// <returns><see langword="true"/> if the user clicked Save; <see langword="false"/> if they cancelled.</returns>
		public static bool ShowAsDialog(bool saveOnClose)
		{
			var window = new ElementTagColorEditorWindow(saveOnClose);
			ElementHost.EnableModelessKeyboardInterop(window);
			return window.ShowDialog() == true;
		}

		private void ChooseColorButton_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as FrameworkElement)?.DataContext is not TagColorItem item)
				return;

			Color currentColor = Color.FromArgb(item.Color.R, item.Color.G, item.Color.B);
			using ColorPicker colorPicker = new ColorPicker();
			colorPicker.Color = XYZ.FromRGB(currentColor);
			if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Color chosen = colorPicker.Color.ToRGB();
				item.Color = System.Windows.Media.Color.FromRgb(chosen.R, chosen.G, chosen.B);
			}
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.CommitChanges();
			if (_saveOnClose)
			{
				await VixenSystem.SaveSystemConfigAsync();
			}
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
