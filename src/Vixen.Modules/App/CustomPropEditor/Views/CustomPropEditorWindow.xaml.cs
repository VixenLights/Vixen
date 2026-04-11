using System.Windows;
using VixenModules.App.CustomPropEditor.ViewModels;
using WPFCommon.Extensions;

namespace VixenModules.App.CustomPropEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CustomPropEditorWindow : Window
    {
	    public CustomPropEditorWindow()
        {
	        InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            var viewModel = new PropEditorViewModel();
            DataContext = viewModel;
        }
    }
}
