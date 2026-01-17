using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Common.OpenGLCommon.Constructs.Shaders
{
	/// <summary>
	/// Color line shader program.
	/// </summary>
	public class ColorLineShaderProgram : ShaderProgramBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ColorLineShaderProgram() : base(VertexShaderSrc, FragmentShaderSrc)
		{
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Transfers the color of the line.
		/// </summary>
		/// <param name="color">Color of the line</param>
		public void TransferColorUniform(Color4 color)
		{			
			GL.Uniform4(GetUniform("uColor"), color);
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// GLSL source for the vertex shader.
		/// </summary>
		protected static string VertexShaderSrc = @"
		#version 330
 
		in vec3 vPosition;
						
		uniform mat4 modelMatrix;
		uniform mat4 viewMatrix;
		uniform mat4 projectionMatrix;		
		 
		void main()
		{
		    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vPosition, 1.0);		    			
		}
		";

		/// <summary>
		/// GLSL source for the fragment shader.
		/// </summary>
		protected static string FragmentShaderSrc = @"
		# version 330 core 
		uniform vec4 uColor;

		out vec4 FragColor; 

		void main() 
		{ 
			FragColor = uColor;			
		}";

		#endregion
	}
}
