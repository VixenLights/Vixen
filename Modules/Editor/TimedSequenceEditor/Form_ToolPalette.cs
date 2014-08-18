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

			toolStripButtonEditColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditColor.Image = Resources.pencil;
			toolStripButtonNewColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewColor.Image = Resources.add;
			toolStripButtonDeleteColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteColor.Image = Resources.delete;

			toolStripButtonEditCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditCurve.Image = Resources.pencil;
			toolStripButtonNewCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewCurve.Image = Resources.add;
			toolStripButtonDeleteCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteCurve.Image = Resources.delete;

			toolStripButtonEditGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditGradient.Image = Resources.pencil;
			toolStripButtonNewGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewGradient.Image = Resources.add;
			toolStripButtonDeleteGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteGradient.Image = Resources.delete;
	
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

			_colorGradientLibrary = ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;
			if (_colorGradientLibrary != null)
			{
				Populate_Gradients();
				_colorGradientLibrary.GradientChanged += GradientLibrary_GradientChanged;
			}
			
			comboBoxGradientHandling.SelectedIndex = 0;

		}

		#endregion

		#region Private Methods

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
			toolStripButtonEditColor.Enabled = toolStripButtonDeleteColor.Enabled = false;
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
			toolStripButtonEditCurve.Enabled = toolStripButtonDeleteCurve.Enabled = false;
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
			toolStripButtonEditGradient.Enabled = toolStripButtonDeleteGradient.Enabled = false;
		}

		#endregion

		#endregion

		#region Event Handlers

		#region Colors

		private void toolStripButtonEditColor_Click(object sender, EventArgs e)
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

		private void toolStripButtonNewColor_Click(object sender, EventArgs e)
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

		private void toolStripButtonDeleteColor_Click(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems == null)
				return;

			DialogResult result = MessageBox.Show("Are you sure you want to delete this color?", "Delete color?", MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				colors.Remove((Color)listViewColors.SelectedItems[0].Tag);
			}

			PopulateColors();
			Save_ColorPaletteFile();
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonEditColor.Enabled = toolStripButtonDeleteColor.Enabled = (listViewColors.SelectedItems.Count == 1);
		}

		private void listViewColors_MouseDoubleClick(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count == 1)
				toolStripButtonEditColor.PerformClick();
		}

		#endregion

		#region Curves

		private void toolStripButtonEditCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count != 1)
				return;

			_curveLibrary.EditLibraryCurve(listViewCurves.SelectedItems[0].Name);
		}

		private void toolStripButtonNewCurve_Click(object sender, EventArgs e)
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
					break;
				}
			}
		}

		private void toolStripButtonDeleteCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			DialogResult result = MessageBox.Show("If you delete this library curve, ALL places it is used will be unlinked and will" +
								" become independent curves. Are you sure you want to continue?", "Delete library curve?", MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes)
			{
				_curveLibrary.RemoveCurve(listViewCurves.SelectedItems[0].Name);

			}
		}

		private void listViewCurves_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonEditCurve.Enabled = toolStripButtonDeleteCurve.Enabled = (listViewCurves.SelectedItems.Count == 1);
		}

		private void listViewCurves_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 1)
				toolStripButtonEditCurve.PerformClick();
		}

		private void CurveLibrary_CurveChanged(object sender, EventArgs e)
		{
				Populate_Curves();
		}

		#endregion

		#region Gradients

		private void toolStripButtonEditGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count != 1)
				return;

			_colorGradientLibrary.EditLibraryItem(listViewGradients.SelectedItems[0].Name);
		}

		private void toolStripButtonNewGradient_Click(object sender, EventArgs e)
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
					break;
				}
			}
		}

		private void toolStripButtonDeleteGradient_Click(object sender, EventArgs e)
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
			}
		}

		private void listViewGradients_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonEditGradient.Enabled = toolStripButtonDeleteGradient.Enabled = (listViewGradients.SelectedItems.Count == 1);
		}

		private void listViewGradients_MouseDoubleClick(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 1)
				toolStripButtonEditGradient.PerformClick();
		}

		public void GradientLibrary_GradientChanged(object sender, EventArgs e)
		{
				Populate_Gradients();
		}

		#endregion

		#endregion

		#region Drag/Drop

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
