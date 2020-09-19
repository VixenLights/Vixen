using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VixenModules.Editor.PolygonEditor.Adorners
{
	public class RubberbandAdorner : Adorner
	{
		private readonly Pen _rubberbandPen;
		private readonly Brush _rubberBandFillBrush;
		private Point? _endPoint;


		public RubberbandAdorner(Canvas canvas, Point? dragStartPoint)
			: base(canvas)
		{
			StartPoint = dragStartPoint;
			_endPoint = dragStartPoint;
			_rubberbandPen = new Pen(Brushes.Blue, .3);
			_rubberBandFillBrush = new SolidColorBrush(Colors.LightBlue);
			_rubberBandFillBrush.Opacity = .2;
		}

		protected Point? StartPoint { get; set; }

		public Point? EndPoint
		{
			get { return _endPoint; }
			set
			{
				_endPoint = value;
				InvalidateVisual();
			}
		}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);

			//// without a background the OnMouseMove event would not be fired !
			//// Alternative: implement a Canvas as a child of this adorner, like
			//// the ConnectionAdorner does.
			//dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

			if (StartPoint.HasValue && EndPoint.HasValue)
			{
				dc.DrawRectangle(_rubberBandFillBrush, _rubberbandPen, new Rect(StartPoint.Value, EndPoint.Value));
			}
		}

	}
}
