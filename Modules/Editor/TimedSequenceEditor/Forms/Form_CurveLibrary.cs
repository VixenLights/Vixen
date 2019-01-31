using System;
using System.Collections;
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
using VixenModules.App.Curves;
using Vixen.Services;
using Vixen.Module.App;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using Catel.Linq;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_CurveLibrary : DockContent
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

		public TimelineControl TimelineControl { get; set; }

		#endregion

		#region Private Members

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private CurveLibrary _curveLibrary;
		private string _lastFolder;
		private Size _imageSize;
		private double _curveLibraryImageScale;
		private double _curveLibraryTextScale;
		private Font _newFontSize;
		private int _dragX;
		private int _dragY;
		private bool _scaleText;

		#endregion

		#region Initialization

		public Form_CurveLibrary(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			toolStripCurves.Renderer = new ThemeToolStripRenderer();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripCurves.ImageScalingSize = new Size(iconSize, iconSize);

			toolStripButtonEditCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditCurve.Image = Tools.GetIcon(Resources.configuration, iconSize);
			toolStripButtonNewCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonNewCurve.Image = Tools.GetIcon(Resources.addItem, iconSize);
			toolStripButtonDeleteCurve.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteCurve.Image = Tools.GetIcon(Resources.delete_32, iconSize);
			toolStripButtonExportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExportCurves.Image = Tools.GetIcon(Resources.folder_go, iconSize);
			toolStripButtonImportCurves.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImportCurves.Image = Tools.GetIcon(Resources.folder_open, iconSize);

			listViewCurves.AllowDrop = true;

			var xml = new XMLProfileSettings();
			_curveLibraryImageScale = Convert.ToDouble(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLibraryImageScale", Name), "1"), CultureInfo.InvariantCulture);
			_curveLibraryTextScale = Convert.ToDouble(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLibraryTextScale", Name), "1"), CultureInfo.InvariantCulture);

			if (_curveLibraryImageScale < 0.1)
				_curveLibraryImageScale = 0.1;
			if (_curveLibraryTextScale < 0.2)
				_curveLibraryTextScale = 0.2;
			ImageSetup();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			//Over-ride the auto theme listview back color
			listViewCurves.BackColor = ThemeColorTable.BackgroundColor;

			listViewCurves.Alignment = ListViewAlignment.Top;
		}

		private void ImageSetup()
		{
			var t = (int)Math.Round(48 * _curveLibraryImageScale * ScalingTools.GetScaleFactor());
			_imageSize = new Size(t, t);
			_newFontSize = new Font(Font.FontFamily.Name, (int)(7 * _curveLibraryTextScale), Font.Style);
			short sideGap = (short)(_imageSize.Width + (5 * ScalingTools.GetScaleFactor()));
			short topGap = (short)(_imageSize.Height + 5 + ScalingTools.MeasureHeight(_newFontSize, "Test") * 2);

			ListViewItem_SetSpacing(listViewCurves, sideGap, topGap);
		}

		private void ColorPalette_Load(object sender, EventArgs e)
		{
			Load_Curves();
		}

		public void Load_Curves()
		{
			_curveLibrary = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;
			if (_curveLibrary != null)
			{
				Populate_Curves();
				_curveLibrary.CurvesChanged += CurveLibrary_CurvesChanged;
			}
		}

		#endregion

		#region Private Methods
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
					gfx.DrawRectangle(p, 0, 0, image.Width, image.Height);
					gfx.Dispose();
					listViewCurves.LargeImageList.Images.Add(name, image);

					ListViewItem item = new ListViewItem
					{
						Text = name,
						Name = name,
						ImageKey = name,
						Font = _newFontSize,
						Tag = c
					};
					
					listViewCurves.Items.Add(item);
				}
			}
			
			listViewCurves.EndUpdate();
			toolStripButtonEditCurve.Enabled = toolStripButtonDeleteCurve.Enabled = false;
		}

		#endregion

		#region Event Handlers

		private void toolStripButtonEditCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			_curveLibrary.EditLibraryCurve(listViewCurves.SelectedItems[0].Name);
			_SelectionChanged();
		}

		private void toolStripButtonNewCurve_Click(object sender, EventArgs e)
		{
			AddCurveToLibrary(new Curve());
		}

		private bool AddCurveToLibrary(Curve c, bool edit=true)
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
						_SelectionChanged();
						return false;
					}

					if (messageBox.DialogResult == DialogResult.Cancel)
					{
						return true;
					}
				}
				else
				{
					_curveLibrary.AddCurve(dialog.Response, c);
					if (edit)
					{
						_curveLibrary.EditLibraryCurve(dialog.Response);
					}
					_SelectionChanged();
					return false;
				}
			}
			return true;
		}

		private void toolStripButtonDeleteCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete the Selected library curve(s), ALL places it is used will be unlinked and will" +
								@" become independent curves. Are you sure you want to continue?", "Delete library curve?", true, true);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
			{
				_curveLibrary.BeginBulkUpdate();
				foreach (ListViewItem item in listViewCurves.SelectedItems) _curveLibrary.RemoveCurve(item.Name);
				_curveLibrary.EndBulkUpdate();
				_SelectionChanged();
			}
		}

		private void listViewCurves_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolStripButtonDeleteCurve.Enabled = (listViewCurves.SelectedItems.Count > 0);
			toolStripButtonEditCurve.Enabled = (listViewCurves.SelectedItems.Count == 1);
		}

		private void listViewCurves_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 1)
				toolStripButtonEditCurve.PerformClick();
		}

		private void CurveLibrary_CurvesChanged(object sender, EventArgs e)
		{
				Populate_Curves();
		}

		public event EventHandler SelectionChanged;

		private void _SelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		#endregion

		#region Drag/Drop

		private void listViewCurves_ItemDrag(object sender, ItemDragEventArgs e)
		{
			listViewCurves.DoDragDrop(listViewCurves.SelectedItems[0], DragDropEffects.Move);
		}

		private void listViewCurves_DragLeave(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0) return;
			Curve newCurve = new Curve((Curve) listViewCurves.SelectedItems[0].Tag);
			if (LinkCurves) newCurve.LibraryReferenceName = listViewCurves.SelectedItems[0].Name;

			newCurve.IsCurrentLibraryCurve = false;
			listViewCurves.DoDragDrop(newCurve, DragDropEffects.Copy);
		}

		private void listViewCurves_DragEnter(object sender, DragEventArgs e)
		{
			_dragX = e.X;
			_dragY = e.Y;

			if (e.Data.GetDataPresent(typeof(ListViewItem)))
			{
				e.Effect = DragDropEffects.Move;
				return;
			}

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
			if (_dragX + 10 < e.X || _dragY + 10 < e.Y || _dragX - 10 > e.X || _dragY - 10 > e.Y)
			{
				Point p = listViewCurves.PointToClient(new Point(e.X, e.Y));
				ListViewItem movetoNewPosition = listViewCurves.GetItemAt(p.X, p.Y);
				if (e.Effect == DragDropEffects.Copy)
				{
					Curve c = (Curve) e.Data.GetData(typeof(Curve));
					int index = movetoNewPosition?.Index ?? listViewCurves.Items.Count;
					
					if(AddCurveToLibrary(c, false)) return;
					
					Populate_Curves();

					if (listViewCurves.Items.Count == _curveLibrary.Count())
					{
						ListViewItem cloneToNew =
							(ListViewItem) listViewCurves.Items[listViewCurves.Items.Count - 1].Clone();
						listViewCurves.Items.Remove(listViewCurves.Items[listViewCurves.Items.Count - 1]);
						listViewCurves.Items.Insert(index, cloneToNew);
					}
				}
				else if (e.Effect == DragDropEffects.Move)
				{
					listViewCurves.BeginUpdate();
					listViewCurves.Alignment = ListViewAlignment.Default;
					List<ListViewItem> listViewItems = listViewCurves.SelectedItems.Cast<ListViewItem>().ToList();
					if (movetoNewPosition != null && listViewCurves.SelectedItems[0].Index > movetoNewPosition.Index) listViewItems.Reverse();
					int index = movetoNewPosition?.Index ?? listViewCurves.Items.Count - 1;
					foreach (ListViewItem item in listViewItems)
					{
						listViewCurves.Items.Remove(item);
						listViewCurves.Items.Insert(index, item);
					}
					listViewCurves.Alignment = ListViewAlignment.Top;
					listViewCurves.EndUpdate();
				}

				_curveLibrary.BeginBulkUpdate();
				_curveLibrary.Library.Clear();
				foreach (ListViewItem curve in listViewCurves.Items) _curveLibrary.Library[curve.Text] = (Curve)curve.Tag;
				_curveLibrary.EndBulkUpdate();
				ImageSetup();
				_SelectionChanged();
			}
		}

		#endregion

		#region Import/Export

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

				_curveLibrary.BeginBulkUpdate();
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
				_curveLibrary.EndBulkUpdate();
				_SelectionChanged();
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

		private void Form_ToolPalette_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_curveLibrary != null)
			{
				_curveLibrary.CurvesChanged -= CurveLibrary_CurvesChanged;
			}
			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLibraryImageScale", Name), _curveLibraryImageScale.ToString(CultureInfo.InvariantCulture));
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLibraryTextScale", Name), _curveLibraryTextScale.ToString(CultureInfo.InvariantCulture));

		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (_scaleText)
				{
					if (e.Delta > 0)
						_curveLibraryTextScale = _curveLibraryTextScale * 1.03;
					else
					{
						_curveLibraryTextScale = _curveLibraryTextScale / 1.03;
						if (_curveLibraryTextScale < 0.2)
							_curveLibraryTextScale = 0.2;
					}
					_newFontSize = new Font(Font.FontFamily.Name, (int)(7 * _curveLibraryTextScale), Font.Style);
					
					foreach (ListViewItem item in listViewCurves.Items)
					{
						item.Font = _newFontSize;
					}

				}
				else
				{
					if (e.Delta > 0)
						_curveLibraryImageScale = _curveLibraryImageScale * 1.03;
					else
					{
						_curveLibraryImageScale = _curveLibraryImageScale / 1.03;
						if (_curveLibraryImageScale < 0.1)
							_curveLibraryImageScale = .1;
					}
					ImageSetup();
					Populate_Curves();
				}
				
			}
		}

		private void listViewCurves_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Shift)
			{
				_scaleText = true;
			}
		}

		private void listViewCurves_KeyUp(object sender, KeyEventArgs e)
		{
			_scaleText = false;
		}
	}
}
