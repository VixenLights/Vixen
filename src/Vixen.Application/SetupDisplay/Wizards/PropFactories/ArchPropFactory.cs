using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models.Arch;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates arch prop nodes from wizard data.
	/// </summary>
	internal class ArchPropFactory : IPropFactory
	{
		#region IPropFactory 
		
		/// <inheritdoc/>		
		public IEnumerable<PropNode> GetProps(IPropWizard wizard)
		{
			// Retrieve the Tree Prop wizard page
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);

			// Create the Arch prop
			Arch arch = VixenSystem.Props.CreateProp<Arch>(archPropPage.Name);

			// Configure the Arch properties
			arch.NodeCount = archPropPage.NodeCount;
			arch.StringType = archPropPage.StringType;

			// Create the collection of prop nodes to return 
			List<PropNode> propNodes = new();

			// Create the arch prop node
			propNodes.Add(new(arch));

			// Return the collection of prop nodes
			return propNodes;
		}

		#endregion
	}
}
