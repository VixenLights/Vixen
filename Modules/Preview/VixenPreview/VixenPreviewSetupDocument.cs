using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetupDocument : DockContent
	{
		public VixenPreviewSetupDocument()
		{
			InitializeComponent();
		}

		public VixenPreviewControl Preview
		{
			get { return previewControl; }
		}

		private void VixenPreviewSetupDocument_Load(object sender, EventArgs e)
		{
			previewControl.EditMode = true;
		}

		private void VixenPreviewSetupDocument_FormClosing(object sender, FormClosingEventArgs e)
		{
			//timerRender.Stop();
		}

		private void VixenPreviewSetupDocument_Paint(object sender, PaintEventArgs e)
		{
			Preview.RenderInForeground();
		}
	}
}