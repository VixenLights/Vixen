namespace Orc.Wizard.ViewModels
{
    using Catel.Services;
    using Orc.Wizard;

    public class FullScreenWizardViewModel : WizardViewModel
    {
        public FullScreenWizardViewModel(IWizard wizard, IMessageService messageService, ILanguageService languageService) 
            : base(wizard, messageService, languageService)
        {
        }
    }
}
