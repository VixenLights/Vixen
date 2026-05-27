namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Represents the static information returned by the <c>GET system/info</c> endpoint.
/// </summary>
public sealed record FppSystemInfo
{
	/// <summary>Gets the configured hostname of the FPP device.</summary>
	public string HostName { get; init; } = string.Empty;

	/// <summary>Gets the user-configured description of the FPP device.</summary>
	public string HostDescription { get; init; } = string.Empty;

	/// <summary>Gets the hardware platform name (e.g. <c>"Raspberry Pi"</c>).</summary>
	public string Platform { get; init; } = string.Empty;

	/// <summary>Gets the hardware variant (e.g. <c>"Pi 4"</c>).</summary>
	public string Variant { get; init; } = string.Empty;

	/// <summary>Gets the FPP operating mode (e.g. <c>"player"</c>).</summary>
	public string Mode { get; init; } = string.Empty;

	/// <summary>Gets the FPP software version string.</summary>
	public string Version { get; init; } = string.Empty;

	/// <summary>Gets the git branch of the installed FPP build.</summary>
	public string Branch { get; init; } = string.Empty;

	/// <summary>Gets the OS image version string.</summary>
	public string OSVersion { get; init; } = string.Empty;

	/// <summary>Gets the OS release string.</summary>
	public string OSRelease { get; init; } = string.Empty;

	/// <summary>Gets the channel ranges handled by this FPP instance.</summary>
	public string ChannelRanges { get; init; } = string.Empty;

	/// <summary>Gets the major version number of the FPP software.</summary>
	public int MajorVersion { get; init; }

	/// <summary>Gets the minor version number of the FPP software.</summary>
	public int MinorVersion { get; init; }

	/// <summary>Gets the hardware type identifier.</summary>
	public int TypeId { get; init; }

	/// <summary>Gets the unique device identifier.</summary>
	public string Uuid { get; init; } = string.Empty;

	/// <summary>Gets the current CPU and memory utilization, or <see langword="null"/> if not reported.</summary>
	public FppUtilization? Utilization { get; init; }

	/// <summary>Gets the list of IP addresses assigned to the FPP device.</summary>
	public string[] IPs { get; init; } = [];
}
