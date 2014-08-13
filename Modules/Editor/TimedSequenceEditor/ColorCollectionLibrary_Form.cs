using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls;
using VixenModules.Sequence.Timed;
using VixenModules.Property.Color;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class ColorCollectionLibrary_Form : Form
	{
		#region Member Variables

		public List<ColorCollection> ColorCollections { get; set; }
		private Boolean isDirty;
		private ColorCollection CurrentCollection;
		private Color ColorValue;

		#endregion

		#region Initialization

		public ColorCollectionLibrary_Form(List<ColorCollection> Collections)
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
			ColorCollections = Collections;
			PopulateCollectionList();
			isDirty = false;
		}

		#endregion

		#region Event Handlers

		private void btnAddColor_Click(object sender, EventArgs e)
		{
			if (CurrentCollection == null)
				return;
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB(ColorValue);
				cp.StartPosition = FormStartPosition.Manual;
				cp.Top = this.Top;
				cp.Left = this.Left + this.Width;
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{
					ColorValue = cp.Color.ToRGB().ToArgb();
					CurrentCollection.Color.Add(ColorValue);
					isDirty = true;
					PopulateCollectionColors(CurrentCollection);
				}
			}
		}

		private void comboBoxCollections_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentCollection = (ColorCollection)comboBoxCollections.SelectedItem;
			textBoxDescription.Text = CurrentCollection.Description;
			btnDeleteCollection.Enabled = textBoxDescription.Enabled = true;
			PopulateCollectionColors(CurrentCollection);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (isDirty)
			{
				var warn_result = MessageBox.Show("You will loose any changes, do you wish to save them now ?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (warn_result == DialogResult.Yes)
				{
					isDirty = false;
					this.DialogResult = DialogResult.OK;
				}
				if (warn_result == DialogResult.Cancel)
					this.DialogResult = DialogResult.None;
			}
		}

		private void btnRemoveColor_Click(object sender, EventArgs e)
		{
			if (CurrentCollection == null || listViewColors.SelectedItems == null)
				return;

			foreach (ListViewItem ListItem in listViewColors.SelectedItems)
			{
				CurrentCollection.Color.Remove((Color)ListItem.Tag);
				isDirty = true;
			}

			PopulateCollectionColors(CurrentCollection);
		}

		private void btnDeleteCollection_Click(object sender, EventArgs e)
		{
			if (CurrentCollection != null)
			{
				DialogResult result = MessageBox.Show(string.Format("Are you sure you want to delete the collection: {0} ?", CurrentCollection.Name), "Delete collection?", MessageBoxButtons.YesNo);
				if (result == DialogResult.Yes)
				{
					ColorCollections.Remove(CurrentCollection);
					listViewColors.Items.Clear();
					textBoxDescription.Text = "";
					textBoxDescription.Enabled = false;
					isDirty = true;
					PopulateCollectionList();
				}
			}
		}

		private void buttonNewCollection_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Collecton Name?");
			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show("Please enter a name.");
					continue;
				}
				ColorCollection item = new ColorCollection();
				item.Name = dialog.Response;
				if (ColorCollections.Contains(item))
				{
					DialogResult result = MessageBox.Show("There is already a collection with the name " + item.Name + ". Do you want to overwrite it?",
														  "Overwrite collection?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						ColorCollections.Remove(item);
						ColorCollections.Add(item);
						isDirty = true;
						PopulateCollectionList();
						comboBoxCollections.Text = item.Name;
						break;
					}
					else if (result == DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					ColorCollections.Add(item);
					isDirty = true;
					PopulateCollectionList();
					comboBoxCollections.Text = item.Name;
					break;
				}
			}
		}

		private void RandomColorLibrary_Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isDirty)
			{
				var warn_result = MessageBox.Show("You will loose any changes, do you wish to save them now ?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (warn_result == DialogResult.Yes)
					this.DialogResult = DialogResult.OK;
				if (warn_result == DialogResult.Cancel)
					e.Cancel = true;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			isDirty = false;
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnRemoveColor.Enabled = (listViewColors.SelectedItems.Count > 0);
		}

		private void textBoxDescription_KeyUp(object sender, KeyEventArgs e)
		{
			if (CurrentCollection == null)
				return;
			CurrentCollection.Description = textBoxDescription.Text;
			isDirty = true;
		}

		#endregion

		#region Drag/Drop

		private void listViewColors_ItemDrag(object sender, ItemDragEventArgs e)
		{
			listViewColors.DoDragDrop(listViewColors.SelectedItems, DragDropEffects.Move);
		}

		private void listViewColors_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
			{
				e.Effect = DragDropEffects.Move;
			}
		}

		private void listViewColors_DragDrop(object sender, DragEventArgs e)
		{
			listViewColors.Alignment = ListViewAlignment.Default;
			if (listViewColors.SelectedItems.Count == 0)
				return;
			Point p = listViewColors.PointToClient(new Point(e.X, e.Y));
			ListViewItem MovetoNewPosition = listViewColors.GetItemAt(p.X, p.Y);
			if (MovetoNewPosition == null) return;
			ListViewItem DropToNewPosition = (e.Data.GetData(typeof(ListView.SelectedListViewItemCollection)) as ListView.SelectedListViewItemCollection)[0];
			ListViewItem CloneToNew = (ListViewItem)DropToNewPosition.Clone();
			int index = MovetoNewPosition.Index;
			listViewColors.Items.Remove(DropToNewPosition);
			listViewColors.Items.Insert(index, CloneToNew);
			listViewColors.Alignment = ListViewAlignment.SnapToGrid;
			UpdateCollectionColorOrder();
		}

		#endregion

		#region Private Methods

		private void UpdateCollectionColorOrder()
		{
			if (CurrentCollection == null)
				return;
			CurrentCollection.Color.Clear();
			foreach (ListViewItem ListItem in listViewColors.Items)
			{
				CurrentCollection.Color.Add((Color)ListItem.Tag);
			}
			isDirty = true;
		}

		private void PopulateCollectionList()
		{
			comboBoxCollections.Items.Clear();
			if (ColorCollections != null)
			{
				foreach (ColorCollection Collection in ColorCollections)
				{
					comboBoxCollections.Items.Add(Collection);
				}
			}
			btnDeleteCollection.Enabled = btnAddColor.Enabled = false;
		}

		private void PopulateCollectionColors(ColorCollection Collection)
		{
			listViewColors.BeginUpdate();
			listViewColors.Items.Clear();

			listViewColors.LargeImageList = new ImageList();
			listViewColors.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewColors.LargeImageList.ImageSize = new Size(48, 48);

			foreach (Color ColorItem in Collection.Color)
			{
				Bitmap result = new Bitmap(48,48);
				Graphics gfx = Graphics.FromImage(result);
				using (SolidBrush brush = new SolidBrush(ColorItem))
				{
					gfx.FillRectangle(brush, 0, 0, 48, 48);
					gfx.DrawRectangle(new Pen(Color.Black, 2), 0, 0, 48, 48);
				}

				listViewColors.LargeImageList.Images.Add(ColorItem.ToString(), result);

				ListViewItem item = new ListViewItem();
				//item.Text = ColorItem.Name;
				item.ToolTipText = string.Format("R: {0} G: {1} B: {2}", ColorItem.R, ColorItem.G, ColorItem.B);
				
				item.ImageKey = ColorItem.ToString();
				item.Tag = ColorItem;
				listViewColors.Items.Add(item);
			}
			listViewColors.EndUpdate();
			btnAddColor.Enabled = true;
		}

		#endregion

	}
}
