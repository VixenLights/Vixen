using System.Windows;
using Vixen.Extensions;
using VixenModules.App.CustomPropEditor.ViewModels;

namespace VixenModules.App.CustomPropEditor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CustomPropEditorWindow : Window
    {
        private readonly PropEditorViewModel _viewModel;
        public CustomPropEditorWindow()
        {
            InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            _viewModel = new PropEditorViewModel();
            DataContext = _viewModel;
        }
    }
}
