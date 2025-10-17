#nullable enable
using System.Drawing;

using Vixen.Sys.Props.Model;

using VixenModules.App.Props.Models.Line;

namespace VixenModules.App.Props.Models.Tree
{
	/// <summary>
	/// Maintains a tree prop model.
	/// </summary>
	[Serializable]
	public class TreeModel : BaseLightModel
	{		
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public TreeModel() : this(16, 50, 2)
		{ 
		}	


		public TreeModel(int strings = 16, int nodesPerString = 50, int nodeSize = 2)
		{
			_topWidth = 20;
			_topHeight = _topWidth / 2;
			_baseHeight = 40;
			_degreesCoverage = 360;
			_strings = strings;
			_nodesPerString = nodesPerString;
			_nodeSize = nodeSize;
			_topRadius = 10;
			_bottomRadius = 100;			
			Nodes = new(Get3DNodePoints());
			PropertyChanged += PropertyModelChanged;			
		}

		#endregion

		#region Public Properties

		private int _strings;

		public int Strings
		{
			get => _strings;
			set => SetProperty(ref _strings, value);
		}

		private int _nodesPerString;

		public int NodesPerString
		{
			get => _nodesPerString;
			set => SetProperty(ref _nodesPerString, value);
		}

		private int _nodeSize;

		public int NodeSize
		{
			get => _nodeSize;
			set => SetProperty(ref _nodeSize, value);
		}

		private int _topWidth;

		public int TopWidth
		{
			get => _topWidth;
			set => SetProperty(ref _topWidth, value);
		}

		private int _topHeight;

		public int TopHeight
		{
			get => _topHeight;
			set => SetProperty(ref _topHeight, value);
		}

		private int _baseHeight;

		public int BaseHeight
		{
			get => _baseHeight;
			set => SetProperty(ref _baseHeight, value);
		}

		private int _degreesCoverage;

		public int DegreesCoverage
		{
			get => _degreesCoverage;
			set => SetProperty(ref _degreesCoverage, value);
		}

		private int _degreesOffset;

		public int DegreesOffset
		{
			get => _degreesOffset;
			set => SetProperty(ref _degreesOffset, value);
		}

		private float _topRadius;

		/// <summary>
		/// Radius at the top of the tree as a percentage.
		/// </summary>
		public float TopRadius 
		{
			get => _topRadius;
			set => SetProperty(ref _topRadius, value);
		}

		private float _bottomRadius;

		/// <summary>
		/// Radius at the bottom of the tree as a percentage.
		/// </summary>
		public float BottomRadius
		{
			get => _bottomRadius;
			set => SetProperty(ref _bottomRadius, value);
		}

		#endregion
						
		/// <summary>
		/// Creates the 3-D points that make up the tree.
		/// </summary>
		/// <returns>3-D points that make up the tree</returns>
		private IEnumerable<NodePoint> Get3DNodePoints(float width, float height)
		{
			// Create the collection of node points
			List<NodePoint> treePoints = new List<NodePoint>();

			// Maximum radius is half the drawing area
			double maxWidth = width / 2.0;
						
			// Calculate the top and bottom radius
			double topRadius = TopRadius / 100.0 * maxWidth;
			double bottomRadius = BottomRadius / 100.0 * maxWidth;

			double radiusDelta = (bottomRadius - topRadius) / NodesPerString;

			// Loop over the number of strands
			for (int i = 0; i < Strings; i++)
			{
				// Calculate the angle between strands
				double angle = (DegreesCoverage / Strings) * i + DegreesOffset;

				// Add a strand to the tree
				treePoints.AddRange(CreateStrand(NodesPerString, angle, bottomRadius, radiusDelta, -height / 2.0, + height / NodesPerString));
			}

			// (Optionally) rotate the points along the X, Y, and Z axis
			//ToDo : Replace null with rotation
			RotatePoints(treePoints, null);	

			return treePoints;
		}
		
		/// <summary>
		/// Creates the 3-D points that make up the tree.
		/// </summary>
		/// <returns>3-D points that make up the tree</returns>
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			return Get3DNodePoints(1.0f, 1.0f);		
		}

		/// <summary>
		/// Creates a strand of nodes.
		/// </summary>
		/// <param name="nodesPerStrand">Number of nodes per strand</param>
		/// <param name="angle">Position of the strand along the tree</param>		
		/// <param name="bottomRadius">Bottom radius as a percentage</param>		
		/// <param name="radiusDelta">Change in radius while moving up the tree</param>		
		/// <param name="yStart"></param>
		/// <param name="yDelta"></param>
		/// <returns></returns>
		private IEnumerable<NodePoint> CreateStrand(
			int nodesPerStrand, 
			double angle, 			
			double bottomRadius, 
			double radiusDelta,
			double yStart, 
			double yDelta)
		{
			// Create the collection of node points
			List<NodePoint> strandPoints = new();

			double radians = angle * Math.PI / 180; // Convert to radians
			 
			// Initialize the Y position of the strand nodes
			double offsetY = yStart;

			// Initialize the starting radius
			double radius = bottomRadius;

			// Loop over the nodes in the strand
			for (int p = 0; p < nodesPerStrand; p++) 
			{
				// Calculate the position of the node
				double offsetZ = Math.Sin(radians) * radius;
				double offsetX = Math.Cos(radians) * radius;

				// Add the node point
				strandPoints.Add(new NodePoint(offsetX, offsetY, offsetZ));

				// Decrement the radius as we move up the strand
				radius -= radiusDelta;

				// Increase the Y coordinate
				offsetY += yDelta;
			}

			// Return the node points that make up the strand
			return strandPoints;
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