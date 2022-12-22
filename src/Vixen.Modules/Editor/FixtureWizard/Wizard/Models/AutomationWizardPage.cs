namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
    /// <summary>
    /// Wizard page configures fixture automation options.
    /// </summary>
    public class AutomationWizardPage : SummaryWizardPageBase
    {
        #region Constructor
        
        /// <summary>
        /// Constructor
        /// </summary>
        public AutomationWizardPage() : 
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/automation/")
        {
            Title = "Automation";
            Description = "These options configure special filters that make it easier to control the fixtures when sequencing.";    
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Controls whether filters are used to automatically open and close the shutter based on
        /// presence or abscence of color intents.
        /// </summary>
        public bool AutomaticallyOpenAndCloseShutter { get; set; }

        /// <summary>
        /// Controls whether filters are used to convert color intents into color wheel commands.
        /// </summary>
        public bool AutomaticallyControlColorWheel { get; set; }

        /// <summary>
        /// Controls whether color intensity is used to control the fixture dimmer channel.
        /// </summary>
        public bool AutomaticallyControlDimmer { get; set; }

        /// <summary>
        /// Controls whether filters are used to automatically open and close the prism based on
        /// presence or abscence of prism index intents.
        /// </summary>
        public bool AutomaticallyOpenAndClosePrism { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the summary string for the page.
        /// </summary>
        /// <returns>Summary string for the page</returns>
        protected override string GetSummaryString()
        {
            string summary = string.Empty;

            // If automatically opening and closing the shutter then...
            if (AutomaticallyOpenAndCloseShutter)
            {
                summary += "Shutter, ";
            }

            // If automatically creating color wheel commands then...
            if (AutomaticallyControlColorWheel)
            {
                summary += "Color Wheel, ";
            }

            // If automatically controlling the dimmer channel then...
            if (AutomaticallyControlDimmer)
            {
                summary += "Dimmer,";
            }

            // If automatically opening and closing the prism then...
            if (AutomaticallyOpenAndClosePrism)
            {
                summary += "Prism, ";
            }

            // Trim off the last separators
            summary = summary.TrimEnd(new char[] { ',', ' ' });

            return summary;
        }

        #endregion
    }
}
