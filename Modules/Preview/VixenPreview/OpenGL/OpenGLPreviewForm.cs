using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Catel.Collections;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Vixen;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.OpenGL.Constructs.Shaders;

namespace VixenModules.Preview.VixenPreview.OpenGL
{

	public partial class OpenGlPreviewForm : Form, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private readonly MillisecondsValue _backgroundDraw;
		private readonly MillisecondsValue _pointsUpdate;
		private readonly MillisecondsValue _pointsDraw;
		private readonly MillisecondsValue _previewUpdate;
		private readonly Stopwatch _sw = Stopwatch.StartNew();
		private readonly Stopwatch _sw2 = Stopwatch.StartNew();
		private readonly Stopwatch _frameRateTimer = Stopwatch.StartNew();

		private int _width = 800, _height = 600;
		private float _focalDepth = 0;
		private float _aspectRatio;

		private ShaderProgram _program;
		
		private Background _background;
		private const double Fov = 45.0;
		private const float FarDistance = 4000f;
		private const float NearDistance = 1f;
		private bool _mouseDown;
		private int _prevX, _prevY;
		private Camera _camera;
		private bool _needsUpdate = true;
		private bool _isRendering;
		private bool _formLoading;
		private string _displayName = "Vixen Preview";

		private readonly ContextMenuStrip _contextMenuStrip = new ContextMenuStrip();
		private bool _showStatus;
		private bool _alwaysOnTop;
		private bool _enableLightScaling = true;

		private float _pointScaleFactor;
		private long _frameCount;

		private string _programLog = string.Empty;

		internal static readonly Object ContextLock = new Object();

