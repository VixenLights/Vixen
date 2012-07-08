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
        private int _height;

        private bool _isUnlocked = true;

        private int _leftOffset;

        private string _name;

        private ObservableCollection<NodeLayout> _nodeLayouts;

        private IDropTarget _target;

        private int _topOffset;

        private int _width;

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public int Height
        {
            get
            {
                if (this._height <= 0)
                {
                    this._height = Preferences.CurrentPreferences.DisplayItemHeightDefault;
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
        public bool IsUnlocked
        {
            get
            {
                return this._isUnlocked;
            }

            set
            {
                this._isUnlocked = value;
                this.PropertyChanged.NotifyPropertyChanged("IsUnlocked", this);
            }
        }

        [DataMember]
        public int LeftOffset
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
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._name))
                {
                    this._name = "New Display Item";
                }

                return this._name;
            }

            set
            {
                this._name = value;
                this.PropertyChanged.NotifyPropertyChanged("Name", this);
            }
        }

        [DataMember]
        public ObservableCollection<NodeLayout> NodeLayouts
        {
            get
            {
                return this._nodeLayouts ?? (this._nodeLayouts = new ObservableCollection<NodeLayout>());
            }

            private set
            {
                this._nodeLayouts = value;
                this.PropertyChanged.NotifyPropertyChanged("NodeLayouts", this);
            }
        }

        public IDropTarget Target
        {
            get
            {
                return this._target ?? (this._target = new DropTarget<ChannelNode>(GetDropEffects, this.Drop));
            }
        }

        [DataMember]
        public int TopOffset
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
                    this._width = Preferences.CurrentPreferences.DisplayItemWidthDefault;
                }

                return this._width;
            }

            set
            {
                this._width = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("Width", this);
            }
        }

        public DisplayItem Clone()
        {
            var item = new DisplayItem
                {
                    Width = this.Width,
                    Height = this.Height,
                    LeftOffset = this.LeftOffset,
                    TopOffset = this.TopOffset,
                    NodeLayouts = new ObservableCollection<NodeLayout>(this.NodeLayouts.Select(x => x.Clone()).ToList()),
                    IsUnlocked = this.IsUnlocked,
                    Name = this.Name
                };
            return item;
        }

        public void ResetColor(bool isRunning)
        {
            foreach (var nodeLayout in this.NodeLayouts)
            {
                nodeLayout.ResetColor(isRunning);
            }
        }

        public void UpdateChannelColors(Dictionary<ChannelNode, Color> colorsByChannel)
        {
            foreach (var colorByChannel in colorsByChannel)
            {
                var channelId = colorByChannel.Key.Id;
                var nodeLayout = this.NodeLayouts.FirstOrDefault(x => x.NodeId == channelId);
                if (nodeLayout != null)
                {
                    var color = colorByChannel.Value;
                    color = color == Colors.Black ? Colors.Transparent : color;
                    nodeLayout.SetNodeBrush(color.AsBrush());
                }
            }
        }

        private static DragDropEffects GetDropEffects(ChannelNode channelNode)
        {
            return channelNode != null && (channelNode.IsLeaf || channelNode.IsRgbNode())
                       ? DragDropEffects.Move
                       : DragDropEffects.None;
        }

        private void Drop(ChannelNode channelNode, Point point)
        {
            var channel = channelNode.Parents.FirstOrDefault(x => x.IsRgbNode()) ?? channelNode;
            var channelLocation = new NodeLayout { LeftOffset = point.X, TopOffset = point.Y, NodeId = channel.Id };
            this.NodeLayouts.Add(channelLocation);
        }
    }
}