using System;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewCustomSetupControl : DisplayItemBaseControl
	{
		public PreviewCustomSetupControl(PreviewLightBaseShape shape) : base(shape)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			comboBoxStringToEdit.ForeColor = ThemeColorTable.ForeColor;

			foreach (PreviewLightBaseShape stringShape in ((PreviewLightBaseShape)Shape)._strings)
			{
				stringShape.OnPropertiesChanged += OnPropertiesChanged;
			}
		
		}

		~PreviewCustomSetupControl()
		{
			foreach (PreviewLightBaseShape stringShape in ((PreviewLightBaseShape)Shape)._strings)
			{
				stringShape.OnPropertiesChanged -= OnPropertiesChanged;
			}		
		}

		private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
		{
			PopulatePropList((comboBoxStringToEdit.SelectedItem as Common.Controls.ComboBoxItem).Value as PreviewLightBaseShape);
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_CustomShape);
		}

		private void PreviewCustomSetupControl_Load(object sender, EventArgs e)
		{
			PopulatePropList();
		}

		private void PopulatePropList(PreviewLightBaseShape selectedShape = null)
		{
			comboBoxStringToEdit.Items.Clear();

			foreach (PreviewLightBaseShape shape in ((PreviewLightBaseShape)Shape)._strings) 
			{
				Common.Controls.ComboBoxItem item = new Common.Controls.ComboBoxItem(shape.Name, shape);
				if (item.Text == null) {
					item.Text = shape.GetType().ToString();
					item.Text = item.Text.Substring(item.Text.LastIndexOf('.') + 1);
				}
				if (item.Text == "")
					item.Text = "Unnamed String";
				comboBoxStringToEdit.Items.Add(item);
			}

			if (comboBoxStringToEdit.Items.Count > 0) {
				if (selectedShape != null) {
					foreach (Common.Controls.ComboBoxItem item in comboBoxStringToEdit.Items)
					{
						if (item.Text == "")
							item.Text = "Unnamed String";
						if ((item.Value as PreviewLightBaseShape) == selectedShape) {
							comboBoxStringToEdit.SelectedItem = item;
							return;
						}
					}
				}
				else {
					comboBoxStringToEdit.SelectedIndex = 0;
				}
			}
			comboBoxStringToEdit.ForeColor = ThemeColorTable.ForeColor;
		}

		public void ShowSetupControl(PreviewLightBaseShape shape)
		{
			panelProperties.Controls.Clear();
			DisplayItemBaseControl setupControl = shape.GetSetupControl();
			if (setupControl != null) {
				panelProperties.Controls.Add(setupControl);
				//setupControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
				setupControl.Dock = DockStyle.Fill;
			}
		}

		private void comboBoxStringToEdit_SelectedIndexChanged(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem item = comboBoxStringToEdit.SelectedItem as Common.Controls.ComboBoxItem;
			if (item != null)
			{
				if (item.Text == "")
					item.Text = "Unnamed String";
				PreviewLightBaseShape shape = item.Value as PreviewLightBaseShape;
				if (shape != null) {
					ShowSetupControl(shape);
				}
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void btnSyncBulbSize_Click(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem item = comboBoxStringToEdit.SelectedItem as Common.Controls.ComboBoxItem;
			if (item != null)
			{
				PreviewLightBaseShape shape = item.Value as PreviewLightBaseShape;
				if (shape != null)
				{
					var size = shape.PixelSize;
					foreach (PreviewLightBaseShape s in ((PreviewLightBaseShape)Shape)._strings)
					{
						s.PixelSize = size;
					}
				}
			}
		}
	}
}