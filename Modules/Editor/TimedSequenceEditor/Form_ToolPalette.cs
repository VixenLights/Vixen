using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;


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

		#region Public Members

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
		private readonly Size _imageSize;
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
			ThemeUpdateControls.UpdateControls(this);
			var t = (int)(48*ScalingTools.GetScaleFactor());
			_imageSize = new Size(t,t);
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripCurves.ImageScalingSize = new Size(iconSize, iconSize);
			toolStripGradients.ImageScalingSize = new Size(iconSize, iconSize);
			toolStripColors.ImageScalingSize = new Size(iconSize, iconSize);
			toolStripButtonEditColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditColor.Image = Tools.GetIcon(Resources.pencil, iconSize);

			toolStripButtonNewColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewColor.Image = Tools.GetIcon(Resources.add, iconSize);
			toolStripButtonDeleteColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteColor.Image = Tools.GetIcon(Resources.delete, iconSize);
			toolStripButtonExportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportColors.Image = Tools.GetIcon(Resources.disk, iconSize);
			toolStripButtonImportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportColors.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			toolStripButtonEditCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditCurve.Image = Tools.GetIcon(Resources.pencil, iconSize);
			toolStripButtonNewCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewCurve.Image = Tools.GetIcon(Resources.add, iconSize);
			toolStripButtonDeleteCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteCurve.Image = Tools.GetIcon(Resources.delete, iconSize);
			toolStripButtonExportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportCurves.Image = Tools.GetIcon(Resources.disk, iconSize);
			toolStripButtonImportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportCurves.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			toolStripButtonEditGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditGradient.Image = Tools.GetIcon(Resources.pencil, iconSize);
			toolStripButtonNewGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewGradient.Image = Tools.GetIcon(Resources.add, iconSize);
			toolStripButtonDeleteGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteGradient.Image = Tools.GetIcon(Resources.delete, iconSize);
			toolStripButtonExportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportGradients.Image = Tools.GetIcon(Resources.disk, iconSize);
			toolStripButtonImportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportGradients.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			short sideGap = (short)(_imageSize.Width + (5*ScalingTools.GetScaleFactor()));
			
			short topGap = (short)(_imageSize.Width + ScalingTools.MeasureHeight(Font, "Test")* 2);

			ListViewItem_SetSpacing(listViewColors, sideGap, sideGap);
			ListViewItem_SetSpacing(listViewCurves, sideGap, topGap);
			ListViewItem_SetSpacing(listViewGradients, sideGap, topGap);

			listViewColors.AllowDrop = true;
			listViewCurves.AllowDrop = true;
			listViewGradients.AllowDrop = true;

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			foreach (Control tab in tabControlEX1.TabPages)
			{
				tab.BackColor = ThemeColorTable.BackgroundColor;
				tab.ForeColor = ThemeColorTable.ForeColor;
			}
			tabControlEX1.SelectedTabColor = ThemeColorTable.BackgroundColor;
			tabControlEX1.TabColor = ThemeColorTable.BackgroundColor;
			tabControlEX1.SelectedTab = tabPageEX1;
			tabControlEX1.SizeMode = TabSizeMode.Fixed;
			SizeF size = ScalingTools.MeasureString(Font, "Gradientsss");
			tabControlEX1.ItemSize = size.ToSize();
			//Over-ride the auto theme listview back color
			listViewColors.BackColor = ThemeColorTable.BackgroundColor;
			listViewCurves.BackColor = ThemeColorTable.BackgroundColor;
			listViewGradients.BackColor = ThemeColorTable.BackgroundColor;
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

			listViewColors.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = _imageSize };

			foreach (Color colorItem in _colors)
			{
				var item = CreateColorListItem(colorItem);

				listViewColors.Items.Add(item);
			}
			listViewColors.EndUpdate();
			toolStripButtonEditColor.Enabled = toolStripButtonDeleteColor.Enabled = false;
		}

		private ListViewItem CreateColorListItem(Color colorItem)
		{
			Bitmap result = new Bitmap(48, 48);
			Graphics gfx = Graphics.FromImage(result);
			using (SolidBrush brush = new SolidBrush(colorItem))
			{
				using (var p = new Pen(ThemeColorTable.BorderColor, 2))
				{
					gfx.FillRectangle(brush, 0, 0, _imageSize.Width, _imageSize.Height);
					gfx.DrawRectangle(p, 0, 0, _imageSize.Width, _imageSize.Height);
				}
			}
			gfx.Dispose();
			listViewColors.LargeImageList.Images.Add(colorItem.ToString(), result);

			ListViewItem item = new ListViewItem
			{
				ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorItem.R, colorItem.G, colorItem.B),
				ImageKey = colorItem.ToString(),
				Tag = colorItem,
				ForeColor = ThemeColorTable.ForeColor
			};
			return item;
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

			listViewCurves.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = _imageSize };
			using (var p = new Pen(ThemeColorTable.BorderColor, 2))
			{
				foreach (KeyValuePair<string, Curve> kvp in _curveLibrary)
				{
					Curve c = kvp.Value;
					string name = kvp.Key;

					var image = c.GenerateGenericCurveImage(_imageSize);
					Graphics gfx = Graphics.FromImage(image);
					gfx.DrawRectangle(p, 0, 0, _imageSize.Width, _imageSize.Height);
					gfx.Dispose();
					listViewCurves.LargeImageList.Images.Add(name, image);

					ListViewItem item = new ListViewItem
					{
						Text = name,
						Name = name,
						ImageKey = name,
						Tag = c,
						ForeColor = ThemeColorTable.ForeColor
					};
					listViewCurves.Items.Add(item);
				}
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

			listViewGradients.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = _imageSize };
			using (var p = new Pen(ThemeColorTable.BorderColor, 2))
			{
				foreach (KeyValuePair<string, ColorGradient> kvp in _colorGradientLibrary)
				{
					ColorGradient gradient = kvp.Value;
					string name = kvp.Key;

					var result = new Bitmap(gradient.GenerateColorGradientImage(_imageSize, false), _imageSize.Width, _imageSize.Height);
					Graphics gfx = Graphics.FromImage(result);
					gfx.DrawRectangle(p, 0, 0, _imageSize.Width, _imageSize.Height);
					gfx.Dispose();
					listViewGradients.LargeImageList.Images.Add(name, result);
					
					ListViewItem item = new ListViewItem
					{
						Text = name,
						Name = name,
						ImageKey = name,
						Tag = gradient,
						ForeColor = ThemeColorTable.ForeColor
					};

					listViewGradients.Items.Add(item);
				}
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
				cp.LockValue_V = false;
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
				cp.LockValue_V = false;
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

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to delete this color?", "Delete color?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
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
			AddCurveToLibrary(new Curve());
		}

		private void AddCurveToLibrary(Curve c, bool edit=true)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Curve name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					var messageBox = new MessageBoxForm("Please enter a name.", "Curve Name", false, false);
					messageBox.ShowDialog();
					continue;
				}

				if (_curveLibrary.Contains(dialog.Response))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There is already a curve with that name. Do you want to overwrite it?", "Overwrite curve?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						_curveLibrary.AddCurve(dialog.Response, c);
						if (edit)
						{
							_curveLibrary.EditLibraryCurve(dialog.Response);	
						}
						break;
					}

					if (messageBox.DialogResult == DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					_curveLibrary.AddCurve(dialog.Response, c);
					if (edit)
					{
						_curveLibrary.EditLibraryCurve(dialog.Response);	
					}
					
					break;
				}
			}
		}

		private void toolStripButtonDeleteCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete this library curve, ALL places it is used will be unlinked and will" +
								@" become independent curves. Are you sure you want to continue?", "Delete library curve?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
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
			AddGradientToLibrary(new ColorGradient());
		}

		private void AddGradientToLibrary(ColorGradient cg, bool edit = true)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Gradient name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Please enter a name.", "Warning", false, false);
					messageBox.ShowDialog();
					continue;
				}

				if (_colorGradientLibrary.Contains(dialog.Response))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There is already a gradient with that name. Do you want to overwrite it?", "Overwrite gradient?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						_colorGradientLibrary.AddColorGradient(dialog.Response, cg);
						if (edit)
						{
							_colorGradientLibrary.EditLibraryItem(dialog.Response);	
						}
						break;
					}

					if (messageBox.DialogResult == DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					_colorGradientLibrary.AddColorGradient(dialog.Response, cg);
					if (edit)
					{
						_colorGradientLibrary.EditLibraryItem(dialog.Response);	
					}
					break;
				}
			}
		}

		private void toolStripButtonDeleteGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 0)
				return;

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete this library gradient, ALL places it is used will be unlinked and will" +
								@" become independent gradients. Are you sure you want to continue?", "Delete library gradient?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
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
				//StartColorDrag(this, e);
				listViewColors.DoDragDrop(listViewColors.SelectedItems[0].Tag, DragDropEffects.Copy);
			} 

		}

		private void listViewColors_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				Color c = (Color)e.Data.GetData(typeof (Color));
				ListViewItem item = CreateColorListItem(c);
				Point p = listViewColors.PointToClient(new Point(e.X, e.Y));
				ListViewItem referenceItem = listViewColors.GetItemAt(p.X, p.Y);
				if (referenceItem != null)
				{
					int index = referenceItem.Index;
					listViewColors.Items.Insert(index, item);
				}
				else
				{
					listViewColors.Items.Add(item);
				}
				
				listViewColors.Alignment = ListViewAlignment.SnapToGrid;
				ListViewItem_SetSpacing(listViewColors, 48 + 5, 48 + 5);
				Update_ColorOrder();
				
			}
			else if (e.Effect == DragDropEffects.Move)
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
				ListViewItem_SetSpacing(listViewColors, 48 + 5, 48 + 5);
				Update_ColorOrder();	
			}
			
		}


		private void listViewColors_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
			{
				e.Effect = DragDropEffects.Move;
				return;
			}
			if (e.Data.GetDataPresent(typeof (Color)))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			e.Effect = DragDropEffects.None;
			
		}

		#endregion

		#region Curves

		private void listViewCurves_ItemDrag(object sender, ItemDragEventArgs e)
		{
			//StartCurveDrag(this, e);
			Curve newCurve = new Curve((Curve)listViewCurves.SelectedItems[0].Tag);
			if (LinkCurves)
			{
				newCurve.LibraryReferenceName = listViewCurves.SelectedItems[0].Name;
			}
			newCurve.IsCurrentLibraryCurve = false;
			listViewCurves.DoDragDrop(newCurve, DragDropEffects.Copy);
		}

		private void listViewCurves_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(Curve)))
			{
				Curve c = (Curve)e.Data.GetData(typeof(Curve));
				if (!c.IsLibraryReference)
				{
					e.Effect = DragDropEffects.Copy;
					return;	
				}
			}
			e.Effect = DragDropEffects.None;
		}

		private void listViewCurves_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				Curve c = (Curve)e.Data.GetData(typeof(Curve));
				AddCurveToLibrary(c, false);
			}	
		}

		#endregion

		#region Gradients

		private void listViewGradients_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Effect == DragDropEffects.Copy)
			{
				ColorGradient c = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
				AddGradientToLibrary(c, false);
			}	
		}

		private void listViewGradients_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ColorGradient)))
			{
				ColorGradient cg = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
				if (!cg.IsLibraryReference)
				{
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}
			e.Effect = DragDropEffects.None;
		}

		private void listViewGradient_ItemDrag(object sender, ItemDragEventArgs e)
		{
			//StartGradientDrag(this, e);

			ColorGradient newGradient = new ColorGradient((ColorGradient)listViewGradients.SelectedItems[0].Tag);
			if (LinkGradients)
			{
				newGradient.LibraryReferenceName = listViewGradients.SelectedItems[0].Name;
			}
			newGradient.IsCurrentLibraryGradient = false;
			listViewGradients.DoDragDrop(newGradient, DragDropEffects.Copy);
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to export data, please check the error log for details", "Unable to export", false, false);
				messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Sorry, we didn't reconize the data in that file as valid Favorite Color data.", "Invalid file", false, false);
				messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to export data, please check the error log for details", "Unable to export", false, false);
				messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Sorry, we didn't reconize the data in that file as valid Curve Library data.", "Invalid file", false, false);
				messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to export data, please check the error log for details.", "Unable to export", false, false);
				messageBox.ShowDialog();
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Sorry, we didn't reconize the data in that file as valid Color Gradient Library data.", "Invalid file", false, false);
				messageBox.ShowDialog();
			}
		}

		#endregion

		private void Form_ToolPalette_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_curveLibrary != null)
			{
				_curveLibrary.CurveChanged -= CurveLibrary_CurveChanged;
			}
			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientChanged -= GradientLibrary_GradientChanged;
			}
		}

		

		#endregion
	}
}
