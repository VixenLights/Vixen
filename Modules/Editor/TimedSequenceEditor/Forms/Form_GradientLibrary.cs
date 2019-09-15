using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using VixenModules.App.ColorGradients;
using Vixen.Services;
using Vixen.Module.App;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_GradientLibrary : DockContent
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
		
		public bool LinkGradients
		{
			get { return checkBoxLinkGradients.Checked; }
			set { checkBoxLinkGradients.Checked = value; }
		}

		public TimelineControl TimelineControl { get; set; }

		#endregion

		#region Private Members

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ColorGradientLibrary _colorGradientLibrary;
		private string _lastFolder;
		private Size _imageSize;
		private double _gradientLibraryImageScale;
		private double _gradientLibraryTextScale;
		private Font _newFontSize;
		private int _dragX;
		private int _dragY;
		private bool _scaleText;

		#endregion

		#region Initialization

		public Form_GradientLibrary(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			toolStripGradients.Renderer = new ThemeToolStripRenderer();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripGradients.ImageScalingSize = new Size(iconSize, iconSize);

			toolStripButtonEditGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditGradient.Image = Tools.GetIcon(Resources.configuration, iconSize);
			toolStripButtonNewGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewGradient.Image = Tools.GetIcon(Resources.addItem, iconSize);
			toolStripButtonDeleteGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteGradient.Image = Tools.GetIcon(Resources.delete_32, iconSize);
			toolStripButtonExportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportGradients.Image = Tools.GetIcon(Resources.folder_go, iconSize);
			toolStripButtonImportGradients.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportGradients.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			listViewGradients.AllowDrop = true;

			var xml = new XMLProfileSettings();
			_gradientLibraryTextScale = Convert.ToDouble(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLibraryTextScale", Name), "1"), CultureInfo.InvariantCulture);
			_gradientLibraryImageScale = Convert.ToDouble(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLibraryImageScale", Name), "1"), CultureInfo.InvariantCulture);

			if (_gradientLibraryTextScale < 0.2)
				_gradientLibraryTextScale = 0.2;
			if (_gradientLibraryImageScale < 0.1)
				_gradientLibraryImageScale = 0.1;
			ImageSetup();

			ThemeUpdateControls.UpdateControls(this);
			//Over-ride the auto theme listview back color
			listViewGradients.BackColor = ThemeColorTable.BackgroundColor;
			_colorGradientLibrary = ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;
		}

		private void ImageSetup()
		{
			var t = (int)Math.Round(48 * _gradientLibraryImageScale * ScalingTools.GetScaleFactor());
			_imageSize = new Size(t, t);
			_newFontSize = new Font(Font.FontFamily.Name, (int)(7 * _gradientLibraryTextScale), Font.Style);
			short sideGap = (short)(_imageSize.Width + (5 * ScalingTools.GetScaleFactor()));
			short topGap = (short)(_imageSize.Height + 5 + ScalingTools.MeasureHeight(_newFontSize, "Test") * 2);

			ListViewItem_SetSpacing(listViewGradients, sideGap, topGap);
		}

		private void ColorPalette_Load(object sender, EventArgs e)
		{
			Load_Gradients();
		}

		public void Load_Gradients()
		{
			if (_colorGradientLibrary != null)
			{
				Populate_Gradients();
				_colorGradientLibrary.GradientsChanged += GradientsLibrary_GradientsChanged;
			}
		}

		#endregion

		#region Private Methods

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
						Font = _newFontSize,
						Tag = gradient
					};

					listViewGradients.Items.Add(item);
				}
			}
			
			listViewGradients.EndUpdate();
			toolStripButtonEditGradient.Enabled = toolStripButtonDeleteGradient.Enabled = false;
		}

		#endregion

		#region Event Handlers

		private void toolStripButtonEditGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 0)
				return;

			_colorGradientLibrary.EditLibraryItem(listViewGradients.SelectedItems[0].Name);
			OnGradientLibraryChanged();
		}

		private void toolStripButtonNewGradient_Click(object sender, EventArgs e)
		{
			AddGradientToLibrary(new ColorGradient());
		}

		internal bool AddGradientToLibrary(ColorGradient cg, bool edit = true)
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
						OnGradientLibraryChanged();
						return false;
					}

					if (messageBox.DialogResult == DialogResult.Cancel)
					{
						return true;
					}
				}
				else
				{
					_colorGradientLibrary.AddColorGradient(dialog.Response, cg);
					if (edit)
					{
						_colorGradientLibrary.EditLibraryItem(dialog.Response);
					}
					OnGradientLibraryChanged();
					return false;
				}
			}
			return true;
		}

		private void toolStripButtonDeleteGradient_Click(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 0)
				return;

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete the selected library gradient(s), ALL places it is used will be unlinked and will" +
								@" become independent gradients. Are you sure you want to continue?", "Delete library gradient?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
			{
				_colorGradientLibrary.BeginBulkUpdate();
				foreach (ListViewItem item in listViewGradients.SelectedItems) _colorGradientLibrary.RemoveColorGradient(item.Name);
				_colorGradientLibrary.EndBulkUpdate();
				OnGradientLibraryChanged();
			}
		}

		private void listViewGradients_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonDeleteGradient.Enabled = (listViewGradients.SelectedItems.Count > 0);
			toolStripButtonEditGradient.Enabled = (listViewGradients.SelectedItems.Count == 1);
		}

		private void listViewGradients_MouseDoubleClick(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 1)
				toolStripButtonEditGradient.PerformClick();
		}

		public void GradientsLibrary_GradientsChanged(object sender, EventArgs e)
		{
				Populate_Gradients();
		}

		public event EventHandler GradientLibraryChanged;

		private void OnGradientLibraryChanged()
		{
			GradientLibraryChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region Drag/Drop

		private void listViewGradient_ItemDrag(object sender, ItemDragEventArgs e)
		{
			listViewGradients.DoDragDrop(listViewGradients.SelectedItems[0], DragDropEffects.Move);
		}

		private void listViewGradients_DragLeave(object sender, EventArgs e)
		{
			if (listViewGradients.SelectedItems.Count == 0) return;
			ColorGradient newGradient = new ColorGradient((ColorGradient)listViewGradients.SelectedItems[0].Tag);
			if (LinkGradients) newGradient.LibraryReferenceName = listViewGradients.SelectedItems[0].Name;

			newGradient.IsCurrentLibraryGradient = false;
			listViewGradients.DoDragDrop(newGradient, DragDropEffects.Copy);
		}

		private void listViewGradients_DragEnter(object sender, DragEventArgs e)
		{
			_dragX = e.X;
			_dragY = e.Y;

			if (e.Data.GetDataPresent(typeof(ListViewItem)))
			{
				e.Effect = DragDropEffects.Move;
				return;
			}

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

		private void listViewGradients_DragDrop(object sender, DragEventArgs e)
		{
			if (_dragX + 10 < e.X || _dragY + 10 < e.Y || _dragX - 10 > e.X || _dragY - 10 > e.Y)
			{
				Point p = listViewGradients.PointToClient(new Point(e.X, e.Y));
				ListViewItem movetoNewPosition = listViewGradients.GetItemAt(p.X, p.Y);
				if (e.Effect == DragDropEffects.Copy)
				{
					ColorGradient c = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
					int index = movetoNewPosition?.Index ?? listViewGradients.Items.Count;

					if(AddGradientToLibrary(c, false)) return;

					Populate_Gradients();

					if (listViewGradients.Items.Count == _colorGradientLibrary.Count())
					{
						ListViewItem cloneToNew =
							(ListViewItem)listViewGradients.Items[listViewGradients.Items.Count - 1].Clone();
						listViewGradients.Items.Remove(listViewGradients.Items[listViewGradients.Items.Count - 1]);
						listViewGradients.Items.Insert(index, cloneToNew);
					}
				}
				else if (e.Effect == DragDropEffects.Move)
				{
					listViewGradients.BeginUpdate();
					listViewGradients.Alignment = ListViewAlignment.Default;
					List<ListViewItem> listViewItems = listViewGradients.SelectedItems.Cast<ListViewItem>().ToList();
					if (movetoNewPosition != null && listViewGradients.SelectedItems[0].Index > movetoNewPosition.Index) listViewItems.Reverse();
					int index = movetoNewPosition?.Index ?? listViewGradients.Items.Count - 1;
					foreach (ListViewItem item in listViewItems)
					{
						listViewGradients.Items.Remove(item);
						listViewGradients.Items.Insert(index, item);
					}
					listViewGradients.Alignment = ListViewAlignment.Top;
					listViewGradients.EndUpdate();
				}

				_colorGradientLibrary.BeginBulkUpdate();
				_colorGradientLibrary.Library.Clear();
				foreach (ListViewItem gradient in listViewGradients.Items) _colorGradientLibrary.Library[gradient.Text] = (ColorGradient)gradient.Tag;
				_colorGradientLibrary.EndBulkUpdate();
				ImageSetup();

				OnGradientLibraryChanged();
			}
		}

		#endregion

		#region Import/Export

		internal void ExportGradientLibrary()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vgl",
				Filter = @"Vixen 3 Color Gradient Library (*.vgl)|*.vgl|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlSettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			try
			{
				Dictionary<string, ColorGradient> gradients =
					_colorGradientLibrary.ToDictionary(gradient => gradient.Key, gradient => gradient.Value);

				DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, ColorGradient>));
				var writer = XmlWriter.Create(saveFileDialog.FileName, xmlSettings);
				ser.WriteObject(writer, gradients);
				writer.Close();
			}
			catch (Exception ex)
			{
				Logging.Error("While exporting Color Gradient Library: " + saveFileDialog.FileName + " " + ex.InnerException);
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon =
					SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to export data, please check the error log for details.",
					"Unable to export", false, false);
				messageBox.ShowDialog();
			}
		}

		internal void ImportGradientLibrary()
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
					gradients = (Dictionary<string, ColorGradient>) ser.ReadObject(reader);
				}

				_colorGradientLibrary.BeginBulkUpdate();
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

				_colorGradientLibrary.EndBulkUpdate();
				OnGradientLibraryChanged();
			}
			catch (Exception ex)
			{
				Logging.Error("Invalid file while importing Color Gradient Library: " + openFileDialog.FileName + " " +
				              ex.InnerException);
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon =
					SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox =
					new MessageBoxForm("Sorry, we didn't reconize the data in that file as valid Color Gradient Library data.",
						"Invalid file", false, false);
				messageBox.ShowDialog();
			}
		}

		#endregion

		#region Events

		private void toolStripButtonExportGradients_Click(object sender, EventArgs e)
		{
			ExportGradientLibrary();
		}

		private void toolStripButtonImportGradients_Click(object sender, EventArgs e)
		{
			ImportGradientLibrary();
		}

		private void Form_ToolPalette_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientsChanged -= GradientsLibrary_GradientsChanged;
			}
			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLibraryTextScale", Name), _gradientLibraryTextScale.ToString(CultureInfo.InvariantCulture));
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLibraryImageScale", Name), _gradientLibraryImageScale.ToString(CultureInfo.InvariantCulture));
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (_scaleText)
				{
					if (e.Delta > 0)
						_gradientLibraryTextScale = _gradientLibraryTextScale * 1.03;
					else
					{
						_gradientLibraryTextScale = _gradientLibraryTextScale / 1.03;
						if (_gradientLibraryTextScale < 0.2)
							_gradientLibraryTextScale = 0.2;
					}
					_newFontSize = new Font(Font.FontFamily.Name, (int)(7 * _gradientLibraryTextScale), Font.Style);

					foreach (ListViewItem item in listViewGradients.Items)
					{
						item.Font = _newFontSize;
					}
				}
				else
				{
					if (e.Delta > 0)
						_gradientLibraryImageScale = _gradientLibraryImageScale * 1.03;
					else
					{
						_gradientLibraryImageScale = _gradientLibraryImageScale / 1.03;
						if (_gradientLibraryImageScale < 0.1)
							_gradientLibraryImageScale = 0.1;
					}
					ImageSetup();
					Populate_Gradients();
				}
			}
		}

		private void listViewGradients_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Shift)
			{
				_scaleText = true;
			}
		}

		private void listViewGradients_KeyUp(object sender, KeyEventArgs e)
		{
			_scaleText = false;
		}

		#endregion
	}
}
