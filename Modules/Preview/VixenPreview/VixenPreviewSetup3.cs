using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources;
using System.IO;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Common.Controls.Scaling;
using VixenModules.Editor.VixenPreviewSetup3.Undo;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Property.Location;
using Common.Resources.Properties;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Undo;
using WeifenLuo.WinFormsUI.Docking;
using Button = System.Windows.Forms.Button;
using Control = System.Windows.Forms.Control;
using Cursors = System.Windows.Forms.Cursors;
using CustomPropEditorWindow = VixenModules.App.CustomPropEditor.Views.CustomPropEditorWindow;
using Size = System.Drawing.Size;

namespace VixenModules.Preview.VixenPreview
{
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
			var scaleFactor = ScalingTools.GetScaleFactor();
			menuStrip.Renderer = new ThemeToolStripRenderer();
			int imageSize = (int)(16 * scaleFactor);
			menuStrip.ImageScalingSize = new Size(imageSize, imageSize);

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * scaleFactor);
			undoButton.Image = Tools.GetIcon(Resources.arrow_undo, iconSize);
			undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.Image = Tools.GetIcon(Resources.arrow_redo, iconSize);
			redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.ButtonType = UndoButtonType.RedoButton;

			btnBulbIncrease.Image = Tools.GetIcon(Resources.buttonBulbBigger, iconSize);
			btnBulbDecrease.Image = Tools.GetIcon(Resources.buttonBulbSmaller, iconSize);
			
			btnAddCustomProp.Image = Tools.GetIcon(Resources.Prop_Add, iconSize);
			btnCustomPropEditor.Image = Tools.GetIcon(Resources.Prop_Edit, iconSize);
			btnCustomPropLibrary.Image = Tools.GetIcon(Resources.folder_explore, iconSize);

			btnAddCustomProp.Text = string.Empty;
			btnCustomPropEditor.Text = string.Empty;
			btnCustomPropLibrary.Text = string.Empty;
			
			buttonAlignLeft.Image = Tools.GetIcon(Resources.buttonAlignLeft_BackgroundImage, iconSize);
			buttonAlignBottom.Image = Tools.GetIcon(Resources.buttonAlignBottom_BackgroundImage, iconSize);
			buttonAlignHorizMid.Image = Tools.GetIcon(Resources.buttonAlignHorizMid_BackgroundImage, iconSize);
			buttonAlignRight.Image = Tools.GetIcon(Resources.buttonAlignRight_BackgroundImage, iconSize);
			buttonAlignTop.Image = Tools.GetIcon(Resources.buttonAlignTop_BackgroundImage, iconSize);
			buttonAlignVertMid.Image = Tools.GetIcon(Resources.buttonAlignVertMid_BackgroundImage, iconSize);
			buttonDistributeHorizontal.Image = Tools.GetIcon(Resources.buttonDistributeHorizontal_BackgroundImage, iconSize);
			buttonDistributeVertical.Image = Tools.GetIcon(Resources.buttonDistributeVertical_BackgroundImage, iconSize);
			buttonMatchProperties.Image = Tools.GetIcon(Resources.buttonMatchProperties_BackgroundImage, iconSize);

			buttonAlignLeft.Text = string.Empty;
			buttonAlignBottom.Text = string.Empty;
			buttonAlignHorizMid.Text = string.Empty;
			buttonAlignRight.Text = string.Empty;
			buttonAlignTop.Text = string.Empty;
			buttonAlignVertMid.Text = string.Empty;
			buttonDistributeHorizontal.Text = string.Empty;
			buttonDistributeVertical.Text = string.Empty;
			buttonMatchProperties.Text = string.Empty;

			tlpToolBar.BorderStyle = BorderStyle.FixedSingle;
			ThemeUpdateControls.UpdateControls(this);
			panel10.BackColor = Color.Black;
			foreach (Control c in panel10.Controls)
			{
				c.BackColor = Color.Black;
			}
			dockPanel.BackColor = ThemeColorTable.BackgroundColor;

			var theme = new VS2015DarkTheme();
			dockPanel.Theme = theme;

			label9.ForeColor = Color.Turquoise;
			label10.ForeColor = Color.LimeGreen;
			label11.ForeColor = Color.White;
			label12.ForeColor = Color.HotPink;
			label13.ForeColor = Color.Yellow;

			this.ShowInTaskbar = false;

			undoToolStripMenuItem.Enabled = false;
			redoToolStripMenuItem.Enabled = false;
			trackerZoom.Maximum = Environment.Is64BitProcess ? 400 : 200;
		}

		private void VixenPreviewSetup3_Load(object sender, EventArgs e) {
			previewForm = new VixenPreviewSetupDocument();
			if (!DesignMode && previewForm != null)
				previewForm.Preview.Data = _data;
			previewForm.Preview.OnSelectDisplayItem += OnSelectDisplayItem;
			previewForm.Preview.OnDeSelectDisplayItem += OnDeSelectDisplayItem;

			previewForm.Preview.OnSelectionChanged += Preview_OnSelectionChanged;

			previewForm.Preview.OnChangeZoomLevel += VixenPreviewSetup3_ChangeZoomLevel;
			PreviewItemsAlignNew += vixenpreviewControl_PreviewItemsAlignNew;

			elementsForm = new VixenPreviewSetupElementsDocument(previewForm.Preview);
			propertiesForm = new VixenPreviewSetupPropertiesDocument(previewForm.Preview);

			previewForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
			elementsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
			propertiesForm.Show(elementsForm.Pane, WeifenLuo.WinFormsUI.Docking.DockAlignment.Bottom, 0.5);

			previewForm.Preview.elementsForm = elementsForm;
			previewForm.Preview.propertiesForm = propertiesForm;

			previewForm.Preview.LoadBackground();	
			
			trackBarBackgroundAlpha.Value = Data.BackgroundAlpha;
			previewForm.Preview.Reload();

			Setup();

			useOpenGLPreviewToolStripMenuItem.Checked = Data.UseOpenGL;
			useOpenGLPreviewToolStripMenuItem.Enabled = VixenPreviewModuleInstance.SupportsOpenGLPreview();
			saveLocationsToolStripMenuItem.Checked = Data.SaveLocations;

			// Choose the select tool to start
			toolbarButton_Click(buttonSelect, new EventArgs());

			SetZoomTextAndTracker(previewForm.Preview.ZoomLevel);
			
			InitUndo();
		}

		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return  Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location)) ||
				Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(new Point(rect.Top, rect.Right)));

		}

		private void VixenPreviewSetup3_FormClosing(object sender, FormClosingEventArgs e)
		{
			previewForm.Preview.ZoomLevel = 1;
			PreviewItemsAlignNew -= vixenpreviewControl_PreviewItemsAlignNew;
			previewForm.Preview.OnSelectDisplayItem -= OnSelectDisplayItem;
			previewForm.Preview.OnDeSelectDisplayItem -= OnDeSelectDisplayItem;
			previewForm.Preview.OnSelectionChanged -= Preview_OnSelectionChanged;
			VixenPreviewControl.PreviewItemsResizingNew -= previewForm.Preview.vixenpreviewControl_PreviewItemsResizingNew;
			VixenPreviewControl.PreviewItemsMovedNew -= previewForm.Preview.vixenpreviewControl_PreviewItemsMovedNew;
			_undoMgr.UndoItemsChanged -= _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged -= _undoMgr_RedoItemsChanged;
			undoButton.ItemChosen -= undoButton_ItemChosen;
			redoButton.ItemChosen -= redoButton_ItemChosen;
			CloseSetup();
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
			propertiesForm.ClearSetupControl();
		}

		private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem) {
			Shapes.DisplayItemBaseControl setupControl = displayItem.Shape.GetSetupControl();
			elementsForm.ClearSelectedNodes();
			if (setupControl != null) {
				propertiesForm.ShowSetupControl(setupControl);
			}
		}

	    private void Preview_OnSelectionChanged(object sender, EventArgs args)
	    {
			SetButtonEnabledState();
		}

		private void SetButtonEnabledState()
	    {
		    btnBulbIncrease.Enabled = btnBulbDecrease.Enabled = previewForm.Preview.IsSingleItemSelected || previewForm.Preview.SelectedDisplayItems.Any();

		    var multiSelect = previewForm.Preview.SelectedDisplayItems.Count > 1;
			//btnBulbMatch.Enabled = multiSelect;
		    buttonAlignBottom.Enabled = multiSelect;
		    buttonAlignHorizMid.Enabled = multiSelect;
		    buttonAlignLeft.Enabled = multiSelect;
		    buttonAlignRight.Enabled = multiSelect;
		    buttonAlignTop.Enabled = multiSelect;
		    buttonAlignVertMid.Enabled = multiSelect;
		    buttonDistributeHorizontal.Enabled = buttonDistributeVertical.Enabled = multiSelect;
		    buttonMatchProperties.Enabled = multiSelect;
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
		    previewForm.Preview.ItemIndex = 1;
		    previewForm.Preview.ItemName = String.Empty;
		   
		    // There must be a way to iterate through an enum so we don't have to do all this crap...

		    // Select Button
			DrawShape = "";
			if (button == buttonSelect)
				previewForm.Preview.CurrentTool = VixenPreviewControl.Tools.Select;
			else if (button == buttonDrawPixel)
			{
				if (Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
				{
					using (PreviewPixelSetupForm inputDialog = new PreviewPixelSetupForm("Pixel", 1, 3))
					{
						if (inputDialog.ShowDialog() == DialogResult.OK)
						{
							if (inputDialog.PrefixName != string.Empty)
							{
								previewForm.Preview.ItemName = inputDialog.PrefixName;
							}
							previewForm.Preview.ItemIndex = inputDialog.StartingIndex;
							previewForm.Preview.ItemBulbSize = inputDialog.LightSize;
						}
					}
					//using (TextDialog textDialog = new TextDialog("Item Name?", "Item Name", "Pixel", true))
					//{
					//	if (textDialog.ShowDialog() == DialogResult.OK)
					//	{
					//		if (textDialog.Response != string.Empty)
					//		{
					//			previewForm.Preview.ItemName = textDialog.Response;
					//		}
					//	}
					//}

					//if (previewForm.Preview.ItemName != String.Empty)
					//{
					//	using (NumberDialog numberDialog = new NumberDialog("Item Index", "Item Start Index", 1, 1))
					//	{
					//		if (numberDialog.ShowDialog() == DialogResult.OK)
					//		{
					//			previewForm.Preview.ItemIndex = numberDialog.Value;
					//		}
					//	}
					//}

				}
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

		public void Setup()
		{

			var desktopBounds =
				new Rectangle(
					new Point(Data.SetupLeft, Data.SetupTop),
					new Size(Data.SetupWidth, Data.SetupHeight));

			if (IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopBounds = desktopBounds;
			}
			else
			{
				StartPosition = FormStartPosition.WindowsDefaultLocation;
			}

			//SetDesktopLocation(Data.SetupLeft, Data.SetupTop);
			//Size = new Size(Data.SetupWidth, Data.SetupHeight);
		}

		private void CloseSetup()
	    {
		    SaveLocationDataForElements();
		    DialogResult = DialogResult.OK;
		    previewForm.Close();
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("An invalid image size was specified!", "Invalid Size", false, true);
					messageBox.ShowDialog();
                }
            }
		}

		#region Templates

		private void buttonAddToPreview_Click(object sender, EventArgs e) {

			TemplateDialog td = new TemplateDialog();
			var result = td.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(td.FileName))
				{
					previewForm.Preview.AddTtemplateToPreview(td.FileName);
				}
			}

			
		}

		private void templateHelpToolStripMenuItem_Click(object sender, EventArgs e)
	    {
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
					foreach (var p in d.Shape.Pixels.Where(pi => pi != null && pi.Node != null))
					{
						if (!p.Node.Properties.Contains(LocationDescriptor._typeId))
							p.Node.Properties.Add(LocationDescriptor._typeId);

						var prop = p.Node.Properties.Get(LocationDescriptor._typeId);
					    ((LocationData) prop.ModuleData).X = p.IsHighPrecision ? (int)(p.Location.X + Data.LocationOffset.X): p.X + Convert.ToInt32(Data.LocationOffset.X);
						((LocationData) prop.ModuleData).Y = p.IsHighPrecision ? (int)(p.Location.Y + Data.LocationOffset.Y): p.Y + Convert.ToInt32(Data.LocationOffset.Y);
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

		private void useOpenGLPreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (useOpenGLPreviewToolStripMenuItem.Checked)
			{
				if (VixenPreviewModuleInstance.SupportsOpenGLPreview())
				{
					Data.UseOpenGL = true;
				}
				else
				{
					Data.UseOpenGL = false;
					useOpenGLPreviewToolStripMenuItem.Checked = false;
					var messageBox = new MessageBoxForm("Open GL Preview is not supported on your hardware. Reverting to the GDI preview.", "Change Preview Viewer", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
			}
			else
			{
				Data.UseOpenGL = false;
			}
		}

	    private void saveLocationsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Data.SaveLocations = saveLocationsToolStripMenuItem.Checked;
        }

        private void trackerZoom_ValueChanged(Common.Controls.ControlsEx.ValueControls.ValueControl sender, Common.Controls.ControlsEx.ValueControls.ValueChangedEventArgs e)
        {
            double zoomLevel = trackerZoom.Value / 100d;
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
			previewForm.Preview.BeginUpdate();
			_undoMgr.Undo();
			previewForm.Preview.EndUpdate();
		}

		private void undoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			previewForm.Preview.BeginUpdate();
			_undoMgr.Undo(e.NumItems);
			previewForm.Preview.EndUpdate();
		}

		private void redoButton_ButtonClick(object sender, EventArgs e)
		{
			previewForm.Preview.BeginUpdate();
			_undoMgr.Redo();
			previewForm.Preview.EndUpdate();
		}

		private void redoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			previewForm.Preview.BeginUpdate();
			_undoMgr.Redo(e.NumItems);
			previewForm.Preview.EndUpdate();
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
		
		private void buttonShapeSelected(Control selectedButton)
	    {
			//foreach (Control c in pnlBasicDrawing.Controls)
			//{
			//	if (c is Button)
			//	{
			//		c.BackColor = ThemeColorTable.BackgroundColor;
			//	}
			//}
			//foreach (Control c in pnlSmartObjects.Controls)
			//{
			//	if (c is Button)
			//	{
			//		c.BackColor = ThemeColorTable.BackgroundColor;
			//	}
			//}
			ResetButtonBackground(pnlBasicDrawing);
			ResetButtonBackground(pnlSmartObjects);
			selectedButton.BackColor = ThemeColorTable.TextBoxBackgroundColor;
	    }

	    private void ResetButtonBackground(Control c)
	    {
		    if (c is Button)
		    {
			    c.BackColor = ThemeColorTable.BackgroundColor;
		    }
		    else
		    {
			    foreach (Control cControl in c.Controls)
			    {
				    ResetButtonBackground(cControl);
			    }
		    }
		}

		private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveLocationDataForElements();
			await VixenSystem.SaveSystemAndModuleConfigAsync();
		}

		private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Main);
		}

	    private void vixenYouTubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
	    {
		    Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.YouTubeChannel);
	    }

		private void locationOffsetSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LocationOffsetForm offsetForm = new LocationOffsetForm(Data.LocationOffset);
	        var result = offsetForm.ShowDialog();
	        if (result == DialogResult.OK)
	        {
		        Data.LocationOffset = offsetForm.Offset;
	        }
        }

		private async void importPropToolStripMenuItem_Click(object sender, EventArgs e)
		{
			await previewForm.Preview.ImportCustomProp();
		}

		private void btnCustomPropEditor_Click(object sender, EventArgs e)
		{
			var form = new CustomPropEditorWindow();
			ElementHost.EnableModelessKeyboardInterop(form);
			form.ShowDialog();
		}

		private void buttonCustomPropLibrary_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
		}

		private void btnBulbDecrease_Click(object sender, EventArgs e)
		{
			previewForm.Preview.DecreaseBulbSize();
		}

		private void btnBulbIncrease_Click(object sender, EventArgs e)
		{
			previewForm.Preview.IncreaseBulbSize();
		}

		private void btnBulbMatch_Click(object sender, EventArgs e)
		{
			previewForm.Preview.MatchBulbSize();
		}
	}


	public class PreviewItemMoveEventArgs : EventArgs
	{
		public PreviewItemMoveEventArgs(PreviewItemResizeMoveInfo info)
		{
			if (info != null)
				PreviousMove = info.OriginalPreviewItem;
		}

		public Dictionary<DisplayItem, PreviewItemPositionInfo> PreviousMove { get; private set; }
	}

	public class PreviewItemResizingEventArgs : EventArgs
	{
		public PreviewItemResizingEventArgs(PreviewItemResizeMoveInfo info)
		{
			if (info != null)
				PreviousSize = info.OriginalPreviewItem;
		}

		public Dictionary<DisplayItem, PreviewItemPositionInfo> PreviousSize { get; private set; }
	}

}