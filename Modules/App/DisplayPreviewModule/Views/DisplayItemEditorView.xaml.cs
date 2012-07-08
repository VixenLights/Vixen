namespace VixenModules.App.DisplayPreview.Views
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    public partial class DisplayItemEditorView
    {
        public DisplayItemEditorView()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GridMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var depObject = sender as DependencyObject;
            if (depObject != null)
            {
                Selector.SetIsSelected(depObject, true);
            }
        }
    }
}
