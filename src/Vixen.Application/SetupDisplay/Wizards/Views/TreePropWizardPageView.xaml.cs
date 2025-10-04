namespace VixenApplication.SetupDisplay.Wizards.Views
{
	/// <summary>
	/// Maintains an Arch prop Wizard page.
	/// </summary>
	public partial class TreePropWizardPageView : WizardPageViewBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public TreePropWizardPageView() 
		{
			// Initialize UI component
			InitializeComponent();

			// Pass the OpenTK WPF control to the base class
			OpenTkCntrl = OpenTkControl;

			// Initialize the OpenTK WPF Control
			Initialize();			
		}

		#endregion
	}
}
