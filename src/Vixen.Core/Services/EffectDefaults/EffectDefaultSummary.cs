namespace Vixen.Services.EffectDefaults
{
	/// <summary>
	/// A read-only projection of a saved effect default, used to list saved defaults (for example, in the export
	/// and import UI added in a later milestone, or for diagnostics) without exposing the raw serialized payload.
	/// </summary>
	public sealed class EffectDefaultSummary
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EffectDefaultSummary"/> class.
		/// </summary>
		/// <param name="typeId">The effect type's descriptor <c>TypeId</c>.</param>
		/// <param name="effectName">The effect type's display name at the time the default was saved.</param>
		/// <param name="savedUtc">The UTC date and time the default was saved.</param>
		/// <param name="moduleInstalled">Whether the effect type is currently installed and resolvable.</param>
		public EffectDefaultSummary(Guid typeId, string effectName, DateTime savedUtc, bool moduleInstalled)
		{
			TypeId = typeId;
			EffectName = effectName;
			SavedUtc = savedUtc;
			ModuleInstalled = moduleInstalled;
		}

		/// <summary>
		/// Gets the effect type's descriptor <c>TypeId</c>.
		/// </summary>
		public Guid TypeId { get; }

		/// <summary>
		/// Gets the effect type's display name at the time the default was saved.
		/// </summary>
		public string EffectName { get; }

		/// <summary>
		/// Gets the UTC date and time the default was saved.
		/// </summary>
		public DateTime SavedUtc { get; }

		/// <summary>
		/// Gets a value indicating whether the effect type is currently installed and resolvable via
		/// <c>Vixen.Sys.Modules.GetDescriptorById</c>.
		/// </summary>
		public bool ModuleInstalled { get; }

		/// <summary>
		/// Returns a short, human-readable description of this summary (the effect name and saved date), for
		/// display in lists such as an export selection dialog.
		/// </summary>
		/// <returns>A string of the form "EffectName (saved yyyy-MM-dd HH:mm)".</returns>
		public override string ToString()
		{
			return $"{EffectName} (saved {SavedUtc:yyyy-MM-dd HH:mm})";
		}
	}
}
