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
			Loaded += (_, _) =>
			{
				NameBox.SelectAll();
				NameBox.Focus();
			};
		}
	}
}
