using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the fixture function names associated with the target node(s).
	/// </summary>
	public class FunctionCollectionNameConverter : EffectListTypeConverterBase<FixtureModule>
	{
		#region Protected Methods

		/// <summary>
		/// Gets a collection of fixture function names associated with the node(s).
		/// </summary>
		/// <param name="context">Effect associated with the request</param>
		/// <returns>Collection of fixture function names</returns>
		protected override List<string> GetStandardValuesInternal(FixtureModule fixtureEffect)
		{
			// Retrieve the functions from the fixture effect
			return fixtureEffect.GetFunctionNames();
		}
		
		#endregion
	}
}
