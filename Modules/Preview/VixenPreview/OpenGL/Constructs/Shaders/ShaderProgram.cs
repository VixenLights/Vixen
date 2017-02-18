using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace OpenTKDemo.Constructs.Shaders
{
	public class ShaderProgram : IDisposable
	{
		#region Properties
		/// <summary>
		/// Specifies the OpenGL shader program ID.
		/// </summary>
		public int ProgramID { get; private set; }

		/// <summary>
		/// Specifies the vertex shader used in this program.
		/// </summary>
		public Shader VertexShader { get; private set; }

		/// <summary>
		/// Specifies the fragment shader used in this program.
		/// </summary>
		public Shader FragmentShader { get; private set; }

		/// <summary>
		/// Specifies whether this program will dispose of the child 
		/// vertex/fragment programs when the IDisposable method is called.
		/// </summary>
		public bool DisposeChildren { get; set; }

		private Dictionary<string, ProgramParam> shaderParams;

		/// <summary>
		/// Queries the shader parameter hashtable to find a matching attribute/uniform.
		/// </summary>
		/// <param name="name">Specifies the case-sensitive name of the shader attribute/uniform.</param>
		/// <returns>The requested attribute/uniform, or null on a failure.</returns>
		public ProgramParam this[string name]
		{
			get { return shaderParams.ContainsKey(name) ? shaderParams[name] : null; }
		}

		/// <summary>
		/// Returns Gl.GetProgramInfoLog(ProgramID), which contains any linking errors.
		/// </summary>
		public string ProgramLog
		{
			get { return GL.GetProgramInfoLog(ProgramID); }
		}
		#endregion

		#region Constructors and Destructor
		/// <summary>
		/// Links a vertex and fragment shader together to create a shader program.
		/// </summary>
		/// <param name="vertexShader">Specifies the vertex shader.</param>
		/// <param name="fragmentShader">Specifies the fragment shader.</param>
		public ShaderProgram(Shader vertexShader, Shader fragmentShader)
		{
			this.VertexShader = vertexShader;
			this.FragmentShader = fragmentShader;
			this.ProgramID = GL.CreateProgram();
			this.DisposeChildren = false;

			GL.AttachShader(ProgramID, vertexShader.ShaderID);
			GL.AttachShader(ProgramID, fragmentShader.ShaderID);
			GL.LinkProgram(ProgramID);

			GetParams();
		}

		/// <summary>
		/// Creates two shaders and then links them together to create a shader program.
		/// </summary>
		/// <param name="vertexShaderSource">Specifies the source code of the vertex shader.</param>
		/// <param name="fragmentShaderSource">Specifies the source code of the fragment shader.</param>
		public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
			: this(new Shader(vertexShaderSource, ShaderType.VertexShader), new Shader(fragmentShaderSource, ShaderType.FragmentShader))
		{
			DisposeChildren = true;
		}

		~ShaderProgram()
		{
			Dispose(false);
		}
		#endregion

		#region GetParams
		/// <summary>
		/// Parses all of the parameters (attributes/uniforms) from the two attached shaders
		/// and then loads their location by passing this shader program into the parameter object.
		/// </summary>
		private void GetParams()
		{
			shaderParams = new Dictionary<string, ProgramParam>();
			foreach (ProgramParam pParam in VertexShader.ShaderParams)
			{
				if (!shaderParams.ContainsKey(pParam.Name))
				{
					shaderParams.Add(pParam.Name, pParam);
					pParam.GetLocation(this);
				}
			}
			foreach (ProgramParam pParam in FragmentShader.ShaderParams)
			{
				if (!shaderParams.ContainsKey(pParam.Name))
				{
					shaderParams.Add(pParam.Name, pParam);
					pParam.GetLocation(this);
				}
			}
		}
		#endregion

		#region Methods
		public void Use()
		{
			GL.UseProgram(this.ProgramID);
		}

		public int GetUniformLocation(string Name)
		{
			Use();
			return GL.GetUniformLocation(ProgramID, Name);
		}

		public int GetAttributeLocation(string Name)
		{
			Use();
			return GL.GetAttribLocation(ProgramID, Name);
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (ProgramID != 0)
			{
				// Make sure this program isn't being used
				//if (GL.CurrentProgram == ProgramID) GL.UseProgram(0);

				GL.DetachShader(ProgramID, VertexShader.ShaderID);
				GL.DetachShader(ProgramID, FragmentShader.ShaderID);
				GL.DeleteProgram(ProgramID);

				if (DisposeChildren)
				{
					VertexShader.Dispose();
					FragmentShader.Dispose();
				}

				ProgramID = 0;
			}
		}
		#endregion
	}
}