namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Represents the current resource utilization of an FPP device.
/// </summary>
public sealed record FppUtilization
{
	/// <summary>Gets the CPU usage as a percentage (0–100).</summary>
	public double CPU { get; init; }

	/// <summary>Gets the memory usage as a percentage (0–100).</summary>
	public double Memory { get; init; }

	/// <summary>Gets the formatted uptime string (e.g. <c>"2 days, 4:30"</c>).</summary>
	public string Uptime { get; init; } = string.Empty;
}
