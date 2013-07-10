using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Execution.Context;
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
		private Color color = Color.White;
		//public Color editColor = Color.White;
		private Brush brush;
		private int _x = 0;
		private int _y = 0;
		private int _z = 0;
		private int size = 3;
		private Rectangle drawArea;
		private ElementNode _node = null;
		private Guid _nodeId;
		private int _maxAlpha = 255;

		//static Hashtable brushes = new Hashtable();
		//static Dictionary<Int32, Brush> brushes = new Dictionary<Int32, Brush>();

		//public static Hashtable IntentNodeToColor = new Hashtable();

		[XmlIgnore] public static Dictionary<ElementNode, Color> IntentNodeToColor = new Dictionary<ElementNode, Color>();
		//public static Dictionary<Guid, IIntentStates> intentStates = new Dictionary<Guid, IIntentStates>();

		public PreviewPixel()
		{
		}

		public PreviewPixel(int xPosition, int yPositoin, int zPosition, int pixelSize)
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
			//editColor = Color.White;
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
			set { _nodeId = value; }
		}

		public ElementNode Node
		{
			get
			{
				if (_node == null) {
					_node = VixenSystem.Nodes.GetElementNode(NodeId);
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
			}
		}

		//public void SetGraphics(Graphics Graphics)
		//{
		//    g = Graphics;
		//}

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
				//IIntentStates nodeIntentStates;
				//if (intentStates.TryGetValue(Node.Id, out nodeIntentStates))
				//{
				//    if (nodeIntentStates != null)
				//    {
				//        foreach (IIntentState<LightingValue> intentState in nodeIntentStates)
				//        {
				//            Draw(fp, intentState.GetValue().GetAlphaChannelIntensityAffectedColor());
				//        }
				//    }
				//}
				//Color color;
				if (PreviewPixel.IntentNodeToColor.TryGetValue(Node, out color)) {
					//if (nodeIntentStates != null)
					//{
					//foreach (IIntentState<LightingValue> intentState in nodeIntentStates)
					//{
					Draw(fp, color);
					//}
					//}
				}
			}
		}

		public void Draw(FastPixel.FastPixel fp, Color newColor)
		{
			fp.DrawCircle(drawArea, newColor);
		}

		public Color GetColorForIntents(IIntentStates states)
		{
			Color c = Color.Empty;

			foreach (IIntentState<LightingValue> intentState in states)
			{
				Color intentColor = ((IIntentState<LightingValue>)intentState).GetValue().GetAlphaChannelIntensityAffectedColor();
				c = Color.FromArgb(Math.Max(c.A, intentColor.A),
								   Math.Max(c.R, intentColor.R),
								   Math.Max(c.G, intentColor.G),
								   Math.Max(c.B, intentColor.B)
								  );
			}

			if (c == Color.Empty || c == Color.Black)
				c = Color.Transparent;

			return c;
		}

		public bool IsDiscretePixel()
		{
			if (_node != null && _node.Properties.Contains(VixenModules.Property.Color.ColorDescriptor._typeId))
			{
				return true;
			}
			return false;
		}

        public void Draw(FastPixel.FastPixel fp, IIntentStates states)
        {
			Rectangle drawRect = new Rectangle(drawArea.X, drawArea.Y, drawArea.Width, drawArea.Height);
			int col = 0;

			//if (states.Count > 0)
			//	Console.WriteLine(states.Count);

			if (IsDiscretePixel())
			{
				foreach (IIntentState<LightingValue> intentState in states)
				{
					Color c = ((IIntentState<LightingValue>)intentState).GetValue().GetAlphaChannelIntensityAffectedColor();
					if (col == 0)
					{
						drawRect.X = drawArea.X;
						col = 1;
					}
					else
					{
						if (drawArea.Width == 3)
							drawRect.X = drawArea.X + 2;
						else if (drawArea.Width == 4)
							drawRect.X = drawArea.X + 2;
						else
							drawRect.X += drawArea.Width - 2;

						col = 0;
					}

					fp.DrawCircle(drawRect, c);

					if (col == 0)
						if (drawArea.Height == 3)
							drawRect.Y += 2;
						else if (drawArea.Height == 4)
							drawRect.Y += 2;
						else
							drawRect.Y += drawArea.Height - 2;
				}
			}
			else
			{
				Color intentColor = Vixen.Intent.ColorIntent.GetColorForIntents(states);
				fp.DrawCircle(drawRect, intentColor);
			}
        }
        
        ~PreviewPixel()
		{
			Dispose(false);
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