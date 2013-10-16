using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Preview.VixenPreview.Shapes;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;
using System.Diagnostics;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetup : Form
	{
		private VixenPreviewData _data;

		public VixenPreviewData Data
		{
			set
			{
				_data = value;
				if (!DesignMode)
					preview.Data = _data;
			}
			get { return _data; }
		}

		public VixenPreviewSetup()
		{
			InitializeComponent();
		}

		private int currentNode = -1;

		private ElementNode GetNextNode()
		{
			currentNode++;
			if (currentNode >= VixenSystem.Nodes.Count()) {
				currentNode = 0;
				Console.WriteLine(VixenSystem.Nodes.Count());
			}
			ElementNode node = VixenSystem.Nodes.GetAllNodes().ElementAt(currentNode);
			if (node.Children.Count() > 0) {
				node = VixenSystem.Nodes.GetAllNodes().ElementAt(2);
			}
			return node;
		}

		private void toolStripSelect_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.Select;
		}

		private void toolStripLightString_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.String;
		}

		//public void UpdateColors(Vixen.Sys.ElementNode node, Color newColor)
		//{
		//    preview.UpdateColors(node, newColor);
		//}

		private void preview_Load(object sender, EventArgs e)
		{
		}

		public void RefreshPreview()
		{
			preview.RenderInForeground();
			//preview.DrawDisplayItemsInBackground();
		}

		private void timerStatus_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabel1.Text = preview.PixelCount.ToString();
			//toolStripAverageUpdate.Text = "Average: " + Math.Round(VixenPreviewControl.averageUpdateTime).ToString() + "ms";
			toolStripStatusCurrentUpdate.Text = string.Format( "Last: {0}ms", Math.Round(VixenPreviewControl.lastUpdateTime));
			//toolStripStatusLastRenderTime.Text = "Render: " + Math.Round(lastRenderTime).ToString() + "ms";
			toolStripStatusLastRenderTime.Text = string.Format("Render: {0}ms", Math.Round(preview.lastRenderUpdateTime));
		}

		//public void ResetColors()
		//{
		//    preview.ResetColors();
		//}

		private void toolStripButtonArch_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.Arch;
		}

		private void toolStripButtonEditMode_Click(object sender, EventArgs e)
		{
			bool EditMode = !toolStripButtonEditMode.Checked;

			foreach (ToolStripButton item in toolStrip.Items) {
				if (item.Tag != null && item.Tag.ToString().StartsWith("Edit")) {
					item.Enabled = EditMode;
				}
			}

			preview.EditMode = EditMode;
			toolStripButtonEditMode.Checked = EditMode;
		}

		private void SelectEditToolbarButton(ToolStripButton button)
		{
			foreach (ToolStripButton item in toolStrip.Items) {
				if (item.Tag != null && item.Tag.ToString().StartsWith("Edit"))
					item.Checked = (item == button);
			}
		}

		private void SelectEditToolbarButton(string tool)
		{
			foreach (ToolStripButton item in toolStrip.Items) {
				if (item.Tag != null) {
					string toolStr = item.Tag.ToString().Substring(4);
					{
						if (toolStr == tool) {
							SelectEditToolbarButton(item);
						}
					}
				}
			}
		}

		//double totalRenderTime = 0;
		//double totalRenderCount = 0;
		//double lastRenderTime = 0;
		private void timerRenderPreview_Tick(object sender, EventArgs e)
		{
			//timerRenderPreview.Stop();

			//Stopwatch timer = new Stopwatch();
			//timer.Start();
			//preview.RenderInBackground();
			//timer.Stop();
			//lastRenderTime = timer.ElapsedMilliseconds;
			////totalRenderCount += 1;
			////totalRenderTime += timer.ElapsedMilliseconds;

			//timerRenderPreview.Start();

			//preview.RenderInBackground();
		}

		public void Setup()
		{
			preview.LoadBackground(Data.BackgroundFileName);

			Top = Data.Top;

			Left = Data.Left;

			if (Data.Width > MinimumSize.Width)
				Width = Data.Width;
			else
				Width = MinimumSize.Width;

			if (Data.Height > MinimumSize.Height)
				Height = Data.Height;
			else
				Height = MinimumSize.Height;

			//if (Data.BackgroundAlpha == 0)
			//    Data.BackgroundAlpha = 255;
			scrollBackgroundAlpha.Value = Data.BackgroundAlpha;
		}

		public void Save()
		{
			Data.Top = Top;
			Data.Left = Left;
			Data.Width = Width;
			Data.Height = Height;
			//Data.BackgroundAlpha = scrollBackgroundAlpha.Value;
		}

		private int lightCount = 75;
		private int treeStrings = 20;
		private int pixelCount = 0;

		private void menuAddStuffToScreen_Click(object sender, EventArgs e)
		{
			int bottomOffset;
			int topOffset;
			int treeHeight;
			int currentBottomX;
			int currentTopX;
			System.Drawing.Point topPoint;
			int startX = 200;

			timerRenderPreview.Stop();

			for (int treeNum = 0; treeNum < 1; treeNum += 2) {
				// Tree Set 1
				topPoint = new System.Drawing.Point(startX, 50);
				bottomOffset = 20;
				topOffset = 2;
				treeHeight = 400;
				currentBottomX = topPoint.X - (int) (((treeStrings/2) - 1)*bottomOffset) - (int) (.5*bottomOffset);
				currentTopX = topPoint.X - (int) (((treeStrings/2) - 1)*topOffset) - (int) (.5*topOffset);
				for (int i = 0; i < treeStrings; i++) {
					DisplayItem displayItem = new DisplayItem();
					displayItem.Shape = new PreviewLine(new PreviewPoint(currentTopX, topPoint.Y),
					                                    new PreviewPoint(currentBottomX, topPoint.Y + treeHeight), lightCount, null);
					displayItem.Shape.PixelSize = 3;
					preview.AddDisplayItem(displayItem);
					pixelCount += displayItem.Shape.Pixels.Count();
					foreach (PreviewPixel pixel in displayItem.Shape.Pixels) {
						pixel.Node = GetNextNode();
					}
					currentBottomX += bottomOffset;
					currentTopX += topOffset;
				}
				// Tree Set 1 Upside Down
				topPoint = new System.Drawing.Point(startX + 300, 50);
				bottomOffset = 2;
				topOffset = 20;
				treeHeight = 400;
				currentBottomX = topPoint.X - (int) (((treeStrings/2) - 1)*bottomOffset) - (int) (.5*bottomOffset);
				currentTopX = topPoint.X - (int) (((treeStrings/2) - 1)*topOffset) - (int) (.5*topOffset);
				for (int i = 0; i < treeStrings; i++) {
					DisplayItem displayItem = new DisplayItem();
					displayItem.Shape = new PreviewLine(new PreviewPoint(currentTopX, topPoint.Y),
					                                    new PreviewPoint(currentBottomX, topPoint.Y + treeHeight), lightCount, null);
					preview.AddDisplayItem(displayItem);
					pixelCount += displayItem.Shape.Pixels.Count();
					foreach (PreviewPixel pixel in displayItem.Shape.Pixels) {
						pixel.Node = GetNextNode();
					}
					currentBottomX += bottomOffset;
					currentTopX += topOffset;
				}


				//// Tree Set 2
				topPoint = new System.Drawing.Point(startX + 600, 50);
				bottomOffset = 20;
				topOffset = 2;
				treeHeight = 400;
				currentBottomX = topPoint.X - (int) (((treeStrings/2) - 1)*bottomOffset) - (int) (.5*bottomOffset);
				currentTopX = topPoint.X - (int) (((treeStrings/2) - 1)*topOffset) - (int) (.5*topOffset);
				for (int i = 0; i < treeStrings; i++) {
					DisplayItem displayItem = new DisplayItem();
					displayItem.Shape = new PreviewLine(new PreviewPoint(currentTopX, topPoint.Y),
					                                    new PreviewPoint(currentBottomX, topPoint.Y + treeHeight), lightCount, null);
					preview.AddDisplayItem(displayItem);
					pixelCount += displayItem.Shape.Pixels.Count();
					currentBottomX += bottomOffset;
					foreach (PreviewPixel pixel in displayItem.Shape.Pixels) {
						pixel.Node = GetNextNode();
					}
					currentTopX += topOffset;
				}
				//// Tree Set 2 Upside Down
				topPoint = new System.Drawing.Point(startX + 900, 50);
				bottomOffset = 2;
				topOffset = 20;
				treeHeight = 400;
				currentBottomX = topPoint.X - (int) (((treeStrings/2) - 1)*bottomOffset) - (int) (.5*bottomOffset);
				currentTopX = topPoint.X - (int) (((treeStrings/2) - 1)*topOffset) - (int) (.5*topOffset);
				for (int i = 0; i < treeStrings; i++) {
					DisplayItem displayItem = new DisplayItem();
					displayItem.Shape = new PreviewLine(new PreviewPoint(currentTopX, topPoint.Y),
					                                    new PreviewPoint(currentBottomX, topPoint.Y + treeHeight), lightCount, null);
					preview.AddDisplayItem(displayItem);
					pixelCount += displayItem.Shape.Pixels.Count();
					currentBottomX += bottomOffset;
					foreach (PreviewPixel pixel in displayItem.Shape.Pixels) {
						pixel.Node = GetNextNode();
					}
					currentTopX += topOffset;
				}

				startX += 600;
			}
			Console.WriteLine("Total Pixels: " + pixelCount.ToString());
			toolStripStatusLabel1.Text = "Pixel Count: " + pixelCount.ToString();

			timerRenderPreview.Start();
		}

		private void toolStripButtonSetBackground_Click(object sender, EventArgs e)
		{
			if (dialogSelectBackground.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				Data.BackgroundFileName = dialogSelectBackground.FileName;
				preview.LoadBackground(dialogSelectBackground.FileName);
				scrollBackgroundAlpha.Value = scrollBackgroundAlpha.Maximum;
				//preview.BackgroundAlpha = scrollBackgroundAlpha.Value;
			}
		}

		private void VixenPreviewSetup_Load(object sender, EventArgs e)
		{
			//ToolStripLabel label = new ToolStripLabel("Background Intensity:")
			//label.Spring
			//statusStrip1.Items.Add(label);

			ToolStripControlHost host = new ToolStripControlHost(scrollBackgroundAlpha);
			statusStrip1.Items.Add(host);

			Setup();
		}

		private void scrollBackgroundAlpha_ValueChanged(object sender, EventArgs e)
		{
			//preview.BackgroundAlpha = scrollBackgroundAlpha.Maximum - scrollBackgroundAlpha.Value;
			preview.BackgroundAlpha = scrollBackgroundAlpha.Value;
		}

		private void toolStripButtonRectangle_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.Rectangle;
		}

		private void toolStripButtonSingleLight_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.Single;
		}

		private void toolStripButtonMegaTree_Click(object sender, EventArgs e)
		{
			//MessageBox.Show("Coming soon!");
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.MegaTree;
		}

		private void toolStripButtonEllipse_Click(object sender, EventArgs e)
		{
			SelectEditToolbarButton(sender as ToolStripButton);
			preview.CurrentTool = VixenPreviewControl.Tools.Ellipse;
		}

		private void preview_MouseUp(object sender, MouseEventArgs e)
		{
			//MessageBox.Show("MouseUp");
			SelectEditToolbarButton(preview.CurrentTool.ToString());
		}

		//public void RenderInBackground()
		//{
		//    preview.RenderInBackground();
		//}
	}
}