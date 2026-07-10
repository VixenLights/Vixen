using System.Windows.Input;
using System.Windows.Threading;
using VixenModules.Property.State.Setup.ViewModels;
using WPFCommon.Extensions;

namespace VixenModules.Property.State.Setup.Views
{
	/// <summary>
	/// Interaction logic for StateDefinitionNameDialogView.xaml.
	/// </summary>
	public partial class StateDefinitionNameDialogView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionNameDialogView"/> class.
		/// </summary>
		/// <param name="viewModel">The dialog view model.</param>
		public StateDefinitionNameDialogView(StateDefinitionNameDialogViewModel viewModel)
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
			DataContext = viewModel;
			ContentRendered += OnContentRendered;
		}

		private void OnContentRendered(object? sender, EventArgs e)
		{
			ContentRendered -= OnContentRendered;
			Dispatcher.BeginInvoke(FocusNameBox, DispatcherPriority.ContextIdle);
		}

		private void FocusNameBox()
		{
			NameBox.Focus();
			Keyboard.Focus(NameBox);
		}
	}
}
