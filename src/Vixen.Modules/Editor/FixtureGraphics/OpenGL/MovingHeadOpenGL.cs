using Common.Controls.ColorManagement.ColorModels;
using QuickFont;
using QuickFont.Configuration;
using System.Drawing;
using OpenTK.Mathematics;
using VixenModules.Preview.VixenPreview.Fixtures.Geometry;
using VixenModules.Editor.FixtureGraphics.OpenGL.Shaders;
using VixenModules.Editor.FixtureGraphics.OpenGL.Volumes;
using Rectangle = VixenModules.Editor.FixtureGraphics.OpenGL.Volumes.Rectangle;

namespace VixenModules.Editor.FixtureGraphics.OpenGL
{	
	/// <summary>
	/// OpenGL implementation of a DMX moving head fixture.
	/// </summary>
	public class MovingHeadOpenGL : IRenderMovingHeadOpenGL, IDisposable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MovingHeadOpenGL()
		{			
			// If the QFont libraries have not been created then...
			if (_qFontDrawing == null)
			{
				// Load the font
				_qFont = new QFont(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\cour.ttf", 32, new QFontBuilderConfiguration(true));				
				_qFontDrawing = new QFontDrawing(); 
			}

			// Create the collections for the volumes
			_grayVolumes = new List<IVolume>();
			_beamVolumes = new List<IVolume>();

			// Create the settings class 
			MovingHead = new MovingHeadSettings();
		}

		#endregion
		
		#region Public Properties

		/// <summary>
		/// This property maintains the settings of the moving head graphics.
		/// </summary>
		public IMovingHead MovingHead { get; set; }

		#endregion

		#region Static Fields

		/// <summary>
		/// Font library used to draw the legend.
		/// </summary>
		private static QFont _qFont;

		/// <summary>
		/// Drawing library used to draw the legend.
		/// </summary>
		private static QFontDrawing _qFontDrawing;

		#endregion

		#region Fields

		/// <summary>
		/// Maintains geometry constants for the moving head fixture.
		/// </summary>
		private IMovingHeadGeometryConstants _geometry;

		/// <summary>
		/// These static volumes make up base and the housing of the light fixture.  They are constant and do NOT change with each frame.
		/// </summary>
		private List<IVolume> _grayVolumes;

		/// <summary>
		/// These objects make up the light fixture light beam.  Because the beam can change length and focus these vertices may
		/// change each frame.
		/// </summary>
		private List<IVolume> _beamVolumes;
		
		/// <summary>
		/// Width and height of the drawing area.
		/// </summary>
		private double _length;

		/// <summary>
		/// Transparency of the light beam.
		/// </summary>
		/// <remarks>This field allows the fixture body to dim as the background dims.</remarks>
		private double _beamTransparency;

		/// <summary>
		/// Beam width as a multiplier of the base beam width.
		/// </summary>
		private int _beamWidthMultiplier;

		/// <summary>
		/// The following fields determine if the static volumes are dirty.
		/// </summary>
		private double _previousHorizontalRotation = -1;
		private double _previousVerticalRotation = -1;
		private double _previousXPosition = -1;
		private double _previousYPosition = -1;

		/// <summary>
		/// The following fields determine if the dynamic volumes are dirty.
		/// </summary>
		private double _previousLength = -1;
		private double _previousMaxBeamLength = -1;
		private int _previousFocus = -1;
		private double _previousBeamLength = -1;
		private Color _previousBeamColor1 = Color.Transparent;
		private Color _previousBeamColor2 = Color.Transparent;
		private bool _previousOnOff = false;
		private int _previousIntensity = -1;

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns true if the dynamic volumes need to updated.
		/// </summary>
		/// <param name="length">Width and height of the drawing area</param>
		/// <param name="maxBeamLength">Maximum light beam length</param>
		/// <returns></returns>
		bool AreDynamicVolumesDirty(double length, double maxBeamLength)
		{
			// Return true if any of the parameters have changed since the last frame
			return !(_previousLength == length &&
					 _previousMaxBeamLength == maxBeamLength &&
					 _previousFocus == MovingHead.Focus &&
					 _previousBeamLength == MovingHead.BeamLength &&
					 _previousBeamColor1 == MovingHead.BeamColorLeft &&
					 _previousBeamColor2 == MovingHead.BeamColorRight &&
					 _previousOnOff == MovingHead.OnOff &&
					 _previousIntensity == MovingHead.Intensity);
		}

