namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Represents aggregate timing and item-count information for an <see cref="FppPlaylist"/>.
/// </summary>
public sealed record FppPlaylistInfo
{
	/// <summary>Gets the total duration of all playlist entries in seconds.</summary>
	public double TotalDuration { get; init; }

	/// <summary>Gets the total number of items across all playlist sections.</summary>
	public int TotalItems { get; init; }
}
