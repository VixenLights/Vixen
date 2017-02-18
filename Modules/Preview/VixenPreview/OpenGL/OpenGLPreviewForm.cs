using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTKDemo.Constructs;
using OpenTKDemo.Constructs.Shaders;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.OpenGL
{

	public partial class OpenGLPreviewForm : Form, IDisplayForm
	{

		private static int _width = 1280, _height = 720;

		private static ShaderProgram _program;
		private static ShaderProgram _backgroundProgram;

		private VBO<Vector3> _points, _background;
		private VBO<Vector3> _pointsColor;
		private VBO<Vector2> _backgroundUv;
		private VBO<int> _backgroundElements;
		private VBO<float> _pointsSizes;
		private float _fov = .45f;
		private bool left, right, up, down, space;
		private bool _mouseDown = false;
		private int downX, downY;
		private int _prevX, _prevY;

		private Texture _backgroundTexture;

		private bool _hasBackground;

		private Stopwatch _watch;
		private Camera _camera;

		public OpenGLPreviewForm(VixenPreviewData data)
		{
			Data = data;
			InitializeComponent();
			Width = _width;
			Height = _height;
			glControl.MouseWheel += GlControl_MouseWheel;
		}

		private const int CP_NOCLOSE_BUTTON = 0x200;
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams myCp = base.CreateParams;
				myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
				return myCp;
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{

			glControl.MouseWheel -= GlControl_MouseWheel;
			_points.Dispose();
			_pointsColor.Dispose();
			_pointsSizes.Dispose();
			_program.DisposeChildren = true;
			_program.Dispose();

			_background.Dispose();
			_backgroundUv.Dispose();
			_backgroundElements.Dispose();
			_backgroundTexture.Dispose();
			_backgroundProgram.DisposeChildren = true;
			_backgroundProgram.Dispose();

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override void OnLoad(EventArgs e)
		{
			Initialize();
		}

		private void EnableFeatures()
		{
			// enable depth testing to ensure correct z-ordering of our fragments
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.PointSprite);
			GL.Enable(EnableCap.ProgramPointSize);
		}

		private void Initialize()
		{

			EnableFeatures();

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			// compile the shader program
			_program = new ShaderProgram(VertexShader, FragmentShader);
			_backgroundProgram = new ShaderProgram(VertexTextureShader, FragmentTextureShader);
			InitializeBackground();

			// create our camera
			_camera = new Camera(new Vector3(0, 0, 50), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));

			// set the view and projection matrix, which are static throughout this tutorial
			_program.Use();
			_program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(_fov, (float)_width / _height, 0.1f, 1000f));
			_program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));

			// program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

			_points = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) });
			_pointsColor = new VBO<Vector3>(new Vector3[] { new Vector3(1f, 0, 0), new Vector3(0, 1f, 0), new Vector3(0, 0, 1f), new Vector3(1f, 1f, 1f) });
			_pointsSizes = new VBO<float>(new[] { 2f, 4f, 8f, 16f }, BufferTarget.ArrayBuffer);
			
			_watch = Stopwatch.StartNew();

			Text += string.Format(" OpenGL v {0}", GL.GetString(StringName.Version));

			Console.Out.WriteLine(GL.GetString(StringName.ShadingLanguageVersion));

			glControl_Resize(this, EventArgs.Empty);
		}

		private void InitializeBackground()
		{
			if (string.IsNullOrEmpty(Data.BackgroundFileName))
			{
				_hasBackground = false;
				return;
			}
			var file = Path.Combine(VixenPreviewDescriptor.ModulePath, Data.BackgroundFileName);
			var i = File.Exists(file) ? new Bitmap(Image.FromFile(file)) : null;

			if (i == null)
			{
				_hasBackground = false;
				return;
			}

			Height = _height = i.Height;
			Width = _width = i.Width;

			_backgroundProgram.Use();
			_backgroundProgram["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(_fov, (float)_width / _height, 0.1f, 1000f));
			float ratio = (float)_height / _width;
			_backgroundProgram["model_matrix"].SetValue(Matrix4.CreateScale(new Vector3(15f, 15f * ratio, 0f)));
			SetBackgroundBrightness(Data.BackgroundAlpha/255f);

			_backgroundTexture = new Texture(i, false);

			_backgroundUv = new VBO<Vector2>(new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)});

			_background = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, -100), new Vector3(1, 1, -100), new Vector3(1, -1, -100), new Vector3(-1, -1, -100) });
			_backgroundElements = new VBO<int>(new int[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);

			_hasBackground = true;
		}

		private static void SetBackgroundBrightness(float value)
		{
			_backgroundProgram["ambientStrength"].SetValue(value);
		}

		#region IDisplayForm

		public VixenPreviewData Data { get; set; }
		public void Setup()
		{

		}

		public void Close()
		{
			
		}

		public void UpdatePreview()
		{
			if (!VixenSystem.Elements.ElementsHaveState)
			{
				glControl.MakeCurrent();
				ClearScreen();
				if (_hasBackground)
				{
					DrawBackground();
				}
				glControl.SwapBuffers();
				glControl.Context.MakeCurrent(null);
				return;
			}

			OnRenderFrame();
		}

		#endregion

		private void glControl_Resize(object sender, EventArgs e)
		{
			if (glControl.ClientSize.Height == 0)
				glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);

			int w = glControl.ClientSize.Width;
			int h = glControl.ClientSize.Height;

			glControl.MakeCurrent();

			_width = w;
			_height = h;

			var perspective = CreatePerspective(_width, _height);

			_program.Use();
			_program["projection_matrix"].SetValue(perspective);

			if (_hasBackground)
			{
				_backgroundProgram.Use();
				_backgroundProgram["projection_matrix"].SetValue(perspective);
			}
		}
		private void GlControl_MouseWheel(object sender, MouseEventArgs e)
		{
			int direction = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

			_fov += direction * .01f;
			if (_fov <= 0.0f)
			{
				_fov = 0.001f;
			}
			else if (_fov > Math.PI)
			{
				_fov = 3.14f;
			}

			var perspective = CreatePerspective(_width, _height);

			_program.Use();
			_program["projection_matrix"].SetValue(perspective);

			if (_hasBackground)
			{
				_backgroundProgram.Use();
				_backgroundProgram["projection_matrix"].SetValue(perspective);
			}

			Console.Out.WriteLine("FOV = {0}", _fov);
		}


		private void glControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.X == _prevX && e.Y == _prevY) return;

			// move the camera when the mouse is down
			if (_mouseDown)
			{
				float yaw = (_prevX - e.X) * 0.002f;
				_camera.Yaw(yaw);

				float pitch = (_prevY - e.Y) * 0.002f;
				_camera.Pitch(pitch);

				_prevX = e.X;
				_prevY = e.Y;
			}
		}

		private void glControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right) return;

			// this method gets called whenever a new mouse button event happens
			_mouseDown = true;

			// if the mouse has just been clicked then we hide the cursor and store the position
			if (_mouseDown)
			{
				Cursor = Cursors.Hand;
				_prevX = downX = e.X;
				_prevY = downY = e.Y;
			}
		}

		private void glControl_MouseUp(object sender, MouseEventArgs e)
		{
			Cursor = DefaultCursor;
		}

		private void PreviewWindow_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			Close();
		}

		private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{

			OnRenderFrame();

		}

		private Matrix4 CreatePerspective(int width, int height)
		{
			var perspective = Matrix4.CreatePerspectiveFieldOfView(_fov, (float)width / height, 0.1f, 1000f);
			return perspective;
		}


		private void OnRenderFrame()
		{
			glControl.MakeCurrent();
			_watch.Stop();

			_watch.Restart();

			ClearScreen();

			// use our shader program
			_program.Use();

			// apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
			_program["view_matrix"].SetValue(_camera.ViewMatrix);

			//Bind up the point data
			GlUtility.BindBufferToShaderAttribute(_points, _program, "vertexPosition");
			GlUtility.BindBufferToShaderAttribute(_pointsColor, _program, "vertexColor");
			GlUtility.BindBufferToShaderAttribute(_pointsSizes, _program, "pointSize");
			
			// draw the points
			GL.DrawArrays(PrimitiveType.Points, 0,4);
			if (_hasBackground)
			{
				DrawBackground();
			}

			glControl.SwapBuffers();

		}

		private static void ClearScreen()
		{
// set up the OpenGL viewport and clear both the color and depth bits
			GL.Viewport(0, 0, _width, _height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		private void DrawBackground()
		{
			_backgroundProgram.Use();
			GL.ActiveTexture(TextureUnit.Texture0);
			GlUtility.BindTexture(_backgroundTexture);

			_backgroundProgram["view_matrix"].SetValue(_camera.ViewMatrix);

			GlUtility.BindBufferToShaderAttribute(_background, _backgroundProgram, "vertexPosition");
			GlUtility.BindBufferToShaderAttribute(_backgroundUv, _backgroundProgram, "vertexUV");
			GlUtility.BindBuffer(_backgroundElements);
			GL.DrawElements(PrimitiveType.Quads, _backgroundElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
		}

		public static string VertexShader = @"
		#version 130

		in vec3 vertexPosition;
		in vec3 vertexColor;
		in float pointSize;
		out vec3 color;

		uniform mat4 projection_matrix;
		uniform mat4 view_matrix;
		uniform mat4 model_matrix;

		void main(void)
		{
			gl_PointSize = pointSize;
		    color = vertexColor;
		    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
		}
		";

		public static string FragmentShader = @"
		#version 130

		in vec3 color;
		out vec4 gl_FragColor;

		void main(void)
		{

		    vec2 circCoord = 2.0 * gl_PointCoord - 1.0;
			if (dot(circCoord, circCoord) > 1)
			{
				discard;
			}

			gl_FragColor = vec4(color, 1);

		}
		";


		public static string VertexTextureShader = @"
#version 130

in vec3 vertexPosition;
in vec2 vertexUV;

out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void)
{
    uv = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

		public static string FragmentTextureShader = @"
#version 130

uniform sampler2D texture;
uniform float ambientStrength;
const vec3 lightColor = vec3(1,1,1);

in vec2 uv;

out vec4 fragment;

void main(void)
{
   	vec4 color = texture2D(texture, uv);
	
	vec3 ambient = ambientStrength * lightColor;

    vec3 result = ambient * color;
    fragment = vec4(result, 1.0f);
}
";

	}
}
