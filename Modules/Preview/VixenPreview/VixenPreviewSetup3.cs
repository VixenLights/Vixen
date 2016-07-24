using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources;
using System.IO;
using Common.Controls.Scaling;
using VixenModules.Editor.VixenPreviewSetup3.Undo;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Location;
using Common.Resources.Properties;
using Button = System.Windows.Forms.Button;
using Control = System.Windows.Forms.Control;

namespace VixenModules.Preview.VixenPreview {
	public partial class VixenPreviewSetup3 : BaseForm
    {
        private VixenPreviewData _data;
		public static VixenPreviewSetupDocument previewForm;
		private VixenPreviewSetupElementsDocument elementsForm;
		private VixenPreviewSetupPropertiesDocument propertiesForm;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public static string DrawShape;
		// Undo manager
		private UndoManager _undoMgr;

		public event EventHandler<PreviewItemMoveEventArgs> PreviewItemsAlignNew;

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
			Icon = Resources.Icon_Vixen3;
			menuStrip.Renderer = new ThemeToolStripRenderer();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			panel10.BackColor = Color.Black;
			foreach (Control c in panel10.Controls)
			{
				c.BackColor = Color.Black;
			}
			dockPanel.BackColor = ThemeColorTable.BackgroundColor;
			label9.ForeColor = Color.Turquoise;
			label10.ForeColor = Color.LimeGreen;
			label11.ForeColor = Color.White;
			label12.ForeColor = Color.HotPink;
			label13.ForeColor = Color.Yellow;

			this.ShowInTaskbar = false;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			undoButton.Image = Tools.GetIcon(Resources.arrow_undo, iconSize);
		    undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
		    redoButton.Image = Tools.GetIcon(Resources.arrow_redo, iconSize);
		    redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
		    redoButton.ButtonType = UndoButtonType.RedoButton;

			undoToolStripMenuItem.Enabled = false;
			redoToolStripMenuItem.Enabled = false;

	    }

	    private void VixenPreviewSetup3_Load(object sender, EventArgs e) {
			previewForm = new VixenPreviewSetupDocument();
			if (!DesignMode && previewForm != null)
				previewForm.Preview.Data = _data;
			previewForm.Preview.OnSelectDisplayItem += OnSelectDisplayItem;
			previewForm.Preview.OnDeSelectDisplayItem += OnDeSelectDisplayItem;

			previewForm.Preview.OnChangeZoomLevel += VixenPreviewSetup3_ChangeZoomLevel;
			PreviewItemsAlignNew += vixenpreviewControl_PreviewItemsAlignNew;

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

		private void VixenPreviewSetup3_FormClosing(object sender, FormClosingEventArgs e)
		{
			PreviewItemsAlignNew -= vixenpreviewControl_PreviewItemsAlignNew;
			previewForm.Preview.OnSelectDisplayItem -= OnSelectDisplayItem;
			previewForm.Preview.OnDeSelectDisplayItem -= OnDeSelectDisplayItem;
			VixenPreviewControl.PreviewItemsResizingNew -= previewForm.Preview.vixenpreviewControl_PreviewItemsResizingNew;
			VixenPreviewControl.PreviewItemsMovedNew -= previewForm.Preview.vixenpreviewControl_PreviewItemsMovedNew;
			_undoMgr.UndoItemsChanged -= _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged -= _undoMgr_RedoItemsChanged;
			undoButton.ItemChosen -= undoButton_ItemChosen;
			redoButton.ItemChosen -= redoButton_ItemChosen;
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
		}

		private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem) {
			Shapes.DisplayItemBaseControl setupControl = displayItem.Shape.GetSetupControl();

			if (setupControl != null) {
				propertiesForm.ShowSetupControl(setupControl);
			}
		}

        private void VixenPreviewSetup3_ChangeZoomLevel(object sender, double zoomLevel) 
        {
            SetZoomTextAndTracker(zoomLevel);
        }

