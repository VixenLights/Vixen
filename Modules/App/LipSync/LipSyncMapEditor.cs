using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using Common.Resources.Properties;
using Vixen.Sys;
using Common.Controls.Theme;
using Vixen.Extensions;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapEditor : Common.Controls.BaseForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private LipSyncMapData _mapping;
		private DataTable _mouthDataTable;
		private DataTable _otherDataTable;
		private static Dictionary<string, Bitmap> _phonemeBitmaps;
		private static Dictionary<string, Bitmap> _faceComponentBitmaps;
		private List<string> _rowNames = null;
		private static string COLOR_COLUMN_NAME = "Color";
		
		public LipSyncMapEditor(LipSyncMapData mapData)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			LoadResourceBitmaps();
			MapData = mapData;
			tabControl.SelectedIndex = 0;
		}

		public string LibraryMappingName
		{
			get { return nameTextBox.Text; }
			set
			{
				nameTextBox.Text = value;
			}
		}

		private DataTable BuildMouthDialogFromMap(LipSyncMapData data)
		{
			nameTextBox.Text = data.LibraryReferenceName;
			notesTextBox.Text = data.Notes;

			DataTable dt = new DataTable(nameTextBox.Text);
			dt.Columns.Add(" ", typeof(string));

			foreach (string key in _phonemeBitmaps.Keys)
			{
				dt.Columns.Add(key, typeof(Boolean));
			}

			dt.Columns.Add(COLOR_COLUMN_NAME, typeof(Color));

			foreach (LipSyncMapItem lsbItem in data.MapItems)
			{
				DataRow dr = dt.Rows.Add();
				dr[0] = lsbItem.Name;
				ElementNode tempNode = VixenSystem.Nodes.GetElementNode(lsbItem.ElementGuid);
				if (tempNode != null)
				{
					dr[0] = (tempNode.Element == null) ? tempNode.Name : tempNode.Element.Name;
				}

				foreach (string key in _phonemeBitmaps.Keys)
				{
					bool result;
					if (lsbItem.PhonemeList.TryGetValue(key, out result))
					{
						dr[key] = result;
					}
					else
					{
						dr[key] = false;
					}
				}
				dr[COLOR_COLUMN_NAME] = lsbItem.ElementColor;
			}

			dt.Columns[" "].ReadOnly = true;
			dt.Columns[COLOR_COLUMN_NAME].ReadOnly = true;

			return dt;
		}

		private DataTable BuildOtherDialogFromMap(LipSyncMapData data)
		{
			nameTextBox.Text = data.LibraryReferenceName;
			notesTextBox.Text = data.Notes;

			DataTable dt = new DataTable(nameTextBox.Text);
			dt.Columns.Add(" ", typeof(string));

			dt.Columns.Add(FaceComponent.Outlines.GetEnumDescription(), typeof(Boolean));
			dt.Columns.Add(FaceComponent.EyesOpen.GetEnumDescription(), typeof(Boolean));
			dt.Columns.Add(FaceComponent.EyesClosed.GetEnumDescription(), typeof(Boolean));
			
			dt.Columns.Add(COLOR_COLUMN_NAME, typeof(Color));

			foreach (LipSyncMapItem lsbItem in data.MapItems)
			{
				DataRow dr = dt.Rows.Add();
				dr[0] = lsbItem.Name;
				ElementNode tempNode = VixenSystem.Nodes.GetElementNode(lsbItem.ElementGuid);
				if (tempNode != null)
				{
					dr[0] = (tempNode.Element == null) ? tempNode.Name : tempNode.Element.Name;
				}

				foreach (FaceComponent key in Enum.GetValues(typeof(FaceComponent)))
				{
					if(key == FaceComponent.Mouth) continue;
					bool result;
					if (lsbItem.FaceComponents.TryGetValue(key, out result))
					{
						dr[key.GetEnumDescription()] = result;
					}
					else
					{
						dr[key.GetEnumDescription()] = false;
					}
				}
				dr[COLOR_COLUMN_NAME] = lsbItem.ElementColor;
			}

			dt.Columns[" "].ReadOnly = true;
			dt.Columns[COLOR_COLUMN_NAME].ReadOnly = true;

			return dt;
		}

		private ElementNode FindElementNode(string elementName)
		{
			ElementNode theNode = VixenSystem.Nodes.ToList().Find(
				delegate(ElementNode node)
				{
					if (node.IsLeaf & node.Element != null)
					{
						return node.Element.Name.Equals(elementName);
					}
					return node.Name.Equals(elementName);
				}
			);

			return theNode;
		}

		private void BuildMapDataFromDialog()
		{
			int currentRow = 0;

			_mapping.LibraryReferenceName = nameTextBox.Text;
			_mapping.Notes = notesTextBox.Text;

			_mapping.StringCount = _rowNames.Count();
			_mapping.MapItems.Clear();

			for (int stringNum = 0; stringNum < _rowNames.Count; stringNum++)
			{
				DataRow dr = _mouthDataTable.Rows[currentRow];
				var otherDataRow = _otherDataTable.Rows[currentRow];
				string elementName = dr[0].ToString();
				LipSyncMapItem item = new LipSyncMapItem();
				ElementNode theNode = FindElementNode(elementName);
					
				item.Name = theNode.Name;
				item.ElementGuid = theNode.Id;
				item.StringNum = stringNum;

				for (int theCount = 1; theCount < dr.ItemArray.Count() - 1; theCount++) 
				{
					bool checkVal = dr[theCount] is bool && (Boolean)dr[theCount];
					item.PhonemeList.Add(
						dr.Table.Columns[theCount].ColumnName, checkVal
						);
				}
				item.ElementColor = (Color)dr[dr.ItemArray.Count() - 1];

				foreach (FaceComponent key in Enum.GetValues(typeof(FaceComponent)))
				{
					if (key == FaceComponent.Mouth)
					{
						item.FaceComponents.Add(key, item.PhonemeList.Values.Any(x => x));
						continue;
					}
					
					var value = otherDataRow[key.GetEnumDescription()];
					if (value is bool)
					{
						item.FaceComponents.Add(key, (bool)value);
					}

				}
				_mapping.MapItems.Add(item);
				currentRow++;
				if (currentRow >= _mouthDataTable.Rows.Count)
				{
					return;
				}
			}
		}

		public LipSyncMapData MapData
		{
			get
			{
				BuildMapDataFromDialog();
				return _mapping;
			}

			set
			{
				ElementNode tempNode = null;
				_mapping = value;
				_rowNames = new List<string>();
				try
				{
					foreach(LipSyncMapItem mapItem in _mapping.MapItems)
					{
						tempNode = VixenSystem.Nodes.GetElementNode(mapItem.ElementGuid);
						if (tempNode == null)
						{
							continue;
						}
						
						if (tempNode.Element != null)
						{
							_rowNames.Add(tempNode.Element.Name);
						}
						else
						{
							_rowNames.Add(tempNode.Name);
						}
					}
				}
				catch (Exception e) { };
				
				_mouthDataTable = BuildMouthDialogFromMap(value);
				_otherDataTable = BuildOtherDialogFromMap(value);
				UpdateMouthDataGridView();
				UpdateOtherDataGridView();
			}
		}

		private void LoadResourceBitmaps()
		{
			if (_phonemeBitmaps == null)
			{
				Assembly assembly = Assembly.Load("LipSyncApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
				if (assembly != null)
				{
					ResourceManager lipSyncRM = new ResourceManager("VixenModules.App.LipSyncApp.LipSyncResources", assembly);
					_phonemeBitmaps = new Dictionary<string, Bitmap>();
					_phonemeBitmaps.Add("AI", (Bitmap)lipSyncRM.GetObject("AI_Transparent"));
					_phonemeBitmaps.Add("E", (Bitmap)lipSyncRM.GetObject("E_Transparent"));
					_phonemeBitmaps.Add("ETC", (Bitmap)lipSyncRM.GetObject("etc_Transparent"));
					_phonemeBitmaps.Add("FV", (Bitmap)lipSyncRM.GetObject("FV_Transparent"));
					_phonemeBitmaps.Add("L", (Bitmap)lipSyncRM.GetObject("L_Transparent"));
					_phonemeBitmaps.Add("MBP", (Bitmap)lipSyncRM.GetObject("MBP_Transparent"));
					_phonemeBitmaps.Add("O", (Bitmap)lipSyncRM.GetObject("O_Transparent"));
					_phonemeBitmaps.Add("REST", (Bitmap)lipSyncRM.GetObject("rest_Transparent"));
					_phonemeBitmaps.Add("U", (Bitmap)lipSyncRM.GetObject("U_Transparent"));
					_phonemeBitmaps.Add("WQ", (Bitmap)lipSyncRM.GetObject("WQ_Transparent"));

					_faceComponentBitmaps = new Dictionary<string, Bitmap>
					{
						{FaceComponent.EyesOpen.GetEnumDescription(), (Bitmap) lipSyncRM.GetObject("WQ_Transparent")},
						{FaceComponent.EyesClosed.GetEnumDescription(), (Bitmap) lipSyncRM.GetObject("WQ_Transparent")},
						{FaceComponent.Outlines.GetEnumDescription(), (Bitmap) lipSyncRM.GetObject("WQ_Transparent")}
					};
				}
			}
		}

		private void LipSyncMapSetup_Load(object sender, EventArgs e)
		{
			SetGridDefaults(dataGridViewMouth);
			SetGridDefaults(dataGridViewOther);
			this.ForeColor = ThemeColorTable.ForeColor;
			this.BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		private void UpdateMouthDataGridView()
		{
			dataGridViewMouth.DataSource = _mouthDataTable;
			
			ConfigureColumns(dataGridViewMouth, 50);
		}

		private void UpdateOtherDataGridView()
		{
			dataGridViewOther.DataSource = _otherDataTable;

			ConfigureColumns(dataGridViewOther, -1);
		}

		private void ConfigureColumns(DataGridView dgv, int width)
		{
			for (int j = 1; j < dgv.Columns.Count - 1; j++)
			{
				if (width < 0)
				{
					dgv.Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				}
				else
				{
					dgv.Columns[j].Width = width;
				}
			}
			dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dgv.Columns[dgv.Columns.Count - 1].Width = 90;
			dgv.ColumnHeadersHeight = 75;
			dgv.Columns[COLOR_COLUMN_NAME].SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		private void SetGridDefaults(DataGridView dgv)
		{
			dgv.DefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dgv.DefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
		    dgv.RowHeadersDefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dgv.RowHeadersDefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			dgv.ForeColor = ThemeColorTable.ForeColor;
			dgv.BackgroundColor = ThemeColorTable.BackgroundColor;
			dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
			dgv.MultiSelect = true;
			dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dgv.EditMode = DataGridViewEditMode.EditOnEnter;
			dgv.AllowUserToAddRows = false;
			dgv.RowHeadersVisible = false;
			dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
		}

		private void ReconfigureMouthDataTable()
		{
			_mouthDataTable.Rows.Clear();
			_mouthDataTable = BuildMouthDialogFromMap(_mapping);
			UpdateMouthDataGridView();
		}

		private void ReconfigureOtherDataTable()
		{
			_otherDataTable.Rows.Clear();
			_otherDataTable = BuildOtherDialogFromMap(_mapping);
			UpdateOtherDataGridView();
		}

		private void dataGridViewOther_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			e.PaintBackground(e.CellBounds, true);
			if (e.RowIndex == -1)
			{
				PaintHeaderCell(e, e.FormattedValue.ToString(), _faceComponentBitmaps);
			}

			if (e.RowIndex > -1)
			{
				PaintRowCell(e, _otherDataTable.Columns.Count - 1);
			}
		}

		private void dataGridViewMouth_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			e.PaintBackground(e.CellBounds, true);
			if (e.RowIndex == -1)
			{
				var phonemeStr = (e.ColumnIndex == -1) ? "" : e.FormattedValue.ToString();
				PaintHeaderCell(e, phonemeStr, _phonemeBitmaps);
			}

			if (e.RowIndex > -1)
			{
				PaintRowCell(e, _mouthDataTable.Columns.Count - 1);
			}

		}

		private static void PaintHeaderCell(DataGridViewCellPaintingEventArgs e, string text, Dictionary<string, Bitmap> imageLookup)
		{
			using (SolidBrush paintBrush = new SolidBrush(ThemeColorTable.BackgroundColor))
			{
				e.Graphics.FillRectangle(paintBrush, e.CellBounds);
			}

			e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Top);

			var stringSize = e.Graphics.MeasureString(text, e.CellStyle.Font);
			var stringLocation = (e.CellBounds.Width - stringSize.Width) / 2;

			Bitmap bitmap;
			if (imageLookup.TryGetValue(text, out bitmap))
			{
				e.Graphics.DrawImage(new Bitmap(bitmap, 48, 48), (e.CellBounds.Width - 48) / 2, 1);
			}

			e.Graphics.DrawString(text, e.CellStyle.Font, new SolidBrush(ThemeColorTable.ForeColor),
				stringLocation, e.CellBounds.Bottom - stringSize.Height - 5);

			e.Graphics.ResetTransform();
			e.Handled = true;
		}

		private static void PaintRowCell(DataGridViewCellPaintingEventArgs e, int colorColumn)
		{
			if (e.ColumnIndex == -1)
			{
				using (SolidBrush paintBrush = new SolidBrush(ThemeColorTable.TextBoxBackgroundColor))
				{
					e.Graphics.FillRectangle(paintBrush, e.CellBounds);
					e.Graphics.DrawRectangle(new Pen(ThemeColorTable.ForeColor, 1),
						e.CellBounds.X,
						e.CellBounds.Y,
						e.CellBounds.Width - 1,
						e.CellBounds.Height);
				}

				e.Handled = true;
			}
			else if (e.ColumnIndex == colorColumn && e.Value is Color)
			{
				e.Graphics.DrawRectangle(new Pen(ThemeColorTable.ForeColor, 2), e.CellBounds);
				e.CellStyle.ForeColor = (Color)e.Value;
				e.CellStyle.BackColor = (Color)e.Value;
				e.CellStyle.SelectionForeColor = (Color)e.Value;
				e.CellStyle.SelectionBackColor = (Color)e.Value;
			}
			else
			{
				e.CellStyle.ForeColor = ThemeColorTable.ForeColor;
				e.CellStyle.BackColor = ThemeColorTable.BackgroundColor;
			}
		}

		private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView view = sender as DataGridView;
			if (view == null)
			{
				//How did that happen?
				return;
			}
			int lastColumn = view.Columns.Count - 1;
			if (e.RowIndex > -1)
			{
				if ((e.ColumnIndex > 0) && (e.ColumnIndex < lastColumn))
				{
					int bias = 0;
					foreach (DataGridViewCell cell in view.SelectedCells)
					{
						if ((cell.ColumnIndex > 0) && (cell.ColumnIndex < lastColumn))
						{
							bias = ((cell.Value is bool) && 
								(bool)cell.Value) ? bias + 1 : bias - 1;
						}

					}

					bool newValue = (bias <= 0);

					foreach (DataGridViewCell cell in view.SelectedCells)
					{
						if ((cell.ColumnIndex > 0) && (cell.ColumnIndex < lastColumn))
						{
							cell.Value = newValue;
						}
					}
				}
			}
		}

		private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView view = sender as DataGridView;
			if (view == null)
			{
				//How did that happen?
				return;
			}

			int lastColumn = view.Columns.Count - 1;
			if (e.RowIndex > -1)
			{
				if (e.ColumnIndex == lastColumn)
				{
					List<ElementNode> chosenNodes = new List<ElementNode>();
					LipSyncMapColorSelect colorDialog1 = new LipSyncMapColorSelect();

					foreach (DataGridViewCell selCell in view.SelectedCells)
					{
						if (selCell.ColumnIndex == lastColumn)
						{
							ElementNode theNode = FindElementNode((string)selCell.OwningRow.Cells[0].Value);
							if (theNode != null)
							{
								chosenNodes.Add(theNode);
							}
						}
					}

					colorDialog1.ChosenNodes = chosenNodes;
					colorDialog1.Color = (Color)view.SelectedCells[0].Value;

					// Show the color dialog.
					DialogResult result = colorDialog1.ShowDialog();

					// See if user pressed ok.
					if (result == DialogResult.OK)
					{
						_mouthDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = false;
						_otherDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = false;
						foreach (DataGridViewCell selCell in view.SelectedCells)
						{
							if (selCell.ColumnIndex == lastColumn)
							{
								_mouthDataTable.Rows[selCell.RowIndex][_mouthDataTable.Columns[COLOR_COLUMN_NAME]] = colorDialog1.Color;
								_otherDataTable.Rows[selCell.RowIndex][_otherDataTable.Columns[COLOR_COLUMN_NAME]] = colorDialog1.Color;
								//selCell.Value = colorDialog1.Color;
							}
						}
						DataGridViewCell cell = view.Rows[e.RowIndex].Cells[e.ColumnIndex];
						cell.Value = colorDialog1.Color;
						_mouthDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = true;
						_otherDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = true;
					}
				}
			}
		}

		private void buttonAssign_Click(object sender, EventArgs e)
		{
			LipSyncNodeSelect nodeSelectDlg = new LipSyncNodeSelect();
			//nodeSelectDlg.MaxNodes = _mapping.MapItems.Count;
			nodeSelectDlg.MaxNodes = Int32.MaxValue;
			nodeSelectDlg.MatrixOptionsOnly = false;
			nodeSelectDlg.SelectedNodeNames = _rowNames;
			nodeSelectDlg.AllowGroups = _mapping.GroupsAllowed;
			nodeSelectDlg.AllowRecursiveAdd = _mapping.RecursionAllowed;

			DialogResult dr = nodeSelectDlg.ShowDialog();
			if ((dr == DialogResult.OK) && (nodeSelectDlg.Changed == true))
			{
				List<LipSyncMapItem> newMappings = new List<LipSyncMapItem>();
				LipSyncMapItem tempMapItem = null;

				_mapping.LibraryReferenceName = nameTextBox.Text;
				_mapping.GroupsAllowed = nodeSelectDlg.AllowGroups;
				_mapping.RecursionAllowed = nodeSelectDlg.AllowRecursiveAdd;

				_rowNames.Clear();
				_rowNames.AddRange(nodeSelectDlg.SelectedNodeNames);

				foreach (string nodeName in nodeSelectDlg.SelectedNodeNames)
				{
					tempMapItem = _mapping.MapItems.Find(
						delegate(LipSyncMapItem item)
						{
							return item.Name.Equals(nodeName);
						});

					if (tempMapItem != null)
					{
						newMappings.Add(tempMapItem);
					}
					else
					{
						newMappings.Add(new LipSyncMapItem(nodeName,-1));
					}
				}

				_mapping.MapItems.Clear();
				
				int stringCount = 0;
				foreach (LipSyncMapItem mapItem in newMappings)
				{
					mapItem.StringNum = stringCount++;
					_mapping.MapItems.Add(mapItem);
				}

				ReconfigureMouthDataTable();
				ReconfigureOtherDataTable();
			}
			Refresh();
		}

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

		
	}
}
