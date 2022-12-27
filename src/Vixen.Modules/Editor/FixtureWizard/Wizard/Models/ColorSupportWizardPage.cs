namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
    /// <summary>
    /// Wizard page configures fixture color options.
    /// </summary>
    public class ColorSupportWizardPage : SummaryWizardPageBase
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ColorSupportWizardPage() :
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/color-support/")
        {
            Title = "Color Support";
            Description = "Select the color support applicable to the fixture.  " +
                "Color Mixing should be selected when the fixture uses a multiple color light source or multiple light sources of different colors.  " +
                "Color wheel should be selected when the fixture has a single white light source and is equipped with a color wheel to change colors.  ";                            
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Indicates the fixture supports color mixing.
        /// </summary>
		public bool ColorMixing { get; set; }

        /// <summary>
        /// Indicates the fixture uses a color wheel as its primary source.
        /// </summary>
        public bool ColorWheel { get; set; }

        /// <summary>
        /// Indicates color is going to be handled by other means (individual pixel nodes).
        /// </summary>
        public bool NoColorSupport { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the summary string for the page.
        /// </summary>
        /// <returns>Summary string for the page</returns>
        protected override string GetSummaryString()
        {
            return ColorMixing ? "Color Mixing" : "Color Wheel";
        }

        #endregion
    }
}
