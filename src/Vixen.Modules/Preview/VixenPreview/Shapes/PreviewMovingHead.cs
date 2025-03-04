using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;
using Common.Controls.Theme;
using OpenTK.Mathematics;
using Vixen.Data.Flow;
using Vixen.Sys;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixtureGraphics;
using VixenModules.Editor.FixtureGraphics.OpenGL;
using VixenModules.Editor.FixtureGraphics.WPF;
using VixenModules.OutputFilter.DimmingFilter;
using VixenModules.OutputFilter.ShutterFilter;
using VixenModules.Property.IntelligentFixture;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Enumeration to use in the shape property grid.
	/// </summary>
	public enum YesNoType
	{
		No,
		Yes,
	}

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

			// Default the strobe min and max rates
			StrobeRateMinimum = DefaultStrobeRateMinimum;
			StrobeRateMaximum = DefaultStrobeRateMaximum;

			// Default the pan and tilt maximum travel times
			MaxPanTravelTime = DefaultMaxPanTravelTime;
			MaxTiltTravelTime = DefaultMaxTiltTravelTime;

			// Default the beam length percentage
			BeamLength = DefaultBeamLength;

			// Default to partially transparent
			BeamTransparency = 40;

			// Default the top beam width to 8 times larger than the base width
			BeamWidthMultiplier = 8;

			// Default the legend to being on
			ShowLegend = true; 
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
		/// Default beam length factor (100%).
		/// </summary>
		const int DefaultBeamLength = 100;

		/// <summary>
		/// Default minimum strobe rate in Hz.
		/// </summary>
		private const int DefaultStrobeRateMinimum = 1;

		/// <summary>
		/// Default maximum strobe rate in Hz.
		/// </summary>
		private const int DefaultStrobeRateMaximum = 25;

		/// <summary>
		/// Default the strobe flash duration to 50ms.
		/// </summary>
		private const int DefaultMaxStrobeDuration = 50;

		/// <summary>
		/// Default time it takes to pan from starting position to max pan position.
		/// </summary>
		private const double DefaultMaxPanTravelTime = 4.6;

		/// <summary>
		/// Default time it takes to tilt from starting position to max tilt position.
		/// </summary>
		private const double DefaultMaxTiltTravelTime = 1.7;

		/// <summary>
		/// Default minimum color wheel rotation speed in seconds.
		/// </summary>
		private const double DefaultMinColorWheelRotationSpeed = 158.0;

		/// <summary>
		/// Default maximum color wheel rotation speed in seconds.
		/// </summary>
		private const double DefaultMaxColorWheelRotationSpeed = 1.0;

		/// <summary>
		/// Minimum Strobe Rate display name.
		/// </summary>
		private const string MinimumStrobeRateDisplayName = "Minimum Strobe Rate (ms)";

		/// <summary>
		/// Maximum Strobe Rate display name.
		/// </summary>
		private const string MaximumStrobeRateDisplayName = "Maximum Strobe Rate (Hz)";

		/// <summary>
		/// Pan Start Position display name.
		/// </summary>
		private const string PanStartDisplayName = "Pan Start Position (Degrees)";

		/// <summary>
		/// Pan Stop Position display name.
		/// </summary>
		private const string PanStopDisplayName = "Pan Stop Position (Degrees)";

		/// <summary>
		/// Tilt Start display name.
		/// </summary>
		private const string TiltStartDisplayName = "Tilt Start Position (Degrees)";

		/// <summary>
		/// Tilt Stop display name.
		/// </summary>
		private const string TiltStopDisplayName = "Tilt Stop Position (Degrees)";

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
		private IMovingHead _previousMovingHeadSettings;

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
			CreateWPFMovingHead(Color.Yellow);
			
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
			// If the legned is to be shown then...
			if (ShowLegend)
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
			Draw(fp, editMode, highlightedElements, selected, false, this.Locked, zoomLevel);
			
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

			// Turn on mouse move events
			IgnoreMouseMove = false;
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
		/// <returns>True if the rectangle is overlaps the shape</returns>
		public override bool ShapeInRect(Rectangle rect, bool allIn = false)
		{
			bool overlaps = false;

			int X1 = Math.Min(rect.X, rect.X + rect.Width);
			int X2 = Math.Max(rect.X, rect.X + rect.Width);
			int Y1 = Math.Min(rect.Y, rect.Y + rect.Height);
			int Y2 = Math.Max(rect.Y, rect.Y + rect.Height);
			
			if (_bottomLeft.X >= X1 &&
			    _bottomLeft.X <= X2 &&
			    _bottomLeft.Y >= Y1 &&
			    _bottomLeft.Y <= Y2)
			{
				overlaps = true;
			}
			else if (_bottomRight.X >= X1 &&
			         _bottomRight.X <= X2 &&
			         _bottomRight.Y >= Y1 &&
			         _bottomRight.Y <= Y2)
			{
				overlaps = true;
			}
			else if (_topLeft.X >= X1 &&
			         _topLeft.X <= X2 &&
			         _topLeft.Y >= Y1 &&
			         _topLeft.Y <= Y2)
			{
				overlaps = true;
			}
			else if (_topRight.X >= X1 &&
			         _topRight.X <= X2 &&
			         _topRight.Y >= Y1 &&
			         _topRight.Y <= Y2)
			{
				overlaps = true;
			}

			return overlaps;
		}

		/// <summary>
		/// Draws the shape for the GDI Preview and when editing the preview.
		/// </summary>
		public override void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected, bool forceDraw, bool locked, double zoomLevel)
		{			
			// This draws the four corners of the rectangle when the fixture is selected
			DrawSelectPoints(fp);

			// Save off the zoom level
			_zoomLevel = zoomLevel;

			// Calculate the width and height of the fixture graphic before scaling
			int widthOrg = Math.Abs(_bottomRight.X - _topLeft.X);
			int heightOrg = Math.Abs(_bottomRight.Y - _topLeft.Y);
						
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

			// Default the beam color to unlinked (white)
			Color beamColor = ThemeColorTable.Unlinked;

			// If the moving head is selected then...
			if (selected)
			{
				// Use the selected color
				beamColor = ThemeColorTable.Selected;
			}
			// If the moving head is selected in the elements tree then...
			else if (highlightedElements != null &&
				highlightedElements.Contains(NodeId))
			{
				// Make the beam color pink
				beamColor = ThemeColorTable.ElementSelected;
			}
			else if (locked)
			{
				beamColor = ThemeColorTable.Locked;
			}
			// If the fixture is linked to a node then...
			else if (Node != null)
			{
				// Make the beam color Turquoise
				beamColor = ThemeColorTable.Linked;
			}

			// If the mounting position has changed then...
			if (_previousMovingHeadSettings != null &&
				_previousMovingHeadSettings.MountingPosition != _movingHeadCurrentSettings.MountingPosition)
			{
				// Clear out the moving head so that it is re-created
				_movingHead = null;
			}

			// If the moving head has been created then...
			if (_movingHead != null)
			{
				// Set the beam color, otherwise the bitmap won't be updated				
				_movingHead.MovingHead.BeamColorLeft = beamColor;
				_movingHead.MovingHead.BeamColorRight = beamColor;
				_movingHeadCurrentSettings.BeamColorLeft = beamColor;
				_movingHeadCurrentSettings.BeamColorRight = beamColor;
			}

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
						CreateWPFMovingHead(beamColor);						
					}
					else
					{
						// If the moving head is associated with another thread
						if (!_movingHead.CheckAccess())
						{
							// Create a new moving head associated with this thread
							CreateWPFMovingHead(beamColor);							
						}
					}
					
					// Draw the fixture using WPF
					// The X offset and Y offset were previously used when using the WPF graphics in the GDI Preview					
					_movingHead.DrawFixture(width, height, scaleFactor, 0, 0, null);

					// Update the settings associated with the bitmap.
					_previousMovingHeadSettings = _movingHeadCurrentSettings.Clone();						
				}
			}

			// If the fixture graphic has been rendered and
			// the current size matches the rendered size then...
			if (_movingHead != null &&
				_movingHead.BitmapColors != null &&
				_movingHead.BitmapColors.Length == width * height)
			{
				// Copy the cached fixture bitmap into the preview
				CopyFixturePixelsIntoPreview(fp, width, height, _zoomLevel);
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
		/// <param name="redraw">Delegate to redraw the preview</param>
		private void CreateIntentHandler(Action redraw)
		{
			// Create the intent handler
			_intentHandler = new MovingHeadIntentHandler(redraw);

			// Give the intent handler the current movement constraints
			_intentHandler.PanStartPosition = PanStartPosition;
			_intentHandler.PanStopPosition = PanStopPosition;
			_intentHandler.TiltStartPosition = TiltStartPosition;
			_intentHandler.TiltStopPosition = TiltStopPosition;
			_intentHandler.InvertPanDirection = InvertPanDirection == YesNoType.Yes;
			_intentHandler.InvertTiltDirection = InvertTiltDirection == YesNoType.Yes;
			
			// Give the intent handler the strobe constraints
			_intentHandler.StrobeRateMinimum = StrobeRateMinimum;
			_intentHandler.StrobeRateMaximum = StrobeRateMaximum;

			// Give the intent handler the maximum pan and tilt travel times
			_intentHandler.MaxPanTravelTime = MaxPanTravelTime;	
			_intentHandler.MaxTiltTravelTime = MaxTiltTravelTime;

			// Give the intent handler the min and max color wheel rotation speeds
			_intentHandler.MinColorWheelRotationSpeed = MinColorWheelRotationSpeed;
			_intentHandler.MaxColorWheelRotationSpeed = MaxColorWheelRotationSpeed;

			// Give the intent handler the maximum strobe duration
			_intentHandler.MaximumStrobeDuration = MaximumStrobeDuration;

			// Configure how the fixture zooms
			_intentHandler.ZoomNarrowToWide = ZoomNarrowToWide;	

			// Give the intent handler the beam length scale factor
			_intentHandler.BeamLength = BeamLength;

			// Give the intent handler the current moving head settings
			_intentHandler.MovingHead = _movingHeadCurrentSettings;

			// If the node for the preview item has been deleted the Node will be null
			if (Node != null && Node.Element != null)
			{
				// Retrieve the data flow for the fixture node
				IDataFlowComponent dataFlow = VixenSystem.DataFlow.GetComponent(Node.Element.Id);

				// Loop over all the children of the node
				for (int i = 0; i < dataFlow.Outputs.Length; i++)
				{
					// Get the specified output
					IEnumerable<IDataFlowComponent> children =
						VixenSystem.DataFlow.GetDestinationsOfComponentOutput(dataFlow, i);
					
					// Look for children that are shutter filters
					foreach (IDataFlowComponent shutterFilter in children.Where(filter => filter is ShutterFilterModule))
					{
						// Copy the automation setting from the output filter to the preview intent handler
						_intentHandler.ConvertColorIntentsIntoShutter = ((ShutterFilterModule)shutterFilter).ConvertColorIntoShutterIntents;
					}

					// Look for children that are dimming filters
					foreach (IDataFlowComponent dimmingFilter in children.Where(filter => filter is DimmingFilterModule))
					{
						// Copy the automation setting from the output filter to the preview intent handler
						_intentHandler.ConvertColorIntentsIntoDimmer = ((DimmingFilterModule)dimmingFilter).ConvertColorIntoDimmingIntents;
					}
				}
			}
		}

		/// <summary>
		/// Creates the WPF moving head graphics.
		/// </summary>
		/// <param name="beamColor">Default beam color of the moving head</param>
		private void CreateWPFMovingHead(Color beamColor)
		{
			// Create the WPF moving head
			_movingHead = new MovingHeadWPF();

			// Assign the beam color
			_movingHead.MovingHead.BeamColorLeft = beamColor;
			_movingHead.MovingHead.BeamColorRight = beamColor;

			// Configure the moving head settings with the mounting position
			_movingHead.MovingHead.MountingPosition = MountingPosition; 

			// Assign the settings to the generic member variable that can switch between WPF and OpenGL
			_movingHeadCurrentSettings = _movingHead.MovingHead;

			// Create the intent handler
			CreateIntentHandler(null);
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
				_bottomRight.X = Left + h;
				_topRight.X = Left + h;
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
				 _previousMovingHeadSettings == null ||
				 _movingHeadCurrentSettings == null ||
				 !_previousMovingHeadSettings.Equals(_movingHeadCurrentSettings));
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

		/// <summary>
		/// Initializes the moving constraints (pan/tilt) of the moving head shape.
		/// </summary>
		/// <param name="selectedNode">Node associated with the moving head shape</param>
		private void InitializeMovingHeadMovementConstraints(ElementNode selectedNode)
		{
			// Default the movement constraints of the moving head
			PanStartPosition = DefaultPanStartPosition;
			PanStopPosition = DefaultPanStopPosition;
			TiltStartPosition = DefaultTiltStartPosition;
			TiltStopPosition = DefaultTiltStopPosition;

			if (selectedNode != null)
			{
				// Retrieve the intelligent fixture property from the node
				IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)selectedNode.Properties.SingleOrDefault(prp => prp is IntelligentFixtureModule);

				// If the property was found then...
				if (fixtureProperty != null)
				{
					// Default the pan start position
					PanStartPosition = DefaultPanStartPosition;

					// Retrieve the pan function from the fixture
					FixtureFunction panFunction = fixtureProperty.FixtureSpecification.FunctionDefinitions.FirstOrDefault(fn => fn.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Pan);

					// If the pan function was found then...
					if (panFunction != null)
					{
						PanStartPosition = panFunction.RotationLimits.StartPosition;
						PanStopPosition = panFunction.RotationLimits.StopPosition;
					}

					// Default the tilt start position
					TiltStartPosition = DefaultTiltStartPosition;

					// Retrieve the tilt function from the fixture
					FixtureFunction tiltFunction = fixtureProperty.FixtureSpecification.FunctionDefinitions.FirstOrDefault(fn => fn.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Tilt);

					// If the tilt function was found then...
					if (tiltFunction != null)
					{
						TiltStartPosition = tiltFunction.RotationLimits.StartPosition;
						TiltStopPosition = tiltFunction.RotationLimits.StopPosition;
					}

					// Retrieve the zoom function from the fixture
					FixtureFunction zoomFunction = fixtureProperty.FixtureSpecification.FunctionDefinitions.FirstOrDefault(fn => fn.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Zoom);

					// If a zoom function was found then...
					if (zoomFunction != null)
					{
						// Initialize the how the fixture zooms
						ZoomNarrowToWide = (zoomFunction.ZoomType == FixtureZoomType.NarrowToWide);
					}
				}			
			}

			// Default the beam length percentage
			BeamLength = DefaultBeamLength;
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
		public void Initialize(int referenceHeight, Action redraw)
		{
			// Create the moving head OpenGL implementation
			_movingHeadOpenGL = new MovingHeadOpenGL();

			// Set the moving head settings to the OpenGL settings
			_movingHeadCurrentSettings = _movingHeadOpenGL.MovingHead;

			// Create the intent handler
			CreateIntentHandler(redraw);
			
			// Initialize the moving head
			_movingHeadOpenGL.Initialize(CalculateDrawingLength(), referenceHeight, (100.0 - BeamTransparency) / 100.0, BeamWidthMultiplier, MountingPosition);

			// Expose the moving head as a property
			MovingHead = _movingHeadOpenGL;			
		}
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void UpdateVolumes(int maxBeamLength, int referenceHeight, bool standardFrame)
		{			
			// Reset the handler for the next frame
			_intentHandler.Reset();

			// If the associated element contains intents then...
			if (Node != null && 
				Node.Element != null && 
				Node.Element.State != null)
			{
				// Retrieve the intents from the element 
				IIntentStates states = Node.Element.State;

				// Give the intent handler a reference to the associated node
				_intentHandler.Node = Node;

				// Dispatch the current intents
				_intentHandler.Dispatch(states);
			}

			// After processing all the intents allow the intent handler
			// to configure fixture state
			_intentHandler.FinalizeFixtureState(standardFrame);

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

				// Transfer movement constraints from the Intelligent Fixture property associated with the node
				InitializeMovingHeadMovementConstraints(node);
			}
		}

		#endregion

		#region Public Properties

		private ElementNode _node;

		/// <summary>
		/// Display element Node associated with the preview shape.
		/// </summary>
		[Editor(typeof(PreviewSetFixtureElementUIEditor), typeof(UITypeEditor)),
		 Category("Settings"),
		 Description("Determines the Intelligent Fixture element the graphic is linked to.  Selecting the ‘…‘ button allows you to pick an Intelligent Fixture element."),
		 DisplayName("Linked Element")]
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
		/// Minimum color wheel rotation speed in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Color Wheel"),
		 Description("The time it takes the color wheel to make a complete rotation in seconds."),
		 DisplayName("Color Wheel Rotation Speed Minimum (s)")]
		public double MinColorWheelRotationSpeed { get; set; }

		/// <summary>
		/// Maximum color wheel rotation speed in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Color Wheel"),
		 Description("The time it takes the color wheel to make a complete rotation in seconds."),
		 DisplayName("Color Wheel Rotation Speed Maximum (s)")]
		public double MaxColorWheelRotationSpeed { get; set; }

		/// <summary>
		/// Maximum pan travel time in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The time it takes of the intelligent fixture to pan from the starting position to the maximum stop position."),
		 DisplayName("Maximum Pan Travel Time (s)")]
		public double MaxPanTravelTime { get; set; }

		/// <summary>
		/// Maximum tilt travel time in seconds.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The time it takes of the intelligent fixture to tilt from the starting position to the maximum stop position."),
		 DisplayName("Maximum Tilt Travel Time (s)")]
		public double MaxTiltTravelTime { get; set; }
		
		/// <summary>
		/// Strobe rate minimum in Hz.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		Category("Strobe"),
		Description("The strobe rate minimum (in Hz)."),
		DisplayName(MinimumStrobeRateDisplayName)]
		public int StrobeRateMinimum { get; set; }
		
		/// <summary>
		/// Strobe rate maximum in Hz.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Strobe"),
		 Description("The strobe rate maximum (in Hz)."),
		 DisplayName(MaximumStrobeRateDisplayName)]
		public int StrobeRateMaximum { get; set; }
		
		/// <summary>
		/// Strobe duration in ms.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Strobe"),
		 Description("The maximum strobe duration in ms."),
		 DisplayName("Maximum Strobe Duration (ms)")]
		public int MaximumStrobeDuration { get; set; }
		
		/// <summary>
		/// Pan start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The pan starting point angle (in degrees)."),
		 DisplayName(PanStartDisplayName)]
		public int PanStartPosition { get; set; }
		
		/// <summary>
		/// Pan stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("The pan stopping point angle (in degrees)."),
		 DisplayName("Pan Stop Position (Degrees)")]
		public int PanStopPosition { get; set; }

		/// <summary>
		/// Tilt start position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The tilt starting point angle (in degrees)."),
		 DisplayName(TiltStartDisplayName)]
		public int TiltStartPosition { get; set; }
		
		/// <summary>
		/// Tilt stop position in degrees.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("The tilt stopping point angle (in degrees)."),
		 DisplayName(TiltStopDisplayName)]
		public int TiltStopPosition { get; set; }

		/// <summary>
		/// Beam length scale factor (1-100%).
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Beam"),
		 Description("Length of the beam as a percentage of the background height."),
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
		 Category("Beam"),
		 Description("Determines the transparency of the light beam. 0% is completely opaque.  100% is completely transparent.  Note at 100% transparency the beam is not visible."),
		 DisplayName("Beam Transparency (%)")]
		public int BeamTransparency
		{
			get;
			set;
		}

		/// <summary>
		/// Beam width multiplier.  Determines the width at the top of the beam.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Beam"),
		 Description("Beam Width Multiplier"),
		 DisplayName("Beam Width Multiplier")]
		public int BeamWidthMultiplier
		{
			get;
			set;
		}

		/// <summary>
		/// Flag that indicates if zoom increases from narrow to wide.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Indicates whether the fixture zooms from a narrow beam to a wide beam or vice-versa"),
		 DisplayName("Zoom Narrow To Wide")]
		public bool ZoomNarrowToWide
		{
			get;
			set;
		}

		/// <summary>
		/// Flag that controls whether the function index legend is displayed.
		/// </summary>
		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("When true enables a legend that will show a function label and the corresponding channel value.  The legend is only applicable to index and range functions that were populated with a Preview Legend character."),
		 DisplayName("Show Legend")]
		public bool ShowLegend
		{
			get;
			set;
		}

		private YesNoType _invertPanDirection;

		[DataMember(EmitDefaultValue = false),
		 Category("Pan"),
		 Description("Changes the start point of the pan by 180 degrees and inverts the direction of movement.  This setting is often used with 'Top' (upside down) mounting position."),
		 DisplayName("Invert Pan Direction")]
		public YesNoType InvertPanDirection
		{
			get
			{
				return _invertPanDirection;
			}
			set
			{
				_invertPanDirection = value;

				if (_intentHandler != null)
				{
					// Give the intent handler the pan direction modifier
					_intentHandler.InvertPanDirection = (value == YesNoType.Yes);
				}
			}
		}

		private YesNoType _invertTiltDirection;

		[DataMember(EmitDefaultValue = false),
		 Category("Tilt"),
		 Description("Swaps the start position with the stop position and inverts the direction of movement.  This setting is often used with 'Top' (upside down) mounting position."),
		 DisplayName("Invert Tilt Direction")]
		public YesNoType InvertTiltDirection
		{
			get
			{
				return _invertTiltDirection;
			}
			set
			{
				_invertTiltDirection = value;

				if (_intentHandler != null)
				{
					// Give the intent handler the tilt direction modifier
					_intentHandler.InvertTiltDirection = (value == YesNoType.Yes);
				}
			}
		}


		private MountingPositionType _mountingPosition;

		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("Selects the mounting position of the fixture.  This property allows for simulating the fixture being mounted upside down."),
		 DisplayName("Mounting Position")]		
		public MountingPositionType MountingPosition
		{
			get
			{
				return _mountingPosition;
			}
			set
			{
				if (_movingHeadCurrentSettings != null)
				{
					// Give the moving head settings the mounting position
					_movingHeadCurrentSettings.MountingPosition = value;
				}
				_mountingPosition = value;
			}
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
				// Need to guard against moving heads that are not associated with a node
				if (_movingHead != null)
				{
					// Forward the background alpha as the intensity of the fixture
					_movingHead.MovingHead.FixtureIntensity = value;

					// Invalidate the moving head so that it is redrawn
					_movingHead.InvalidateGeometry();
				}
				
				// Let the base class store off the new background alpha
				base.BackgroundAlpha = value;
			}
		}

		#endregion

		#region Public Methods
		
		/// <inheritdoc/>				
		public void Validate(string propertyLabel)
		{
			// If the property being changed is Pan Start or Stop then...
			if (propertyLabel == PanStartDisplayName ||
				propertyLabel == PanStopDisplayName)
			{
				// If the Pan Start is greater than the Pan Stop then...
				if (PanStartPosition > PanStopPosition)
				{
					MessageBox.Show("The 'Pan Start Position' should be less than the 'Pan Stop Position'.  " + 
									"Please correct the values.", "Data Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			// If the property being changed is Tilt Start or Stop then...
			else if (propertyLabel == TiltStartDisplayName ||
					 propertyLabel == TiltStopDisplayName)
			{
				// If the Tilt Start is greater than the Tilt Stop then...
				if (TiltStartPosition > TiltStopPosition)
				{
					MessageBox.Show("The 'Tilt Start Position' should be less than the 'Tilt Stop Position'.  " + 
									"Please correct the values.", "Data Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			// If the property being changed is Strobe Rate then...
			else if (propertyLabel == MinimumStrobeRateDisplayName ||
					 propertyLabel == MaximumStrobeRateDisplayName)
			{
				// If the Strobe Minimum is greater than the Strobe Maximum then...
				if (StrobeRateMinimum > StrobeRateMaximum)
				{
					MessageBox.Show("The 'Strobe Rate Minimum' should be less than the 'Strobe Rate Maximum'.  " +
									"Please correct the values.", "Data Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		#endregion
	}
}