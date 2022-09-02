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
	/// Wizard view model page for editing the profile's functions.
	/// </summary>
	public class EditProfileFunctionsWizardPageViewModel : EditWizardPageViewModelBase<EditProfileFunctionsWizardPage>
    {
		#region Constructor

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="wizardPage">Edit profile functions wizard page model</param>
		public EditProfileFunctionsWizardPageViewModel(EditProfileFunctionsWizardPage wizardPage) :
            base(wizardPage)
        {
            // Retrieve the select profile page model
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);
            
            // Create the Tuple of fixture functions, initial function selection, and a delegate to refresh the wizard navigation buttons
            Functions = new Tuple<List<FixtureFunction>, string, Action>(selectProfilePage.Fixture.FunctionDefinitions, "Pan", RefreshCanSave);            
        }

        #endregion
       
        #region Public Catel Properties

        /// <summary>
        /// Tuple that contains the following:
        /// 1. Fixture Functions
        /// 2. Initial function to select
        /// 3. Delegate to a method that refreshes the wizard navigation buttons
        /// </summary>
        public Tuple<List<FixtureFunction>, string, Action> Functions
        {
            get
            {
                return GetValue<Tuple<List<FixtureFunction>, string, Action>>(FunctionsProperty);
            }
            set
            {
                SetValue(FunctionsProperty, value);
            }
        }

        /// <summary>
        /// Functions property data.
        /// </summary>
        public static readonly PropertyData FunctionsProperty = RegisterProperty(nameof(Functions), typeof(Tuple<List<FixtureFunction>, string, Action>), null);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns a Task for saving the fixture profile function data.
        /// </summary>
        /// <returns>Task for saving the fixture profile data</returns>
        protected override Task<bool> SaveAsync()
        {
            // Get the child view models
            IEnumerable<IViewModel> childViewModels = this.GetChildViewModels();

            // Retreive the function type view model
            FunctionTypeViewModel childVM = (FunctionTypeViewModel)childViewModels.Single(vm => vm is FunctionTypeViewModel);

            // Retrieve the Select Profile wizard page model
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);

            // Update the functions on the fixture
            selectProfilePage.Fixture.FunctionDefinitions = childVM.GetFunctionData();

            // Call the Catel base class implementation
            return base.SaveAsync();
        }
       
        #endregion
    }
}
