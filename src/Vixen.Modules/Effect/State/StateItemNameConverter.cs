using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Provides the State item names available to the State effect.
	/// </summary>
	public sealed class StateItemNameConverter : EffectListTypeConverterBase<State>
	{
		/// <inheritdoc />
		protected override List<string> GetStandardValuesInternal(State effectModule)
		{
			return effectModule.GetStateItemOptions();
		}
	}
}
