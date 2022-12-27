namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	using Catel.MVVM;
	using Orc.Wizard;
	using System.Linq;
	using System.Windows.Forms;
	using System.Windows.Input;
	using VixenModules.App.Curves;
	using VixenModules.Editor.FixtureWizard.Wizard.Models;

    /// <summary>
	/// Wizard view model page that configures dimming curves for the fixture.
	/// </summary>
	public class DimmingCurveWizardPageViewModel : WizardPageViewModelBase<DimmingCurveWizardPage>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wizardPage">Dimming support wizard page model</param>
        public DimmingCurveWizardPageViewModel(DimmingCurveWizardPage wizardPage) :
            base(wizardPage)
        {
            // Retrieve the color support wizard page model
            ColorSupportWizardPage colorSupportPage = (ColorSupportWizardPage)Wizard.Pages.Single(page => page is ColorSupportWizardPage);

            // Only allow adding a dimming curve per color for color mixing fixtures
            EnableDimmingCurvePerColor = colorSupportPage.ColorMixing;
            
            // Create the command to edit the dimming curve
            EditDimmingCurveCommand = new Command(EditDimmingCurve);                       
        }

		#endregion

		#region Private Methods

        /// <summary>
        /// Edits the dimming curve associated with the fixture.
        /// </summary>
		private void EditDimmingCurve()
        {
            // Create the curve editor
            using (CurveEditor editor = new CurveEditor(DimmingCurve))
            {
                // Show the curve editor
                // If the user selects OK button on the editor then...
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    // Update the dimming curve associated with the fixture
                    DimmingCurve = editor.Curve;                    
                }
            }
        }

		#endregion

		#region Public Catel Properties

        /// <summary>
        /// Dimming curve associated with the fixture.
        /// </summary>
		[ViewModelToModel]
        public Curve DimmingCurve { get; set; }

        /// <summary>
        /// Indicates no dimming curve should be associated with the fixture.
        /// </summary>
        [ViewModelToModel]
        public bool NoDimmingCurve { get; set; }

        /// <summary>
        /// Indicates one dimming curve should be inserted into the fixture color flow.
        /// </summary>
        [ViewModelToModel]
        public bool OneDimmingCurvePerFixture { get; set; }

        /// <summary>
        /// Indicates that a dimming curve should be applied to each color channel of the fixture.
        /// </summary>
        [ViewModelToModel]
        public bool OneDimmingCurvePerColor { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Command for editing the dimming curve.
		/// </summary>
		public ICommand EditDimmingCurveCommand { get; set; }
        
        /// <summary>
        /// Flag indicates if the dimming curve per color channel option should be enabled.
        /// </summary>
        public bool EnableDimmingCurvePerColor { get; set; }

        #endregion
    }
}
