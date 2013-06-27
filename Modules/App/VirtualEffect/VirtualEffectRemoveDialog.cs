using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.VirtualEffect
{
	public partial class VirtualEffectRemoveDialog : Form
	{
		public VirtualEffectRemoveDialog(VirtualEffectLibrary library)
		{
			InitializeComponent();
			foreach (KeyValuePair<Guid, VirtualEffect> pair in library) {
				ListViewItem lvItem = new ListViewItem(pair.Value.Name);
				lvItem.SubItems.Add(pair.Key.ToString());
				listViewVirtualEffects.Items.Add(lvItem);
			}
		}

		private void listViewVirtualEffects_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (listViewVirtualEffects.SelectedIndices.Count == 0) {
				buttonRemove.Enabled = false;
			}
			else {
				buttonRemove.Enabled = true;
			}
		}

		private List<Guid> getRemoveList()
		{
			List<Guid> itemGuids = new List<Guid>();
			foreach (ListViewItem lvItem in listViewVirtualEffects.SelectedItems) {
				itemGuids.Add(new Guid(lvItem.SubItems[1].Text));
			}
			return itemGuids;
		}

		public List<Guid> virtualEffectsToRemove
		{
			get { return getRemoveList(); }
		}
	}
}