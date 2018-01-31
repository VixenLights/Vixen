using System.Windows;
using Vixen.Extensions;

namespace VixenModules.App.CustomPropEditor.View
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
        }
    }
}
