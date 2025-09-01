using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Wizard;
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
		/// <returns>A collection of prop nodes</returns>
		IEnumerable<PropNode> GetProps(IPropWizard wizard);		 
	}
}
