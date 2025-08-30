using Catel.IoC;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Wizard;
//using VixenModules.Editor.FixtureWizard.Wizard;

namespace VixenApplication.SetupDisplay.Wizards.Factory
{
	public static class PropWizardFactory
    {
        public static IPropWizard? CreateInstance(PropType propType, ITypeFactory typeFactory)
        {
			switch (propType)
            {
                case PropType.Arch:
                    return typeFactory.CreateInstance(typeof(ArchPropWizard)) as IPropWizard;
                case PropType.Tree:
	                return typeFactory.CreateInstance(typeof(TreePropWizard)) as IPropWizard;
				//case PropType.IntelligentFixture:
				//	return typeFactory.CreateInstance(typeof(IntelligentFixtureWizard)) as IPropWizard;	
				default:
                    return null;
			}   

        }
    }
}