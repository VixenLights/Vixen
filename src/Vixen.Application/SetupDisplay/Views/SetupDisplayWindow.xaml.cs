using System.Windows;
using VixenApplication.SetupDisplay.ViewModels;
using WPFCommon.Extensions;

namespace VixenApplication.SetupDisplay.Views
{
	public partial class SetupDisplayWindow: Window
	{
        public SetupDisplayWindow()
		{
            InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            DataContext = new SetupDisplayViewModel();
        }
	}
}