		/// <summary>
		/// Updates the dynamic volumes for geometry changes.
		/// </summary>
		/// <param name="length">Length of the light beam</param>
		/// <param name="maxBeamLength">Maximum length of the beam</param>
		/// <param name="beamTransparency">Transparency of the light beam</param>
		/// <param name="beamWidthMultiplier">Transparency of the light beam</param>
		private void UpdateDynamicVolumes(double length, double maxBeamLength, double beamTransparency, int beamWidthMultiplier)
		{
			// Determine the current length of the light beam
			double lightSimulationLength = maxBeamLength * MovingHead.BeamLength / 100.0; 
			
			// Determine the bottom radius of the light beam
			double lightBottomRadius = _geometry.GetLightBeamBottomRadius();

			// Determine the top radius of the light beam
			double lightTopRadius = beamWidthMultiplier * lightBottomRadius;

			// Adjust the top radius for the focus
			lightTopRadius = lightTopRadius * (MovingHead.Focus / 100.0);

			// Convert the RGB color to HSV format
			HSV hsvLeft = HSV.FromRGB(MovingHead.BeamColorLeft);

			// Set the brightness value
			hsvLeft.V *= MovingHead.Intensity / 100.0;

			// Convert the HSV color back to RGB
			Color beamColorLeft = hsvLeft.ToRGB();

			// Convert the RGB color to HSV format
			HSV hsvRight = HSV.FromRGB(MovingHead.BeamColorRight);

			// Set the brightness value
			hsvRight.V *= MovingHead.Intensity / 100.0;

			// Convert the HSV color back to RGB
			Color beamColorRight = hsvRight.ToRGB();

			// If the beam has not been created then...
			if (_beamVolumes.Count == 0)
			{
				// Create the light beam with the specified left color
				_beamVolumes.Add(new BeamRotatingCylinderWithEndCaps(
					0.0f,
					(float)lightSimulationLength,
					(float)lightBottomRadius,
					(float)lightTopRadius,
					true,
					BeamRotatingCylinderWithEndCaps.LeftHalfPanelIndices));

				// Create the light beam with the specified right color
				_beamVolumes.Add(new BeamRotatingCylinderWithEndCaps(
					0.0f,
					(float)lightSimulationLength,
					(float)lightBottomRadius,
					(float)lightTopRadius,
					true,
					BeamRotatingCylinderWithEndCaps.RightHalfPanelIndices));

				// Set the beam transparency
				((ISpecifyVolumeTransparency)_beamVolumes[0]).Transparency = beamTransparency;
				((ISpecifyVolumeTransparency)_beamVolumes[1]).Transparency = beamTransparency;

				// Set the light beam color
				((ISpecifyVolumeColor)_beamVolumes[0]).Color = beamColorLeft;
				((ISpecifyVolumeColor)_beamVolumes[1]).Color = beamColorRight;
			}
			// Otherwise the light beam volume has been created
			else
			{
				// Beam is always visible because when a true beam is not displayed
				// we are displaying a black disc
				_beamVolumes[0].Visible = true;
				_beamVolumes[1].Visible = true;	

				// Determine if the light beam should be visible
				// (Transparent is used for the default beam color for color mixing fixtures)
				if (MovingHead.OnOff &&
				    MovingHead.BeamColorLeft != Color.Transparent)
				{
					// Set the light beam color
					((ISpecifyVolumeColor)_beamVolumes[0]).Color = beamColorLeft;
					((ISpecifyVolumeColor)_beamVolumes[1]).Color = beamColorRight;

					// Update the geometry on the light beam left side
					((IUpdateCylinder)_beamVolumes[0]).Update(
						0.0f,
						(float)lightSimulationLength,
						(float)lightBottomRadius,
						(float)lightTopRadius,
						BeamRotatingCylinderWithEndCaps.LeftHalfPanelIndices);

					// Update the geometry on the light beam right side
					((IUpdateCylinder)_beamVolumes[1]).Update(
						0.0f,
						(float)lightSimulationLength,
						(float)lightBottomRadius,
						(float)lightTopRadius,
						BeamRotatingCylinderWithEndCaps.RightHalfPanelIndices);
				}
				else
				{
					// Turn the beam black
					((ISpecifyVolumeColor)_beamVolumes[0]).Color = Color.Black;
					((ISpecifyVolumeColor)_beamVolumes[1]).Color = Color.Black;

					// Set the beam transparency to 100% opaque
					((ISpecifyVolumeTransparency)_beamVolumes[0]).Transparency = 1.0;
					((ISpecifyVolumeTransparency)_beamVolumes[1]).Transparency = 1.0;

					// Update the geometry on the light beam left side
					((IUpdateCylinder)_beamVolumes[0]).Update(
						0.0f,
						(float)0.5, // Going smaller displayed artifacts
						(float)lightBottomRadius,
						(float)lightBottomRadius,
						BeamRotatingCylinderWithEndCaps.LeftHalfPanelIndices);
					
					// Update the geometry on the light beam right side
					((IUpdateCylinder)_beamVolumes[1]).Update(
						0.0f,
						(float)0.5, // Going smaller displayed artifacts
						(float)lightBottomRadius,
						(float)lightBottomRadius,
						BeamRotatingCylinderWithEndCaps.RightHalfPanelIndices);
				}
			}
		}

