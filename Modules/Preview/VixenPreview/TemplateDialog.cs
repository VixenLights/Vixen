using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview
{
	public partial class TemplateDialog : BaseForm
	{
		public TemplateDialog()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		public string FileName { get; private set; }

		private void PopulateTemplateList()
		{
			TemplateComboBoxItem selectedTemplateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			comboBoxTemplates.Items.Clear();

			IEnumerable<string> files = System.IO.Directory.EnumerateFiles(PreviewTools.TemplateFolder, "*.xml");
			foreach (string file in files)
			{
				string fileName = PreviewTools.TemplateWithFolder(file);
				try
				{
					// Read the entire template file (stoopid waste of resources, but how else?)
					string xml = System.IO.File.ReadAllText(fileName);
					DisplayItem newDisplayItem = PreviewTools.DeSerializeToDisplayItem(xml, typeof(DisplayItem));
					TemplateComboBoxItem newTemplateItem = new TemplateComboBoxItem(newDisplayItem.Shape.Name, fileName);
					comboBoxTemplates.Items.Add(newTemplateItem);
				}
				catch (Exception ex)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There was an error loading the template file (" + file + "): " + ex.Message,
						"Error Loading Template", false, true);
					messageBox.ShowDialog();
				}
				finally
				{
					if (selectedTemplateItem != null && comboBoxTemplates.Items.IndexOf(selectedTemplateItem) >= 0)
					{
						comboBoxTemplates.SelectedItem = selectedTemplateItem;
					}
					if (comboBoxTemplates.SelectedItem == null && comboBoxTemplates.Items.Count > 0)
					{
						comboBoxTemplates.SelectedIndex = 0;
					}
				}
			}

		}

		private void buttonDeleteTemplate_Click(object sender, EventArgs e)
		{
			TemplateComboBoxItem templateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			if (templateItem != null)
			{
				if (System.IO.File.Exists(templateItem.FileName))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Are you sure you want to delete the template '" + templateItem.Caption + "'?", "Delete Template", true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						System.IO.File.Delete(templateItem.FileName);
						PopulateTemplateList();
					}
				}
			}
		}

		private void comboBoxTemplates_SelectedIndexChanged(object sender, EventArgs e)
		{
			TemplateComboBoxItem templateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			FileName = templateItem?.FileName;
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
	}
}
