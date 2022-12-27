namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	using Catel.MVVM;
	using Orc.Wizard;
	using System.Linq;
	using VixenModules.Editor.FixtureWizard.Wizard.Models;

	/// <summary>
	/// Wizard view model page that configures color support for the fixture.
	/// </summary>
	public class ColorSupportWizardPageViewModel : WizardPageViewModelBase<ColorSupportWizardPage>
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Color support wizard page model</param>
		public ColorSupportWizardPageViewModel(ColorSupportWizardPage wizardPage) :
            base(wizardPage)
        {
            // Retrieve the Select Fixture wizard page
            SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)Wizard.Pages.Single(page => page is SelectProfileWizardPage);

            // If the fixture is color mixing then...
            if (selectProfilePage.Fixture.IsColorMixing())
            {
                // Default the fixture to color mixing
                ColorMixing = true;                
            }
            else if (selectProfilePage.Fixture.ContainsColorWheel())
            {
                // Default the fixture to using a color wheel
                ColorWheel = true;
            }  
            else
			{
                // Default to color being handled by other means
                NoColorSupport = true;
			}
        }

		#endregion

		#region Public Catel Properties

        /// <summary>
        /// Indicates the fixture supports color mixing.
        /// </summary>
		[ViewModelToModel]
        public bool ColorMixing { get; set; }

        /// <summary>
        /// Indicates the fixture uses a color wheel to achieve colors.
        /// </summary>
        [ViewModelToModel]
        public bool ColorWheel { get; set; }

        /// <summary>
        /// Indicates color is going to be handled by other means (individual pixel nodes).
        /// </summary>
        [ViewModelToModel]
        public bool NoColorSupport { get; set; }

        #endregion
    }
}
