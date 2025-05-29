using Catel.IoC;
using Orc.Wizard;
using VixenApplication.SetupDisplay.Wizards.Pages;

namespace VixenApplication.SetupDisplay.Wizards.Wizard
{
	public class ArchPropWizard: SideNavigationWizardBase
	{
        public ArchPropWizard(ITypeFactory typeFactory) : base(typeFactory)
        {
            Title = "Arch Prop";
            this.AddPage<ArchPropWizardPage>();
        }
    }
}
