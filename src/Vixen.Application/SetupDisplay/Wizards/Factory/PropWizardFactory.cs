using Catel.IoC;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.PropFactories;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using VixenModules.Editor.FixtureWizard.Wizard;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.Factory
{
	public static class PropFactory
	{
		public static IPropFactory CreateInstance(PropType propType)
		{
			switch (propType)
			{
				case PropType.Arch:
					return (new ArchPropFactory());
				case PropType.Tree:
					return (new TreePropFactory());
				case PropType.IntelligentFixture:
					return (new IntelligentFixturePropFactory());
				default:
					throw new Exception("Unsupported Prop Type");
			}
		}
	}

	/// <summary>
	/// Wizard Factory that creates a prop wizard for a specific prop type specified by the caller.
	/// </summary>
	public static class PropWizardFactory
    {
		#region Public Static Methods

		/// <summary>
		/// Creates both a prop wizard and a prop factory for the specified prop type.
		/// </summary>
		/// <param name="propType">Prop type to create a wizard and prop factory for</param>
		/// <param name="typeFactory">Catel type factory</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static (IPropWizard?, IPropFactory) CreateInstance(PropType propType, ITypeFactory typeFactory)
        {
			switch (propType)
            {
                case PropType.Arch:
                    return (typeFactory.CreateInstance(typeof(ArchPropWizard)) as IPropWizard, new ArchPropFactory());
                case PropType.Tree:
	                return (typeFactory.CreateInstance(typeof(TreePropWizard)) as IPropWizard, new TreePropFactory());
				case PropType.IntelligentFixture:
					return (typeFactory.CreateInstance(typeof(IntelligentFixtureWizard)) as IPropWizard, new IntelligentFixturePropFactory());
				default:
					throw new Exception("Unsupported Prop Type");
			}   
        }

		#endregion 
	}
}