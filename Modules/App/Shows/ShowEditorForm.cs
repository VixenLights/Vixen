using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VixenModules.App.Shows
{
	public partial class ShowEditorForm : Form
	{
		ShowItemType currentShowItemType;

		public ShowEditorForm(Show show)
		{
			InitializeComponent();
			ShowData = show;
		}

		private void ShowEditorForm_Load(object sender, EventArgs e)
		{
			currentShowItemType = ShowItemType.Startup;
			PopulateActions();
			PopulateItemList(null);
			labelShowName.Text = ShowData.Name;
			LoadCurrentTab();
			CheckButtons();
		}

		public Show ShowData { get; set; }

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			// This is serious shit. The user could use a lot of work. Make sure this is their intent.
			if (MessageBox.Show("You will lose all of your changes to this show. Are you sure you want to cancel?", "Cancel Edit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
			{
				DialogResult = System.Windows.Forms.DialogResult.Cancel;
				Close();
			}
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Show_Editor);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvItem in listViewShowItems.Items)
			{
				ShowItem item = lvItem.Tag as ShowItem;
				item.ItemOrder = lvItem.Index;
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		public void PopulateItemList(ShowItem selectedItem)
		{
			listViewShowItems.Items.Clear();

			ShowData.Items.Sort((item1, item2) => item1.ItemOrder.CompareTo(item2.ItemOrder));

			foreach (ShowItem item in ShowData.Items)
			{
				if (item.ItemType == currentShowItemType)
				{
					AddItemToList(item);
				}
			}
		}

		public ListViewItem AddItemToList(ShowItem item)
		{
			ListViewItem lvItem = new ListViewItem(item.Name);
			lvItem.Tag = item;
			return listViewShowItems.Items.Add(lvItem);
		}

		private void listViewStartup_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewShowItems.SelectedItems.Count > 0)
			{
				// There can be only one!
				ListViewItem lvItem = listViewShowItems.SelectedItems[0];
				ShowItem item = lvItem.Tag as ShowItem;
				//currentItem = item;
			}
		}

		private ActionType ActionStringToAction(string actionString)
		{
			foreach (ActionType action in Enum.GetValues(typeof(ActionType)))
			{
				if (actionString == action.ToString())
					return action;
			}
			return ActionType.Sequence;
		}

		UserControl currentEditor = null;
		private void SetCurrentEditor(string action)
		{
			if (action == "")
			{
				if (currentEditor != null)
				{
					groupBoxItemEdit.Controls.Remove(currentEditor);
				}
			}
			else
			{
				if (currentEditor != null)
				{
					groupBoxItemEdit.Controls.Remove(currentEditor);
				}
				if (SelectedShowItem != null)
				{
					SelectedShowItem.Action = ActionStringToAction(action);
					currentEditor = SelectedShowItem.Editor;
					currentEditor.Location = new Point(4, 16);
					currentEditor.Width = groupBoxItemEdit.Width - (currentEditor.Left * 2);
					groupBoxItemEdit.Controls.Add(currentEditor);
				}
				else
				{
					MessageBox.Show("SetCurrentEditor: SelectedShowItem == null");
				}
			}
		}

		private void PopulateActions()
		{
			foreach (ActionType action in Enum.GetValues(typeof(ActionType)))
			{
				comboBoxActions.Items.Add(action.ToString());
			}
		}

		private ShowItem SelectedShowItem
		{
			get
			{
				ShowItem item = null;
				if (listViewShowItems != null)
				{
					if (listViewShowItems.SelectedItems.Count > 0)
					{
						item = listViewShowItems.SelectedItems[0].Tag as ShowItem;
						//}
						//else
						//{
						//    MessageBox.Show("SelectedShowItem: currentListView.SelectedItems.Count <= 0");
					}
				}
				else
				{
					//MessageBox.Show("SelectedShowItem: currentListView == null");
				}
				return item;
			}
		}

		private void comboBoxActions_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetCurrentEditor(comboBoxActions.SelectedItem.ToString());
		}

		private void tabControlShowItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadCurrentTab();
		}

		private void LoadCurrentTab()
		{
			if (tabControlShowItems.SelectedTab == tabPageStartup)
			{
				currentShowItemType = ShowItemType.Startup;
			}
			else if (tabControlShowItems.SelectedTab == tabPageBackground)
			{
				currentShowItemType = ShowItemType.Background;
			}
			else if (tabControlShowItems.SelectedTab == tabPageSequential)
			{
				currentShowItemType = ShowItemType.Sequential;
			}
			else if (tabControlShowItems.SelectedTab == tabPageInput)
			{
				currentShowItemType = ShowItemType.Input;
			}
			else if (tabControlShowItems.SelectedTab == tabPageShutdown)
			{
				currentShowItemType = ShowItemType.Shutdown;
			}

			SetHelpLabel();
			LoadSelectedItem();
			SetCurrentEditor("");
			PopulateItemList(null);
			CheckButtons();
		}

		private void CheckButtons()
		{
			buttonMoveItemUp.Enabled = (listViewShowItems.SelectedItems.Count > 0);
			buttonMoveItemUp.Visible = (tabControlShowItems.SelectedTab != tabPageBackground);

			buttonMoveItemDown.Enabled = (listViewShowItems.SelectedItems.Count > 0);
			buttonMoveItemDown.Visible = (tabControlShowItems.SelectedTab != tabPageBackground);

			buttonDeleteItem.Enabled = (listViewShowItems.SelectedItems.Count > 0);
			groupBoxItemEdit.Enabled = (listViewShowItems.SelectedItems.Count > 0);
			groupBoxAction.Enabled = (listViewShowItems.SelectedItems.Count > 0);
		}

		private void SetHelpLabel()
		{
			if (tabControlShowItems.SelectedTab != null)
				labelHelp.Text = tabControlShowItems.SelectedTab.Tag as String;
		}

		private void buttonAddItem_Click(object sender, EventArgs e)
		{
			ShowItem item = ShowData.AddItem(currentShowItemType, "New Item");
			item.ItemType = currentShowItemType;
			item.Action = ActionType.Sequence;
			ListViewItem lvItem = AddItemToList(item);
			lvItem.Selected = true;
		}

		private void buttonDeleteItem_Click(object sender, EventArgs e)
		{
			DeleteSelectedItem();
		}

		private void listViewShowItems_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label != null && e.Label.Length > 0)
			{
				(listViewShowItems.Items[e.Item].Tag as ShowItem).Name = e.Label;
			}
		}

		private void listViewShowItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadSelectedItem();
		}

		private void LoadSelectedItem()
		{
			if (SelectedShowItem != null)
			{
				comboBoxActions.SelectedItem = SelectedShowItem.Action.ToString();
				SetCurrentEditor(SelectedShowItem.Action.ToString());
				//}
				//else
				//{
				//    MessageBox.Show("LoadSelectedItem: SelectedShowItem == null");
			}
			CheckButtons();
		}

		private void DeleteSelectedItem()
		{
			if (SelectedShowItem != null)
			{
				ShowData.DeleteItem(SelectedShowItem);
				listViewShowItems.Items.RemoveAt(listViewShowItems.SelectedIndices[0]);
			}
			CheckButtons();
		}

		private void buttonMoveItemUp_Click(object sender, EventArgs e)
		{
			if (listViewShowItems.SelectedItems.Count > 0)
			{
				MoveSelectedItem(listViewShowItems, listViewShowItems.SelectedIndices[0], true);
			}
		}

		private void buttonMoveItemDown_Click(object sender, EventArgs e)
		{
			if (listViewShowItems.SelectedItems.Count > 0)
			{
				MoveSelectedItem(listViewShowItems, listViewShowItems.SelectedIndices[0], false);
			}
		}

		// Based upon http://www.knowdotnet.com/articles/listviewmoveitem.html
		public static void MoveSelectedItem(System.Windows.Forms.ListView lv, int idx, bool moveUp)
		{
			// Gotta have >1 item in order to move
			if (lv.Items.Count > 1)
			{
				int offset = 0;
				if (idx >= 0)
				{
					if (moveUp)
					{
						// ignore moveup of row(0)
						offset = -1;
					}
					else
					{
						// ignore movedown of last item
						if (idx < (lv.Items.Count - 1))
							offset = 1;
					}
				}

				if (offset != 0)
				{
					lv.BeginUpdate();

					int selitem = idx + offset;
					if (selitem >= 0)
					{
						for (int i = 0; i < lv.Items[idx].SubItems.Count; i++)
						{
							string cache = lv.Items[selitem].SubItems[i].Text;
							lv.Items[selitem].SubItems[i].Text = lv.Items[idx].SubItems[i].Text;
							lv.Items[idx].SubItems[i].Text = cache;
						}

						var tagIdx = lv.Items[selitem].Tag;
						var tagSel = lv.Items[idx].Tag;
						lv.Items[selitem].Tag = tagSel;
						lv.Items[idx].Tag = tagIdx;

						lv.Focus();
						lv.Items[selitem].Selected = true;
						lv.EnsureVisible(selitem);
					}
					lv.EndUpdate();
				}
			}
		}

		private void listViewShowItems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			//Console.WriteLine("e.Item: " + e.Item + " " + e.Item.Selected);
			if (!e.Item.Selected)
			{
				ShowItem item = e.Item.Tag as ShowItem;
				e.Item.SubItems[0].Text = item.Name;
			}
		}
	}
}
