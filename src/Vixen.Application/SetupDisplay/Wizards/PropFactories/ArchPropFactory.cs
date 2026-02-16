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
		/// <summary>
		/// Create a default Arch prop
		/// </summary>
		/// <returns>Returns both <see cref="IProp"/> which specifies the new Prop and <see cref="IPropGroup"/> which specifies the group that contains the Prop</returns>
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

		/// <summary>
		/// Sets up a Prop Group with the existing Prop
		/// </summary>
		/// <param name="arch">Specifies the Arch prop to edit</param>
		/// <returns>Returns <see cref="IPropGroup"/> which specifies the group that contains the Prop</returns>
		public IPropGroup EditExistingProp(IProp arch)
		{
			// Create the collection of props to return 
			IPropGroup propGroup = new PropGroup();

			// Add the Arch to the prop collections 
			propGroup.Props.Add(arch);

			// Return the collection of props
			return propGroup;
		}

		/// <summary>
		/// Transfers data from the Arch Prop into the Wizard
		/// </summary>
		/// <param name="prop">Specifies the Arch prop source</param>
		/// <param name="wizard">Specifies the Arch prop wizard destination</param>
		public void LoadWizard(IProp prop, IPropWizard wizard)
		{
			Arch arch = (Arch)prop;

			// Configure the wizard with the base Arch properties
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);
			archPropPage.Name = arch.Name;
			archPropPage.NodeCount = arch.NodeCount;
			archPropPage.StringType = arch.StringType;
			archPropPage.ArchWiringStart = arch.ArchWiringStart;
			archPropPage.LightSize = arch.LightSize;
			archPropPage.Rotations = AxisRotationViewModel.ConvertToViewModel(arch.Rotations);

			// Configure the wizard with the Additional Arch properties
			ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
			additionalPage.LeftRight = arch.LeftRight;

			// Configure the wizard with the Dimming Arch properties
			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			dimmingPage.Curve = arch.Curve;
			dimmingPage.Brightness = arch.Brightness;
			dimmingPage.Gamma = arch.Gamma;
			dimmingPage.DimmingTypeOption = arch.DimmingTypeOption;
		}

		/// <summary>
		/// Transfers data from the Wizard into the Arch Prop
		/// </summary>
		/// <param name="prop">Specifies the Arch prop destination</param>
		/// <param name="wizard">Specifies the Arch prop wizard source</param>
		public void UpdateProp(IProp prop, IPropWizard wizard)
		{
			Arch arch = (Arch)prop;

			// Configure the base Arch properties
			ArchPropWizardPage archPropPage = (ArchPropWizardPage)wizard.Pages.Single(page => page is ArchPropWizardPage);
			arch.Name = archPropPage.Name;
			arch.NodeCount = archPropPage.NodeCount;
			arch.StringType = archPropPage.StringType;
			arch.ArchWiringStart = archPropPage.ArchWiringStart;
			arch.LightSize = archPropPage.LightSize;
			arch.Rotations = AxisRotationViewModel.ConvertToModel(archPropPage.Rotations);
			arch.PropModel = archPropPage.LightPropModel;

			// Configure the Additional Arch properties
			ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
			arch.LeftRight = additionalPage.LeftRight;

			// Configure the Dimming Arch properties
			DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
			arch.Curve = dimmingPage.Curve;
			arch.Brightness = dimmingPage.Brightness;
			arch.Gamma = dimmingPage.Gamma;
			arch.DimmingTypeOption = dimmingPage.DimmingTypeOption;
		}
	}
}
