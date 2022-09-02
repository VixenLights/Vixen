using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VixenModules.Preview.VixenPreview.Fixtures.Geometry;
using Color = System.Drawing.Color;

namespace VixenModules.Editor.FixtureGraphics.WPF
{
	/// <summary>
	/// Creates a 3-D moving head fixture using WPF.
	/// </summary>
	/// <remarks>
	/// Originally this class was intended to provide moving head support to the GDI Preview
	/// but there were performance issues converting the WPF to a bitmap.  This class is still
	/// used to support editing the preview.
	/// </remarks>
	public class MovingHeadWPF 
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MovingHeadWPF()
		{
			// Create the moving head settings
			MovingHead = new MovingHeadSettings();

			// Default the beam length to something that looks reasonable in the edit preview
			MovingHead.BeamLength = 20.0;

			// Default the beam to On, yellow and full intensity				
			MovingHead.OnOff = true;
			MovingHead.BeamColor = Color.Yellow;
			MovingHead.Intensity = 100;

			// Default the beam to max beam area
			MovingHead.Focus = 100;
			
			// Include the legend by default
			MovingHead.IncludeLegend = true;

			// Default the legend text to red
			MovingHead.LegendColor = Color.Red;
		}

		#endregion
		
		#region Public Properties

		/// <summary>
		/// This property maintains the settings of the moving head graphics.
		/// </summary>
		public IMovingHead MovingHead { get; set; }

		#endregion
		
		#region Private Fields

		// Extra fields are used in this class in an attempt to improve performance.
		// When rendering the moving head the goal was to create as few objects as possible.

		// Maintains the geometry of the shapes that make up the moving head fixture.
		// These are member variables so that we only have to create them once on initialization.
		private GeometryModel3D _baseGeometry;
		private GeometryModel3D _baseLegendGeometry;
		private GeometryModel3D _lightHousingGeometry;
		private GeometryModel3D _lightBeamGeometry;

		/// <summary>
		/// Translates the base.  This transform is not used in postage stamp mode.		
		/// </summary>
		private TranslateTransform3D _baseTranslateTransform;
		
		/// <summary>
		/// This is the pan rotation of the moving head.
		/// </summary>
		private RotateTransform3D _baseRotateTransform;

		/// <summary>
		/// Group of transforms for the base (pan and translate).
		/// </summary>
		private Transform3DGroup _baseTransformsGroup;

		/// <summary>
		/// This is the tilt transform group.  This transform involves
		/// both a translate and a rotation.
		/// </summary>
		private Transform3DGroup _tiltTransforms;

		// Geometry Model Groups.
		// To reduce object creation there are group with and without the light beam geometry.		
		private Model3DGroup _mainModelWithBeamGroup;
		private Model3DGroup _mainModelWithoutBeamGroup;
		private Model3DGroup _baseModelGroup;
		private Model3DGroup _sidesModelGroup;
		private Model3DGroup _lightWithBeamModelGroup;
		private Model3DGroup _lightWithoutBeamModelGroup;		
		private Model3DGroup _lightAndSupportsWithBeamModelGroup;
		private Model3DGroup _lightAndSupportsWithoutBeamModelGroup;

		/// <summary>
		/// Top level model with the light beam.
		/// </summary>
		private ModelVisual3D _modelWithBeamVisual3D;

		/// <summary>
		/// Top level model without the light beam.
		/// </summary>
		private ModelVisual3D _modelWithoutBeamVisual3D;

		/// <summary>
		/// Defines the perspective for viewing the moving head.
		/// </summary>
		private PerspectiveCamera _camera;

		/// <summary>
		/// Defines the lighting for rendering the moving head.
		/// </summary>
		private AmbientLight _ambientLight;
		private DirectionalLight _directionalLight;

		/// <summary>
		/// Viewport for rendering the moving head WITH the light beam.
		/// </summary>
		private Viewport3D _viewportWithBeam;

		/// <summary>
		/// Viewport for rendering the moving head without the light beam.
		/// </summary>
		private Viewport3D _viewportWithoutBeam;

		/// <summary>
		/// Center of the light housing tilt rotation.
		/// </summary>
		private Point3D _centerOfLightHousingRotation;

		/// <summary>
		/// Length of the light beam.  Used to determine when the beam length has changed.
		/// </summary>	
		double _beamLength;

		/// <summary>
		/// Focus of the light beam.
		/// </summary>
		int _beamFocus;

		/// <summary>
		/// Intensity of the light beam.
		/// </summary>
		int _beamIntensity;

		/// <summary>
		/// Color of the light beam;
		/// </summary>
		Color _beamColor = Color.Transparent;

		/// <summary>
		/// Legend text to determine if the legend is stale and needs to be recreated.
		/// </summary>
		string _legendText;

		/// <summary>
		/// Legend color to determine if the legend is stale and needs to be recreated.
		/// </summary>
		Color _legendColor;

		/// <summary>
		/// Flag used to remember that the fixture geometry has already been established.
		/// </summary>
		private bool _initializedFixtureGeometry = false;

		/// <summary>
		/// Matrix of pixels.  This matrix is an intermediate step from the RenderTargetBitmap to PixelColors property.
		/// </summary>
		private PixelColor[,] _bmpPixels;

		/// <summary>
		/// Rotates the base of the moving head.
		/// </summary>
		private AxisAngleRotation3D _baseRotation;

		/// <summary>
		/// Flag that indicates the cached bitmap is stale and needs to be regenerated.
		/// </summary>
		bool _refresh = false;

		#endregion

		#region Private Methods

