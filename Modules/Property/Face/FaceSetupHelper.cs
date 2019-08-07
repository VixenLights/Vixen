using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Extensions;
using Vixen.Rule;
using Vixen.Sys;
using ColorProperty = VixenModules.Property.Color;
using Color = System.Drawing.Color;


namespace VixenModules.Property.Face {
	public partial class FaceSetupHelper : BaseForm, IElementSetupHelper
	{
		private DataTable _mouthDataTable;
		private DataTable _otherDataTable;
		private static Dictionary<string, Bitmap> _phonemeBitmaps;
		private static Dictionary<string, Bitmap> _faceComponentBitmaps;
		private static string COLOR_COLUMN_NAME = "Color";
		private IEnumerable<IElementNode> _targetNodes;

		public FaceSetupHelper() {
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			Icon = Resources.Icon_Vixen3;
		}

		private void FaceSetupHelper_Load(object sender, EventArgs e)
		{
			SetGridDefaults(dataGridViewMouth);
			SetGridDefaults(dataGridViewOther);
			ThemeUpdateControls.UpdateControls(this);
			tabControl.SelectedIndex = 0;
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

		#region Implementation of IElementSetupHelper

		/// <inheritdoc />
		public string HelperName => "Face Mapping";

		/// <inheritdoc />
		public bool Perform(IEnumerable<IElementNode> selectedNodes)
		{
			LoadResourceBitmaps();
			_targetNodes = selectedNodes;
			_mouthDataTable =  BuildMouthDialogFromMap();
			_otherDataTable = BuildOtherDialogFromMap();
			UpdateMouthDataGridView();
			UpdateOtherDataGridView();

			DialogResult dr = ShowDialog();
			if (dr != DialogResult.OK)
			{
				return false;
			}

			BuildPropertiesFromDialog();

			return true;
		}

		#endregion

		private void BuildPropertiesFromDialog()
		{
			for (int num = 0; num < _mouthDataTable.Rows.Count; num++)
			{
				DataRow dr = _mouthDataTable.Rows[num];
				var otherDataRow = _otherDataTable.Rows[num];
				string elementName = dr[0].ToString();
				FaceMapItem item = new FaceMapItem();
				IElementNode node = FindElementNode(elementName);

				FaceModule fm;
				if (node.Properties.Contains(FaceDescriptor.ModuleId))
				{
					fm = node.Properties.Get(FaceDescriptor.ModuleId) as FaceModule;
					
				}
				else
				{
					fm = node.Properties.Add(FaceDescriptor.ModuleId) as FaceModule;
				}

				if(fm == null) continue;
				
				fm.PhonemeList.Clear();
			    for (int theCount = 1; theCount < dr.ItemArray.Count() - 1; theCount++)
				{
					bool checkVal = dr[theCount] is bool && (Boolean)dr[theCount];
					fm.PhonemeList.Add(
						dr.Table.Columns[theCount].ColumnName, checkVal
					);
				}
				fm.DefaultColor = ValidateColorForElement(node, (System.Drawing.Color)dr[dr.ItemArray.Count() - 1]);

				fm.FaceComponents.Clear();
				foreach (FaceComponent key in Enum.GetValues(typeof(FaceComponent)))
				{
					if (key == FaceComponent.Mouth)
					{
						fm.FaceComponents.Add(key, item.PhonemeList.Values.Any(x => x));
						continue;
					}

					var value = otherDataRow[key.GetEnumDescription()];
					if (value is bool)
					{
						fm.FaceComponents.Add(key, (bool)value);
					}

				}
			}
		}

		private DataTable BuildMouthDialogFromMap()
		{
			DataTable dt = new DataTable("Mouth Table");
			dt.Columns.Add(" ", typeof(string));

			foreach (string key in _phonemeBitmaps.Keys)
			{
				dt.Columns.Add(key, typeof(Boolean));
			}

			dt.Columns.Add(COLOR_COLUMN_NAME, typeof(System.Drawing.Color));

			foreach (var element in _targetNodes)
			{
				var fm = FaceModule.GetFaceModuleForElement(element);
				
				DataRow dr = dt.Rows.Add();
				dr[0] = element.Name;
				
				foreach (string key in _phonemeBitmaps.Keys)
				{
					bool result;
					if (fm != null && fm.PhonemeList.TryGetValue(key, out result))
					{
						dr[key] = result;
					}
					else
					{
						dr[key] = false;
					}
				}
				dr[COLOR_COLUMN_NAME] = ValidateColorForElement(element, fm?.DefaultColor??System.Drawing.Color.White);
			}

			dt.Columns[" "].ReadOnly = true;
			dt.Columns[COLOR_COLUMN_NAME].ReadOnly = true;

			return dt;
		}

		private DataTable BuildOtherDialogFromMap()
		{
			DataTable dt = new DataTable("Face Components Table");
			dt.Columns.Add(" ", typeof(string));

			dt.Columns.Add(FaceComponent.Outlines.GetEnumDescription(), typeof(Boolean));
			dt.Columns.Add(FaceComponent.EyesOpen.GetEnumDescription(), typeof(Boolean));
			dt.Columns.Add(FaceComponent.EyesClosed.GetEnumDescription(), typeof(Boolean));

			dt.Columns.Add(COLOR_COLUMN_NAME, typeof(System.Drawing.Color));

			foreach (var element in _targetNodes)
			{
				DataRow dr = dt.Rows.Add();
				dr[0] = element.Name;
				var fm = FaceModule.GetFaceModuleForElement(element);
				foreach (FaceComponent key in Enum.GetValues(typeof(FaceComponent)))
				{
					if (key == FaceComponent.Mouth) continue;
					bool result;
					if (fm != null && fm.FaceComponents.TryGetValue(key, out result))
					{
						dr[key.GetEnumDescription()] = result;
					}
					else
					{
						dr[key.GetEnumDescription()] = false;
					}
				}
				dr[COLOR_COLUMN_NAME] = ValidateColorForElement(element, fm?.DefaultColor ?? System.Drawing.Color.White);
			}

			dt.Columns[" "].ReadOnly = true;
			dt.Columns[COLOR_COLUMN_NAME].ReadOnly = true;

			return dt;
		}

		private void LoadResourceBitmaps()
		{
			if (_phonemeBitmaps == null)
			{

				_phonemeBitmaps = new Dictionary<string, Bitmap>();
				_phonemeBitmaps.Add("AI", Tools.GetIcon(Resources.AI_Transparent, 48));
				_phonemeBitmaps.Add("E", Tools.GetIcon(Resources.E_Transparent, 48));
				_phonemeBitmaps.Add("ETC", Tools.GetIcon(Resources.etc_Transparent, 48));
				_phonemeBitmaps.Add("FV", Tools.GetIcon(Resources.FV_Transparent, 48));
				_phonemeBitmaps.Add("L", Tools.GetIcon(Resources.L_Transparent, 48));
				_phonemeBitmaps.Add("MBP", Tools.GetIcon(Resources.MBP_Transparent, 48));
				_phonemeBitmaps.Add("O", Tools.GetIcon(Resources.O_Transparent, 48));
				_phonemeBitmaps.Add("REST", Tools.GetIcon(Resources.rest_Transparent, 48));
				_phonemeBitmaps.Add("U", Tools.GetIcon(Resources.U_Transparent, 48));
				_phonemeBitmaps.Add("WQ", Tools.GetIcon(Resources.WQ_Transparent, 48));

				//Temp placeholder for outline and eye images.
				Bitmap blank = new Bitmap(48, 48);
				using (Graphics gr = Graphics.FromImage(blank))
				{
					gr.Clear(System.Drawing.Color.Transparent);
				}
				_faceComponentBitmaps = new Dictionary<string, Bitmap>
				{
					{
						FaceComponent.EyesOpen.GetEnumDescription(), blank //Tools.GetIcon(Resources.NoImage, 48)
					},
					{
						FaceComponent.EyesClosed.GetEnumDescription(), blank  //Tools.GetIcon(Resources.NoImage, 48)
					},
					{
						FaceComponent.Outlines.GetEnumDescription(), blank  //Tools.GetIcon(Resources.NoImage, 48)
					}
				};
			}
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

		private IElementNode FindElementNode(string elementName)
		{
			return _targetNodes.First(x => x.Name.Equals(elementName));
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
			else if (e.ColumnIndex == colorColumn && e.Value is System.Drawing.Color)
			{
				e.Graphics.DrawRectangle(new Pen(ThemeColorTable.ForeColor, 2), e.CellBounds);
				e.CellStyle.ForeColor = (System.Drawing.Color)e.Value;
				e.CellStyle.BackColor = (System.Drawing.Color)e.Value;
				e.CellStyle.SelectionForeColor = (System.Drawing.Color)e.Value;
				e.CellStyle.SelectionBackColor = (System.Drawing.Color)e.Value;
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
					List<IElementNode> chosenNodes = new List<IElementNode>();
					//LipSyncMapColorSelect colorDialog1 = new LipSyncMapColorSelect();

					foreach (DataGridViewCell selCell in view.SelectedCells)
					{
						if (selCell.ColumnIndex == lastColumn)
						{
							IElementNode theNode = FindElementNode((string)selCell.OwningRow.Cells[0].Value);
							if (theNode != null)
							{
								chosenNodes.Add(theNode);
							}
						}
					}

					//colorDialog1.ChosenNodes = chosenNodes;
					var origColor = (System.Drawing.Color)view.SelectedCells[0].Value;

					// Show the color dialog.
					//DialogResult result = colorDialog1.ShowDialog();
					var result = ChooseColor(chosenNodes, origColor);

					// See if user pressed ok.
					if (result.Item1 == DialogResult.OK)
					{
						_mouthDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = false;
						_otherDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = false;
						foreach (DataGridViewCell selCell in view.SelectedCells)
						{
							if (selCell.ColumnIndex == lastColumn)
							{
								_mouthDataTable.Rows[selCell.RowIndex][_mouthDataTable.Columns[COLOR_COLUMN_NAME]] = result.Item2;
								_otherDataTable.Rows[selCell.RowIndex][_otherDataTable.Columns[COLOR_COLUMN_NAME]] = result.Item2;
								//selCell.Value = colorDialog1.Color;
							}
						}
						DataGridViewCell cell = view.Rows[e.RowIndex].Cells[e.ColumnIndex];
						cell.Value = result.Item2;
						_mouthDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = true;
						_otherDataTable.Columns[COLOR_COLUMN_NAME].ReadOnly = true;
					}
				}
			}
		}

		private System.Drawing.Color ValidateColorForElement(IElementNode e, System.Drawing.Color defaultColor)
		{
			var colors = ColorProperty.ColorModule.getValidColorsForElementNode(e, false);
			if (colors.Any())
			{
				if (!colors.Contains(defaultColor))
				{
					return colors.First();
				}
			}

			return defaultColor;
		}

		private HashSet<System.Drawing.Color> ValidDiscreteColors(List<IElementNode> nodeList)
		{
			HashSet<System.Drawing.Color> validColors = new HashSet<System.Drawing.Color>();

			if (nodeList == null) return validColors;
			// look for the color property of the target effect element, and restrict the gradient.
			// If it's a group, iterate through all children (and their children, etc.), finding as many color
			// properties as possible; then we can decide what to do based on that.
			validColors.AddRange(nodeList.SelectMany(x => ColorProperty.ColorModule.getValidColorsForElementNode(x, true)));
			return validColors;
		}

		private Tuple<DialogResult,System.Drawing.Color> ChooseColor(List<IElementNode> selectedNodes, System.Drawing.Color color)
		{
			var returnColor = color;
			var colors = ValidDiscreteColors(selectedNodes);
			DialogResult result = DialogResult.Abort;
			if (colors.Any())
			{
				using (DiscreteColorPicker dcp = new DiscreteColorPicker())
				{
					dcp.ValidColors = colors;
					dcp.SingleColorOnly = true;
					dcp.SelectedColors = new List<System.Drawing.Color> { color };
					result = dcp.ShowDialog();
					if (result == DialogResult.OK)
					{
						if (!dcp.SelectedColors.Any())
						{
							returnColor = System.Drawing.Color.White;
						}
						else
						{
							returnColor = dcp.SelectedColors.First();
						}
					}
				}
			}
			else
			{
				using (ColorPicker cp = new ColorPicker())
				{
					cp.LockValue_V = false;
					cp.Color = XYZ.FromRGB(color);
					result = cp.ShowDialog();
					if (result == DialogResult.OK)
					{
						returnColor = cp.Color.ToRGB();
					}
				}
			}

			return new Tuple<DialogResult, System.Drawing.Color>(result, returnColor);
		}
	}
}
