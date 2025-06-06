namespace Vixen.Sys.Props.Model.Line
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

    }
}