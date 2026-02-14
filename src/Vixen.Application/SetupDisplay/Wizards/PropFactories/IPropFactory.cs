using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using VixenModules.Editor.PropWizard;
using VixenModules.App.Props.Models.Arch;

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

		(IProp, IPropGroup) CreateBaseProp();

		void LoadWizard(IProp prop, IPropWizard wizard);

		void UpdateProp(IProp prop, IPropWizard wizard);
	}
}
