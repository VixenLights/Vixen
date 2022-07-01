using OpenTK.Graphics.OpenGL;

namespace VixenModules.Editor.FixtureGraphics.OpenGL.Primitives
{
	/// <summary>
	/// Maintains OpenGL attribute meta-data.
	/// </summary>
	public class AttributeInfo : MetaDataInfo
	{
		#region Public Properties

		/// <summary>
		/// Type of the attribute.
		/// </summary>
		public ActiveAttribType AttribType { get; set; }

		#endregion
	}
}
