namespace Orc.Wizard
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Services;
    using ViewModels;

    public class WizardService : IWizardService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IUIVisualizerService _uiVisualizerService;

        public WizardService(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;
        }

        public Task<bool?> ShowWizardAsync(IWizard wizard)
        {
            Argument.IsNotNull(() => wizard);

            Log.Debug("Showing wizard '{0}'", wizard.GetType().GetSafeFullName(false));

            if (wizard is SideNavigationWizardBase)
            {
                return _uiVisualizerService.ShowDialogAsync<SideNavigationWizardViewModel>(wizard);
            }

            if (wizard is FullScreenWizardBase)
            {
                return _uiVisualizerService.ShowDialogAsync<FullScreenWizardViewModel>(wizard);
            }

            return _uiVisualizerService.ShowDialogAsync<WizardViewModel>(wizard);
        }
    }
}
