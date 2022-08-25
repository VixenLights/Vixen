namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	using Catel.Data;
	using Catel.MVVM;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using VixenModules.App.Fixture;
	using VixenModules.Editor.FixturePropertyEditor.ViewModels;
	using VixenModules.Editor.FixtureWizard.Wizard.Models;

	/// <summary>
	/// Wizard view model page for editing the profile's channels.
	/// </summary>
	public class EditProfileWizardPageViewModel : EditWizardPageViewModelBase<EditProfileWizardPage>, IIntelligentFixtureWizardPageViewModel
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Edit profile wizard page model</param>
        public EditProfileWizardPageViewModel(EditProfileWizardPage wizardPage) :
            base(wizardPage)
        {
            // Retrieve the select profile wizard page model
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);

            // Initalize the fixture to edit with the fixture profile and a delegate to refresh the navigation buttons
            FixtureSpecification = new Tuple<FixtureSpecification, Action, bool>(selectProfilePage.Fixture, RefreshCanSave, false);

            // Configure Catel to validate immediately
            DeferValidationUntilFirstSaveCall = false;
        }

		#endregion
		
        #region Public Properties

        /// <summary>
        /// Fixture specification associated with the property.
        /// </summary>
        public Tuple<FixtureSpecification, Action, bool> FixtureSpecification
        {
            get
            {
                return GetValue<Tuple<FixtureSpecification, Action, bool>>(FixtureSpecificationProperty);
            }
            set
            {
                SetValue(FixtureSpecificationProperty, value);
            }
        }

        /// <summary>
        /// Fixture specification property data.
        /// </summary>
        public static readonly PropertyData FixtureSpecificationProperty = RegisterProperty(nameof(FixtureSpecification), typeof(Tuple<FixtureSpecification, Action, bool>), null);

		#endregion

		#region Protected Methods
       
        /// <summary>
        /// Returns a Task for saving the fixture profile data.
        /// </summary>
        /// <returns>Task for saving the fixture profile data</returns>
        protected override Task<bool> SaveAsync()
        {
            // Get the child view models
            IEnumerable<IViewModel> childViewModels = this.GetChildViewModels();

            // Retreive the fixture property editor view model
            FixturePropertyEditorViewModel fixtureEditor = (FixturePropertyEditorViewModel)childViewModels.Single(vm => vm is FixturePropertyEditorViewModel);

            // Retrieve the fixture profile from the view model
            FixtureSpecification fixtureSpecification = fixtureEditor.GetFixtureSpecification();

            // Retrieve the Select Profile wizard page model
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);

            // Update the fixture associated with the model page
            selectProfilePage.Fixture = fixtureSpecification;

            // Update the selected fixture string
            selectProfilePage.SelectedFixture = fixtureSpecification.Name;

            // Call the base class implementation
            return base.SaveAsync();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns true if the page contains valid and complete data.
        /// </summary>
        /// <returns>True if the page contains valid and complete data</returns>
        private bool IsPageValid()
        {
	        bool isPageValid = true;

	        // Get the child view models
	        IEnumerable<IViewModel> childViewModels = this.GetChildViewModels();

	        // If the child view models have been created then...
	        if (childViewModels.Count() > 0)
	        {
		        // Retreive the fixture property editor view model
		        FixturePropertyEditorViewModel fixtureEditor =
			        (FixturePropertyEditorViewModel)childViewModels.SingleOrDefault(vm => vm is FixturePropertyEditorViewModel);

		        // If the fixture editor view model was found then...
		        if (fixtureEditor != null)
		        {
			        // Get the validation results from the fixture editor view model
			        string results = fixtureEditor.GetValidationResults();

			        // Only allow the user to go to the previous page if the current page is valid
			        isPageValid = string.IsNullOrEmpty(results);
		        }
	        }

	        return isPageValid;
        }

        #endregion

        #region IIntelligentFixtureWizardPageViewModel

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool CanMoveBack()
        {
	        return IsPageValid();
        }

        /// <summary>
        /// Refer to interface documentation.
        /// </summary>
        public bool CanMoveNext()
        {
	        return IsPageValid();
        }

        #endregion
	}
}
