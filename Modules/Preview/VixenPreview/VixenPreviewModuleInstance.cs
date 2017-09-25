using System;
using System.Windows.Forms;
using System.Diagnostics;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.GDIPreview;

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

		public bool UseGDIPreviewRendering
		{
			get { return true; }
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

				if (UseGDIPreviewRendering)
				{
					_displayForm = new GDIPreviewForm(GetDataModel());
					_displayForm.DisplayName = Name;
				}
				else
				{
					_displayForm = new VixenPreviewDisplayD2D();
					_displayForm.Data = GetDataModel();
				}

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
			_setupForm.Data = GetDataModel();

			_setupForm.ShowDialog();

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