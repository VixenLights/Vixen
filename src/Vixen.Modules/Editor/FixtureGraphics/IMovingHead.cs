﻿using System.Drawing;

namespace VixenModules.Editor.FixtureGraphics
{
	/// <summary>
	/// Defines the supported mounting positions of fixtures.
	/// </summary>
	public enum MountingPositionType
	{
		Bottom,
		Top,
	}

	/// <summary>
	/// Maintains properties of a DMX moving head intelligent fixture.
	/// </summary>
	public interface IMovingHead
	{
		/// <summary>
		/// Mounting position of the moving head.
		/// </summary>
		MountingPositionType MountingPosition { get; set; }

		/// <summary>
		/// Horizontal angle (in degrees) of the base the moving head.
		/// </summary>
		double PanAngle { get; set; }

		/// <summary>
		/// Vertical tilt angle (in degrees) of the beam housing.
		/// </summary>
		double TiltAngle { get; set; }

		/// <summary>
		/// Length or strength of the beam (1-100).
		/// TODO: Should we change this to an INT?
		/// </summary>
		double BeamLength { get; set; }

		/// <summary>
		/// Determines whether the beam is visible or not.
		/// </summary>
		/// <remarks>This may be the result of either electronic control or mechanical shutter</remarks>
		bool OnOff { get; set; }

		/// <summary>
		/// Color of the left side of the light beam.
		/// </summary>
		/// <remarks>Some color wheel fixtures support displaying a half step of two colors</remarks>
		Color BeamColorLeft { get; set; }

		/// <summary>
		/// Color of the right side of the light beam.
		/// </summary>
		/// <remarks>Some color wheel fixtures support displaying a half step of two colors</remarks>
		Color BeamColorRight { get; set; }

		/// <summary>
		/// Intensity of the light beam (1-100).
		/// </summary>
		int Intensity { get; set; }

		/// <summary>
		/// Focus of the light beam (1-100).
		/// </summary>
		int Focus { get; set; }

		/// <summary>
		/// Legend text to display below the fixture.
		/// The legend is intended to reflect fixture settings that are not easily modeled in a preview.
		/// </summary>
		string Legend { get; set; }

		/// <summary>
		/// Configures whether the legend is displayed in the preview.
		/// </summary>
		bool IncludeLegend { get; set; }

		/// <summary>
		/// Color of the legend text.
		/// </summary>
		Color LegendColor { get; set; }

		/// <summary>
		/// Enables GDI Preview support.
		/// </summary>
		bool EnableGDI { get; set; }
		
		/// <summary>
		/// Clones the moving head settings.
		/// </summary>
		/// <returns>Clone of the moving head settings.</returns>
		IMovingHead Clone();

		/// <summary>
		/// Intensity of the fixture (0-255).
		/// </summary>
		/// <remarks>This property allows the fixture to match the intensity of the background as it is dimmed.</remarks>
		int FixtureIntensity { get; set; }

		/// <summary>
		/// Returns either +1 or -1 based on the orientation of the fixture.
		/// </summary>
		/// <returns>+1 or -1 based on the orientation of the fixture</returns>
		double GetOrientationSign();
	}
}
