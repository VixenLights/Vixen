using System.Windows;
using WPFCommon.Extensions;

namespace VixenApplication.DisplaySetup.Views
{
	public partial class DisplaySetupWindow: Window
	{
		public DisplaySetupWindow()
		{
			InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            //_viewModel = new PropEditorViewModel();
           // DataContext = _viewModel;
		}
	}
}
