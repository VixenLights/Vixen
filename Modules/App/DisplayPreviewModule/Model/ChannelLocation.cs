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
    public class ChannelLocation : INotifyPropertyChanged
    {
        private Color _channelColor;

        private Guid _channelId;

        private string _channelName;

        private int _height;

        private double _leftOffset;

        private IShape _shape;

        private double _topOffset;

        private int _width;

        public ChannelLocation()
        {
            this.Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Color ChannelColor
        {
            get
            {
                return this._channelColor;
            }

            set
            {
                this._channelColor = value;
                this.PropertyChanged.NotifyPropertyChanged("ChannelColor", this);
            }
        }

        [DataMember]
        public Guid ChannelId
        {
            get
            {
                return this._channelId;
            }

            set
            {
                this._channelId = value;
                this.PropertyChanged.NotifyPropertyChanged("ChannelId", this);
            }
        }

        public string ChannelName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._channelName))
                {
                    var channel = this.Channel;
                    if (channel != null)
                    {
                        this._channelName = channel.Name;
                    }
                }

                return this._channelName;
            }
        }

        public Brush DisplayColor
        {
            get
            {
                if (this.Channel.IsRgbNode())
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
                if (this._height <= 0)
                {
                    this._height = Preferences.DefaultSettings.ChannelHeightDefault;
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
        public IShape Shape
        {
            get
            {
                return this._shape;
            }

            set
            {
                this._shape = value;
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
                    this._width = Preferences.DefaultSettings.ChannelWidthDefault;
                }

                return this._width;
            }

            set
            {
                this._width = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        private ChannelNode Channel
        {
            get
            {
                return VixenSystem.Nodes.GetAllNodes().FirstOrDefault(x => x.Id == this.ChannelId);
            }
        }

        public ChannelLocation Clone()
        {
            return new ChannelLocation
                {
                   TopOffset = this.TopOffset, LeftOffset = this.LeftOffset, ChannelId = this.ChannelId 
                };
        }

        private void Initialize()
        {
            this.ChannelColor = Colors.Black;
            this.Shape = new OutlinedCircle();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}