namespace VixenModules.App.DisplayPreview.Views
{
    using System.Windows;

    public partial class NodeEditorView
    {
        public NodeEditorView()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
