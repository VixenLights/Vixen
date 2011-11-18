namespace VixenModules.App.DisplayPreview.Model
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Model.Shapes;

    [DataContract]
    [KnownType(typeof(OutlinedCircle))]
    [KnownType(typeof(SolidCircle))]
    [KnownType(typeof(Line))]
    [KnownType(typeof(SolidRectangle))]
    [KnownType(typeof(OutlinedRectangle))]
    [KnownType(typeof(SolidStar))]
    [KnownType(typeof(OutlinedStar))]
    [KnownType(typeof(SolidTriangle))]
    [KnownType(typeof(OutlinedTriangle))]
    [KnownType(typeof(Arc))]
    public class NodeLayout : INotifyPropertyChanged
    {
        private Color _nodeColor;

        private Guid _nodeId;

        private string _nodeName;

        private int _height;

        private double _leftOffset;

        private IShape _shape;

        private double _topOffset;

        private int _width;

        public NodeLayout()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Color NodeColor
        {
            get
            {
                return _nodeColor;
            }

            set
            {
                _nodeColor = value;
                _shape.NodeColor = _nodeColor;
                PropertyChanged.NotifyPropertyChanged("ChannelColor", this);
            }
        }

        [DataMember]
        public Guid NodeId
        {
            get
            {
                return _nodeId;
            }

            set
            {
                _nodeId = value;
                PropertyChanged.NotifyPropertyChanged("ChannelId", this);
            }
        }

        public string NodeName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_nodeName))
                {
                    var channel = Node;
                    if (channel != null)
                    {
                        _nodeName = channel.Name;
                    }
                }

                return _nodeName;
            }
        }

        public Brush DisplayColor
        {
            get
            {
                if (Node.IsRgbNode())
                {
                    var brush = new LinearGradientBrush();
                    brush.GradientStops.Add(new GradientStop(Colors.Red, 0));
                    brush.GradientStops.Add(new GradientStop(Colors.Green, .5));
                    brush.GradientStops.Add(new GradientStop(Colors.Blue, 1));
                    return brush;
                }

                return new SolidColorBrush(Colors.Aqua);
            }
        }

        [DataMember]
        public int Height
        {
            get
            {
                if (_height <= 0)
                {
                    _height = Preferences.DefaultSettings.ChannelHeightDefault;
                }

                return _height;
            }

            set
            {
                _height = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("Height", this);
            }
        }

        [DataMember]
        public double LeftOffset
        {
            get
            {
                return _leftOffset;
            }

            set
            {
                _leftOffset = value;
                PropertyChanged.NotifyPropertyChanged("LeftOffset", this);
            }
        }

        [DataMember]
        public IShape Shape
        {
            get
            {
                return _shape;
            }

            set
            {
                _shape = value;
                PropertyChanged.NotifyPropertyChanged("Shape", this);
            }
        }

        [DataMember]
        public double TopOffset
        {
            get
            {
                return _topOffset;
            }

            set
            {
                _topOffset = value;
                PropertyChanged.NotifyPropertyChanged("TopOffset", this);
            }
        }

        [DataMember]
        public int Width
        {
            get
            {
                if (_width <= 0)
                {
                    _width = Preferences.DefaultSettings.ChannelWidthDefault;
                }

                return _width;
            }

            set
            {
                _width = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        private ChannelNode Node
        {
            get
            {
                return VixenSystem.Nodes.GetAllNodes().FirstOrDefault(x => x.Id == NodeId);
            }
        }

        public NodeLayout Clone()
        {
            return new NodeLayout
                   {
                       TopOffset = TopOffset, 
                       LeftOffset = LeftOffset, 
                       NodeId = NodeId,
                       Height = Height,
                       Width = Width,
                       NodeColor = NodeColor,
                       Shape = Shape.Clone(),
                   };
        }

        private void Initialize()
        {
            Shape = new OutlinedCircle();
            NodeColor = Colors.Black;            
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }
    }
}
