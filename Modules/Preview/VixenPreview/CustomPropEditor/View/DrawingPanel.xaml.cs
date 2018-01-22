using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Controls;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.View
{
    /// <summary>
    /// Interaction logic for DrawingPanel.xaml
    /// </summary>
    public partial class DrawingPanel : UserControl
    {

        private Canvas _canvas;
        public DrawingPanel()
        {
            InitializeComponent();
            Loaded += DrawingPanel_Loaded;
        }

        private void DrawingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _canvas = (Canvas)Template.FindName("PropCanvas", this);
            LightPaths = new ObservableCollection<LightNode>();
            LightPaths.Add(new LightNode(new System.Drawing.Point(10, 10), 3));
            LightPaths.Add(new LightNode(new System.Drawing.Point(20, 10), 3));
            LightPaths.Add(new LightNode(new System.Drawing.Point(30, 10), 3));
            LightPaths.Add(new LightNode(new System.Drawing.Point(40, 10), 3));
            //_canvas.MouseMove += _canvas_MouseMove; ;
            //_canvas.MouseDown += CanvasOnMouseDown;
            //_canvas.MouseLeftButtonDown += CanvasOnMouseLeftButtonDown;
            //OnComponentChanged();
            //OnGradientValueChanged();
        }

        public ObservableCollection<LightNode> LightPaths { get; set; }
    }
}
