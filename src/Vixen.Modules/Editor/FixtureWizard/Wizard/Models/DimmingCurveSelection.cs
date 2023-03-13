namespace VixenModules.Editor.FixtureWizard.Wizard.Models
{
	/// <summary>
	/// Identifies if and how dimming curves should be applied to the fixture.
	/// </summary>
	public enum DimmingCurveSelection
    {
        /// <summary>
        /// Don't add any dimming curves.
        /// </summary>
        NoDimmingCurve,

        /// <summary>
        /// Add one dimming curve to the fixture's color flow.
        /// </summary>
        OneDimmingCurvePerFixture,

        /// <summary>
        /// Add one dimming curve for each color channel.
        /// </summary>
        OneDimmingCurvePerColor,
    };
}
