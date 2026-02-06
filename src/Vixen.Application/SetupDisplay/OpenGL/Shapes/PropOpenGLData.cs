using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
using OpenTK.Mathematics;

using Vixen.Sys.Props.Model;

namespace VixenApplication.SetupDisplay.OpenGL.Shapes
{
	/// <summary>
	/// Base class for OpenGL prop data.
	/// </summary>
	public abstract class PropOpenGLData : OpenGLDrawablePrimitive, IPropOpenGLData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propModel">Prop model of the prop</param>
		protected PropOpenGLData(IPropModel propModel)
		{
			// Initialize the resize corner handles
			_upperLeftResizeCorner = new OpenGLDrawablePrimitive();
			_upperRightResizeCorner = new OpenGLDrawablePrimitive();
			_lowerRightResizeCorner = new OpenGLDrawablePrimitive();
			_lowerLeftResizeCorner = new OpenGLDrawablePrimitive();

			// Initialize selection cuboid
			_selectionCuboidLeft = new OpenGLDrawablePrimitive();
			_selectionCuboidRight = new OpenGLDrawablePrimitive();
			_selectionCuboidTop = new OpenGLDrawablePrimitive();
			_selectionCuboidBottom = new OpenGLDrawablePrimitive();
			_selectionCuboidFront = new OpenGLDrawablePrimitive();
			_selectionCuboidBack = new OpenGLDrawablePrimitive();

			// Initialize the center drag move handle
			_centerX = new OpenGLDrawablePrimitive();

			// Configure the base class to NOT calculate the minimum and maximum
			// from the vertices
			CalculateMinAndMaxFromVertices = false;

			// Store off the prop model
			PropModelId = propModel.Id;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Upper left corner resize handle.
		/// </summary>
		private IOpenGLDrawablePrimitive _upperLeftResizeCorner;

		/// <summary>
		/// Upper right corner resize handle.
		/// </summary>
		private IOpenGLDrawablePrimitive _upperRightResizeCorner;

		/// <summary>
		/// Lower right corner resize handle.
		/// </summary>
		private IOpenGLDrawablePrimitive _lowerRightResizeCorner;

		/// <summary>
		/// Lower left corner resize handle.
		/// </summary>
		private IOpenGLDrawablePrimitive _lowerLeftResizeCorner;
				
		/// <summary>
		/// Left side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidLeft;

		/// <summary>
		/// Right side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidRight;

		/// <summary>
		/// Top side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidTop;

		/// <summary>
		/// Bottom side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidBottom;

		/// <summary>
		/// Front side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidFront;

		/// <summary>
		/// Back side of the prop selection cuboid.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionCuboidBack;

		/// <summary>
		/// Prop center move handle.
		/// </summary>
		private IOpenGLDrawablePrimitive _centerX;

		#endregion

		#region IPropOpenGLData
		
		/// <inheritdoc/>
		public bool Selected { get; set; }

		/// <inheritdoc/>
		public float X { get; set; }

		/// <inheritdoc/>
		public float Y { get; set; }

		/// <inheritdoc/>
		public float SizeX { get; set; }

		/// <inheritdoc/>
		public float SizeY { get; set; }

		/// <inheritdoc/>
		public float SizeZ { get; set; }

		/// <inheritdoc/>
		public bool MouseOverResizeHandle(
			Vector3 cameraPosition,
			Matrix4 projectionMatrix, 
			Matrix4 viewMatrix, 
			int width, 
			int height, 
			Vector2 mousePos, 
			out ResizeHandles handle)
		{
			// Default the out parameter
			handle = ResizeHandles.BottomRight;

			// Check to see if the mouse is over the upper left resize handle
			bool upperLeft = _upperLeftResizeCorner.MouseOver(cameraPosition, projectionMatrix, viewMatrix, width, height, mousePos);
			if (upperLeft)
			{
				handle = ResizeHandles.TopLeft;
			}

			// Check to see if the mouse is over the upper right resize handle
			bool upperRight = _upperRightResizeCorner.MouseOver(cameraPosition, projectionMatrix, viewMatrix, width, height, mousePos);
			if (upperRight)
			{
				handle = ResizeHandles.TopRight;
			}

			// Check to see if the mouse is over the lower left resize handle
			bool lowerLeft = _lowerLeftResizeCorner.MouseOver(cameraPosition, projectionMatrix, viewMatrix, width, height, mousePos);
			if (lowerLeft)
			{
				handle = ResizeHandles.BottomLeft;
			}

			// Check to see if the mouse is over the lower right resize handle
			bool lowerRight = _lowerRightResizeCorner.MouseOver(cameraPosition, projectionMatrix, viewMatrix, width, height, mousePos);
			if (lowerRight)
			{
				handle = ResizeHandles.BottomRight;
			}

			// Return whether the mouse was over a resize handle
			return upperLeft || upperRight || lowerLeft || lowerRight;
		}

		/// <inheritdoc/>
		public Guid PropModelId { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Initializes the drawing primitive selection vertices.
		/// </summary>
		/// <param name="coordinates">Vertices of the prop</param>
		/// <param name="heightY">Size of the drawing control along the Y axis</param>
		protected void InitializeSelectionVertices(List<Vector3> coordinates, float heightY)
		{
			Vector3 minimum;
			Vector3 maximum;
		
			// Calculate the absolute minimum and maximum vertices
			// This takes into account each axis (x,y,z) independently
			CalculateAbsoluteMinAndMaxVertices(coordinates, out minimum, out maximum);			
			
			// Initialize the selection rectangle vertices
			// Adding 1 to the z component to avoid overlap with the prop
			InitializeSelectionRectangle(minimum.X, minimum.Y, minimum.Z, maximum.X, maximum.Y, maximum.Z + 1);
			
			// Initialize the resize handle vertices
			// Adding 1 to the z component to avoid overlap with the prop
			InitializeResizeCornerHandles(minimum.X, minimum.Y, minimum.Z, maximum.X, maximum.Y, maximum.Z + 1);
			
			// Initialize the center drag move handle vertices
			InitializeCenterXMoveHandle(heightY, maximum.Z);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Calculates the absolute minimum and maximum vertices along all axis.
		/// </summary>
		/// <param name="coordinates">Coordinates to analyze</param>		
		private void CalculateAbsoluteMinAndMaxVertices(
			List<Vector3> coordinates, 			
			out Vector3 minimum,
			out Vector3 maximum)
		{
			// Initialize results to large or small values
			float minX = float.PositiveInfinity;
			float maxX = float.NegativeInfinity;
			float minY = float.PositiveInfinity;
			float maxY = float.NegativeInfinity;
			float minZ = float.PositiveInfinity;
			float maxZ = float.NegativeInfinity;

			// Loop over all the vertices of the prop
			foreach (Vector3 p in coordinates)
			{
				// Look for the minimum X coordinate
				if (p.X < minX)
				{
					minX = p.X;
				}

				// Look for the maximum X coordinate
				if (p.X > maxX)
				{
					maxX = p.X;
				}

				// Look for the minimum Y coordinate
				if (p.Y < minY)
				{
					minY = p.Y;
				}

				// Look for the maximum Y coordinate
				if (p.Y > maxY)
				{
					maxY = p.Y;
				}

				// Look for the minimum Z coordinate
				if (p.Z < minZ)
				{
					minZ = p.Z;
				}

				// Look for the maximum Z coordinate
				if (p.Z > maxZ)
				{
					maxZ = p.Z;
				}
			}
			
			// Return the results via the out arguments
			minimum = new Vector3(minX, minY, minZ);
			maximum = new Vector3(maxX, maxY, maxZ);

			// Set protected properties in the base class
			Minimum = minimum;
			Maximum = maximum;
		}

		/// <summary>
		/// Initializes the vertices of the center X drag move handle.
		/// </summary>
		/// <param name="sizeY">Size of the prop in the Y axis</param>
		private void InitializeCenterXMoveHandle(float sizeY, float minZ)
		{ 
			// Clear the center X selection handle vertices
			_centerX.Vertices.Clear();

			// X ->
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(minZ);     // Z

			_centerX.Vertices.Add(0.2f * sizeY + X); // X
			_centerX.Vertices.Add(0.0f * sizeY + Y); // Y
			_centerX.Vertices.Add(minZ);             // Z

			// <- X
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(minZ);     // Z

			_centerX.Vertices.Add(-0.2f * sizeY + X); // X
			_centerX.Vertices.Add(0.0f + Y);          // Y
			_centerX.Vertices.Add(minZ);              // Z

			// Y Up
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(minZ);     // Z

			_centerX.Vertices.Add(0.0f + X);         // X
			_centerX.Vertices.Add(0.2f * sizeY + Y); // Y
			_centerX.Vertices.Add(minZ);             // Z

			// Y Down
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(minZ);     // Z

			_centerX.Vertices.Add(0.0f + X);          // X
			_centerX.Vertices.Add(-0.2f * sizeY + Y); // Y
			_centerX.Vertices.Add(minZ);              // Z
		}

		/// <summary>
		/// Initializes the vertices of the selection rectangle.
		/// </summary>
		/// <param name="minX">Minimum X coordinate of the prop</param>
		/// <param name="minY">Minimum Y coordinate of the prop</param>
		/// <param name="maxX">Maximum X coordinate of the prop</param>
		/// <param name="maxY">Maximum Y coordinate of the prop</param>
		private void InitializeSelectionRectangle(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
		{
			// ** FRONT **
			_selectionCuboidFront.Vertices.Clear();

			// Upper Left
			_selectionCuboidFront.Vertices.Add(minX);
			_selectionCuboidFront.Vertices.Add(minY);
			_selectionCuboidFront.Vertices.Add(minZ);

			// Upper Right
			_selectionCuboidFront.Vertices.Add(maxX);
			_selectionCuboidFront.Vertices.Add(minY);
			_selectionCuboidFront.Vertices.Add(minZ);

			// Lower Right
			_selectionCuboidFront.Vertices.Add(maxX);
			_selectionCuboidFront.Vertices.Add(maxY);
			_selectionCuboidFront.Vertices.Add(minZ);

			// Lower Left
			_selectionCuboidFront.Vertices.Add(minX);
			_selectionCuboidFront.Vertices.Add(maxY);
			_selectionCuboidFront.Vertices.Add(minZ);

			// ** Back **
			_selectionCuboidBack.Vertices.Clear();

			// Upper Left
			_selectionCuboidBack.Vertices.Add(minX);
			_selectionCuboidBack.Vertices.Add(minY);
			_selectionCuboidBack.Vertices.Add(maxZ);

			// Upper Right
			_selectionCuboidBack.Vertices.Add(maxX);
			_selectionCuboidBack.Vertices.Add(minY);
			_selectionCuboidBack.Vertices.Add(maxZ);

			// Lower Right
			_selectionCuboidBack.Vertices.Add(maxX);
			_selectionCuboidBack.Vertices.Add(maxY);
			_selectionCuboidBack.Vertices.Add(maxZ);

			// Lower Left
			_selectionCuboidBack.Vertices.Add(minX);
			_selectionCuboidBack.Vertices.Add(maxY);
			_selectionCuboidBack.Vertices.Add(maxZ);

			// ** Left Side **
			_selectionCuboidLeft.Vertices.Clear();

			// Upper Left
			_selectionCuboidLeft.Vertices.Add(minX);
			_selectionCuboidLeft.Vertices.Add(minY);
			_selectionCuboidLeft.Vertices.Add(minZ);

			// Lower Left
			_selectionCuboidLeft.Vertices.Add(minX);
			_selectionCuboidLeft.Vertices.Add(maxY);
			_selectionCuboidLeft.Vertices.Add(minZ);

			// Lower Rear
			_selectionCuboidLeft.Vertices.Add(minX);
			_selectionCuboidLeft.Vertices.Add(maxY);
			_selectionCuboidLeft.Vertices.Add(maxZ);

			// Upper Rear
			_selectionCuboidLeft.Vertices.Add(minX);
			_selectionCuboidLeft.Vertices.Add(minY);
			_selectionCuboidLeft.Vertices.Add(maxZ);

			// ** Right Side **
			_selectionCuboidRight.Vertices.Clear();

			// Upper Left
			_selectionCuboidRight.Vertices.Add(maxX);
			_selectionCuboidRight.Vertices.Add(minY);
			_selectionCuboidRight.Vertices.Add(minZ);

			// Lower Left
			_selectionCuboidRight.Vertices.Add(maxX);
			_selectionCuboidRight.Vertices.Add(maxY);
			_selectionCuboidRight.Vertices.Add(minZ);

			// Lower Rear
			_selectionCuboidRight.Vertices.Add(maxX);
			_selectionCuboidRight.Vertices.Add(maxY);
			_selectionCuboidRight.Vertices.Add(maxZ);

			// Upper Rear
			_selectionCuboidRight.Vertices.Add(maxX);
			_selectionCuboidRight.Vertices.Add(minY);
			_selectionCuboidRight.Vertices.Add(maxZ);
		}

		private void InitializeResizeCornerHandles(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
		{
			// Size of the resize handle in pixel
			const float SizeOfHandle = 50f;

			// Lower Left Resize Handle Box
			_lowerLeftResizeCorner.Vertices.Clear();

			_lowerLeftResizeCorner.Vertices.Add(minX);
			_lowerLeftResizeCorner.Vertices.Add(minY);
			_lowerLeftResizeCorner.Vertices.Add(maxZ);

			_lowerLeftResizeCorner.Vertices.Add(minX);
			_lowerLeftResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(maxZ);

			_lowerLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(maxZ);

			_lowerLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(minY);
			_lowerLeftResizeCorner.Vertices.Add(maxZ);

			// Upper Right Resize Handle Box
			_upperRightResizeCorner.Vertices.Clear();
			_upperRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxY);
			_upperRightResizeCorner.Vertices.Add(maxZ);

			_upperRightResizeCorner.Vertices.Add(maxX);
			_upperRightResizeCorner.Vertices.Add(maxY);
			_upperRightResizeCorner.Vertices.Add(maxZ);

			_upperRightResizeCorner.Vertices.Add(maxX);
			_upperRightResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxZ);

			_upperRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxZ);

			// Lower Right Resize Handle Box
			_lowerRightResizeCorner.Vertices.Clear();
			_lowerRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(minY);
			_lowerRightResizeCorner.Vertices.Add(maxZ);

			_lowerRightResizeCorner.Vertices.Add(maxX);
			_lowerRightResizeCorner.Vertices.Add(minY);
			_lowerRightResizeCorner.Vertices.Add(maxZ);

			_lowerRightResizeCorner.Vertices.Add(maxX);
			_lowerRightResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(maxZ);

			_lowerRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(maxZ);

			// Upper Left Resize Handle Box
			_upperLeftResizeCorner.Vertices.Clear();
			_upperLeftResizeCorner.Vertices.Add(minX);
			_upperLeftResizeCorner.Vertices.Add(maxY);
			_upperLeftResizeCorner.Vertices.Add(maxZ);

			_upperLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxY);
			_upperLeftResizeCorner.Vertices.Add(maxZ);

			_upperLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxZ);

			_upperLeftResizeCorner.Vertices.Add(minX);
			_upperLeftResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxZ);
		}

		#endregion

		#region IOpenGLSelectable

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetCenterX()
		{
			return _centerX;
		}

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetSelectionCuboidLeftSide()
		{
			return _selectionCuboidLeft;
		}

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetSelectionCuboidRightSide()
		{
			return _selectionCuboidRight;
		}

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetSelectionCuboidFrontSide()
		{
			return _selectionCuboidFront;
		}

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetSelectionCuboidBackSide()
		{
			return _selectionCuboidBack;
		}
		
		/// <inheritdoc/>
		public IOpenGLDrawablePrimitive GetUpperLeftCornerResizeBox()
		{
			return _upperLeftResizeCorner;
		}
		
		/// <inheritdoc/>
		public IOpenGLDrawablePrimitive GetUpperRightCornerResizeBox()
		{
			return _upperRightResizeCorner;
		}
		
		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetLowerRightCornerResizeBox()
		{
			return _lowerRightResizeCorner;
		}
		
		/// <inheritdoc/>
		public IOpenGLDrawablePrimitive GetLowerLeftCornerResizeBox()
		{
			return _lowerLeftResizeCorner;
		}

		#endregion
		
		#region IDisposable 

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Dispose()
		{
			base.Dispose();

			// Dispose of the drawing primitives
			_upperLeftResizeCorner.Dispose();						
			_upperRightResizeCorner.Dispose();									
			_lowerRightResizeCorner.Dispose();						
			_lowerLeftResizeCorner.Dispose();										
			_centerX.Dispose();							
		}

		#endregion
	}
}
