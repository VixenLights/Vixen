
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
			set { _prop.Channels = value; }
		}
		public int MaxChannelID
		{
			get
			{
				int maxID = 0;
				foreach (PropChannel item in listBox1.Items)
				{
					if (item.ID > maxID) maxID = item.ID;

				}
				return maxID;
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
			foreach (PropChannel item in listBox1.Items)
			{
				names.Add(item.Text);
			}
			while (names.Contains(channelName))
			{
				channelName = string.Format("Channel_{0}", i++);
			}

			return channelName;
		}

		int _currentRowHeight = -1;

		private void ResizeRows()
		{
			var rowHeight = (double)dataGridPropView.Parent.Height / (double)dataGridPropView.Rows.Count;

			if (_currentRowHeight != rowHeight)
			{
				foreach (DataGridViewRow row in dataGridPropView.Rows)
				{
					row.Height = (int)rowHeight;
				}
				_currentRowHeight = (int)rowHeight;
			}
		}

		#endregion


		#region  Context Menu
		private void toolStripMenuItem_Add_Click(object sender, EventArgs e)
		{
			ChannelNaming form = new ChannelNaming();
			form.Value = GenerateNewChannelName();
			var result = form.ShowDialog();
			if (result == DialogResult.OK)
			{
				PropChannel item = new PropChannel(form.Value, MaxChannelID);
				Channels.Add(item);
				listBox1.Items.Add(item);
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

					PropChannel item = new PropChannel(GenerateNewChannelName(form.TemplateName, i), MaxChannelID);
					Channels.Add(item);
					listBox1.Items.Add(item);
				}
			}
		}

		private void toolStripMenuItem_Rename_Click(object sender, EventArgs e)
		{
			var prop = listBox1.SelectedItem as PropChannel;
			if (prop != null)
			{
				ChannelNaming form = new ChannelNaming();
				form.Value = prop.Text;
				var result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					prop.Text = form.Value;
				}
			}
		}

		private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
		{
			var prop = listBox1.SelectedItem as PropChannel;
			if (prop != null)
				RemoveChannel(prop.ID);
		}

		private void RemoveChannel(int id, bool renameOnly = false)
		{
			var channel = Channels.Where(s => s.ID == id).FirstOrDefault();
			if (!renameOnly)
			{
				Channels.Remove(channel);
			}

			Channels.Where(r => r.ID == id + 1).ToList().ForEach(c => c.ID = id);
		 
	 

		}
	 

		private void changeChannelColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PropChannel item = listBox1.SelectedItem as PropChannel;
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
			_prop.Name = this.textBox1.Text;
			SaveProp(_prop.Name);
		}

		private void btnUpdateChannelCount_Click(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			PropChannel item = listBox1.SelectedItem as PropChannel;
			if (item == null)
			{
				_prop.SelectedChannelId = 0;
				_prop.SelectedChannelName = null;
			}
			else
			{
				_prop.SelectedChannelId = item.ID;
				_prop.SelectedChannelName = item.Text;
			}
		}

		private void CustomPropForm_Load(object sender, EventArgs e)
		{
			if (_fileName == null)
			{
				_prop = new Prop(this.dataGridPropView, (int)numGridWidth.Value, (int)numGridHeight.Value);
			}
			else
			{
				var prop = Prop.FromFile(_fileName);


				if (prop != null)
				{
					_prop = new Prop(prop);

					this.textBox1.Text = _prop.Name;
					listBox1.Items.Clear();
					listBox1.Items.AddRange(_prop.Channels.OrderBy(o => o.ID).ToArray());
					this.numGridHeight.Value = _prop.Height;
					this.numGridWidth.Value = _prop.Width;
				}
			}
			dataGridPropView.DataSource = _prop.Data;

			//	dataGridPropView.Font = new System.Drawing.Font("Arial Black", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			dataGridPropView.ForeColor = Color.Black;
			ResizeRows();
		}

		private void contextMenuChannels_Opening(object sender, CancelEventArgs e)
		{
			this.toolStripMenuItem_Rename.Visible = this.toolStripMenuItem_Rename.Visible = true;

			if (this.listBox1.SelectedItems.Count != 1)
			{
				this.toolStripMenuItem_Rename.Visible = this.toolStripMenuItem_Rename.Visible = false;
			}
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
						this.dataGridPropView.DataSource = _prop.Data;
						this.textBox1.Text = _prop.Name;
						listBox1.Items.Clear();
						listBox1.Items.AddRange(_prop.Channels.OrderBy(o => o.ID).ToArray());
					}

				}
			}
		}

	 
		private void panel1_Resize(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

		}


		private void contextMenuGrid_Opening(object sender, CancelEventArgs e)
		{
			if (dataGridPropView.SelectedCells.Count == 0)
			{
				e.Cancel = true;
				return;
			}
			else
			{

				//	bool hasData = false;
				//	bool hasBlank = false;

				//	foreach (DataGridViewCell item in dataGridPropView.SelectedCells)
				//	{
				//		if (item.Value == null) hasBlank = true;
				//		if (item.Value != null) hasData = true;
				//	}
				//	clearToolStripMenuItem.Visible = hasData;
				applyToolStripMenuItem.Visible = _prop.SelectedChannelId > 0;
				//	if (!hasBlank && !hasData) e.Cancel = true; //Something really went wrong here... LOL
			}

		}

		private void applyToolStripMenuItem_Click(object sender, EventArgs e)
		{

			foreach (DataGridViewCell item in dataGridPropView.SelectedCells)
			{
				item.Value = _prop.SelectedChannelId;
			}
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewCell item in dataGridPropView.SelectedCells)
			{
				item.Value = null;
			}
		}

		private void dataGridPropView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			dataGridPropView.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		}

		private void dataGridPropView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		}


		private void dataGridPropView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{

			float fontSize = NewFontSize(e.Graphics, e.CellBounds.Size, e.CellStyle.Font, e.Value as string);
			if (!float.IsInfinity(fontSize))
				e.CellStyle.Font = new Font(e.CellStyle.Font.Name, fontSize, e.CellStyle.Font.Style);

			e.PaintBackground(e.ClipBounds, true);

			e.PaintContent(e.ClipBounds);
			using (Pen p = new Pen(Brushes.Black, 3))
			{
				e.Graphics.DrawLine(p, new Point(e.CellBounds.Left, e.CellBounds.Bottom),
									   new Point(e.CellBounds.Right, e.CellBounds.Bottom));
			}
			using (Pen p = new Pen(Brushes.Black, 3))
			{
				e.Graphics.DrawLine(p, new Point(e.CellBounds.Right, e.CellBounds.Top),
									   new Point(e.CellBounds.Right, e.CellBounds.Bottom));
			}


			e.Handled = true;
		}

		public static float NewFontSize(Graphics graphics, Size size, Font font, string str)
		{
			SizeF stringSize = graphics.MeasureString(str, font);
			float wRatio = (size.Width) / stringSize.Width;
			float hRatio = (size.Height) / stringSize.Height;
			float ratio = Math.Min(hRatio, wRatio);
			return font.Size * ratio;
		}

		private void dataGridPropView_KeyUp(object sender, KeyEventArgs e)
		{
			var grid = sender as DataGridView;
			if (grid == null) return;
			if (e.KeyCode == Keys.Delete)
			{
				if (grid.SelectedCells.Count > 0)
				{
					foreach (DataGridViewCell item in dataGridPropView.SelectedCells)
					{
						item.Value = null;
					}
				}
			}
		}

		private void dataGridPropView_Resize(object sender, EventArgs e)
		{
			ResizeRows();
		}


	}
}
