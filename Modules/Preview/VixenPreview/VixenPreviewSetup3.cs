using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using System.IO;
using VixenModules.Editor.VixenPreviewSetup3.Undo;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Location;
using System.Windows.Forms.Design;
using Common.Resources.Properties;
using Element = Vixen.Sys.Element;

namespace VixenModules.Preview.VixenPreview {
    public partial class VixenPreviewSetup3 : Form
    {
        private VixenPreviewData _data;
		private static VixenPreviewSetupDocument previewForm;
		private VixenPreviewSetupElementsDocument elementsForm;
		private VixenPreviewSetupPropertiesDocument propertiesForm;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public static string DrawShape;
		// Undo manager
		public static UndoManager _undoMgr;

		public VixenPreviewData Data {
			set {
				_data = value;
				if (!DesignMode && previewForm != null)
					previewForm.Preview.Data = _data;
			}
			get
			{
				if (_data == null) {
					Logging.Warn("VixenPreviewSetup3: access of null Data. (Thread ID: " +
					                            System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				}
				return _data;
			}
		}

		public VixenPreviewSetup3() {
			InitializeComponent();
			undoButton.Image = Tools.GetIcon(Resources.arrow_undo, 24);
			undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.Image = Tools.GetIcon(Resources.arrow_redo, 24);
			redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.ButtonType = UndoButtonType.RedoButton;

			VixenPreviewControl.ElementsPreviewMovedNew += vixenpreviewControl_ElementsMovedNew;
			Icon = Resources.Icon_Vixen3;
			this.ShowInTaskbar = false;

		}

		public void vixenpreviewControl_ElementsMovedNew(object sender, ElementsChangedPreviewEventArgs e)
		{
			 var action = new ElementsMoveUndoAction(this, e.PreviousTimes, e.Type);
			_undoMgr.AddUndoAction(action);
		}

		private void VixenPreviewSetup3_Load(object sender, EventArgs e) {
			previewForm = new VixenPreviewSetupDocument();
			if (!DesignMode && previewForm != null)
				previewForm.Preview.Data = _data;
			previewForm.Preview.OnSelectDisplayItem += OnSelectDisplayItem;
			previewForm.Preview.OnDeSelectDisplayItem += OnDeSelectDisplayItem;

            previewForm.Preview.OnChangeZoomLevel += VixenPreviewSetup3_ChangeZoomLevel;

			elementsForm = new VixenPreviewSetupElementsDocument(previewForm.Preview);
			propertiesForm = new VixenPreviewSetupPropertiesDocument();

			previewForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
			elementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
			propertiesForm.Show(elementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

			previewForm.Preview.elementsForm = elementsForm;
			previewForm.Preview.propertiesForm = propertiesForm;

			previewForm.Preview.LoadBackground();	
			
			trackBarBackgroundAlpha.Value = Data.BackgroundAlpha;
			previewForm.Preview.Reload();

			PopulateTemplateList();

			Setup();

			// disable the D2D preview option for now; the GDI performs just as well, and is more reliable (eg. older machines, not HW accelerated machines, etc.)
			//performanceToolStripMenuItem.Visible = Vixen.Sys.VixenSystem.VersionBeyondWindowsXP;
			performanceToolStripMenuItem.Visible = false;

			Properties.Settings settings = new Properties.Settings();

			useDirect2DPreviewRenderingToolStripMenuItem.Checked = !settings.UseGDIRendering;
			saveLocationsToolStripMenuItem.Checked = Data.SaveLocations;

			// Choose the select tool to start
			toolbarButton_Click(buttonSelect, new EventArgs());

            if (IntPtr.Size != 8)
            {
                trackerZoom.Maximum = 200;
            }
			InitUndo();
		}

		private void buttonSetBackground_Click(object sender, EventArgs e) {
			if (dialogSelectBackground.ShowDialog() == DialogResult.OK) {
				// Copy the file to the Vixen folder
				var imageFile = new FileInfo(dialogSelectBackground.FileName);
                string imageFileName = Guid.NewGuid() + Path.GetExtension(dialogSelectBackground.FileName);
                var destFileName = Path.Combine(VixenPreviewDescriptor.ModulePath, imageFileName);
				var sourceFileName = imageFile.FullName;
				if (sourceFileName != destFileName) {
					File.Copy(sourceFileName, destFileName, true);
				}

				// Set the backgrounds
				Data.BackgroundFileName = imageFileName;
				previewForm.Preview.LoadBackground();
				trackBarBackgroundAlpha.Value = trackBarBackgroundAlpha.Maximum;
				previewForm.Preview.BackgroundAlpha = trackBarBackgroundAlpha.Value;
			}
		}

		private void OnDeSelectDisplayItem(object sender, Shapes.DisplayItem displayItem) {
			propertiesForm.ShowSetupControl(null);
			reenableToolButtons();
		}

		private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem) {
			Shapes.DisplayItemBaseControl setupControl = displayItem.Shape.GetSetupControl();

			if (setupControl != null) {
				propertiesForm.ShowSetupControl(setupControl);
			}
			reenableToolButtons();
		}

        private void VixenPreviewSetup3_ChangeZoomLevel(object sender, double zoomLevel) 
        {
            SetZoomTextAndTracker(zoomLevel);
        }

        private void EnableButton(Control.ControlCollection parent, VixenPreviewControl.Tools tool)
        {
            if (parent != null)
            {
                foreach (Control c in parent)
                {
                    if (c is Button && c.Tag != null && c.Tag.ToString() != "")
                    {
                        Button button = c as Button;
                        if (c.Tag.ToString() == previewForm.Preview.CurrentTool.ToString())
                        {
                            button.BackColor = Color.Gainsboro;
                            button.FlatAppearance.BorderColor = button.BackColor;
                        }
                        else
                        {
                            button.BackColor = Color.White;
                            button.FlatAppearance.BorderColor = button.BackColor;
                        }
                    }
                    EnableButton(c.Controls, tool);
                }
            }
        }

		private void reenableToolButtons()
		{
            EnableButton(this.Controls, previewForm.Preview.CurrentTool);

            //buttonSelect.BackColor = Color.White;
            //buttonSelect.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonCane.BackColor = buttonSelect.BackColor;
            //buttonCane.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonDrawPixel.BackColor = buttonSelect.BackColor;
            //buttonDrawPixel.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonLine.BackColor = buttonSelect.BackColor;
            //buttonLine.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonSemiCircle.BackColor = buttonSelect.BackColor;
            //buttonSemiCircle.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonRectangle.BackColor = buttonSelect.BackColor;
            //buttonRectangle.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonEllipse.BackColor = buttonSelect.BackColor;
            //buttonEllipse.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonTriangle.BackColor = buttonSelect.BackColor;
            //buttonTriangle.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonNet.BackColor = buttonSelect.BackColor;
            //buttonNet.FlatAppearance.BorderColor = buttonSelect.BackColor;
            ////buttonFlood.BackColor = buttonSelect.BackColor;
            ////buttonFlood.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonStar.BackColor = buttonSelect.BackColor;
            //buttonStar.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonStarBurst.BackColor = buttonSelect.BackColor;
            //buttonStarBurst.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonMegaTree.BackColor = buttonSelect.BackColor;
            //buttonMegaTree.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonPixelGrid.BackColor = buttonSelect.BackColor;
            //buttonPixelGrid.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonIcicle.BackColor = buttonSelect.BackColor;
            //buttonIcicle.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonPolyLine.BackColor = buttonSelect.BackColor;
            //buttonPolyLine.FlatAppearance.BorderColor = buttonSelect.BackColor;
            //buttonMultiString.BackColor = buttonSelect.BackColor;
            //buttonMultiString.FlatAppearance.BorderColor = buttonSelect.BackColor;
        }

		private void toolbarButton_Click(object sender, EventArgs e) {
			Button button = sender as Button;
            //reenableToolButtons();

            // There must be a way to iterate through an enum so we don't have to do all this crap...

			// Select Button
	        DrawShape = "";
            if (button == buttonSelect)
                previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Select;
            else if (button == buttonDrawPixel)
            {
	            DrawShape = "Pixel";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Single;
            }
            else if (button == buttonLine)
            {
	            DrawShape = "Line";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.String;
            }
            else if (button == buttonSemiCircle)
            {
	            DrawShape = "Arch";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Arch;
            }
            else if (button == buttonRectangle)
            {
	            DrawShape = "Rectangle";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Rectangle;
            }
            else if (button == buttonEllipse)
            {
	            DrawShape = "Ellipse";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Ellipse;
            }
            else if (button == buttonTriangle)
            {
	            DrawShape = "Triangle";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Triangle;
            }
            else if (button == buttonNet)
            {
	            DrawShape = "Net";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Net;
            }
	            //else if (button == buttonFlood)
	            //    previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Flood;
            else if (button == buttonCane)
            {
	            DrawShape = "Candy Cane";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Cane;
            }
            else if (button == buttonStar)
            {
	            DrawShape = "Star";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Star;
            }
            else if (button == buttonStarBurst)
            {
	            DrawShape = "Star Burst";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.StarBurst;
            }
            else if (button == buttonHelp)
	            Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Main);
            else if (button == buttonMegaTree)
            {
	            DrawShape = "Mega Tree";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.MegaTree;
            }
            else if (button == buttonPixelGrid)
            {
	            DrawShape = "Grid";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.PixelGrid;
            }
            else if (button == buttonIcicle)
            {
	            DrawShape = "Icicle";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Icicle;
            }
            else if (button == buttonPolyLine)
            {
	            DrawShape = "PolyLine";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.PolyLine;
            }
            else if (button == buttonMultiString)
            {
	            DrawShape = "Multi String";
	            previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.MultiString;
            }
			//button.Enabled = false;
			//button.BackColor = Color.Gainsboro;
			//button.FlatAppearance.BorderColor = Color.Gainsboro;
			//buttonSelect.Focus();
            reenableToolButtons();
        }