		/// <summary>
		/// Adds a cylinder with the specified radius to the geometry mesh.
		/// </summary>
		/// <param name="mesh">Geometry mesh to add the cylinder to</param>
		/// <param name="endPoint">End point of the cylinder</param>
		/// <param name="axis">Axis the cylinder is to wrap</param>
		/// <param name="bottomRadius">Bottom radius of the cylinder</param>
		/// <param name="topRadius">Top radius of the cylinder</param>
		/// <param name="numberOfSides">Number of sides on the cylinder</param>
		private void AddCylinder(MeshGeometry3D mesh, Point3D endPoint, Vector3D axis, double bottomRadius, double topRadius, int numberOfSides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1Bottom;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1Bottom = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1Bottom = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2Bottom = Vector3D.CrossProduct(v1Bottom, axis);

            // Make the vectors have length radius.
            v1Bottom *= (bottomRadius / v1Bottom.Length);
            v2Bottom *= (bottomRadius / v2Bottom.Length);

            Vector3D v1Top;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
	            v1Top = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
	            v1Top = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2Top = Vector3D.CrossProduct(v1Top, axis);

            // Make the vectors have length radius.
            v1Top *= (topRadius / v1Top.Length);
            v2Top *= (topRadius / v2Top.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / numberOfSides;
            for (int i = 0; i < numberOfSides; i++)
            {
                Point3D p1 = endPoint +
                             Math.Cos(theta) * v1Bottom +
                             Math.Sin(theta) * v2Bottom;
                Point3D p1Top = endPoint + 
                                Math.Cos(theta) * v1Top +
                                Math.Sin(theta) * v2Top;

                theta += dtheta;
                Point3D p2 = endPoint + Math.Cos(theta) * v1Bottom + Math.Sin(theta) * v2Bottom;
                Point3D p2Top = endPoint + 
                                Math.Cos(theta) * v1Top +
                                Math.Sin(theta) * v2Top;

                AddTriangle(mesh, endPoint, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = endPoint + axis;
            theta = 0;
            for (int i = 0; i < numberOfSides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * v1Bottom +
                    Math.Sin(theta) * v2Bottom;
                Point3D p1Top = end_point2 +
                             Math.Cos(theta) * v1Top +
                             Math.Sin(theta) * v2Top;

                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * v1Bottom +
                    Math.Sin(theta) * v2Bottom;
                Point3D p2Top = end_point2 +
                             Math.Cos(theta) * v1Top +
                             Math.Sin(theta) * v2Top;
                AddTriangle(mesh, end_point2, p2Top, p1Top);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < numberOfSides; i++)
            {
                Point3D p1 = endPoint +
                             Math.Cos(theta) * v1Bottom +
                             Math.Sin(theta) * v2Bottom;
                Point3D p1Top = endPoint +
                                Math.Cos(theta) * v1Top +
                                Math.Sin(theta) * v2Top;
                theta += dtheta;
                Point3D p2 = endPoint +
                             Math.Cos(theta) * v1Bottom +
                             Math.Sin(theta) * v2Bottom;
                Point3D p2Top = endPoint +
                                Math.Cos(theta) * v1Top +
                                Math.Sin(theta) * v2Top;

                Point3D p3 = p1Top + axis;
                Point3D p4 = p2Top + axis;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }
        }

		/// <summary>
		/// Add the specified triangle to the specified geometry mesh.
		/// </summary>
		/// <param name="mesh">Geometry mesh to add the triangle to</param>
		/// <param name="point1">1st point of the triangle</param>
		/// <param name="point2">2nd point of the triangle</param>
		/// <param name="point3">3rd point of the triangle</param>
		/// <remarks>Do not reuse points so triangles don't share normals.</remarks>
		private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
	        // Create the points
	        int index1 = mesh.Positions.Count;
	        mesh.Positions.Add(point1);
	        mesh.Positions.Add(point2);
	        mesh.Positions.Add(point3);

	        // Add the triangle indices
	        mesh.TriangleIndices.Add(index1++);
	        mesh.TriangleIndices.Add(index1++);
	        mesh.TriangleIndices.Add(index1);
        }

		/// <summary>
		/// Creates the cylinder for the light housing.
		/// </summary>
		/// <param name="length">Length of the rectangle we are drawing in</param>
		/// <param name="numberOfCylinderSides">Number of sides on the cylinder</param>
		/// <param name="geometry">Geometry of the fixture</param>
		/// <returns>Geometry of the light housing</returns>
		private GeometryModel3D CreateLightHousing(
			double length, 
			int numberOfCylinderSides,
			IMovingHeadGeometryConstants geometry)
		{
			// Create the geometry mesh for the light housing
			MeshGeometry3D mesh = new MeshGeometry3D();

			// Add the light housing cylinder
			AddCylinder(
				mesh,
				new Point3D(0, geometry.GetLightHousingYPosition(), 0),
				new Vector3D(0, geometry.GetLightHousingLength(), 0),
				geometry.GetLightHousingRadius(),
				geometry.GetLightHousingRadius(),
				numberOfCylinderSides);

			// Color the light housing dark gray
			SolidColorBrush brush = new SolidColorBrush(GetGrayTint()); 

			DiffuseMaterial material = new DiffuseMaterial(brush);
			GeometryModel3D lightHousingGeometry = new GeometryModel3D(mesh, material);

			return lightHousingGeometry;
		}

		/// <summary>
		/// Creates the light beam geometry.
		/// </summary>
		/// <param name="length">Length of the rectangle we are drawing in</param>
		/// <param name="numberOfCylinderSides">Number of sides on the cylinder</param>
		/// <param name="maxBeamLength">Maximum length of the light beam</param>
		/// <param name="geometry">Geometry of the fixture</param>
		/// <returns>Light beam geometry</returns>
		private GeometryModel3D CreateLightBeam(
			double length, 
			int numberOfCylinderSides, 
			double maxBeamLength,
			IMovingHeadGeometryConstants geometry)
		{
			// Make a cylinder along the Y axis for the light beam
			MeshGeometry3D lightBeamMesh = new MeshGeometry3D();

			// Retrieve light beam top radius which we are using as a delta on top of the bottom radius
			double lightTopRadius = geometry.GetLightBeamTopRadius();
			
			// Retrieve light beam bottom radius
			double lightBottomRadius = geometry.GetLightBeamBottomRadius();

			// Calculate the light beam top radius taking into account the fixture focus
			lightTopRadius = lightBottomRadius + lightTopRadius * (MovingHead.Focus / 100.0);

			// Add the light beam
			AddCylinder(
				lightBeamMesh,
				new Point3D(0, geometry.GetLightHousingYPosition() + geometry.GetLightHousingLength(), 0),
				new Vector3D(0, maxBeamLength  * MovingHead.BeamLength / 100.0, 0),
				geometry.GetLightBeamBottomRadius(),
				lightTopRadius,
				numberOfCylinderSides);

			// Convert the RGB color to HSV format
			HSV hsv = HSV.FromRGB(MovingHead.BeamColor);

			// Update the beam color for the fixture intensity 
			hsv.V *= MovingHead.Intensity / 100.0;

			// Convert the HSV color back to RGB
			Color color = hsv.ToRGB();

			// Get the beam color taking into account the intensity
			System.Windows.Media.Color newColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

			// Color the light beam
			SolidColorBrush brush = new SolidColorBrush(newColor); 
			DiffuseMaterial material = new DiffuseMaterial(brush);
			GeometryModel3D lightBeamGeometry = new GeometryModel3D(lightBeamMesh, material);

			return lightBeamGeometry;
		}

		/// <summary>
		/// Creates the light housing horizontal support.
		/// </summary>
		/// <param name="length">Length of the rectangle we are drawing in</param>
		/// <param name="numberOfSides">Number of sides on the cylinder</param>
		/// <param name="geometry">Geometry of the fixture</param>
		/// <returns>Horizontal support for the light housing</returns>
		private GeometryModel3D CreateHorizontalSupport(
			double length, 
			int numberOfSides,
			IMovingHeadGeometryConstants geometry)
		{
			// Make a cylinder along the X axis.
			MeshGeometry3D mesh2 = new MeshGeometry3D();
			AddCylinder(mesh2,
				new Point3D(-geometry.GetSupportXOffset(), geometry.GetHorizontalCylinderYPosition(), 0),
				new Vector3D(2 * geometry.GetSupportXOffset(), 0, 0),
				geometry.GetHorizontalCylinderRadius(),
				geometry.GetHorizontalCylinderRadius(),
				numberOfSides);

			// Color the horizontal support gray			
			SolidColorBrush brush = new SolidColorBrush(GetGrayTint());

			DiffuseMaterial material = new DiffuseMaterial(brush);
			GeometryModel3D model2 = new GeometryModel3D(mesh2, material);

			return model2;
		}

		/// <summary>
		/// Gets the gray color taking into acount the fixture intensity.
		/// </summary>
		/// <returns>Gray color taking into account the fixture intensity</returns>
		private System.Windows.Media.Color GetGrayTint()
		{
			// Convert the RGB color to HSV format
			HSV hsv = HSV.FromRGB(Color.Gray);

			// Update the beam color for the fixture intensity 
			hsv.V *= MovingHead.FixtureIntensity / 255.0;

			// Convert the HSV color back to RGB
			Color color = hsv.ToRGB();

			// Get the beam color taking into account the intensity
			return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Creates the side supports for the light housing.
		/// </summary>
		/// <param name="length">Length of the rectangle we are drawing in</param>
		/// <param name="geometry">Geometry of the fixture</param>
		/// <returns>Side support model group</returns>
		private Model3DGroup CreateSideSupports(
			double length,
			IMovingHeadGeometryConstants geometry)
		{
			// Create model group for the sides of the fixture
			Model3DGroup sidesModelGroup = new Model3DGroup();

			// Create the left support
			sidesModelGroup.Children.Add(
				CreateVerticalSupport(
					-geometry.GetSupportXOffset(),
					geometry.GetSupportYPosition(),
					geometry.GetSupportWidth(),
					geometry.GetSupportHeight(),
					geometry.GetSupportBaseDepth(),
					geometry.GetSupportTopDepth()));

			// Create the right support
			sidesModelGroup.Children.Add(
				CreateVerticalSupport(
					geometry.GetSupportXOffset(),
					geometry.GetSupportYPosition(),
					geometry.GetSupportWidth(),
					geometry.GetSupportHeight(),
					geometry.GetSupportBaseDepth(),
					geometry.GetSupportTopDepth()));

			return sidesModelGroup;
		}

		/// <summary>
		/// Position the camera view point.
		/// </summary>
		/// <param name="camera">Camera to position</param>
		private void PositionCamera(PerspectiveCamera camera, int zPositionOfCamera)
		{
			// Initialize the position of the camera
			int x = 0;
			int y = 0;
			int z = zPositionOfCamera;

			// Position the camera
			camera.Position = new Point3D(0, 0, zPositionOfCamera);

			// Look toward the origin.
			camera.LookDirection = new Vector3D(-x, -y, -z);

			// Set the Up direction of the camera
			camera.UpDirection = new Vector3D(0, 1, 0);
		}

		/// <summary>
		/// Defines the lights that help render the fixture.
		/// </summary>
		/// <param name="positionOfLight">Position of the light in the 3D coordinate system</param>
		private void DefineLights(Vector3D positionOfLight)
		{
			_ambientLight = new AmbientLight(Colors.Gray);
			_directionalLight = new DirectionalLight(Colors.Gray,  positionOfLight);
		}
		
		/// <summary>
		/// Creates the base of the moving head fixture.
		/// </summary>
		/// <param name="width">Width of the base</param>
		/// <param name="height">Height of the base</param>
		/// <param name="depth">Depth of the base</param>
		/// <param name="yOffset">Y Offset from the center of the coordinate system</param>		
		/// <returns>Fixture base geometry</returns>
		private GeometryModel3D CreateBase(double width, double height, double depth, double yOffset)
		{
			// Create the base
			return CreateCube(width, height, depth, 0, yOffset);			
		}

		/// <summary>
		/// Creates the legend underneath the base.
		/// </summary>
		/// <param name="charcterHeight">Height of the font</param>
		/// <param name="xPosition">X position of the legend</param>
		/// <param name="yPosition">Y position of the legend</param>
		/// <returns>Base Label Geometry</returns>
		private GeometryModel3D CreateBaseLabel(double charcterHeight, double xPosition, double yPosition)
		{
			// Convert the color to the System.Windows.Media namespace format
			System.Windows.Media.Color newLegendColor = System.Windows.Media.Color.FromArgb(
				MovingHead.LegendColor.A, 
				MovingHead.LegendColor.R, 
				MovingHead.LegendColor.G, 
				MovingHead.LegendColor.B);

			// Create and return the legend geometry			
			return CreateTextLabel3D(
				MovingHead.Legend,
				new SolidColorBrush(newLegendColor),
				false,
				charcterHeight,
				new Point3D(xPosition, yPosition, 0),
				false,
				new Vector3D(1, 0, 0),
				new Vector3D(0, 1, 0));
		}

		/// <summary>
		/// Creates a 3-D cube with the specified width, height, and depth.
		/// </summary>
		/// <param name="width">Width of the cube</param>
		/// <param name="height">Height of the cube</param>
		/// <param name="depth">Depth of the cube</param>
		/// <param name="xOffset">X offset of the cube</param>
		/// <param name="yOffset">Y offset of the cube</param>
		/// <returns>Cube geometry model</returns>
		private GeometryModel3D CreateCube(double width, double height, double depth, double xOffset, double yOffset)
		{
			// Create the mesh geometry
			MeshGeometry3D cube = new MeshGeometry3D();

			// Add the corners of the cube
			Point3DCollection corners = new Point3DCollection();
			corners.Add(new Point3D(width + xOffset, height + yOffset, depth));
			corners.Add(new Point3D(-width + xOffset, height + yOffset, depth));
			corners.Add(new Point3D(-width + xOffset, -height + yOffset, depth));
			corners.Add(new Point3D(width + xOffset, -height + yOffset, depth));
			corners.Add(new Point3D(width + xOffset, height + yOffset, -depth));
			corners.Add(new Point3D(-width + xOffset, height + yOffset, -depth));
			corners.Add(new Point3D(-width + xOffset, -height + yOffset, -depth));
			corners.Add(new Point3D(width + xOffset, -height + yOffset, -depth));

			// Assign the vertices to the mesh
			cube.Positions = corners;

			// Create the indices of the cube
			Int32[] indices =
			{
				0,1,2, // Front
				0,2,3,
				4,7,6, // Back
				4,6,5,
				4,0,3, // Right
				4,3,7,
				1,5,6, // Right
				1,6,2,
				1,0,4, // Top
				1,4,5,
				2,6,7, // Bottom
				2,7,3
			};

			// Create a collection of indices
			Int32Collection triangles = new Int32Collection();
			foreach (Int32 index in indices)
			{
				triangles.Add(index);
			}

			// Assign the indices to the mesh
			cube.TriangleIndices = triangles;

			// Create the model 3D for the cube
			GeometryModel3D cubeGeometryModel = new GeometryModel3D();
			MeshGeometry3D cubeMesh = cube;
			cubeGeometryModel.Geometry = cubeMesh;

			// Color the cube Dark Gray
			cubeGeometryModel.Material = new DiffuseMaterial(new SolidColorBrush(GetGrayTint()));
			
			return cubeGeometryModel;
		}

		/// <summary>
		/// Creates a vertical support for the light housing.
		/// </summary>
		/// <param name="supportXOffset">X offset of the vertical support</param>
		/// <param name="supportYOffset">Y offset of the vertical support</param>
		/// <param name="supportWidth">Width of the vertical support</param>
		/// <param name="supportHeight">Height of the vertical support</param>
		/// <param name="supportBaseDepth">Base depth of the vertical support</param>
		/// <param name="supportTopDepth">Top depth of the vertical support</param>
		/// <returns></returns>
		private GeometryModel3D CreateVerticalSupport(double supportXOffset, double supportYOffset, double supportWidth, double supportHeight, double supportBaseDepth, double supportTopDepth)
		{
			// Create the mesh geometry
			MeshGeometry3D cube = new MeshGeometry3D();

			// Add the corners of the cube
			Point3DCollection corners = new Point3DCollection();
			corners.Add(new Point3D(supportWidth + supportXOffset, supportHeight + supportYOffset, supportTopDepth));
			corners.Add(new Point3D(-supportWidth + supportXOffset, supportHeight + supportYOffset, supportTopDepth));
			corners.Add(new Point3D(-supportWidth + supportXOffset, -supportHeight + supportYOffset, supportBaseDepth));
			corners.Add(new Point3D(supportWidth + supportXOffset, -supportHeight + supportYOffset, supportBaseDepth));
			corners.Add(new Point3D(supportWidth + supportXOffset, supportHeight + supportYOffset, -supportTopDepth));
			corners.Add(new Point3D(-supportWidth + supportXOffset, supportHeight + supportYOffset, -supportTopDepth));
			corners.Add(new Point3D(-supportWidth + supportXOffset, -supportHeight + supportYOffset, -supportBaseDepth));
			corners.Add(new Point3D(supportWidth + supportXOffset, -supportHeight + supportYOffset, -supportBaseDepth));

			// Assign the vertices to the mesh
			cube.Positions = corners;

			// Create the indices of the support
			Int32[] indices =
			{
				0,1,2, // Front
				0,2,3,
				4,7,6, // Back
				4,6,5,
				4,0,3, // Right
				4,3,7,
				1,5,6, // Left
				1,6,2,
				1,0,4, // Top
				1,4,5,
				2,6,7, // Bottom
				2,7,3
			};

			// Create a collection of indices
			Int32Collection Triangles = new Int32Collection();
			foreach (Int32 index in indices)
			{
				Triangles.Add(index);
			}

			// Assign the indices to the mesh
			cube.TriangleIndices = Triangles;

			// Create the model 3D for the support
			GeometryModel3D support = new GeometryModel3D();
			MeshGeometry3D supportMesh = cube;
			support.Geometry = supportMesh;

			// Color the support Dark Gray
			support.Material = new DiffuseMaterial(new SolidColorBrush(GetGrayTint()));

			return support;
		}

		/// <summary>
		/// Creates a label geometry model with the specified text.
		/// </summary>
		/// <param name="text">The string to be drawn</param>
		/// <param name="textColor">The color of the text.</param> 
		/// <param name="isDoubleSided">Visible from both sides?</param> 
		/// <param name="height">Height of the characters</param> 
		/// <param name="basePoint">The base point of the label</param> 
		/// <param name="isBasePointCenterPoint">if set to <c>true</c> the base point 
		/// is center point of the label.</param> 
		/// <param name="vectorOver">Horizontal direction of the label</param>  
		/// <param name="vectorUp">Vertical direction of the label</param>  
		/// <returns>Suitable for adding to your Viewport3D</returns>
		/// <remarks>Two vectors: vectorOver and vectorUp are creating the surface
		/// on which we are drawing the text. Both those vectors are used for 
		/// calculation of label size, so it is reasonable that each co-ordinate 
		/// should be 0 or 1. e.g. [1,1,0] or [1,0,1], etc...</remarks> 
		/// <returns>Geometry model for a text legend</returns>
		/// <remarks>This code was based on the following web page:
		/// https://www.codeproject.com/Articles/33893/WPF-Creation-of-Text-Labels-for-3D-Scene
		/// </remarks>
		private GeometryModel3D CreateTextLabel3D(
			string text,
			Brush textColor,
			bool isDoubleSided,
			double height,
			Point3D basePoint,
			bool isBasePointCenterPoint,
			Vector3D vectorOver,
			Vector3D vectorUp)
		{
			TextBlock textblock = new TextBlock(new Run(text));
			textblock.Foreground = textColor; // setting the text color
											  //textblock.Background = new SolidColorBrush(System.Windows.Media.Colors.Gray);
			textblock.FontFamily = new FontFamily("Arial"); // setting the font to be used

			textblock.Measure(new System.Windows.Size(text.Length * height, height));

			textblock.Arrange(new Rect(0, 0, text.Length * height, height));
			
			DiffuseMaterial mataterialWithLabel = new DiffuseMaterial(); //new SolidColorBrush(System.Windows.Media.Colors.Green));
																		 // Allows the application of a 2-D brush,
																		 // like a SolidColorBrush or TileBrush, to a diffusely-lit 3-D model.
																		 // we are creating the brush from the TextBlock

			VisualBrush myVisualBrush = new VisualBrush(textblock);
			//myVisualBrush.AutoLayoutContent = true;
			mataterialWithLabel.Brush = myVisualBrush;

			//calculation of text width (assuming that characters are square):
			double width = text.Length * height;
			// we need to find the four corners
			// p0: the lower left corner; p1: the upper left
			// p2: the lower right; p3: the upper right
			Point3D p0 = basePoint;
			// when the base point is the center point we have to set it up in different way
			if (isBasePointCenterPoint)
				p0 = basePoint - width / 2 * vectorOver - height / 2 * vectorUp;
			Point3D p1 = p0 + vectorUp * 1 * height;
			Point3D p2 = p0 + vectorOver * width;
			Point3D p3 = p0 + vectorUp * 1 * height + vectorOver * width;
			// we are going to create object in 3D now:
			// this object will be painted using the (text) brush created before
			// the object is rectangle made of two triangles (on each side).
			MeshGeometry3D mg_RestangleIn3D = new MeshGeometry3D();
			mg_RestangleIn3D.Positions = new Point3DCollection();
			mg_RestangleIn3D.Positions.Add(p0); // 0
			mg_RestangleIn3D.Positions.Add(p1); // 1
			mg_RestangleIn3D.Positions.Add(p2); // 2
			mg_RestangleIn3D.Positions.Add(p3); // 3
												// when we want to see the text on both sides:
			if (isDoubleSided)
			{
				mg_RestangleIn3D.Positions.Add(p0); // 4
				mg_RestangleIn3D.Positions.Add(p1); // 5
				mg_RestangleIn3D.Positions.Add(p2); // 6
				mg_RestangleIn3D.Positions.Add(p3); // 7
			}
			mg_RestangleIn3D.TriangleIndices.Add(0);
			mg_RestangleIn3D.TriangleIndices.Add(3);
			mg_RestangleIn3D.TriangleIndices.Add(1);
			mg_RestangleIn3D.TriangleIndices.Add(0);
			mg_RestangleIn3D.TriangleIndices.Add(2);
			mg_RestangleIn3D.TriangleIndices.Add(3);
			// when we want to see the text on both sides:
			if (isDoubleSided)
			{
				mg_RestangleIn3D.TriangleIndices.Add(4);
				mg_RestangleIn3D.TriangleIndices.Add(5);
				mg_RestangleIn3D.TriangleIndices.Add(7);
				mg_RestangleIn3D.TriangleIndices.Add(4);
				mg_RestangleIn3D.TriangleIndices.Add(7);
				mg_RestangleIn3D.TriangleIndices.Add(6);
			}

			// texture coordinates must be set to
			// stretch the TextBox brush to cover
			// the full side of the 3D label.
			mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
			mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
			mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
			mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));
			// when the label is double sided:
			if (isDoubleSided)
			{
				mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
				mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));
				mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
				mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
			}

