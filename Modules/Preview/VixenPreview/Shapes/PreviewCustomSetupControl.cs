using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewCustomSetupControl : DisplayItemBaseControl
	{
		public PreviewCustomSetupControl(PreviewBaseShape shape) : base(shape)
		{
			InitializeComponent();
			foreach (PreviewBaseShape stringShape in Shape._strings) {
				stringShape.OnPropertiesChanged += OnPropertiesChanged;
			}
		}

		~PreviewCustomSetupControl()
		{
			foreach (PreviewBaseShape stringShape in Shape._strings) {
				stringShape.OnPropertiesChanged -= OnPropertiesChanged;
			}
		}

		private void OnPropertiesChanged(object sender, PreviewBaseShape shape)
		{
			PopulatePropList((comboBoxStringToEdit.SelectedItem as Common.Controls.ComboBoxItem).Value as PreviewBaseShape);
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_CustomShape);
		}

		private void PreviewCustomSetupControl_Load(object sender, EventArgs e)
		{
			PopulatePropList();
		}

		private void PopulatePropList(PreviewBaseShape selectedShape = null)
		{
			comboBoxStringToEdit.Items.Clear();
			foreach (PreviewBaseShape shape in Shape._strings) {
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
						if ((item.Value as PreviewBaseShape) == selectedShape) {
							comboBoxStringToEdit.SelectedItem = item;
							return;
						}
					}
				}
				else {
					comboBoxStringToEdit.SelectedIndex = 0;
				}
			}
		}

		public void ShowSetupControl(PreviewBaseShape shape)
		{
			panelProperties.Controls.Clear();
			Shapes.DisplayItemBaseControl setupControl = shape.GetSetupControl();
			if (setupControl != null) {
				panelProperties.Controls.Add(setupControl);
				setupControl.Dock = DockStyle.Fill;
			}
		}

		private void comboBoxStringToEdit_SelectedIndexChanged(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem item = comboBoxStringToEdit.SelectedItem as Common.Controls.ComboBoxItem;
			if (item != null) {
				if (item.Text == "")
					item.Text = "Unnamed String";
				PreviewBaseShape shape = item.Value as PreviewBaseShape;
				if (shape != null) {
					ShowSetupControl(shape);
				}
			}
		}
	}
}