using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.ViewModels;
using VixenApplication.SetupDisplay.Wizards.Pages;
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
            // Create the Arch prop
            Arch arch = VixenSystem.Props.CreateProp<Arch>(VixenSystem.Props.GenerateUniquePropTitle(PropType.Arch));

            // Transfer the data from the wizard into the arch prop
            UpdateProp(arch, wizard);

            // Create the collection of props to return 
            IPropGroup propGroup = new PropGroup();

            // Create the arch prop 
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
            archPropPage.ArchWiringStart = arch.ArchWiringStart;
            archPropPage.LightSize = arch.LightSize;
            archPropPage.Rotations = AxisRotationViewModel.ConvertToViewModel(arch.Rotations);

            // Configure the wizard with the Additional Arch properties
            ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
            additionalPage.LeftRight = arch.LeftRight;

            // Configure the wizard with the Dimming Arch properties
            DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
            dimmingPage.Brightness = arch.Brightness;
            dimmingPage.Gamma = arch.Gamma;

            // Configure the Color Arch properties
            ColorWizardPage colorPage = (ColorWizardPage)wizard.Pages.Single(page => page is ColorWizardPage);
            colorPage.StringType = arch.StringType;
            colorPage.SingleColorOption = arch.SingleColorOption;
            colorPage.SelectedColorSet = arch.SelectedColorSet;
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
            arch.ArchWiringStart = archPropPage.ArchWiringStart;
            arch.LightSize = archPropPage.LightSize;
            arch.Rotations = AxisRotationViewModel.ConvertToModel(archPropPage.Rotations);

            // Configure the Additional Arch properties
            ArchPropAdditionalWizardPage additionalPage = (ArchPropAdditionalWizardPage)wizard.Pages.Single(page => page is ArchPropAdditionalWizardPage);
            arch.LeftRight = additionalPage.LeftRight;

            // Configure the Dimming Arch properties
            DimmingWizardPage dimmingPage = (DimmingWizardPage)wizard.Pages.Single(page => page is DimmingWizardPage);
            arch.Brightness = dimmingPage.Brightness;
            arch.Gamma = dimmingPage.Gamma;

            // Configure the Color Arch properties
            ColorWizardPage colorPage = (ColorWizardPage)wizard.Pages.Single(page => page is ColorWizardPage);
            arch.StringType = colorPage.StringType;
            arch.SingleColorOption = colorPage.SingleColorOption;
            arch.SelectedColorSet = colorPage.SelectedColorSet;
        }

        #endregion
    }
}