namespace Vixen.Modules.DisplayPreviewModule.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media;
    using Vixen.Modules.DisplayPreviewModule.Behaviors;
    using Vixen.Sys;

    [DataContract]
    public class DisplayItem : INotifyPropertyChanged
    {
        private int _height;
        private bool _isUnlocked;
        private int _leftOffset;
        private string _name;
        private ChannelLocation _selectedChannelLocation;
        private IDropTarget _target;
        private int _topOffset;
        private int _width;

        public DisplayItem(
            int width, int height, int leftOffset, int topOffset, ObservableCollection<ChannelLocation> mappedChannels, bool isUnlocked)
        {
            ChannelLocations = mappedChannels;
            Height = height;
            LeftOffset = leftOffset;
            TopOffset = topOffset;
            Width = width;
            IsUnlocked = isUnlocked;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public ObservableCollection<ChannelLocation> ChannelLocations { get; private set; }

        [DataMember]
        public int Height
        {
            get
            {
                return _height;
            }

            set
            {
                _height = value;
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
                return _name;
            }

            set
            {
                _name = value;
                PropertyChanged.NotifyPropertyChanged("Name", this);
            }
        }

        public ChannelLocation SelectedChannelLocation
        {
            get
            {
                return _selectedChannelLocation;
            }

            set
            {
                if (_selectedChannelLocation != null)
                {
                    _selectedChannelLocation.IsSelected = false;
                }

                _selectedChannelLocation = value;
                if (_selectedChannelLocation != null)
                {
                    _selectedChannelLocation.IsSelected = true;
                }

                PropertyChanged.NotifyPropertyChanged("SelectedChannelLocation", this);
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
                return _width;
            }

            set
            {
                _width = value;
                PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        public DisplayItem Clone()
        {
            return new DisplayItem(
                Width, 
                Height, 
                LeftOffset, 
                TopOffset, 
                new ObservableCollection<ChannelLocation>(ChannelLocations.Select(channelLocation => channelLocation.Clone()).ToList()), 
                IsUnlocked);
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

        private void Drop(ChannelNode channelNode, Point point)
        {
            var channel = channelNode.Parents.FirstOrDefault(x => x.IsRgbNode());
            if (channel == null)
            {
                channel = channelNode;
            }

            var channelLocation = new ChannelLocation { LeftOffset = point.X, TopOffset = point.Y, ChannelId = channel.Id };
            ChannelLocations.Add(channelLocation);
        }

        private DragDropEffects GetDropEffects(ChannelNode channelNode)
        {
            if (ChannelLocations.Any(x => x.ChannelId == channelNode.Id || channelNode.Children.Any(y => y.Id == x.ChannelId)))
            {
                return DragDropEffects.None;
            }

            return DragDropEffects.Move;
        }
    }
}
