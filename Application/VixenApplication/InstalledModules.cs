using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module;

namespace VixenApplication
{
	public partial class InstalledModules : BaseForm
	{
		private const string NOT_PROVIDED = "(Not Provided)";

		public InstalledModules()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			listViewModules.AllowRowReorder = false;
		}

		private void InstalledModules_Load(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			try {
				string[] moduleTypes = ApplicationServices.GetTypesOfModules().OrderBy(x => x).ToArray();
				foreach (string moduleType in moduleTypes) {
					ListViewGroup group = listViewModules.Groups.Add(moduleType, moduleType);
					IModuleDescriptor[] descriptors = ApplicationServices.GetModuleDescriptors(moduleType);
					foreach (IModuleDescriptor descriptor in descriptors) {
						ListViewItem item =
							new ListViewItem(new[]
							                 	{
							                 		descriptor.TypeName, _GetModuleDescription(descriptor), _GetModuleAuthor(descriptor),
							                 		_GetModuleVersion(descriptor), descriptor.FileName
							                 	});
						item.Tag = descriptor;
						item.Group = group;
						listViewModules.Items.Add(item);
					}
				}

				listViewModules.ColumnAutoSize();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			finally {
				Cursor = Cursors.Default;
			}
		}

		private string _GetModuleDescription(IModuleDescriptor descriptor)
		{
			try {
				return descriptor.Description;
			}
			catch {
				return NOT_PROVIDED;
			}
		}

		private string _GetModuleAuthor(IModuleDescriptor descriptor)
		{
			try {
				return descriptor.Author;
			}
			catch {
				return NOT_PROVIDED;
			}
		}

		private string _GetModuleVersion(IModuleDescriptor descriptor)
		{
			try {
				return descriptor.Version;
			}
			catch {
				return NOT_PROVIDED;
			}
		}

		private void listViewModules_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonCopyModuleId.Enabled = listViewModules.SelectedItems.Count > 0;
		}

		private void buttonCopyModuleId_Click(object sender, EventArgs e)
		{
			IModuleDescriptor descriptor = listViewModules.SelectedItems[0].Tag as IModuleDescriptor;
			Clipboard.SetText(descriptor.TypeId.ToString());
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