	    private void toolbarButton_Click(object sender, EventArgs e)
	    {
		    Button button = sender as Button;
		    buttonShapeSelected(button);
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
        }

        private void toolbarAlignButton_Click(object sender, EventArgs e)
        {
			VixenPreviewControl.modifyType = "Move";
			previewForm.Preview.beginResize_Move(true); //Starts the Undo Process
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

			PreviewItemsAlignNew(this, new PreviewItemMoveEventArgs(previewForm.Preview.m_previewItemResizeMoveInfo));
			previewForm.Preview.m_previewItemResizeMoveInfo = null;
        }

		public void vixenpreviewControl_PreviewItemsAlignNew(object sender, PreviewItemMoveEventArgs e)
		{
			var action = new PreviewItemsMoveUndoAction(previewForm.Preview, e.PreviousMove);
			_undoMgr.AddUndoAction(action);
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
			SaveLocationDataForElements();
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("An invalid image size was specified!", "Invalid Size", false, true);
					messageBox.ShowDialog();
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There was an error loading the template file (" + file + "): " + ex.Message,
									"Error Loading Template", false, true);
					messageBox.ShowDialog();
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Are you sure you want to delete the template '" + templateItem.FileName + "'", "Delete Template", true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
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
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Preview will be restarted. This is a system-wide change that will apply to all previews. Are you sure you want to do this?", "Change Preview", true, false);
			messageBox.ShowDialog();
			if (messageBox.DialogResult == DialogResult.OK)
			{
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

		#region Undo/Redo Control

		private void InitUndo()
		{
			_undoMgr = new UndoManager();
			_undoMgr.UndoItemsChanged += _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged += _undoMgr_RedoItemsChanged;

			undoButton.Enabled = false;
			undoButton.ItemChosen += undoButton_ItemChosen;

			redoButton.Enabled = false;
			redoButton.ItemChosen += redoButton_ItemChosen;
			previewForm.Preview.UndoManager = _undoMgr;
		}

		private void undoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Undo();
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
				undoToolStripMenuItem.Enabled = false;
				return;
			}

			undoButton.Enabled = true;
			undoToolStripMenuItem.Enabled = true;
			undoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.UndoActions)
			{
				undoButton.UndoItems.Add(act.Description);
			}
		}

	    private void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
	    {
		    if (_undoMgr.NumRedoable == 0)
		    {
				redoButton.Enabled = false;
				redoToolStripMenuItem.Enabled = false;
			    return;
		    }

			redoButton.Enabled = true;
			redoToolStripMenuItem.Enabled = true;
		    redoButton.UndoItems.Clear();
		    foreach (var act in _undoMgr.RedoActions)
		    {
				redoButton.UndoItems.Add(act.Description);
			}
		}
		#endregion


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

		private void buttonShapeSelected(Control selectedButton)
	    {
			foreach (Control c in panel3.Controls)
			{
				if (c is Button)
				{
					c.BackColor = ThemeColorTable.BackgroundColor;
				}
			}
			foreach (Control c in panel4.Controls)
			{
				if (c is Button)
				{
					c.BackColor = ThemeColorTable.BackgroundColor;
				}
			}
			selectedButton.BackColor = ThemeColorTable.TextBoxBackgroundColor;
	    }

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

	}


	public class PreviewItemMoveEventArgs : EventArgs
	{
		public PreviewItemMoveEventArgs(VixenPreviewControl.PreviewItemResizeMoveInfo info)
		{
			if (info != null)
				PreviousMove = info.OriginalPreviewItem;
		}

		public Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> PreviousMove { get; private set; }
	}

	public class PreviewItemResizingEventArgs : EventArgs
	{
		public PreviewItemResizingEventArgs(VixenPreviewControl.PreviewItemResizeMoveInfo info)
		{
			if (info != null)
				PreviousSize = info.OriginalPreviewItem;
		}

		public Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> PreviousSize { get; private set; }
	}

}