
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using VixenModules.Preview.VixenPreview.Shapes;
using Common.Resources.Properties;
using Common.Controls.Theme;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes.CustomProp
{
	public partial class CustomPropForm : Form
	{
		public CustomPropForm()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
			this.textBox1.Enabled = true;
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			ThemePropertyGridRenderer.PropertyGridRender(propertyGrid);
		}
		public CustomPropForm(string fileName)
		{
			InitializeComponent();

			if (File.Exists(fileName))
			{
				_fileName = fileName;
				this.textBox1.Enabled = false;


			}
			else this.textBox1.Enabled = true;
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;

			ThemeUpdateControls.UpdateControls(this);
		}
		private string _fileName = null;
		#region Private Variables
		private Prop _prop;
		private static readonly string PropDirectory = PreviewTools.PropsFolder;//Path.Combine(new FileInfo(Application.ExecutablePath).DirectoryName, "Props");
		private const string PROP_EXTENSION = ".prop";


		#endregion
		#region Properties
		public bool BackgroundImageMaintainAspect { get { return this.chkMaintainAspect.Checked; } set { this.chkMaintainAspect.Checked = value; } }
		public int BackgroundImageOpacity { get { return _prop.BackgroundImageOpacity; } set { _prop.BackgroundImageOpacity = value; } }

		public string BackgroundImageFileName
		{
			get
			{
				return _prop.BackgroundImage;
			}
			set
			{
				if (_prop.BackgroundImage != value)
				{
					if (System.IO.File.Exists(value))
						BackgroundImage = Image.FromFile(value);
				}

				this.txtBackgroundImage.Text = _prop.BackgroundImage = value;

			}
		}
		public List<PropChannel> Channels
		{
			get
			{
				if (_prop == null) return null;
				if (_prop.Channels == null) _prop.Channels = new List<PropChannel>();
				return _prop.Channels;
			}
			set
			{
				_prop.Channels = value;
				PopulateNodeTreeMultiSelect();
			}
		}


		#endregion

		#region Private Methods


		private void SaveProp(string Name)
		{
			if (!Directory.Exists(PropDirectory)) Directory.CreateDirectory(PropDirectory);
			var fileName = Path.Combine(PropDirectory, Name + PROP_EXTENSION);

			_prop.ToFile(fileName);
		}

		public bool NameLocked { get { return !this.textBox1.Enabled; } set { this.textBox1.Enabled = !value; } }
		private string GenerateNewChannelName(string templateName = "Channel_{0}", int index = 1)
		{

			int i = index;
			if (i < 1) i = 1;
			string channelName = string.Format(templateName, i);
			List<string> names = new List<string>();
			//foreach (PropChannel item in listBox1.Items)
			//{
			//	names.Add(item.Text);
			//}
			while (names.Contains(channelName))
			{
				channelName = string.Format(templateName, i++);
			}

			return channelName;
		}

		int _currentRowHeight = -1;

		#endregion


		#region  Context Menu
		private void toolStripMenuItem_Add_Click(object sender, EventArgs e)
		{
			ChannelNaming form = new ChannelNaming();
			form.Value = GenerateNewChannelName();
			var result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				PropChannel item = new PropChannel(form.Value);
				//Channels.Add(item);
				//listBox1.Items.Add(item);
			}
		}

		private void toolStripMenuItem_AddMultiple_Click(object sender, EventArgs e)
		{
			AddMultipleChannels form = new AddMultipleChannels();
			var result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				for (int i = 0; i < form.ChannelCount; i++)
				{

					PropChannel item = new PropChannel(GenerateNewChannelName(form.TemplateName, i));
					//Channels.Add(item);
					//listBox1.Items.Add(item);
				}
			}
		}

		private void toolStripMenuItem_Rename_Click(object sender, EventArgs e)
		{
			//var prop = listBox1.SelectedItem as PropChannel;
			//if (prop != null)
			//{
			//	ChannelNaming form = new ChannelNaming();
			//	form.Value = prop.Text;
			//	var result = form.ShowDialog();
			//	if (result == DialogResult.OK)
			//	{
			//		prop.Text = form.Value;
			//	}
			//}
		}

		private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
		{
			//var prop =  listBox1.SelectedItem as PropChannel;
			//if (prop != null)
			//	RemoveChannel(prop.ID);
		}


		#endregion



		private void btnSave_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.textBox1.Text))
			{
				_prop.Name = this.textBox1.Text;
				SaveProp(_prop.Name);
				this.Close();
			}
		}



		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			PropChannel item = null;// listBox1.SelectedItem as PropChannel;
			if (item == null)
			{
				_prop.SelectedChannelId = null;
				_prop.SelectedChannelName = null;
			}
			else
			{
				_prop.SelectedChannelId = item.Id;
				_prop.SelectedChannelName = item.Name;
			}
		}

		private void CustomPropForm_Load(object sender, EventArgs e)
		{
			this.trkImageOpacity.Value = 100;
			this.textBox1.Text = "[!Rename Me!]";
			if (_fileName == null)
			{
				_prop = new Prop(100, 100);
				this.txtBackgroundImage.Text = null;
				this.trkImageOpacity.Value = 100;

			}
			else
			{
				var prop = Prop.FromFile(_fileName);


				if (prop != null)
				{
					_prop = new Prop(prop);

					this.textBox1.Text = _prop.Name;


					this.BackgroundImageFileName = _prop.BackgroundImage;
					this.trkImageOpacity.Value = _prop.BackgroundImageOpacity;
				}

			}
			SetBackGroundImage();

			PopulateNodeTreeMultiSelect();

		}
		private void PopulateNodeTreeMultiSelect()
		{
			treeViewChannels.BeginUpdate();
			treeViewChannels.Nodes.Clear();

			foreach (var channel in this.Channels)
			{
				AddNodeToTree(treeViewChannels.Nodes, channel);
			}

			treeViewChannels.EndUpdate();
		}

		private void AddNodeToTree(TreeNodeCollection collection, PropChannel channel)
		{
			TreeNode addedNode = SetTreeNodeValues(channel);

			collection.Add(addedNode);
			if (channel.Children != null)
				foreach (PropChannel childNode in channel.Children)
				{
					AddNodeToTree(addedNode.Nodes, childNode);
				}
		}

		private static TreeNode SetTreeNodeValues(PropChannel channel, TreeNode addedNode = null)
		{
			if (addedNode == null)
				addedNode = new TreeNode();

			addedNode.Name = channel.Id.ToString();
			addedNode.Text = channel.Name;
			addedNode.Tag = channel;

			return addedNode;
		}


		private void contextMenuChannels_Opening(object sender, CancelEventArgs e)
		{
			this.toolStripMenuItem_Rename.Visible = this.toolStripMenuItem_Rename.Visible = true;

			//if (this.listBox1.SelectedItems.Count != 1)
			//{
			//	this.toolStripMenuItem_Rename.Visible = this.toolStripMenuItem_Rename.Visible = false;
			//}
		}


		private void btnOpen_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.AddExtension = true;
				dlg.DefaultExt = ".prop";
				dlg.InitialDirectory = PropDirectory;
				var results = dlg.ShowDialog();
				if (results == System.Windows.Forms.DialogResult.OK)
				{


					var prop = Prop.FromFile(dlg.FileName);

					if (prop != null)
					{
						_prop = new Prop(prop);

						this.textBox1.Text = _prop.Name;
						//listBox1.Items.Clear();
						//listBox1.Items.AddRange(_prop.Channels.OrderBy(o => o.ID).ToArray());
					}

				}
			}
		}



		public static float NewFontSize(Graphics graphics, Size size, Font font, string str)
		{
			SizeF stringSize = graphics.MeasureString(str, font);
			float wRatio = (size.Width) / stringSize.Width;
			float hRatio = (size.Height) / stringSize.Height;
			float ratio = Math.Min(hRatio, wRatio);
			return font.Size * ratio;
		}




		private void btnLoadBackgroundImage_Click(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.CheckFileExists = true;
				dlg.Filter = GetImageFilter();
				var result = dlg.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					this.BackgroundImageFileName = dlg.FileName;
					SetBackGroundImage();
				}
			}
		}


		public static Image ChangeOpacity(Image image, float value)
		{
			//Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
			//using (Graphics graphics = Graphics.FromImage(bmp))
			//{
			//	ColorMatrix colormatrix = new ColorMatrix { Matrix33 = opacityvalue };
			//	ImageAttributes imgAttribute = new ImageAttributes();
			//	imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			//	graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
			//}

			Image img = image;


			float sb = (float)value / 255F;

			float[][] colorMatrixElements =
				  {
                        new float[] {sb,  0,  0,  0, 0},
                        new float[] {0,  sb,  0,  0, 0},
                        new float[] {0,  0,  sb,  0, 0},
                        new float[] {0,  0,  0,  1, 0},
                        new float[] {0,  0,  0,  0, 1}

                  };

			ColorMatrix cm = new ColorMatrix(colorMatrixElements);

			using (ImageAttributes imgattr = new ImageAttributes())
			{
				Rectangle rc = new Rectangle(0, 0, img.Width, img.Height);
				using (Graphics g = Graphics.FromImage(img))
				{
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					imgattr.SetColorMatrix(cm);
					g.DrawImage(img, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgattr);
				}
			}


			return img;
		}
		public static Color getDominantColor(Bitmap bmp)
		{
			//Used for tally
			int r = 0;
			int g = 0;
			int b = 0;

			int total = 0;

			for (int x = 0; x < bmp.Width; x++)
			{
				for (int y = 0; y < bmp.Height; y++)
				{
					Color clr = bmp.GetPixel(x, y);
					r += clr.R;
					g += clr.G;
					b += clr.B;
					total++;
				}
			}

			//Calculate average
			r /= total;
			g /= total;
			b /= total;

			return Color.FromArgb(r, g, b);
		}
		public string GetImageFilter()
		{
			StringBuilder allImageExtensions = new StringBuilder();
			string separator = "";
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			Dictionary<string, string> images = new Dictionary<string, string>();
			foreach (ImageCodecInfo codec in codecs)
			{
				allImageExtensions.Append(separator);
				allImageExtensions.Append(codec.FilenameExtension);
				separator = ";";
				images.Add(string.Format("{0} Files: ({1})", codec.FormatDescription, codec.FilenameExtension),
						   codec.FilenameExtension);
			}
			StringBuilder sb = new StringBuilder();
			if (allImageExtensions.Length > 0)
			{
				sb.AppendFormat("{0}|{1}", "All Images", allImageExtensions.ToString());
			}
			images.Add("All Files", "*.*");
			foreach (KeyValuePair<string, string> image in images)
			{
				sb.AppendFormat("|{0}|{1}", image.Key, image.Value);
			}
			return sb.ToString();
		}



		private void trkImageOpacity_ValueChanged(object sender, EventArgs e)
		{

			BackgroundImageOpacity = trkImageOpacity.Value;
			SetBackGroundImage();
		}



		private void gridPanel_MouseDown(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					//Draw point
					if ((Control.ModifierKeys & Keys.Control) != Keys.None)
					{
						if (treeViewChannels.SelectedNode != null)
						{
							var prop = treeViewChannels.SelectedNode.Tag as PropChannel;
							if (prop == null) return;
							var pixelSize = prop.PixelSize;
							if (!prop.Points.Any(a => a.X == e.Location.X && a.Y == e.Location.Y)) //Add logic to ensure we dont overlap any points for this channel
							{
								prop.Points.Add(new PreviewPixel(e.Location.X, e.Location.Y, 0, prop.PixelSize));
							}
							DrawPoints(prop);
						}
					}
					break;
				case MouseButtons.Middle:
					break;

				case MouseButtons.Right:
					break;
				default:
					break;
			}
		}

		void DrawPoints()
		{
			foreach (TreeNode item in treeViewChannels.Nodes)
			{
				var prop = item.Tag as PropChannel;
				DrawPoints(prop);
			}
		}
		void DrawPoints(PropChannel channel)
		{
			bool isSelected = channel == treeViewChannels.SelectedNode.Tag as PropChannel;
			foreach (var point in channel.Points)
			{
				DrawCircle(this.gridPanel, point, channel.PixelSize, isSelected ? Color.Yellow : Color.White);
			}
			foreach (var item in channel.Children)
			{
				DrawPoints(item);
			}
		}

		private void treeViewChannels_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid.SelectedObject = e.Node.Tag as PropChannel;
		}

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			var channel = treeViewChannels.SelectedNode.Tag as PropChannel;
			if (channel != null)
				treeViewChannels.SelectedNode = SetTreeNodeValues(channel, treeViewChannels.SelectedNode);
		}

		private static void DrawGrid(Control control, int xSize, int ySize, Color backgroundColor)
		{
			if (control == null) return;
			if (control.BackgroundImage == null)
				control.BackgroundImage = new Bitmap(control.Width, control.Height);

			using (var g = Graphics.FromImage(control.BackgroundImage))
			{

				int cellSizeX = control.BackgroundImage.Width / xSize;
				int cellSizeY = control.BackgroundImage.Height / ySize;

				var r = new Rectangle(new Point(control.Left, control.Top), control.BackgroundImage.Size);
				System.Windows.Forms.ControlPaint.DrawGrid(g, r, new Size(cellSizeX, cellSizeY), backgroundColor);

			}

		}

		private static void DrawCircle(Control control, PreviewPixel pixel, int size, Color color)
		{
			using (var g = Graphics.FromImage(control.BackgroundImage))
			{

				using (Brush b = new SolidBrush(color))
				{
					using (Pen p = new Pen(b))
					{
						g.DrawEllipse(p, pixel.X, pixel.Y, size, size);
						g.FillEllipse(b, new Rectangle(new Point(pixel.X, pixel.Y), new Size(2, 2)));
					}
				}
			}
		}

		private static Color GetDominantColor(Image image)
		{
			if (image == null) return Color.White;
			//Used for tally
			int r = 0;
			int g = 0;
			int b = 0;

			int total = 0;
			using (Bitmap bmp = new Bitmap(image))
			{
				for (int x = 0; x < bmp.Width; x++)
				{
					for (int y = 0; y < bmp.Height; y++)
					{
						Color clr = bmp.GetPixel(x, y);
						r += clr.R;
						g += clr.G;
						b += clr.B;
						total++;
					}
				}
			}

			//Calculate average
			r /= total;
			g /= total;
			b /= total;

			return Color.FromArgb(r, g, b);
		}
		private static Image ResizeImage(Image image, int maximumWidth, int maximumHeight, bool enforceRatio, bool addPadding)
		{

			var imageEncoders = ImageCodecInfo.GetImageEncoders();
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
			var canvasWidth = maximumWidth;
			var canvasHeight = maximumHeight;
			var newImageWidth = maximumWidth;
			var newImageHeight = maximumHeight;
			var xPosition = 0;
			var yPosition = 0;


			if (enforceRatio)
			{
				var ratioX = maximumWidth / (double)image.Width;
				var ratioY = maximumHeight / (double)image.Height;
				var ratio = ratioX < ratioY ? ratioX : ratioY;
				newImageHeight = (int)(image.Height * ratio);
				newImageWidth = (int)(image.Width * ratio);

				if (addPadding)
				{
					xPosition = (int)((maximumWidth - (image.Width * ratio)) / 2);
					yPosition = (int)((maximumHeight - (image.Height * ratio)) / 2);
				}
				else
				{
					canvasWidth = newImageWidth;
					canvasHeight = newImageHeight;
				}
			}

			var thumbnail = new Bitmap(canvasWidth, canvasHeight);

			using (var graphic = Graphics.FromImage(thumbnail))
			{

				if (enforceRatio && addPadding)
				{
					graphic.Clear(Color.White);
				}

				graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphic.SmoothingMode = SmoothingMode.HighQuality;
				graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphic.CompositingQuality = CompositingQuality.HighQuality;
				graphic.DrawImage(image, xPosition, yPosition, newImageWidth, newImageHeight);
			}

			return thumbnail;
		}

		private static void SetGridBackground(Control control, Control control2, string fileName, int opacity = 100, bool maintainAspectRatio = true)
		{
			//control.BackColor = Color.Black;

			if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
			{
				var image = Image.FromFile(fileName);

				control.BackgroundImage = ChangeOpacity(ResizeImage(image, control.Width, control.Height, maintainAspectRatio, false), opacity);

				control2.BackgroundImageLayout = control.BackgroundImageLayout = maintainAspectRatio ? ImageLayout.Center : ImageLayout.Stretch;

			}
			else
			{

				control2.BackgroundImageLayout = ImageLayout.Stretch;
				control.BackgroundImage = null;
			}
			control2.BackColor = Color.Transparent;
		}

		private void SetBackGroundImage()
		{
			SetGridBackground(this.splitContainer1.Panel1, this.gridPanel, this.BackgroundImageFileName, this.BackgroundImageOpacity, BackgroundImageMaintainAspect);


			DrawGrid(this.gridPanel, 100, 100, File.Exists(this.BackgroundImageFileName) ? GetDominantColor(this.gridPanel.BackgroundImage) : this.splitContainer1.Panel1.BackColor);

		}

		private void chkMaintainAspect_CheckedChanged(object sender, EventArgs e)
		{
			SetBackGroundImage();
		}



	}
}
