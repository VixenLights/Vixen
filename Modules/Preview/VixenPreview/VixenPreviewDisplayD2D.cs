using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common.Controls.Direct2D;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview {
	public partial class VixenPreviewDisplayD2D : Form {
		private VixenPreviewData _data;

		public VixenPreviewData Data {
			set {
				_data = value;

			}
			get { return _data; }
		}


		public VixenPreviewDisplayD2D() {
			InitializeComponent();
		}



		public void Setup() {
			direct2DControlWinForm1.BackgroundImage = Image.FromFile(Data.BackgroundFileName);

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
			Size = new Size(direct2DControlWinForm1.BackgroundImage.Width, direct2DControlWinForm1.BackgroundImage.Height);
			Reload();
		}


		public ConcurrentDictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<ElementNode, List<PreviewPixel>>();
		public List<DisplayItem> DisplayItems {
			get {
				if (Data != null) {
					return Data.DisplayItems;
				}
				else {
					return null;
				}
			}
		}
		public void Reload() {
			//lock (PreviewTools.renderLock)
			//{
			if (NodeToPixel == null) PreviewTools.Throw("PreviewBase.NodeToPixel == null");
			NodeToPixel.Clear();

			if (DisplayItems == null) PreviewTools.Throw("DisplayItems == null");
			foreach (DisplayItem item in DisplayItems) {
				if (item.Shape.Pixels == null) PreviewTools.Throw("item.Shape.Pixels == null");
				foreach (PreviewPixel pixel in item.Shape.Pixels) {
					if (pixel.Node != null) {
						List<PreviewPixel> pixels;
						if (NodeToPixel.TryGetValue(pixel.Node, out pixels)) {
							if (!pixels.Contains(pixel)) {
								pixels.Add(pixel);
							}
						}
						else {
							pixels = new List<PreviewPixel>();
							pixels.Add(pixel);
							NodeToPixel.TryAdd(pixel.Node, pixels);
						}
					}
				}
			}
			//LoadBackground();
			//}
		}
		public void ProcessUpdateParallel(ElementIntentStates elementStates) {
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			if (!direct2DControlWinForm1.Paused) {

				try {
					direct2DControlWinForm1.Points = new List<DisplayScene.DisplayPoint>();

					//elementStates.AsParallel().WithCancellation(tokenSource.Token).ForAll(channelIntentState => {
					elementStates.ToList().ForEach(channelIntentState => {
						var elementId = channelIntentState.Key;
						Element element = VixenSystem.Elements.GetElement(elementId);
						if (element != null) {
							ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
							if (node != null) {
								List<PreviewPixel> pixels;
								if (NodeToPixel.TryGetValue(node, out pixels)) {


									foreach (PreviewPixel pixel in pixels) {
										 Points.Add(new DisplayScene.DisplayPoint() {
											Color = pixel.PixelColor,
											PixelSize = pixel.PixelSize,
											Shape = new Rectangle() {
												X = pixel.X,
												Y = pixel.Y,
												Height = pixel.PixelSize,
												Width = pixel.PixelSize
											}
										});
									}
								}


							}
						}
					});
					direct2DControlWinForm1.WritePoints(Points);
					Points.Clear();
				}
				catch (Exception e) {
					tokenSource.Cancel();
					//Console.WriteLine(e.Message);
				}
			}


		}

		List<DisplayScene.DisplayPoint> Points = new List<DisplayScene.DisplayPoint>();

		private void VixenPreviewDisplay_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {
				MessageBox.Show("The preview can only be closed from the Preview Configuration dialog.", "Close",
								MessageBoxButtons.OKCancel);
				e.Cancel = true;
			}
		}




	}
}