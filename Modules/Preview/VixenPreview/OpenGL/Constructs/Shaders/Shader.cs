using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

#if USE_NUMERICS
using System.Numerics;
#endif

namespace VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders
{
	public class Shader : IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		#region Properties
		/// <summary>
		/// Specifies the OpenGL ShaderID.
		/// </summary>
		public int ShaderID { get; private set; }

		/// <summary>
		/// Specifies the type of shader.
		/// </summary>
		public ShaderType ShaderType { get; private set; }

		/// <summary>
		/// Contains all of the attributes and uniforms parsed from this shader source.
		/// </summary>
		public ProgramParam[] ShaderParams { get; private set; }

		/// <summary>
		/// Returns Gl.GetShaderInfoLog(ShaderID), which contains any compilation errors.
		/// </summary>
		public string ShaderLog
		{
			get { return GL.GetShaderInfoLog(ShaderID); }
		}
		#endregion

		#region Constructor and Destructor
		/// <summary>
		/// Compiles a shader, which can be either vertex, fragment or geometry.
		/// </summary>
		/// <param name="source">Specifies the source code of the shader object.</param>
		/// <param name="type">Specifies the type of shader to create (either vertex, fragment or geometry).</param>
		public Shader(string source, ShaderType type)
		{
			this.ShaderType = type;
			this.ShaderID = GL.CreateShader(type);
			GL.ShaderSource(ShaderID, source);
			GL.CompileShader(ShaderID);

			int status;
			GL.GetShader(ShaderID, ShaderParameter.CompileStatus, out status);
			if (status == 0)

				Logging.Error("Error compiling shader: {0}", GL.GetShaderInfoLog(ShaderID));

			GetParams(source);
		}

		~Shader()
		{
			Dispose(false);
		}
		#endregion

		#region GetParams
		private Type GlslTypeFromString(string type)
		{
			switch (type.ToLower())
			{
				case "float": return typeof(float);
				case "bool": return typeof(bool);
				case "int": return typeof(int);
				case "uint": return typeof(uint);
				case "vec2": return typeof(Vector2);
				case "vec3": return typeof(Vector3);
				case "vec4": return typeof(Vector4);
				case "mat3": return typeof(Matrix3);
				case "mat4": return typeof(Matrix4);
				case "sampler2d":
				case "sampler2dshadow":
				case "sampler1d":
				case "sampler1dshadow":
				case "sampler3d":
				case "sampler2darray":
				case "sampler2darrayshadow": return typeof(Texture);
				default: throw new Exception(string.Format("Unsupported GLSL type {0}", type));
			}
		}

		/// <summary>
		/// Parses the shader source code and finds all attributes and uniforms
		/// to cache their location for speedy lookup.
		/// </summary>
		/// <param name="source">Specifies the source code of the shader.</param>
		private void GetParams(string source)
		{
			List<ProgramParam> shaderParams = new List<ProgramParam>();
			var tokens = GlslLexer.GetTokensFromMemory(source);

			for (int i = 0; i < tokens.Count; i++)
			{
				if (tokens[i].TokenType == GlslLexer.TokenType.Keyword && (tokens[i].Text == "uniform" || tokens[i].Text == "attribute" || tokens[i].Text == "in"))
				{
					// get the parameter type (either uniform or attribute/in)
					ParamType paramType = (tokens[i].Text == "uniform" ? ParamType.Uniform : ParamType.Attribute);
					i++;    // move past the uniform/attribute/in keyword

					// get the glsl type of the parameter
					if (i == tokens.Count) break;
					Type type = GlslTypeFromString(tokens[i].Text);
					i++;    // move past the type

					// now continue reading parameters until we hit EOF, semi-colon or the glsl programmer assigns a default value
					while (i < tokens.Count && tokens[i].Text != ";")
					{
						if (tokens[i].TokenType == GlslLexer.TokenType.Word) shaderParams.Add(new ProgramParam(type, paramType, tokens[i].Text));
						else if (tokens[i].Text == "=") break;  // they've assigned a default value, so continue on
						i++;
					}
				}
			}

			this.ShaderParams = shaderParams.ToArray();
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
			if (ShaderID != 0)
			{
				GL.DeleteShader(ShaderID);
				ShaderID = 0;
			}
		}
		#endregion
	}
}