		private readonly ParallelOptions _parallelOptions = new ParallelOptions()
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		};

		public OpenGlPreviewForm(VixenPreviewData data, Guid instanceId)
		{
			_formLoading = true;
			Icon = Resources.Icon_Vixen3;
			Data = data;
			InstanceId = instanceId;
			InitializeComponent();
			double scaleFactor = ScalingTools.GetScaleFactor();
			_contextMenuStrip.Renderer = new ThemeToolStripRenderer();
			int imageSize = (int)(16 * scaleFactor);
			_contextMenuStrip.ImageScalingSize = new Size(imageSize, imageSize);
			_backgroundDraw = new MillisecondsValue("OpenGL preview background draw");
			_pointsUpdate = new MillisecondsValue("OpenGL preview points update");
			_pointsDraw = new MillisecondsValue("OpenGL preview points draw");
			_previewUpdate = new MillisecondsValue("OpenGL preview update");
			VixenSystem.Instrumentation.AddValue(_backgroundDraw);
			VixenSystem.Instrumentation.AddValue(_pointsUpdate);
			VixenSystem.Instrumentation.AddValue(_pointsDraw);
			VixenSystem.Instrumentation.AddValue(_previewUpdate);
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

		private void ConfigureAlwaysOnTop()
		{
			TopMost = _alwaysOnTop;
		}

		private void ConfigureStatusBar()
		{
			statusStrip.Visible = _showStatus;
		}

		private void HandleContextMenu()
		{
			_contextMenuStrip.Items.Clear();

			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());

			var item = new ToolStripMenuItem("Show Status");
			item.ToolTipText = @"Enable/Disable the preview status bar.";

			if (_showStatus)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_showStatus = !_showStatus;
				ConfigureStatusBar();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			_contextMenuStrip.Items.Add(item);

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Always On Top");
			item.ToolTipText = @"Enable/Disable the window always on top.";

			if (_alwaysOnTop)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_alwaysOnTop = !_alwaysOnTop;
				if (_alwaysOnTop)
				{
					IsOnTopWhenPlaying = false;
				}
				ConfigureAlwaysOnTop();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Auto On Top");
			item.ToolTipText = @"Enable/Disable bringing this preview on top and back automatically.";

			if (IsOnTopWhenPlaying)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				IsOnTopWhenPlaying = !IsOnTopWhenPlaying;
				if (IsOnTopWhenPlaying)
				{
					_alwaysOnTop = false;
					ConfigureAlwaysOnTop();
				}
				else if (TopMost)
				{
					TopMost = false;
				}

				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Reset Size");
			item.ToolTipText = @"Resets the viewable size to match the background size.";
			item.Enabled = _background.HasBackground;
			item.Click += (sender, args) =>
			{
				_width = _background.Width;
				_height = _background.Height;
				ClientSize = new Size(_width, _height);
				_focalDepth = (float)(1 / Math.Tan(ConvertToRadians(Fov / 2)) * (ClientSize.Height / 2.0));
				_camera.Position = new Vector3(_width / 2f, _height / 2f, _focalDepth);
				_camera.SetDirection(new Vector3(0, 0, -1));
				CalculatePointScaleFactor();
				glControl.Invalidate();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Enable Light Scaling");
			item.ToolTipText = @"Scales the light size as the camera is zoomed in or out.";
			item.Checked = _enableLightScaling;
			item.Click += (sender, args) =>
			{
				_enableLightScaling = !_enableLightScaling;
				CalculatePointScaleFactor();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			var seperator = new ToolStripSeparator();
			_contextMenuStrip.Items.Add(seperator);

			var locationLabel = new ToolStripLabel(string.Format("Location: {0},{1}", DesktopLocation.X, DesktopLocation.Y));
			_contextMenuStrip.Items.Add(locationLabel);

			var sizeLabel = new ToolStripLabel(string.Format("Size: {0} X {1}", ClientSize.Width, ClientSize.Height));
			_contextMenuStrip.Items.Add(sizeLabel);

			_contextMenuStrip.Show(MousePosition);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && glControl != null)
			{
				glControl.MouseWheel -= GlControl_MouseWheel;
				lock (ContextLock)
				{
					glControl.MakeCurrent();
					if (_program != null)
					{
						_program.DisposeChildren = true;
						_program.Dispose();
						_background.Dispose();
					}
					glControl.Context.MakeCurrent(null);
				}

				glControl.Dispose();
			}
			

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
			//GL.Enable(EnableCap.DepthTest);
			lock (ContextLock)
			{
#if DEBUG
				//Logging.Info("Debug Output");
				GL.Enable(EnableCap.DebugOutput);
				//Logging.Info("Debug Output Sync");
				GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
				//Logging.Info("Blend");
				GL.Enable(EnableCap.Blend);
				//Logging.Info("Blend Func");
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
				//Logging.Info("Point Sprite");
				GL.Enable(EnableCap.PointSprite);
				//Logging.Info("Vertex Point Sprite");
				GL.Enable(EnableCap.VertexProgramPointSize);;
				//Logging.Info("Program Point");
				GL.Enable(EnableCap.ProgramPointSize);
				//Logging.Info("Cull Face");
				GL.Disable(EnableCap.CullFace);
			}
			
		}

		private void Initialize()
		{
			_formLoading = true;
			
			EnableFeatures();

			// compile the shader program
			_program = new ShaderProgram(VertexShader, FragmentShader);

			_background = new Background(Data);
			
			RestoreWindowState();

			_aspectRatio = (float)_width / _height;
			
			_focalDepth = (float)( 1 / Math.Tan(ConvertToRadians(Fov / 2)) * (ClientSize.Height / 2.0));
			
			CreateCamera();
			
			Logging.Info("OpenGL v {0}", GL.GetString(StringName.Version));
			Logging.Info("Vendor {0}, Renderer {1}", GL.GetString(StringName.Vendor), GL.GetString(StringName.Renderer));
			Logging.Info("Shading language version {0}", GL.GetString(StringName.ShadingLanguageVersion));
			Logging.Info("Extensions {0}", GL.GetString(StringName.Extensions));
			_programLog = _program.ProgramLog;
			_programLog += _program.VertexShader.ShaderLog;
			_programLog += _program.FragmentShader.ShaderLog;
			if (!string.IsNullOrEmpty(_programLog))
			{
				Logging.Error($"Shader log output: {_programLog}");
			}

			glControl_Resize(this, EventArgs.Empty);

			CalculatePointScaleFactor();
			_formLoading = false;
		}



		#region IDisplayForm

		public VixenPreviewData Data { get; set; }
		public void Setup()
		{
			var pixelCount = 0;
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				dataDisplayItem.Shape.Layout();
				pixelCount += dataDisplayItem.Shape.UpdatePixelCache();
			}
			toolStripStatusPixels.Text = pixelCount.ToString();
		}
		
		public void UpdatePreview()
		{
			if (_formLoading) return;
			if (VixenSystem.Elements.ElementsHaveState)
			{
				OnRenderFrame();
				_needsUpdate = true;
				toolStripStatusFPS.Text = FrameRate.ToString();
			}
			else
			{
				if (_needsUpdate)
				{
					OnRenderFrame();
					_needsUpdate = false;
				}
				toolStripStatusFPS.Text = @"0";
			}
		}

		private void UpdateFrameRate()
		{
			_frameCount++;

			if (_frameRateTimer.ElapsedMilliseconds > 999)
			{
				FrameRate = _frameCount;
				_frameCount = 0;
				_frameRateTimer.Restart();
			}
		}

		public long FrameRate { get; private set; }

		public string DisplayName
		{
			get { return _displayName; }
			set
			{
				_displayName = value;
				if (InvokeRequired)
				{
					Invoke(new Delegates.GenericDelegate(UpdateDisplayName));
				}
				else
				{
					UpdateDisplayName();
				}

			}
		}

		/// <inheritdoc />
		public Guid InstanceId { get; set; }

		/// <inheritdoc />
		public bool IsOnTopWhenPlaying { get; private set; }

		public void UpdateDisplayName()
		{
			Text = _displayName;
		}

		#endregion

		private void glControl_Resize(object sender, EventArgs e)
		{
			if (glControl.ClientSize.Height == 0)
				glControl.ClientSize = new Size(glControl.ClientSize.Width, 100);

			_width = glControl.ClientSize.Width;
			_height = glControl.ClientSize.Height;
			_aspectRatio = (float)_width / _height;
			if (!_formLoading)
			{
				SaveWindowState();
			}
			glControl.Invalidate();
		}

		private void glControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.I || e.KeyCode == Keys.O)
			{
				int factor = 20;
				if ((ModifierKeys & Keys.Shift) == Keys.Shift)
				{
					factor = 5;
				}

				int delta = 120;
				if (e.KeyCode != Keys.I)
				{
					delta = -delta;
				}

				int direction = -(delta * SystemInformation.MouseWheelScrollLines / factor);

				Zoom(direction);
			}
		}

		private void GlControl_MouseWheel(object sender, MouseEventArgs e)
		{
			int factor = 20;
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				factor = 5;
			}
			int direction = -(e.Delta * SystemInformation.MouseWheelScrollLines / factor);

			Zoom(direction);
		}

		private void Zoom(int direction)
		{
			var distance = _camera.Position.Z + direction;

			if (distance > FarDistance || distance < NearDistance)
			{
				return;
			}

			_camera.Move(new Vector3(0, 0, direction));
			CalculatePointScaleFactor();
			glControl.Invalidate();
			SaveWindowState();
		}

		private void UpdateStatusDistance(float distance)
		{
			toolStripStatusDistance.Text = distance.ToString("0.0");
		}


		private void glControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X == _prevX && e.Y == _prevY) return;

			// move the camera when the mouse is down
			if(_mouseDown && e.Button == MouseButtons.Left)
			{
				
				var moveFactor = (_camera.Position.Z / FarDistance) * 7;
				float yaw = (_prevX - e.X) * moveFactor;
				float pitch = (_prevY - e.Y) * moveFactor;
				_camera.Move(new Vector3(yaw, -pitch, 0f));
				
				_prevX = e.X;
				_prevY = e.Y;

				CalculatePointScaleFactor();
				glControl.Invalidate();
			}
		}

		private void glControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				HandleContextMenu();
				return;
			}

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
			SaveWindowState();
		}

		private void OpenGlPreviewForm_Move(object sender, EventArgs e)
		{
			if (!_formLoading) SaveWindowState();
		}

		private void PreviewWindow_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				var messageBox = new MessageBoxForm("The preview can only be closed from the Preview Configuration dialog.", "Close", MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.ShowDialog();
				e.Cancel = true;
			}
			else
			{
				glControl.MouseWheel -= GlControl_MouseWheel;
				Close();
			}
		}

		private void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			OnRenderFrame();
		}

		private Matrix4 CreatePerspective()
		{
			var perspective = Matrix4.CreatePerspectiveFieldOfView((float)ConvertToRadians(Fov), _aspectRatio>0?_aspectRatio:1, 1f, 7500f);
			return perspective;
		}


		private void OnRenderFrame()
		{
			//Logging.Debug("Entering RenderFrame");
			if (_isRendering || _formLoading || WindowState==FormWindowState.Minimized) return;
			UpdateStatusDistance(_camera.Position.Z);
			_isRendering = true;
			_sw.Restart();
			var perspective = CreatePerspective();
			
			if (VixenSystem.Elements.ElementsHaveState)
			{
				//Logging.Debug("Elements have state.");
				_sw2.Restart();
				UpdateShapePoints();
				_pointsUpdate.Set(_sw2.ElapsedMilliseconds);
				var mvp = Matrix4.Identity * _camera.ViewMatrix * perspective;
				lock (ContextLock)
				{
					glControl.MakeCurrent();
					ClearScreen();
					_sw2.Restart();
					_background.Draw(perspective, _camera.ViewMatrix);
					_backgroundDraw.Set(_sw2.ElapsedMilliseconds);
					_sw2.Restart();
					DrawPoints(mvp);
					_pointsDraw.Set(_sw2.ElapsedMilliseconds);
					glControl.SwapBuffers();
					glControl.Context.MakeCurrent(null);
				}
			}
			else
			{
				lock (ContextLock)
				{
					glControl.MakeCurrent();
					ClearScreen();
					_sw2.Restart();
					_background.Draw(perspective, _camera.ViewMatrix);
					_backgroundDraw.Set(_sw2.ElapsedMilliseconds);
					glControl.SwapBuffers();
					glControl.Context.MakeCurrent(null);
				}
			}
			_isRendering = false;
			_previewUpdate.Set(_sw.ElapsedMilliseconds);
			UpdateFrameRate();
			//Logging.Debug("Exiting RenderFrame");
		}

		private void ClearScreen()
		{
			// set up the OpenGL viewport and clear both the color and depth bits
			GL.Viewport(0, 0, _width, _height);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		private void DrawPoints(Matrix4 mvp)
		{
			try
			{
				//Logging.Debug("Selecting point program.");
				_program.Use();
				_program["mvp"].SetValue(mvp);
				_program["pointScale"].SetValue(_pointScaleFactor);

				foreach (var dataDisplayItem in Data.DisplayItems)
				{
					dataDisplayItem.Shape.Draw(_program);
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured rendering display items.");
			}
			//Logging.Debug("Exiting DrawPoints.");
		}

		private void CalculatePointScaleFactor()
		{
			if (!_enableLightScaling)
			{
				_pointScaleFactor = 1;
				return;
			}
			float scale = _focalDepth / _camera.Position.Z;
			float sizeScale = 0;
			if (_background.HasBackground)
			{
				sizeScale = ((float) _width / _background.Width + (float) _height / _background.Height) / 2f;
			}
			else
			{
				sizeScale = 1;
			}

			scale *= sizeScale;

			scale = scale >= .1f ? scale : .1f;

			_pointScaleFactor = scale;

		}

		private void UpdateShapePoints()
		{
			//Prepare the points
			//Logging.Debug("Begin Update Shape Points.");
			int height = _background.HasBackground ? _background.Height : Height;
			Parallel.ForEach(Data.DisplayItems, _parallelOptions, d => d.Shape.UpdateDrawPoints(height));
		}

		private double ConvertToRadians(double angle)
		{
			return angle * Math.PI / 180;
		}

		private void SaveWindowState()
		{
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"OpenGLPreview_{InstanceId}";
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientHeight", name), ClientSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientWidth", name), ClientSize.Width);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), Location.X);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), Location.Y);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionX", name), _camera.Position.X);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionY", name), _camera.Position.Y);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", name), _camera.Position.Z);
			//xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), WindowState.ToString());

			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), _showStatus);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), _alwaysOnTop);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EnableLightScaling", name), _enableLightScaling);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), IsOnTopWhenPlaying);
		}

		private void RestoreWindowState()
		{
			WindowState = FormWindowState.Normal;
			StartPosition = FormStartPosition.WindowsDefaultBounds;
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"OpenGLPreview_{InstanceId}";

			_showStatus = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), true);
			_alwaysOnTop = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), false);
			_enableLightScaling = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EnableLightScaling", name), true);
			IsOnTopWhenPlaying = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), false);

			ConfigureStatusBar();
			ConfigureAlwaysOnTop();

			var desktopBounds =
				new Rectangle(
					new Point(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), Location.X),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), Location.Y)),
					new Size(100, 100));

			if (IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopLocation = desktopBounds.Location;

				//if (
				//	xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), "Normal")
				//		.Equals("Maximized"))
				//{
				//	WindowState = FormWindowState.Maximized;
				//}
			}
			else
			{
				// this resets the upper left corner of the window to windows standards
				StartPosition = FormStartPosition.WindowsDefaultLocation;
			}

			var cHeight = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientHeight", name),
				_background.HasBackground ? _background.Height : 0);
			var cWidth = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientWidth", name),
				_background.HasBackground ? _background.Width : 0);

			if (cHeight == 0 && cWidth == 0)
			{
				var size = FindMaxPreviewSize();
				if (size.Height > 50 && size.Width > 50)
				{
					cHeight = size.Height;
					cWidth = size.Width;
				}
				else
				{
					cHeight = _height;
					cWidth = _width;
				}
				
			}
			ClientSize = new Size(cWidth, cHeight);
			_width = cWidth;
			_height = cHeight;
		}

		private void CreateCamera()
		{
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"OpenGLPreview_{InstanceId}";
			// create our camera
			_camera = new Camera(new Vector3((_background.HasBackground ? _background.Width:ClientSize.Width) / 2f, (_background.HasBackground ? _background.Height:ClientSize.Height) / 2f, _focalDepth), Quaternion.Identity);
			_camera.SetDirection(new Vector3(0, 0, -1));
			//_camera.Position = new Vector3(_camera.Position.X, _camera.Position.Y, xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", Name),_camera.Position.Z));
			_camera.Position = new Vector3(
				xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionX", name), _camera.Position.X),
				xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionY", name), _camera.Position.Y),
				xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", name), _camera.Position.Z));
			UpdateStatusDistance(_camera.Position.Z);
		}

		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location)) ||
				Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(new Point(rect.Top, rect.Right)));
		}

		private Size FindMaxPreviewSize()
		{
			int bottom = 0;
			int right = 0;
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				bottom = Math.Max(bottom, dataDisplayItem.Shape.Bottom);
				right = Math.Max(right, dataDisplayItem.Shape.Right);
			}

			return new Size(right, bottom);
		}

		
		public static string VertexShader = @"
		#version 330

		in vec3 vertexPosition;
		in vec4 vertexColor;
		in float vertexSize;
		
		out vec4 color;
		out float pSize;

		uniform float pointScale;
		uniform mat4 mvp;

		void main(void)
		{
			color = vertexColor;

			gl_Position = mvp * vec4(vertexPosition, 1);

			gl_PointSize = vertexSize * pointScale;
			
			if(vertexSize < 1)
			{
				gl_PointSize = 1;
			}
			
			pSize = gl_PointSize; //pass through
			
		}
		";

		public static string FragmentShader = @"
		#version 330

		in vec4 color;
		in float pSize;
		out vec4 fragColor;

		void main(void)
		{
			if(pSize > 2) //We only need to round points that are bigger than 2
			{
				vec2 circCoord = 2.0 * gl_PointCoord - 1.0;
				if (dot(circCoord, circCoord) > 1)
				{
					discard;
				}
			}
			
			fragColor = vec4(color.r/255.0, color.g/255.0, color.b/255.0, color.a/255.0);

		}
		";

	}
}
