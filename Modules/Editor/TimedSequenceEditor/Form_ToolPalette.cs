using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_ToolPalette : DockContent
	{

		#region ListView Padding

		//Allows the padding between images in the listview controls to be set.
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		public int MakeLong(short lowPart, short highPart)
		{
			return (int)(((ushort)lowPart) | (uint)(highPart << 16));
		}

		public void ListViewItem_SetSpacing(ListView listview, short leftPadding, short topPadding)
		{
			const int lvmFirst = 0x1000;
			const int lvmSeticonspacing = lvmFirst + 53;
			SendMessage(listview.Handle, lvmSeticonspacing, IntPtr.Zero, (IntPtr)MakeLong(leftPadding, topPadding));
		}

		#endregion

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

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private List<Color> _colors = new List<Color>();
		private string _colorFilePath;
		private CurveLibrary _curveLibrary;
		private ColorGradientLibrary _colorGradientLibrary;
		private string _lastFolder;
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
			toolStripButtonExportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportColors.Image = Resources.disk;
			toolStripButtonImportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportColors.Image = Resources.folder_open;

			toolStripButtonEditCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditCurve.Image = Resources.pencil;
			toolStripButtonNewCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewCurve.Image = Resources.add;
			toolStripButtonDeleteCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteCurve.Image = Resources.delete;
			toolStripButtonExportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportCurves.Image = Resources.disk;
			toolStripButtonImportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportCurves.Image = Resources.folder_open;

			toolStripButtonEditGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditGradient.Image = Resources.pencil;
			toolStripButtonNewGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewGradient.Image = Resources.add;
			toolStripButtonDeleteGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteGradient.Image = Resources.delete;
			toolStripButtonExportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportGradients.Image = Resources.disk;
			toolStripButtonImportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportGradients.Image = Resources.folder_open;

			ListViewItem_SetSpacing(listViewColors, 48 + 5, 48 + 5);
			ListViewItem_SetSpacing(listViewCurves, 48 + 5, 48 + 30);
			ListViewItem_SetSpacing(listViewGradients, 48 + 5, 48 + 30);
		}

		private void ColorPalette_Load(object sender, EventArgs e)
		{
			_colorFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ColorPalette.xml");

			if (File.Exists(_colorFilePath))
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
			if (File.Exists(_colorFilePath))
			{
				using (FileStream reader = new FileStream(_colorFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
					_colors = (List<Color>)ser.ReadObject(reader);
				}
			}
		}

		public void Save_ColorPaletteFile()
		{
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<Color>));
			var dataWriter = XmlWriter.Create(_colorFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, _colors);
			dataWriter.Close();
		}

		private void PopulateColors()
		{
			listViewColors.BeginUpdate();
			listViewColors.Items.Clear();

			listViewColors.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(48, 48) };

			foreach (Color colorItem in _colors)
			{
				Bitmap result = new Bitmap(48, 48);
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
			toolStripButtonEditColor.Enabled = toolStripButtonDeleteColor.Enabled = false;
		}

		private void Update_ColorOrder()
		{
			_colors.Clear();
			foreach (ListViewItem item in listViewColors.Items)
			{
				_colors.Add((Color)item.Tag);
			}
			Save_ColorPaletteFile();
		}

		#endregion

		#region Curves

		private void Populate_Curves()
		{
			listViewCurves.BeginUpdate();
			listViewCurves.Items.Clear();

			listViewCurves.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(48, 48) };

			foreach (KeyValuePair<string, Curve> kvp in _curveLibrary)
			{
				Curve c = kvp.Value;
				string name = kvp.Key;

				listViewCurves.LargeImageList.Images.Add(name, c.GenerateCurveImage(new Size(48, 48)));

				ListViewItem item = new ListViewItem {Text = name, Name = name, ImageKey = name};

				if (item != null) listViewCurves.Items.Add(item);
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

			listViewGradients.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(48, 48) };

			foreach (KeyValuePair<string, ColorGradient> kvp in _colorGradientLibrary)
			{
				ColorGradient gradient = kvp.Value;
				string name = kvp.Key;

				var result = new Bitmap(gradient.GenerateColorGradientImage(new Size(48, 48), false), 48, 48);
				Graphics gfx = Graphics.FromImage(result);
				gfx.DrawRectangle(new Pen(Color.Black, 2), 0, 0, 48, 48);
				listViewGradients.LargeImageList.Images.Add(name, result);

				ListViewItem item = new ListViewItem {Text = name, Name = name, ImageKey = name};

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
				if (result != DialogResult.OK) return;
				Color colorValue = cp.Color.ToRGB().ToArgb();

				listViewColors.BeginUpdate();
				listViewColors.SelectedItems[0].ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorValue.R, colorValue.G, colorValue.B);
				listViewColors.SelectedItems[0].ImageKey = colorValue.ToString();
				listViewColors.SelectedItems[0].Tag = colorValue;
				listViewColors.EndUpdate();

				Update_ColorOrder();
				PopulateColors();
			}
		}

		private void toolStripButtonNewColor_Click(object sender, EventArgs e)
		{
			using (ColorPicker cp = new ColorPicker())
			{
				cp.LockValue_V = true;
				cp.Color = XYZ.FromRGB(Color.White);
				DialogResult result = cp.ShowDialog();
				if (result != DialogResult.OK) return;
				Color colorValue = cp.Color.ToRGB().ToArgb();

				_colors.Add(colorValue);
				PopulateColors();
				Save_ColorPaletteFile();
			}
		}

		private void toolStripButtonDeleteColor_Click(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems == null)
				return;

			DialogResult result = MessageBox.Show(@"Are you sure you want to delete this color?", @"Delete color?", MessageBoxButtons.YesNoCancel);

			if (result == DialogResult.Yes)
			{
				_colors.Remove((Color)listViewColors.SelectedItems[0].Tag);
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
					MessageBox.Show(@"Please enter a name.");
					continue;
				}

				if (_curveLibrary.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show(@"There is already a curve with that name. Do you want to overwrite it?",
														  @"Overwrite curve?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						_curveLibrary.AddCurve(dialog.Response, new Curve());
						_curveLibrary.EditLibraryCurve(dialog.Response);
						Populate_Curves();
						break;
					}

					if (result == DialogResult.Cancel)
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

			DialogResult result = MessageBox.Show(@"If you delete this library curve, ALL places it is used will be unlinked and will" +
								@" become independent curves. Are you sure you want to continue?", @"Delete library curve?", MessageBoxButtons.YesNoCancel);

			if (result == DialogResult.Yes)
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

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show(@"Please enter a name.");
					continue;
				}

				if (_colorGradientLibrary.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show(@"There is already a gradient with that name. Do you want to overwrite it?",
														  @"Overwrite gradient?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						_colorGradientLibrary.AddColorGradient(dialog.Response, new ColorGradient());
						_colorGradientLibrary.EditLibraryItem(dialog.Response);
						Populate_Gradients();
						break;
					}
					
					if (result == DialogResult.Cancel)
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
				MessageBox.Show(@"If you delete this library gradient, ALL places it is used will be unlinked and will" +
								@" become independent gradients. Are you sure you want to continue?", @"Delete library gradient?",
								MessageBoxButtons.YesNoCancel);

			if (result == DialogResult.Yes)
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
			ListViewItem movetoNewPosition = listViewColors.GetItemAt(p.X, p.Y);
			if (movetoNewPosition == null) return;
			ListViewItem dropToNewPosition = (e.Data.GetData(typeof(ListView.SelectedListViewItemCollection)) as ListView.SelectedListViewItemCollection)[0];
			ListViewItem cloneToNew = (ListViewItem)dropToNewPosition.Clone();
			int index = movetoNewPosition.Index;
			listViewColors.Items.Remove(dropToNewPosition);
			listViewColors.Items.Insert(index, cloneToNew);
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

		#region Import/Export

		#region Colors

		private void toolStripButtonExportColors_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vfc",
				Filter = @"Vixen 3 Favorite Colors (*.vfc)|*.vfc|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			try
			{
				DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
				var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
				ser.WriteObject(writer, _colors);
				writer.Close();
			}
			catch (Exception ex)
			{
				Logging.Error("While exporting Favorite Colors: " + saveFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Unable to export data, please check the error log for details", @"Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void toolStripButtonImportColors_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".vfc",
				Filter = @"Vixen 3 Favorite Colors (*.vfc)|*.vfc|All Files (*.*)|*.*",
				FilterIndex = 0
			};

			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			List<Color> colors = new List<Color>();

			try
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
					colors = (List<Color>)ser.ReadObject(reader);
				}

				foreach (Color color in colors)
				{
					_colors.Add(color);
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Invalid file while importing Favorite Colors: " + openFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Sorry, we didn't reconize the data in that file as valid Favorite Color data.", @"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Warning);

			}
			PopulateColors();
			Save_ColorPaletteFile();
		}

		#endregion

		#region Curves

		private void toolStripButtonExportCurves_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vcl",
				Filter = @"Vixen 3 Curve Library (*.vcl)|*.vcl|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			try
			{
				Dictionary<string, Curve> curves = _curveLibrary.ToDictionary(curve => curve.Key, curve => curve.Value);

				DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, Curve>));
				var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
				ser.WriteObject(writer, curves);
				writer.Close();
			}
			catch (Exception ex)
			{
				Logging.Error("While exporting Curve Library: " + saveFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Unable to export data, please check the error log for details", @"Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void toolStripButtonImportCurves_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".vcl",
				Filter = @"Vixen 3 Curve Library (*.vcl)|*.vcl|All Files (*.*)|*.*",
				FilterIndex = 0
			};
			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			Dictionary<string, Curve> curves = new Dictionary<string, Curve>();

			try
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, Curve>));
					curves = (Dictionary<string, Curve>)ser.ReadObject(reader);
				}

				foreach (KeyValuePair<string, Curve> curve in curves)
				{
					//This was just easier than prompting for a rename
					//and rechecking, and repormpting... and on and on and on...
					string curveName = curve.Key;
					int i = 2;
					while (_curveLibrary.Contains(curveName))
					{
						curveName = curve.Key + " " + i;
						i++;
					}

					_curveLibrary.AddCurve(curveName, curve.Value);
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Invalid file while importing Curve Library: " + openFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Sorry, we didn't reconize the data in that file as valid Curve Library data.", @"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					
			}
		}


		#endregion

		#region Gradients

		private void toolStripButtonExportGradients_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vgl",
				Filter = @"Vixen 3 Color Gradient Library (*.vgl)|*.vgl|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			try
			{
				Dictionary<string, ColorGradient> gradients = _colorGradientLibrary.ToDictionary(gradient => gradient.Key, gradient => gradient.Value);

				DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, ColorGradient>));
				var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
				ser.WriteObject(writer, gradients);
				writer.Close();
			}
			catch (Exception ex)
			{
				Logging.Error("While exporting Color Gradient Library: " + saveFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Unable to export data, please check the error log for details", @"Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void toolStripButtonImportGradients_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".vgl",
				Filter = @"Vixen 3 Color Gradient Library (*.vgl)|*.vgl|All Files (*.*)|*.*",
				FilterIndex = 0
			};

			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
			Dictionary<string, ColorGradient> gradients = new Dictionary<string, ColorGradient>();

			try
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, ColorGradient>));
					gradients = (Dictionary<string, ColorGradient>)ser.ReadObject(reader);
				}

				foreach (KeyValuePair<string, ColorGradient> gradient in gradients)
				{
					//This was just easier than prompting for a rename
					//and rechecking, and repormpting... and on and on and on...
					string gradientName = gradient.Key;
					int i = 2;
					while (_colorGradientLibrary.Contains(gradientName))
					{
						gradientName = gradient.Key + " " + i;
						i++;
					}

					_colorGradientLibrary.AddColorGradient(gradientName, gradient.Value);
				}
			}
			catch (Exception ex)
			{
				Logging.Error("Invalid file while importing Color Gradient Library: " + openFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Sorry, we didn't reconize the data in that file as valid Color Gradient Library data.", @"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Warning);

			}
		}

		#endregion

		#endregion

	}
}
