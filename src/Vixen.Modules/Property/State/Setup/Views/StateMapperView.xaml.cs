using VixenModules.Property.State.Setup.ViewModels;
using WPFCommon.Extensions;

namespace VixenModules.Property.State.Setup.Views
{
	/// <summary>
	/// Interaction logic for StateMapperView.xaml
	/// </summary>
	public partial class StateMapperView 
	{
		public StateMapperView(StateMapperViewModel viewModel)
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
			DataContext = viewModel;
		}
	}
}
