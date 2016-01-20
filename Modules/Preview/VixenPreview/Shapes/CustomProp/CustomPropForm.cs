using Polenter.Serialization;
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

namespace VixenModules.Preview.VixenPreview.CustomProp
{
	public partial class CustomPropForm : Form
	{
		public CustomPropForm()
		{
			InitializeComponent();
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
			var serializer = new SharpSerializer();

			serializer.Serialize(_prop, fileName);

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

		}

		private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
		{

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

			_prop.SelectedChannelId = item.ID;
			_prop.SelectedChannelName = item.Text;
		}

		private void CustomPropForm_Load(object sender, EventArgs e)
		{
			if (_fileName == null)
			{
				_prop = new Prop(this.panel1, (int)numGridWidth.Value, (int)numGridHeight.Value);
			}
			else
			{
				var serializer = new SharpSerializer();
				var prop = serializer.Deserialize(_fileName) as Prop;

				if (prop != null)
				{
					_prop = new Prop(panel1, prop);
					this.textBox1.Text = _prop.Name;
					listBox1.Items.Clear();
					listBox1.Items.AddRange(_prop.Channels.OrderBy(o => o.ID).ToArray());

				}
			}
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

					var serializer = new SharpSerializer();
					var prop = serializer.Deserialize(dlg.FileName) as Prop;

					if (prop != null)
					{
						_prop = new Prop(panel1, prop);
						this.textBox1.Text = _prop.Name;
						listBox1.Items.Clear();
						listBox1.Items.AddRange(_prop.Channels.OrderBy(o => o.ID).ToArray());
					}

				}
			}
		}

		private void numGridHeight_ValueChanged(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

		}

		private void panel1_Resize(object sender, EventArgs e)
		{
			_prop.UpdateGrid((int)numGridHeight.Value, (int)numGridWidth.Value);

		}




	}
}