		/// <summary>
		/// Initialize the static volumes making up the moving head fixture.
		/// </summary>
		/// <param name="length">Width and height of the drawing area</param>
		/// <param name="mountingPosition">Mounting position of the fixture</param>
		private void InitializeStaticVolumes(double length, MountingPositionType mountingPosition)
		{
			// Store off the width and height of the drawing area
			_length = length;

			// Store off the mounting position
			MovingHead.MountingPosition = mountingPosition;

			// Create the base of the fixture
			_grayVolumes.Add(new Rectangle(
				(float)_geometry.GetBaseWidth(),
				(float)_geometry.GetBaseHeight(),
				(float)_geometry.GetBaseDepth(),
				false));

			// If the mounting position is at the bottom then...
			if (mountingPosition == MountingPositionType.Bottom)
			{
				// Create the left support of the fixture
				_grayVolumes.Add(new TaperedRectangle(
					(float)_geometry.GetSupportWidth(),
					(float)_geometry.GetSupportHeight(),
					(float)_geometry.GetSupportBaseDepth(),
					(float)_geometry.GetSupportTopDepth(),
					false));

				// Create the right support of the fixture
				_grayVolumes.Add(new TaperedRectangle(
					(float)_geometry.GetSupportWidth(),
					(float)_geometry.GetSupportHeight(),
					(float)_geometry.GetSupportBaseDepth(),
					(float)_geometry.GetSupportTopDepth(),
					false));
			}
			else
			{
				// Create the left support of the fixture
				_grayVolumes.Add(new TaperedRectangle(
					(float)_geometry.GetSupportWidth(),
					(float)_geometry.GetSupportHeight(),
					(float)_geometry.GetSupportTopDepth(),
					(float)_geometry.GetSupportBaseDepth(),
					false));

				// Create the right support of the fixture
				_grayVolumes.Add(new TaperedRectangle(
					(float)_geometry.GetSupportWidth(),
					(float)_geometry.GetSupportHeight(),
					(float)_geometry.GetSupportTopDepth(),
					(float)_geometry.GetSupportBaseDepth(),
					false));
			}

			// Create the horizontal support the holds the light housing
			_grayVolumes.Add(new Cylinder(
				(float)(2 * _geometry.GetSupportXOffset()),
				(float)_geometry.GetHorizontalCylinderRadius(),
				(float)_geometry.GetHorizontalCylinderRadius(),
				false));

			// Determine the Y offset for the part of the light housing that is below the horizontal support
			double lightHousingYOffset = _geometry.GetLightHousingLength() *
			                             _geometry.GetLightHousingPercentageBelowHorizontalSupport();
			// Create the light housing 
			_grayVolumes.Add(new RotatingCylinderWithEndCaps(
				(float)lightHousingYOffset,
				(float)_geometry.GetLightHousingLength(),
				(float)_geometry.GetLightHousingRadius(),
				(float)_geometry.GetLightHousingRadius(),
				false,
				Cylinder.AllPanelsIndices));
		}

