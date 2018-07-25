using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex;

namespace VixenModules.Preview.VixenPreview.OpenGL
{
	public class Background: IDrawable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private VixenPreviewData _data;
		private Texture _backgroundTexture;
		
		private ShaderProgram _backgroundProgram;

		private VBO<float> _points;
		//private VBO<Vector2> _backgroundUv;
		private VBO<int> _backgroundElements;

		public Background(VixenPreviewData data)
		{
			_data = data;
			InitializeBackground();
		}

		public int Height { get; set; }

		public int Width { get; set; }

		public bool HasBackground { get; private set; }

		public void Draw(Matrix4 fov, Matrix4 cameraView)
		{
			//Logging.Debug("Entering background draw.");
			if (!HasBackground) return;
			//Logging.Debug("We have a background.");
			_backgroundProgram.Use();
			//Logging.Debug("Set background program.");
			GL.ActiveTexture(TextureUnit.Texture0);
			//Logging.Debug("Set active texture.");
			GlUtility.BindTexture(_backgroundTexture);

			//Logging.Debug("Bound background texture.");

			_backgroundProgram["projection_matrix"].SetValue(fov);
			_backgroundProgram["view_matrix"].SetValue(cameraView);

			//Logging.Debug("Bound background uniforms.");

			GlUtility.BindBuffer(_points);

			//Logging.Debug("Bound background points.");

			//vertexPosition
			GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, _points.PointerType, false, 5 * Marshal.SizeOf(typeof(float)), IntPtr.Zero);
			GL.EnableVertexAttribArray(ShaderProgram.VertexPosition);

			//Logging.Debug("Bound background positions to array.");

			GL.DisableVertexAttribArray(ShaderProgram.VertexColor);

			//Logging.Debug("Disabled VertexColor array.");

			GL.DisableVertexAttribArray(ShaderProgram.VertexSize);

			//textureCoords
			GL.VertexAttribPointer(ShaderProgram.TextureCoords, 2, _points.PointerType, false, 5 * Marshal.SizeOf(typeof(float)), Vector3.SizeInBytes);
			GL.EnableVertexAttribArray(ShaderProgram.TextureCoords);

			//Logging.Debug("Bound background texture coords to array.");

			GlUtility.BindBuffer(_backgroundElements);
			GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

			//Logging.Debug("Exit background draw.");
		}

		private void InitializeBackground()
		{
			if (string.IsNullOrEmpty(_data.BackgroundFileName))
			{
				HasBackground = false;
				return;
			}
			var file = Path.Combine(VixenPreviewDescriptor.ModulePath, _data.BackgroundFileName);
			var i = File.Exists(file) ? new Bitmap(Image.FromFile(file)) : null;

			if (i == null)
			{
				HasBackground = false;
				return;
			}

			Height = i.Height;
			Width = i.Width;

			_backgroundProgram = new ShaderProgram(VertexTextureShader, FragmentTextureShader);

			_backgroundProgram.Use();

			var log = _backgroundProgram.ProgramLog;
			if (!string.IsNullOrWhiteSpace(log))
			{
				Logging.Info("Texture program log: {0}", log);
			}

			_backgroundProgram["model_matrix"].SetValue(   Matrix4.Identity);
			SetBackgroundBrightness(_data.BackgroundAlpha / 255f);

			_backgroundTexture = new Texture(i);

			_points = new VBO<float>(new[] {

				//Positions        /// Tex Coord
				Width, Height, -.1f,     1f, 1f,         //Top Right
				Width, 0f, -.1f,      1f, 0f,		//Bottom Right
				0f, 0f, -.1f,      0f, 0f,		//Bottom Left
				0f, Height, -.1f,     0f, 1f			//Top Left
			});

			//Form two triangles
			_backgroundElements = new VBO<int>(new [] { 0, 1, 3, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);

			HasBackground = true;
		}

		private void SetBackgroundBrightness(float value)
		{
			_backgroundProgram["ambientStrength"].SetValue(value);
		}



		public static string VertexTextureShader = @"
#version 330

in vec3 vertexPosition;
in vec2 textureCoord;

out vec2 texCoord;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    texCoord = textureCoord;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";


		public static string FragmentTextureShader = @"
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


//		public static string FragmentTextureShader = @"
//#version 130

//uniform sampler2D texture;
//uniform float ambientStrength;
//const vec3 lightColor = vec3(1,1,1);

//in vec2 uv;

//out vec4 fragment;

//void main(void)
//{
//   	vec4 color = texture2D(texture, uv);
	
//	vec3 ambient = ambientStrength * lightColor;

//    vec3 result = ambient * color;
//    fragment = vec4(result, 1.0f);
//}
//";

		public void Dispose()
		{
			if (_backgroundTexture != null) _backgroundTexture.Dispose();
			if (_points != null) _points.Dispose();
			//if (_backgroundUv != null) _backgroundUv.Dispose();
			if (_backgroundElements != null) _backgroundElements.Dispose();
			if (_backgroundProgram != null)
			{
				_backgroundProgram.DisposeChildren = true;
				_backgroundProgram.Dispose();
			}
		}
	}
}
