namespace Orc.Wizard
{
    using Catel.IoC;

    public abstract class FullScreenWizardBase : WizardBase
    {
        protected FullScreenWizardBase(ITypeFactory typeFactory) 
            : base(typeFactory)
        {
        }

        public bool HideNavigationSystem { get; protected set; }
    }
}
