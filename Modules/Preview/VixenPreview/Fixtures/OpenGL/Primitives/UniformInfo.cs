using OpenTK.Graphics.OpenGL;

namespace VixenModules.Preview.VixenPreview.Fixtures.OpenGL.Primitives
{
	/// <summary>
	/// Maintains OpenGL uniform meta-data.
	/// </summary>
	public class UniformInfo : MetaDataInfo
	{
		#region Public Properties
		
		/// <summary>
		/// Type of the uniform.
		/// </summary>
		public ActiveUniformType UniformType { get; set; }

		#endregion
	}
}
