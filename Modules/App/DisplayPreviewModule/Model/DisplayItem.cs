namespace VixenModules.App.DisplayPreview.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Behaviors;

    [DataContract]
    public class DisplayItem : INotifyPropertyChanged
    {
        private ObservableCollection<ChannelLocation> _channelLocations;
        private int _height;
        private bool _isUnlocked;
        private int _leftOffset;
        private string _name;
        private IDropTarget _target;
        private int _topOffset;
        private int _width;

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public ObservableCollection<ChannelLocation> ChannelLocations
        {
            get
            {
                return _channelLocations ?? (_channelLocations = new ObservableCollection<ChannelLocation>());
            }

            private set
            {
                _channelLocations = value;
                PropertyChanged.NotifyPropertyChanged("ChannelLocations", this);
            }
        }

        [DataMember]
        public int Height
        {
            get
            {
                if (_height <= 0)
                {
                    _height = Preferences.DefaultSettings.DisplayItemHeightDefault;
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
        public bool IsUnlocked
        {
            get
            {
                return _isUnlocked;
            }

            set
            {
                _isUnlocked = value;
                PropertyChanged.NotifyPropertyChanged("IsUnlocked", this);
            }
        }

        [DataMember]
        public int LeftOffset
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
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_name))
                {
                    _name = "New Display Item";
                }

                return _name;
            }

            set
            {
                _name = value;
                PropertyChanged.NotifyPropertyChanged("Name", this);
            }
        }

        public IDropTarget Target
        {
            get
            {
                return _target ?? (_target = new DropTarget<ChannelNode>(GetDropEffects, Drop));
            }
        }

        [DataMember]
        public int TopOffset
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
                    _width = Preferences.DefaultSettings.DisplayItemWidthDefault;
                }

                return _width;
            }

            set
            {
                _width = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        public DisplayItem Clone()
        {
            var item = new DisplayItem
                       {
                           Width = Width, 
                           Height = Height, 
                           LeftOffset = LeftOffset, 
                           TopOffset = TopOffset, 
                           ChannelLocations =
                               new ObservableCollection<ChannelLocation>(
                               ChannelLocations.Select(channelLocation => channelLocation.Clone()).ToList()), 
                           IsUnlocked = IsUnlocked
                       };
            return item;
        }

        public void UpdateChannelColors(Dictionary<ChannelNode, Color> colorsByChannel)
        {
            foreach (var colorByChannel in colorsByChannel)
            {
                var channelId = colorByChannel.Key.Id;
                var channelLocation = ChannelLocations.FirstOrDefault(x => x.ChannelId == channelId);
                if (channelLocation != null)
                {
                    var color = colorByChannel.Value;
                    channelLocation.ChannelColor = color == Colors.Black ? Colors.Transparent : color;
                }
            }
        }

        private static DragDropEffects GetDropEffects(ChannelNode channelNode)
        {
            return channelNode.IsLeaf || channelNode.IsRgbNode() ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void Drop(ChannelNode channelNode, Point point)
        {
            var channel = channelNode.Parents.FirstOrDefault(x => x.IsRgbNode()) ?? channelNode;
            var channelLocation = new ChannelLocation { LeftOffset = point.X, TopOffset = point.Y, ChannelId = channel.Id };
            ChannelLocations.Add(channelLocation);
        }
    }
}
