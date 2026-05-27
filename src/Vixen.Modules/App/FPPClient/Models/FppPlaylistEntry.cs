namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Represents a single entry within a playlist section.
/// </summary>
/// <remarks>
/// The <see cref="Type"/> field determines which optional fields are populated.
/// Entries of type <c>"sequence"</c> or <c>"both"</c> populate <see cref="SequenceName"/>;
/// entries of type <c>"both"</c> or <c>"media"</c> also populate <see cref="MediaName"/>.
/// </remarks>
public sealed record FppPlaylistEntry
{
	/// <summary>
	/// Gets the entry type (e.g. <c>"sequence"</c>, <c>"both"</c>, <c>"media"</c>, <c>"pause"</c>).
	/// </summary>
	public string Type { get; init; } = string.Empty;

	/// <summary>Gets a value indicating whether this entry is enabled (1) or disabled (0).</summary>
	public int Enabled { get; init; }

	/// <summary>Gets a value indicating whether this entry plays only once (1) or repeats (0).</summary>
	public int PlayOnce { get; init; }

	/// <summary>
	/// Gets the FSEQ sequence filename, or <see langword="null"/> for entry types that do not reference a sequence.
	/// </summary>
	public string? SequenceName { get; init; }

	/// <summary>
	/// Gets the media filename, or <see langword="null"/> for entry types that do not reference media.
	/// </summary>
	public string? MediaName { get; init; }

	/// <summary>Gets the optional note attached to this entry, or <see langword="null"/> if none.</summary>
	public string? Note { get; init; }
}
