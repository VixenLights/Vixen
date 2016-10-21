using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapMatrixEditor : BaseForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private LipSyncMapData _origMapping;
        private LipSyncMapData _newMapping;
        private Dictionary<PhonemeType, DataTable> dataTables;
        private static Dictionary<PhonemeType, Bitmap> _phonemeBitmaps = null;
        private List<string> _rowNames = null;
        private bool _doMatrixUpdate = true;
        private int zoomSteps = 1;
        private const int ZOOM_STEP_DELTA = 1;
        private const int CELL_BASE_WIDTH = 50;
        private int currentPhonemeIndex;
        private PhonemeType[] phonemeArray;
        private bool stringsAreRows;
        private int startMapIndex = -1;
        private int stringCount = 1;
        private int pixelCount = 1;
        

        public LipSyncMapMatrixEditor()
        {
            _rowNames = new List<string>();
            this.LibraryMappingName = "Default";
            InitializeComponent();
            LoadResourceBitmaps();
            dataTables = new Dictionary<PhonemeType, DataTable>();
            stringsAreRows = true;
            startMapIndex = -1;
            zoomSteps = 1;
            
        }

        public LipSyncMapMatrixEditor(LipSyncMapData mapData)
        {
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X - 150, ActiveForm.Location.Y - 100) : new Point(200, 100);
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			lipSyncMapColorCtrl1.BackColor = BackColor = ThemeColorTable.BackgroundColor;
			lipSyncMapColorCtrl1.panelColor.BackColor = Color.White;
	        zoomTrackbar.BackColor = ThemeColorTable.BackgroundColor;
			Icon = Resources.Icon_Vixen3;
            LoadResourceBitmaps();
            dataTables = new Dictionary<PhonemeType, DataTable>();
            this.MapData = (LipSyncMapData)mapData.Clone();
            stringsAreRows = mapData.StringsAreRows;
            startMapIndex = -1;
            zoomSteps = mapData.ZoomLevel;
        }

        public string LibraryMappingName
        {
            get { return nameTextBox.Text; }
            set
            {
                nameTextBox.Text = value;
            }
        }

        private int CalcNumDataGridRows
        {
            get
            {
                return (stringsAreRows) ? stringCount : pixelCount;
            }
        }

        private int CalcNumDataGridCols
        {
            get
            {
                return (stringsAreRows) ? pixelCount : stringCount;
            }
        }

        private LipSyncMapItem FindRenderMapItem (int row, int column)
        {
            
            if (startMapIndex == -1)
            {
                startMapIndex = _newMapping.MapItems.FindIndex(
                delegate(LipSyncMapItem mapItem)
                {
                    return mapItem.Name.Equals(_newMapping.StartNode);
                });
            }

            LipSyncMapItem retVal = null;

            int numGridCols = CalcNumDataGridCols;
            int numGridRows = CalcNumDataGridRows;
            int calcIndex;

            calcIndex = (stringsAreRows) ?
                ((numGridCols * (numGridRows - row - 1)) + column) + startMapIndex :
                ((numGridRows * column) + (numGridRows - row - 1)) + startMapIndex;

            if ((calcIndex >= 0) && (calcIndex < _newMapping.MapItems.Count))
            {
                retVal = _newMapping.MapItems.ElementAt(calcIndex);
            }

            return retVal;
        }

        private DataTable BuildBlankTable()
        {
            DataTable dt = new DataTable(nameTextBox.Text);

            int cols = CalcNumDataGridCols;
            int rows = CalcNumDataGridRows;

            for (int col = 0; col < cols; col++)
            {
                dt.Columns.Add(col.ToString(), typeof(Color));
            }

            for (int row = 0; row < rows; row++)
            {
                DataRow dr = dt.Rows.Add();

                for (int col = 0; col < cols; col++)
                {
                    dr[col] = Color.Black;
                }
            }
            return dt;
        }

        private DataTable BuildDialogFromMap(LipSyncMapData data)
        {
            nameTextBox.Text = data.LibraryReferenceName;

            stringCount = _newMapping.MatrixStringCount;
            pixelCount = _newMapping.MatrixPixelsPerString;
            zoomTrackbar.Value = _newMapping.ZoomLevel;

            DataTable dt = BuildBlankTable();
            int cols = CalcNumDataGridCols;
            int rows = CalcNumDataGridRows;

            LipSyncMapItem mapItem = null;

            DataRow dr;
            for (int row = 0; row < rows; row++)
            {
                dr = dt.Rows[row];
                for (int col = 0; col < cols; col++)
                {
                    mapItem = FindRenderMapItem(row,col);
                    

                    if (mapItem != null)
                    {
                        dr[col] =
                            (mapItem.ElementColors.ContainsKey(phonemeArray[currentPhonemeIndex]) == false) ?
                                Color.Black : mapItem.ElementColors[phonemeArray[currentPhonemeIndex]];
                    }
                    else
                    {
                        dr[col] = Color.Gray;
                    }
                }
            }

            return dt;
        }

        private ElementNode FindElementNode(string elementName)
        {
            ElementNode theNode = VixenSystem.Nodes.ToList().Find(
                delegate(ElementNode node)
                {
                    if (node.IsLeaf)
                    {
                        return node.Element.Name.Equals(elementName);
                    }
                    else
                    {
                        return node.Name.Equals(elementName);
                    }
                    
                }
            );

            return theNode;
        }

        private void BuildMapDataFromDialog()
        {
            BuildMapDataFromDialog(phonemeArray[currentPhonemeIndex]);
        }

        private void BuildMapDataFromDialog(PhonemeType phoneme)
        {
            LipSyncMapItem mapItem;
            int numRows = CalcNumDataGridRows;
            int numCols = CalcNumDataGridCols;

            for (int row = 0; row < numRows; row++)
            {
                DataRow dr = currentDataTable.Rows[row];

                for (int col = 0; col < numCols; col++)
                {
                    mapItem = FindRenderMapItem(row, col);
                    if (mapItem != null)
                    {
                        if (mapItem.ElementColors == null)
                        {
                            mapItem.ElementColors = new Dictionary<PhonemeType, Color>();
                        }
                        mapItem.ElementColors[phoneme] = (Color)dr[col];
                    }
                }
            }

            _newMapping.IsMatrix = true;
            _newMapping.MatrixStringCount = stringCount;
            _newMapping.MatrixPixelsPerString = pixelCount;
            _newMapping.ZoomLevel = zoomTrackbar.Value;
            _newMapping.LibraryReferenceName = nameTextBox.Text;
        }

        private DataTable currentDataTable
        {
            get
            {
                PhonemeType currentPhoneme = phonemeArray[currentPhonemeIndex];

                if (!dataTables.ContainsKey(currentPhoneme))
                {
                    dataTables.Add(currentPhoneme, BuildBlankTable());
                }
                return dataTables[currentPhoneme];
            }

            set
            {
                dataTables[phonemeArray[currentPhonemeIndex]] = value;
                doDataGridResize();
            }
        }

        private void BuildRowNameList()
        {
            _rowNames = new List<string>();
            
            try
            {
                _newMapping.MapItems.ForEach(x => _rowNames.Add(x.Name));
            }
            catch (Exception e) { };

        }

        public LipSyncMapData MapData
        {
            get
            {
                return _newMapping;
            }

            set
            {
                _origMapping = value;
                _newMapping = (LipSyncMapData)_origMapping.Clone();

                BuildRowNameList();
                zoomSteps = _newMapping.ZoomLevel;
                doDataGridResize();
            }
        }

        public string HelperName
        {
            get { return "Phoneme Mapping"; }
        }

        private void NextPhonmeIndex()
        {
            BuildMapDataFromDialog(); 
            currentPhonemeIndex++;
            currentPhonemeIndex %= phonemeArray.Count();
            SetPhonemePicture();
            currentDataTable = BuildDialogFromMap(_newMapping);
            updatedataGridView1();
        }

        private void PrevPhonemeIndex()
        {
            BuildMapDataFromDialog();
            currentPhonemeIndex = 
                (currentPhonemeIndex == 0) ? phonemeArray.Count() : currentPhonemeIndex;
            currentPhonemeIndex--;
            SetPhonemePicture();
            currentDataTable = BuildDialogFromMap(_newMapping);
            updatedataGridView1();
        }

        private string CurrentPhonemeString
        {
            get
            {
                return phonemeArray[currentPhonemeIndex].ToString();
            }
        }

        private void SetPhonemePicture()
        {
            phonemePicture.Image = 
                new Bitmap(_phonemeBitmaps[phonemeArray[currentPhonemeIndex]], 48, 48);
            phonemeLabel.Text = CurrentPhonemeString;
        }

        private void LoadResourceBitmaps()
        {
            if (_phonemeBitmaps == null)
            {
                Assembly assembly = Assembly.Load("LipSyncApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                if (assembly != null)
                {
                    ResourceManager lipSyncRM = new ResourceManager("VixenModules.App.LipSyncApp.LipSyncResources", assembly);
                    _phonemeBitmaps = new Dictionary<PhonemeType, Bitmap>();
                    _phonemeBitmaps.Add(PhonemeType.AI, (Bitmap)lipSyncRM.GetObject("AI"));
                    _phonemeBitmaps.Add(PhonemeType.E, (Bitmap)lipSyncRM.GetObject("E"));
                    _phonemeBitmaps.Add(PhonemeType.ETC, (Bitmap)lipSyncRM.GetObject("etc"));
                    _phonemeBitmaps.Add(PhonemeType.FV, (Bitmap)lipSyncRM.GetObject("FV"));
                    _phonemeBitmaps.Add(PhonemeType.L, (Bitmap)lipSyncRM.GetObject("L"));
                    _phonemeBitmaps.Add(PhonemeType.MBP, (Bitmap)lipSyncRM.GetObject("MBP"));
                    _phonemeBitmaps.Add(PhonemeType.O, (Bitmap)lipSyncRM.GetObject("O"));
                    _phonemeBitmaps.Add(PhonemeType.REST, (Bitmap)lipSyncRM.GetObject("rest"));
                    _phonemeBitmaps.Add(PhonemeType.U, (Bitmap)lipSyncRM.GetObject("U"));
                    _phonemeBitmaps.Add(PhonemeType.WQ, (Bitmap)lipSyncRM.GetObject("WQ"));
                }
            }

            currentPhonemeIndex = 0;
            phonemeArray = _phonemeBitmaps.Keys.ToArray();
            SetPhonemePicture();
        }

        private void LipSyncMapSetup_Load(object sender, EventArgs e)
        {
            if ((_newMapping.MatrixStringCount != 1) && (_newMapping.MapItems.Count == 1))
            {
                assignNodes();
            }
            
            currentDataTable = BuildDialogFromMap(_newMapping);
            updatedataGridView1();
        }

        private void updatedataGridView1()
        {
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridView1.ColumnHeadersHeight = 50;
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            dataGridView1.MultiSelect = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.DataSource = currentDataTable;

            int x = CalcNumDataGridCols;
            int y = CalcNumDataGridRows;

            DataGridViewColumn dgvCol;
            int colIndexVal = 0;
            for (int j = 0; j < dataGridView1.Columns.Count; j++)
            {
                dgvCol = dataGridView1.Columns[j];
                dgvCol.Width = CELL_BASE_WIDTH + (int)(ZOOM_STEP_DELTA * zoomSteps);
                dgvCol.SortMode = DataGridViewColumnSortMode.NotSortable;

                colIndexVal = j;
                dgvCol.HeaderCell.Value = colIndexVal.ToString();
            }

            DataGridViewRow dgvRow;
            int rowIndexVal = 0;

            for (int j = 0; j < dataGridView1.Rows.Count; j++)
            {
                dgvRow = dataGridView1.Rows[j];
                dgvRow.Height = CELL_BASE_WIDTH + (int)(ZOOM_STEP_DELTA * zoomSteps);

                rowIndexVal = dataGridView1.Rows.Count - j - 1;

                dgvRow.HeaderCell.Value = rowIndexVal.ToString();
            }
			doDataGridResize();
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string colStr;

            if ((e.RowIndex == -1) && (e.ColumnIndex >= 0))
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                colStr = e.FormattedValue.ToString();
                e.Graphics.DrawString(colStr, e.CellStyle.Font, Brushes.Black, 5, 5);

                e.Graphics.ResetTransform();
                e.Handled = true;

            }

            Color useColor;
            if (e.RowIndex > -1)
            {
 
                if (e.ColumnIndex >= 0)
                {
                    useColor = ((e.FormattedValue == null) ||
                        (e.FormattedValue.Equals(""))) ? Color.Black : (Color)e.Value;

                    using (SolidBrush paintBrush = new SolidBrush(useColor))
                    {
                        e.Graphics.FillRectangle(paintBrush,
                                                    e.CellBounds.X + 2,
                                                    e.CellBounds.Y + 2,
                                                    e.CellBounds.Width - 3,
                                                    e.CellBounds.Height - 3);

                        e.Graphics.DrawRectangle(new Pen(Color.Gray, 2), e.CellBounds);

                        e.CellStyle.ForeColor = useColor;
                        e.CellStyle.SelectionForeColor = useColor;
                        e.CellStyle.SelectionBackColor = useColor;
                        e.CellStyle.BackColor = useColor;
                    }

                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            BuildMapDataFromDialog();
        }


        private void LipSyncBreakdownSetup_Resize(object sender, EventArgs e)
        {
            doDataGridResize();
        }

        private void reconfigureDataTable()
        {
            if (_doMatrixUpdate == true)
            {
                currentDataTable.Rows.Clear();
                currentDataTable = BuildDialogFromMap(_newMapping);
                updatedataGridView1();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Color newColor =
                (e.Button == MouseButtons.Left) ? lipSyncMapColorCtrl1.Color : Color.Black;
            
            LipSyncMapItem item = FindRenderMapItem(e.RowIndex, e.ColumnIndex);
			item.PhonemeList[CurrentPhonemeString] = (newColor != Color.Black);
			dataGridView1.CurrentCell.Value = (item == null) ? Color.Gray : newColor;
        }

        private void colsUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (pixelCount != _newMapping.MatrixPixelsPerString)
            {
                _newMapping.MatrixPixelsPerString = pixelCount;
                currentDataTable = BuildDialogFromMap(_newMapping);
                updatedataGridView1(); 
                BuildMapDataFromDialog();
            }
        }

        private void rowsUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (stringCount != _newMapping.MatrixStringCount)
            {
                _newMapping.MatrixStringCount = stringCount;
                currentDataTable = BuildDialogFromMap(_newMapping);
                BuildMapDataFromDialog();
                updatedataGridView1();
            }
        }

        private void doDataGridResize()
        {
            int height = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                height += row.Height;
            }
            height += dataGridView1.ColumnHeadersHeight;

            int width = 0;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                width += col.Width;
            }
            width += dataGridView1.RowHeadersWidth;

            width = Math.Min(width + 2, this.Width - 200);
            height = Math.Min(height + 2, this.Height - 200);

            dataGridView1.ClientSize = new Size(width + 2, height + 2);

            dataGridView1.Location = new Point(25, 150);
			dataGridView1.Invalidate();
			Refresh();
        }

        private void zoomTrackbar_ValueChanged(object sender, EventArgs e)
        {
            zoomSteps = zoomTrackbar.Value;
            if (zoomSteps == 0)
            {
                zoomSteps = 1;
            }
            updatedataGridView1();
        }


        private void buttonAssign_Click(object sender, EventArgs e)
        {
            BuildMapDataFromDialog(); 
            assignNodes();
            updatedataGridView1();
			Refresh();
        }

        private void assignNodes()
        {
            BuildRowNameList();

            LipSyncNodeSelect nodeSelectDlg = new LipSyncNodeSelect();
            nodeSelectDlg.MaxNodes = _newMapping.MatrixStringCount * _newMapping.MatrixPixelsPerString;
            nodeSelectDlg.MatrixOptionsOnly = true;
            nodeSelectDlg.StringsAreRows = _newMapping.StringsAreRows;
            nodeSelectDlg.NodeNames = _rowNames;

            DialogResult dr = nodeSelectDlg.ShowDialog();
            if ((dr == DialogResult.OK) && (nodeSelectDlg.Changed == true))
            {
                List<LipSyncMapItem> newMappings = new List<LipSyncMapItem>();
                LipSyncMapItem tempMapItem = null;

                _newMapping.StringsAreRows = nodeSelectDlg.StringsAreRows;
                stringsAreRows = _newMapping.StringsAreRows;

                _rowNames.Clear();
                _rowNames.AddRange(nodeSelectDlg.NodeNames);

                foreach (string nodeName in nodeSelectDlg.NodeNames)
                {
                    tempMapItem = _newMapping.MapItems.Find(
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
                        newMappings.Add(new LipSyncMapItem(nodeName, -1));
                    }
                }

                _newMapping.MapItems.Clear();

                int stringCount = 0;
                foreach (LipSyncMapItem mapItem in newMappings)
                {
                    mapItem.StringNum = stringCount++;
                    _newMapping.MapItems.Add(mapItem);
                }

                _newMapping.StartNode =
                    (_newMapping.MapItems.Count != 0) ? _newMapping.MapItems[0].Name : "";

                reconfigureDataTable();
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "Image Files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All Files(*.*)|*.*";
            DialogResult result = fileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                LipSyncMapItem mapItem;
                Color pixelColor;

                Bitmap rawBitmap = new Bitmap(fileDlg.FileName);
                FastPixel.FastPixel scaledImage = 
                    new FastPixel.FastPixel(new Bitmap(rawBitmap, CalcNumDataGridCols, CalcNumDataGridRows));
                
                int cols = CalcNumDataGridCols;
                int rows = CalcNumDataGridRows;

                scaledImage.Lock();
                for (int row = 0; row < rows; row++)
                {
                    DataRow dr = currentDataTable.Rows[row];
                    for (int col = 0; col < cols; col++)
                    {
                        mapItem = FindRenderMapItem(row, col);
                        if (mapItem != null)
                        {
                            pixelColor = scaledImage.GetPixel(col, row);
                            dr[col] = pixelColor;
                            mapItem.PhonemeList[CurrentPhonemeString] = (pixelColor != Color.Black);
                        }
                        else
                        {
                            dr[col] = Color.Gray;
                        }
                    }
                }
                scaledImage.Unlock(false);

                BuildDialogFromMap(_newMapping);
                updatedataGridView1();
				Refresh();
            }
        }

        private void nextPhonemeButton_Click(object sender, EventArgs e)
        {
            NextPhonmeIndex();
        }

        private void prevPhonemeButton_Click(object sender, EventArgs e)
        {
            PrevPhonemeIndex();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            BuildMapDataFromDialog();

            SaveFileDialog fileDlg = new SaveFileDialog();
            fileDlg.Filter = "Bitmap Files (*.bmp )|*.bmp |All Files(*.*)|*.*";
            DialogResult result = fileDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                LipSyncMapItem mapItem;

                int cols = CalcNumDataGridCols;
                int rows = CalcNumDataGridRows;

                Bitmap rawBitmap = new Bitmap(cols, rows);

                for (int row = 0; row < rows; row++)
                {
                    DataRow dr = currentDataTable.Rows[row];
                    for (int col = 0; col < cols; col++)
                    {
                        mapItem = FindRenderMapItem(row, col);
                        if (mapItem != null)
                        {
                            rawBitmap.SetPixel(col, row, (Color)dr[col]);
                        }
                        else
                        {
                            rawBitmap.SetPixel(col, row, Color.Black);
                        }
                    }
                }
                rawBitmap.Save(fileDlg.FileName);
            }
			Refresh();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            currentDataTable = BuildBlankTable();
            updatedataGridView1();
            BuildMapDataFromDialog();
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
