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
	public partial class GDIPreviewForm : Form
	{
		public GDIPreviewForm(VixenPreviewData data)
		{
			InitializeComponent();
			Data = data;
		}

		public VixenPreviewData Data { get; set; }

		private Stopwatch lastUpdate = new Stopwatch();
		public void Update(ElementIntentStates elementStates)
		{
			gdiControl.BeginUpdate();

			lastUpdate.Stop();
			if (lastUpdate.ElapsedMilliseconds > 500)
			{
				toolStripStatusLabel2.Text = lastUpdate.ElapsedMilliseconds.ToString();
			}
			lastUpdate.Restart();

			CancellationTokenSource tokenSource = new CancellationTokenSource();

			elementStates.AsParallel().WithCancellation(tokenSource.Token).ForAll(channelIntentState =>
			{
				var elementId = channelIntentState.Key;
				Element element = VixenSystem.Elements.GetElement(elementId);
				if (element != null)
				{
					ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
					if (node != null)
					{
						//Color pixColor = Vixen.Intent.ColorIntent.GetAlphaColorForIntents(channelIntentState.Value);

						List<PreviewPixel> pixels;
						if (NodeToPixel.TryGetValue(node, out pixels))
						{
							foreach (PreviewPixel pixel in pixels)
							{
								pixel.Draw(gdiControl.FastPixel, channelIntentState.Value);
								//pixel.Draw(gdiControl.FastPixel, pixColor);
								//gdiControl.SetPixel(pixel.X, pixel.Y, pixColor);
							}
						}

						//List<PreviewPixel> pixels;
						//if (NodeToPixel.TryGetValue(node, out pixels))
						//{
						//    foreach (PreviewPixel pixel in pixels)
						//    {
						//        pixel.Draw(gdiControl.FastPixel, channelIntentState.Value);
						//    }
						//}
					}
				}
			});

			gdiControl.EndUpdate();

			gdiControl.RenderImage();
			toolStripStatusLabel1.Text = gdiControl.RenderTime.ToString() + "ms";
		}

		private void GDIPreviewForm_Load(object sender, EventArgs e)
		{
			//gdiControl.Background = Bitmap.FromFile("C:\\SkyDrive\\Christmas Lights\\Hillary 2012 1500x489.jpg");
			Reload();
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
				foreach (DisplayItem item in DisplayItems)
				{
					if (item.Shape.Pixels == null)
						throw new System.ArgumentException("item.Shape.Pixels == null");

					foreach (PreviewPixel pixel in item.Shape.Pixels)
					{
						if (pixel.Node != null)
						{
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

			if (System.IO.File.Exists(Data.BackgroundFileName))
				gdiControl.Background = Image.FromFile(Data.BackgroundFileName);
			else
				gdiControl.Background = null;
		}

	}
}
