using Orc.Wizard;

namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
    /// <summary>
	/// Wizard page edits the fixture's associated functions.
	/// </summary>
    public class EditProfileFunctionsWizardPage : FixtureWizardPageBase
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public EditProfileFunctionsWizardPage() :
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/edit-functions/")
        {
            Title = "Edit Functions";
            Description = "Define the fixture functions.";
        }

        #endregion
    }
}
