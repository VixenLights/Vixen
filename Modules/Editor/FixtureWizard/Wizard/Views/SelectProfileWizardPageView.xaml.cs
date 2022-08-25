namespace VixenModules.Editor.FixtureWizard.Wizard.Views
{
    /// <summary>
    /// Maintains the fixture select profile wizard page view.
    /// </summary>
    public partial class SelectProfileWizardPageView
    {
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public SelectProfileWizardPageView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// User control loaded event, used to draw background graphics.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e"><Event arguments/param>
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Draw two moving heads providing bookends to the widgets in the middle of the page
            DrawMovingHeads(300, System.Drawing.Color.DodgerBlue, LeftMainViewport, RightMainViewport);
        }

        #endregion
    }
}