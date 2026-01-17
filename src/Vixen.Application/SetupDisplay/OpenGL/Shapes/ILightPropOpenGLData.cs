using VixenApplication.SetupDisplay.OpenGL.Shapes;

namespace VixenApplication.SetupDisplay.OpenGL
{
	/// <summary>
	/// Maintains OpenGL data required to draw a light based prop.
	/// </summary>
	public interface ILightPropOpenGLData : IPropOpenGLData
	{
		/// <summary>
		/// Determines what light points need to be drawn and with what color.
		/// </summary>
		/// <param name="referenceHeight">Logical height of preview view</param>
		void UpdateDrawPoints(float referenceHeight);		
	}
}
