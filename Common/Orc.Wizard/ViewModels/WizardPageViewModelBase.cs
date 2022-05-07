namespace Orc.Wizard
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;

    public class WizardPageViewModelBase<TWizardPage> : ViewModelBase, IWizardPageViewModel
        where TWizardPage : class, IWizardPage
    {
        #region Constructors
        public WizardPageViewModelBase(TWizardPage wizardPage)
        {
            Argument.IsNotNull(() => wizardPage);

            DeferValidationUntilFirstSaveCall = true;
            WizardPage = wizardPage;
            QuickNavigateToPage = new TaskCommand<IWizardPage>(QuickNavigateToPageExecuteAsync, QuickNavigateToPageCanExecute);
        }
        #endregion

        #region Properties

        [Model(SupportIEditableObject = false)]
        public TWizardPage WizardPage { get; private set; }

        public IWizard Wizard
        {
            get
            {
                var wizardPage = WizardPage;
                if (wizardPage is null)
                {
                    return null;
                }

                return wizardPage.Wizard;
            }
        }

        public virtual void EnableValidationExposure()
        {
            DeferValidationUntilFirstSaveCall = false;

            Validate(true);
        }
        #endregion

        #region Commands

        public TaskCommand<IWizardPage> QuickNavigateToPage { get; private set; }

        public bool QuickNavigateToPageCanExecute(IWizardPage parameter)
        {
            if (!Wizard.AllowQuickNavigation)
            {
                return false;
            }

            if (!parameter.IsVisited)
            {
                return false;
            }

            if (Wizard.CurrentPage == parameter)
            {
                return false;
            }

            return true;
        }

        public async Task QuickNavigateToPageExecuteAsync(IWizardPage parameter)
        {
            var page = parameter;
            if (page is not null && page.IsVisited && Wizard.Pages is System.Collections.Generic.List<IWizardPage>)
            {
                var list = Wizard.Pages.ToList();
                var index = list.IndexOf(page);

                await Wizard.MoveToPageAsync(index);
            }
        }
        #endregion
    }
}
