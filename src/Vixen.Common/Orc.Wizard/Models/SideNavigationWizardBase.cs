namespace Orc.Wizard
{
    using Catel.IoC;

    public abstract class SideNavigationWizardBase : WizardBase
    {
        protected SideNavigationWizardBase(ITypeFactory typeFactory) 
            : base(typeFactory)
        {
        }

        public bool ShowFullScreen { get; set; }
    }
}
