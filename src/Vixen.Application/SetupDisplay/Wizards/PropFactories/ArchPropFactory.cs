using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenModules.App.Props.Models;
using VixenModules.App.Props.Models.Arch;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates arch props from wizard data.
	/// </summary>
	internal class ArchPropFactory : IPropFactory
	{
		#region IPropFactory 
		
		/// <inheritdoc/>		
		public IPropGroup GetProps(IPropWizard wizard)
		{
			// Retrieve the Prop wizard pages
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);

			// Create the Arch prop
			Arch arch = VixenSystem.Props.CreateProp<Arch>(archPropPage.Name);
			// Configure the Arch properties
			arch.Name = archPropPage.Name;
			arch.NodeCount = archPropPage.NodeCount;
			arch.StringType = archPropPage.StringType;
			arch.ArchWiringStart = archPropPage.ArchWiringStart;
			arch.LightSize = archPropPage.LightSize;
			arch.Rotations = AxisRotationViewModel.ConvertToModel(archPropPage.Rotations);
			arch.PropModel = archPropPage.LightPropModel;

			ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
			arch.LeftRight = additionalPage.LeftRight;

			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			arch.Curve = dimmingPage.Curve;
			arch.Brightness = dimmingPage.Brightness;
			arch.Gamma = dimmingPage.Gamma;
			arch.DimmingTypeOption = dimmingPage.DimmingTypeOption;

			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Create the arch prop 
			propGroup.Props.Add(arch);

			// Return the collection of props
			return propGroup;
		}

		public (IProp, IPropGroup) CreateBaseProp()
		{
			// Create the Arch prop
			Arch arch = VixenSystem.Props.CreateProp<Arch>(VixenSystem.Props.GenerateUniquePropTitle(PropType.Arch));

			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Add the Arch to the prop collections 
			propGroup.Props.Add(arch);

			// Return the collection of props
			return (arch, propGroup);
		}

		public IPropGroup EditExistingProp(IProp arch)
		{
			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Add the Arch to the prop collections 
			propGroup.Props.Add(arch);

			// Return the collection of props
			return propGroup;
		}

		public void LoadWizard(IProp prop, IPropWizard wizard)
		{
			Arch arch = (Arch)prop;

			// Retrieve the Prop wizard pages
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);
			archPropPage.Name = arch.Name;
			archPropPage.NodeCount = arch.NodeCount;
			archPropPage.StringType = arch.StringType;
			archPropPage.ArchWiringStart = arch.ArchWiringStart;
			archPropPage.LightSize = arch.LightSize;
			archPropPage.Rotations = AxisRotationViewModel.ConvertToViewModel(arch.Rotations);

			ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
			additionalPage.LeftRight = arch.LeftRight;

			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			dimmingPage.Curve = arch.Curve;
			dimmingPage.Brightness = arch.Brightness;
			dimmingPage.Gamma = arch.Gamma;
			dimmingPage.DimmingTypeOption = arch.DimmingTypeOption;
		}

		public void UpdateProp(IProp prop, IPropWizard wizard)
		{
			Arch arch = (Arch)prop;

			// Retrieve the Prop wizard pages
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);
			// Configure the Arch properties
			arch.Name = archPropPage.Name;
			arch.NodeCount = archPropPage.NodeCount;
			arch.StringType = archPropPage.StringType;
			arch.ArchWiringStart = archPropPage.ArchWiringStart;
			arch.LightSize = archPropPage.LightSize;
			arch.Rotations = AxisRotationViewModel.ConvertToModel(archPropPage.Rotations);
			arch.PropModel = archPropPage.LightPropModel;

			ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
			arch.LeftRight = additionalPage.LeftRight;

			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			arch.Curve = dimmingPage.Curve;
			arch.Brightness = dimmingPage.Brightness;
			arch.Gamma = dimmingPage.Gamma;
			arch.DimmingTypeOption = dimmingPage.DimmingTypeOption;
		}

		#endregion
	}
}
