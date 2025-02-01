using System.IO;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewCustomCreateForm : BaseForm
	{
		public PreviewCustomCreateForm()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		public string TemplateName
		{
			get { return textBoxTemplateName.Text; }
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (TemplateName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("The template name must be a valid file name. Please ensure there are no invalid characters in the template name.",
					"Invalid Template Name", false, true);
				messageBox.ShowDialog();
				DialogResult = messageBox.DialogResult;
			}
			else {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}