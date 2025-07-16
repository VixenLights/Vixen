using Catel.IoC;
using Orc.Wizard;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Wizard;

namespace VixenApplication.SetupDisplay.Wizards.Factory
{
    public static class PropWizardFactory
    {
        public static IWizard? CreateInstance(PropType propType, ITypeFactory typeFactory)
        {
			switch (propType)
            {
                case PropType.Arch:
                    return typeFactory.CreateInstance(typeof(ArchPropWizard)) as IWizard;
                case PropType.Tree:
	                return typeFactory.CreateInstance(typeof(TreePropWizard)) as IWizard;
				default:
                    return null;
			}   

        }
    }
}