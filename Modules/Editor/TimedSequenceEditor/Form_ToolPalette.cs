using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.ColorManagement.ColorModels;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using Vixen.Services;
using Vixen.Module.App;
using Vixen.Sys;
using WeifenLuo.WinFormsUI.Docking;
using Vixen.Module.Effect;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_ToolPalette : DockContent
	{
		#region Public EventHandlers

		public EventHandler StartColorDrag;
		public EventHandler StartCurveDrag;
		public EventHandler StartGradientDrag;

		#endregion

		#region Public Members

		public int GradientHandling	{ get { return comboBoxGradientHandling.SelectedIndex + 1; } }
		
		public bool LinkCurves
		{ 
			get { return checkBoxLinkCurves.Checked; }
			set { checkBoxLinkCurves.Checked = value; }
		}
		
		public bool LinkGradients
		{
			get { return checkBoxLinkGradients.Checked; }
			set { checkBoxLinkGradients.Checked = value; }
		}

		public TimelineControl TimelineControl { get; set; }

		#endregion

		#region Private Members

		private List<Color> colors = new List<Color>();
		private string colorFilePath;
		private CurveLibrary _curveLibrary;
		private ColorGradientLibrary _colorGradientLibrary;
		private bool ShiftPressed
		{
			get { return ModifierKeys.HasFlag(Keys.Shift); }
		}
		
		#endregion

		#region Initialization

		public Form_ToolPalette(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
			Icon = Resources.Icon_Vixen3;	
		}

		private void ColorPalette_Load(object sender, EventArgs e)
		{
			colorFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ColorPalette.xml");

			if (File.Exists(colorFilePath))
			{
				Load_ColorPaletteFile();
				PopulateColors();
			}

			_curveLibrary = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;
			if (_curveLibrary != null)
			{
				Populate_Curves();
				_curveLibrary.CurveChanged += CurveLibrary_CurveChanged;
			}
			else //We have no curves :(
			{
				tabCurves.Hide();
			}

			_colorGradientLibrary = ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;
			if (_colorGradientLibrary != null)
			{
				Populate_Gradients();
				comboBoxGradientHandling.SelectedIndex = 0;
				_colorGradientLibrary.GradientChanged += GradientLibrary_GradientChanged;
			}
			else //No Gradients either ? Double :(
			{
				tabGradients.Hide();
			}

			Populate_Effects();

			labelHelp.Text = "The Tool Palette organizes your Favorite Colors, Curves, Color Gradients, and Effects all into one convienient place.\n\n" +
								"Simply Drag and Drop any of the items to effects in the sequence.\n" +
								"For Alternating effects, use the Control Key or Right mouse button to apply Color/Curve/Gradient 2.\n\n" +
								"Dropping an item to a selected effect, will apply the item to all selected effects. If the effect is not selected\n" +
								"it will be the only one the drop applies to.\n\n" +
								"For further documentaion on the Tool Palette, please visit the link at the top of this page.";
		}

		#endregion

		#region Private Methods

		#region Effects

		private void Populate_Effects()
		{
			listViewEffects.BeginUpdate();
			listViewEffects.Items.Clear();
			listViewEffects.LargeImageList = new ImageList();
			listViewEffects.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewEffects.LargeImageList.ImageSize = new Size(48, 48);

			foreach (
				IEffectModuleDescriptor effectDesriptor in ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				listViewEffects.LargeImageList.Images.Add(effectDesriptor.EffectName, effectDesriptor.GetRepresentativeImage(48, 48));

				ListViewItem effectItem = new ListViewItem();
				effectItem.Tag = effectDesriptor.TypeId;
				effectItem.Text = effectDesriptor.EffectName;
				effectItem.ImageKey = effectDesriptor.EffectName;

				listViewEffects.Items.Add(effectItem);
			}
			listViewEffects.EndUpdate();
		}
		
		#endregion

		#region Colors

		public void Load_ColorPaletteFile()
		{
			if (System.IO.File.Exists(colorFilePath))
			{
				using (FileStream reader = new FileStream(colorFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
					colors = (List<Color>)ser.ReadObject(reader);
				}
			}
		}

		public void Save_ColorPaletteFile()
		{
			var xmlsettings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<Color>));
			var dataWriter = XmlWriter.Create(colorFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, colors);
			dataWriter.Close();
		}

		private void PopulateColors()
		{
			listViewColors.BeginUpdate();
			listViewColors.Items.Clear();

			listViewColors.LargeImageList = new ImageList();
			listViewColors.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			listViewColors.LargeImageList.ImageSize = new Size(48, 48);

			foreach (Color colorItem in colors)
			{
				Bitmap result = new Bitmap(48, 48);
				Graphics gfx = Graphics.FromImage(result);
				using (SolidBrush brush = new SolidBrush(colorItem))
				{
					gfx.FillRectangle(brush, 0, 0, 48, 48);
					gfx.DrawRectangle(new Pen(Color.Black, 2), 0, 0, 48, 48);
				}

				listViewColors.LargeImageList.Images.Add(colorItem.ToString(), result);

				ListViewItem item = new ListViewItem();
				item.ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorItem.R, colorItem.G, colorItem.B);
				item.ImageKey = colorItem.ToString();
				item.Tag = colorItem;
				listViewColors.Items.Add(item);
			}
			listViewColors.EndUpdate();
			buttonEditColor.Enabled = buttonDeleteColor.Enabled = false;
		}

		private void Update_ColorOrder()
		{
			colors.Clear();
			foreach (ListViewItem item in listViewColors.Items)
			{
				colors.Add((Color)item.Tag);
			}
			Save_ColorPaletteFile();
		}

		#endregion

		#region Curves

		private void Populate_Curves()
		{
			listViewCurves.BeginUpdate();
			listViewCurves.Items.Clear();

			listViewCurves.LargeImageList = new ImageList();

			foreach (KeyValuePair<string, Curve> kvp in _curveLibrary)
			{
				Curve c = kvp.Value;
				string name = kvp.Key;

				listViewCurves.LargeImageList.ImageSize = new Size(48, 48);
				listViewCurves.LargeImageList.Images.Add(name, c.GenerateCurveImage(new Size(48, 48)));

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;

				listViewCurves.Items.Add(item);
			}

			listViewCurves.EndUpdate();
			buttonEditCurve.Enabled = buttonDeleteCurve.Enabled = false;
		}

		#endregion

		#region Gradients

		private void Populate_Gradients()
		{
			listViewGradients.BeginUpdate();
			listViewGradients.Items.Clear();

			listViewGradients.LargeImageList = new ImageList();
			listViewGradients.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

			foreach (KeyValuePair<string, ColorGradient> kvp in _colorGradientLibrary)
			{
				ColorGradient gradient = kvp.Value;
				string name = kvp.Key;

				listViewGradients.LargeImageList.ImageSize = new Size(48, 48);
				listViewGradients.LargeImageList.Images.Add(name, gradient.GenerateColorGradientImage(new Size(48, 48), false));

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;

				listViewGradients.Items.Add(item);
			}

			listViewGradients.EndUpdate();
			buttonEditGradient.Enabled = buttonDeleteGradient.Enabled = false;
		}

		#endregion

		#endregion

		#region Event Handlers

		#region Colors

		private void buttonNewColor_Click(object sender, EventArgs e)
		{
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB(Color.White);
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{
					Color ColorValue = cp.Color.ToRGB().ToArgb();
					colors.Add(ColorValue);
					PopulateColors();
					Save_ColorPaletteFile();
				}
			}
		}

		private void buttonEditColor_Click(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count != 1)
				return;

			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB((Color)listViewColors.SelectedItems[0].Tag);
				DialogResult result = cp.ShowDialog();
				if (result == DialogResult.OK)
				{
					Color ColorValue = cp.Color.ToRGB().ToArgb();

					listViewColors.BeginUpdate();
					listViewColors.SelectedItems[0].ToolTipText = string.Format("R: {0} G: {1} B: {2}", ColorValue.R, ColorValue.G, ColorValue.B);
					listViewColors.SelectedItems[0].ImageKey = ColorValue.ToString();
					listViewColors.SelectedItems[0].Tag = ColorValue;
					listViewColors.EndUpdate();

					Update_ColorOrder();
					PopulateColors();

				}
			}
		}

		private void buttonDeleteColor_Click(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems == null)
				return;

			foreach (ListViewItem ListItem in listViewColors.SelectedItems)
			{
				colors.Remove((Color)ListItem.Tag);
			}

			PopulateColors();
			Save_ColorPaletteFile();
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditColor.Enabled = (listViewColors.SelectedIndices.Count == 1);
			buttonDeleteColor.Enabled = (listViewColors.SelectedIndices.Count == 1);
		}

		private void listViewColors_DoubleClick(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count == 1)
				buttonEditColor.PerformClick();
		}

		#endregion

		#region Curves

		private void buttonNewCurve_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Curve name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (_curveLibrary.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show("There is already a curve with that name. Do you want to overwrite it?",
														  "Overwrite curve?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						_curveLibrary.AddCurve(dialog.Response, new Curve());
						_curveLibrary.EditLibraryCurve(dialog.Response);
						Populate_Curves();
						break;
					}
					else if (result == DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					_curveLibrary.AddCurve(dialog.Response, new Curve());
					_curveLibrary.EditLibraryCurve(dialog.Response);
					//Populate_Curves();
					break;
				}
			}
		}

		private void buttonEditCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count != 1)
				return;

			_curveLibrary.EditLibraryCurve(listViewCurves.SelectedItems[0].Name);
			//Populate_Curves();
		}

		private void buttonDeleteCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			DialogResult result =
				MessageBox.Show("If you delete this library curve, ALL places it is used will be unlinked and will" +
								" become independent curves. Are you sure you want to continue?", "Delete library curve?",
								MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				_curveLibrary.RemoveCurve(listViewCurves.SelectedItems[0].Name);
				//Populate_Curves();
			}
		}

		private void listViewCurves_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
			buttonDeleteCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
		}

		private void listViewCurves_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 1)
				buttonEditCurve.PerformClick();
		}

		private void CurveLibrary_CurveChanged(object sender, EventArgs e)
		{
			if (_curveLibrary.Count() > 0)
			{
				tabCurves.Show();
				Populate_Curves();
			}
			else
			{
				tabCurves.Hide();
			}

		}

		#endregion

		#region Gradients

		private void buttonNewGradient_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Gradient name?");

			while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (_colorGradientLibrary.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show("There is already a gradient with that name. Do you want to overwrite it?",
														  "Overwrite gradient?", MessageBoxButtons.YesNoCancel);
					if (result == System.Windows.Forms.DialogResult.Yes)
					{
						_colorGradientLibrary.AddColorGradient(dialog.Response, new ColorGradient());
						_colorGradientLibrary.EditLibraryItem(dialog.Response);
						Populate_Gradients();
						break;
					}
					else if (result == System.Windows.Forms.DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					_colorGradientLibrary.AddColorGradient(dialog.Response, new ColorGradient());
					_colorGradientLibrary.EditLibraryItem(dialog.Response);
					//Populate_Gradients();
					break;
				}
			}
		}

		private void buttonEditGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count != 1)
				return;

			_colorGradientLibrary.EditLibraryItem(listViewGradients.SelectedItems[0].Name);
			//Populate_Gradients();
		}

		private void buttonDeleteGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 0)
				return;

			DialogResult result =
				MessageBox.Show("If you delete this library gradient, ALL places it is used will be unlinked and will" +
								" become independent gradients. Are you sure you want to continue?", "Delete library gradient?",
								MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				_colorGradientLibrary.RemoveColorGradient(listViewGradients.SelectedItems[0].Name);
				//Populate_Gradients();
			}
		}

		private void listViewGradients_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditGradient.Enabled = (listViewGradients.SelectedIndices.Count == 1);
			buttonDeleteGradient.Enabled = (listViewGradients.SelectedIndices.Count == 1);
		}

		private void listViewGradients_DoubleClick(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 1)
				buttonEditGradient.PerformClick();
		}

		public void GradientLibrary_GradientChanged(object sender, EventArgs e)
		{
			if (_colorGradientLibrary.Count() > 0)
			{
				tabGradients.Show();
				Populate_Gradients();
			}
			else
			{
				tabGradients.Hide();
			}
		}

		#endregion

		private void linkToolPaletteDocumentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.vixenlights.com/vixen-3-documentation/sequencer/");
		}

		private void ToolPalette_FormClosing(object sender, FormClosingEventArgs e)
		{
			//As this came about late in development, Im leaving the original Save call, just commented out for now.
			//if (e.CloseReason != CloseReason.UserClosing) return;
			//e.Cancel = true;
			//Hide();
			//SaveToolPaletteLocation(this, e);
		}

		#endregion

		#region Drag/Drop

		#region Effects

		private void listViewEffects_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (listViewEffects.SelectedItems == null)
				return;
			DataObject data = new DataObject(DataFormats.Serializable, listViewEffects.SelectedItems[0].Tag);
			listViewEffects.DoDragDrop(data, DragDropEffects.Copy);
		}

		#endregion

		#region Colors

		private void listViewColors_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (listViewColors.SelectedItems == null)
				return;
			
			if (ShiftPressed)
			{
				listViewColors.DoDragDrop(listViewColors.SelectedItems, DragDropEffects.Move);
			}
			else
			{
				StartColorDrag(this, e);
				listViewColors.DoDragDrop(listViewColors.SelectedItems[0].Tag, DragDropEffects.Copy);
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
			Update_ColorOrder();
		}


		private void listViewColors_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
			{
				e.Effect = DragDropEffects.Move;
			}
		}

		#endregion

		#region Curves

		private void listViewCurves_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (listViewCurves.SelectedItems == null)
				return;

			StartCurveDrag(this, e);
			listViewCurves.DoDragDrop(listViewCurves.SelectedItems[0].Name, DragDropEffects.Copy);
		}

		#endregion

		#region Gradients

		private void listViewGradient_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (listViewGradients.SelectedItems == null)
				return;

			StartGradientDrag(this, e);
			listViewGradients.DoDragDrop(listViewGradients.SelectedItems[0].Name, DragDropEffects.Copy);
		}

		#endregion

		#endregion


	}
}
