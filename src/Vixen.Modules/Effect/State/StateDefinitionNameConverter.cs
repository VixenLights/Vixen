using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Provides the State definitions available to the State effect.
	/// </summary>
	public sealed class StateDefinitionNameConverter : EffectListTypeConverterBase<State>
	{
		/// <inheritdoc />
		protected override List<string> GetStandardValuesInternal(State effectModule)
		{
			return effectModule.GetStateDefinitionOptions();
		}
	}
}
