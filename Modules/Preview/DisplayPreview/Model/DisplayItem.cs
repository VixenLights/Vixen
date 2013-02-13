using System;
using System.Drawing;
using Vixen.Data.Value;

namespace VixenModules.Preview.DisplayPreview.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Behaviors;

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
                if (_height <= 0)
                {
                    _height = Preferences.CurrentPreferences.DisplayItemHeightDefault;
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

        [DataMember]
        public ObservableCollection<NodeLayout> NodeLayouts
        {
            get
            {
                return _nodeLayouts ?? (_nodeLayouts = new ObservableCollection<NodeLayout>());
            }

            private set
            {
                _nodeLayouts = value;
                PropertyChanged.NotifyPropertyChanged("NodeLayouts", this);
            }
        }

        public IDropTarget Target
        {
            get
            {
                return _target ?? (_target = new DropTarget<ElementNode>(GetDropEffects, Drop));
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
                    _width = Preferences.CurrentPreferences.DisplayItemWidthDefault;
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
                           NodeLayouts = new ObservableCollection<NodeLayout>(NodeLayouts.Select(x => x.Clone()).ToList()),
                           IsUnlocked = IsUnlocked,
                           Name = Name
                       };
            return item;
        }

        public void ResetColor(bool isRunning)
        {
            foreach (var nodeLayout in NodeLayouts)
            {
                nodeLayout.ResetColor(isRunning);
            }
        }

		public void UpdateElementColors(ElementIntentStates elementIntentStates)
		{
			
			foreach (var elementIntentState in elementIntentStates)
            {
				var elementId = elementIntentState.Key;
				Element element = VixenSystem.Elements.GetElement(elementId);
				if (element == null) continue;
				ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
				if (node == null) continue;

				var nodeLayout = NodeLayouts.FirstOrDefault(x => x.NodeId == node.Id);
				if (nodeLayout != null)
				{
					nodeLayout.ElementState = elementIntentState.Value;
				}
			}
        }

        private static DragDropEffects GetDropEffects(ElementNode elementNode)
        {
            return elementNode != null && (elementNode.IsLeaf) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void Drop(ElementNode elementNode, Point point)
        {
            var element = elementNode;
            var elementLocation = new NodeLayout { LeftOffset = point.X, TopOffset = point.Y, NodeId = element.Id };
            NodeLayouts.Add(elementLocation);
        }
    }
}
