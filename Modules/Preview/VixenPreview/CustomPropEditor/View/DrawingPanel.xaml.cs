using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;
using VixenModules.Preview.VixenPreview.CustomPropEditor.ViewModel;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.View
{
    /// <summary>
    /// Interaction logic for DrawingPanel.xaml
    /// </summary>
    public partial class DrawingPanel : UserControl
    {

        public DrawingPanel()
        {
            InitializeComponent();
           
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
	        var el = sender as FrameworkElement;
			var model = DataContext as PropEditorViewModel;
	        if (model != null && el != null)
	        {
		        var node = (LightNode)el.DataContext;
				model.MoveLight(node, e.HorizontalChange, e.VerticalChange);
			}
		}

    }
}
