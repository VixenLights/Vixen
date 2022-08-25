using Catel.Services;
using Orc.Wizard;
using System.Windows;

namespace VixenModules.Editor.FixtureWizard.Wizard
{
	/// <summary>
	/// Orc Wizard navigation controller that injects Vixen button styles into the Orc Wizard.
	/// </summary>
	public class FixtureWizardNavigationController : FastForwardNavigationController
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wizard">Wizard associated with the controller</param>
		/// <param name="languageService">Language service for localization</param>
		/// <param name="messageService">Message service for displaying message boxes</param>
		public FixtureWizardNavigationController(
			IWizard wizard, 
			ILanguageService languageService,
			IMessageService messageService) : base(wizard, languageService, messageService)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Configures the specified wizard button with the Vixen button style.
		/// </summary>
		/// <param name="wizardButton"></param>
		private void ConfigureVixenStyleOnWizardNavigationButton(WizardNavigationButton wizardButton)
		{
			// The Orc Wizard changes the style using this delegate.
			// It this delegate is NOT cleared then the Orc styles will get re-applied.
			wizardButton.StyleEvaluator = null;

			// Name (key) of the Vixen style
			const string VixenButtonStyle = "VixenButtonStyle";

			// Configure the wizard button with the Vixen style
			wizardButton.Style = (Style)Application.Current.FindResource(VixenButtonStyle);
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override WizardNavigationButton CreateBackButton(IWizard wizard)
		{
			// Call the base class implementation
			WizardNavigationButton button = base.CreateBackButton(wizard);

			// Configure the button with the Vixen style
			ConfigureVixenStyleOnWizardNavigationButton(button);
			
			return button;
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override WizardNavigationButton CreateForwardButton(IWizard wizard)
		{
			// Call the base class implementation
			WizardNavigationButton button = base.CreateForwardButton(wizard);

			// Configure the button with the Vixen style
			ConfigureVixenStyleOnWizardNavigationButton(button);

			return button;
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override WizardNavigationButton CreateFinishButton(IWizard wizard)
		{
			// Call the base class implementation
			WizardNavigationButton button = base.CreateFinishButton(wizard);

			// Configure the button with the Vixen style
			ConfigureVixenStyleOnWizardNavigationButton(button);

			return button;
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override WizardNavigationButton CreateCancelButton(IWizard wizard)
		{
			// Call the base class implementation
			WizardNavigationButton button = base.CreateCancelButton(wizard);

			// Configure the button with the Vixen style
			ConfigureVixenStyleOnWizardNavigationButton(button);

			return button;
		}

		#endregion
	}
}
