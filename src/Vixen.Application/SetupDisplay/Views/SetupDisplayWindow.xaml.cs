using System.Windows;
using Orc.Theming;
using VixenApplication.SetupDisplay.ViewModels;
using WPFCommon.Extensions;

namespace VixenApplication.SetupDisplay.Views
{
	public partial class SetupDisplayWindow: Window
	{
        public SetupDisplayWindow()
		{
            ThemeManager.Current.SynchronizeTheme();
			InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            DataContext = new SetupDisplayViewModel();
        }
	}
}
