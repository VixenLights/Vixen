using System;
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

		public GDIPreviewForm(VixenPreviewData data)
		{
			InitializeComponent();
			Data = data;
		}

		public VixenPreviewData Data { get; set; }

		public void UpdatePreview(/*Vixen.Preview.PreviewElementIntentStates elementStates*/)
		{
			if (!gdiControl.IsUpdating)
			{
				gdiControl.BeginUpdate();

				Vixen.Sys.Managers.ElementManager elements = VixenSystem.Elements;

				Element[] elementArray = elements.Where(e => e.State.Where(i => (i as IIntentState<LightingValue>) !=null).Where(i => (i as IIntentState<LightingValue>).GetValue().Intensity > 0).Any()).ToArray();
				//Console.WriteLine(elements.Count() + ":" + elementArray.Count());
				CancellationTokenSource tokenSource = new CancellationTokenSource();

				//elements.AsParallel().WithCancellation(tokenSource.Token).ForAll(element =>
				elementArray.AsParallel().WithCancellation(tokenSource.Token).ForAll(element =>
				{
					if (element != null)
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
					}
				});
				gdiControl.EndUpdate();

				gdiControl.RenderImage();
				toolStripStatusFPS.Text = gdiControl.FrameRate.ToString() + "fps";
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
		}

		public void Setup()
		{
			Reload();

			var minX = Screen.AllScreens.Min(m => m.Bounds.X);
			var maxX = Screen.AllScreens.Sum(m => m.Bounds.Width) + minX;

			var minY = Screen.AllScreens.Min(m => m.Bounds.Y);
			var maxY = Screen.AllScreens.Sum(m => m.Bounds.Height) + minY;

			// avoid 0 with/height in case Data comes in 'bad'
			if (Data.Width == 0)
				Data.Width = 400;
			if (Data.SetupWidth == 0)
				Data.SetupWidth = 400;
			if (Data.Height == 0)
				Data.Height = 300;
			if (Data.SetupHeight == 0)
				Data.SetupHeight = 300;

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
