using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.ViewModel;

namespace VixenModules.App.CustomPropEditor.View
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
	            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

	            if (el.RenderTransform != null)
	            {
	                dragDelta = el.RenderTransform.Transform(dragDelta);
	            }

                var node = (LightNode)el.DataContext;
				model.MoveLight(node, new Point(dragDelta.X + el.ActualHeight/2, dragDelta.Y + el.ActualWidth/2));
			}
		}

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            //var el = sender as HelixViewport3D;
            //if (el != null)
            //{
            //    var coord = e.GetPosition(el);
            //    //PointHitTestParameters hitParams = new PointHitTestParameters(coord);
            //    //VisualTreeHelper.HitTest(el, null, ResultCallback, hitParams);
            //    var hits = el.Viewport.FindHits(coord);

            //}

          
            base.OnMouseDown(e);
            
        }

        private HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            RayHitTestResult rayResult = result as RayHitTestResult;
            if (rayResult != null)
            {
                // Did we hit a MeshGeometry3D?
                RayMeshGeometry3DHitTestResult rayMeshResult =
                    rayResult as RayMeshGeometry3DHitTestResult;

                if (rayMeshResult != null)
                {
                    // Yes we did!
                }
            }

            return HitTestResultBehavior.Continue;
        }
    }
}
