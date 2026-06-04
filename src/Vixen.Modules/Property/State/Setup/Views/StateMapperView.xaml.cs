using System.Windows.Controls;
using System.Windows.Threading;
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

		private void StateItemsGrid_Sorting(object sender, DataGridSortingEventArgs e)
		{
			Dispatcher.BeginInvoke(
				() =>
				{
					if (DataContext is StateMapperViewModel viewModel)
					{
						viewModel.SynchronizeStateItemOrder(
							StateItemsGrid.Items.OfType<StateItemViewModel>().ToList());
					}
				},
				DispatcherPriority.Background);
		}
	}
}
