using System.Collections.Generic;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;

namespace VixenModules.Preview.DisplayPreview.Model
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Model.Shapes;

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
    [KnownType(typeof(CandyCane))]
	[KnownType(typeof(UserDefinedShape))]
	public class NodeLayout : INotifyPropertyChanged, IHandler<IIntentState<LightingValue>>, IHandler<IIntentState<CommandValue>>
    {
        private int _height;

        private double _leftOffset;

        private Brush _nodeBrush;
    	private List<Color> _nodeColors;

        private Guid _nodeId;

        private string _nodeName;

        private IShape _shape;

        private double _topOffset;

        private int _width;

        public NodeLayout()
        {
            Initialize();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public int Height
        {
            get
            {
                if (_height <= 0)
                {
                    _height = Preferences.CurrentPreferences.ChannelHeightDefault;
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
                if (value != null)
                {
                    value.Brush = Colors.White.AsBrush();
                }
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
                    _width = Preferences.CurrentPreferences.ChannelWidthDefault;
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
                       _nodeBrush = _nodeBrush,
                       Shape = Shape.Clone(),
                   };
        }

        public void ResetColor(bool isRunning)
        {
			_nodeColors = new List<Color>();
            var brush = isRunning ? Colors.Transparent.AsBrush() : Colors.White.AsBrush();
            SetNodeBrush(brush);
        }

        public void SetNodeBrush(Brush brush)
        {
            _nodeBrush = brush;
            _shape.Brush = brush;
        }

		public void AddColorToNode(Color color)
		{
			// this was written to use a list of colors that it interates through whenever a new color gets added,
			// as I thought we needed the total number of colors for the calculations. Turns out we don't -- using
			// max values is fine -- so we can rewrite this without the list if needed.
			_nodeColors.Add(color);

			RGB result = new RGB(0, 0, 0);

			foreach (Color nodeColor in _nodeColors) {
				result.R = Math.Max((double)nodeColor.R / byte.MaxValue, result.R);
				result.G = Math.Max((double)nodeColor.G / byte.MaxValue, result.G);
				result.B = Math.Max((double)nodeColor.B / byte.MaxValue, result.B);
			}

			double intensity = HSV.FromRGB(result).V;

			SolidColorBrush brush = new SolidColorBrush(Color.FromArgb((byte)(intensity * byte.MaxValue), result.ToArgb().R, result.ToArgb().G, result.ToArgb().B));
			brush.Freeze();
			SetNodeBrush(brush);
		}

        private void Initialize()
        {
            Shape = new OutlinedCircle();
            SetNodeBrush(Colors.White.AsBrush());
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

		public IIntentStates ChannelState
		{
			set
			{
				foreach (IIntentState intentState in value)
				{
					intentState.Dispatch(this);
				}

			}
		}

    	public void Handle(IIntentState<LightingValue> state)
    	{
			System.Drawing.Color color = state.GetValue().GetOpaqueIntensityAffectedColor();
			AddColorToNode(Color.FromArgb(color.A, color.R, color.G, color.B));
    	}

    	#region Implementation of IHandler<in IIntentState<CommandValue>>

    	public void Handle(IIntentState<CommandValue> state)
    	{
			
    	}

    	#endregion
    }
}
