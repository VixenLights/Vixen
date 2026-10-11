using System;

namespace VixenModules.App.Modeling;

/// <summary>
/// Stores the validated, per-export settings for a wire diagram.
/// </summary>
public sealed record WireDiagramExportOptions
{
	/// <summary>
	/// Gets or sets the interval between displayed node labels.
	/// </summary>
	/// <value>The positive interval, from 1 through 1000. The default is 1, which shows all labels.</value>
	public int LabelInterval { get; init; } = 1;

	/// <summary>
	/// Gets or sets the radius of a middle-node marker in SVG drawing units.
	/// </summary>
	/// <value>The marker radius from 1 through 30. The default is 6.</value>
	public float NodeRadius { get; init; } = 6f;

	/// <summary>
	/// Gets or sets the enlargement factor applied to the complete exported page.
	/// </summary>
	/// <value>The factor from 1 through 4. The default is 1.</value>
	public float ExportScale { get; init; } = 1f;

	/// <summary>
	/// Validates that the option values are within their supported ranges.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">An option is outside its supported range or is not finite.</exception>
	public void Validate()
	{
		if (LabelInterval is < 1 or > 1000)
		{
			throw new ArgumentOutOfRangeException(nameof(LabelInterval), LabelInterval, "The label interval must be between 1 and 1000.");
		}

		if (!float.IsFinite(NodeRadius) || NodeRadius is < 1f or > 30f)
		{
			throw new ArgumentOutOfRangeException(nameof(NodeRadius), NodeRadius, "The node radius must be finite and between 1 and 30.");
		}

		if (!float.IsFinite(ExportScale) || ExportScale is < 1f or > 4f)
		{
			throw new ArgumentOutOfRangeException(nameof(ExportScale), ExportScale, "The export scale must be finite and between 1 and 4.");
		}
	}
}