        private void toolbarAlignButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == buttonAlignBottom)
            {
                previewForm.Preview.AlignBottom();
            }
            else if (button == buttonAlignHorizMid)
            {
                previewForm.Preview.AlignHorizontal();
            }
            else if (button == buttonAlignLeft)
            {
                previewForm.Preview.AlignLeft();
            }
            else if (button == buttonAlignRight)
            {
                previewForm.Preview.AlignRight();
            }
            else if (button == buttonAlignTop)
            {
                previewForm.Preview.AlignTop();
            }
            else if (button == buttonAlignVertMid)
            {
                previewForm.Preview.AlignVertical();
            }
            else if (button == buttonDistributeHorizontal)
            {
                previewForm.Preview.DistributeHorizontal();
            }
            else if (button == buttonDistributeVertical)
            {
                previewForm.Preview.DistributeVertical();
            }
            else if (button == buttonMatchProperties)
            {
                previewForm.Preview.MatchProperties();
            }
        }

        private void trackBarBackgroundAlpha_ValueChanged(object sender, EventArgs e) {
			previewForm.Preview.BackgroundAlpha = trackBarBackgroundAlpha.Value;
		}

		public void Setup() {
			SetDesktopLocation(Data.SetupLeft, Data.SetupTop);
			Size = new Size(Data.SetupWidth, Data.SetupHeight);
		}

		private void buttonSave_Click(object sender, EventArgs e) {
			SaveLocationDataForElements();
			DialogResult = System.Windows.Forms.DialogResult.OK;
			previewForm.Close();
			Close();
		}

		private void VixenPreviewSetup3_Move(object sender, EventArgs e) {
			if (Data == null) {
				Logging.Warn("VixenPreviewSetup3_Move: Data is null. abandoning move. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.SetupTop = Top;
			Data.SetupLeft = Left;
		}

		private void VixenPreviewSetup3_Resize(object sender, EventArgs e) {
			if (Data == null) {
				Logging.Warn("VixenPreviewSetup3_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.SetupWidth = Width;
			Data.SetupHeight = Height;
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
			previewForm.Preview.Cut();
		}

		private void copyToolStripMenuItem1_Click(object sender, EventArgs e) {
			previewForm.Preview.Copy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
			previewForm.Preview.Paste();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
			previewForm.Preview.Delete();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}

		private void backgroundPropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
			ResizePreviewForm resizeForm = new ResizePreviewForm(previewForm.Preview.Background.Width,
																 previewForm.Preview.Background.Height);
			if (resizeForm.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (resizeForm.Height > 10 && resizeForm.Width > 10)
                {
                    previewForm.Preview.ResizeBackground(resizeForm.Width, resizeForm.Height);
                    previewForm.Refresh();
                }
                else
                {
                    MessageBox.Show("An invalid image size was specified!", "Invalid Size", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
            }
		}

		#region Templates

		private void PopulateTemplateList() {
			TemplateComboBoxItem selectedTemplateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			comboBoxTemplates.Items.Clear();

			IEnumerable<string> files = System.IO.Directory.EnumerateFiles(PreviewTools.TemplateFolder, "*.xml");
			foreach (string file in files) {
				string fileName = PreviewTools.TemplateWithFolder(file);
				try {
					// Read the entire template file (stoopid waste of resources, but how else?)
					string xml = System.IO.File.ReadAllText(fileName);
					DisplayItem newDisplayItem = (DisplayItem)PreviewTools.DeSerializeToDisplayItem(xml, typeof(DisplayItem));
					TemplateComboBoxItem newTemplateItem = new TemplateComboBoxItem(newDisplayItem.Shape.Name, fileName);
					comboBoxTemplates.Items.Add(newTemplateItem);
				}
				catch (Exception ex) {
					MessageBox.Show("There was an error loading the template file (" + file + "): " + ex.Message,
									"Error Loading Template", MessageBoxButtons.OKCancel);
				}
				finally {
					if (selectedTemplateItem != null && comboBoxTemplates.Items.IndexOf(selectedTemplateItem) >= 0) {
						comboBoxTemplates.SelectedItem = selectedTemplateItem;
					}
					if (comboBoxTemplates.SelectedItem == null && comboBoxTemplates.Items.Count > 0) {
						comboBoxTemplates.SelectedIndex = 0;
					}
				}
			}
		}

		private void buttonAddTemplate_Click(object sender, EventArgs e) {
			previewForm.Preview.CreateTemplate();
			PopulateTemplateList();
		}

		private void buttonAddToPreview_Click(object sender, EventArgs e) {
			TemplateComboBoxItem templateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			if (templateItem != null) {
				previewForm.Preview.AddTtemplateToPreview(templateItem.FileName);
			}
		}

		private void buttonDeleteTemplate_Click(object sender, EventArgs e) {
			TemplateComboBoxItem templateItem = comboBoxTemplates.SelectedItem as TemplateComboBoxItem;
			if (templateItem != null) {
				if (System.IO.File.Exists(templateItem.FileName)) {
					if (
						MessageBox.Show("Are you sure you want to delete the template '" + templateItem.FileName + "'", "Delete Template",
										MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes) {
						System.IO.File.Delete(templateItem.FileName);
						PopulateTemplateList();
					}
				}
			}
		}

		private void buttonTemplateHelp_Click(object sender, EventArgs e) {
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_CustomShape);
		}

		#endregion // Templates

		private void SaveLocationDataForElements()
		{
			if (Data.SaveLocations)
			{
				Cursor = Cursors.WaitCursor;
				foreach (var d in _data.DisplayItems)
				{
					//_data.DisplayItems.ForEach(d => {
					foreach (var p in d.Shape.Pixels.Where(pi => pi != null && pi.Node != null))
					{

						//LocationModule prop= null;
						if (!p.Node.Properties.Contains(LocationDescriptor._typeId))
							p.Node.Properties.Add(LocationDescriptor._typeId);

						//d.Shape._pixels.ForEach(p => {

						var prop = p.Node.Properties.Get(LocationDescriptor._typeId);
						((LocationData) prop.ModuleData).X = p.X;
						((LocationData) prop.ModuleData).Y = p.Y;

						//});
						//});
					}
				}
				Cursor = Cursors.Default;
			}
		}

		private void propInformationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			previewForm.Preview.ShowInfo = !previewForm.Preview.ShowInfo;
			propInformationToolStripMenuItem.Checked = previewForm.Preview.ShowInfo;
		}

		private void useDirect2DPreviewRenderingToolStripMenuItem_Click(object sender, EventArgs e) {
			var msg = MessageBox.Show("Preview will be restarted. This is a system-wide change that will apply to all previews. Are you sure you want to do this?", "Change Preview", MessageBoxButtons.YesNo);
			if (msg == System.Windows.Forms.DialogResult.Yes) {
				Properties.Settings settings = new Properties.Settings();
				settings.UseGDIRendering = !useDirect2DPreviewRenderingToolStripMenuItem.Checked;
				settings.Save();
			}
		}

		private void saveLocationsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Data.SaveLocations = saveLocationsToolStripMenuItem.Checked;
        }

        private void trackerZoom_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            double zoomLevel = Convert.ToDouble(Convert.ToDouble(trackerZoom.Value) / 100d);
            previewForm.Preview.ZoomLevel = zoomLevel;
        }

        private void SetZoomTextAndTracker(double zoomLevel)
        {
            int zoomPercent = Convert.ToInt32(zoomLevel * 100);
            labelZoomLevel.Text = zoomPercent + "%";
            trackerZoom.Value = Convert.ToInt32(zoomPercent);
            trackerZoom.Invalidate();
        }

		#region Undo

		private void InitUndo()
		{
			_undoMgr = new UndoManager();
			_undoMgr.UndoItemsChanged += _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged += _undoMgr_RedoItemsChanged;

			undoButton.Enabled = false;
			undoButton.ItemChosen += undoButton_ItemChosen;

			redoButton.Enabled = false;
			redoButton.ItemChosen += redoButton_ItemChosen;
		}


		private void undoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Undo();
			previewForm.Preview.DeSelectSelectedDisplayItem();
		}

		private void undoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Undo(e.NumItems);
		}

		private void redoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Redo();
		}

		private void redoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Redo(e.NumItems);
		}


		private void _undoMgr_UndoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumUndoable == 0)
			{
				undoButton.Enabled = false;
				return;
			}

			undoButton.Enabled = true;
			undoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.UndoActions)
				undoButton.UndoItems.Add(act.Description);
		}

		private void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumRedoable == 0)
			{
				redoButton.Enabled = false;
				return;
			}

			redoButton.Enabled = true;
			redoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.RedoActions)
				redoButton.UndoItems.Add(act.Description);
		}

		public void SwapPlaces(Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> changedElements)
		{
			SwapElementPlacement(changedElements);
		}

		public void SwapElementPlacement(Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> changedElements)
		{
			foreach (KeyValuePair<DisplayItem, VixenPreviewControl.ElementPositionInfo> e in changedElements)
			{
				// Key is reference to actual element. Value is class with its times before move.
				// Swap the element's times with the saved times from before the move, so we can restore them later in redo.
				SwapPlaces(e.Key, e.Value);
			}
		}

		public static void SwapPlaces(DisplayItem lhs, VixenPreviewControl.ElementPositionInfo rhs)
		{
			int temp = lhs.Shape.Left;
			lhs.Shape.Left = rhs.LeftPosition;
			rhs.LeftPosition = temp;

			temp = lhs.Shape.Top;
			lhs.Shape.Top = rhs.TopPosition;
			rhs.TopPosition = temp;
		}
		#endregion
	}
	public class ElementsChangedPreviewEventArgs : EventArgs
	{
		public ElementsChangedPreviewEventArgs(VixenPreviewControl.ElementPreviewMoveInfo info, VixenPreviewControl.DisplayMoveType type)
		{
			if (info != null)
			{
				PreviousTimes = info.OriginalPreviewElements;
				Type = type;
			}
		}

		public Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> PreviousTimes { get; private set; }

		public VixenPreviewControl.DisplayMoveType Type { get; private set; }
	}

}