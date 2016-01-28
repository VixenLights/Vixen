
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

		private void changeChannelColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PropChannel item = null;// listBox1.SelectedItem as PropChannel;
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = item.ItemColor;
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{

					item.ItemColor = cp.Color;

				}
			}
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

		private void btnUpdateChannelCount_Click(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

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
				_prop = new Prop((int)numGridWidth.Value, (int)numGridHeight.Value);
				this.txtBackgroundImage.Text = null;
				this.trkImageOpacity.Value = 100;
				SetGridBackground();
			}
			else
			{
				var prop = Prop.FromFile(_fileName);


				if (prop != null)
				{
					_prop = new Prop(prop);

					this.textBox1.Text = _prop.Name;
 					 
					this.numGridHeight.Value = _prop.Height;
					this.numGridWidth.Value = _prop.Width;
					this.txtBackgroundImage.Text = _prop.BackgroundImage;
					this.trkImageOpacity.Value = _prop.BackgroundImageOpacity;
					SetGridBackground(this._prop.BackgroundImage, _prop.BackgroundImageOpacity);
				}
				else
					SetGridBackground();
			}


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

		private void AddNodeToTree(TreeNodeCollection collection, PropChannel elementNode)
		{
			TreeNode addedNode = new TreeNode()
			{
				Name = elementNode.Id.ToString(),
				Text = elementNode.Name,
				Tag = elementNode
			};

			collection.Add(addedNode);
			if (elementNode.Children != null)
			foreach (PropChannel childNode in elementNode.Children)
			{
				AddNodeToTree(addedNode.Nodes, childNode);
			}
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


		private void panel1_Resize(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

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

					this._prop.BackgroundImage = this.txtBackgroundImage.Text = dlg.FileName;

					SetGridBackground(this._prop.BackgroundImage, _prop.BackgroundImageOpacity);

				}
			}
		}

		private void SetGridBackground(string image = "", int opacity = 100)
		{
			splitContainer1.Panel1.BackColor = Color.Black;

			if (!string.IsNullOrWhiteSpace(image) && File.Exists(image))
			{
				splitContainer1.Panel1.BackgroundImage = ChangeOpacity(Image.FromFile(image), opacity);
				splitContainer1.Panel1.BackgroundImageLayout = ImageLayout.Stretch;
			}
			else
			{
				splitContainer1.Panel1.BackgroundImage = null;

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
			_prop.BackgroundImageOpacity = trkImageOpacity.Value;
			SetGridBackground(_prop.BackgroundImage, _prop.BackgroundImageOpacity);
		}

		private void trkImageOpacity_Scroll(object sender, EventArgs e)
		{
			_prop.BackgroundImageOpacity = trkImageOpacity.Value;
			SetGridBackground(_prop.BackgroundImage, _prop.BackgroundImageOpacity);
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void numGridHeight_ValueChanged(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void toolStripSeparator2_Click(object sender, EventArgs e)
		{

		}

		private void toolStripSeparator1_Click(object sender, EventArgs e)
		{

		}

		private void numGridWidth_ValueChanged(object sender, EventArgs e)
		{

		}

		private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}

		private void dataGridPropView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void txtBackgroundImage_TextChanged(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}



	}
}
