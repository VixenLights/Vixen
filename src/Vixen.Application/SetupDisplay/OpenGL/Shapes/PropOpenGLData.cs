using Common.OpenGLCommon.Constructs.DrawingEngine.Primitive;
using OpenTK.Mathematics;

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
		protected PropOpenGLData()
		{
			// Initialize the resize corner handles
			_upperLeftResizeCorner = new OpenGLDrawablePrimitive();
			_upperRightResizeCorner = new OpenGLDrawablePrimitive();
			_lowerRightResizeCorner = new OpenGLDrawablePrimitive();
			_lowerLeftResizeCorner = new OpenGLDrawablePrimitive();
			
			// Initialize the selection rectangle
			_selectionRectangle = new OpenGLDrawablePrimitive();
			
			// Initialize the center drag move handle
			_centerX = new OpenGLDrawablePrimitive();

			// Configure the base class to NOT calculate the minimum and maximum
			// from the vertices
			CalculateMinAndMaxFromVertices = false;
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
		/// Prop selection rectangular outline primitive.
		/// </summary>
		private IOpenGLDrawablePrimitive _selectionRectangle;
		
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
			bool upperLeft = _upperLeftResizeCorner.MouseOver(projectionMatrix, viewMatrix, width, height, mousePos);
			if (upperLeft)
			{
				handle = ResizeHandles.TopLeft;
			}

			// Check to see if the mouse is over the upper right resize handle
			bool upperRight = _upperRightResizeCorner.MouseOver(projectionMatrix, viewMatrix, width, height, mousePos);
			if (upperRight)
			{
				handle = ResizeHandles.TopRight;
			}

			// Check to see if the mouse is over the lower left resize handle
			bool lowerLeft = _lowerLeftResizeCorner.MouseOver(projectionMatrix, viewMatrix, width, height, mousePos);
			if (lowerLeft)
			{
				handle = ResizeHandles.BottomLeft;
			}

			// Check to see if the mouse is over the lower right resize handle
			bool lowerRight = _lowerRightResizeCorner.MouseOver(projectionMatrix, viewMatrix, width, height, mousePos);
			if (lowerRight)
			{
				handle = ResizeHandles.BottomRight;
			}

			// Return whether the mouse was over a resize handle
			return upperLeft || upperRight || lowerLeft || lowerRight;
		}

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
			InitializeSelectionRectangle(minimum.X, minimum.Y, maximum.X, maximum.Y);
			
			// Initialize the resize handle vertices
			InitializeResizeCornerHandles(minimum.X, minimum.Y, maximum.X, maximum.Y);
			
			// Initialize the center drag move handle vertices
			InitializeCenterXMoveHandle(heightY);
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
		private void InitializeCenterXMoveHandle(float sizeY)
		{ 
			// Clear the center X selection handle vertices
			_centerX.Vertices.Clear();

			// X ->
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(0.0f);     // Z

			_centerX.Vertices.Add(0.2f * sizeY + X); // X
			_centerX.Vertices.Add(0.0f * sizeY + Y); // Y
			_centerX.Vertices.Add(0.0f);             // Z

			// <- X
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(0.0f);     // Z

			_centerX.Vertices.Add(-0.2f * sizeY + X); // X
			_centerX.Vertices.Add(0.0f + Y);          // Y
			_centerX.Vertices.Add(0.0f);              // Z

			// Y Up
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(0.0f);     // Z

			_centerX.Vertices.Add(0.0f + X);         // X
			_centerX.Vertices.Add(0.2f * sizeY + Y); // Y
			_centerX.Vertices.Add(0.0f);             // Z

			// Y Down
			_centerX.Vertices.Add(0.0f + X); // X
			_centerX.Vertices.Add(0.0f + Y); // Y
			_centerX.Vertices.Add(0.0f);     // Z

			_centerX.Vertices.Add(0.0f + X);          // X
			_centerX.Vertices.Add(-0.2f * sizeY + Y); // Y
			_centerX.Vertices.Add(0.0f);              // Z
		}

		/// <summary>
		/// Initializes the vertices of the selection rectangle.
		/// </summary>
		/// <param name="minX">Minimum X coordinate of the prop</param>
		/// <param name="minY">Minimum Y coordinate of the prop</param>
		/// <param name="maxX">Maximum X coordinate of the prop</param>
		/// <param name="maxY">Maximum Y coordinate of the prop</param>
		private void InitializeSelectionRectangle(float minX, float minY, float maxX, float maxY)
		{
			_selectionRectangle.Vertices.Clear();

			// Upper Left
			_selectionRectangle.Vertices.Add(minX);
			_selectionRectangle.Vertices.Add(minY);
			_selectionRectangle.Vertices.Add(0.0f);

			// Upper Right
			_selectionRectangle.Vertices.Add(maxX);
			_selectionRectangle.Vertices.Add(minY);
			_selectionRectangle.Vertices.Add(0.0f);

			// Lower Right
			_selectionRectangle.Vertices.Add(maxX);
			_selectionRectangle.Vertices.Add(maxY);
			_selectionRectangle.Vertices.Add(0.0f);

			// Lower Left
			_selectionRectangle.Vertices.Add(minX);
			_selectionRectangle.Vertices.Add(maxY);
			_selectionRectangle.Vertices.Add(0.0f);
		}

		private void InitializeResizeCornerHandles(float minX, float minY, float maxX, float maxY)
		{
			// Size of the resize handle in pixel
			const float SizeOfHandle = 50f;

			// Lower Left Resize Handle Box
			_lowerLeftResizeCorner.Vertices.Clear();

			_lowerLeftResizeCorner.Vertices.Add(minX);
			_lowerLeftResizeCorner.Vertices.Add(minY);
			_lowerLeftResizeCorner.Vertices.Add(0.0f);

			_lowerLeftResizeCorner.Vertices.Add(minX);
			_lowerLeftResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(0.0f);

			_lowerLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(0.0f);

			_lowerLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_lowerLeftResizeCorner.Vertices.Add(minY);
			_lowerLeftResizeCorner.Vertices.Add(0.0f);

			// Upper Right Resize Handle Box
			_upperRightResizeCorner.Vertices.Clear();
			_upperRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxY);
			_upperRightResizeCorner.Vertices.Add(0.0f);

			_upperRightResizeCorner.Vertices.Add(maxX);
			_upperRightResizeCorner.Vertices.Add(maxY);
			_upperRightResizeCorner.Vertices.Add(0.0f);

			_upperRightResizeCorner.Vertices.Add(maxX);
			_upperRightResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(0.0f);

			_upperRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperRightResizeCorner.Vertices.Add(0.0f);

			// Lower Right Resize Handle Box
			_lowerRightResizeCorner.Vertices.Clear();
			_lowerRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(minY);
			_lowerRightResizeCorner.Vertices.Add(0.0f);

			_lowerRightResizeCorner.Vertices.Add(maxX);
			_lowerRightResizeCorner.Vertices.Add(minY);
			_lowerRightResizeCorner.Vertices.Add(0.0f);

			_lowerRightResizeCorner.Vertices.Add(maxX);
			_lowerRightResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(0.0f);

			_lowerRightResizeCorner.Vertices.Add(maxX - SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(minY + SizeOfHandle);
			_lowerRightResizeCorner.Vertices.Add(0.0f);

			// Upper Left Resize Handle Box
			_upperLeftResizeCorner.Vertices.Clear();
			_upperLeftResizeCorner.Vertices.Add(minX);
			_upperLeftResizeCorner.Vertices.Add(maxY);
			_upperLeftResizeCorner.Vertices.Add(0.0f);

			_upperLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxY);
			_upperLeftResizeCorner.Vertices.Add(0.0f);

			_upperLeftResizeCorner.Vertices.Add(minX + SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(0.0f);

			_upperLeftResizeCorner.Vertices.Add(minX);
			_upperLeftResizeCorner.Vertices.Add(maxY - SizeOfHandle);
			_upperLeftResizeCorner.Vertices.Add(0.0f);
		}

		#endregion

		#region IOpenGLSelectable

		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetCenterX()
		{
			return _centerX;
		}
		
		/// <inheritdoc/>		
		public IOpenGLDrawablePrimitive GetSelectionRectangle()
		{
			return _selectionRectangle;	
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
			_selectionRectangle.Dispose();
			_centerX.Dispose();							
		}

		#endregion
	}
}
