using System.Runtime.Serialization;
using System.Windows;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewDoublePoint
	{
		private double _x = 0;
		private double _y = 0;
		private Shapes.PreviewPoint.PointTypes _pointType = Shapes.PreviewPoint.PointTypes.None;

		public PreviewDoublePoint()
		{
		}

		public Shapes.PreviewPoint.PointTypes PointType
		{
			get { return _pointType; }
			set { _pointType = value; }
		}

		public PreviewDoublePoint(double x, double y, Shapes.PreviewPoint.PointTypes type) : this(x, y)
		{
			PointType = type;
		}

		public PreviewDoublePoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		public PreviewDoublePoint(Shapes.PreviewPoint pointToClone)
		{
			_x = pointToClone.X;
			_y = pointToClone.Y;
		}

		public PreviewDoublePoint(Point point)
		{
			_x = point.X;
			_y = point.Y;
		}

		[DataMember]
		public double X
		{
			get { return _x; }
			set { _x = value; }
		}

		[DataMember]
		public double Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public Point ToPoint()
		{
			return new Point(_x, _y);
		}

		public override string ToString()
		{
			return $"{{{X},{Y}}}";
		}
	}
}