		/// <summary>
		/// Returns true if the static 
		/// </summary>
		/// <param name="horizontalRotation">Horizontal rotation of light beam and supports</param>
		/// <param name="verticalRotation">Vertical pan of the light housing</param>
		/// <param name="xPosition">X Position of the fixture</param>
		/// <param name="yPosition">Y Position of the fixture</param>
		/// <returns></returns>
		private bool AreStaticVolumesDirty(double horizontalRotation, double verticalRotation, double xPosition, double yPosition)
		{
			// Return true if any of the parameters have changed since the last frame
			return !(_previousHorizontalRotation == horizontalRotation &&
					 _previousVerticalRotation == verticalRotation &&
					 _previousXPosition == xPosition &&
					 _previousYPosition == yPosition);
		}

		/// <summary>
		/// Updates the position of the volumes for the current frame.
		/// </summary>
		/// <param name="horizontalRotation">Horizontal rotation of the base in degrees</param>
		/// <param name="verticalRotation">Vertical tilt of the light housing in degrees</param>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		private void UpdateFrame(double horizontalRotation, double verticalRotation, double xPosition, double yPosition)
		{
			// Convert the horizontal rotation into radians
			float horizontalRotationRadians = (float)(horizontalRotation * Math.PI / 180.0f);
			
			// Convert the tilt angle rotation into radians
			float tiltAngleRadians = (float)(verticalRotation * Math.PI / 180.0f);
			
			// Position the moving heads behind the X-Y plane so that they don't obscure the pixel props
			float zPosition = (float)(-2.0 * _geometry.GetBaseDepth());

			// Configure the position of the base
			_grayVolumes[0].Position = new Vector3((float)xPosition, (float)(-1.0 * MovingHead.GetOrientationSign() * _geometry.GetBaseYPosition() + yPosition), zPosition);

			// Configure the left support
			UpdateFrameLeftSupport(xPosition, yPosition, horizontalRotationRadians, zPosition);

			// Configure the right support
			UpdateFrameRightSupport(xPosition, yPosition, horizontalRotationRadians, zPosition);

			// Configure the horizontal support
			UpdateFrameHorizontalSupport(xPosition, yPosition, horizontalRotationRadians, zPosition);

			// Configure the light housing
			UpdateFrameLightHousing(xPosition, yPosition, horizontalRotationRadians, zPosition, tiltAngleRadians);

			// Update the light beam
			UpdateFrameLightBeam(xPosition, yPosition, horizontalRotationRadians, zPosition, tiltAngleRadians);

			// Store off the current rotations
			_previousHorizontalRotation = horizontalRotation;
			_previousVerticalRotation = verticalRotation;

			// Store off the current position
			_previousXPosition = xPosition;
			_previousYPosition = yPosition;
		}

