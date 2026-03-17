using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.ViewModels;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models.Tree;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates Tree props from wizard data.
	/// </summary>
	internal class TreePropFactory : IPropFactory
	{
		/// <summary>
		/// Create a default Tree prop
		/// </summary>
		/// <returns>Returns both <see cref="IProp"/> which specifies the new Prop and <see cref="IPropGroup"/> which specifies the group that contains the Prop</returns>
		public (IProp, IPropGroup) CreateBaseProp()
		{
			// Create the Tree prop
			Tree Tree = VixenSystem.Props.CreateProp<Tree>(VixenSystem.Props.GenerateUniquePropTitle(PropType.Tree));

			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Add the Tree to the prop collections 
			propGroup.Props.Add(Tree);

			// Return the collection of props
			return (Tree, propGroup);
		}

		/// <summary>
		/// Transfers data from the Tree Prop into the Wizard
		/// </summary>
		/// <param name="prop">Specifies the Tree prop source</param>
		/// <param name="wizard">Specifies the Tree prop wizard destination</param>
		public void LoadWizard(IProp prop, IPropWizard wizard)
		{
			Tree tree = (Tree)prop;

			// Configure the wizard with the base Tree properties
			TreePropWizardPage treePropPage = (TreePropWizardPage)wizard.Pages.Single(page => page is TreePropWizardPage);
			treePropPage.Name = tree.Name;
			treePropPage.Strings = tree.Strings;
			treePropPage.NodesPerString = tree.NodesPerString;
			treePropPage.LightSize = tree.LightSize;
			treePropPage.DegreeOffset = tree.DegreeOffset;
			treePropPage.DegreesCoverage = tree.DegreesCoverage;
			treePropPage.BaseHeight = tree.BaseHeight;
			treePropPage.TopHeight = tree.TopHeight;
			treePropPage.TopWidth = tree.TopWidth;
			treePropPage.StartLocation = tree.StartLocation;
			treePropPage.ZigZag = tree.ZigZag;
			treePropPage.ZigZagOffset = tree.ZigZagOffset;
			treePropPage.TopRadius = tree.TopRadius;
			treePropPage.BottomRadius = tree.BottomRadius;
			treePropPage.Rotations = AxisRotationViewModel.ConvertToViewModel(tree.Rotations);

			// Configure the wizard with the Additional Tree properties
			//TreePropAdditionalWizardPage additionalPage = (TreePropAdditionalWizardPage)wizard.Pages.Single(page => page is TreePropAdditionalWizardPage);
			//additionalPage.LeftRight = tree.LeftRight;

			// Configure the wizard with the Dimming Tree properties
			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			dimmingPage.Brightness = tree.Brightness;
			dimmingPage.Gamma = tree.Gamma;

			// Configure the Color Tree properties
			ColorWizardPage colorPage = (ColorWizardPage)wizard.Pages.Single(page => page is ColorWizardPage);
			colorPage.StringType = tree.StringType;
			colorPage.SingleColorOption = tree.SingleColorOption;
			colorPage.SelectedColorSet = tree.SelectedColorSet;
		}

		/// <summary>
		/// Transfers data from the Wizard into the Tree Prop
		/// </summary>
		/// <param name="prop">Specifies the Tree prop destination</param>
		/// <param name="wizard">Specifies the Tree prop wizard source</param>
		public void UpdateProp(IProp prop, IPropWizard wizard)
		{
			Tree tree = (Tree)prop;

			// Configure the base Tree properties
			TreePropWizardPage treePropPage = (TreePropWizardPage)wizard.Pages.Single(page => page is TreePropWizardPage);
			tree.Name = treePropPage.Name;
			tree.Strings = treePropPage.Strings;
			tree.NodesPerString = treePropPage.NodesPerString;
			tree.LightSize = treePropPage.LightSize;
			tree.DegreeOffset = treePropPage.DegreeOffset;
			tree.DegreesCoverage = treePropPage.DegreesCoverage;
			tree.BaseHeight = treePropPage.BaseHeight;
			tree.TopHeight = treePropPage.TopHeight;
			tree.TopWidth = treePropPage.TopWidth;
			tree.StartLocation = treePropPage.StartLocation;
			tree.ZigZag = treePropPage.ZigZag;
			tree.ZigZagOffset = treePropPage.ZigZagOffset;
			tree.TopRadius = treePropPage.TopRadius;
			tree.BottomRadius = treePropPage.BottomRadius;
			tree.Rotations = AxisRotationViewModel.ConvertToModel(treePropPage.Rotations);

            // Configure the Additional Tree properties
            //TreePropAdditionalWizardPage additionalPage = (TreePropAdditionalWizardPage)wizard.Pages.Single(page => page is TreePropAdditionalWizardPage);
            //tree.LeftRight = additionalPage.LeftRight;

            // Configure the Dimming Tree properties
            DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			tree.Brightness = dimmingPage.Brightness;
			tree.Gamma = dimmingPage.Gamma;

			// Configure the Color Tree properties
			ColorWizardPage colorPage = (ColorWizardPage)wizard.Pages.Single(page => page is ColorWizardPage);
			tree.StringType = colorPage.StringType;
			tree.SingleColorOption = colorPage.SingleColorOption;
			tree.SelectedColorSet = colorPage.SelectedColorSet;
		}
	}
}
