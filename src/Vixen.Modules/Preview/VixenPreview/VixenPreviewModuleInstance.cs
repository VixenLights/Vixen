//#define OPENGL_PREVIEW_WIN_FORMS

using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.WinForms;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.GDIPreview;
using VixenModules.Preview.VixenPreview.OpenGL;
using System.Windows;
using Catel.MVVM;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewModuleInstance
	{
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
											int X, int Y, int cx, int cy, uint uFlags);

		private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		private const uint SWP_NOSIZE = 0x0001;
		private const uint SWP_NOMOVE = 0x0002;

		private VixenPreviewSetup3 _setupForm;
		private IDisplayForm _displayForm;
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private readonly MillisecondsValue _updateTimeValue = new MillisecondsValue("Update time for preview");

		public VixenPreviewModuleInstance()
		{
			VixenSystem.Instrumentation.AddValue(_updateTimeValue);
		}

		public override Vixen.Module.IModuleDataModel ModuleData
		{
			get
			{
				if (base.ModuleData == null) {
					base.ModuleData = new VixenPreviewData();
					Logging.Warn("VixenPreview: access of null ModuleData. Creating new one. (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				return base.ModuleData;
			}
			set
			{
				base.ModuleData = value;

				UseWPF = GetDataModel().UseOpenGL;
			}
		}

		public bool UseOpenGLRendering
		{
			get
			{
				if (GetDataModel().UseOpenGL)
				{
					var supported = SupportsOpenGLPreview();
					if (!supported)
					{
						GetDataModel().UseOpenGL = false;
					}
				}

				return GetDataModel().UseOpenGL;
			}
		}


		internal static bool SupportsOpenGLPreview()
		{
			Logging.Info("Checking for OpenGL 3.3 Support" );
			
			bool supported = false;

#if OPENGL_PREVIEW_WIN_FORMS
			lock (OpenGlPreviewForm.ContextLock)
#else
			lock (OpenGLPreviewDrawingEngine.ContextLock)
#endif
			{

				try
				{

					GLFWProvider.CheckForMainThread = false;
					GLFWProvider.EnsureInitialized();
					
					using OpenTK.Windowing.Desktop.NativeWindow window = new OpenTK.Windowing.Desktop.NativeWindow(new NativeWindowSettings()
					{
						APIVersion = new Version(3, 3, 0),
						StartVisible = false,
						AutoLoadBindings = true
					});
					
					window.Context.MakeCurrent();
				
					var major = GL.GetInteger(GetPName.MajorVersion);
					var minor = GL.GetInteger(GetPName.MinorVersion);
					if (major > 3 || (major == 3 && minor >= 3))
					{
						Logging.Info($"Open GL version supported: {major}.{minor}");
						supported = true;
					}
					else
					{
						Logging.Error($"Open GL version not supported. {major}.{minor}");
					}

				}
				catch (Exception e)
				{
					Logging.Error(e, "An error occurred testing for OpenGL support.");
				}

			}
			
			Logging.Info($"OpenGL supported: {supported}");
			return supported;

		}

		protected override Form InitializeForm()
		{			
			SetupPreviewForm();
			return (Form)_displayForm;			
		}

		protected override Window InitializeWindow()
		{			
			SetupPreviewForm();
			return (Window)_displayForm;						
		}

		private readonly object _formLock = new object();
		private void SetupPreviewForm()
		{
			lock (_formLock) {

				if (!UseOpenGLRendering)
				{
					_displayForm = new GDIPreviewForm(GetDataModel(), InstanceId);
				}
				else
				{
					try
					{
#if OPENGL_PREVIEW_WIN_FORMS
						_displayForm = new OpenGlPreviewForm(GetDataModel(), InstanceId);					
#else
						_displayForm = new WPFOpenGLPreviewView(
							GetDataModel(), 
							InstanceId, 
							new WPFOpenGLPreviewViewModel(GetDataModel(), InstanceId, new OpenGLPreviewDrawingEngine(GetDataModel())));													
#endif
					}
					catch (Exception ex)
					{

						Logging.Error(ex, "An error occurred trying to create the OpenGL Preview.");
					}
					
				}

				_displayForm.DisplayName = Name;
				_displayForm.Setup();
			}
		}

		private String _name;
		public override string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				if (_displayForm != null)
				{
					_displayForm.DisplayName = value;
				}
			}
		}

		private VixenPreviewData GetDataModel()
		{
			return ModuleData as VixenPreviewData;
		}

		public override bool Setup()
		{
			_setupForm = new VixenPreviewSetup3();
			var data = GetDataModel();
			_setupForm.Data = data;
			
			_setupForm.ShowDialog();

			if (data.UseOpenGL && _displayForm?.GetType() != typeof(OpenGlPreviewForm))
			{
				_displayForm?.Close();
				SetupPreviewForm();
			}

			if (_displayForm != null)
			{
				_displayForm.Data = GetDataModel();
				_displayForm.Setup();
			}

			return base.Setup();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_displayForm != null)
					_displayForm.Close();
			}
			
			base.Dispose(disposing);
		}


#if OPENGL_PREVIEW_WIN_FORMS

		/// <inheritdoc />
		protected override void PlayerActivatedImpl()
		{
			
			if (_displayForm.IsOnTopWhenPlaying)
			{
				((Window)_displayForm).Topmost = true;
			}			
		}
#else
		/// <inheritdoc />
		protected override void PlayerActivatedImpl()
		{
			if (UseOpenGLRendering)
			{
				if (_displayForm.IsOnTopWhenPlaying)
				{
					((Window)_displayForm).Topmost = true;
				}
			}
			else
			{
				if (_displayForm.IsOnTopWhenPlaying)
				{
					((Form)_displayForm).TopMost = true;
				}
			}
		}
#endif

#if OPENGL_PREVIEW_WIN_FORMS
		/// <inheritdoc />
		protected override void PlayerDeactivatedImpl()
		{						
			if (_displayForm.IsOnTopWhenPlaying)
			{
				((Window)_displayForm).Topmost = false;
				((Window)_displayForm).Dispatcher.Invoke(SendToBack);
			}			
		}
#else
		/// <inheritdoc />
		protected override void PlayerDeactivatedImpl()
		{
			if (UseOpenGLRendering)
			{
				if (_displayForm.IsOnTopWhenPlaying)
				{
					((Form)_displayForm).TopMost = false;
					((Form)_displayForm).SendToBack();
				}
				else
				{
					((Form)_displayForm).TopMost = false;
					((Form)_displayForm).SendToBack();
				}
			}
		}
#endif

		public void SendToBack()
		{
			var hWnd = new System.Windows.Interop.WindowInteropHelper((Window)_displayForm).Handle;
			SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
		}

		protected override void Update()
		{
			var sw = Stopwatch.StartNew();
			try {
				_displayForm.UpdatePreview();
			}
			catch (Exception e) {
				Logging.Error("Exception in preview update {0} - {1}", e.Message, e.StackTrace);
			}
			_updateTimeValue.Set(sw.ElapsedMilliseconds);
		}
	}
}