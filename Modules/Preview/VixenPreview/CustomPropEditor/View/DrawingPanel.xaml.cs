using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;

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
            DataContext = this;
            LightNodes = new ObservableCollection<LightNode>();
            LightNodes.Add(new LightNode(new Point(10, 10), 6));
            LightNodes.Add(new LightNode(new Point(20, 10), 6));
            LightNodes.Add(new LightNode(new Point(30, 10), 6));
            LightNodes.Add(new LightNode(new Point(40, 10), 6));

            LightNodes.Add(new LightNode(new Point(40, 20), 6));
            Loaded += DrawingPanel_Loaded;
        }

        private void DrawingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //_canvas.MouseMove += _canvas_MouseMove; ;
            //_canvas.MouseDown += CanvasOnMouseDown;
            //_canvas.MouseLeftButtonDown += CanvasOnMouseLeftButtonDown;
            //OnComponentChanged();
            //OnGradientValueChanged();
        }

        public ObservableCollection<LightNode> LightNodes { get; set; }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {

            var canvas = FindParent<Canvas>((FrameworkElement)sender);
            var thumb = sender as Thumb;
            if (canvas != null && thumb != null)
            {
                
                var node = (LightNode)thumb.DataContext;
                var halfSize = node.Size / 2;

                var x = node.Center.X + e.HorizontalChange;
                if (x > canvas.Width - halfSize)
                {
                    x = canvas.Width - halfSize;
                }
                else if(x < halfSize)
                {
                    x = halfSize;
                }
                var y = node.Center.Y + e.VerticalChange;
                if (y > canvas.Height - halfSize)
                {
                    y = canvas.Height - halfSize;
                }
                else if (y < halfSize)
                {
                    y = halfSize;
                }

                Point p = new Point(x, y);
                node.Center = p;
            }
         
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
