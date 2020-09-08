using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public class PreviewPoint
	{
		private int _x = 0;
		private int _y = 0;
		private PointTypes _pointType = PointTypes.None;

		public enum PointTypes
		{
			None,
			Size,
			SkewNS,
			SkewWE,
			Rotate
		}

		public PreviewPoint()
		{
		}

		public PointTypes PointType
		{
			get { return _pointType; }
			set { _pointType = value; }
		}

		public PreviewPoint(int x, int y, PointTypes type):this(x, y)
		{
			PointType = type;
		}

		public PreviewPoint(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public PreviewPoint(PreviewPoint pointToClone)
		{
			_x = pointToClone.X;
			_y = pointToClone.Y;
			_pointType = pointToClone.PointType;
		}

		public PreviewPoint(Point point)
		{
			_x = point.X;
			_y = point.Y;
		}

		[DataMember]
		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		[DataMember]
		public int Y
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
			return string.Format("{{{0},{1}}}", X,Y);
		}

		public PreviewPoint Copy()
		{
			return new PreviewPoint(this);
		}
	}
}