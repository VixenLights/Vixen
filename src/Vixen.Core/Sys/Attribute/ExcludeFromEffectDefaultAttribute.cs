namespace Vixen.Sys.Attribute
{
	/// <summary>
	/// Decorate a property or field on an effect's <c>ModuleData</c> class to exclude it from the
	/// effect defaults feature (see <c>Vixen.Services.EffectDefaults.EffectDefaultsService</c>).
	/// When a user saves an effect's current settings as the default for that effect type, every
	/// member carrying this attribute is reset to its type's <see langword="default"/> value before
	/// the settings are stored, instead of being captured with the value it had on the live effect.
	/// This is intended for values that are meaningful only within the sequence the effect currently
	/// lives in, such as a Mark Collection identifier, which would refer to nothing (or coincidentally
	/// to an unrelated collection) if carried into a different sequence.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ExcludeFromEffectDefaultAttribute : System.Attribute
	{
	}
}
