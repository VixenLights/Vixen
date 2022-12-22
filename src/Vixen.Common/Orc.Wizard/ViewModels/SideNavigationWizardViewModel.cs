namespace Orc.Wizard.ViewModels
{
    using Catel.Services;
    using Orc.Wizard;

    public class SideNavigationWizardViewModel : WizardViewModel
    {
        public SideNavigationWizardViewModel(IWizard wizard, IMessageService messageService, ILanguageService languageService) 
            : base(wizard, messageService, languageService)
        {
        }
    }
}
