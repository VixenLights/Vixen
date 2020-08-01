using System;
using System.Collections;
using System.Collections.Generic;
using Vixen.Data.Value;
using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[Serializable] 
	[DataContract]
	[KnownType(typeof (PreviewLine))]
	[KnownType(typeof (PreviewEllipse))]
	[KnownType(typeof (PreviewArch))]
	[KnownType(typeof (PreviewRectangle))]
	[KnownType(typeof (PreviewSingle))]
	[KnownType(typeof (PreviewEllipse))]
	[KnownType(typeof (PreviewTriangle))]
	[KnownType(typeof (PreviewNet))]
	[KnownType(typeof (PreviewFlood))]
	[KnownType(typeof (PreviewCane))]
	[KnownType(typeof (PreviewStar))]
    [KnownType(typeof (PreviewStarBurst))]
    [KnownType(typeof (PreviewMegaTree))]
	[KnownType(typeof (PreviewCustom))]
	[KnownType(typeof (PreviewPixelGrid))]
    [KnownType(typeof (PreviewIcicle))]
    [KnownType(typeof(PreviewPolyLine))]
    [KnownType(typeof(PreviewMultiString))]
	[KnownType(typeof(PreviewCustomProp))]
	public class DisplayItem : IHandler<IIntentState<LightingValue>>, IHandler<IIntentState<CommandValue>>, IDisposable, IEnumerable<DisplayItem>, ICloneable
	{
		private PreviewBaseShape _shape;

		public DisplayItem()
		{
			_shape = new PreviewLine(new PreviewPoint(1, 1), new PreviewPoint(10, 10), 1, null, ZoomLevel);
		}

		[DataMember]
		public PreviewBaseShape Shape
		{
			get { return _shape; }
			set { _shape = value; }
		}

		public void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected, bool forceDraw)
		{
			_shape.Draw(fp, editMode, highlightedElements, selected, forceDraw);
		}

		public void DrawInfo(Graphics g)
		{
			if (Shape.Pixels.Count > 0) 
			{
				int margin = 1;
				string info;
				info = "Z:" + Shape.Pixels[0].Z;
				Font font = new Font(SystemFonts.MessageBoxFont.FontFamily, 7);
				SizeF textSize = g.MeasureString(info, font);
				Rectangle rect = new Rectangle(Shape.Left, Shape.Top, (int)textSize.Width + (margin * 2), (int)textSize.Height + (margin * 2));

				Rectangle propRect = new Rectangle(Shape.Left, Shape.Top, Shape.Right - Shape.Left, Shape.Bottom - Shape.Top);
				g.DrawRectangle(Pens.LightBlue, propRect);

				g.FillRectangle(Brushes.White, rect);
				g.DrawString(info, font, Brushes.Black, new PointF(Shape.Left+margin, Shape.Top+margin));
			}
		}

		public void Handle(IIntentState<LightingValue> state)
		{
			//System.Drawing.Color color = state.GetValue().GetOpaqueIntensityAffectedColor();
			//AddColorToNode(Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		public void Handle(IIntentState<CommandValue> state)
		{
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_shape != null)
					_shape.Dispose();
			}
			_shape = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		private double _zoomLevel = 1;
		public double ZoomLevel
		{
			get
			{
				return _zoomLevel;
			}
			set
			{
				if (value > 0)
					_zoomLevel = value;
				_zoomLevel = value;
				Shape.ZoomLevel = _zoomLevel;
			}
		}

		public IEnumerator<DisplayItem> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public object Clone()
		{
			DisplayItem item = (DisplayItem) MemberwiseClone();
			item.Shape = Shape.Clone() as PreviewBaseShape;
			return item;
		}
	}
}