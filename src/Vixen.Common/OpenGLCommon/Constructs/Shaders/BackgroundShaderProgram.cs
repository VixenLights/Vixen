using OpenTK.Graphics.OpenGL;

namespace Common.OpenGLCommon.Constructs.Shaders
{
	/// <summary>
	/// Shader program to draw the preview background.
	/// </summary>
	public class BackgroundShaderProgram : SingleUseShaderProgramBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public BackgroundShaderProgram() : base(VertexShaderSrc, FragmentShaderSrc)
		{
		}

		#endregion
		
		#region Private Constants

		/// <summary>
		/// Vertex GLSL shader source.
		/// </summary>
		private const string VertexShaderSrc = @"
			#version 330

			in vec3 vertexPosition;
			in vec2 textureCoord;

			out vec2 texCoord;

			uniform mat4 projectionMatrix;
			uniform mat4 viewMatrix;
			uniform mat4 modelMatrix;

			void main(void)
			{
				texCoord = textureCoord;
				gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertexPosition, 1);
			}
			";

		/// <summary>
		/// Fragment GLSL shader source.
		/// </summary>
		private const string FragmentShaderSrc = @"
			#version 330

			uniform sampler2D textureIn;
			uniform float ambientStrength;
			const vec3 lightColor = vec3(1,1,1);

			in vec2 texCoord;

			out vec4 fragment;

			void main(void)
			{
   				vec4 color = texture(textureIn, texCoord);
	
				vec3 ambient = ambientStrength * lightColor;

				vec3 result = ambient * color.rgb;
				fragment = vec4(result, 1.0f);
			}
			";

		#endregion

		#region Public Methods

		/// <summary>
		/// Transfers the background brightness to the GPU.
		/// </summary>
		/// <param name="brightness">Brightness of the background</param>
		public void TransferBrightnessUniform(float brightness)
		{
			GL.Uniform1(GetUniform("ambientStrength"), brightness);
		}

		#endregion
	}
}
