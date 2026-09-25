using System.Text.Json.Serialization;

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

	/// <summary>Gets the uptime as supplied by the device, formatting numeric milliseconds as a duration.</summary>
	[JsonConverter(typeof(FppUptimeConverter))]
	public string Uptime { get; init; } = string.Empty;
}
