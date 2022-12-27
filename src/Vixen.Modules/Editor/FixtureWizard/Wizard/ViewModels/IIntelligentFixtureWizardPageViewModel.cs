namespace VixenModules.Editor.FixtureWizard.Wizard.ViewModels
{
	/// <summary>
	/// Maintains an intelligent fixture wizard page view model.
	/// </summary>
	public interface IIntelligentFixtureWizardPageViewModel
	{
		/// <summary>
		/// Returns whether it is allowed to move back to the previous wizard page.
		/// </summary>
		/// <returns>True when it is allowed to move back to the previous wizard page</returns>
		bool CanMoveBack();

		/// <summary>
		/// Returns whether is is allowed to move to the next wizard page.
		/// </summary>
		/// <returns>True when it is allowed to move to the next wizard page.</returns>
		bool CanMoveNext();
	}
}
