using System.Windows.Forms.Integration;
using Common.ElementTagManager.ViewModels;

namespace Common.ElementTagManager.Views
{
	/// <summary>
	/// A small standalone dialog for editing the <see cref="Vixen.Sys.ElementTagDefinition.DisplayColor"/> of the
	/// built-in element tags (<c>Deprecated</c>, <c>Hidden</c>, <c>Prop</c>).
	/// </summary>
	/// <remarks>
	/// This is the baseline window for a future, broader Tag Manager feature (user-defined tag catalog
	/// management, etc.) - hence the project/window name is not scoped to "color editing" even though that
	/// is the only capability it exposes today. All interactive behavior lives in
	/// <see cref="ElementTagManagerWindowViewModel"/> and <see cref="TagColorItem"/>; this code-behind only
	/// wires up the view model and hosts the WinForms/WPF interop plumbing needed to show a modal WPF window
	/// from a WinForms-hosting caller (<see cref="ElementHost.EnableModelessKeyboardInterop"/>). Whether the
	/// window persists its own changes on Save is decided entirely by the caller via
	/// <paramref name="saveOnClose"/> passed to the constructor - a caller with its own OK/Cancel-gated save
	/// (Display Setup, Preview Setup) should pass <see langword="false"/> and let its own save path capture
	/// or discard the change; a caller with no other save path (the Sequencer) should pass
	/// <see langword="true"/>. Most callers should use the <see cref="ShowAsDialog"/> static helper rather
	/// than constructing this window directly, since it also keeps the WPF/Catel types out of the caller,
	/// letting a purely WinForms-hosting project (one without <c>UseWPF</c> set) open this window without
	/// needing any WPF assembly references of its own.
	/// </remarks>
	public partial class ElementTagManagerWindow
	{
		public ElementTagManagerWindow(bool saveOnClose)
		{
			InitializeComponent();
			DataContext = new ElementTagManagerWindowViewModel(saveOnClose);
		}

		/// <summary>
		/// Opens a modal <see cref="ElementTagManagerWindow"/> and returns whether the user saved.
		/// </summary>
		/// <param name="saveOnClose">
		/// <see langword="true"/> if the window itself should persist the change via
		/// <see cref="Vixen.Sys.VixenSystem.SaveSystemConfigAsync"/> when Save is clicked (for a caller with no other
		/// save path, e.g. the Sequencer); <see langword="false"/> if the caller has its own OK/Cancel-gated
		/// save that will capture or discard the in-memory change itself (Display Setup, Preview Setup).
		/// </param>
		/// <returns><see langword="true"/> if the user clicked Save; <see langword="false"/> if they cancelled.</returns>
		public static bool ShowAsDialog(bool saveOnClose)
		{
			var window = new ElementTagManagerWindow(saveOnClose);
			ElementHost.EnableModelessKeyboardInterop(window);
			return window.ShowDialog() == true;
		}
	}
}