			return new GeometryModel3D(mg_RestangleIn3D, mataterialWithLabel);
		}


		/// <summary>
		/// Initialize the geometry model of the moving head fixture.
		/// </summary>
		/// <param name="length">Length of the rectangle we are drawing in</param>
		/// <param name="maxBeamLength">Maximum beam length</param>
		/// <param name="xOffset">X offset of the moving head</param>
		/// <param name="yOffset">Y offset of the moving head</param>
		/// <param name="viewportWithBeam">(Optional) viewport with beam</param>
		private void InitializeModel(
			double length, 
			double maxBeamLength,
			int xOffset,
			int yOffset,
			Viewport3D viewportWithBeam)			
		{
			// Create the geometry used to create the moving head fixture
			IMovingHeadGeometryConstants geometry = new MovingHeadGeometryConstants(length);

			// Create the 3-D Visual
			_modelWithoutBeamVisual3D = new ModelVisual3D();
			_modelWithBeamVisual3D = new ModelVisual3D();

			// Create the top level 3-D group
			_mainModelWithoutBeamGroup = new Model3DGroup();
			_mainModelWithBeamGroup = new Model3DGroup();

			// Create the 3-D viewports
			
			// Check to see if a viewport was provided by the caller
			if (viewportWithBeam != null)
			{
				// The viewport with beam is being used in XAML controls to create static graphics
				_viewportWithBeam = viewportWithBeam;
			}
			else
			{
				// A generic viewport is used to support the preview
				_viewportWithBeam = new Viewport3D();
			}
			
			_viewportWithoutBeam = new Viewport3D();

			// Create the camera used to view the fixture
			_camera = new PerspectiveCamera();

			// Associate the camera with the viewport
			_viewportWithBeam.Camera = _camera;
			_viewportWithoutBeam.Camera = _camera;

			// Give the camera its field of view and position
			_camera.FieldOfView = 45;
			PositionCamera(_camera, 10);

			// Define the lighting used to render the fixture
			DefineLights(new Vector3D(0, 0, -10)); //-1.0, -3.0, -2.0));

			// Associate the lights with main 3-D group
			_mainModelWithoutBeamGroup.Children.Add(_ambientLight);
			_mainModelWithBeamGroup.Children.Add(_ambientLight);
			_mainModelWithoutBeamGroup.Children.Add(_directionalLight);
			_mainModelWithBeamGroup.Children.Add(_directionalLight);

			// Add the main group of models to the visual
			_modelWithBeamVisual3D.Content = _mainModelWithBeamGroup;
			_modelWithoutBeamVisual3D.Content = _mainModelWithoutBeamGroup;

			// Create the base of the fixture
			_baseGeometry = CreateBase(
				geometry.GetBaseWidth(),
				geometry.GetBaseHeight(),
				geometry.GetBaseDepth(),
				geometry.GetBaseYPosition());

			// Create the base of the label
			_baseLegendGeometry = CreateBaseLabel(
				geometry.GetLegendCharacterHeight(),
				geometry.GetBaseLegendXPosition(),
				geometry.GetBaseLegendYPosition());
				
			// Create a 3-D group to hold the base and the label
			_baseModelGroup = new Model3DGroup();

			// Add the base and label to the group
			_baseModelGroup.Children.Add(_baseGeometry);
			_baseModelGroup.Children.Add(_baseLegendGeometry);

			// Create a translate transform for the base
			_baseTranslateTransform = new TranslateTransform3D();

			// Initialize the base translate transform
			_baseTranslateTransform.OffsetX = xOffset;
			_baseTranslateTransform.OffsetY = yOffset;
			_baseTranslateTransform.OffsetZ = 0;

			// Assign the translate transform to the base and the base label
			_baseGeometry.Transform = _baseTranslateTransform;
			_baseLegendGeometry.Transform = _baseTranslateTransform;

			// Define the number of faces for the cylinders
			const int NumberOfSidesOnCylinders = 16;

			// Create the light housing cylinder
			_lightHousingGeometry = CreateLightHousing(length, NumberOfSidesOnCylinders, geometry);

			// Create the light beam
			_lightBeamGeometry = CreateLightBeam(length, NumberOfSidesOnCylinders, maxBeamLength, geometry);

			// Create the side supports of the light housing
			_sidesModelGroup = CreateSideSupports(length, geometry);

			// Draw the horizontal support that holds up the light housing
			GeometryModel3D horizontalSupportGeometry = CreateHorizontalSupport(length, NumberOfSidesOnCylinders, geometry);

			// Add the horizontal support to the sides model group
			_sidesModelGroup.Children.Add(horizontalSupportGeometry);

			// Create a rotation transform so that the light fixture can be rotated
			// Rotating about the Y axis
			_baseRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 1);
			_baseRotateTransform = new RotateTransform3D(_baseRotation);

			// Create a group for holding the base and the base transform
			_baseTransformsGroup = new Transform3DGroup();
			_baseTransformsGroup.Children.Add(_baseRotateTransform);
			_baseTransformsGroup.Children.Add(_baseTranslateTransform);

			// Set the center of rotation for the light housing
			_centerOfLightHousingRotation = new Point3D(0, 0 + geometry.GetHorizontalCylinderYPosition(), 0);

			// Create a model group of the light housing
			// There is only geometry in this group but it gives us a target to apply the tilt rotation
			_lightWithoutBeamModelGroup = new Model3DGroup();
			_lightWithoutBeamModelGroup.Children.Add(_lightHousingGeometry);
			
			// Create a model group of the light housing and light beam
			_lightWithBeamModelGroup = new Model3DGroup();
			_lightWithBeamModelGroup.Children.Add(_lightHousingGeometry);
			_lightWithBeamModelGroup.Children.Add(_lightBeamGeometry);

			// Create a group that contains the light supports and light housing
			// This group gives us a target for the pan rotation
			_lightAndSupportsWithoutBeamModelGroup = new Model3DGroup();
			_lightAndSupportsWithoutBeamModelGroup.Children.Add(_sidesModelGroup);
			_lightAndSupportsWithoutBeamModelGroup.Children.Add(_lightWithoutBeamModelGroup);

			// Create a group that contains the light supports, light housing, and light beam
			// This group gives us a target for the pan rotation
			_lightAndSupportsWithBeamModelGroup = new Model3DGroup();
			_lightAndSupportsWithBeamModelGroup.Children.Add(_sidesModelGroup);
			_lightAndSupportsWithBeamModelGroup.Children.Add(_lightWithBeamModelGroup);

			// Assign the base transforms to the base and light support group
			_lightAndSupportsWithoutBeamModelGroup.Transform = _baseTransformsGroup;
			_lightAndSupportsWithBeamModelGroup.Transform = _baseTransformsGroup;
						
			// Create the main group without the light beam
			_mainModelWithoutBeamGroup.Children.Add(_baseModelGroup);
			_mainModelWithoutBeamGroup.Children.Add(_lightAndSupportsWithoutBeamModelGroup);
			
			// Create the main group with the light beam
			_mainModelWithBeamGroup.Children.Add(_baseModelGroup);
			_mainModelWithBeamGroup.Children.Add(_lightAndSupportsWithBeamModelGroup);

			// If the transforms group have not been created then...
			if (_tiltTransforms == null)
			{
				// Create the transforms group
				_tiltTransforms = new Transform3DGroup();

				// Rotation around X
				_tiltTransforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0)));

				// Rotation around Y 
				_tiltTransforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0)));

				// Rotation around Z
				_tiltTransforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0)));

				// Translate transform 
				_tiltTransforms.Children.Add(new TranslateTransform3D(0, 0, 0));
			}

			// Assign the tilt rotation transforms to the light model groups
			_lightWithBeamModelGroup.Transform = _tiltTransforms;
			_lightWithoutBeamModelGroup.Transform = _tiltTransforms;

			// Assign the visuals to the viewports
			_viewportWithoutBeam.Children.Add(_modelWithoutBeamVisual3D);
			_viewportWithBeam.Children.Add(_modelWithBeamVisual3D);
		}

		/// <summary>
		/// Updates the moving head geometry model.
		/// </summary>
		/// <param name="width">Width of the model</param>
		/// <param name="height">Height of the model</param>
		/// <param name="scaleFactor">Scale factor used when drawing</param>
		/// <param name="xOffset">X Offset to draw with</param>
		/// <param name="yOffset">Y Offset to draw with</param>
		/// <param name="cameraPosition">Position of the camera in the Z-axis</param>
		/// <param name="rtb">Rendeer target bitmap used to render the moving head</param>
		/// <param name="viewportWithBeam">Optional viewport with beam</param>
		private void UpdateModel3D(
			int width, 
			int height, 
			double scaleFactor, 
			int xOffset, 
			int yOffset, 
			int cameraPosition,
			RenderTargetBitmap rtb,
			Viewport3D viewportWithBeam)
		{
			// Determine the width of the drawing area
			// This equation calculates half of the drawing area in the X axis
			double length = Math.Tan(45.0 / 2.0 * Math.PI / 180.0) * cameraPosition;

			// Make the maximum beam length the entire display area
			double maxBeamLength = 2.0 * length;

			// Determine the height based on ratio between the height and the width
			double orgHeight = length * height / width;

			// Create the constants to help define the moving head geometry
			IMovingHeadGeometryConstants movingHeadConstants = new MovingHeadGeometryConstants(length);

			// If the fixture geometry has not been initialized then...
			if (!_initializedFixtureGeometry)
			{
				// Initialize the fixture model
				InitializeModel(length, maxBeamLength, xOffset, yOffset, viewportWithBeam);
				
				// Remember that we have initialized the model
				_initializedFixtureGeometry = true;
			}
			
			// If the beam parameters have changed then...
			if (_beamLength != MovingHead.BeamLength ||
				_beamFocus != MovingHead.Focus ||
				_beamIntensity != MovingHead.Intensity ||
				_beamColor != MovingHead.BeamColor)
			{
				// Store off the new beam length
				_beamLength = MovingHead.BeamLength;

				// Store off the new beam focus
				_beamFocus = MovingHead.Focus;

				// Store off the new beam color
				_beamColor = MovingHead.BeamColor;

				// Store off the new beam intensity
				_beamIntensity = MovingHead.Intensity;

				// Remove the previous beam geometry 
				_lightWithBeamModelGroup.Children.Remove(_lightBeamGeometry);

				// Create a new light beam geometry with the new length
				_lightBeamGeometry = CreateLightBeam(length, 16, maxBeamLength, movingHeadConstants);

				// Add the new light beam geometry to the group
				_lightWithBeamModelGroup.Children.Add(_lightBeamGeometry);
			}

			// If the legend is to be displayed and the base model group does not contain the legend then...				
			if (MovingHead.IncludeLegend && _baseModelGroup.Children.Count < 2)
			{
				// Add the legend to the group
				_baseModelGroup.Children.Add(_baseLegendGeometry);
			}
			// Otherwise if the Legend is NOT to be displayed and it is in the base model group then...
			else if (!MovingHead.IncludeLegend && _baseModelGroup.Children.Count == 2)
			{
				// Remove the legend from the group
				_baseModelGroup.Children.Remove(_baseLegendGeometry);
			}

			// If the legend text or legend color has changed then...				
			if (MovingHead.IncludeLegend &&
				(_legendText != MovingHead.Legend ||
					_legendColor != MovingHead.LegendColor))
			{
				// Remove the legend from the base model group
				_baseModelGroup.Children.Remove(_baseLegendGeometry);
					
				// Create a new base label
				_baseLegendGeometry = CreateBaseLabel(
					movingHeadConstants.GetLegendCharacterHeight(),
					movingHeadConstants.GetBaseLegendXPosition(),
					movingHeadConstants.GetBaseLegendYPosition());

				// Add the new label to the group
				_baseModelGroup.Children.Add(_baseLegendGeometry);

				// Save off the current label text
				_legendText = MovingHead.Legend;

				// Save off the legend color
				_legendColor = MovingHead.LegendColor;
			}

			// Set the pan angle on the rotation transformation
			_baseRotation.Angle = MovingHead.PanAngle;

			// Set the tilt angle
			SetTiltRotation(MovingHead.TiltAngle /* + 180.0*/, 0, 0);
						
			// If the window size has changed then...
			if (_viewportWithoutBeam.Width != width || _viewportWithoutBeam.Height != height || _refresh)
			{
				// Give the viewport the new dimensions
				_viewportWithoutBeam.Width = width;
				_viewportWithoutBeam.Height = height;
				_viewportWithBeam.Width = width;
				_viewportWithBeam.Height = height;

				// Layout the viewport to the specified width and height
				_viewportWithoutBeam.Measure(new Size(width, height));
				_viewportWithBeam.Measure(new Size(width, height));
				_viewportWithoutBeam.Arrange(new Rect(0, 0, width, height));
				_viewportWithBeam.Arrange(new Rect(0, 0, width, height));

				// Not creating bitmap when producing form graphics and animations
				if (rtb != null)
				{
					// Create the cached bitmap for the fixture using the specified width and height
					BitmapColors = new Color[(int)rtb.Width, (int)rtb.Height];

					// Create a matrix of Pixel Colors for the specified width and height
					_bmpPixels = new PixelColor[(int)rtb.Width, (int)rtb.Height];
				}
			}
		}

		/// <summary>
		/// Copy pixel data from PixelColor to color format.
		/// </summary>
		/// <param name="width">Width of matrix</param>
		/// <param name="height">Height of matrix</param>
		private void CopyPixelColorsToColorMatrix(int width, int height)
		{		
			// Convert the pixels into something Vixen can consume
			for (int x = 0; x < width; x++)
			{		
				for (int y = 0; y < height; y++)
				{
					BitmapColors[x, y] = Color.FromArgb(
						_bmpPixels[x, y].Alpha,
						_bmpPixels[x, y].Red,
						_bmpPixels[x, y].Green,
						_bmpPixels[x, y].Blue);
				}
			}
		}

		/// <summary>
		/// Sets the tilt rotation of the light housing.
		/// </summary>
		/// <param name="amountX">Angle to rotate around the X axis</param>
		/// <param name="amountY">Angle to rotate around the Y axis</param>
		/// <param name="amountZ">Angle to rotate around the Z axis</param>
		public void SetTiltRotation(double amountX, double amountY, double amountZ)
		{
			TranslateTransform3D translation = (TranslateTransform3D)_tiltTransforms.Children[3];

			Point3D translatedCenter = translation.Transform(_centerOfLightHousingRotation);

			RotateTransform3D rotX = (_tiltTransforms.Children[0] as RotateTransform3D);
			RotateTransform3D rotY = (_tiltTransforms.Children[1] as RotateTransform3D);
			RotateTransform3D rotZ = (_tiltTransforms.Children[2] as RotateTransform3D);

			// Set the center point of transformation to the translated center of the object
			rotX.CenterX = rotY.CenterX = rotZ.CenterX = translatedCenter.X;
			rotX.CenterY = rotY.CenterY = rotZ.CenterY = translatedCenter.Y;
			rotX.CenterZ = rotY.CenterZ = rotZ.CenterZ = translatedCenter.Z;

			// Apply the angle amounts
			(rotX.Rotation as AxisAngleRotation3D).Angle = amountX;
			(rotY.Rotation as AxisAngleRotation3D).Angle = amountY;
			(rotZ.Rotation as AxisAngleRotation3D).Angle = amountZ;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Cached image of the moving head fixture.
		/// </summary>
		public Color[,] BitmapColors
		{
			get;
			private set;
		}

		#endregion

		#region Public Methods
		
		/// <summary>
		/// Returns true if the calling thread has access to the moving head graphical objects.
		/// </summary>
		/// <returns></returns>
		public bool CheckAccess()
		{
			// Delegate the call one of the graphical objects
			return _baseRotation.CheckAccess();
		}
		
		/// <summary>
		/// Draws the moving head fixture into a bitmap or a viewport.
		/// </summary>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="scaleFactor">Scale factor of the drawing area</param>
		/// <param name="xOffset">X Offset into the drawing area for the fixture</param>
		/// <param name="yOffset">Y Offset into the drawing area for the fixture</param>
		/// <param name="viewportWithBeam">Optional viewport with beam</param>        
		public void DrawFixture(int width, int height, double scaleFactor, int xOffset, int yOffset, Viewport3D viewportWithBeam)
        {
			// Create the render target bitmap for the specified width and height
			// Caching this object is not a good option because when we redraw the bitmap would need 
			// to be cleared which is more costly than just creating a new bitmap.
			RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

			// Update the model for parameters that can change each frame
			UpdateModel3D(width, height, scaleFactor, xOffset, yOffset, 10, rtb, viewportWithBeam);
		
			// If the light beam is On then...
			if (MovingHead.OnOff)
			{
				// Render the viewport in the target bitmap
				rtb.Render(_viewportWithBeam);
			}
			else
			{
				// Render the viewport in the target bitmap
				rtb.Render(_viewportWithoutBeam);
			}

			// If the viewport width and height match the render target bitmap width and height then...
			if (_viewportWithoutBeam.Width == rtb.Width &&
				_viewportWithoutBeam.Height == rtb.Height)
			{
				// Copy the BitmapSource to PixelColors
				rtb.CopyPixelsVixen(_bmpPixels, (((int)rtb.Width) * rtb.Format.BitsPerPixel) / 8, 0);

				// Copy pixels to a format that easily mergeable into the preview
				CopyPixelColorsToColorMatrix((int)rtb.Width, (int)rtb.Height);
			}
			else
			{
				// Indicate that the cached bitmap needs to refreshed 
				_refresh = true;
			}		
        }

		/// <summary>
		/// Draws the moving head fixture into a bitmap or a viewport.
		/// </summary>
		/// <param name="width">Width of the drawing area</param>
		/// <param name="height">Height of the drawing area</param>
		/// <param name="scaleFactor">Scale factor of the drawing area</param>
		/// <param name="xOffset">X Offset into the drawing area for the fixture</param>
		/// <param name="yOffset">Y Offset into the drawing area for the fixture</param>
		/// <param name="viewportWithBeam">Optional viewport with beam</param>        
		public void DrawFixtureNoBitmap(int width, int height, double scaleFactor, int xOffset, int yOffset, Viewport3D viewportWithBeam)
		{			
			// Update the model for parameters that can change each frame
			UpdateModel3D(width, height, scaleFactor, xOffset, yOffset, 10, null, viewportWithBeam);			
		}

		/// <summary>
		/// Invalidates the geometry of the moving head.
		/// </summary>
		public void InvalidateGeometry()
		{
			// Set flags to force the moving head to redraw
			_initializedFixtureGeometry = false;
			_refresh = true;
		}

		#endregion
	}
}
