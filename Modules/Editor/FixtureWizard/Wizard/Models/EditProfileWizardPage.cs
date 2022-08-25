using Orc.Wizard;

namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
    /// <summary>
	/// Wizard page edits the fixture's associated channels.
	/// </summary>
    public class EditProfileWizardPage : FixtureWizardPageBase
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public EditProfileWizardPage() :
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/edit-channels/")
        {
            Title = "Edit Channels";
            Description = "Define the fixture's channels.";            
        }

        #endregion
    }
}
