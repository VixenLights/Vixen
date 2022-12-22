using Catel.MVVM;
using Orc.Wizard;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	/// <summary>
	/// Base class for fixture wizard view models that host portions of the Fixture property editor.
	/// </summary>
	/// <typeparam name="TWizardPage">Wizard model class associated with the page</typeparam>
	public abstract class EditWizardPageViewModelBase<TWizardPage> : WizardPageViewModelBase<TWizardPage>
         where TWizardPage : class, IWizardPage
    {
        #region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="wizardPage">Wizard page model</param>
        public EditWizardPageViewModelBase(TWizardPage wizardPage) :
            base(wizardPage)
		{
            // Ensure Catel validates immediately
            DeferValidationUntilFirstSaveCall = false;
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Refreshes the wizard navigation buttons and re-validates all the view models.
        /// </summary>
        protected void RefreshCanSave()
        {            
            // Have Catel validate the view model
            Validate(true);

            // Retrieve the child view models
            IEnumerable<IViewModel> childViewModels = GetChildViewModels();

            //bool canSave = true;

            // Loop over the child view models
            foreach (IViewModel childViewModel in GetChildViewModels())
            {
                // Have the child view model validate
                /*canSave &=*/ ((IFixtureSaveable)childViewModel).CanSave();
            }            

            //if (!canSave)
			//{                
                // Make sure the validation bar is displayed
                //HideValidationResults = false;
            //}

            // Have the wizard refresh the navigation commands
            ((IntelligentFixtureWizard)Wizard).RefreshNavigationCommands();
        }

        #endregion
    }
}
