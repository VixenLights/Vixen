using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Sys;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_ColorLibrary : DockContent
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

		public TimelineControl TimelineControl { get; set; }

		#endregion

		#region Private Members

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private List<Color> _colors = new List<Color>();
		private string _colorFilePath;
		private string _lastFolder;
		private Size _imageSize;
		private double _colorLibraryScale = 1;
		private int _dragX;
		private int _dragY;
		private short _sideGap;
		
		#endregion

		#region Initialization

		public Form_ColorLibrary(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			toolStripColors.Renderer = new ThemeToolStripRenderer();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripColors.ImageScalingSize = new Size(iconSize, iconSize);
			toolStripButtonEditColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditColor.Image = Tools.GetIcon(Resources.configuration, iconSize);

			toolStripButtonNewColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewColor.Image = Tools.GetIcon(Resources.addItem, iconSize);
			toolStripButtonDeleteColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteColor.Image = Tools.GetIcon(Resources.delete_32, iconSize);
			toolStripButtonExportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportColors.Image = Tools.GetIcon(Resources.folder_go, iconSize);
			toolStripButtonImportColors.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportColors.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			listViewColors.AllowDrop = true;

			var xml = new XMLProfileSettings();
			_colorLibraryScale = Convert.ToDouble(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ColorLibraryScale", Name), "1"), CultureInfo.InvariantCulture);

			ImageSetup();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			//Over-ride the auto theme listview back color
			listViewColors.BackColor = ThemeColorTable.BackgroundColor;
			listViewColors.Alignment = ListViewAlignment.Top;
		}

		private void ImageSetup()
		{
			var t = (int)Math.Round(48 * _colorLibraryScale * ScalingTools.GetScaleFactor());
			_imageSize = new Size(t, t);
			_sideGap = (short)(_imageSize.Width + (10 * ScalingTools.GetScaleFactor()));

			ListViewItem_SetSpacing(listViewColors, _sideGap, _sideGap);
		}

		private void ColorPalette_Load(object sender, EventArgs e)
		{
			_colorFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ColorPalette.xml");

			if (File.Exists(_colorFilePath))
			{
				Load_ColorPaletteFile();
				PopulateColors();
			}
			else
			{
				listViewColors.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = _imageSize };
			}
			
		}

		#endregion

		#region Private Methods

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
			_SelectionChanged();
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
			_SelectionChanged();
		}

		public void PopulateColors()
		{
			listViewColors.BeginUpdate();
			listViewColors.Items.Clear();
			var newFontSize = new Font(Font.FontFamily.Name, 1, Font.Style);
			listViewColors.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = _imageSize };

			foreach (Color colorItem in _colors)
			{
				var item = CreateColorListItem(colorItem);
				item.Font = newFontSize;
				item.ForeColor = ThemeColorTable.BackgroundColor;
				listViewColors.Items.Add(item);
			}
			listViewColors.EndUpdate();
			toolStripButtonEditColor.Enabled = toolStripButtonDeleteColor.Enabled = false;
			_SelectionChanged();
		}

		private ListViewItem CreateColorListItem(Color colorItem)
		{
			Bitmap result = new Bitmap(_imageSize.Width, _imageSize.Width);
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
				Tag = colorItem
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

		#region Event Handlers

		private void toolStripButtonEditColor_Click(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count == 0)
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
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to delete the selected color(s)?", "Delete color?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
			{
				foreach (ListViewItem item in listViewColors.SelectedItems)
				{
					_colors.Remove((Color)item.Tag);
				}
			}

			PopulateColors();
			Save_ColorPaletteFile();
		}

		private void listViewColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonDeleteColor.Enabled = (listViewColors.SelectedItems.Count > 0);
			toolStripButtonEditColor.Enabled = (listViewColors.SelectedItems.Count == 1);
			foreach (ListViewItem VARIABLE in listViewColors.Items)
			{
				VARIABLE.Text = VARIABLE.Selected ? "" : " ";
			}
		}

		private void listViewColors_MouseDoubleClick(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count == 1)
				toolStripButtonEditColor.PerformClick();
		}

		public event EventHandler SelectionChanged;

		private void _SelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		#endregion

		#region Drag/Drop

		private void listViewColors_ItemDrag(object sender, ItemDragEventArgs e)
		{
			listViewColors.DoDragDrop(listViewColors.SelectedItems, DragDropEffects.Move);
		}
		private void listViewColors_DragLeave(object sender, EventArgs e)
		{
			if (listViewColors.SelectedItems.Count == 0) return;
			listViewColors.DoDragDrop(listViewColors.SelectedItems[0].Tag, DragDropEffects.Copy);
		}

		private void listViewColors_DragEnter(object sender, DragEventArgs e)
		{
			_dragX = e.X;
			_dragY = e.Y;

			if (e.Data.GetDataPresent(typeof(ListView.SelectedListViewItemCollection)))
			{
				e.Effect = DragDropEffects.Move;
				return;
			}
			if (e.Data.GetDataPresent(typeof(Color)))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			e.Effect = DragDropEffects.None;
		}

		private void listViewColors_DragDrop(object sender, DragEventArgs e)
		{
			if (_dragX + 10 < e.X || _dragY + 10 < e.Y || _dragX - 10 > e.X || _dragY - 10 > e.Y)
			{
				Point p = listViewColors.PointToClient(new Point(e.X, e.Y));
				ListViewItem movetoNewPosition = listViewColors.GetItemAt(p.X, p.Y);
				if (e.Effect == DragDropEffects.Copy)
				{
					Color c = (Color) e.Data.GetData(typeof (Color));
					ListViewItem item = CreateColorListItem(c);
					if (movetoNewPosition != null)
					{
						int index = movetoNewPosition.Index;
						listViewColors.Items.Insert(index, item);
					}
					else
					{
						listViewColors.Items.Add(item);
					}
					
					ListViewItem_SetSpacing(listViewColors, _sideGap, _sideGap);
					Update_ColorOrder();

				}
				else if (e.Effect == DragDropEffects.Move)
				{
					listViewColors.BeginUpdate();
					listViewColors.Alignment = ListViewAlignment.Default;
					List<ListViewItem> listViewItems = listViewColors.SelectedItems.Cast<ListViewItem>().ToList();
					if (movetoNewPosition != null && listViewColors.SelectedItems[0].Index > movetoNewPosition.Index) listViewItems.Reverse();
					int index = movetoNewPosition?.Index ?? listViewColors.Items.Count - 1;
					foreach (ListViewItem item in listViewItems)
					{
						listViewColors.Items.Remove(item);
						listViewColors.Items.Insert(index, item);
					}

					listViewColors.Alignment = ListViewAlignment.Top;
					listViewColors.EndUpdate();
					Update_ColorOrder();
				}
				ImageSetup();
			}
		}

		#endregion

		#region Import/Export

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

		#region Adjust Color Icons height
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (e.Delta > 0)
					_colorLibraryScale = _colorLibraryScale * 1.03;
				else
				{
					_colorLibraryScale = _colorLibraryScale / 1.03;
				}
				ImageSetup();
				PopulateColors();
			}
		}

		#endregion

		private void Form_ColorLibrary_FormClosing(object sender, FormClosingEventArgs e)
		{
			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ColorLibraryScale", Name), _colorLibraryScale.ToString(CultureInfo.InvariantCulture));
		}

	}
}
