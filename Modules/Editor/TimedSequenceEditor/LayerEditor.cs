using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class LayerEditor : DockContent
	{
		private readonly SequenceLayers _layers;
		private ListViewItem.ListViewSubItem _lvEditedItem;
		private ILayer _lvEditedDefinition;
		private readonly TimedSequenceEditorForm _parent;
		private ILayerMixingFilterInstance _defaultFilter;
		
		public LayerEditor(SequenceLayers layers, TimedSequenceEditorForm parent)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			toolStripButtonAddLayer.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonAddLayer.Image = Resources.add;
			toolStripButtonRemoveLayer.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonRemoveLayer.Image = Resources.delete;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_parent = parent;
			toolStripButtonRemoveLayer.Enabled = false;
			lvMixingFilters.DragDrop += LvMixingFilters_DragDrop;
			lvMixingFilters.ItemSelectionChanged += LvMixingFilters_ItemSelectionChanged;
			lvMixingFilters.MouseDoubleClick += LvMixingFilters_MouseDoubleClick;
			_layers = layers;
		}

		
		public event EventHandler<LayerEditorEventArgs> MixingLayerFiltersChanged;
		protected virtual void OnMixingLayerFilterCollectionChanged(LayerEditorEventArgs e)
		{
			if (MixingLayerFiltersChanged != null)
				MixingLayerFiltersChanged(this, e);
		}

		private void LvMixingFilters_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			toolStripButtonRemoveLayer.Enabled = false;
			if (e.ItemIndex == lvMixingFilters.Items.Count - 1)
			{
				//Don't allow selection of default layer
				e.Item.Selected = false;
			}
			else
			{
				if (lvMixingFilters.SelectedItems.Count > 0)
				{
					toolStripButtonRemoveLayer.Enabled = true;
				}
			}
		}

		private void LvMixingFilters_DragDrop(object sender, DragEventArgs e)
		{
			var count = lvMixingFilters.Items.Count;
			for (int i = 0; i < count; i++)
			{
				_layers.ReplaceLayerAt(i, (Layer)lvMixingFilters.Items[count-i-1].Tag);
			}

			OnMixingLayerFilterCollectionChanged(new LayerEditorEventArgs(false));
		}

		private void toolStripButtonAddLayer_Click(object sender, EventArgs e)
		{
			var layer = new StandardLayer(EnsureUniqueName("Default"));
			layer.LayerMixingFilter = _defaultFilter;
			_layers.AddLayer(layer);
			AddLayerItem(layer, 0);
			OnMixingLayerFilterCollectionChanged(new LayerEditorEventArgs(false));
		}

		private void toolStripButtonRemoveLayer_Click(object sender, EventArgs e)
		{
			MessageBoxForm msg = new MessageBoxForm("Are you sure you want to remove this layer?\nAny effects assigned to the layer will be reassigned to the default layer.","Confirm Delete",MessageBoxButtons.OKCancel, SystemIcons.Warning);
			msg.ShowDialog();
			if (msg.DialogResult == DialogResult.OK)
			{
				if (lvMixingFilters.SelectedIndices.Count>0)
				{
					var count = lvMixingFilters.SelectedIndices.Count;
					for (int i = 0; i < count; i++)
					{
						_layers.RemoveLayerAt(_layers.Count - lvMixingFilters.SelectedIndices[i] - 1);
						lvMixingFilters.Items.RemoveAt(lvMixingFilters.SelectedIndices[i]);
					}
					toolStripButtonRemoveLayer.Enabled = false;
					OnMixingLayerFilterCollectionChanged(new LayerEditorEventArgs(true));
				}
			}

		}

		private void MixingFilterEditor_Load(object sender, EventArgs e)
		{
			var descriptors = ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>();
			Dictionary<string, ILayerMixingFilterInstance> filters = descriptors.Select(filterType => ApplicationServices.Get<ILayerMixingFilterInstance>(filterType.TypeId)).ToDictionary(filter => filter.Descriptor.TypeName);
			_defaultFilter = filters.Values.FirstOrDefault();
			cboListViewCombo.DataSource = new BindingSource(filters, null);
			cboListViewCombo.DisplayMember = "Key";
			cboListViewCombo.ValueMember = "Value";

			lvMixingFilters.View = View.Details;
			lvMixingFilters.FullRowSelect = true;

			foreach (var baseMixingLayerDefinition in _layers.GetLayers())
			{
				var item = AddLayerItem(baseMixingLayerDefinition, 0);
				if (baseMixingLayerDefinition.Type == LayerType.Default)
				{
					lvMixingFilters.ReorderExcludeItem(item);
				}
			}

			lvMixingFilters.ColumnAutoSize();
			lvMixingFilters.SetLastColumnWidth();
		}

		private ListViewItem AddLayerItem(ILayer layer, int index)
		{
			var listViewItem = new ListViewItem(layer.LayerName);
			listViewItem.Name = @"Name";
			ListViewItem.ListViewSubItem item = new ListViewItem.ListViewSubItem();
			item.Text = layer.FilterName;
			listViewItem.Tag = layer;
			item.Name = @"Type";
			listViewItem.SubItems.Add(item);
			lvMixingFilters.Items.Insert(index, listViewItem);
			return listViewItem;
		}

		private string EnsureUniqueName(string name)
		{
			if (lvMixingFilters.Items.Cast<ListViewItem>().Any(x => x.Text == name))
			{
				string originalName = name;
				bool unique;
				int counter = 2;
				do
				{
					name = string.Format("{0} - {1}", originalName, counter++);
					unique = lvMixingFilters.Items.Cast<ListViewItem>().All(x => x.Text != name);
				} while (!unique);
			}
			return name;
		}

		private void txtListViewEdit_Leave(object sender, EventArgs e)
		{
			_parent.IgnoreKeyDownEvents = false;
			EditingValueChanged();
		}

		private void cboListViewCombo_SelectedValueChanged(object sender, EventArgs e)
		{
			EditingValueChanged();
		}

		private void cboListViewCombo_Leave(object sender, EventArgs e)
		{
			EditingValueChanged();
		}

		private void EditingValueChanged()
		{
			if (_lvEditedItem != null)
			{
				if (_lvEditedItem.Name.Equals("Name"))
				{
					if (!txtListViewEdit.Text.Equals(_lvEditedItem.Text))
					{
						var name = EnsureUniqueName(txtListViewEdit.Text);
						_lvEditedItem.Text = name;
						_lvEditedDefinition.LayerName = name;
					}
					
					txtListViewEdit.Visible = false;
				}
				else
				{
					_lvEditedItem.Text = ((KeyValuePair<string, ILayerMixingFilterInstance>)cboListViewCombo.SelectedItem).Key;
					_lvEditedDefinition.LayerMixingFilter = ((KeyValuePair<string, ILayerMixingFilterInstance>)cboListViewCombo.SelectedItem).Value;
					cboListViewCombo.Visible = false;
				}
				
				OnMixingLayerFilterCollectionChanged(new LayerEditorEventArgs(false));
			}
			
		}

		private void cboListViewCombo_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Verify that the user presses ESC.
			switch (e.KeyChar)
			{
				case (char) (int) Keys.Escape:
				{
					// Reset the original text value, and then hide the ComboBox.
					cboListViewCombo.Text = _lvEditedItem.Text;
					cboListViewCombo.Visible = false;
					break;
				}

				case (char) (int) Keys.Enter:
				{
					// Hide the ComboBox.
					cboListViewCombo.Visible = false;
					break;
				}
			}
		}

		private void txtListViewEdit_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case (char)(int)Keys.Escape:
					{
						// Reset the original text value, and then hide the TextBox.
						txtListViewEdit.Text = _lvEditedItem.Text;
						txtListViewEdit.Visible = false;
						_parent.IgnoreKeyDownEvents = false;
						break;
					}

				case (char)(int)Keys.Enter:
					{
						// Hide the TextBox.
						txtListViewEdit.Visible = false;
						_parent.IgnoreKeyDownEvents = false;
						EditingValueChanged();
						break;
					}
			}
		}
		
		private void LvMixingFilters_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// Get the item on the row that is clicked.
			var item = lvMixingFilters.GetItemAt(e.X, e.Y);
			if (item == null) return;

			//Check to see if it is a item we don't want to edit.
			if (item.Index == lvMixingFilters.Items.Count-1)
			{
				//can't edit the default one
				return;
			}

			_lvEditedDefinition = (ILayer) item.Tag;
			_lvEditedItem = item.GetSubItemAt(e.X, e.Y);
			
			// Make sure that an item is clicked.
			if (_lvEditedItem != null)
			{
				// Get the bounds of the item that is clicked.
				Rectangle clickedItem = _lvEditedItem.Bounds;

				int colIndex = 0;
				if (_lvEditedItem.Name.Equals("Type"))
				{
					colIndex = 1;
				}

				// Verify that the column is completely scrolled off to the left.
				if ((clickedItem.Left + lvMixingFilters.Columns[colIndex].Width) < 0)
				{
					// If the cell is out of view to the left, do nothing.
					return;
				}

				// Verify that the column is partially scrolled off to the left.
				if (clickedItem.Left < 0)
				{
					// Determine if column extends beyond right side of ListView.
					if ((clickedItem.Left + lvMixingFilters.Columns[colIndex].Width) > lvMixingFilters.Width)
					{
						// Set width of column to match width of ListView.
						clickedItem.Width = lvMixingFilters.Width;
						clickedItem.X = 0;
					}
					else
					{
						// Right side of cell is in view.
						clickedItem.Width = lvMixingFilters.Columns[colIndex].Width + clickedItem.Left;
						clickedItem.X = 2;
					}
				}
				else if (lvMixingFilters.Columns[1].Width > lvMixingFilters.Width)
				{
					clickedItem.Width = lvMixingFilters.Width;
				}
				
				// Adjust the top to account for the location of the ListView.
				clickedItem.Y += lvMixingFilters.Top;
				clickedItem.X += lvMixingFilters.Left;

				//Determine which control to use
				if (_lvEditedItem.Name.Equals("Name"))
				{
					//Use a textbox
					// Assign calculated bounds to the TextBox.
					txtListViewEdit.Bounds = clickedItem;
					// Set default text for TextBox to match the item that is clicked.
					txtListViewEdit.Text = _lvEditedItem.Text;
					//Ensure the top level form does not intercept our keys for editing
					_parent.IgnoreKeyDownEvents = true;
					// Display the TextBox, and make sure that it is on top with focus.
					txtListViewEdit.Visible = true;
					txtListViewEdit.BringToFront();
					txtListViewEdit.Focus();
				}
				else
				{
					// Assign calculated bounds to the ComboBox.
					cboListViewCombo.Bounds = clickedItem;

					// Set default text for ComboBox to match the item that is clicked.
					cboListViewCombo.Text = _lvEditedItem.Text;

					// Display the ComboBox, and make sure that it is on top with focus.
					cboListViewCombo.Visible = true;
					cboListViewCombo.BringToFront();
					cboListViewCombo.Focus();
				}

				
			}
		}

		
	}

	public class LayerEditorEventArgs : EventArgs
	{
		public LayerEditorEventArgs(bool layerDefinitionRemoved)
		{
			LayerDefinitionRemoved = layerDefinitionRemoved;
		}
		public bool LayerDefinitionRemoved { get; private set; }
	}
}
