using Catel.IoC;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.PropFactories;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.Factory
{
	public static class PropWizardFactory
    {
        public static (IPropWizard?, IPropFactory)  CreateInstance(PropType propType, ITypeFactory typeFactory)
        {
			switch (propType)
            {
                case PropType.Arch:
                    return (typeFactory.CreateInstance(typeof(ArchPropWizard)) as IPropWizard, new ArchPropFactory());
                case PropType.Tree:
	                return (typeFactory.CreateInstance(typeof(TreePropWizard)) as IPropWizard, new TreePropFactory());
				//case PropType.IntelligentFixture:
				//	return (typeFactory.CreateInstance(typeof(IntelligentFixtureWizard)) as IPropWizard, new IntelligentFixturePropFactory());
				default:
					throw new Exception("Unsupported Prop Type");
			}   

        }
    }
}