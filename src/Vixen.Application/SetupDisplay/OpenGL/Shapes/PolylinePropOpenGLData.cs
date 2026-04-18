using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
using Vixen.Sys.Props.Model;
using VixenModules.App.Props.Models.Polyline;

namespace VixenApplication.SetupDisplay.OpenGL.Shapes
{
    /// <summary>
    /// Maintains a polyline prop OpenGL data.
    /// </summary>
	public class PolylinePropOpenGLData : LightPropOpenGLData
	{
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lightPropModel">Associated light prop model</param>
        public PolylinePropOpenGLData(ILightPropModel lightPropModel) : base(lightPropModel)
		{
            // Save off the polyline model
			PolylineMdl = (PolylineModel)lightPropModel;

            // The initial desgin of the polyline is to allow the user to add points on the
            // preview in world coordinates.  When the polyline is complete the world coordinates
            // are normalized.  This requires the Sizes to be a value of 1.0 for the math to work.
			SizeX = 1.0f;
			SizeY = 1.0f;
			SizeZ = 1.0f;
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag that indicates of the polyline points have been normalized with respect to a center point.
        /// </summary>
        public bool Normalized { get; set; }

        /// <summary>
        /// Collection of polyline points.
        /// </summary>
		public List<IOpenGLDrawablePrimitive> Points { get; set; } = new List<IOpenGLDrawablePrimitive>();

        /// <summary>
        /// Polyline model associated with this OpenGL data.
        /// </summary>
		public PolylineModel PolylineMdl { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the center point of the polyline.
        /// </summary>
        public void CalculateCenterPointAndNormalize()
		{
            // Initialize local variable to calculate the minimum and maximum points
			int minX = int.MaxValue;
			int maxX = int.MinValue;
			int minY = int.MaxValue;
			int maxY = int.MinValue;

            // Loop over all the points in the polyline
			foreach (PolylinePointOpenGLDrawablePrimitive pt in Points)
			{
                // If the point is less than the current minimum then...
				if (pt.WorldPtX < minX)
				{
                    // Update the min X position
					minX = (int)pt.WorldPtX;
				}

                // If the point is greater than the current maximum then...
				if (pt.WorldPtX > maxX)
				{
                    // Update the max X position
					maxX = (int)pt.WorldPtX;
				}

                // If the point is less than the current minimum then...
                if (pt.WorldPtY < minY)
				{
                    // Update the min Y position
					minY = (int)pt.WorldPtY;
				}

                // If the point is greater than the current maximum then...
                if (pt.WorldPtY > maxY)
				{
                    // Update the max Y position
					maxY = (int)pt.WorldPtY;
				}
			}
           
            // Set the position of the prop to be the center of the polyline
            X = (maxX - minX) / 2.0f + minX;
            Y = (maxY - minY) / 2.0f + minY;
            
            // Calculate the X length of the polyline
            int xLength = maxX - minX;

            // Calculate the Y length of the polyline
            int yLength = maxY - minY;

            // Determine the maximum dimensions
            int maxLength = Math.Max(xLength, yLength);

            // Set the prop size to the max dimension
			SizeX = maxLength; 
			SizeY = maxLength; 
			SizeZ = maxLength; 

            // Loop over the points of the polyline
			foreach (PolylinePointOpenGLDrawablePrimitive pt in Points)
			{
                // Normalize the points
				pt.WorldPtX -= X;
				pt.WorldPtY -= Y;				
				pt.WorldPtX = pt.WorldPtX / maxLength;
				pt.WorldPtY = pt.WorldPtY / maxLength;

                // Record that the point has been normalized
                Normalized = true;
            }

            // Loop over the polyline segments
			foreach (PolylineSegment segment in PolylineMdl.Segments)
			{
                // Normalize the segment points
				segment.StartX -= X;
				segment.StartY -= Y;
				segment.EndX -= X;
				segment.EndY -= Y;
				segment.StartX = segment.StartX / maxLength;
				segment.StartY = segment.StartY / maxLength;
				segment.EndX = segment.EndX / maxLength;
				segment.EndY= segment.EndY / maxLength;
			}
		}

        #endregion
    }
}
