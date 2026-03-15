using System.Drawing;
using Vixen.Sys.Props.Model;

namespace VixenModules.App.Props.Models.Line
{
    public class LineModel:BaseLightModel
    {
        public bool AddStartPadding { get; set; }

        public bool AddEndPadding { get; set; }

        public int NodeCount { get; set; }

        public static List<NodePoint> GetLinePoints(double numPoints, PointF start, PointF end, int size,
            int rotationAngle = 0, bool startPadding = false, bool endPadding = false)
        {
            var nodePoints = new List<NodePoint>();
            if (numPoints > 0)
            {
                var count = startPadding ? numPoints : numPoints - 1;
                count = endPadding ? count + 1 : count;
                double xSpacing = (start.X - end.X) / count;
                double ySpacing = (start.Y - end.Y) / count;
                double x = start.X;
                double y = start.Y;
                if (startPadding)
                {
                    x -= xSpacing;
                    y -= ySpacing;
                }

                for (int i = 0; i < numPoints; i++)
                {
                    var nodePoint = new NodePoint(x, y){Size = size};
                    nodePoints.Add(nodePoint);
                    x -= xSpacing;
                    y -= ySpacing;
                }

                RotateNodePoints(nodePoints, rotationAngle);
            }

            return nodePoints;
        }

        /// <summary>
        /// Rotates the NodePoints around the center of a 0,1 matrix.
        /// </summary>
        /// <param name="nodePoints"></param>
        /// <param name="angleInDegrees"></param>
        protected static void RotateNodePoints(List<NodePoint> nodePoints, int angleInDegrees)
        {
	        double centerX = .5;
	        double centerY = .5;
	        double angleInRadians = angleInDegrees * (Math.PI / 180);
	        double cosTheta = Math.Cos(angleInRadians);
	        double sinTheta = Math.Sin(angleInRadians);
	        foreach (var nodePoint in nodePoints)
	        {
		        double x =
			        cosTheta * (nodePoint.X - centerX) -
			        sinTheta * (nodePoint.Y - centerY);
		        double y =
			        sinTheta * (nodePoint.X - centerX) +
			        cosTheta * (nodePoint.Y - centerY);

		        nodePoint.X = x + centerX;
		        nodePoint.Y = y + centerY;
	        }
        }
		/// <inheritdoc/>				
		protected override IEnumerable<NodePoint> Get3DNodePoints()
		{
			throw new NotImplementedException();
		}
	}
}