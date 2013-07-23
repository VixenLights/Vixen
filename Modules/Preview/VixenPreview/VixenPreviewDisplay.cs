using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewDisplay : Form
	{
		private VixenPreviewData _data;

		public VixenPreviewData Data
		{
			set
			{
				if (value == null) {
					VixenSystem.Logging.Warning("VixenPreviewDisplay: Data set as null! (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				_data = value;
				if (!DesignMode)
					preview.Data = _data;
			}
			get
			{
				if (_data == null) {
					VixenSystem.Logging.Warning("VixenPreviewDisplay: Data get, _data is null! (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				return _data;
			}

		}

		public VixenPreviewDisplay()
		{
			InitializeComponent();
		}

		public VixenPreviewControl PreviewControl
		{
			get { return preview; }
		}

		public void Setup()
		{
			preview.LoadBackground(Data.BackgroundFileName);

			//Sometimes the preview shows up outside the bounds of the display....
			//this will reset that value if it happens

			var minX = Screen.AllScreens.Min(m => m.Bounds.X);
			var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

			var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
			var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

			if (Data.Left < minX || Data.Left > maxX)
				Data.Left = 0;
			if (Data.Top < minY || Data.Top > maxY)
				Data.Top = 0;

			SetDesktopLocation(Data.Left, Data.Top);
			Size = new Size(Data.Width, Data.Height);
		}

		private void timerStatus_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabel1.Text = "Lights: " + preview.PixelCount.ToString();
			//toolStripAverageUpdate.Text = "Average: " + Math.Round(VixenPreviewControl.averageUpdateTime).ToString() + "ms";
			//toolStripStatusCurrentUpdate.Text = "Last: " + Math.Round(VixenPreviewControl.lastUpdateTime).ToString() + "ms";
			//toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(lastRenderTime).ToString() + "ms";
			toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(preview.lastRenderUpdateTime).ToString() + "ms";
		}

		private void VixenPreviewDisplay_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing) {
				MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
				                MessageBoxButtons.OKCancel);
				e.Cancel = true;
			}
		}

		private void VixenPreviewDisplay_Move(object sender, EventArgs e)
		{
			if (Data == null) {
				VixenSystem.Logging.Warning("VixenPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Top = Top;
			Data.Left = Left;
		}

		private void VixenPreviewDisplay_Resize(object sender, EventArgs e)
		{
			if (Data == null) {
				VixenSystem.Logging.Warning("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Width = Width;
			Data.Height = Height;
		}

		private void VixenPreviewDisplay_Load(object sender, EventArgs e)
		{
			preview.Reload();
		}
	}
}