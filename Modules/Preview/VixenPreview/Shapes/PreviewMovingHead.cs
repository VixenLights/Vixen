using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Sys;
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

			// Default the movement constraints of the moving head
			PanStartPosition = DefaultPanStartPosition;
			PanStopPosition = DefaultPanStopPosition;
			TiltStartPosition = DefaultTiltStartPosition;
			TiltStartPosition = DefaultTiltStopPosition;

			// Default the beam length percentage
			BeamLength = DefaultBeamLength;

			// Default to partially transparent
			BeamTransparency = 40;
		}

		#endregion

		#region Constants

		/// <summary>
		/// Default pan start position in degrees.
		/// </summary>
		const int DefaultPanStartPosition = 0;

		/// <summary>
		/// Default pan stop position in degrees.
		/// </summary>
		const int DefaultPanStopPosition = 360;

		/// <summary>
		/// Default tilt start position in degrees.
		/// </summary>
		const int DefaultTiltStartPosition = 0;

		/// <summary>
		/// Default tilt stop position in degrees.
		/// </summary>
		const int DefaultTiltStopPosition = 180;

		/// <summary>
		/// Default beam length factor (50%).
		/// </summary>
		const int DefaultBeamLength = 50;

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

		/// <summary>
		/// Current moving head settings.  This property aids with determining if the graphics are stale.
		/// </summary>
		private IMovingHead _movingHeadCurrentSettings;

		/// <summary>
		/// Moving head intent handler for the shape.
		/// </summary>
		private MovingHeadIntentHandler _intentHandler;

		#endregion

		#region IDrawStaticPreviewShape

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void InitializeGDI()
		{
			// Create the WPF moving head grpahics
			CreateWPFMovingHead();
			
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
			// If an intent handler has been created then...
			if (_intentHandler != null)
			{
				// Retrieve the intents
				IIntentStates states = Node.Element.State;

				// Dispatch the current intents
				_intentHandler.Dispatch(states);
			}

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
			if (point.X >= Left * ZoomLevel &&
			    point.X <= Right * ZoomLevel &&
			    point.Y >= Top * ZoomLevel &&
			    point.Y <= Bottom * ZoomLevel)
			{
				// Indicate the point is inside the shape
				pointInShape = true;
			}
			
			return pointInShape;
		}

		/// <summary>
		/// Returns true if any corner of the specified rect is inside the moving head rectangle.
		/// </summary>
		/// <param name="rect">Rectangle to evaluate</param>
		/// <returns>True if the rectangle is inside the shape</returns>
		public override bool ShapeInRect(Rectangle rect)
		{
			// Create preview points for the rectangle
			PreviewPoint topLeft = new PreviewPoint(rect.X, rect.Y);
			PreviewPoint topRight = new PreviewPoint(rect.X + rect.Width, rect.Y);
			PreviewPoint bottomLeft = new PreviewPoint(rect.X, rect.Y + rect.Height);
			PreviewPoint bottomRight = new PreviewPoint(rect.X +  rect.Width, rect.Y + rect.Height);

			// Check to see if any of the points are inside the shape
			return PointInShape(topLeft) ||
				PointInShape(topRight) ||
				PointInShape(bottomLeft) ||
				PointInShape(bottomRight);
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
						// Create the WPF moving head graphics
						CreateWPFMovingHead();						
					}
					else
					{
						// If the moving head is associated with another thread
						if (!_movingHead.CheckAccess())
						{
							// Create a new moving head associated with this thread
							CreateWPFMovingHead();							
						}
					}
					
					// Draw the fixture using WPF
					// The X offset and Y offset were previously used when using the WPF graphics in the GDI Preview					
					_movingHead.DrawFixture(width, height, scaleFactor, 0, 0);

					// Update the settings associated with the bitmap.
					_prevousMovingHeadSettings = _movingHeadCurrentSettings.Clone();						
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

			// If the fixture has been selected then..
			if (selected)
			{
				// Draw the outline around the fixture
				DrawRectangleOutline(fp);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the intent handler for the moving head position.
		/// </summary>
		private void CreateIntentHandler()
		{
			// Create the intent handler
			_intentHandler = new MovingHeadIntentHandler();

			// Give the intent handler the current movement constraints
			_intentHandler.PanStartPosition = PanStartPosition;
			_intentHandler.PanStopPosition = PanStopPosition;
			_intentHandler.TiltStartPosition = TiltStartPosition;
			_intentHandler.TiltStopPosition = TiltStopPosition;

			// Give the intent handler the beam length scale factor
			_intentHandler.BeamLength = BeamLength;

			// Give the intent handler the current moving head settings
			_intentHandler.MovingHead = _movingHeadCurrentSettings;
		}

		/// <summary>
		/// Creates the WPF moving head graphics.
		/// </summary>
		private void CreateWPFMovingHead()
		{
			// Create the WPF moving head
			_movingHead = new MovingHeadWPF();

			// Assign the settings to the generic member variable that can switch between WPF and OpenGL
			_movingHeadCurrentSettings = _movingHead.MovingHead;

			// Create the intent handler
			CreateIntentHandler();
		}

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
				 _movingHeadCurrentSettings == null ||
				 !_prevousMovingHeadSettings.Equals(_movingHeadCurrentSettings));
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
			// Set the outline color to Hot Pink
			Color outlineColor = Color.HotPink;
			
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
		[Browsable(false)]
		public IRenderMovingHeadOpenGL MovingHead { get; private set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Initialize(int referenceHeight)
		{
			// Create the moving head OpenGL implementation
			_movingHeadOpenGL = new MovingHeadOpenGL();

			// Set the moving head settings to the OpenGL settings
			_movingHeadCurrentSettings = _movingHeadOpenGL.MovingHead;

			// Create the intent handler
			CreateIntentHandler();
			
			// Initialize the moving head
			_movingHeadOpenGL.Initialize(CalculateDrawingLength(), referenceHeight, BeamTransparency / 100.0);

			// Expose the moving head as a property
			MovingHead = _movingHeadOpenGL;			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void UpdateVolumes(int maxBeamLength, int referenceHeight)
		{
			// Turn off the moving head 
			_movingHeadOpenGL.MovingHead.OnOff = false;

			// If the associated element contains intents then...
			if (Node != null && 
				Node.Element != null && 
				Node.Element.State != null)
			{
				// Retrieve the intents from the element 
				IIntentStates states = Node.Element.State;
				
				// Dispatch the current intents
				_intentHandler.Dispatch(states);
			}

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

		#region Internal Methods

		/// <summary>
		/// Reconfigures the shape with an element.
		/// </summary>
		/// <param name="node">Element node associated with the shape</param>
		internal sealed override void Reconfigure(ElementNode node)
		{
			// Store off the element node associated with the shape
			_node = node;

			// If the node is not NULL then...
			if (node != null)
			{
				// Save off the node ID associated with the shape
				_nodeId = node.Id;
			}
		}

		#endregion

		#region Public Properties

		private ElementNode _node;

		/// <summary>
		/// Display element Node associated with the preview shape.
		/// </summary>
		[Browsable(false)]
		public ElementNode Node
		{
			get
			{
				if (_node == null)
				{
					_node = VixenSystem.Nodes.GetElementNode(NodeId);					
				}
				return _node;
			}
			set
			{
				if (value == null)
					NodeId = Guid.Empty;
				else
					NodeId = value.Id;
				_node = value;				
			}
		}
		
		private Guid _nodeId;

		/// <summary>
		/// Display element Node Id associated with the preview shape.
		/// </summary>
		[Browsable(false)]
		[DataMember(EmitDefaultValue = false)]
		public Guid NodeId
		{
			get { return _nodeId; }
			set
			{
				_nodeId = value;
				_node = VixenSystem.Nodes.GetElementNode(NodeId);				
			}
		}

		/// <summary>
		/// Pan start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("The pan starting point angle (in degrees)."),
		 DisplayName("Pan Start Position (Degrees)")]
		public int PanStartPosition { get; set; }

		/// <summary>
		/// Pan stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("The pan stopping point angle (in degrees)."),
		 DisplayName("Pan Stop Position (Degrees)")]
		public int PanStopPosition { get; set; }

		/// <summary>
		/// Tilt start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("The tilt starting point angle (in degrees)."),
		 DisplayName("Tilt Start Position (Degrees)")]
		public int TiltStartPosition { get; set; }

		/// <summary>
		/// Tilt stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("The tilt stopping point angle (in degrees)."),
		 DisplayName("Tilt Stop Position (Degrees)")]
		public int TiltStopPosition { get; set; }

		/// <summary>
		/// Beam length scale factor (1-100%).
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Length of the beam."),
		 DisplayName("Beam Length")]
		public int BeamLength
		{
			get;
			set;
		}

		/// <summary>
		/// Beam transparecny (1-100%).
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Beam transparency"),
		 DisplayName("Beam Transparency (%)")]
		public int BeamTransparency
		{
			get;
			set;
		}

		/// <summary>
		/// Alpha value of the preview background.
		/// </summary>
		/// <remarks>The moving head fixture dims with the background.</remarks>
		[Browsable(false)]
		public override int BackgroundAlpha 
		{ 
			get
			{
				return base.BackgroundAlpha;
			}
			set
			{
				// Forward the background alpha as the intensity of the fixture
				_movingHead.MovingHead.FixtureIntensity = value;
				
				// Invalidate the moving head so that it is redrawn
				_movingHead.InvalidateGeometry();
				
				// Let the base class store off the new background alpha
				base.BackgroundAlpha = value;
			}
		}

		#endregion
	}
}