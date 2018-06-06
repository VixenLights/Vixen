using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using Common.Resources.Properties;
using Vixen.Sys;
using Common.Controls.Theme;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapEditor : Common.Controls.BaseForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private LipSyncMapData _mapping;
		private DataTable currentDataTable;
		private static Dictionary<string, Bitmap> _phonemeBitmaps = null;
		private List<string> _rowNames = null;
		private static string COLOR_COLUMN_NAME = "Color";
		//private bool _doMatrixUpdate = false;

		public LipSyncMapEditor()
		{
			_rowNames = new List<string>();
			this.LibraryMappingName = "Default";
			InitializeComponent();
			LoadResourceBitmaps();
		}

		public LipSyncMapEditor(LipSyncMapData mapData)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			dataGridView1.DefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dataGridView1.DefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			dataGridView1.RowHeadersDefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dataGridView1.RowHeadersDefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			Icon = Resources.Icon_Vixen3;
			//_doMatrixUpdate = false;
			LoadResourceBitmaps();
			this.MapData = mapData;
			//_doMatrixUpdate = true;
		}

		public string LibraryMappingName
		{
			get { return nameTextBox.Text; }
			set
			{
				nameTextBox.Text = value;
			}
		}

		private DataTable BuildDialogFromMap(LipSyncMapData data)
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
				DataRow dr = currentDataTable.Rows[currentRow];
				string elementName = dr[0].ToString();
				LipSyncMapItem item = new LipSyncMapItem();
				ElementNode theNode = FindElementNode(elementName);
					
				item.Name = dr[0].ToString();
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

				_mapping.MapItems.Add(item);
				currentRow++;
				if (currentRow >= currentDataTable.Rows.Count)
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
				
				currentDataTable = BuildDialogFromMap(value);
				UpdatedataGridView();
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
					_phonemeBitmaps.Add("AI", (Bitmap)lipSyncRM.GetObject("AI"));
					_phonemeBitmaps.Add("E", (Bitmap)lipSyncRM.GetObject("E"));
					_phonemeBitmaps.Add("ETC", (Bitmap)lipSyncRM.GetObject("etc"));
					_phonemeBitmaps.Add("FV", (Bitmap)lipSyncRM.GetObject("FV"));
					_phonemeBitmaps.Add("L", (Bitmap)lipSyncRM.GetObject("L"));
					_phonemeBitmaps.Add("MBP", (Bitmap)lipSyncRM.GetObject("MBP"));
					_phonemeBitmaps.Add("O", (Bitmap)lipSyncRM.GetObject("O"));
					_phonemeBitmaps.Add("REST", (Bitmap)lipSyncRM.GetObject("rest"));
					_phonemeBitmaps.Add("U", (Bitmap)lipSyncRM.GetObject("U"));
					_phonemeBitmaps.Add("WQ", (Bitmap)lipSyncRM.GetObject("WQ"));
				}
			}
		}

		private void LipSyncMapSetup_Load(object sender, EventArgs e)
		{
			UpdatedataGridView();
			this.ForeColor = ThemeColorTable.ForeColor;
			this.BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		private void UpdatedataGridView()
		{
			dataGridView1.DefaultCellStyle.ForeColor = ThemeColorTable.ForeColor;
			dataGridView1.DefaultCellStyle.BackColor = ThemeColorTable.BackgroundColor;
			dataGridView1.ForeColor = ThemeColorTable.ForeColor;
			dataGridView1.BackgroundColor = ThemeColorTable.BackgroundColor;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
			dataGridView1.ColumnHeadersHeight = 75;
			dataGridView1.MultiSelect = false;
			dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
			dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
			dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
			dataGridView1.DataSource = currentDataTable;

			//dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			for (int j = 1; j < dataGridView1.Columns.Count - 1; j++)
			{
				dataGridView1.Columns[j].Width = 50;
			}

			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width = 90;
			dataGridView1.Columns[COLOR_COLUMN_NAME].SortMode = DataGridViewColumnSortMode.NotSortable;

		}

		private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			e.PaintBackground(e.CellBounds, true);
			if (e.RowIndex == -1)
			{

				using (SolidBrush paintBrush = new SolidBrush(e.ColumnIndex !=0?ThemeColorTable.TextBoxBackgroundColor:ThemeColorTable.BackgroundColor))
				{
					e.Graphics.FillRectangle(paintBrush, e.CellBounds);
					//e.Graphics.DrawRectangle(new Pen(ThemeColorTable.ForeColor, 1),
					//							e.CellBounds.X,
					//							e.CellBounds.Y,
					//							e.CellBounds.Width,
					//							e.CellBounds.Height-1);
				}

				e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Top);
				var phonemeStr = (e.ColumnIndex == -1) ? "" : e.FormattedValue.ToString();

				var stringSize = e.Graphics.MeasureString(phonemeStr, e.CellStyle.Font);
				var stringLocation = (e.CellBounds.Width - stringSize.Width) / 2;

				Bitmap phonemeBitmap;
				if (_phonemeBitmaps.TryGetValue(phonemeStr, out phonemeBitmap))
				{
					e.Graphics.DrawImage(new Bitmap(_phonemeBitmaps[phonemeStr], 48, 48), (e.CellBounds.Width - 48) / 2, 1);
				}
				
				e.Graphics.DrawString(phonemeStr, e.CellStyle.Font, new SolidBrush(ThemeColorTable.ForeColor), stringLocation, e.CellBounds.Bottom - stringSize.Height - 5);

				e.Graphics.ResetTransform();
				e.Handled = true;
			}

			if (e.RowIndex > -1)
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
				else if (e.ColumnIndex >= currentDataTable.Columns.Count - 1)
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

		}


		private void LipSyncBreakdownSetup_Resize(object sender, EventArgs e)
		{
			dataGridView1.Size = new Size(this.Size.Width - 40, this.Size.Height - 150);
		}

		private void ReconfigureDataTable()
		{
			currentDataTable.Rows.Clear();
			currentDataTable = BuildDialogFromMap(_mapping);
			UpdatedataGridView();
		}

		private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if ((e.RowIndex > -1) && (e.ColumnIndex > 0))
			{
				BuildMapDataFromDialog();
			}
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			int lastColumn = dataGridView1.Columns.Count - 1;
			if (e.RowIndex > -1)
			{
				if ((e.ColumnIndex > 0) && (e.ColumnIndex < lastColumn))
				{
					int bias = 0;
					foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
					{
						if ((cell.ColumnIndex > 0) && (cell.ColumnIndex < lastColumn))
						{
							bias = ((cell.Value.GetType() == typeof(bool)) && 
								(bool)cell.Value == true) ? bias + 1 : bias - 1;
						}

					}

					bool newValue = (bias > 0) ? false : true;

					foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
					{
						if ((cell.ColumnIndex > 0) && (cell.ColumnIndex < lastColumn))
						{
							cell.Value = newValue;
						}
					}
				}
			}
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			List<ElementNode> chosenNodes = new List<ElementNode>();


			int lastColumn = currentDataTable.Columns.Count - 1;
			if (e.RowIndex > -1)
			{
				if (e.ColumnIndex == lastColumn)
				{

					LipSyncMapColorSelect colorDialog1 = new LipSyncMapColorSelect();

					foreach (DataGridViewCell selCell in dataGridView1.SelectedCells)
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
					colorDialog1.Color = (Color)dataGridView1.SelectedCells[0].Value;

					// Show the color dialog.
					DialogResult result = colorDialog1.ShowDialog();

					// See if user pressed ok.
					if (result == DialogResult.OK)
					{
						currentDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = false;
						foreach (DataGridViewCell selCell in dataGridView1.SelectedCells)
						{
							if (selCell.ColumnIndex == lastColumn)
							{
								selCell.Value = colorDialog1.Color;
							}
						}
						DataGridViewCell cell = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
						cell.Value = colorDialog1.Color;
						currentDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = true;
					}
				}
			}
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			dataGridView1.ClearSelection();
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

				ReconfigureDataTable();
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
