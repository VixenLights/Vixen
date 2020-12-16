using System;
using System.Windows.Forms;
using System.Windows.Shapes;
using Common.Controls.Theme;
using VixenModules.Preview.VixenPreview.Shapes;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetupPropertiesDocument : DockContent
	{
		private readonly VixenPreviewControl _previewControl;
		private DisplayItemBaseControl _setupControl;

		public VixenPreviewSetupPropertiesDocument(VixenPreviewControl previewControl)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			_previewControl = previewControl;
		}

		public void ShowSetupControl(DisplayItemBaseControl setupControl)
		{
			if (_setupControl != null)
			{
				_setupControl.PropertyEdited -= SetupControlPropertyEdited;
			}

			_setupControl = setupControl;
			Controls.Clear();
			if (setupControl != null)
			{
				setupControl.PropertyEdited += SetupControlPropertyEdited;
				Controls.Add(setupControl);
				setupControl.Dock = DockStyle.Fill;
				Text = setupControl.Title;
			}
			else {
				Text = @"Properties";
			}
		}

		public PreviewBaseShape SetupPreviewShape()
		{
			return _setupControl?.Shape;
		}

		private void SetupControlPropertyEdited(object sender, EventArgs e)
		{
			_previewControl.EndUpdate();
		}

		public void ClearSetupControl()
		{
			Controls.Clear();
			Text = @"Properties";
            _setupControl = null;
        }
	}
}