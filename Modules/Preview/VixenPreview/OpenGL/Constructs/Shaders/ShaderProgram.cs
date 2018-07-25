using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders
{
	public class ShaderProgram : IDisposable
	{
		#region Fields
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public static int VertexPosition = 0;

		public static int VertexColor = 1;

		public static int VertexSize = 2;

		public static int TextureCoords = 3;

		#endregion

		#region Properties
		/// <summary>
		/// Specifies the OpenGL shader program ID.
		/// </summary>
		public int ProgramId { get; private set; }

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
			get { return GL.GetProgramInfoLog(ProgramId); }
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
			this.ProgramId = GL.CreateProgram();
			this.DisposeChildren = false;

			GL.BindAttribLocation(ProgramId, VertexPosition, "vertexPosition");
			GL.BindAttribLocation(ProgramId, VertexColor, "vertexColor");
			GL.BindAttribLocation(ProgramId, VertexSize, "vertexSize");
			GL.BindAttribLocation(ProgramId, TextureCoords, "textureCoord");

			GL.AttachShader(ProgramId, vertexShader.ShaderID);
			GL.AttachShader(ProgramId, fragmentShader.ShaderID);

			GL.LinkProgram(ProgramId);

			int status;
			GL.GetProgram(ProgramId, GetProgramParameterName.LinkStatus, out status);
			if (status == 0)
			{
				Logging.Error("Error Linking Program: {0}", ProgramLog);
			}

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
			GL.UseProgram(this.ProgramId);
		}

		public int GetUniformLocation(string Name)
		{
			Use();
			return GL.GetUniformLocation(ProgramId, Name);
		}

		public int GetAttributeLocation(string Name)
		{
			Use();
			return GL.GetAttribLocation(ProgramId, Name);
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
			if (ProgramId != 0)
			{
				GL.DetachShader(ProgramId, VertexShader.ShaderID);
				GL.DetachShader(ProgramId, FragmentShader.ShaderID);
				GL.DeleteProgram(ProgramId);

				if (DisposeChildren)
				{
					VertexShader.Dispose();
					FragmentShader.Dispose();
				}

				ProgramId = 0;
			}
		}
		#endregion
	}
}