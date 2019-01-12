using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using VixenModules.Preview.VixenPreview.Direct2D;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview {
	public partial class VixenPreviewDisplayD2D : BaseForm, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public VixenPreviewDisplayD2D() {
			InitializeComponent();
			BackColor = ThemeColorTable.BackgroundColor;
			//direct2DControlWinForm1. = new DisplayScene(null, DisplayID);
			Scene = new DisplayScene(null);
			Scene.IsAnimating = true;
			 
		}

		public void UpdatePreview()
		{
			Scene.Update();
		}

		public DisplayScene Scene { get { return (DisplayScene)previewWinform1.Scene; } set { previewWinform1.Scene = value; } }

		VixenPreviewData _data;
		private void setSceneData() {
			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericDelegate(setSceneData));
			else
				Scene.Data = _data;
		}

		public VixenPreviewData Data {
			set {

				_data = value;
				setSceneData();
				Reload();

			}
			get { return _data; }
		}

		 

		public void IsAnimating(bool enabled) {
			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericBool(IsAnimating), enabled);
			else
				Scene.IsAnimating = enabled;
		}
		public void Reload() {
			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericDelegate(Reload));
			else
				Scene.Reload();
		}

		public override Image BackgroundImage {
			get {
				return Scene.BackgroundImage;
			}
			set {
				if (Scene == null) {
					Scene = new DisplayScene(value);
					Scene.IsAnimating = true;
				}
				Scene.BackgroundImage = value;
			}
		}

		public void Setup() {
			
			var file = Path.Combine(VixenPreviewDescriptor.ModulePath, Data.BackgroundFileName);
			BackgroundImage = File.Exists(file) ? Image.FromFile(file) : null;
			
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
			if (BackgroundImage != null) {
				this.previewWinform1.BackgroundImage = BackgroundImage;
				Size = new Size(this.previewWinform1.BackgroundImage.Width, previewWinform1.BackgroundImage.Height+40);
			}
			else
				Size = new System.Drawing.Size(Data.Width, Data.Height);

			Scene.Reload();
			Scene.IsAnimating = true;
		}


		private void VixenPreviewDisplay_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
								MessageBoxButtons.OK);
				e.Cancel = true;
			}
		}

		private void VixenPreviewDisplay_Move(object sender, EventArgs e) {
			if (Data == null) {
				 Logging.Warn("VixenPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Top = Top;
			Data.Left = Left;
		}

		private void VixenPreviewDisplay_Resize(object sender, EventArgs e) {
			if (Data == null) {
				Logging.Warn("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Width = Width;
			Data.Height = Height;
		}

		public string DisplayName { get; set; }

		/// <inheritdoc />
		public Guid InstanceId { get; set; }

		/// <inheritdoc />
		public bool IsOnTopWhenPlaying { get; }
	}
}