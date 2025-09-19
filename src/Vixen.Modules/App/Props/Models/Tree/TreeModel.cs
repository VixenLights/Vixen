#nullable enable
using System.Drawing;
using Vixen.Extensions;
using Vixen.Sys.Props.Model;
using VixenModules.App.Props.Models.Line;

namespace VixenModules.App.Props.Models.Tree
{
	[Serializable]
	public class TreeModel : BaseLightModel
	{
		private int _strings;
		private int _nodesPerString;
		private int _nodeSize;
		private int _topWidth;
		private int _topHeight;
		private int _baseHeight;
		private int _degreesCoverage;
		private int _degreesOffset;

		public TreeModel(int strings = 16, int nodesPerString = 50, int nodeSize = 2)
		{
			_topWidth = 20;
			_topHeight = _topWidth / 2;
			_baseHeight = 40;
			_degreesCoverage = 360;
			_strings = strings;
			_nodesPerString = nodesPerString;
			_nodeSize = nodeSize;
			Nodes = new(GetTreePoints());
			PropertyChanged += TreeModel_PropertyChanged;
		}

		public int Strings
		{
			get => _strings;
			set => SetProperty(ref _strings, value);
		}

		public int NodesPerString
		{
			get => _nodesPerString;
			set => SetProperty(ref _nodesPerString, value);
		}

		public int NodeSize
		{
			get => _nodeSize;
			set => SetProperty(ref _nodeSize, value);
		}

		public int TopWidth
		{
			get => _topWidth;
			set => SetProperty(ref _topWidth, value);
		}

		public int TopHeight
		{
			get => _topHeight;
			set => SetProperty(ref _topHeight, value);
		}

		public int BaseHeight
		{
			get => _baseHeight;
			set => SetProperty(ref _baseHeight, value);
		}

		public int DegreesCoverage
		{
			get => _degreesCoverage;
			set => SetProperty(ref _degreesCoverage, value);
		}

		public int DegreesOffset
		{
			get => _degreesOffset;
			set => SetProperty(ref _degreesOffset, value);
		}

		private void TreeModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//TODO make this smarter to do the minimal to add, subtract, or update node size or rotation angle.
			Nodes.Clear();
            Nodes.AddRange(GetTreePoints());
		}

		private IEnumerable<NodePoint> GetTreePoints()
		{
			int width = 95;

			double topLeftOffset = 5 + width / 2d - _topWidth / 2d;
			double bottomTopOffset = 95 - _baseHeight;

			double totalStringsInEllipse = Math.Ceiling(360d / Convert.ToDouble(_degreesCoverage) * Convert.ToDouble(Strings));

			var topEllipsePoints = GetEllipsePoints(topLeftOffset,
				0,
				_topWidth,
				_topHeight,
				totalStringsInEllipse,
				_degreesCoverage,
				_degreesOffset);
			var baseEllipsePoints = GetEllipsePoints(
				0,
				 bottomTopOffset,
				 width,
				 _baseHeight,
				 totalStringsInEllipse,
				 _degreesCoverage,
				 _degreesOffset);

			var strings = new List<List<NodePoint>>();

			for (int stringNum = 0; stringNum < _strings; stringNum++)
			{
				if (stringNum < _strings && stringNum < topEllipsePoints.Count())
				{
					var topPoint = topEllipsePoints[_strings - 1 - stringNum];
					var basePoint = baseEllipsePoints[_strings - 1 - stringNum];
                    topPoint.X /= 100f;
                    topPoint.Y /= 100f;
                    basePoint.X /= 100f;
                    basePoint.Y /= 100f;
					var s = LineModel.GetLinePoints(_nodesPerString, basePoint, topPoint, _nodeSize);
					strings.Add(s);
				}
			}

			//Flatten the strings of NodePoints
			return strings.SelectMany(x => x);

		}
		public static List<PointF> GetEllipsePoints(
			double leftOffset,
			double topOffset,
			double width,
			double height,
			double totalPoints,
			double degrees,
			double degreeOffset)
		{

			var points = new List<PointF>();

			double totalRadians = degrees * Math.PI / 180d;
			double numPoints = totalPoints / 2;
			double centerX = width / 2d;
			double centerY = height / 2d;
			double radianIncrement;
			double radianOffset = degreeOffset * Math.PI / 180d;
			double startRadian = radianOffset;
			double endRadian = totalRadians + radianOffset;

			if (degrees <= 180)
			{
				radianIncrement = Math.PI / (numPoints - 1);
			}
			else
			{
				radianIncrement = Math.PI * 2 / totalPoints;
			}

			// watch out for rounding on the fp adds
			for (double t = startRadian; t < endRadian + radianIncrement / 10; t += radianIncrement)
			{
				double x = centerX + width / 2 * Math.Cos(t) + leftOffset;
				double y = centerY + height / 2 * Math.Sin(t) + topOffset;
				points.Add(new PointF((float)x, (float)y));
			}
			return points;
		}
    }
}