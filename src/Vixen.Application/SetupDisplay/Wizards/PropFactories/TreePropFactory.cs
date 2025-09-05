using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using VixenModules.App.Props.Models.Tree;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates tree prop nodes from wizard data.
	/// </summary>
	internal class TreePropFactory : IPropFactory
	{
		#region IPropFactory

		/// <inheritdoc/>		
		public IPropGroup GetProps(IPropWizard wizard)
		{
			// Retrieve the Tree Prop wizard page
			TreePropWizardPage treePropPage = (TreePropWizardPage)wizard.Pages.Single(page => page is TreePropWizardPage);

			// Create the Tree prop
			Tree tree = VixenSystem.Props.CreateProp<Tree>(treePropPage.Name);

			// Configure the Tree properties
			tree.Strings = treePropPage.Strings;
			tree.NodesPerString = treePropPage.NodesPerString;
			tree.StringType = treePropPage.StringType;

			//TODO add in other fields when wizard has full function

			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Create the tree prop node
			propGroup.Props.Add(tree);

			// Return the collection of props
			return propGroup;						
		}

		#endregion
	}
}