		/// <summary>
		/// Updates the position of the left support.
		/// </summary>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		/// <param name="horizontalRotationRadians">Horizontal rotation of the base</param>	
		/// <param name="zPosition">Z Position of the moving head</param>
		private void UpdateFrameLeftSupport(double xPosition, double yPosition, float horizontalRotationRadians, float zPosition)
		{			
			// Rotate the left support
			((TaperedRectangle)_grayVolumes[1]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);

			// Position the left support
			((TaperedRectangle)_grayVolumes[1]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-1.0 * MovingHead.GetOrientationSign() *_geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2 * _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[1].Position = new Vector3(
				(float)(-_geometry.GetSupportXOffset()),
				(float)(MovingHead.GetOrientationSign() * _geometry.GetSupportHeight()),
				0);
		}

		/// <summary>
		/// Updates the position of the right support.
		/// </summary>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		/// <param name="horizontalRotationRadians">Horizontal rotation of the base</param>	
		/// <param name="zPosition">Z Position of the moving head</param>
		private void UpdateFrameRightSupport(double xPosition, double yPosition, float horizontalRotationRadians, float zPosition)
		{
			// Rotate the right support
			((TaperedRectangle)_grayVolumes[2]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);
			
			// Position the right support
			((TaperedRectangle)_grayVolumes[2]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-1.0 * MovingHead.GetOrientationSign() *_geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2 * _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[2].Position = new Vector3(
				(float)(_geometry.GetSupportXOffset()),
				(float)(MovingHead.GetOrientationSign() * _geometry.GetSupportHeight()),
				0);
		}

		/// <summary>
		/// Updates the position of the horizontal support.
		/// </summary>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		/// <param name="horizontalRotationRadians">Horizontal rotation of the base</param>	
		/// <param name="zPosition">Z Position of the moving head</param>
		private void UpdateFrameHorizontalSupport(double xPosition, double yPosition, float horizontalRotationRadians, float zPosition)
		{
			// Rotate the horizontal support
			((CylinderBase)_grayVolumes[3]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);
			
			// Position the horizontal support
			((CylinderBase)_grayVolumes[3]).GroupTranslation = new Vector3(
				(float)xPosition,			
				(float)(-1.0 * MovingHead.GetOrientationSign() * _geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2.0 * _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[3].Position = new Vector3(
				0,
				(float)(MovingHead.GetOrientationSign() * _geometry.GetLightHousingLength() * 0.75),
				0);
		}

		/// <summary>
		/// Updates the position of the light housing.
		/// </summary>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		/// <param name="horizontalRotationRadians">Horizontal rotation of the base</param>	
		/// <param name="zPosition">Z Position of the moving head</param>
		/// <param name="tiltAngleRadians">Tilt of the light housing and light beam</param>
		private void UpdateFrameLightHousing(
			double xPosition, 
			double yPosition, 
			float horizontalRotationRadians, 
			float zPosition,
			float tiltAngleRadians)
		{
			// Rotate the light housing
			((CylinderWithEndCaps)_grayVolumes[4]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);
			
			// Position the light housing
			((CylinderWithEndCaps)_grayVolumes[4]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-1.0 * MovingHead.GetOrientationSign() * _geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2.0 *_geometry.GetBaseHeight() + yPosition),
				zPosition);

			_grayVolumes[4].Position = new Vector3(
				0,
				0,
				0);

			// Tilt the light housing
			((IRotatableCylinder)_grayVolumes[4]).TiltRotation = new Vector3(tiltAngleRadians, 0, 0);
			((IRotatableCylinder)_grayVolumes[4]).TiltTranslation = new Vector3(
				0,
				(float)(MovingHead.GetOrientationSign() * _geometry.GetLightHousingLength() * 0.75),
				0);			
		}

		/// <summary>
		/// Updates the position of the light beam.
		/// </summary>
		/// <param name="xPosition">X Position of the moving head</param>
		/// <param name="yPosition">Y Position of the moving head</param>
		/// <param name="horizontalRotationRadians">Horizontal rotation of the base</param>	
		/// <param name="zPosition">Z Position of the moving head</param>
		/// <param name="tiltAngleRadians">Tilt of the light housing and light beam</param>
		private void UpdateFrameLightBeam(
			double xPosition,
			double yPosition,
			float horizontalRotationRadians,
			float zPosition,
			float tiltAngleRadians)
		{
			// Rotate the light beam
			((CylinderWithEndCaps)_beamVolumes[0]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);
			((CylinderWithEndCaps)_beamVolumes[1]).GroupRotation = new Vector3(0, horizontalRotationRadians, 0);

			// Position the light beam
			double lightHousingYOffset = _geometry.GetLightHousingLength() *
										 _geometry.GetLightHousingPercentageBelowHorizontalSupport();
			double endOfLightHousing = _geometry.GetLightHousingLength() - lightHousingYOffset;
			
			((CylinderWithEndCaps)_beamVolumes[0]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-1.0 * MovingHead.GetOrientationSign() * _geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2 * _geometry.GetBaseHeight() + yPosition),
				zPosition);

			((CylinderWithEndCaps)_beamVolumes[1]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-1.0 * MovingHead.GetOrientationSign() * _geometry.GetBottomOfViewport() + MovingHead.GetOrientationSign() * 2 * _geometry.GetBaseHeight() + yPosition),
				zPosition);

			// Position the left side of the light beam
			_beamVolumes[0].Position = new Vector3(
				0,
				(float)endOfLightHousing,
				0);

			// Position the right side of the light beam
			_beamVolumes[1].Position = new Vector3(
				0,
				(float)endOfLightHousing,
				0);

			// Tilt the light beam left side
			((IRotatableCylinder)_beamVolumes[0]).TiltRotation = new Vector3(tiltAngleRadians, 0, 0);
			((IRotatableCylinder)_beamVolumes[0]).TiltTranslation = new Vector3(
				0,
				(float)(MovingHead.GetOrientationSign() * _geometry.GetLightHousingLength() * 0.75),
				0);

			// Tilt the light beam right side
			((IRotatableCylinder)_beamVolumes[1]).TiltRotation = new Vector3(tiltAngleRadians, 0, 0);
			((IRotatableCylinder)_beamVolumes[1]).TiltTranslation = new Vector3(
				0,
				(float)(MovingHead.GetOrientationSign() * _geometry.GetLightHousingLength() * 0.75),
				0);
		}

		#endregion

		#region IRenderMovingHeadOpenGL

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Initialize(double length, double maxBeamLength, double beamTransparency, int beamWidthMultiplier, MountingPositionType mountingPosition)
		{
			// Create the geometry constants object
			_geometry = new MovingHeadGeometryConstants(length);

			// Create the static volumes
			InitializeStaticVolumes(length, mountingPosition);

			// Save off the beam transparency setting
			_beamTransparency = beamTransparency;

			// Save off top of the beam width multiplier			
			_beamWidthMultiplier = beamWidthMultiplier;

			// Create / update the dynamic volumes
			UpdateDynamicVolumes(length, maxBeamLength, beamTransparency, beamWidthMultiplier);			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public IEnumerable<Tuple<IVolume, Guid>> GetVolumes()
		{
			// Retrieve the gray static volumes
			List<Tuple<IVolume, Guid>> volumes = _grayVolumes.Select(volume => new Tuple<IVolume, Guid>(volume, GrayVolumeShader.ShaderID)).ToList();
			
			// Add the colored volumes
			volumes.AddRange(_beamVolumes.Select(volume => new Tuple<IVolume, Guid>(volume, ColorVolumeShader1.ShaderID)));
			volumes.AddRange(_beamVolumes.Select(volume => new Tuple<IVolume, Guid>(volume, ColorVolumeShader2.ShaderID)));

			return volumes;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void UpdateVolumes(
			int translateX,
			int translateY,
			double maxBeamLength)			
		{
			// If the static volumes are dirty then...
			if (AreStaticVolumesDirty(MovingHead.PanAngle, MovingHead.TiltAngle, translateX, translateY))
			{
				// If the fixture is mounted towards the bottom then...
				if (MovingHead.MountingPosition == MountingPositionType.Bottom)
				{
					// Update the position and rotation of the static volumes
					UpdateFrame(MovingHead.PanAngle, MovingHead.TiltAngle + 180.0, translateX, translateY);
				}
				else
				{
					// Update the position and rotation of the static volumes
					UpdateFrame(MovingHead.PanAngle, MovingHead.TiltAngle , translateX, translateY);
				}
			}

			// If the dynamic volumes are dirty then...
			if (AreDynamicVolumesDirty(_length, maxBeamLength))
			{
				// Update the position and rotation of the dynamic volumes
				UpdateDynamicVolumes(_length, maxBeamLength, _beamTransparency, _beamWidthMultiplier);
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void DrawLegend(
			int translateX, 
			int translateY, 		
			Matrix4 projectionMatrix, 
			Matrix4 viewMatrix)
		{			
			// If the legend is to be drawn then...
			if (MovingHead.IncludeLegend)
			{
				// Clear the font drawing primitives
				_qFontDrawing.DrawingPrimitives.Clear();	
						
				// Determine the position of the legend text
				Vector3 positionOfText = new Vector3(
					(float)(translateX + _geometry.GetBaseLegendXPosition()),
					(float)(translateY -1.0 * _geometry.GetBottomOfViewport()),  
					(float)-_geometry.GetBaseDepth());
				
				// Draw the text
				_qFontDrawing.Print(_qFont, MovingHead.Legend, positionOfText, QFontAlignment.Left, MovingHead.LegendColor);
				_qFontDrawing.RefreshBuffers();
				_qFontDrawing.ProjectionMatrix = viewMatrix * projectionMatrix;				
				_qFontDrawing.Draw();
			}
		}

		#endregion

		#region IDisposable 

		/// <summary>
		/// Refer to MSDN documentation.
		/// </summary>
		public void Dispose()
		{			
			// If the QFont library has not been disposed of then...
			if (_qFont != null)
			{
				// Dispose of the library
				_qFont.Dispose();
				_qFont = null;
			}

			// If the QFont Drawing library has not been disposed of then...
			if (_qFontDrawing != null)
			{
				// Dispose of the library
				_qFontDrawing.Dispose();
				//QFontDrawing.DisposeStaticState();  //TODO .NET 6 Migration: figure out if this is needed! 
				_qFontDrawing = null;
			}						
		}

		#endregion
	}
}
