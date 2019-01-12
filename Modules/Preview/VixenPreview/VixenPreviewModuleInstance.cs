using System;
using System.Windows.Forms;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.GDIPreview;
using VixenModules.Preview.VixenPreview.OpenGL;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewModuleInstance
	{
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
			bool supported = false;
			try
			{
				lock (OpenGlPreviewForm.ContextLock)
				{
					var control = new GLControl();
					control.MakeCurrent();
					var major = GL.GetInteger(GetPName.MajorVersion);
					var minor = GL.GetInteger(GetPName.MinorVersion);
					if (major > 3 || (major == 3 && minor >=3))
					{
						Logging.Info($"Open GL version supported!. {major}.{minor}");
						supported = true;
					}
					else
					{
						Logging.Error($"Open GL version not supported. {major}.{minor}");
					}

					control.Context.MakeCurrent(null);
					control.Dispose();
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured testing for OpenGL support.");
			}

			return supported;

		}

		protected override Form Initialize()
		{
			SetupPreviewForm();
			return (Form)_displayForm;
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
						_displayForm = new OpenGlPreviewForm(GetDataModel(), InstanceId);
					}
					catch (Exception ex)
					{

						Logging.Error(ex, "An error occured trying to create the OpenGL Preview.");
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

		/// <inheritdoc />
		protected override void PlayerActivatedImpl()
		{
			if (_displayForm.IsOnTopWhenPlaying)
			{
				((Form) _displayForm).TopMost = true;
			}
		}

		/// <inheritdoc />
		protected override void PlayerDeactivatedImpl()
		{
			if (_displayForm.IsOnTopWhenPlaying)
			{
				((Form)_displayForm).TopMost = false;
				((Form)_displayForm).SendToBack();
			}
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