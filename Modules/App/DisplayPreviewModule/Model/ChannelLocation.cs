namespace VixenModules.App.DisplayPreview.Model
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using Vixen.Sys;

    [DataContract]
    public class ChannelLocation : INotifyPropertyChanged
    {
        private Color _channelColor;
        private Guid _channelId;
        private string _channelName;
        private int _height;
        private bool _isSelected;
        private double _leftOffset;
        private double _topOffset;
        private int _width;

        public ChannelLocation()
        {
            ChannelColor = Colors.Black;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Color ChannelColor
        {
            get
            {
                return _channelColor;
            }

            set
            {
                _channelColor = value;
                PropertyChanged.NotifyPropertyChanged("ChannelColor", this);
            }
        }

        [DataMember]
        public Guid ChannelId
        {
            get
            {
                return _channelId;
            }

            set
            {
                _channelId = value;
                PropertyChanged.NotifyPropertyChanged("ChannelId", this);
            }
        }

        public string ChannelName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_channelName))
                {
                    var channel = Channel;
                    if (channel != null)
                    {
                        _channelName = channel.Name;
                    }
                }

                return _channelName;
            }
        }

        public Brush DisplayColor
        {
            get
            {
                if (Channel.IsRgbNode())
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

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                PropertyChanged.NotifyPropertyChanged("IsSelected", this);
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

        private ChannelNode Channel
        {
            get
            {
                return VixenSystem.Nodes.GetAllNodes().FirstOrDefault(x => x.Id == ChannelId);
            }
        }

        public ChannelLocation Clone()
        {
            return new ChannelLocation { TopOffset = TopOffset, LeftOffset = LeftOffset, ChannelId = ChannelId };
        }
    }
}
