namespace VixenApplication.SetupDisplay.Wizards.Pages
{
	/// <summary>
	/// Maintains light prop wizard page data.
	/// </summary>
	public interface ILightPropWizardPage : IBasePropWizardPage
	{
		/// <summary>
		/// Size of the lights.
		/// </summary>
		int LightSize { get; set; }
	}
}
