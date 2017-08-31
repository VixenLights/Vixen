using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using VixenModules.Sequence.Timed;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class ColorCollectionLibrary_Form : BaseForm
	{
		#region Member Variables

		public List<ColorCollection> ColorCollections { get; set; }
		private Boolean _isDirty;
		private ColorCollection _currentCollection;
		private Color _colorValue;
		private string _lastFolder;
		private readonly Pen _borderPen = new Pen(ThemeColorTable.BorderColor, 2);

		#endregion

		#region Initialization

		public ColorCollectionLibrary_Form(List<ColorCollection> collections)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			listViewColors.BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonNewCollection.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonNewCollection.Text = "";
			buttonDeleteCollection.Image = Tools.GetIcon(Resources.minus, iconSize);
			buttonDeleteCollection.Text = "";
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
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
			buttonDeleteCollection.Enabled = textBoxDescription.Enabled = true;
			PopulateCollectionColors(_currentCollection);
		}

		private void RandomColorLibrary_Form_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_isDirty)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("You will loose any changes, do you wish to save them now ?", "Warning", true, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
					DialogResult = DialogResult.OK;
				if (messageBox.DialogResult == DialogResult.Cancel)
					e.Cancel = true;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_isDirty = false;
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonRemoveColor.Enabled = (listViewColors.SelectedItems.Count > 0);
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
				cp.LockValue_V = false;
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
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("A collection with the name " + colorCollection.Name + @" already exists. Do you want to overwrite it?", "Overwrite collection?", true, false);
							messageBox.ShowDialog();
							if (messageBox.DialogResult == DialogResult.OK)
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
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Imported " + importCount + @" Color Collections.", "Color Collections Import", false, false);
					messageBox.ShowDialog();
				}
			}			
		}


		private void DeleteColorCollection()
		{
			if (_currentCollection != null)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(string.Format("Are you sure you want to delete the collection: {0} ?", _currentCollection.Name), "Delete collection?", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Please enter a name.", "Color Collection Name", false, false);
					messageBox.ShowDialog();
					continue;
				}
				ColorCollection item = new ColorCollection {Name = dialog.Response};
				if (ColorCollections.Contains(item))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("A collection with the name " + item.Name + @" already exists. Do you want to overwrite it?", "Overwrite collection?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
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
			buttonDeleteCollection.Enabled = buttonAddColor.Enabled = false;
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
					gfx.DrawRectangle(_borderPen, 0, 0, 48, 48);
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
			buttonAddColor.Enabled = true;
		}

		#endregion

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

		private void buttonNewCollection_Click(object sender, EventArgs e)
		{
			NewColorCollection();
		}

		private void buttonDeleteCollection_Click(object sender, EventArgs e)
		{
			DeleteColorCollection();
		}

		private void buttonImportCollection_Click(object sender, EventArgs e)
		{
			ImportColorCollections();
		}

		private void buttonExportCollection_Click(object sender, EventArgs e)
		{
			ExportColorCollections();
		}

		private void buttonAddColor_Click(object sender, EventArgs e)
		{
			AddColorToCollection();
		}

		private void buttonRemoveColor_Click(object sender, EventArgs e)
		{
			RemoveColorFromCollection();
		}

		private void buttonTextColorChange(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.ForeColor = btn.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}
