using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;

namespace VixenModules.Preview.VixenPreview.OpenGL
{

	public partial class OpenGlPreviewForm : Form, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private static int _width = 1280, _height = 720;

		private static ShaderProgram _program;
		
		private Background _background;
		private const float Fov = .45f;
		private const float FarDistance = 7000f;
		private const float NearDistance = 1f;
		private bool _mouseDown;
		private int _prevX, _prevY;
		private Camera _camera;
		private bool _needsUpdate = true;

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
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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
				ClientSize = new Size(_background.Width, _background.Height);
				_width = _background.Width;
				_height = _background.Height;
			}
			else
			{
				ClientSize = new Size(_width, _height);
			}

			// create our camera
			_camera = new Camera(new Vector3(ClientSize.Width / 2f, ClientSize.Height / 2f, ClientSize.Width), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));

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
			if (VixenSystem.Elements.ElementsHaveState)
			{
				OnRenderFrame();
				_needsUpdate = true;
			}
			else
			{
				if (_needsUpdate)
				{
					OnRenderFrame();
					_needsUpdate = false;
				}
			}
		}

		#endregion

		private void glControl_Resize(object sender, EventArgs e)
		{
			if (glControl.ClientSize.Height == 0)
				glControl.ClientSize = new Size(glControl.ClientSize.Width, 100);

			_width = glControl.ClientSize.Width;
			_height = glControl.ClientSize.Height;
			glControl.Invalidate();
		}

		private void glControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.I || e.KeyCode == Keys.O)
			{
				int direction = 3;
				if (e.KeyCode != Keys.O)
				{
					direction *= -5;
				}

				var distance = _camera.Position.Z + direction;
				if (distance > FarDistance || distance < NearDistance)
				{
					return;
				}

				_camera.MoveRelative(new Vector3(0, 0, direction));
				glControl.Invalidate();
			}

			
		}

		private void GlControl_MouseWheel(object sender, MouseEventArgs e)
		{
			int factor = 30;
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				factor = 10;
			}
			int direction = e.Delta * SystemInformation.MouseWheelScrollLines / factor;

			var distance = _camera.Position.Z + direction;
			Console.Out.WriteLine(distance);
			if (distance > FarDistance || distance < NearDistance)
			{
				return;
			}

			var moveFactor = Math.Abs(distance) / _camera.Position.Z;
			if (direction < 0)
			{
				moveFactor *= -1;
			}
			
			_camera.MoveRelative(new Vector3(0,0, direction));
			//_camera.Move(new Vector3(moveFactor, moveFactor, 0f));

			glControl.Invalidate();
		}


		private void glControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X == _prevX && e.Y == _prevY) return;

			// move the camera when the mouse is down
			if (_mouseDown && (e.Button == MouseButtons.Right))
			{
				float yaw = (_prevX - e.X) * 0.05f;
				_camera.Yaw(yaw);

				float pitch = (_prevY - e.Y) * 0.05f;
				_camera.Pitch(pitch);

				_prevX = e.X;
				_prevY = e.Y;

				glControl.Invalidate();
			}
			else if(_mouseDown && e.Button == MouseButtons.Left)
			{
				
				var moveFactor = (_camera.Position.Z / FarDistance) * 3;
				float yaw = (_prevX - e.X) * moveFactor;
				float pitch = (_prevY - e.Y) * moveFactor;
				_camera.Move(new Vector3(yaw, -pitch, 0f));
				
				_prevX = e.X;
				_prevY = e.Y;

				glControl.Invalidate();
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
			var perspective = Matrix4.CreatePerspectiveFieldOfView(Fov, (float)width / height, 1f, 7500f);
			return perspective;
		}


		private void OnRenderFrame()
		{
			var perspective = CreatePerspective();
			
			glControl.MakeCurrent();
			ClearScreen();
			

			if (VixenSystem.Elements.ElementsHaveState)
			{
				var mvp = Matrix4.Identity * _camera.ViewMatrix * perspective;
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
			//_program["cameraPosition"].SetValue(_camera.Position);
		
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				dataDisplayItem.Shape.Draw(_program, _background.Height);
			}
		}

		public static string VertexShader = @"
		#version 130

		in vec3 vertexPosition;
		in vec4 vertexColor;
		
		out vec4 color;
		out float pSize;

		uniform float pointSize;
		uniform mat4 mvp;
		
		void main(void)
		{
			color = vertexColor;
			gl_PointSize = pointSize;
			pSize = pointSize; //pass through
			
			gl_Position = mvp * vec4(vertexPosition, 1);
		}
		";
		
		public static string FragmentShader = @"
		#version 330

		in vec4 color;
		in float pSize;
		out vec4 gl_FragColor;

		void main(void)
		{
			if(pSize > 1) //We only need to round points that are bigger than 1
			{
				vec2 circCoord = 2.0 * gl_PointCoord - 1.0;
				if (dot(circCoord, circCoord) > 1)
				{
					discard;
				}
			}

			gl_FragColor = color;

		}
		";

	}
}
