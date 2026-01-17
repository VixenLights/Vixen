using System.Diagnostics;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using VixenModules.Editor.FixtureGraphics.OpenGL.Primitives;

namespace Common.OpenGLCommon.Constructs.Shaders
{
	/// <summary>
	/// Base class for a shader program.
	/// </summary>
	public abstract class ShaderProgramBase
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>		
		protected ShaderProgramBase(string vertexShaderSrc, string fragmentShaderSrc)
		{			
			// Create a dictionary to store attributes by name
			Attributes = new Dictionary<string, AttributeInfo>();

			// Create a dictionary to store uniforms by name
			Uniforms = new Dictionary<string, UniformInfo>();

			// Create and compile the vertex shader
			VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(VertexShaderID, vertexShaderSrc);
			GL.CompileShader(VertexShaderID);

			// Create and compile the fragment shader
			FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(FragmentShaderID, fragmentShaderSrc);
			GL.CompileShader(FragmentShaderID);

			// Create the program
			ProgramID = GL.CreateProgram();
			
			// Attach the vertex shader to the program
			GL.AttachShader(ProgramID, VertexShaderID);
			string info = GL.GetShaderInfoLog(VertexShaderID);
			Debug.Assert(string.IsNullOrEmpty(info));

			// Attach the fragment shader to the program
			GL.AttachShader(ProgramID, FragmentShaderID);
			string info2 = GL.GetShaderInfoLog(FragmentShaderID);
			Debug.Assert(string.IsNullOrEmpty(info2));

			// Link the program
			Link();

			// Detach the shaders since they have already been compiled and linked to the program
			GL.DetachShader(ProgramID, VertexShaderID);
			GL.DetachShader(ProgramID, FragmentShaderID);

			// Delete te shaders since they have already been compiled and linked to the program
			GL.DeleteShader(FragmentShaderID);
			GL.DeleteShader(VertexShaderID);
		}
				
		#endregion

		#region Fields

		/// <summary>
		/// Dictionary of OpenGL attributes name.
		/// </summary>
		private readonly Dictionary<String, AttributeInfo> Attributes;

		/// <summary>
		/// Dictionary of OpenGL uniforms by name.
		/// </summary>
		private readonly Dictionary<String, UniformInfo> Uniforms;

		/// <summary>
		/// Dictionary of OpenGL buffer addresses by name.
		/// </summary>
		private readonly Dictionary<String, uint> Buffers;

		/// <summary>
		/// Flag indicating if the resources have been disposed.
		/// </summary>
		private bool _disposed;

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets the ID of the Shader Program.
		/// </summary>
		protected int ProgramID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the Vertex Shader.
		/// </summary>
		private int VertexShaderID { get; set; }

		/// <summary>
		/// Gets or sets the ID of the Fragment Shader.
		/// </summary>
		private int FragmentShaderID { get; set; }
		
		#endregion

		#region Private Methods
		
		/// <summary>
		/// Links the shader program.
		/// </summary>
		protected virtual void Link()
		{			
			// Link the shader program
			GL.LinkProgram(ProgramID);

			// Check the info log for problems
			string infoLog = GL.GetProgramInfoLog(ProgramID);
			Debug.Assert(string.IsNullOrEmpty(infoLog));

			// Gets the number of attributes associated with the shader program
			int attributeCount;
			GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out attributeCount);

			// Gets the number of uniforms associated with the shader program 
			int uniformCount;
			GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out uniformCount);

			// Loop over the discovered attributes
			for (int i = 0; i < attributeCount; i++)
			{
				// Retrieve information about the attribute
				AttributeInfo info = new AttributeInfo();
				string name;
				int length = 0;
				int size;
				ActiveAttribType attributeType;
				GL.GetActiveAttrib(ProgramID, i, 256, out length, out size, out attributeType, out name);

				// Store off the attribute size
				info.Size = size;

				// Store off the attribute type
				info.AttribType = attributeType;

				// Store off the attribute's name, address and information in the dictionary				
				info.Name = name;
				info.Address = GL.GetAttribLocation(ProgramID, info.Name);
				Attributes.Add(name.ToString(), info);
			}

			// Loop over the discovered uniforms
			for (int i = 0; i < uniformCount; i++)
			{
				// Retrieve information about the uniform
				UniformInfo info = new UniformInfo();
				int length = 0;
				string name;
				int size;
				ActiveUniformType uniformType;
				GL.GetActiveUniform(ProgramID, i, 256, out length, out size, out uniformType, out name);

				// Store off the uniform type
				info.UniformType = uniformType;

				// Store off the uniform size
				info.Size = size;

				// Store off the uniform's name, address and information in the dictionary
				info.Name = name.ToString();
				Uniforms.Add(name.ToString(), info);
				info.Address = GL.GetUniformLocation(ProgramID, info.Name);
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Gets the address of the uniform with the specified name.
		/// </summary>
		/// <param name="name">Name of the uniform</param>
		/// <returns>Address of the uniform</returns>
		protected int GetUniform(string name)
		{
			Debug.Assert(Uniforms.ContainsKey(name));
			return Uniforms[name].Address;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public virtual void Use()
		{
			// Indicate to the GPU to use the program associated with this shader
			GL.UseProgram(ProgramID);			
		}

		/// <summary>
		/// Transfers the model matrix to the GPU.
		/// </summary>
		/// <param name="modelMatrix"></param>
		public void TransferModelMatrixUniform(Matrix4 modelMatrix)
		{
			// Transfer the model matrix
			GL.UniformMatrix4(GetUniform("modelMatrix"), false, ref modelMatrix);			
		}

		/// <summary>
		/// Transfers global uniforms that are shared by multiple lines.
		/// </summary>		
		/// <param name="projectionMatrix">Projection matrix used to draw</param>
		/// <param name="viewMatrix">View matrix used to draw</param>
		public virtual void TransferGlobalUniforms(
			Matrix4? projectionMatrix,
			Matrix4? viewMatrix)
		{
			// If the view matrix has a value then...
			if (viewMatrix.HasValue)
			{
				// Transfer the view matrix 
				Matrix4 localViewMatrix = viewMatrix.Value;
				GL.UniformMatrix4(GetUniform("viewMatrix"), false, ref localViewMatrix);
			}

			// If the projection matrix has a value then...
			if (projectionMatrix.HasValue)
			{
				// Transfer the projection matrix
				Matrix4 localProjectionMatrix = projectionMatrix.Value;
				GL.UniformMatrix4(GetUniform("projectionMatrix"), false, ref localProjectionMatrix);
			}
		}

		/// <summary>
		/// Returns Gl.GetProgramInfoLog(ProgramID), which contains any linking errors.
		/// </summary>
		public string ProgramLog
		{
			get { return GL.GetProgramInfoLog(ProgramID); }
		}

		#endregion
	}
}
