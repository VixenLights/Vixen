namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
	using System.Diagnostics;
	using VixenModules.App.Curves;

	/// <summary>
	/// Wizard page configures fixture dimming curves.
	/// </summary>
	public class DimmingCurveWizardPage : SummaryWizardPageBase
    {
		#region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
		public DimmingCurveWizardPage() : 
            base("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/setup-display-elements/intelligent-fixture-wizard/dimming-curves/")
        {
            Title = "Dimming Curves";
            Description = "These options configure if any dimming curves are applied to the fixture outputs.";
            
            // Default to not including a dimming curve
            NoDimmingCurve = true;

            DimmingCurve = new Curve();
        }

		#endregion

		#region Public Properties

        /// <summary>
        /// Dimming curve being inserted into the flow.
        /// </summary>
		public Curve DimmingCurve { get; set; }

        /// <summary>
        /// No dimming curve is desired.
        /// </summary>
        public bool NoDimmingCurve { get; set; }

        /// <summary>
        /// One dimming curve for the fixture is desired.
        /// </summary>
        public bool OneDimmingCurvePerFixture { get; set; }

        /// <summary>
        /// One dimming curve per color channel is desired.
        /// </summary>
        public bool OneDimmingCurvePerColor { get; set; }

		#endregion

		#region Public Methods

        /// <summary>
        /// Gets the user's dimming curve selection as an enumeration.
        /// </summary>
        /// <returns>Dimming curve selection as an enumeration</returns>
		public DimmingCurveSelection GetDimmingCurveSelection()
        {
            // Default to no dimming curve selectd
            DimmingCurveSelection dimmingCurveSelection = DimmingCurveSelection.NoDimmingCurve;

            // If one dimming curve for the fixture was selected then...
            if (OneDimmingCurvePerFixture)
            {
                dimmingCurveSelection = DimmingCurveSelection.OneDimmingCurvePerFixture;
            }
            // Otherwise if a dimming curve per color channel was selected then...
            else if (OneDimmingCurvePerColor)
            {
                dimmingCurveSelection = DimmingCurveSelection.OneDimmingCurvePerColor;
            }

            return dimmingCurveSelection;   
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the summary string for the page.
        /// </summary>
        /// <returns>Summary string for the page</returns>
        protected override string GetSummaryString()
        {
            string summary = string.Empty;

            switch(GetDimmingCurveSelection())
            {
                case DimmingCurveSelection.NoDimmingCurve:
                    summary = "No Dimming Selected";
                    break;
                case DimmingCurveSelection.OneDimmingCurvePerFixture:
                    summary = "Dimming Curve for Fixture";
                    break;
                case DimmingCurveSelection.OneDimmingCurvePerColor:
                    summary = "Dimming Curve for Each Color Channel";
                    break;
                default:
                    Debug.Assert(false, "Unsupported Dimming Curve Selection");
                    break;
            }
                                  
            return summary;
        }

        #endregion
    }
}
