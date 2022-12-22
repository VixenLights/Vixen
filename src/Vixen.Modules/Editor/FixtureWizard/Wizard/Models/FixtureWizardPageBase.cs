using Orc.Wizard;

namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
	/// <summary>
	/// Base class for an intelligent fixture wizard page.
	/// </summary>
	public abstract class FixtureWizardPageBase : WizardPageBase
	{
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpURL">Online help URL</param>
		public FixtureWizardPageBase(string helpURL)
		{
            // Store off the help URL
            HelpURL = helpURL;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Online help URL associated with the page.
        /// </summary>
        public string HelpURL { get; private set; }

        #endregion
    }
}
