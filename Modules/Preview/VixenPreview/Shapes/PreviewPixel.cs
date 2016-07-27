using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Execution.Context;
using Vixen.Intent;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;
using System.Xml.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewPixel : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Color color = Color.White;
		private Brush brush;
		private int _x = 0;
		private int _y = 0;
		private int _z = 0;
		private int size = 3;
		private Rectangle drawArea;
		private ElementNode _node = null;
		private Guid _nodeId;
		private int _maxAlpha = 255;
		private bool _isDiscreteColored = false;
		private DiscreteIntentHandler _discreteHandler;
		private FullColorIntentHandler _fullColorHandler;

		[XmlIgnore] public static Dictionary<ElementNode, Color> IntentNodeToColor = new Dictionary<ElementNode, Color>();

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
			brush = new SolidBrush(Color.White);
			Resize();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			_discreteHandler = new DiscreteIntentHandler();
			_fullColorHandler = new FullColorIntentHandler();
		}

		public PreviewPixel Clone()
		{
			PreviewPixel p = new PreviewPixel(X, Y, Z, size);
			p.color = color;
			p.drawArea = new Rectangle(drawArea.X, drawArea.Y, drawArea.Width, drawArea.Height);
			p.Node = Node;
			p.MaxAlpha = _maxAlpha;

			return p;
		}

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

		[DataMember]
		public Guid NodeId
		{
			get { return _nodeId; }
			set
			{
				_nodeId = value;
				_node = VixenSystem.Nodes.GetElementNode(NodeId);
				_isDiscreteColored = _node != null && Property.Color.ColorModule.isElementNodeDiscreteColored(_node);
			}
		}

		public ElementNode Node
		{
			get
			{
				if (_node == null) {
					_node = VixenSystem.Nodes.GetElementNode(NodeId);
					_isDiscreteColored = _node != null && Property.Color.ColorModule.isElementNodeDiscreteColored(_node);
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
				_isDiscreteColored = _node!=null && Property.Color.ColorModule.isElementNodeDiscreteColored(_node);
			}

		}

		public void Resize()
		{
			drawArea = new Rectangle(X, Y, size, size);
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

		[DataMember]
		public int Z
		{
			get { return _z; }
			set
			{
				_z = value;
				Resize();
			}
		}
		
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

		public void Draw(Graphics graphics, Color c)
		{
			graphics.FillEllipse(new SolidBrush(c), drawArea);
		}

		public void Draw(FastPixel.FastPixel fp, bool forceDraw)
		{
			if (forceDraw) {
				Draw(fp, color);
			}
			else if (Node != null) {
				if (PreviewPixel.IntentNodeToColor.TryGetValue(Node, out color)) {
					Draw(fp, color);
				}
			}
		}

		public void Draw(FastPixel.FastPixel fp, Color newColor)
		{
			fp.DrawCircle(drawArea, newColor);
		}

        public void Draw(FastPixel.FastPixel fp, IIntentStates states)
        {
			
			if(_isDiscreteColored)
			{
				int col = 1;
				Rectangle drawRect = new Rectangle(drawArea.X, drawArea.Y, drawArea.Width, drawArea.Height);
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
							drawRect.X = drawArea.X;
						} else
						{
							drawRect.X = drawArea.X + PixelSize;
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
						fp.DrawCircle(drawArea, intentColor);
					}
			
				}
				
			}
        }
        
		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (brush != null)
					brush.Dispose();
			}
			brush = null;
			_node = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}