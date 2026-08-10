using System.Runtime.Serialization;

namespace Vixen.Services.EffectDefaults
{
	/// <summary>
	/// A single saved effect default: the scrubbed, serialized <c>ModuleData</c> for one effect type,
	/// captured by <see cref="EffectDefaultsService.SaveDefault"/>.
	/// </summary>
	[DataContract]
	public class EffectDefaultEntry
	{
		/// <summary>
		/// Gets or sets the effect type's descriptor <c>TypeId</c> (see <c>Vixen.Module.IModuleDescriptor.TypeId</c>).
		/// This is the key used to look up the entry when a new effect instance of that type is created.
		/// </summary>
		[DataMember]
		public Guid TypeId { get; set; }

		/// <summary>
		/// Gets or sets the effect type's display name at the time the default was saved. This is never used to
		/// resolve behavior; it exists purely so a human reading the diagnostic dump can identify entries.
		/// </summary>
		[DataMember]
		public string EffectName { get; set; }

		/// <summary>
		/// Gets or sets the full name of the effect's <c>ModuleData</c> class at the time the default was saved.
		/// This is never used to resolve the payload's <see cref="Type"/> at load time; it exists purely so a
		/// mismatch between this value and the effect type's currently-installed data model can be detected and
		/// logged as a warning rather than silently producing corrupt data.
		/// </summary>
		[DataMember]
		public string DataModelTypeName { get; set; }

		/// <summary>
		/// Gets or sets the UTC date and time the default was saved.
		/// </summary>
		[DataMember]
		public DateTime SavedUtc { get; set; }

		/// <summary>
		/// Gets or sets the scrubbed <c>ModuleData</c> object, serialized with a binary
		/// <see cref="System.Runtime.Serialization.DataContractSerializer"/>.
		/// </summary>
		[DataMember]
		public byte[] Payload { get; set; }
	}

	/// <summary>
	/// The on-disk root object for the effect defaults store: a versioned collection of
	/// <see cref="EffectDefaultEntry"/> records, one per effect type that has a saved default. An instance of
	/// this class is what gets written to and read from the profile-scoped store file managed by
	/// <see cref="EffectDefaultsService"/>.
	/// </summary>
	[DataContract]
	public class EffectDefaultsStore
	{
		/// <summary>
		/// Gets or sets the schema version of this store file, so a future format change can detect and migrate
		/// older files. The current version is 1.
		/// </summary>
		[DataMember]
		public int Version { get; set; } = 1;

		/// <summary>
		/// Gets or sets the saved default entries, at most one per effect type <c>TypeId</c>.
		/// </summary>
		[DataMember]
		public List<EffectDefaultEntry> Entries { get; set; } = new List<EffectDefaultEntry>();
	}
}
