using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Vixen.Module.Script;

namespace VixenModules.Editor.ScriptEditor {
	public partial class ProjectConfigForm : Form {
		public ProjectConfigForm() {
			InitializeComponent();
		}

		private void ProjectConfigForm_Load(object sender, EventArgs e) {
			IScriptModuleDescriptor[] scriptModuleDescriptors = Vixen.Services.ApplicationServices.GetModuleDescriptors<IScriptModuleInstance>().Cast<IScriptModuleDescriptor>().ToArray();
			comboBoxLanguage.DisplayMember = "LanguageName";
			comboBoxLanguage.ValueMember = null;
			comboBoxLanguage.DataSource = scriptModuleDescriptors;
		}

		public string SelectedProjectName { get; private set; }

		public string SelectedFileName { get; private set; }

		public IScriptModuleInstance SelectedLanguage { get; private set; }

		private IScriptModuleDescriptor _SelectedLanguageDescriptor {
			get { return (IScriptModuleDescriptor)comboBoxLanguage.SelectedValue; }
		}

		private bool _Validate() {
			textBoxFileName.Text = Path.GetFileName(textBoxFileName.Text);

			if(!_IsValidFileName(textBoxProjectName.Text)) {
				MessageBox.Show(this, "Project name is not a valid file name.");
				return false;
			}

			if(!_IsValidFileName(textBoxFileName.Text)) {
				MessageBox.Show(this, "Initial file name is not a valid file name.");
				return false;
			}

			if(_SelectedLanguageDescriptor == null) {
				MessageBox.Show(this, "A language must be selected.");
				return false;
			}

			return true;
		}

		private bool _IsValidFileName(string fileName) {
			Regex regex = new Regex(@"^[^ \\/:*?""<>|]+([ ]+[^ \\/:*?""<>|]+)*$");
			return regex.IsMatch(fileName);
		}

		private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e) {
			if(_IsValidFileName(textBoxFileName.Text) && _SelectedLanguageDescriptor != null) {
				textBoxFileName.Text = Path.ChangeExtension(textBoxFileName.Text, _SelectedLanguageDescriptor.FileExtension);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(!_Validate()) {
				DialogResult = DialogResult.None;
			} else {
				SelectedProjectName = textBoxProjectName.Text;
				SelectedFileName = textBoxFileName.Text;
				SelectedLanguage = Vixen.Services.ApplicationServices.Get<IScriptModuleInstance>(_SelectedLanguageDescriptor.TypeId);
			}
		}
	}
}
