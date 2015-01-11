using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using VixenModules.Property.Location;

namespace VixenModules.Preview.VixenPreview
{
	public partial class GDIPreviewForm : Form, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private bool needsUpdate = true;

		public GDIPreviewForm(VixenPreviewData data)
		{
			InitializeComponent();
			Data = data;
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

		public VixenPreviewData Data { get; set; }

		public void UpdatePreview(/*Vixen.Preview.PreviewElementIntentStates elementStates*/)
		{
			if (!gdiControl.IsUpdating)
			{
				
				IEnumerable<Element> elementArray = VixenSystem.Elements.Where(e => e.State.Any());

				if (!elementArray.Any())
				{
					if (needsUpdate)
					{
						needsUpdate = false;
						gdiControl.BeginUpdate();
						gdiControl.EndUpdate();
						gdiControl.Invalidate();
					}

					toolStripStatusFPS.Text =  "0 fps";
					return;
				}

				needsUpdate = true;

				gdiControl.BeginUpdate();

				try
				{
					var po = new ParallelOptions
					{
						MaxDegreeOfParallelism = Environment.ProcessorCount
					};

					Parallel.ForEach(elementArray, po, element => 
					{
						ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
						if (node != null)
						{
							List<PreviewPixel> pixels;
							if (NodeToPixel.TryGetValue(node, out pixels))
							{
								foreach (PreviewPixel pixel in pixels)
								{
									pixel.Draw(gdiControl.FastPixel, element.State);
								}
							}
						}
					});
				} catch (Exception e)
				{
					Logging.Error(e.Message, e);
				}
				
				gdiControl.EndUpdate();
				gdiControl.Invalidate();

				toolStripStatusFPS.Text = string.Format("{0} fps", gdiControl.FrameRate);
			}
		}

		public ConcurrentDictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<ElementNode, List<PreviewPixel>>();
		public List<DisplayItem> DisplayItems
		{
			get
			{
				if (Data != null)
				{
					return Data.DisplayItems;
				}
				else
				{
					return null;
				}
			}
		}

		public void Reload()
		{
			if (NodeToPixel == null)
				throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

			NodeToPixel.Clear();

			if (DisplayItems == null)
				throw new System.ArgumentException("DisplayItems == null");

			if (DisplayItems != null)
			{
				int pixelCount = 0;
				foreach (DisplayItem item in DisplayItems)
				{
					item.Shape.Layout();
					if (item.Shape.Pixels == null)
						throw new System.ArgumentException("item.Shape.Pixels == null");

					foreach (PreviewPixel pixel in item.Shape.Pixels)
					{
						if (pixel.Node != null)
						{
							pixelCount++;
							List<PreviewPixel> pixels;
							if (NodeToPixel.TryGetValue(pixel.Node, out pixels))
							{
								if (!pixels.Contains(pixel))
								{
									pixels.Add(pixel);
								}
							}
							else
							{
								pixels = new List<PreviewPixel>();
								pixels.Add(pixel);
								NodeToPixel.TryAdd(pixel.Node, pixels);
							}
						}
					}
				}
				toolStripStatusPixels.Text = pixelCount.ToString();
			}

			gdiControl.BackgroundAlpha = Data.BackgroundAlpha;
			if (System.IO.File.Exists(Data.BackgroundFileName))
				gdiControl.Background = Image.FromFile(Data.BackgroundFileName);
			else
				gdiControl.Background = null;

			LayoutProps();
		}

		public void LayoutProps()
		{
			if (DisplayItems != null)
			{
				foreach (DisplayItem item in DisplayItems)
				{
					item.Shape.Layout();
				}
			}
		}

		public void Setup()
		{
			Reload();

			var minX = Screen.AllScreens.Min(m => m.Bounds.X);
			var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

			var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
			var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

			// avoid 0 with/height in case Data comes in 'bad' -- even small is bad,
			// as it doesn't give a sizeable enough canvas to render on.
			if (Data.Width < 300) {
				if (gdiControl.Background != null && gdiControl.Background.Width > 300)
					Data.Width = gdiControl.Background.Width;
				else
					Data.Width = 400;
			}

			if (Data.SetupWidth < 300) {
				if (gdiControl.Background != null && gdiControl.Background.Width > 300)
					Data.SetupWidth = gdiControl.Background.Width;
				else
					Data.SetupWidth = 400;
			}

			if (Data.Height < 200) {
				if (gdiControl.Background != null && gdiControl.Background.Height > 200)
					Data.Height = gdiControl.Background.Height;
				else
					Data.Height = 300;
			}

			if (Data.SetupHeight < 200) {
				if (gdiControl.Background != null && gdiControl.Background.Height > 200)
					Data.SetupHeight = gdiControl.Background.Height;
				else
					Data.SetupHeight = 300;
			}

			if (Data.Left < minX || Data.Left > maxX)
				Data.Left = 0;
			if (Data.Top < minY || Data.Top > maxY)
				Data.Top = 0;

			SetDesktopLocation(Data.Left, Data.Top);
			Size = new Size(Data.Width, Data.Height);
		}

		private void GDIPreviewForm_Move(object sender, EventArgs e)
		{
			if (Data == null)
			{
				Logging.Warn("VixenPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Top = Top;
			Data.Left = Left;
		}

		private void GDIPreviewForm_Resize(object sender, EventArgs e)
		{
			if (Data == null)
			{
				Logging.Warn("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Width = Width;
			Data.Height = Height;
		}

		private void GDIPreviewForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
								MessageBoxButtons.OKCancel);
				e.Cancel = true;
			}
		}
	}
}
