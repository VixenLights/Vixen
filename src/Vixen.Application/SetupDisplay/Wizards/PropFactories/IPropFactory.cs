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
		(IProp, IPropGroup) CreateBaseProp();

		IPropGroup EditExistingProp(IProp prop);

		void LoadWizard(IProp prop, IPropWizard wizard);

		void UpdateProp(IProp prop, IPropWizard wizard);
	}
}
