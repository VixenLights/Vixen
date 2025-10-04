using VixenApplication.SetupDisplay.OpenGL;

namespace VixenApplication.SetupDisplay.Wizards.ViewModels
{
	/// <summary>
	/// Maintains a prop wizard page view model.
	/// </summary>
	public interface IPropWizardPageViewModel
	{
		/// <summary>
		/// Engine for drawing OpenGL props.
		/// </summary>
		OpenGLPropDrawingEngine DrawingEngine { get; set; }
	}
}
