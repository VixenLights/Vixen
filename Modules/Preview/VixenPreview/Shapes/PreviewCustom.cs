using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewCustom : PreviewLightBaseShape
	{
		[DataMember] private PreviewPoint _topLeft;
		private PreviewPoint topLeftStart;
		private int startWidth, startHeight;

		public override string TypeName => @"Custom Template";

		public PreviewCustom(PreviewPoint point, List<PreviewLightBaseShape> shapes)
		{
			_topLeft = point;
			if (shapes != null)
				Strings = shapes;

			Layout();
		}

		[OnDeserialized]
		private new void OnDeserialized(StreamingContext context)
		{
			Layout();
		}

		// We don't want the parent to do any processing, so override here
		[Browsable(false)]
		public override List<PreviewLightBaseShape> Strings
		{
			get
			{
				if (_strings == null)
					_strings = new List<PreviewBaseShape>();
				return _strings.Cast<PreviewLightBaseShape>().ToList();
			}
		}

		[Browsable(false)]
		public override int PixelSize
		{
			get { return base.PixelSize; }
			set
			{
				// Do nothing
			}
		}

		public override void ResizePixels()
		{
			// Gotta override this. We don't want to do anything
		}


		[CategoryAttribute("Position"),
		 DisplayName("Position"),
		 DescriptionAttribute("Top, left position of this custom object.")]
		public Point TopLeft
		{
			get
			{
				Point p = new Point(Left, Top);
				return p;
			}
			set { MoveTo(value.X, value.Y); }
		}

		[Browsable(false)]
		public Point BottomRight
		{
			get
			{
				Point p = new Point(Right, Bottom);
				return p;
			}
			set { Layout(); }
		}

		[Browsable(false)]
		public override int Top
		{
			get
			{
				int y = int.MaxValue;
				foreach (PreviewLightBaseShape shape in Strings) {
					y = Math.Min(y, shape.Top);
				}
				return y;
			}
			set
			{
				int delta = Top - value;
				foreach (PreviewLightBaseShape shape in Strings) {
					shape.Top -= delta;
				}
				Layout();
			}
		}

		[Browsable(false)]
		public override int Left
		{
			get
			{
				int x = int.MaxValue;
				foreach (PreviewLightBaseShape shape in Strings) {
					x = Math.Min(x, shape.Left);
				}
				return x;
			}
			set
			{
				int delta = Left - value;
				foreach (PreviewLightBaseShape shape in Strings) {
					shape.Left -= delta;
				}
				Layout();
			}
		}

        public override int Right
        {
            get
            {
	            int x = 0;
                foreach (PreviewLightBaseShape shape in Strings)
                {
                    x = Math.Max(x, shape.Right);
                }
                return x;
			}
        }

        public override int Bottom
        {
            get
            {
                int x = 0;
                foreach (PreviewLightBaseShape shape in Strings)
                {
                    x = Math.Max(x, shape.Bottom);
                }
                return x;
			}
        }
        
        public override void Match(PreviewBaseShape matchShape)
        {
            PreviewCustom shape = (matchShape as PreviewCustom);
            PixelSize = shape.PixelSize;
            // TODO
            Layout();
        }

		public override void Layout()
		{
			foreach (PreviewLightBaseShape shape in Strings) {
				shape.Layout();
			}

			SetPixelZoom();
		}

		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                          bool forceDraw, double zoomLevel)
		{
			foreach (PreviewLightBaseShape shape in Strings) {
				shape.Draw(fp, editMode, highlightedElements, selected, forceDraw, zoomLevel);
			}
			DrawSelectPoints(fp);
		}

		public override void MouseMove(int x, int y, int changeX, int changeY)
		{
            //PreviewPoint point = PointToZoomPoint(new PreviewPoint(x, y));
			// See if we're resizing
			if (_selectedPoint != null && _selectedPoint.PointType == PreviewPoint.PointTypes.Size)
			{
				double aspect = ((double) startWidth + (double) changeX)/(double) startWidth;
				//Resize(aspect);
                foreach (PreviewLightBaseShape shape in Strings)
                {
                    shape.ResizeFromOriginal(aspect);
                }
				MoveTo(topLeftStart.X, topLeftStart.Y);
				//This is still not great, but it acts a little better until I can put more time into it.
				_selectedPoint.X = Right;
				_selectedPoint.Y = Bottom;
				
			}
				// If we get here, we're moving
			else {
				_topLeft.X = topLeftStart.X + changeX;
				_topLeft.Y = topLeftStart.Y + changeY;
				SelectDragPoints();
				foreach (PreviewLightBaseShape shape in Strings) {
					shape.MouseMove(x, y, changeX, changeY);
				}
			}
		}

		public override void MouseUp(object sender, MouseEventArgs e)
		{
			SelectDragPoints();
			base.MouseUp(sender, e);
		}

		public override void SetSelectPoint(PreviewPoint point)
		{
			foreach (PreviewLightBaseShape shape in Strings) {
				shape.SetSelectPoint(null);
			}
			topLeftStart = new PreviewPoint(_topLeft.X, _topLeft.Y);
			startWidth = BottomRight.X - TopLeft.X;
			startHeight = BottomRight.Y - TopLeft.Y;
			_selectedPoint = point;
		}

		public override void Select(bool selectDragPoints)
		{
			base.Select(selectDragPoints);
		}

		public override void SelectDragPoints()
		{
			List<PreviewPoint> points = new List<PreviewPoint>();
			//points.Add(new PreviewPoint(TopLeft));
			points.Add(new PreviewPoint(BottomRight));
			SetSelectPoints(points, null);
		}

		public override bool PointInShape(PreviewPoint point)
		{
			//foreach (PreviewBaseShape shape in Strings) {
			//	if (shape.PointInShape(point))
			//		return true;
			//}
			PreviewPoint p = PointToZoomPoint(point);
			Rectangle r = new Rectangle(Left, Top, Right - Left, Bottom - Top);
			return r.Contains(p.X, p.Y);
		}

		public override void SelectDefaultSelectPoint()
		{
			_selectedPoint = _topLeft;
		}

		public override void MoveTo(int x, int y)
		{
			int deltaX = x - Left;
			int deltaY = y - Top;
			foreach (PreviewLightBaseShape shape in Strings) {
				shape.MoveTo(shape.Left + deltaX, shape.Top + deltaY);
			}
			_topLeft.X = x;
			_topLeft.Y = y;
			Layout();
		}

		public override void Resize(double aspect)
		{
			foreach (PreviewLightBaseShape shape in Strings) {
				shape.Resize(aspect);
			}
			Layout();
		}

		public override void ResizeFromOriginal(double aspect)
		{
			throw new NotImplementedException();
		}

		#region Overrides of PreviewBaseShape

		/// <inheritdoc />
		public override object Clone()
		{
			var newCustom = (PreviewCustom) MemberwiseClone();

			newCustom.Strings = new List<PreviewLightBaseShape>(Strings.Count);

			newCustom._topLeft = _topLeft.Copy();

			foreach (var previewBaseShape in Strings)
			{
				newCustom.Strings.Add((PreviewLightBaseShape)previewBaseShape.Clone());
			}


			return newCustom;
		}

		#endregion
	}
}