using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows;
using Vixen.Sys;
using System.Xml.Serialization;
using Point = System.Windows.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewPixel : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Color color = Color.White;
		private int _x = 0;
		private int _y = 0;
		private int _z = 0;
		private int size = 3;
		private ElementNode _node = null;
		private Guid _nodeId;
		private int _maxAlpha = 255;
		private bool _isDiscreteColored = false;
		private DiscreteIntentHandler _discreteHandler;
		private FullColorIntentHandler _fullColorHandler;
		private int savedX;
		private int savedY;
		private int savedZ;
		private int savedPixelSize;
		private Point _location;

		//[XmlIgnore] public static Dictionary<ElementNode, Color> IntentNodeToColor = new Dictionary<ElementNode, Color>();

		public PreviewPixel()
		{
			_fullColorHandler = new FullColorIntentHandler();
			_discreteHandler = new DiscreteIntentHandler();
		}

		public PreviewPixel(int xPosition, int yPositoin, int zPosition, int pixelSize):this()
		{
			X = xPosition;
			Y = yPositoin;
			Z = zPosition;
			size = pixelSize;
			Resize();
		}

		public PreviewPixel(Point location, int pixelSize) : this()
		{
			IsHighPrecision = true;
			Location = location;
			size = pixelSize;
			Resize();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (_isDiscreteColored)
			{
				_discreteHandler = new DiscreteIntentHandler();
			}
			else
			{
				_fullColorHandler = new FullColorIntentHandler();
			}
			
		}

		public PreviewPixel Clone()
		{
			PreviewPixel p = new PreviewPixel(X, Y, Z, size);
			p.color = color;
			p.Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
			p.Node = Node;
			p.NodeId = NodeId;
			p.MaxAlpha = _maxAlpha;
			if (IsHighPrecision)
			{
				p.IsHighPrecision = true;
				p.Location = new Point(Location.X, Location.Y);
			}

			return p;
		}

		public Rectangle Bounds { get; private set; }

		public int MaxAlpha
		{
			get
			{
				if (_maxAlpha == 0)
					_maxAlpha = 255;
				return _maxAlpha;
			}
			set { _maxAlpha = value; }
		}

		[DataMember(EmitDefaultValue = false)]
		public Guid NodeId
		{
			get { return _nodeId; }
			set
			{
				_nodeId = value;
				_node = VixenSystem.Nodes.GetElementNode(NodeId);
				TestForDiscrete();
			}
		}

		public ElementNode Node
		{
			get
			{
				if (_node == null) {
					_node = VixenSystem.Nodes.GetElementNode(NodeId);
					TestForDiscrete();
				}
				return _node;
			}
			set
			{
				if (value == null)
					NodeId = Guid.Empty;
				else
					NodeId = value.Id;
				_node = value;
				TestForDiscrete();
			}

		}

		internal void TestForDiscrete()
		{
			_isDiscreteColored = _node != null && Property.Color.ColorModule.isElementNodeDiscreteColored(_node);
			if (_isDiscreteColored)
			{
				_fullColorHandler = null;
				if (_discreteHandler == null)
				{
					_discreteHandler = new DiscreteIntentHandler();
				}
			}
			else
			{
				_discreteHandler = null;
				if (_fullColorHandler == null)
				{
					_fullColorHandler = new FullColorIntentHandler();
				}
			}
		}

		public void Resize()
		{
			if (IsHighPrecision)
			{
				Bounds = new Rectangle((int)Math.Round(Location.X), (int)Math.Round(Location.Y), size, size);
			}
			else
			{
				Bounds = new Rectangle(X, Y, size, size);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public bool IsHighPrecision { get; set; }

		/// <summary>
		/// This location property is for shapes that need to maintain a higher precision location.
		/// you must set IsHighPrecision for this to be utilized.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public Point Location
		{
			get { return _location; }
			set
			{
				_location = value;
				Resize();
			}
		}

		public PreviewPoint Point
		{
			get
			{
				if (IsHighPrecision)
				{
					return new PreviewPoint((int)Location.X, (int)Location.Y);
				}
				else
				{
					return new PreviewPoint(X, Y);
				}

			}
		}


		public int X
		{
			get { return _x; }
			set
			{
				_x = value;
				Resize();
			}
		}

		public int Y
		{
			get { return _y; }
			set
			{
				_y = value;
				Resize();
			}
		}

		public int Z
		{
			get { return _z; }
			set
			{
				_z = value;
				Resize();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int PixelSize
		{
			get { return size; }
			set
			{
				size = value;
				Resize();
			}
		}

		public Color PixelColor
		{
			get { return color; }
			set { color = value; }
		}

		internal bool IsDiscreteColored
		{
			get { return _isDiscreteColored; }
			set { _isDiscreteColored = value; }
		}

		public void Draw(Graphics graphics, Color c)
		{
			graphics.FillEllipse(new SolidBrush(c), Bounds);
		}

		public void Draw(FastPixel.FastPixel fp, bool forceDraw)
		{
			if (forceDraw) {
				Draw(fp, color);
			}
		}

		public void Draw(FastPixel.FastPixel fp, Color newColor)
		{
			fp.DrawCircle(Bounds, newColor);
		}

        public void Draw(FastPixel.FastPixel fp, IIntentStates states, double zoomLevel)
        {
			
			if(_isDiscreteColored)
			{
				int col = 1;
				int zoomedX = (int) (Bounds.X * zoomLevel);
				Rectangle drawRect = new Rectangle(zoomedX, (int)(Bounds.Y * zoomLevel), Bounds.Width, Bounds.Height);
				// Get states for each color
				List<Color> colors = _discreteHandler.GetAlphaAffectedColor(states);
				foreach (Color c in colors)
				{
					if (c != Color.Transparent && c.A > byte.MinValue)
					{
						fp.DrawCircle(drawRect, c);

						if (col % 2 == 0)
						{
							drawRect.Y += PixelSize;
							drawRect.X = zoomedX;
						} else
						{
							drawRect.X = zoomedX + PixelSize;
						}

						col++;
					}
				}
			}
			else
			{
				var state = states.FirstOrDefault();
				if (state != null)
				{
					Color intentColor = _fullColorHandler.GetFullColor(state);
					if (intentColor.A > 0)
					{
						Rectangle drawRect = zoomLevel != 1?new Rectangle((int)(Bounds.X * zoomLevel), (int)(Bounds.Y * zoomLevel), Bounds.Width, Bounds.Height):Bounds;
						fp.DrawCircle(drawRect, intentColor);
					}
			
				}
				
			}
        }

		[OnSerializing]
		void OnSerializing(StreamingContext context)
		{
			if (!IsHighPrecision)
			{
				savedX = X;
				savedY = Y;
				savedZ = Z;
				savedPixelSize = PixelSize;
				X = default(int); // will not be serialized
				Y = default(int); // will not be serialized
				Z = default(int); // will not be serialized
				PixelSize = default(int); // will not be serialized
			}
			
		}

		[OnSerialized]
		void OnSerialized(StreamingContext context)
		{
			if (!IsHighPrecision)
			{
				X = savedX;
				Y = savedY;
				Z = savedZ;
				PixelSize = savedPixelSize;
			}
			
		}


		public Color GetFullColor(IIntentStates states)
		{
			var state = states[0];
			if (state != null)
			{
				return _fullColorHandler.GetFullColor(state);
			}
			return Color.Empty;
		}

		public List<Color> GetDiscreteColors(IIntentStates states)
		{
			// Get states for each color
			List<Color> colors = _discreteHandler.GetAlphaAffectedColor(states);
			return colors;
		}

		protected void Dispose(bool disposing)
		{
			_node = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}