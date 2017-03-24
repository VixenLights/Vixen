using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Common.Controls.Scaling;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Vertex;

namespace VixenModules.Preview.VixenPreview.OpenGL
{

	public partial class OpenGlPreviewForm : Form, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private static int _width = 1280, _height = 720;

		private static ShaderProgram _program;
		
		private VBO<float> _points;
		private Background _background;
		private const float Fov = .45f;
		private const float FarDistance = 100f;
		private const float NearDistance = .1f;
		private bool _mouseDown;
		private int _prevX, _prevY;
		private Camera _camera;

		public OpenGlPreviewForm(VixenPreviewData data)
		{
			Data = data;
			InitializeComponent();
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
			_program.DisposeChildren = true;
			_program.Dispose();

			_background.Dispose();
			
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
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.ProgramPointSize);
		}

		private void Initialize()
		{

			EnableFeatures();

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			// compile the shader program
			_program = new ShaderProgram(VertexShader, FragmentShader);

			//InitializeBackground;
			_background = new Background(Data);
			if (_background.HasBackground)
			{
				Width = _background.Width;
				Height = _background.Height;
			}
			else
			{
				Width = _width;
				Height = _height;
			}

			// create our camera
			_camera = new Camera(new Vector3(0, 0, 50), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));

			_points = new VBO<float>(new [] {

				//Positions        /// Colors
				-1f, 1f, 0f,     1f, 0f, 0f,
				1f, 1f, 0f,      0f, 1f, 0f,
				1f, -1f, 0f,      0f, 0f, 1f,
				-1f, -1f, 0f,     1f, 1f, 1f
			});
			
			Logging.Info("OpenGL v {0}", GL.GetString(StringName.Version));
			Logging.Info("Vendor {0}, Renderer {1}", GL.GetString(StringName.Vendor), GL.GetString(StringName.Renderer));
			Logging.Info("Extensions {0}", GL.GetString(StringName.Extensions));
			glControl_Resize(this, EventArgs.Empty);
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
			OnRenderFrame();
		}

		#endregion

		private void glControl_Resize(object sender, EventArgs e)
		{
			if (glControl.ClientSize.Height == 0)
				glControl.ClientSize = new Size(glControl.ClientSize.Width, 1);

			_width = glControl.ClientSize.Width;
			_height = glControl.ClientSize.Height;
		}

		private void GlControl_MouseWheel(object sender, MouseEventArgs e)
		{
			int direction = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

			var distance = _camera.Position.Z + direction;
			if (distance > 100 || distance < NearDistance)
			{
				return;
			}

			_camera.Move(new Vector3(0,0,direction));
		}


		private void glControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X == _prevX && e.Y == _prevY) return;

			// move the camera when the mouse is down
			if (_mouseDown && (e.Button == MouseButtons.Right))
			{
				float yaw = (_prevX - e.X) * 0.003f;
				_camera.Yaw(yaw);

				float pitch = (_prevY - e.Y) * 0.003f;
				_camera.Pitch(pitch);

				_prevX = e.X;
				_prevY = e.Y;
			}
			else if(_mouseDown && e.Button == MouseButtons.Left)
			{
				float yaw = (_prevX - e.X) * 0.02f;
				float pitch = (_prevY - e.Y) * 0.02f;
				_camera.Move(new Vector3(yaw, -pitch, 0f));
				
				_prevX = e.X;
				_prevY = e.Y;
			}
		}

		private void glControl_MouseDown(object sender, MouseEventArgs e)
		{
			//if (e.Button != MouseButtons.Right) return;

			// this method gets called whenever a new mouse button event happens
			_mouseDown = true;

			// if the mouse has just been clicked then we hide the cursor and store the position
			if (_mouseDown)
			{
				Cursor = Cursors.Hand;
				_prevX = e.X;
				_prevY = e.Y;
			}
		}

		private void glControl_MouseUp(object sender, MouseEventArgs e)
		{
			Cursor = DefaultCursor;
			_mouseDown = false;
		}

		private void PreviewWindow_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			Close();
		}

		private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			OnRenderFrame();
		}

		private Matrix4 CreatePerspective()
		{
			return CreatePerspective(_width, _height);
		}

		private Matrix4 CreatePerspective(int width, int height)
		{
			var perspective = Matrix4.CreatePerspectiveFieldOfView(Fov, (float)width / height, NearDistance, FarDistance);
			return perspective;
		}


		private void OnRenderFrame()
		{
			var perspective = CreatePerspective();
			
			glControl.MakeCurrent();
			ClearScreen();

			if (!VixenSystem.Elements.ElementsHaveState)
			{
				float ratio = (float)Height / Width;
				var modelMatrix = Matrix4.CreateScale(new Vector3(15f, 15f * ratio, 0));
				var mvp = modelMatrix * _camera.ViewMatrix * perspective;
				DrawPoints(mvp);
			}

			_background.Draw(perspective, _camera.ViewMatrix);
			glControl.SwapBuffers();
			glControl.Context.MakeCurrent(null);

		}

		private static void ClearScreen()
		{
			// set up the OpenGL viewport and clear both the color and depth bits
			GL.Viewport(0, 0, _width, _height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		private void DrawPoints(Matrix4 mvp)
		{
			// use our shader program
			_program.Use();

			_program["mvp"].SetValue(mvp);
			_program["cameraPosition"].SetValue(_camera.Position);
			_program["pointSize"].SetValue(8f);

			//GlUtility.BindBuffer(_points);
			//GL.VertexAttribPointer(ShaderProgram.VertexPosition, 3, VertexAttribPointerType.Float, false, 6* Marshal.SizeOf(typeof(float)), IntPtr.Zero);
			//GL.EnableVertexAttribArray(0);

			//GL.VertexAttribPointer(ShaderProgram.VertexColor, 3, VertexAttribPointerType.Float, true, 6* Marshal.SizeOf(typeof(float)), Vector3.SizeInBytes);
			//GL.EnableVertexAttribArray(1);

			//// draw the points
			//GL.DrawArrays(PrimitiveType.Points, 0, 4);
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				dataDisplayItem.Shape.Draw(_program);
			}
		}

		public static string VertexShader = @"
		#version 130

		in vec3 vertexPosition;
		in vec3 vertexColor;
		
		out vec3 color;

		uniform float pointSize;
		uniform mat4 mvp;
		uniform vec3 cameraPosition; 

		const float minPointScale = 0.1;
		const float maxPointScale = 1;
		const float maxDistance   = 100.0;

		void main(void)
		{
			color = vertexColor;
			
			gl_Position = mvp * vec4(vertexPosition, 1);

			// Calculate point scale based on distance from the viewer
			// to compensate for the fact that gl_PointSize is the point
			// size in rasterized points / pixels.
			float cameraDist = distance(vertexPosition.xyz, cameraPosition);
			float pointScale = 1 - (cameraDist / maxDistance);
			pointScale = max(pointScale, minPointScale);
			//pointScale = min(pointScale, maxPointScale);

			gl_PointSize = pointSize * pointScale;
		}
		";

		//public static string VertexShader = @"
		//#version 130

		//in vec3 vertexPosition;
		//in vec3 vertexColor;
		//in float pointSize;
		//out vec3 color;

		//uniform mat4 mvp;
		//uniform vec3 cameraPosition; 

		//const float minPointScale = 0.1;
		//const float maxPointScale = 1;
		//const float maxDistance   = 100.0;

		//void main(void)
		//{
		//	color = vertexColor;
			
		//	gl_Position = mvp * vec4(vertexPosition, 1);

		//	// Calculate point scale based on distance from the viewer
		//	// to compensate for the fact that gl_PointSize is the point
		//	// size in rasterized points / pixels.
		//	float cameraDist = distance(vertexPosition.xyz, cameraPosition);
		//	float pointScale = 1.0 - (cameraDist / maxDistance);
		//	pointScale = max(pointScale, minPointScale);
		//	pointScale = min(pointScale, maxPointScale);

		//	gl_PointSize = pointSize * pointScale;
		//}
		//";

		public static string FragmentShader = @"
		#version 330

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

	}
}
