using Vixen.Sys.Props;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates a collection of prop nodes from wizard data.
	/// </summary>
	public interface IPropFactory
	{
		/// <summary>
		/// Creates a collection of prop nodes from wizard data.
		/// </summary>
		/// <param name="wizard">Prop wizard to create prop nodes from</param>
		/// <returns>A collection of props in an optional group</returns>
		IPropGroup GetProps(IPropWizard wizard);

		/// <summary>
		/// Loads the specified wizard with data from the specified prop.
		/// </summary>
		/// <param name="prop">Prop to load into wizard</param>
		/// <param name="wizard">Wizard to load with data</param>
		void LoadWizard(IProp prop, IPropWizard wizard);

		/// <summary>
		/// Transfers data from the specified wizard into the specified prop.
		/// </summary>
		/// <param name="prop">Prop to update</param>
		/// <param name="wizard">Wizard to retrieve data from</param>
		void UpdateProp(IProp prop, IPropWizard wizard);
	}
}