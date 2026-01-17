using OpenTK.Graphics.OpenGL;

namespace Common.OpenGLCommon.Constructs.Shaders
{
	/// <summary>
	/// Base class for a shader that is associated with a single VAO ID.
	/// </summary>
	public abstract class SingleUseShaderProgramBase : ShaderProgramBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="vertexShaderSrc"></param>
		/// <param name="fragmentShaderSrc"></param>
		protected SingleUseShaderProgramBase(string vertexShaderSrc, string fragmentShaderSrc) : base(vertexShaderSrc, fragmentShaderSrc)
		{			
			// Initialize the vertex array object ID to invalid
			VaoID = -1;

			// Create the vertex array object 
			GL.GenVertexArrays(1, out int vao);
			VaoID = vao;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Vertex array object ID associated with this shader program.
		/// </summary>		
		public int VaoID { get; set; }

		#endregion

		#region Public Methods

		/// <inheritdoc/>
		public override void Use()
		{
			// Call the base class implementation
			base.Use();

			// Bind the vertex array
			GL.BindVertexArray(VaoID);
		}

		#endregion
	}
}
