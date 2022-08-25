using Orc.Wizard;

namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
    /// <summary>
    /// Base Wizard page that provides a summary string for the last page of the wizard.
    /// </summary>
    public abstract class SummaryWizardPageBase : FixtureWizardPageBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpURL">Help URL</param>
        public SummaryWizardPageBase(string helpURL) : base(helpURL)
        {            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the summary info for the page.
        /// This information is displayed on the last page of the wizard.
        /// </summary>
        /// <returns></returns>
        public override ISummaryItem GetSummary()
        {
            return new SummaryItem
            {
                Title = Title,
                Summary = GetSummaryString(),
            };
        }

        #endregion
      
        #region Protected Methods

        /// <summary>
        /// Allows the derived page to construct the summary string.
        /// </summary>
        /// <returns>The string to display on the summary page</returns>
        protected abstract string GetSummaryString();

        #endregion
    }
}
