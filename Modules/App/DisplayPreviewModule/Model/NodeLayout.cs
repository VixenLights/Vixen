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
        private int _height;

        private double _leftOffset;

        private Brush _nodeBrush;

        private Guid _nodeId;

        private string _nodeName;

        private IShape _shape;

        private double _topOffset;

        private int _width;

        public NodeLayout()
        {
            this.Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public int Height
        {
            get
            {
                if (this._height <= 0)
                {
                    this._height = Preferences.CurrentPreferences.ChannelHeightDefault;
                }

                return this._height;
            }

            set
            {
                this._height = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("Height", this);
            }
        }

        [DataMember]
        public double LeftOffset
        {
            get
            {
                return this._leftOffset;
            }

            set
            {
                this._leftOffset = value;
                this.PropertyChanged.NotifyPropertyChanged("LeftOffset", this);
            }
        }

        [DataMember]
        public Guid NodeId
        {
            get
            {
                return this._nodeId;
            }

            set
            {
                this._nodeId = value;
                this.PropertyChanged.NotifyPropertyChanged("ChannelId", this);
            }
        }

        public string NodeName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._nodeName))
                {
                    var channel = this.Node;
                    if (channel != null)
                    {
                        this._nodeName = channel.Name;
                    }
                }

                return this._nodeName;
            }
        }

        [DataMember]
        public IShape Shape
        {
            get
            {
                return this._shape;
            }

            set
            {
                this._shape = value;
                if (value != null)
                {
                    value.Brush = this.Node.IsRgbNode() ? ColorManager.RgbBrush : Colors.White.AsBrush();
                }
                this.PropertyChanged.NotifyPropertyChanged("Shape", this);
            }
        }

        [DataMember]
        public double TopOffset
        {
            get
            {
                return this._topOffset;
            }

            set
            {
                this._topOffset = value;
                this.PropertyChanged.NotifyPropertyChanged("TopOffset", this);
            }
        }

        [DataMember]
        public int Width
        {
            get
            {
                if (this._width <= 0)
                {
                    this._width = Preferences.CurrentPreferences.ChannelWidthDefault;
                }

                return this._width;
            }

            set
            {
                this._width = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        private ChannelNode Node
        {
            get
            {
                return VixenSystem.Nodes.GetAllNodes().FirstOrDefault(x => x.Id == this.NodeId);
            }
        }

        public NodeLayout Clone()
        {
            return new NodeLayout
                {
                    TopOffset = this.TopOffset, 
                    LeftOffset = this.LeftOffset, 
                    NodeId = this.NodeId, 
                    Height = this.Height, 
                    Width = this.Width, 
                    _nodeBrush = this._nodeBrush, 
                    Shape = this.Shape.Clone(), 
                };
        }

        public void ResetColor(bool isRunning)
        {
            var brush = isRunning
                            ? Colors.Transparent.AsBrush()
                            : (this.Node.IsRgbNode() ? ColorManager.RgbBrush : Colors.White.AsBrush());
            this.SetNodeBrush(brush);
        }

        public void SetNodeBrush(Brush brush)
        {
            this._nodeBrush = brush;
            this._shape.Brush = brush;
        }

        private void Initialize()
        {
            this.Shape = new OutlinedCircle();
            this.SetNodeBrush(Colors.White.AsBrush());
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}