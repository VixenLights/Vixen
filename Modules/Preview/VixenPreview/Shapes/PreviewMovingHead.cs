using OpenTK;
using System;
using System.Collections.Generic;
using VixenModules.Preview.VixenPreview.Fixtures;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL;
using VixenModules.Preview.VixenPreview.Fixtures.WPF;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Maintains a moving head preview graphic.
	/// </summary>
	/// <remarks>
	/// This class was based on the <c>PreviewRectangle</c>.  Partial class is being used so that is easy to see
	/// the methods that were changed.  The methods in PreviewMovingHeadPartial.cs should be identical/similar to PreviewRectangle.cs.
	/// This cs file contains the methods that are unique to the moving head preview.
	/// </remarks>	
	public partial class PreviewMovingHead : PreviewBaseShape, IDrawStaticPreviewShape, IDisposable, IOpenGLMovingHeadShape
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PreviewMovingHead()
		{
			// Initialize the WPF moving head 
			InitializeGDI();			
		}

		#endregion

		#region Fields

		/// <summary>
		/// OpenGL implementation of the moving head graphic.
		/// </summary>
		private MovingHeadOpenGL _movingHeadOpenGL;

		/// <summary>
		/// WPF implementation of the moving head graphic.
		/// </summary>
		private MovingHeadWPF _movingHead;

		/// <summary>
		/// Zoom level of the GDI preview.
		/// </summary>
		private double _zoomLevel = 1.0;
		
		/// <summary>
		/// The following fields are used to determine when the cached WPF bitmap needs to be re-created.
		/// </summary>
		private int _fixtureWidth;
		private int _fixtureHeight;
		private double _fixtureZoom;

		/// <summary>
		/// Moving head settings associated with the cached bitmap.
		/// </summary>
		private IMovingHead _prevousMovingHeadSettings;

		#endregion

		#region IDrawStaticPreviewShape

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void InitializeGDI()
		{
			// Create the WPF moving head
			_movingHead = new MovingHeadWPF();
			
			// Initialize the meta-data on the cached image to ensure
			// the bitmap is rendered on the first frame.			
			_fixtureWidth = -1;
			_fixtureHeight = -1;
			_fixtureZoom = -1;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void DisposeOpenGLResources()
		{
			// If the moving head has not already been disposed then...
			if (_movingHeadOpenGL != null)
			{
				// Dispose the OpenGL resources
				_movingHeadOpenGL.Dispose();
				_movingHeadOpenGL = null;
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		/// <remarks>
		/// The majority of the moving head drawing is done by the parent form and <c>MovingHeadRenderStrategy</c> class.
		/// The drawing is done by the parent so that if there are multiple moving heads all the graphical volumes that use the same
		/// shader can be collected.  This minimizes the number of shader program transitions per frame.
		/// </remarks>
		public void DrawOpenGL(float zLength, int width, int height, Matrix4 projectionMatrix, Matrix4 viewMatrix, double scaleFactor, int referenceHeight, Vector3 camera)
		{
			// Calculate the width and height of the fixture
			int fixtureHeight = Bottom - Top;
			int fixtureWidth = Right - Left;

			// Draw the legend under the moving head			
			_movingHeadOpenGL.DrawLegend(
				(int)(Left + fixtureWidth / 2.0),
				(int)(referenceHeight - (Top + fixtureHeight / 2.0)),
				projectionMatrix,
				viewMatrix);
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void DrawGDI(FastPixel.FastPixel fp, bool editMode, bool selected, double zoomLevel)
		{
			// Create an empty HashSet to indicate that no elements are selected
			HashSet<Guid> highlightedElements = new HashSet<Guid>();

			// Store off the zoom level
			_zoomLevel = zoomLevel;

			// Called the shared Draw method
			Draw(fp, editMode, highlightedElements, selected, false, zoomLevel);
			
			// Restore the _zoomLevel back to 1.0 to support editing the preview
			_zoomLevel = 1.0;
		}

		#endregion

		#region Overrides of PreviewBaseShape

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void EndAddNew()
		{		
			// If the user created the shape right to left then...
			if (_bottomLeft.X > _bottomRight.X)
			{
				// Swap the X coordinates 
				int temp = _bottomLeft.X;
				_bottomLeft.X = _bottomRight.X;
				_bottomRight.X = temp;				
			}

			// If the user created the shape right to left then...
			if (_topLeft.X > _topRight.X)
			{
				// Swap the X coordinates 
				int temp = _topLeft.X;
				_topLeft.X = _topRight.X;
				_topRight.X = temp;
			}

			// If the user created the shape bottom top then...
			if (_bottomLeft.Y < _topLeft.Y)
			{
				// Swap the top for the bottom
				PreviewPoint tempLeft = _bottomLeft;
				PreviewPoint tempRight = _bottomRight;
				_bottomLeft = _topLeft;
				_bottomRight = _topRight;

				_topLeft = tempLeft;
				_topRight = tempRight;
			}

			// Adjust the shape to form a square
			AdjustShapeToSquare();
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		public override void OnMovePoint()
		{
			// If the selected point is the top left then...
			if (_selectedPoint == _topLeft)
			{
				// Adjust the bottom left X and top right Y coordinates so we have a perfect square
				_bottomLeft.X = _topLeft.X;
				_topRight.Y = _topLeft.Y;
			}
			// If the selected point is the bottom left then...
			else if (_selectedPoint == _bottomLeft)
			{
				// Adjust the top left X and bottom right Y coordinates so we have a perfect square
				_topLeft.X = _bottomLeft.X;
				_bottomRight.Y = _bottomLeft.Y;
			}
			// If the selected point is the top right then...
			else if (_selectedPoint == _topRight)
			{
				// Adjust the bottom X and top left Y coordinates so we have a perfect square
				_bottomRight.X = _topRight.X;
				_topLeft.Y = _topRight.Y;
			}
			// IF the selected point is the bottom right then
			else if (_selectedPoint == _bottomRight)
			{
				// Adjust the top X and bottom Y coordinates so we have a perfect square
				_topRight.X = _bottomRight.X;
				_bottomLeft.Y = _bottomRight.Y;
			}

			// Adjust the shape to form a square
			AdjustShapeToSquare();
		}
				        	
		/// <summary>
		/// Returns true if the point is inside the shape.
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the point is inside the shape</returns>
		public override bool PointInShape(PreviewPoint point)
		{
			// Default to NOT inside the shape
			bool pointInShape = false;
			
			// If the point is inside the rectangle then...
			if (point.X >= Left &&
			    point.X <= Right &&
			    point.Y >= Top &&
			    point.Y <= Bottom)
			{
				// Indicate the point is inside the shape
				pointInShape = true;
			}
			
			return pointInShape;
		}

		/// <summary>
		/// Draws the shape for the GDI Preview and when editing the preview.
		/// </summary>
		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected, bool forceDraw, double zoomLevel)
		{			
			// This draws the four corners of the rectangle when the fixture is selected
			DrawSelectPoints(fp);

			// Save off the zoom level
			_zoomLevel = zoomLevel;

			// Calculate the width and height of the fixture graphic before scaling
			int widthOrg = _bottomRight.X - _topLeft.X;
			int heightOrg = _bottomRight.Y - _topLeft.Y;

			// Initialize the width and height
			int width = widthOrg;
			int height = heightOrg;

			// If the zoom level is NOT zero then...
			if (_zoomLevel != 0)
			{
				// Update the width and height for the zoom level
				width = (int)(width * _zoomLevel);
				height = (int)(height * _zoomLevel);
			}

			// Calculate the scale factor
			double scaleFactor = ((double)width) / (double)widthOrg;

			// If the fixture graphic is stale and needs to be redrawn then...			
			if (NeedToRenderBitmap(width, height))
			{
				// If the width and height are positive values then...
				if (widthOrg > 0 && heightOrg > 0)
				{
					// Save off the width of the graphic
					_fixtureWidth = width;

					// Save off the height of thre graphic
					_fixtureHeight = height;

					// Save off the zoom level of the preview
					_fixtureZoom = _zoomLevel;
										
					// If the WPF moving head has not been created yet then...
					if (_movingHead == null)
					{
						// Create the WPF moving head graphic class
						_movingHead = new MovingHeadWPF();						
					}
					else
					{
						// If the moving head is associated with another thread
						if (!_movingHead.CheckAccess())
						{
							// Create a new moving head associated with this thread
							_movingHead = new MovingHeadWPF();							
						}
					}
					
					// Draw the fixture using WPF
					// The X offset and Y offset were previously used when using the WPF graphics in the GDI Preview					
					_movingHead.DrawFixture(width, height, scaleFactor, 0, 0);

					// Update the settings associated with the bitmap.
					_prevousMovingHeadSettings = MovingHeadWPF.MovingHead.Clone();					
				}
			}

			// If the fixture graphic has been rendered and
			// the current size matches the rendered size then...
			if (_movingHead != null &&
				_movingHead.BitmapColors != null &&
				_movingHead.BitmapColors.Length == width * height)
			{
				// Copy the cached fixture bitmap into the preview
				CopyFixturePixelsIntoPreview(fp, width, height, scaleFactor);
			}
		
			// Draw the outline around the fixture
			DrawRectangleOutline(fp);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Adjusts the dimensions of the shape to keep a perfect square.
		/// </summary>
		void AdjustShapeToSquare()
		{
			// Determine the new width
			int w = Right - Left;

			// Determine the new height
			int h = Bottom - Top;

			// If the height is greater than the width then...
			if (h > w)
			{
				// Adjust the height to match the smaller width
				_bottomRight.Y = Top + w;
				_bottomLeft.Y = Top + w;
			}
			else
			{
				// Adjust the width to match the smaller height
				_bottomRight.X = Left + w;
				_topRight.X = Left + w;
			}
		}
		
		/// <summary>
		/// Returns true when the moving head bitmap needs to be updated.
		/// </summary>
		/// <param name="width">Width of drawing area</param>
		/// <param name="height">Height of drawing area</param>
		/// <returns>True when the moving head bitmap needs to be updated</returns>
		private bool NeedToRenderBitmap(int width, int height)
		{
			// If the dimensions of the bitmap or the
			// zoom level have changed then update the bitmap
			return
				(_fixtureWidth != width ||
				 _fixtureHeight != height ||
				 _fixtureZoom != _zoomLevel ||
				 _prevousMovingHeadSettings == null ||
				 !_prevousMovingHeadSettings.Equals(MovingHeadOpenGL.MovingHead));
		}

		/// <summary>
		/// Copies pixels from the cached bitmap into the preview FastPixel buffer.
		/// </summary>
		/// <param name="fp">Fast Pixel buffer</param>
		/// <param name="width">Width of the fixture</param>
		/// <param name="height">Height of the fixture</param>
		/// <param name="scaleFactor">Active scale factor</param>
		private void CopyFixturePixelsIntoPreview(FastPixel.FastPixel fp, int width, int height, double scaleFactor)
		{			
			// Loop over the X dimension
			for (int x = 0; x < width; x++)
			{				
				// Loop over the Y dimension
				for (int y = 0; y < height; y++)
				{
					// Set the pixel in the FastPixel buffer from the cached bitmap
					fp.SetPixel(
						new Point(x + (int)(Left * scaleFactor), 
						          y + (int)(Top * scaleFactor)),
						_movingHead.BitmapColors[x, y]);
				}
			}
		}
		
		/// <summary>
		/// Draws a rectangle around the moving head drawing area.
		/// </summary>
		/// <param name="fp">FastPixel frame buffer</param>
		private void DrawRectangleOutline(FastPixel.FastPixel fp)
		{
			// Default the outline color to gray
			Color outlineColor = Color.Gray;

			// If the fixture is selected then...
			if (Selected)
			{
				// Set the outline color to Hot Pink
				outlineColor = Color.HotPink;
			}

			// Loop over the width of the rectangle
			for (int x = Left; x <= Right; x++)
			{
				// Draw the top and bottom of the rectangle outline
				fp.SetPixel(new Point((int)(x * _zoomLevel), (int)(Top * _zoomLevel)), outlineColor);
				fp.SetPixel(new Point((int)(x * _zoomLevel), (int)(Bottom * _zoomLevel)), outlineColor);
			}

			// Loop over the height of the rectangle
			for (int y = Top; y <= Bottom; y++)
			{
				// Draw the left and right sides of the rectangle outline
				fp.SetPixel(new Point((int)(Right * _zoomLevel), (int)(y * _zoomLevel)), outlineColor);
				fp.SetPixel(new Point((int)(Left * _zoomLevel), (int)(y * _zoomLevel)), outlineColor);
			}
		}
		
		/// <summary>
		/// Calculates the width and height of the drawing area.  
		/// The fixture is always drawn in a square where the width and height are equal.
		/// </summary>
		/// <returns>Length (width & height) of the drawing area</returns>
		private double CalculateDrawingLength()
		{
			// Calculate the height of the drawing area
			int fixtureHeight = Bottom - Top;

			// Calculate the width of the drawing area
			int fixtureWidth = Right - Left;

			// Default the length to the fixture height
			double length = fixtureHeight;

			// If the width is smaller than the height
			if (fixtureWidth < fixtureHeight)
			{
				// Set the length to the smaller dimension
				length = fixtureWidth;
			}

			return length;
		}

		#endregion

		#region IDrawMovingHeadVolumes

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IRenderMovingHeadOpenGL MovingHead { get; private set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Initialize(int referenceHeight)
		{
			// Create the moving head OpenGL implementation
			_movingHeadOpenGL = new MovingHeadOpenGL();
			
			// Initialize the moving head
			_movingHeadOpenGL.Initialize(CalculateDrawingLength(), referenceHeight);

			// Expose the moving head as a property
			MovingHead = _movingHeadOpenGL;			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void UpdateVolumes(int maxBeamLength, int referenceHeight)
		{
			// Calculate the height of the drawing area
			int fixtureHeight = Bottom - Top;

			// Calculate the width of the drawing area
			int fixtureWidth = Right - Left;
			
			// Calculate the position of the fixture
			int translateX = (int)(Left + fixtureWidth / 2.0);
			int translateY = (int)(Top + fixtureHeight / 2.0);
			
			// Update Y coordinate for (0,0) being at the top of the screen
			translateY = (int)(referenceHeight - translateY);

			// Update the volumes on the OpenGL implementation
			_movingHeadOpenGL.UpdateVolumes(
				translateX,
				translateY,
				maxBeamLength);
		}

		#endregion
	}
}