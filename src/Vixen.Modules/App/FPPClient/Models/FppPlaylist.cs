namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Represents a playlist retrieved from the <c>GET playlist/:PlaylistName</c> endpoint.
/// </summary>
public sealed record FppPlaylist
{
	/// <summary>Gets the name of the playlist.</summary>
	public string Name { get; init; } = string.Empty;

	/// <summary>Gets the entries that play before the main playlist.</summary>
	public FppPlaylistEntry[] LeadIn { get; init; } = [];

	/// <summary>Gets the main playlist entries.</summary>
	public FppPlaylistEntry[] MainPlaylist { get; init; } = [];

	/// <summary>Gets the entries that play after the main playlist.</summary>
	public FppPlaylistEntry[] LeadOut { get; init; } = [];

	/// <summary>Gets aggregate timing and item-count information for the playlist, or <see langword="null"/> if not reported.</summary>
	public FppPlaylistInfo? PlaylistInfo { get; init; }
}
