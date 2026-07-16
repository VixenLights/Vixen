namespace Vixen.Marks
{
	/// <summary>
	/// Describes a Mark Collection name changed while repairing duplicate collection names.
	/// </summary>
	/// <param name="CollectionId">The identifier of the renamed collection.</param>
	/// <param name="OldName">The collection name before repair.</param>
	/// <param name="NewName">The collection name after repair.</param>
	public sealed record MarkCollectionRenameRecord(Guid CollectionId, string OldName, string NewName);
}
