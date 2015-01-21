using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class ColorCollectionLibrary_Form : Form
	{
		#region Member Variables

		public List<ColorCollection> ColorCollections { get; set; }
		private Boolean _isDirty;
		private ColorCollection _currentCollection;
		private Color _colorValue;
		private string _lastFolder;

		#endregion

		#region Initialization

		public ColorCollectionLibrary_Form(List<ColorCollection> collections)
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
			ColorCollections = collections;
			PopulateCollectionList();
			_isDirty = false;
		}

		#endregion

		#region Event Handlers

		private void addColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddColorToCollection();
		}

		private void removeColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RemoveColorFromCollection();
		}

		private void exportCollectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportColorCollections();
		}

		private void importCollectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ImportColorCollections();
		}

		private void newCollectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewColorCollection();
		}

		private void deleteCollectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteColorCollection();
		}

		private void comboBoxCollections_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentCollection = (ColorCollection)comboBoxCollections.SelectedItem;
			textBoxDescription.Text = _currentCollection.Description;
			deleteCollectionToolStripMenuItem.Enabled = textBoxDescription.Enabled = true;
			PopulateCollectionColors(_currentCollection);
		}

		private void RandomColorLibrary_Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_isDirty)
			{
				var warnResult = MessageBox.Show(@"You will loose any changes, do you wish to save them now ?", @"Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (warnResult == DialogResult.Yes)
					DialogResult = DialogResult.OK;
				if (warnResult == DialogResult.Cancel)
					e.Cancel = true;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_isDirty = false;
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			removeColorToolStripMenuItem.Enabled = (listViewColors.SelectedItems.Count > 0);
		}

		private void textBoxDescription_KeyUp(object sender, KeyEventArgs e)
		{
			if (_currentCollection == null)
				return;
			_currentCollection.Description = textBoxDescription.Text;
			_isDirty = true;
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
			ListViewItem movetoNewPosition = listViewColors.GetItemAt(p.X, p.Y);
			if (movetoNewPosition == null) return;
			ListViewItem dropToNewPosition = (e.Data.GetData(typeof(ListView.SelectedListViewItemCollection)) as ListView.SelectedListViewItemCollection)[0];
			ListViewItem cloneToNew = (ListViewItem)dropToNewPosition.Clone();
			int index = movetoNewPosition.Index;
			listViewColors.Items.Remove(dropToNewPosition);
			listViewColors.Items.Insert(index, cloneToNew);
			listViewColors.Alignment = ListViewAlignment.SnapToGrid;
			UpdateCollectionColorOrder();
		}

		#endregion

		#region Private Methods

		private void RemoveColorFromCollection()
		{
			if (_currentCollection == null || listViewColors.SelectedItems == null)
				return;

			foreach (ListViewItem listItem in listViewColors.SelectedItems)
			{
				_currentCollection.Color.Remove((Color) listItem.Tag);
				_isDirty = true;
			}

			PopulateCollectionColors(_currentCollection);
		}

		private void AddColorToCollection()
		{
			if (_currentCollection == null)
				return;
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB(_colorValue);
				cp.StartPosition = FormStartPosition.Manual;
				cp.Top = Top;
				cp.Left = Left + Width;
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{
					_colorValue = cp.Color.ToRGB().ToArgb();
					_currentCollection.Color.Add(_colorValue);
					_isDirty = true;
					PopulateCollectionColors(_currentCollection);
				}
			}
		}

		private void ExportColorCollections()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vcc",
				Filter = @"Vixen 3 Color Collections (*.vcc)|*.vcc|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<ColorCollection>));
			var dataWriter = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
			dataSer.WriteObject(dataWriter, ColorCollections);
			dataWriter.Close();			
		}

		private void ImportColorCollections()
		{
			int importCount = 0;

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".vcc",
				Filter = @"Vixen 3 Color Collections (*.vcc)|*.vcc|All Files (*.*)|*.*",
				FilterIndex = 0
			};

			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);

			if (File.Exists(openFileDialog.FileName))
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<ColorCollection>));
					foreach (ColorCollection colorCollection in (List<ColorCollection>) ser.ReadObject(reader))
					{
						if (ColorCollections.Contains(colorCollection))
						{
							DialogResult result = MessageBox.Show(@"A collection with the name " + colorCollection.Name + @" already exists. Do you want to overwrite it?",
										  @"Overwrite collection?", MessageBoxButtons.YesNoCancel);
							if (result == DialogResult.Yes)
							{
								//Remove the collection to overwrite, we will add the new one below.
								ColorCollections.Remove(colorCollection);
							}
							else
							{
								continue;
							}
						}
						importCount++;
						ColorCollections.Add(colorCollection);
						_isDirty = true;
					}
				}
				PopulateCollectionList();
				if (_currentCollection != null) comboBoxCollections.Text = _currentCollection.Name;
				MessageBox.Show(@"Imported " + importCount + @" Color Collections.", @"Color Collections Import", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}			
		}


		private void DeleteColorCollection()
		{
			if (_currentCollection != null)
			{
				DialogResult result = MessageBox.Show(string.Format("Are you sure you want to delete the collection: {0} ?", _currentCollection.Name), @"Delete collection?", MessageBoxButtons.YesNo);
				if (result == DialogResult.Yes)
				{
					ColorCollections.Remove(_currentCollection);
					listViewColors.Items.Clear();
					textBoxDescription.Text = "";
					textBoxDescription.Enabled = false;
					_isDirty = true;
					PopulateCollectionList();
				}
			}
		}

		private void NewColorCollection()
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Collecton Name?");
			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show(@"Please enter a name.");
					continue;
				}
				ColorCollection item = new ColorCollection {Name = dialog.Response};
				if (ColorCollections.Contains(item))
				{
					DialogResult result = MessageBox.Show(@"A collection with the name " + item.Name + @" already exists. Do you want to overwrite it?",
														  @"Overwrite collection?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						ColorCollections.Remove(item);
						ColorCollections.Add(item);
						_isDirty = true;
						PopulateCollectionList();
						comboBoxCollections.Text = item.Name;
					}

					break;

				}

				ColorCollections.Add(item);
				_isDirty = true;
				PopulateCollectionList();
				comboBoxCollections.Text = item.Name;
				break;

			}
		}

		private void UpdateCollectionColorOrder()
		{
			if (_currentCollection == null)
				return;
			_currentCollection.Color.Clear();
			foreach (ListViewItem listItem in listViewColors.Items)
			{
				_currentCollection.Color.Add((Color)listItem.Tag);
			}
			_isDirty = true;
		}

		private void PopulateCollectionList()
		{
			comboBoxCollections.Items.Clear();
			if (ColorCollections != null)
			{
				foreach (ColorCollection collection in ColorCollections)
				{
					comboBoxCollections.Items.Add(collection);
				}
			}
			deleteCollectionToolStripMenuItem.Enabled = addColorToolStripMenuItem.Enabled = false;
		}

		private void PopulateCollectionColors(ColorCollection collection)
		{
			listViewColors.BeginUpdate();
			listViewColors.Items.Clear();

			listViewColors.LargeImageList = new ImageList();
			listViewColors.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewColors.LargeImageList.ImageSize = new Size(48, 48);

			foreach (Color colorItem in collection.Color)
			{
				Bitmap result = new Bitmap(48,48);
				Graphics gfx = Graphics.FromImage(result);
				using (SolidBrush brush = new SolidBrush(colorItem))
				{
					gfx.FillRectangle(brush, 0, 0, 48, 48);
					gfx.DrawRectangle(new Pen(Color.Black, 2), 0, 0, 48, 48);
				}

				listViewColors.LargeImageList.Images.Add(colorItem.ToString(), result);

				ListViewItem item = new ListViewItem
				{
					ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorItem.R, colorItem.G, colorItem.B),
					ImageKey = colorItem.ToString(),
					Tag = colorItem
				};

				listViewColors.Items.Add(item);
			}
			listViewColors.EndUpdate();
			addColorToolStripMenuItem.Enabled = true;
		}

		#endregion

	}
}
