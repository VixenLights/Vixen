using Common.Controls.ColorManagement.ColorModels;
using OpenTK;
using QuickFont;
using QuickFont.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VixenModules.Preview.VixenPreview.Fixtures.Geometry;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Shaders;
using VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes;
using Rectangle = VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Volumes.Rectangle;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL
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
		/// The following fields determine if the static volumes are dirty.
		/// </summary>
		double _previousHorizontalRotation = -1;
		double _previousVerticalRotation = -1;
		double _previousXPosition = -1;
		double _previousYPosition = -1;

		/// <summary>
		/// The following fields determine if the dynamic volumes are dirty.
		/// </summary>
		double _previousLength = -1;
		double _previousMaxBeamLength = -1;
		int _previousFocus = -1;
		double _previousBeamLength = -1;
		Color _previousBeamColor = Color.Transparent;
		bool _previousOnOff = false;
		int _previousIntensity = -1;

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
					 _previousBeamColor == MovingHead.BeamColor &&
					 _previousOnOff == MovingHead.OnOff &&
					 _previousIntensity == MovingHead.Intensity);
		}

		/// <summary>
		/// Updates the dynamic volumes for geometry changes.
		/// </summary>
		/// <param name="length">Length of the light beam</param>
		/// <param name="maxBeamLength">Maximum length of the beam</param>
		/// <param name="beamTransparency">Transparency of the light beam</param>
		private void UpdateDynamicVolumes(double length, double maxBeamLength, double beamTransparency)
		{
			// Determine the current length of the light beam
			double lightSimulationLength = maxBeamLength * MovingHead.BeamLength / 100.0; 
			
			// Determine the bottom radius of the light beam
			double lightBottomRadius = _geometry.GetLightBeamBottomRadius(); 
			
			// Determine the top radius of the light beam
			double lightTopRadius = _geometry.GetLightBeamTopRadius();  

			// Adjust the top radius for the focus
			lightTopRadius = lightBottomRadius + lightTopRadius * (MovingHead.Focus / 100.0);

			// Convert the RGB color to HSV format
			HSV hsv = HSV.FromRGB(MovingHead.BeamColor);

			// Set the brightness value
			hsv.V *= MovingHead.Intensity / 100.0;

			// Convert the HSV color back to RGB
			Color beamColor = hsv.ToRGB();
			
			// If the beam has not been created then...
			if (_beamVolumes.Count == 0)
			{
				// Create the light beam with the specified color
				_beamVolumes.Add(new BeamRotatingCylinderWithEndCaps(
					0.0f,
					(float) lightSimulationLength,
					(float) lightBottomRadius,
					(float) lightTopRadius,					
					true));

				// Set the beam transparency
				((ISpecifyVolumeTransparency)_beamVolumes[0]).Transparency = beamTransparency;

				// Set the light beam color
				((ISpecifyVolumeColor)_beamVolumes[0]).Color = beamColor;
			}
			// Otherwise the light beam volume has been created
			else
			{
				// Determine if the light beam should be visible
				_beamVolumes[0].Visible = MovingHead.OnOff;

				// Set the light beam color
				((ISpecifyVolumeColor)_beamVolumes[0]).Color = beamColor;

				// Update the geometry on the light beam
				((IUpdateCylinder)_beamVolumes[0]).Update(
					0.0f,
					(float)lightSimulationLength,
					(float)lightBottomRadius,
					(float)lightTopRadius);					
			}
		}

		/// <summary>
		/// Initialize the static volumes making up the moving head fixture.
		/// </summary>
		/// <param name="length">Width and height of the drawing area</param>
		private void InitializeStaticVolumes(double length)
		{
			// Store off the width and height of the drawing area
			_length = length;

			// Create the base of the fixture
			_grayVolumes.Add(new Rectangle(
				(float)_geometry.GetBaseWidth(),
				(float)_geometry.GetBaseHeight(),
				(float)_geometry.GetBaseDepth(),
				false));

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
				false));
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
			
			// Position the moving heads behind the X-Y plan so that they don't obscure the pixel props
			float zPosition = (float)(-2.0 * _geometry.GetBaseDepth());

			// Configure the position of the base
			_grayVolumes[0].Position = new Vector3((float)xPosition, (float)(-_geometry.GetBottomOfViewport() + yPosition), zPosition);

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
				(float)(-_geometry.GetBottomOfViewport() + _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[1].Position = new Vector3(
				(float)(-_geometry.GetSupportXOffset()),
				(float)(_geometry.GetSupportHeight()),
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
				(float)(-_geometry.GetBottomOfViewport() + _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[2].Position = new Vector3(
				(float)(_geometry.GetSupportXOffset()),
				(float)(_geometry.GetSupportHeight()),
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
				(float)(-_geometry.GetBottomOfViewport() + _geometry.GetBaseHeight() + yPosition),
				zPosition);
			_grayVolumes[3].Position = new Vector3(
				0,
				(float)(_geometry.GetLightHousingLength() * _geometry.GetLightHousingPercentageAboveHorizontalSupport()),
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
				(float)(-_geometry.GetBottomOfViewport() + _geometry.GetBaseHeight() + yPosition),
				zPosition);

			_grayVolumes[4].Position = new Vector3(
				0,
				0,
				0);

			// Tilt the light housing
			((IRotatableCylinder)_grayVolumes[4]).TiltRotation = new Vector3(tiltAngleRadians, 0, 0);
			((IRotatableCylinder)_grayVolumes[4]).TiltTranslation = new Vector3(
				0,
				(float)(_geometry.GetLightHousingLength() * _geometry.GetLightHousingPercentageAboveHorizontalSupport()),
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

			// Position the light beam
			double lightHousingYOffset = _geometry.GetLightHousingLength() *
										 _geometry.GetLightHousingPercentageBelowHorizontalSupport();
			double endOfLightHousing = _geometry.GetLightHousingLength() - lightHousingYOffset;
			
			((CylinderWithEndCaps)_beamVolumes[0]).GroupTranslation = new Vector3(
				(float)xPosition,
				(float)(-_geometry.GetBottomOfViewport() + _geometry.GetBaseHeight() + yPosition),
				zPosition);

			_beamVolumes[0].Position = new Vector3(
				0,
				(float)endOfLightHousing,
				0);

			// Tilt the light beam
			((IRotatableCylinder)_beamVolumes[0]).TiltRotation = new Vector3(tiltAngleRadians, 0, 0);
			((IRotatableCylinder)_beamVolumes[0]).TiltTranslation = new Vector3(
				0,
				(float)(_geometry.GetLightHousingLength() * _geometry.GetLightHousingPercentageAboveHorizontalSupport()),
				0);			
		}

		#endregion

		#region IRenderMovingHeadOpenGL

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void Initialize(double length, double maxBeamLength, double beamTransparency)
		{
			// Create the geometry constants object
			_geometry = new MovingHeadGeometryConstants(length);

			// Create the static volumes
			InitializeStaticVolumes(length);

			// Save off the beam transparency setting
			_beamTransparency = beamTransparency;

			// Create / update the dynamic volumes
			UpdateDynamicVolumes(length, maxBeamLength, beamTransparency);			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public IEnumerable<Tuple<IVolume, Guid>> GetVolumes()
		{
			// Retrieve the gray static volumes
			List<Tuple<IVolume, Guid>> volumes = _grayVolumes.Select(volume => new Tuple<IVolume, Guid>(volume, GrayVolumeShader.ShaderID)).ToList();
			
			// Add the colored volumes
			volumes.AddRange(_beamVolumes.Select(volume => new Tuple<IVolume, Guid>(volume, ColorVolumeShader.ShaderID)));
			
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
				// Update the position and rotation of the static volumes
				UpdateFrame(MovingHead.PanAngle, MovingHead.TiltAngle + 180.0, translateX, translateY);
			}

			// If the dynamic volumes are dirty then...
			if (AreDynamicVolumesDirty(_length, maxBeamLength))
			{
				// Update the position and rotation of the dynamic volumes
				UpdateDynamicVolumes(_length, maxBeamLength, _beamTransparency);
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
					(float)(translateY + _geometry.GetBaseLegendYPosition() + _geometry.GetBaseHeight()),
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
				QFontDrawing.DisposeStaticState();
				_qFontDrawing = null;
			}						
		}

		#endregion
	}
}
