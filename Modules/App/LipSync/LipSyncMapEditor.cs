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
	public partial class LipSyncMapEditor : BaseForm
    {
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        private LipSyncMapData _mapping;
        private DataTable currentDataTable;
        private static Dictionary<string, Bitmap> _phonemeBitmaps = null;
        private List<string> _rowNames = null;
        private static string COLOR_COLUMN_NAME = "Color";
        private bool _doMatrixUpdate = false;

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
			dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
			Icon = Resources.Icon_Vixen3;
            _doMatrixUpdate = false;
            LoadResourceBitmaps();
            this.MapData = mapData;
            _doMatrixUpdate = true;
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

            DataTable dt = new DataTable(nameTextBox.Text);
            dt.Columns.Add(" ", typeof(string));

            foreach (string key in _phonemeBitmaps.Keys)
            {
                dt.Columns.Add(key, typeof(System.Boolean));
            }

            dt.Columns.Add(COLOR_COLUMN_NAME, typeof(Color));
            
            bool result = false;
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
                    if (lsbItem.PhonemeList.TryGetValue(key, out result) == true)
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

        private void BuilMapDataFromDialog()
        {
            int currentRow = 0;

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
                    bool checkVal =
                        (dr[theCount].GetType() == typeof(Boolean)) ? (Boolean)dr[theCount] : false;
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
                BuilMapDataFromDialog();
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
                updatedataGridView1();
            }
        }

        public string HelperName
        {
            get { return "Phoneme Mapping"; }
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
					_phonemeBitmaps.Add("AI", (Bitmap)lipSyncRM.GetObject("AI_LightGray"));
					_phonemeBitmaps.Add("E", (Bitmap)lipSyncRM.GetObject("E_LightGray"));
					_phonemeBitmaps.Add("ETC", (Bitmap)lipSyncRM.GetObject("etc_LightGray"));
					_phonemeBitmaps.Add("FV", (Bitmap)lipSyncRM.GetObject("FV_LightGray"));
					_phonemeBitmaps.Add("L", (Bitmap)lipSyncRM.GetObject("L_LightGray"));
					_phonemeBitmaps.Add("MBP", (Bitmap)lipSyncRM.GetObject("MBP_LightGray"));
					_phonemeBitmaps.Add("O", (Bitmap)lipSyncRM.GetObject("O_LightGray"));
					_phonemeBitmaps.Add("REST", (Bitmap)lipSyncRM.GetObject("rest_LightGray"));
					_phonemeBitmaps.Add("U", (Bitmap)lipSyncRM.GetObject("U_LightGray"));
					_phonemeBitmaps.Add("WQ", (Bitmap)lipSyncRM.GetObject("WQ_LightGray"));
                }
            }
        }

        private void LipSyncMapSetup_Load(object sender, EventArgs e)
        {
            updatedataGridView1();
        }

        private void updatedataGridView1()
        {
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridView1.ColumnHeadersHeight = 100;
            dataGridView1.MultiSelect = true;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            dataGridView1.DataSource = currentDataTable;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            for (int j = 1; j < dataGridView1.Columns.Count - 1; j++)
            {
                dataGridView1.Columns[j].Width = 60;
            }
			dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width = 90;
            dataGridView1.Columns[COLOR_COLUMN_NAME].SortMode = DataGridViewColumnSortMode.NotSortable;

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string phonemeStr;
            Bitmap phonemeBitmap;

            if ((e.RowIndex == -1) && (e.ColumnIndex >= 0))
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);
                phonemeStr = e.FormattedValue.ToString();
                if (_phonemeBitmaps.TryGetValue(phonemeStr, out phonemeBitmap))
                {
                    e.Graphics.DrawImage(new Bitmap(_phonemeBitmaps[phonemeStr], 48, 48), 5, 5);
                    e.Graphics.DrawString(phonemeStr, e.CellStyle.Font, Brushes.Black, 55, 5);
                }
                else
                {
                    e.Graphics.DrawString(phonemeStr, e.CellStyle.Font, Brushes.Black, 5, 5);
                }

                e.Graphics.ResetTransform();
                e.Handled = true;

            }

            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex >= currentDataTable.Columns.Count - 1)
                {
                    using (SolidBrush paintBrush = new SolidBrush((Color)e.Value))
                    {
                        e.Graphics.FillRectangle(paintBrush,
                                                    e.CellBounds.X + 2,
                                                    e.CellBounds.Y + 2,
                                                    e.CellBounds.Width - 3,
                                                    e.CellBounds.Height - 3);

                        e.Graphics.DrawRectangle(new Pen(Color.Gray, 2), e.CellBounds);

                        e.CellStyle.ForeColor = (Color)e.Value;
                        e.CellStyle.SelectionForeColor = (Color)e.Value;
                        e.CellStyle.SelectionBackColor = Color.Black;
                        e.CellStyle.BackColor = (Color)e.Value;
                    }
                }
                else
                {
                    e.CellStyle.BackColor =
                        ((e.RowIndex % 2) == 0) ? Color.White : Color.LightGreen;
                }
            }

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }


        private void LipSyncBreakdownSetup_Resize(object sender, EventArgs e)
        {
            dataGridView1.Size = new Size(this.Size.Width - 40, this.Size.Height - 150);
        }

        private void reconfigureDataTable()
        {
            if (_doMatrixUpdate == true)
            {
                currentDataTable.Rows.Clear();
                currentDataTable = BuildDialogFromMap(_mapping);
                updatedataGridView1();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex > -1) && (e.ColumnIndex > 0))
            {
                BuilMapDataFromDialog();
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

                reconfigureDataTable();
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
