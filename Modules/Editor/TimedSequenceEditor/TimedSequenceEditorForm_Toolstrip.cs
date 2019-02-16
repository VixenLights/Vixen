using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.App;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys.State;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using ZedGraph;
using Size = System.Drawing.Size;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		#region Member Variables

		//Used for the Effects ToolStrip
		private ToolStripItem _mToolStripEffects;
		private bool _beginNewEffectDragDrop;
		private List<AllToolStripItems> _allToolStripItems;
		private readonly string _toolStripFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ToolStripLocationSettings.xml");
		private bool _mouseDown;

		// Libraries
		private readonly string _colorFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ColorPalette.xml");
		private List<Color> _colors = new List<Color>();
		private ToolStrip _currentToolStrip = null;
		private ToolStripButton _selectedButton;
		private bool _toolStripButtonAlreadyChecked;
		private ToolStrip _contextToolStrip;
		private string _lastFolder;
		private bool _itemMove;
		private bool _dragValid;

		#endregion

		#region Initial Sequence Editor Load

		private void InitializeToolBars()
		{
			// Audio Toolstrip Settings
			AudioToolStripSetup();
			// PlayBack Toolstrip Settings
			PlayBackToolStripSetup();
			// Edit Toolstrip Settings
			EditToolStripSetup();
			// Alignment Toolstrip Settings
			AlignmentToolStripSetup();
			// File Toolstrip Settings
			FileToolStripSetup();
			// View Toolstrip Settings
			ViewToolStripSetup();
			// Tools Toolstrip Settings
			ToolsToolStripSetup();
			// Mode Toolstrip Settings
			ModeToolStripSetup();
			
			XMLProfileSettings xml = new XMLProfileSettings();

			// Effect Toolstrip Context menu Label setting.
			string effectLabelPosition = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/EffectLabelIndex", Name), "2");
			foreach (ToolStripMenuItem subItem in toolStripMenuItemLabelPosition.DropDown.Items)
			{
				if (subItem.Tag.ToString() == effectLabelPosition)
				{
					subItem.Checked = true;
					break;
				}
			}

			// Effect context menu Groups.
			basicToolStripMenuItem.Checked = (xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/BasicEffectToolStrip", Name), true));
			pixelToolStripMenuItem.Checked = (xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/PixelEffectToolStrip", Name), true));
			deviceToolStripMenuItem.Checked = (xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/DeviceEffectToolStrip", Name), true));
			
			// Load ToolStrip Items form saved file.
			Load_ToolsStripItemsFile();

			PopulateEffectsToolStrip_Context();
			PopulateColorLibraryToolStrip_Context();
			PopulateCurveLibraryToolStrip_Context();
			PopulateGradientLibraryToolStrip_Context();
			PopulateAllToolStrips();

			// Adjust ToolStrip Height
			int imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/ColorLibrary", Name), _toolStripImageSize);
			ChangeToolStripImageSize(toolStripColorLibrary, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/GradientLibrary", Name), _toolStripImageSize);
			ChangeToolStripImageSize(toolStripGradientLibrary, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/CurveLibrary", Name), _toolStripImageSize);
			ChangeToolStripImageSize(toolStripCurveLibrary, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/PlayBack", Name), _iconSize);
			ChangeToolStripImageSize(toolStripPlayBack, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Alignment", Name), _iconSize);
			ChangeToolStripImageSize(toolStripAlignment, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Audio", Name), _iconSize);
			ChangeToolStripImageSize(toolStripAudio, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Edit", Name), _iconSize);
			ChangeToolStripImageSize(toolStripEdit, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Mode", Name), _iconSize);
			ChangeToolStripImageSize(toolStripMode, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/View", Name), _iconSize);
			ChangeToolStripImageSize(toolStripView, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/File", Name), _iconSize);
			ChangeToolStripImageSize(toolStripFile, imageSize);
			imageSize = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Tools", Name), _iconSize);
			ChangeToolStripImageSize(toolStripTools, imageSize);

		}

		private void PlayBackToolStripSetup()
		{
			toolStripPlayBack.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			playBackToolStripButton_Start.Image = Resources.control_start_blue;
			playBackToolStripButton_Start.DisplayStyle = ToolStripItemDisplayStyle.Image;
			playBackToolStripButton_Play.Image = Resources.control_play_blue;
			playBackToolStripButton_Play.DisplayStyle = ToolStripItemDisplayStyle.Image;
			playBackToolStripButton_Stop.Image = Resources.control_stop_blue;
			playBackToolStripButton_Stop.DisplayStyle = ToolStripItemDisplayStyle.Image;
			playBackToolStripButton_Pause.Image = Resources.control_pause_blue;
			playBackToolStripButton_Pause.DisplayStyle = ToolStripItemDisplayStyle.Image;
			playBackToolStripButton_End.Image = Resources.control_end_blue;
			playBackToolStripButton_End.DisplayStyle = ToolStripItemDisplayStyle.Image;
			playBackToolStripButton_Loop.Image = Resources.arrow_repeat;
			playBackToolStripButton_Loop.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void EditToolStripSetup()
		{
			toolStripEdit.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			editToolStripButton_Undo.Image = Resources.arrow_undo;
			editToolStripButton_Undo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			editToolStripButton_Redo.Image = Resources.arrow_redo;
			editToolStripButton_Redo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			editToolStripButton_Redo.ButtonType = UndoButtonType.RedoButton;
			editToolStripButton_Cut.Image = Resources.cut;
			editToolStripButton_Cut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			editToolStripButton_Copy.Image = Resources.page_white_copy;
			editToolStripButton_Copy.DisplayStyle = ToolStripItemDisplayStyle.Image;
			editToolStripButton_Paste.Image = Resources.page_white_paste;
			editToolStripButton_Paste.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void AlignmentToolStripSetup()
		{
			toolStripAlignment.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			alignmentToolStripButton_CloseGaps.Image = Resources.fill_gaps;
			alignmentToolStripButton_CloseGaps.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripDropDownButton_AlignTo.Image = Resources.alignment;
			alignmentToolStripDropDownButton_AlignTo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripDropDownButton_AlignTo.ShowDropDownArrow = false;
			alignmentToolStripButton_Start.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripButton_Start.Image = Resources.alignStartMark;
			alignmentToolStripButton_End.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripButton_End.Image = Resources.alignEndMark;
			alignmentToolStripButton_Both.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripButton_Both.Image = Resources.alignBothMark;
			alignmentToolStripButton_Distribute.DisplayStyle = ToolStripItemDisplayStyle.Image;
			alignmentToolStripButton_Distribute.Image = Resources.distribute;
		}

		private void FileToolStripSetup()
		{
			toolStripFile.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			fileToolStripButton_Close.DisplayStyle = ToolStripItemDisplayStyle.Image;
			fileToolStripButton_Close.Image = Resources.Exit;
			fileToolStripButton_AutoSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
			fileToolStripButton_AutoSave.Image = Resources.Auto_Save;
			fileToolStripButton_Export.DisplayStyle = ToolStripItemDisplayStyle.Image;
			fileToolStripButton_Export.Image = Resources.folder_go;
			fileToolStripButton_Save.Image = Resources.Save;
			fileToolStripButton_Save.DisplayStyle = ToolStripItemDisplayStyle.Image;
			fileToolStripButton_SaveAs.Image = Resources.SaveAs;
			fileToolStripButton_SaveAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void ViewToolStripSetup()
		{
			toolStripView.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			viewToolStripButton_ZoomTimeIn.Image = Resources.zoom_time_in;
			viewToolStripButton_ZoomTimeIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
			viewToolStripButton_ZoomTimeOut.Image = Resources.zoom_time_out;
			viewToolStripButton_ZoomTimeOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			viewToolStripButton_ZoomRowIn.Image = Resources.zoom_row_in;
			viewToolStripButton_ZoomRowIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
			viewToolStripButton_ZoomRowOut.Image = Resources.zoom_row_out;
			viewToolStripButton_ZoomRowOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void ToolsToolStripSetup()
		{
			toolStripTools.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			toolsToolStripButton_LipSync.Image = Resources.Lipsync;
			toolsToolStripButton_LipSync.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolsToolStripButton_CurveLibrary.Image = Resources.Curve;
			toolsToolStripButton_CurveLibrary.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolsToolStripButton_ColorLibrary.Image = Resources.Color;
			toolsToolStripButton_ColorLibrary.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolsToolStripButton_ColorGradient.Image = Resources.ColorGradient;
			toolsToolStripButton_ColorGradient.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void AudioToolStripSetup()
		{
			toolStripAudio.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			toolStripButton_AssociateAudio.Image = Resources.music;
			toolStripButton_AssociateAudio.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_IncreaseTimingSpeed.Image = Resources.plus;
			toolStripButton_IncreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_DecreaseTimingSpeed.Image = Resources.minus;
			toolStripButton_DecreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			audioToolStripButton_Audio_Devices.Image = Resources.Audio_Devices;
			audioToolStripButton_Audio_Devices.DisplayStyle = ToolStripItemDisplayStyle.Image;
			audioToolStripLabel_TimingSpeed.Image  = (Image)SpeedVisualisation();
			audioToolStripLabel_TimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripAudio.MouseWheel += toolStripAudio_MouseWheel;
		}

		private void ModeToolStripSetup()
		{
			toolStripMode.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			modeToolStripButton_SnapTo.Image = Resources.magnet;
			modeToolStripButton_SnapTo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			modeToolStripButton_DrawMode.Image = Resources.pencil;
			modeToolStripButton_DrawMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
			modeToolStripButton_SelectionMode.Image = Resources.cursor_arrow;
			modeToolStripButton_SelectionMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
			modeToolStripButton_DragBoxFilter.Image = Resources.table_select_big;
			modeToolStripButton_DragBoxFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
		}

		private void SetToolBarLayout()
		{
			// Set layout and settings for all Toolbars.
			XMLProfileSettings xml = new XMLProfileSettings();
			
			toolStripContainer.SuspendLayout();
			
			// Custom Toolstrip View Menu setting.
			int toolStripCount = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/ToolStripCount", Name), 0);
			
			for (int i = 0; i <= toolStripCount; i++)
			{
				int toolStripLocationX = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripX{1}", Name, i), 0);
				int toolStripLocationY = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripY{1}", Name, i), 0);
				string toolStripName = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripName{1}", Name, i), "");
				bool toolStripVisible = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripVisible{1}", Name, i), false);
				
				switch (toolStripName)
				{
					case "toolStripEffects":
						toolStripEffects.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripEffects.Visible = toolStripVisible;
						break;
					case "toolStripAudio":
						toolStripAudio.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripAudio.Visible = toolStripVisible;
						break;
					case "toolStripAlignment":
						toolStripAlignment.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripAlignment.Visible = toolStripVisible;
						break;
					case "toolStripPlayBack":
						toolStripPlayBack.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripPlayBack.Visible = toolStripVisible;
						break;
					case "toolStripEdit":
						toolStripEdit.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripEdit.Visible = toolStripVisible;
						break;
					case "toolStripFile":
						toolStripFile.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripFile.Visible = toolStripVisible;
						break;
					case "toolStripView":
						toolStripView.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripView.Visible = toolStripVisible;
						break;
					case "toolStripTools":
						toolStripTools.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripTools.Visible = toolStripVisible;
						break;
					case "toolStripMode":
						toolStripMode.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripMode.Visible = toolStripVisible;
						break;
					case "toolStripColorLibrary":
						toolStripColorLibrary.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripColorLibrary.Visible = toolStripVisible;
						break;
					case "toolStripCurveLibrary":
						toolStripCurveLibrary.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripCurveLibrary.Visible = toolStripVisible;
						break;
					case "toolStripGradientLibrary":
						toolStripGradientLibrary.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripGradientLibrary.Visible = toolStripVisible;
						break;
				}
				SetToolStripStartPosition();
			}

			// Will only go hear when the user first loads the new Toolstrip management or when all Toolstrips are hidden.
			if (toolStripCount == 0)
			{
				toolStripPlayBack.Location = new Point(3, 0 + 5);
				toolStripPlayBack.Location = new Point(3, 0 + 5);
				toolStripEdit.Location = new Point(toolStripPlayBack.Location.X + toolStripPlayBack.Width, 5);
				toolStripEdit.Location = new Point(toolStripPlayBack.Location.X + toolStripPlayBack.Width, 5);
				toolStripMode.Location = new Point(toolStripEdit.Location.X + toolStripEdit.Width, 5);
				toolStripMode.Location = new Point(toolStripEdit.Location.X + toolStripEdit.Width, 5);
				toolStripView.Location = new Point(toolStripMode.Location.X + toolStripMode.Width, 5);
				toolStripView.Location = new Point(toolStripMode.Location.X + toolStripMode.Width, 5);
				toolStripAudio.Location = new Point(toolStripView.Location.X + toolStripView.Width, 5);
				toolStripAudio.Location = new Point(toolStripView.Location.X + toolStripView.Width, 5);
			}

			SetToolStripStartPosition();

			toolStripContainer.ResumeLayout();
			toolStripContainer.PerformLayout();

			// Add Theme to all ToolStrips.
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					toolsStripItems.Renderer = new ThemeToolStripRenderer();
				}
			}

			// Add Toolbar list to Context menu
			PopulateContextToolBars(toolbarToolStripMenuItem);
		}

#endregion

		#region All Toolstrips Initialization (Except Effects and Libraries)

		private void PopulateAllToolStrips()
		{
			toolStripContainer.SuspendLayout();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					if (toolsStripItems.Name == "toolStripEffects") continue; // || toolsStripItems.Name.Contains("Library")) continue; // Skip the Effects Toolstrip
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						foreach (AllToolStripItems it in _allToolStripItems)
						{
							if (item.ToolTipText == it.ItemName)
							{
								item.Visible = it.Visible;
								break;
							}
						}

					}
				}
			}
			toolStripContainer.ResumeLayout();
		}

		#endregion

		#region All Toolstrip Events (Except Effects and Libraries)

		// Show/Hide selected Toolstrip item.
		private void ToolStripAllItem_Changed(object sender, EventArgs e)
		{
			ItemsChanged(sender);
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						AllToolStripItems ts = new AllToolStripItems();
						ts.ItemName = item.ToolTipText;
						ts.Visible = item.Visible;
						_allToolStripItems.Add(ts);
					}
				}
			}
		}

		private void ItemsChanged(object sender)
		{
			_allToolStripItems.Clear();
			ToolStripMenuItem tsi = sender as ToolStripMenuItem;
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						if (item.ToolTipText != null && item.ToolTipText == tsi.Text ||
						    item.Tag != null && item.Tag.ToString() == tsi.Text)
						{
							item.Visible = tsi.Checked;
							return;
						}
					}
				}
			}

			SetToolStripStartPosition();
		}

		// Show/Hide selected ToolBar.
		private void ToolStripItem_Changed(object sender, EventArgs e)
		{
			ToolStripMenuItem tsi = sender as ToolStripMenuItem;
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					if (toolsStripItems.Text != null && toolsStripItems.Text == tsi.Text)
					{
						toolsStripItems.Visible = tsi.Checked;
						break;
					}
				}
			}

			// Add Toolbar list to Context menu
			PopulateContextToolBars(toolbarsToolStripMenuItem);
		}

		// Generate Context Menu
		private void contextMenuStripAll_Opening(object sender, CancelEventArgs e)
		{
			ContextMenuStrip toolStripContext = sender as ContextMenuStrip;
			_contextToolStrip = toolStripContext.SourceControl as ToolStrip;
			// Add Toolstrip items to Context menu
			resetToolStripMenuItem.Visible = true;
			add_RemoveContextToolStripMenuItem.Visible = true;
			imageSizeToolStripMenuItem.Visible = true;

			if (_contextToolStrip != null && _contextToolStrip.Parent.Name != "toolStripContainer")
			{
				add_RemoveContextToolStripMenuItem.DropDownItems.Clear();
				// Add Toolbar Items list to Context menu
				PopulateContextToolBarItems(add_RemoveContextToolStripMenuItem);
				resetToolStripMenuItem.Tag = _contextToolStrip;
			}
			else
			{
				resetToolStripMenuItem.Visible = false;
				add_RemoveContextToolStripMenuItem.Visible = false;
				imageSizeToolStripMenuItem.Visible = false;
			}

			// Add Toolbar list to Context menu
			PopulateContextToolBars(toolbarsToolStripMenuItem);
		}

		// Resets Context menu items to default.
		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Resets Context menu items to default.
			ToolStripItem tsm = sender as ToolStripItem;
			ToolStrip ts = tsm.Tag as ToolStrip;
			foreach (ToolStripItem item in ts.Items) item.Visible = true;
		}

		#endregion

		#region Library Toolstrips

		#region All Library Toolstrips

		private void UpdateCurrentLibrary()
		{
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					UpdateColorSettings();
					break;
				case "toolStripCurveLibrary":
					//This is just plain wrong. We should keep track of what moved and do proper reordering.
					//In order for this to happen, the library needs to have support for ordering added
					//or the containers that use them need to maintain order.
					_curveLibrary.BeginBulkUpdate();
					_curveLibrary.Library.Clear();  
					foreach (ToolStripButton tsb in toolStripCurveLibrary.Items)
					{
						_curveLibrary.AddCurve(tsb.Name, (Curve)tsb.Tag);
					}
					_curveLibrary.EndBulkUpdate();
					break;
				case "toolStripGradientLibrary":
					//This is just plain wrong. We should keep track of what moved and do proper reordering.
					//In order for this to happen, the library needs to have support for ordering added
					//or the containers that use them need to maintain order.
					_colorGradientLibrary.BeginBulkUpdate();
					_colorGradientLibrary.Library.Clear();
					foreach (ToolStripButton tsb in toolStripGradientLibrary.Items)
					{
						_colorGradientLibrary.AddColorGradient(tsb.Name, (ColorGradient)tsb.Tag);
					}
					_colorGradientLibrary.EndBulkUpdate();
					break;
			}
		}

		private void SelectNodeForDrawing()
		{
			if (ModifierKeys != Keys.Control)
			{
				foreach (ToolStripItem tsi in _selectedButton.Owner.Items)
				{
					if (tsi.CanSelect)
					{
						ToolStripButton tsb = tsi as ToolStripButton;
						tsb.Checked = false;
					}
				}
				_selectedButton.Checked = _toolStripButtonAlreadyChecked = true;
			}
			else
			{
				_selectedButton.Checked = !_toolStripButtonAlreadyChecked;
				_toolStripButtonAlreadyChecked = true;
			}
		}

		#endregion

		#region All Library Toolstrips Events

		private void contextMenuStripLibraries_Opening(object sender, CancelEventArgs e)
		{
			ContextMenuStrip toolStripContext = sender as ContextMenuStrip;
			_contextToolStrip = toolStripContext.SourceControl as ToolStrip;
			add_RemoveLibraryToolStripMenuItem.DropDownItems.Clear();

			// Add Toolbar Items list to Context menu
			PopulateContextToolBarItems(add_RemoveLibraryToolStripMenuItem);

			resetLibraryToolStripMenuItem.Tag = _contextToolStrip;

			// Add Toolbar list to Context menu
			PopulateContextToolBars(toolBarsToolStripMenuItemLibraries);
		}

		private void toolStrips_MouseEnter(object sender, EventArgs e)
		{
			_currentToolStrip = sender as ToolStrip;
			if (_currentToolStrip != null)
			{
				_contextToolStrip = _currentToolStrip;
				_currentToolStrip.Focus();
			}
		}

		private void toolStripLibraries_MouseEnter(object sender, EventArgs e)
		{
			_selectedButton = sender as ToolStripButton;
			if (_selectedButton != null)
			{
				_toolStripButtonAlreadyChecked = _selectedButton.Checked;
				_selectedButton.Checked = true;
			}
			_contextToolStrip = _selectedButton.Owner;
			_contextToolStrip.Focus();
		}

		private void toolStripLibraries_MouseLeave(object sender, EventArgs e)
		{
			_selectedButton = sender as ToolStripButton;
			if (_selectedButton != null)
			{
				if (!_toolStripButtonAlreadyChecked)
					_selectedButton.Checked = false;
			}
		}

		private void toolStripLibrary_Leave(object sender, EventArgs e)
		{
			if (_mouseDown) return;
			_currentToolStrip = null;
			TimelineControl.Focus();
		}

		void toolStripLibraries_MouseDown(object sender, MouseEventArgs e)
		{
			_selectedButton = sender as ToolStripButton;
			if (_selectedButton == null) return;
			_contextToolStrip = _selectedButton.Owner;
			_contextToolStrip.Focus();
			if (e.Clicks == 2)
			{
				toolStripMenuItemEditItem_Click(sender, null);
				return;
			}
			if (e.Button == MouseButtons.Right)
			{
				_mouseDown = true;
				SelectNodeForDrawing();
			}
			else 
			{
				SelectNodeForDrawing();
				// Move a toolstrip item within the same toolstrip.
				_itemMove = true;
				_dragValid = true;
				_contextToolStrip.DoDragDrop(_selectedButton.Tag, DragDropEffects.Move);
			}
		}

		private void toolStripLibrary_DragLeave(object sender, EventArgs e)
		{
			// Copy the Curve/Gradient/Color to an Effect.
			if (!_dragValid) return;
			_itemMove = false;
			bool link = ModifierKeys == Keys.Control;

			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					_selectedButton.Owner.DoDragDrop(_selectedButton.Tag, DragDropEffects.Copy);
					break;
				case "toolStripCurveLibrary":
					Curve newCrve = new Curve((Curve)_selectedButton.Tag);
					if (link) newCrve.LibraryReferenceName = _selectedButton.Name;
					newCrve.IsCurrentLibraryCurve = false;
					_selectedButton.Owner.DoDragDrop(newCrve, DragDropEffects.Copy);
					break;
				case "toolStripGradientLibrary":
					ColorGradient newGradient = new ColorGradient((ColorGradient)_selectedButton.Tag);
					if (link) newGradient.LibraryReferenceName = _selectedButton.Name;
					newGradient.IsCurrentLibraryGradient = false;
					_selectedButton.Owner.DoDragDrop(newGradient, DragDropEffects.Copy);
					break;
			}
		}

		private void ToolStripItem_Move(DragEventArgs e)
		{
			// Get destination Item index
			int destinationIndex = GetItemIndex(e.X + _selectedButton.Width) - 1;
			
			if (destinationIndex < 0) destinationIndex = 0;
			_contextToolStrip.Items.Remove(_selectedButton);
			_contextToolStrip.Items.Insert(destinationIndex, _selectedButton);
			UpdateCurrentLibrary();
			_itemMove = false;
		}
		
		private int GetItemIndex(int X)
		{
			int index = 0;
			int selectedItemLocation = _contextToolStrip.DisplayRectangle.X + _contextToolStrip.Location.X;
			foreach (ToolStripItem item in _contextToolStrip.Items)
			{
				if (selectedItemLocation < X &&
				    selectedItemLocation + item.Size.Width > X)
				{
					break;
				}
				if (item.Visible) selectedItemLocation += item.Size.Width;
				index++;
			}
			return index;
		}

		#endregion

		#region All Library Export/Import

		private void toolStripMenuItemImport_Click(object sender, EventArgs e)
		{
			string filter;
			string defaultExt;
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					defaultExt = ".vfc";
					filter = @"Vixen 3 Favorite Colors (*.vfc)|*.vfc|All Files (*.*)|*.*";
					break;
				case "toolStripCurveLibrary":
					defaultExt = ".vcl";
					filter = @"Vixen 3 Curve Library (*.vcl)|*.vcl|All Files (*.*)|*.*";
					break;
				default:
					defaultExt = ".vgl";
					filter = @"Vixen 3 Color Gradient Library (*.vgl)|*.vgl|All Files (*.*)|*.*";
					break;
			}

			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = defaultExt,
				Filter = filter,
				FilterIndex = 0
			};

			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);

			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					List<Color> colors = new List<Color>();

					try
					{
						using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
						{
							DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
							colors = (List<Color>)ser.ReadObject(reader);
						}

						foreach (Color color in colors)
						{
							_colors.Add(color);
						}
					}
					catch (Exception ex)
					{
						SendImportErrorMsg(ex, "Invalid file while importing Favorite Colors: ");
					}
					PopulateColors();
					Save_ColorPaletteFile();
					ColorLibraryForm.Load_ColorPaletteFile();
					ColorLibraryForm.PopulateColors();
					break;
				case "toolStripCurveLibrary":
					Dictionary<string, Curve> curves = new Dictionary<string, Curve>();

					try
					{
						using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
						{
							DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, Curve>));
							curves = (Dictionary<string, Curve>)ser.ReadObject(reader);
						}

						_contextMenuStrip.SuspendLayout();
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
						Populate_Curves();
						_contextMenuStrip.ResumeLayout();
					}
					catch (Exception ex)
					{
						SendImportErrorMsg(ex, "Invalid file while importing Curve Library: ");
					}
					break;
				case "toolStripGradientLibrary":
					Dictionary<string, ColorGradient> gradients = new Dictionary<string, ColorGradient>();
					try
					{
						using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
						{
							DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, ColorGradient>));
							gradients = (Dictionary<string, ColorGradient>)ser.ReadObject(reader);
						}
						
						_contextToolStrip.SuspendLayout();
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
						Populate_Gradients();
						_contextToolStrip.ResumeLayout();

					}
					catch (Exception ex)
					{
						SendImportErrorMsg(ex, "Invalid file while importing Color Gradient Library: ");
					}
					break;
			}

		}

		private void toolStripMenuItemExport_Click(object sender, EventArgs e)
		{
			string filter;
			string defaultExt;
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					defaultExt = ".vfc";
					filter = @"Vixen 3 Favorite Colors (*.vfc)|*.vfc|All Files (*.*)|*.*";
					break;
				case "toolStripCurveLibrary":
					defaultExt = ".vcl";
					filter = @"Vixen 3 Curve Library (*.vcl)|*.vcl|All Files (*.*)|*.*";
					break;
				default:
					defaultExt = ".vgl";
					filter = @"Vixen 3 Color Gradient Library (*.vgl)|*.vgl|All Files (*.*)|*.*";
					break;
			}

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = defaultExt,
				Filter = filter
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					try
					{
						DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
						var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
						ser.WriteObject(writer, _colors);
						writer.Close();
					}
					catch (Exception ex)
					{
						SendExportErrorMsg(ex, "While exporting Favorite Colors: ");
					}
					break;
				case "toolStripCurveLibrary":
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
						SendExportErrorMsg(ex, "While exporting Curve Library: ");
					}
					break;
				case "toolStripGradientLibrary":
					try
					{
						Dictionary<string, ColorGradient> gradients = _colorGradientLibrary.ToDictionary(gradient => gradient.Key, gradient => gradient.Value);

						DataContractSerializer ser = new DataContractSerializer(typeof(Dictionary<string, ColorGradient>));
						var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
						ser.WriteObject(writer, gradients);
						writer.Close();
					}
					catch (Exception ex)
					{
						SendExportErrorMsg(ex, "While exporting Color Gradient Library: ");
					}
					break;
			}
		}

		private void SendExportErrorMsg(Exception ex, string errorMsg)
		{
			Logging.Error(errorMsg + saveFileDialog.FileName + " " + ex.InnerException);
			var messageBox = new MessageBoxForm("Unable to export data, please check the error log for details.",
				"Error", MessageBoxButtons.OK, SystemIcons.Error);
			messageBox.ShowDialog();
		}

		private void SendImportErrorMsg(Exception ex, string errorMsg)
		{
			Logging.Error(errorMsg + openFileDialog.FileName + " " + ex.InnerException);
			var messageBox = new MessageBoxForm("Sorry, we didn't recognize the data in that file.", "Invalid file", MessageBoxButtons.OK, SystemIcons.Error);
			messageBox.ShowDialog();
		}

		#endregion

		#region All Library Contex Menus

		private void toolStripMenuItemNewItem_Click(object sender, EventArgs e)
		{
			ToolStripMenu(false, null);
		}

		private void ToolStripMenu(bool dragDrop, string newButtonName)
		{
			ToolStripButton tsb = new ToolStripButton();
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					using (ColorPicker cp = new ColorPicker())
					{
						cp.LockValue_V = false;
						cp.Color = XYZ.FromRGB(Color.White);
						DialogResult result = cp.ShowDialog();
						if (result != DialogResult.OK) return;
						Color colorValue = cp.Color.ToRGB().ToArgb();

						_colors.Add(colorValue);
						tsb = new ToolStripButton
						{
							ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorValue.R, colorValue.G, colorValue.B),
							Tag = colorValue,
							Image = CreateColorListItem(colorValue)
						};

						toolStripColorLibrary.Items.Add(tsb);

						UpdateColorSettings();
					}
					break;
				case "toolStripCurveLibrary":
					if (!dragDrop) newButtonName = AddCurveToLibrary(new Curve());
					
					foreach (ToolStripItem item in toolStripCurveLibrary.Items)
					{
						if (item.Name == newButtonName)
						{
							Populate_Curves();
							return;
						}
					}

					foreach (KeyValuePair<string, Curve> kvp in _curveLibrary)
					{
						if (kvp.Key == newButtonName)
						{
							var curveImage = kvp.Value.GenerateGenericCurveImage(new Size(_iconSize, _iconSize));

							curveImage = DrawButtonBorder(curveImage);
							string curveName = kvp.Key;

							tsb = new ToolStripButton
							{
								ToolTipText = curveName,
								Tag = kvp.Value,
								Name = curveName,
								ImageKey = curveName,
								Image = curveImage
							};

							toolStripCurveLibrary.Items.Add(tsb);
						}
					}

					break;
				case "toolStripGradientLibrary":
					if (!dragDrop) newButtonName = AddGradientToLibrary(new ColorGradient());

					foreach (ToolStripItem item in toolStripGradientLibrary.Items)
					{
						if (item.Name == newButtonName)
						{
							Populate_Gradients();
							return;
						}
					}

					foreach (KeyValuePair<string, ColorGradient> kvp in _colorGradientLibrary)
					{
						if (kvp.Key == newButtonName)
						{

							var gradientImage =
								new Bitmap(kvp.Value.GenerateColorGradientImage(new Size(_iconSize, _iconSize), false),
									_iconSize,
									_iconSize);
							gradientImage = DrawButtonBorder(gradientImage);
							string name = kvp.Key;

							tsb = new ToolStripButton
							{
								ToolTipText = name,
								Tag = kvp.Value,
								Name = name,
								ImageKey = name,
								Image = gradientImage
							};

							toolStripGradientLibrary.Items.Add(tsb);
						}
					}
					break;
			}

			tsb.MouseDown += toolStripLibraries_MouseDown;
			tsb.MouseEnter += toolStripLibraries_MouseEnter;
			tsb.MouseLeave += toolStripLibraries_MouseLeave;
		}

		private void toolStripMenuItemEditItem_Click(object sender, EventArgs e)
		{
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					using (ColorPicker cp = new ColorPicker())
					{
						cp.LockValue_V = false;
						cp.Color = XYZ.FromRGB((Color)_selectedButton.Tag);
						DialogResult result = cp.ShowDialog();
						if (result != DialogResult.OK) return;
						Color colorValue = cp.Color.ToRGB().ToArgb();

						_selectedButton.ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorValue.R, colorValue.G, colorValue.B);
						_selectedButton.ImageKey = colorValue.ToString();
						_selectedButton.Tag = colorValue;
						_selectedButton.Image = CreateColorListItem(colorValue);

						UpdateColorSettings();
					}
					break;
				case "toolStripCurveLibrary":
					if (_curveLibrary.EditLibraryCurve(_selectedButton.Name))
					{
						var curve = _curveLibrary.GetCurve(_selectedButton.Name); //Get the edited curve from the library.
						if (curve != null)
						{
							var curveImage = curve.GenerateGenericCurveImage(new Size(_iconSize - 1, _iconSize - 1));
							curveImage = DrawButtonBorder(curveImage);
							_selectedButton.Tag = curve;
							_selectedButton.Image = curveImage;
						}
					}
					break;
				case "toolStripGradientLibrary":
					if (_colorGradientLibrary.EditLibraryItem(_selectedButton.Name))
					{
						ColorGradient gradient = _colorGradientLibrary.GetColorGradient(_selectedButton.Name); //Get the edited curve from the library
						if (gradient != null)
						{
							var gradientImage = new Bitmap(gradient.GenerateColorGradientImage(new Size(_iconSize - 1, _iconSize - 1), false),
								_iconSize,
								_iconSize);

							gradientImage = DrawButtonBorder(gradientImage);
							_selectedButton.Tag = gradient;
							_selectedButton.Image = gradientImage;
						}
					}
					break;
			}
		}

		private void toolStripMenuItemDeleteItem_Click(object sender, EventArgs e)
		{
			List<ToolStripButton> removeButtons = new List<ToolStripButton>();
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					foreach (ToolStripButton tsb in toolStripColorLibrary.Items)
					{
						if (tsb.Checked)
						{
							_colors.Remove((Color)tsb.Tag);
							removeButtons.Add(tsb);
						}
					}
					foreach (ToolStripButton tsb in removeButtons)
					{
						toolStripColorLibrary.Items.Remove(tsb);
					}

					UpdateColorSettings();
					break;
				case "toolStripCurveLibrary":
					_curveLibrary.BeginBulkUpdate();
					foreach (ToolStripButton tsb in toolStripCurveLibrary.Items)
					{
						if (tsb.Checked)
						{
							_curveLibrary.RemoveCurve(tsb.Name);
							removeButtons.Add(tsb);
						}
					}
					foreach (ToolStripButton tsb in removeButtons)
					{
						toolStripCurveLibrary.Items.Remove(tsb);
					}
					_curveLibrary.EndBulkUpdate();
					break;
				case "toolStripGradientLibrary":
					_colorGradientLibrary.BeginBulkUpdate();
					foreach (ToolStripButton tsb in toolStripGradientLibrary.Items)
					{
						if (tsb.Checked)
						{
							_colorGradientLibrary.RemoveColorGradient(tsb.Name);
							removeButtons.Add(tsb);
						}
					}
					foreach (ToolStripButton tsb in removeButtons)
					{
						toolStripGradientLibrary.Items.Remove(tsb);
					}
					_colorGradientLibrary.EndBulkUpdate();
					break;
			}
		}
		
		private void imageSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ChangeToolStripImageSize(_contextToolStrip, _iconSize);
		}

		private void ChangeToolStripImageSize(ToolStrip toolStrip, int imageSize)
		{
			TextImageRelation imagePosition;
			if (toolStrip.ImageScalingSize.Height != imageSize)
			{
				toolStrip.ImageScalingSize = new Size(_iconSize, _iconSize);
				imagePosition = TextImageRelation.ImageAboveText;
			}
			else
			{
				toolStrip.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
				imagePosition = TextImageRelation.TextBeforeImage;
			}

			toolStrip.SuspendLayout();
			foreach (ToolStripItem tsi in toolStrip.Items) tsi.TextImageRelation = imagePosition;
			toolStrip.ResumeLayout();
		}

		private Bitmap DrawButtonBorder(Bitmap image)
		{
			using (var p = new Pen(ThemeColorTable.BorderColor, 2))
			{
				Graphics gfx = Graphics.FromImage(image);
				gfx.DrawRectangle(p, 0, 0, image.Width - 1, image.Height - 1);
				gfx.Dispose();
				return image;
			}
		}

		#endregion

		#region Color Library Toolstrip Initialization

		private void Populate_Colors(object sender, EventArgs e)
		{
			_currentToolStrip = null;
			PopulateColorLibraryToolStrip_Context();
		}

		private void PopulateColorLibraryToolStrip_Context()
		{
			if (File.Exists(_colorFilePath))
			{
				Load_ColorPaletteFile();
				PopulateColors();
			}
		}

		public void PopulateColors()
		{
			if (_currentToolStrip == null)
			{
				toolStripColorLibrary.SuspendLayout();
				toolStripColorLibrary.Items.Clear();
				foreach (Color colorItem in _colors)
				{
					ToolStripButton tsb = new ToolStripButton
					{
						ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorItem.R, colorItem.G, colorItem.B),
						Tag = colorItem,
						Image = CreateColorListItem(colorItem),
						TextImageRelation = TextImageRelation.ImageAboveText
					};

					tsb.MouseDown += toolStripLibraries_MouseDown;
					tsb.MouseEnter += toolStripLibraries_MouseEnter;
					tsb.MouseLeave += toolStripLibraries_MouseLeave;
					toolStripColorLibrary.Items.Add(tsb);
				}
				toolStripColorLibrary.ResumeLayout();
			}
		}

		public void Load_ColorPaletteFile()
		{
			if (File.Exists(_colorFilePath))
			{
				using (FileStream reader = new FileStream(_colorFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<Color>));
					_colors = (List<Color>)ser.ReadObject(reader);
				}
			}
		}

		public void Save_ColorPaletteFile()
		{
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<Color>));
			var dataWriter = XmlWriter.Create(_colorFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, _colors);
			dataWriter.Close();
		}

		private Bitmap CreateColorListItem(Color colorItem)
		{
			Bitmap result = new Bitmap(_iconSize, _iconSize);
			Graphics gfx = Graphics.FromImage(result);
			using (SolidBrush brush = new SolidBrush(colorItem))
			{
				using (var p = new Pen(ThemeColorTable.BorderColor, 2))
				{
					gfx.FillRectangle(brush, 0, 0, _iconSize, _iconSize);
					gfx.DrawRectangle(p, 0, 0, _iconSize, _iconSize);
				}
			}
			gfx.Dispose();
			return result;
		}

		private void Update_ColorOrder()
		{
			_colors.Clear();
			foreach (ToolStripButton tsb in toolStripColorLibrary.Items) _colors.Add((Color)tsb.Tag);
			Save_ColorPaletteFile();
		}

		private void UpdateColorSettings()
		{
			Update_ColorOrder();

			if (_colorLibraryForm != null && _colors != null)
			{
				ColorLibraryForm.Load_ColorPaletteFile();
				ColorLibraryForm.PopulateColors();
			}
		}

		public event EventHandler SelectionChanged
		{
			add { ColorLibraryForm.SelectionChanged += value; }
			remove { if (ColorLibraryForm != null) ColorLibraryForm.SelectionChanged -= value; }
		}

		#endregion

		#region Color Library Toolstrip Enter and Drop

		private void toolStripColorLibrary_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(Color)) && _itemMove)
			{
				// Move a toolstrip item within the same toolstrip.
				_dragValid = true;
				e.Effect = DragDropEffects.Move;
				return;
			}

			if (e.Data.GetDataPresent(typeof(Color)))
			{
				_dragValid = true;
				Color col = (Color)e.Data.GetData(typeof(Color));
				if (_selectedButton != null) _selectedButton.Tag = col;
				e.Effect = DragDropEffects.Copy;
				return;
			}

			_dragValid = false;
			e.Effect = DragDropEffects.None;
		}

		private void toolStripColorLibrary_DragDrop(object sender, DragEventArgs e)
		{
			ToolStrip selectedToolStrip = sender as ToolStrip;
			if (selectedToolStrip != null) _contextToolStrip = selectedToolStrip;
			if (e.Effect == DragDropEffects.Copy)
			{
				Color colorItem = (Color)e.Data.GetData(typeof(Color));

				ToolStripButton tsb = new ToolStripButton
				{
					ToolTipText = string.Format("R: {0} G: {1} B: {2}", colorItem.R, colorItem.G, colorItem.B),
					Tag = colorItem,
					Image = CreateColorListItem(colorItem)
				};

				tsb.MouseDown += toolStripLibraries_MouseDown;
				tsb.MouseEnter += toolStripLibraries_MouseEnter;
				tsb.MouseLeave += toolStripLibraries_MouseLeave;
				toolStripColorLibrary.Items.Add(tsb);

				UpdateColorSettings();
			}
			else if (e.Effect == DragDropEffects.Move)
			{
				// Move a toolstrip item within the same toolstrip.
				ToolStripItem_Move(e);
			}
		}

		#endregion

		#region Curve Library Toolstrip Initialization

		private void Populate_Curves(object sender, EventArgs e)
		{
			_currentToolStrip = null;
			PopulateCurveLibraryToolStrip_Context();
		}

		private void PopulateCurveLibraryToolStrip_Context()
		{
			if (_curveLibrary != null) Populate_Curves();
		}

		private void Populate_Curves()
		{
			if (_currentToolStrip == null)
			{
				toolStripCurveLibrary.SuspendLayout();
				toolStripCurveLibrary.Items.Clear();
				using (var p = new Pen(ThemeColorTable.BorderColor, 2))
				{
					foreach (KeyValuePair<string, Curve> kvp in _curveLibrary)
					{
						Curve c = kvp.Value;
						string name = kvp.Key;

						var result = c.GenerateGenericCurveImage(new Size(_iconSize, _iconSize));
						Graphics gfx = Graphics.FromImage(result);
						gfx.DrawRectangle(p, 0, 0, result.Width - 1, result.Height - 1);
						gfx.Dispose();

						ToolStripButton tsb = new ToolStripButton
						{
							ToolTipText = name,
							Tag = c,
							Name = name,
							ImageKey = name,
							Image = result,
							TextImageRelation = TextImageRelation.ImageAboveText
						};

						tsb.MouseDown += toolStripLibraries_MouseDown;
						tsb.MouseEnter += toolStripLibraries_MouseEnter;
						tsb.MouseLeave += toolStripLibraries_MouseLeave;
						toolStripCurveLibrary.Items.Add(tsb);
					}
				}

				foreach (ToolStripItem item in toolStripCurveLibrary.Items)
				{
					foreach (AllToolStripItems it in _allToolStripItems)
					{
						if (item.ToolTipText == it.ItemName)
						{
							item.Visible = it.Visible;
							break;
						}
					}

				}
				toolStripCurveLibrary.ResumeLayout();
			}
		}

		private string AddCurveToLibrary(Curve c, bool edit = true)
		{
			TextDialog dialog = new TextDialog("Curve name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					var messageBox = new MessageBoxForm("Please enter a name.", "Curve Name", MessageBoxButtons.OK, SystemIcons.Warning);
					messageBox.ShowDialog();
					continue;
				}

				if (_curveLibrary.Contains(dialog.Response))
				{
					var messageBox = new MessageBoxForm("There is already a curve with that name. Do you want to overwrite it?", "Overwrite curve?", MessageBoxButtons.YesNoCancel, SystemIcons.Error);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						if (edit) _curveLibrary.EditLibraryCurve(dialog.Response);
						break;
					}

					if (messageBox.DialogResult == DialogResult.Cancel) break;
				}
				else
				{
					_curveLibrary.AddCurve(dialog.Response, c);
					if (edit) _curveLibrary.EditLibraryCurve(dialog.Response);
					break;
				}
			}
			return dialog.Response;
		}

		#endregion

		#region Curve Library Toolstrip Enter and Drop

		private void toolStripCurveLibrary_DragDrop(object sender, DragEventArgs e)
		{
			ToolStrip selectedToolStrip = sender as ToolStrip;
			if (selectedToolStrip != null) _contextToolStrip = selectedToolStrip;
			if (e.Effect == DragDropEffects.Copy)
			{
				// Copy the Curve to an Effect.
				_currentToolStrip = null;
				Curve c = (Curve)e.Data.GetData(typeof(Curve));
				string newButtonName = AddCurveToLibrary(c, false);

				ToolStripMenu(true, newButtonName);
				_currentToolStrip = selectedToolStrip;
			}
			else if (e.Effect == DragDropEffects.Move)
			{
				// Move a toolstrip item within the same toolstrip.
				ToolStripItem_Move(e);
			}
		}

		private void toolStripCurveLibrary_DragEnter(object sender, DragEventArgs e)
		{
			_contextToolStrip = sender as ToolStrip;

			if (e.Data.GetDataPresent(typeof(Curve)) && _itemMove)
			{
				// Move a toolstrip item within the same toolstrip.
				_dragValid = true;
				e.Effect = DragDropEffects.Move;
				return;
			}
			if (e.Data.GetDataPresent(typeof(Curve)))
			{
				// Copy the Curve to an Effect.
				Curve c = (Curve)e.Data.GetData(typeof(Curve));
				if (!c.IsLibraryReference)
				{
					_dragValid = true;
					if (_selectedButton != null) _selectedButton.Tag = c;
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}

			_dragValid = false;
			e.Effect = DragDropEffects.None;
		}

		#endregion

		#region Gradient Library Toolstrip Initialization

		private void Populate_Gradients(object sender, EventArgs e)
		{
			_currentToolStrip = null;
			PopulateGradientLibraryToolStrip_Context();
		}

		private void PopulateGradientLibraryToolStrip_Context()
		{
			if (_colorGradientLibrary != null) Populate_Gradients();
		}

		private void Populate_Gradients()
		{
			if (_currentToolStrip == null)
			{
				toolStripGradientLibrary.SuspendLayout();
				toolStripGradientLibrary.Items.Clear();
				using (var p = new Pen(ThemeColorTable.BorderColor, 2))
				{
					foreach (KeyValuePair<string, ColorGradient> kvp in _colorGradientLibrary)
					{
						ColorGradient gradient = kvp.Value;
						string name = kvp.Key;

						var result = new Bitmap(gradient.GenerateColorGradientImage(new Size(_iconSize, _iconSize), false), _iconSize, _iconSize);
						Graphics gfx = Graphics.FromImage(result);
						gfx.DrawRectangle(p, 0, 0, _iconSize, _iconSize);
						gfx.Dispose();
						
						ToolStripButton tsb = new ToolStripButton
						{
							ToolTipText = name,
							Tag = gradient,
							Name = name,
							ImageKey = name,
							Image = result,
							TextImageRelation = TextImageRelation.ImageAboveText
						};

						tsb.MouseDown += toolStripLibraries_MouseDown;
						tsb.MouseEnter += toolStripLibraries_MouseEnter;
						tsb.MouseLeave += toolStripLibraries_MouseLeave;
						toolStripGradientLibrary.Items.Add(tsb);
					}
				}

				foreach (ToolStripItem item in toolStripGradientLibrary.Items)
				{
					foreach (AllToolStripItems it in _allToolStripItems)
					{
						if (item.ToolTipText == it.ItemName)
						{
							item.Visible = it.Visible;
							break;
						}
					}

				}
				toolStripGradientLibrary.ResumeLayout();
			}
		}

		private string AddGradientToLibrary(ColorGradient cg, bool edit = true)
		{
			TextDialog dialog = new TextDialog("Gradient name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					var messageBox = new MessageBoxForm("Please enter a name.", "Gradient Name", MessageBoxButtons.OK, SystemIcons.Warning);
					messageBox.ShowDialog();
					continue;
				}

				if (_colorGradientLibrary.Contains(dialog.Response))
				{
					var messageBox = new MessageBoxForm("There is already a Gradient with that name. Do you want to overwrite it?", "Overwrite Gradient?", MessageBoxButtons.YesNoCancel, SystemIcons.Error);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						if (edit) _colorGradientLibrary.EditLibraryItem(dialog.Response);
						break;
					}

					if (messageBox.DialogResult == DialogResult.Cancel) break;
				}
				else
				{
					_colorGradientLibrary.AddColorGradient(dialog.Response, cg);
					if (edit) _colorGradientLibrary.EditLibraryItem(dialog.Response);
					break;
				}
			}
			return dialog.Response;
		}

		#endregion

		#region Gradient Library Toolstrip Enter and Drop

		private void toolStripGradientLibrary_DragDrop(object sender, DragEventArgs e)
		{
			ToolStrip selectedToolStrip = sender as ToolStrip;
			if (selectedToolStrip != null) _contextToolStrip = selectedToolStrip;
			if (e.Effect == DragDropEffects.Copy)
			{
				// Copy the Gradient to an Effect.
				_currentToolStrip = null;
				ColorGradient c = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
				string newButtonName = AddGradientToLibrary(c, false);

				ToolStripMenu(true, newButtonName);
				_currentToolStrip = selectedToolStrip;
			}
			else if (e.Effect == DragDropEffects.Move)
			{
				// Move a toolstrip item within the same toolstrip.
				ToolStripItem_Move(e);
			}
		}

		private void toolStripGradientLibrary_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ColorGradient)) && _itemMove)
			{
				// Move a toolstrip item within the same toolstrip.
				e.Effect = DragDropEffects.Move;
				return;
			}
			if (e.Data.GetDataPresent(typeof(ColorGradient)))
			{
				ColorGradient cg = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
				if (!cg.IsLibraryReference)
				{
					if (_selectedButton != null) _selectedButton.Tag = cg;
					_dragValid = true;
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}

			_dragValid = false;
			e.Effect = DragDropEffects.None;
		}

		#endregion

		#endregion

		#region Effect ToolStrip Initialization

		private void resetEffectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Resets all Effect Context menu items to checked.
			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				dropDownItem.Checked = true;
				foreach (ToolStripMenuItem groupDropDownItem in dropDownItem.DropDownItems) groupDropDownItem.Checked = true;
			}

			// Resets all Effect Toolstrip Items to visible.
			foreach (ToolStripItem tsi in toolStripEffects.Items)tsi.Visible = true;
		}

		private void PopulateEffectsToolStrip_Context()
		{
			foreach (ToolStripMenuItem groupItems in effectGroupsToolStripMenuItem.DropDownItems) groupItems.DropDownItems.Clear();
			
			foreach (
				IEffectModuleDescriptor effectDescriptor in
				ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				if (effectDescriptor.EffectName == "Nutcracker") continue;
				switch (effectDescriptor.EffectGroup)
				{
					case EffectGroups.Basic:
						AddEffect(effectDescriptor, basicToolStripMenuItem);
						break;
					case EffectGroups.Pixel:
						AddEffect(effectDescriptor, pixelToolStripMenuItem);
						break;
					case EffectGroups.Device:
						AddEffect(effectDescriptor, deviceToolStripMenuItem);
						break;
				}
			}

			toolStripEffects.SuspendLayout();
			toolStripEffects.Items.Clear();
			foreach (ToolStripMenuItem groupItems in effectGroupsToolStripMenuItem.DropDownItems)
			{
				foreach (ToolStripMenuItem items in groupItems.DropDownItems)
				{
					toolStripEffects.Items.Add((ToolStripItem) items.Tag);
				}
			}
			AlignText();
			toolStripEffects.ResumeLayout();
		}

		private void AddEffect(IEffectModuleDescriptor effectDescriptor, ToolStripMenuItem toolStripMenuItem)
		{
			ToolStripButton tsb = new ToolStripButton
			{
				ToolTipText = effectDescriptor.EffectName,
				Tag = effectDescriptor.TypeId,
				ImageIndex = toolStripEffects.Items.Count - 1,
				ImageKey = effectDescriptor.EffectName,
				Image = effectDescriptor.GetRepresentativeImage(),
				Text = effectDescriptor.EffectName,
				Visible = true,
			};
			tsb.MouseDown += tsb_MouseDown;
			tsb.MouseMove += tsb_MouseMove;

			ToolStripMenuItem tsmi = new ToolStripMenuItem();
			tsmi.Text = effectDescriptor.EffectName;
			tsmi.CheckState = CheckState.Checked;
			tsmi.Checked = true;
			tsmi.CheckOnClick = true;
			tsmi.Image = tsb.Image;
			tsmi.Click += ToolStripEffectItem_Changed;
			foreach (AllToolStripItems it in _allToolStripItems)
			{
				if (tsmi.Text == it.ItemName)
				{
					tsmi.Checked = it.Visible;
					if (!toolStripMenuItem.Checked) tsb.Visible = false;
					else if (!tsmi.Checked) tsb.Visible = false;
					break;
				}
			}
			tsmi.Tag = tsb;
			toolStripMenuItem.DropDownItems.Add(tsmi);
		}

		private void SelectNodeForDrawing(ToolStripButton selectedButton)
		{
			// This is for when Drawing effects is enabled.

			if (selectedButton.Checked) // If button is already selected then deselect and remove GUID from grid selected effect.
			{
				TimelineControl.grid.SelectedEffect = Guid.Empty;
				selectedButton.Checked = false;
			}
			else
			{
				TimelineControl.grid.SelectedEffect = (Guid)_mToolStripEffects.Tag;
				foreach (ToolStripItem tsi in toolStripEffects.Items)
				{
					if (tsi != null && tsi.CanSelect) if (tsi is ToolStripButton tsb) tsb.Checked = false;
				}
				// Ensures the one selected to be drawn is Checked.
				selectedButton.Checked = true;
			}
		}

		private void AlignText()
		{
			//Adjusts location of the effect label.
			TextImageRelation alignText = TextImageRelation.ImageAboveText;
			var newFontSize = new Font(Font.FontFamily.Name, Font.Size-2, Font.Style);
			bool noText = false;
			int effectLabelPosition = 2;
			foreach (ToolStripMenuItem subItem in toolStripMenuItemLabelPosition.DropDown.Items)
			{
				if (subItem.Checked)
				{
					effectLabelPosition = Convert.ToInt16(subItem.Tag);
					break;
				}
			}
			
			switch (effectLabelPosition)
			{
				case 0:
					noText = true;
					break;
				case 1:
					alignText = TextImageRelation.TextAboveImage;
					break;
				case 2:
					alignText = TextImageRelation.ImageAboveText;
					break;
			}

			foreach (ToolStripItem tsi in toolStripEffects.Items)
			{
				if (!noText)
				{
					tsi.TextImageRelation = alignText;
					tsi.Text = tsi.ToolTipText;
				}
				else tsi.Text = "";
				tsi.Font = newFontSize;
			} 
		}

		#endregion

		#region Effect Toolstrip Events

		private void contextMenuStripEffect_Opening(object sender, CancelEventArgs e)
		{
			// Add Toolbar list to Context menu
			PopulateContextToolBars(toolbarsToolStripMenuItem_Effect);
		}

		// Show/Hide selected Toolstrip item.
		private void ToolStripEffectItem_Changed(object sender, EventArgs e)
		{
			ToolStripMenuItem tsi = sender as ToolStripMenuItem;
			ToolStripMenuItem tso = tsi.OwnerItem as ToolStripMenuItem;
			tso.Checked = true;

			foreach (ToolStripItem item in toolStripEffects.Items)
			{
				if (item.ToolTipText != null && item.ToolTipText == tsi.Text ||
				    item.Tag != null && item.Tag.ToString() == tsi.Text)
				{
					item.Visible = tsi.Checked;
					break;
				}
			}

			toolStripMenuItemEffectGroup_Change(tso);
		}

		private void tsb_MouseDown(object sender, MouseEventArgs e)
		{
			_mToolStripEffects = sender as ToolStripItem;
			ToolStripButton selectedButton = sender as ToolStripButton;

			if (selectedButton == null) return;

			_beginNewEffectDragDrop =
				(_mToolStripEffects != null) &&
				(e.Button == MouseButtons.Left && e.Clicks == 1);

			if (modeToolStripButton_DrawMode.Checked) SelectNodeForDrawing(selectedButton);
			else selectedButton.Checked = false;
		}

		private void tsb_MouseMove(object sender, MouseEventArgs e)
		{
			ToolStripButton item = sender as ToolStripButton;
			if (e.Button == MouseButtons.Left && _beginNewEffectDragDrop)
			{
				_beginNewEffectDragDrop = false;
				DataObject data = new DataObject(_mToolStripEffects.Tag);
				item?.Owner.DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void toolStripMenuItemLabelPosition_Click(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem item && !item.Checked)
			{
				foreach (ToolStripMenuItem subItem in item.Owner.Items)
				{
					if (!item.Equals(subItem) && subItem != null) subItem.Checked = false;
				}
				item.Checked = true;
			}

			toolStripEffects.SuspendLayout();
			AlignText();
			toolStripEffects.ResumeLayout();
		}

		private void toolStripMenuItemEffectGroup_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			toolStripMenuItemEffectGroup_Change(menuItem);
		}

		private void toolStripMenuItemEffectGroup_Change(ToolStripMenuItem menuItem)
		{
			foreach (ToolStripMenuItem ddi in menuItem.DropDownItems)
			{
				foreach (ToolStripItem item in toolStripEffects.Items)
				{
					if (item.ToolTipText == ddi.Text) item.Visible = menuItem.Checked && ddi.Checked;
				}
			}
		}

		#endregion

		#region Playback Toolstrip Events

		private void toolStripButton_Start_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
			TimelineControl.PlaybackStartTime = _mPrevPlaybackStart = TimeSpan.Zero;
			TimelineControl.VisibleTimeStart = TimeSpan.Zero;
		}

		private void toolStripButton_Play_Click(object sender, EventArgs e)
		{
			PlaySequence();
		}

		private void toolStripButton_Stop_Click(object sender, EventArgs e)
		{
			StopSequence();
		}

		private void toolStripButton_Pause_Click(object sender, EventArgs e)
		{
			PauseSequence();
		}

		private void toolStripButton_End_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
			TimelineControl.PlaybackStartTime = _mPrevPlaybackEnd = _sequence.Length;
			TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;
		}

		private void toolStripButton_Loop_CheckedChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_Loop.Checked = playBackToolStripButton_Loop.Checked;
		}

		#endregion

		#region Audio Toolstrip Events
		
		private void audioToolStripLabel_TimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(1.0f);
		}

		private void audioToolStripLabel_TimingSpeed_DoubleClick(object sender, EventArgs e)
		{
			_SetTimingSpeed(4.0f);
		}

		private Bitmap SpeedVisualisation()
		{
			// Set Color based on Speed value.
			HSBColor mainColor = _timingSpeed >= 1 ? new HSBColor(120 - (int)(30.0f / 4 * _timingSpeed * 4), 255, 255) : new HSBColor((int)(10.0f / 4.0f * _timingSpeed * 60) - 20, 255, 255);
			
			// Set bitmap size. This is the same as the standard icons that we bring in as the Toolstrip wil autosize based on scaling, so 32 is a good size.
			int bitmapSize = 32;

			// Set Colors to use.
			Pen line = new Pen(ThemeColorTable.ForeColor, 3);
			Pen arch = new Pen(mainColor, 5);
			SolidBrush brush = new SolidBrush(mainColor);

			// Create points that define the line start and end location.
			int needleLength = 19;
			double needleAngle = (Math.PI / 180) * (350 - (160 - (int)(40.0f / 4.0f * _timingSpeed * 4)));
			Point point1 = new Point(bitmapSize / 2, bitmapSize);
			Point point2 = new Point((int)(bitmapSize / 2 + Math.Cos(needleAngle) * needleLength), (int)(bitmapSize + Math.Sin(needleAngle) * needleLength));
			
			// Create Bitmap and draw the image.
			Bitmap bmp = new Bitmap(bitmapSize, bitmapSize);
			using (var graphics = Graphics.FromImage(bmp))
			{
				// Draw Speed value.
				using (Font arialFont = new Font("Arial", 10, FontStyle.Bold))
				{
					int speedValueXLocation = _timingSpeed >= 0.97f ? 3 : 6; // Adjust the start location of the Speed text if over 100 as the text is longer then values under 100.
					graphics.DrawString(((float)Math.Round(_timingSpeed, 1) * 100).ToString(), arialFont, brush,
						new PointF(speedValueXLocation, 0));
				}

				// Draw arc.
				Rectangle rect = new Rectangle(3, bitmapSize / 2, bitmapSize - 6, bitmapSize - 3);
				float startAngle = 180.0F;
				float sweepAngle = 180.0F;
				graphics.DrawArc(arch, rect, startAngle, sweepAngle);

				// Draw line.
				graphics.DrawLine(line, point1, point2);
			}

			line.Dispose();
			arch.Dispose();
			brush.Dispose();
			return bmp;
		}

		private void toolStripAudio_MouseWheel(object sender, MouseEventArgs e)
		{
			// The Mouse wheel event is only on the Toolstrip so we need to go through each visible button and check if mouse is on top of
			// the Timing Button and if it is then change the Timing Speed.
			ToolStrip tsmi = sender as ToolStrip;
			tsmi.SuspendLayout();
			
			int toolStripItemXLocation = tsmi.DisplayRectangle.X;
			foreach (ToolStripItem item in tsmi.Items)
			{
				if (toolStripItemXLocation - 5 < e.X && toolStripItemXLocation + item.Size.Width + 5 > e.X && item.Name == "audioToolStripLabel_TimingSpeed")
				{
					if (e.Delta > 0)
					{
						_SetTimingSpeed((float)Math.Round(_timingSpeed, 1) + _timingChangeDelta);
					}
					else
					{
						_SetTimingSpeed((float)Math.Round(_timingSpeed, 1) - _timingChangeDelta);
					}

					break;
				}

				if (item.Visible) toolStripItemXLocation += item.Size.Width;
			}
			tsmi.ResumeLayout();
		}

		private void audioToolStripLabel_TimingSpeed_MouseLeave(object sender, EventArgs e)
		{
			TimelineControl.Focus();
		}

		private void audioToolStripLabel_TimingSpeed_MouseEnter(object sender, EventArgs e)
		{
			toolStripAudio.Focus();
		}

		private void toolStripButton_IncreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed + _timingChangeDelta);
		}

		private void toolStripButton_DecreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed - _timingChangeDelta);
		}
		private void audioDevicesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripDropDownItem menuItem = sender as ToolStripDropDownItem;
			foreach (ToolStripDropDownItem item in audioToolStripButton_Audio_Devices.DropDownItems) ((ToolStripMenuItem) item).Checked = item == menuItem;
			if (menuItem != null) Variables.SelectedAudioDeviceIndex = (int) menuItem.Tag;
		}

		#endregion

		#region Alignment Toolstrip Events

		private void toolStripButtonCloseGapStrength_MenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && !item.Checked)
			{
				foreach (ToolStripMenuItem subItem in item.Owner.Items)
				{
					if (!item.Equals(subItem) && subItem != null) subItem.Checked = false;
				}
				item.Checked = true;
				TimelineControl.grid.CloseGap_Threshold = item.Tag.ToString();
			}
		}

		private void toolStripDropDownButtonAlignToStrength_MenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && !item.Checked)
			{
				foreach (ToolStripMenuItem subItem in item.Owner.Items)
				{
					if (!item.Equals(subItem) && subItem != null) subItem.Checked = false;
				}
				item.Checked = true;
				AlignTo_Threshold = item.ToString();
			}
		}

		private void alignmentToolStripButton_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("Start");
		}

		private void alignmentToolStripButton_End_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("End");
		}

		private void alignmentToolStripButton_Both_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("Both");
		}

		private void alignmentToolStripButton_Distribute_Click(object sender, EventArgs e)
		{
			DistributeSelectedEffectsEqually();
		}

		private void alignmentToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			toolStripAlignment.Visible = !toolStripAlignment.Visible;
		}

		#endregion

		#region File Toolstrip Events
		
		private void fileToolStripButton_AutoSave_Click(object sender, EventArgs e)
		{
			fileToolStripButton_AutoSave.Checked = !fileToolStripButton_AutoSave.Checked;
			autoSaveToolStripMenuItem.Checked = fileToolStripButton_AutoSave.Checked;
			SetAutoSave();
		}

		#endregion

		#region Mode Toolstrip Events
		
		private void toolStripButton_DrawMode_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.EnableDrawMode = true;
			modeToolStripButton_DrawMode.Checked = true;
			modeToolStripButton_SelectionMode.Checked = false;
		}

		private void toolStripButton_SelectionMode_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.EnableDrawMode = false;
			modeToolStripButton_SelectionMode.Checked = true;
			modeToolStripButton_DrawMode.Checked = false;

			// Ensure any Effect buttons in the toolstrip do not have the selected box around it.
			foreach (ToolStripItem effectButton in toolStripEffects.Items)
			{
				if (effectButton is ToolStripButton) ((ToolStripButton)effectButton).Checked = false;
			}
		}

		private void toolStripButton_DragBoxFilter_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.DragBoxFilterEnabled = modeToolStripButton_DragBoxFilter.Checked;
		}

		private void toolStripSplitButton_CloseGaps_ButtonClick(object sender, EventArgs e)
		{
			TimelineControl.grid.CloseGapsBetweenElements();
		}

		private void toolStripButton_SnapTo_CheckedChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_SnapTo.Checked = modeToolStripButton_SnapTo.Checked;
			TimelineControl.grid.EnableSnapTo = modeToolStripButton_SnapTo.Checked;
		}

		private void toolStripButtonSnapToStrength_MenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (item != null && !item.Checked)
			{
				foreach (ToolStripMenuItem subItem in item.Owner.Items)
				{
					if (!item.Equals(subItem) && subItem != null) subItem.Checked = false;
				}
				item.Checked = true;
				PopulateSnapStrength(Convert.ToInt32(item.Tag));
			}
			// clicking the currently checked one--do not uncheck it			
		}

		#endregion

		#region ToolStrip Helpers

		private void PopulateContextToolBarItems(ToolStripMenuItem tsm)
		{
			foreach (ToolStripItem tsi in _contextToolStrip.Items)
			{
				ToolStripMenuItem tsmi = new ToolStripMenuItem();
				tsmi.Text = tsi.ToolTipText;
				tsmi.CheckOnClick = true;
				tsmi.CheckState = CheckState.Checked;
				tsmi.Click += ToolStripAllItem_Changed;
				tsmi.Checked = tsi.Visible;
				if (tsi.DisplayStyle != ToolStripItemDisplayStyle.None) tsmi.Image = tsi.Image;
				tsm.DropDownItems.Add(tsmi);
			}
		}

		private void PopulateContextToolBars(ToolStripMenuItem currentContextMenu)
		{
			// Add Toolbar list to Context menu
			currentContextMenu.DropDownItems.Clear();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					tsmi.Visible = true;
					tsmi.Text = toolsStripItems.Text;
					tsmi.CheckOnClick = true;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Click += ToolStripItem_Changed;
					tsmi.Checked = toolsStripItems.Visible;
					currentContextMenu.DropDownItems.Add(tsmi);
				}
			}
			SortToolStripMenuItemAlphabetically(currentContextMenu);

		}

		private void SortToolStripMenuItemAlphabetically(ToolStripMenuItem tsmi)
		{
			ToolStripItem[] temp = new ToolStripItem[tsmi.DropDownItems.Count];
			tsmi.DropDownItems.CopyTo(temp, 0);
			Array.Sort(temp,
				delegate (ToolStripItem tsiA, ToolStripItem tsiB)
				{
					return tsiA.Text.Replace("&", string.Empty).CompareTo(tsiB.Text.Replace("&", string.Empty));
				});
			tsmi.DropDownItems.Clear();
			tsmi.DropDownItems.AddRange(temp);
		}

		private void toolStripMenuItem_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
			SetToolStripStartPosition();
		}

		private void contextMenuStripLibraries_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			_mouseDown = false;
			UpdateCurrentLibrary();
			_currentToolStrip = null;
			TimelineControl.Focus();
		}

		private void toolStrip_EndDrag(object sender, EventArgs e)
		{
			SetToolStripStartPosition();
		}

		private void SetToolStripStartPosition()
		{
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (var toolStrip in row.Controls) toolStrip.Location = new Point(3, toolStrip.Location.Y);
			}
		}

		public void Load_ToolsStripItemsFile()
		{
			_allToolStripItems = new List<AllToolStripItems>();

			if (File.Exists(_toolStripFilePath))
			{
				using (FileStream reader = new FileStream(_toolStripFilePath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<AllToolStripItems>));
					_allToolStripItems = (List<AllToolStripItems>)ser.ReadObject(reader);
				}
			}
			else
			{
				PopulateToolStripClass(true);
			}
		}

		public void Save_ToolsStripItemsFile()
		{
			// Save Toolbar layout.
			int i = 0;
			XMLProfileSettings xml = new XMLProfileSettings();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (var toolsStrip in row.Controls)
				{
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripName{1}", Name, i), toolsStrip.Name);
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripVisible{1}", Name, i), toolsStrip.Visible);
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripX{1}", Name, i), toolsStrip.Location.X);
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripY{1}", Name, i), toolsStrip.Location.Y);
					i++;
				}
			}
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/ToolStripCount", Name), i);
			
			// Effect Toolstrip Label settings
			foreach (ToolStripMenuItem subItem in toolStripMenuItemLabelPosition.DropDown.Items)
			{
				if (subItem.Checked)
				{
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/EffectLabelIndex", Name),
						subItem.Tag.ToString());
					break;
				}
			}
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/BasicEffectToolStrip", Name),
				basicToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/PixelEffectToolStrip", Name),
				pixelToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/DeviceEffectToolStrip", Name),
				deviceToolStripMenuItem.Checked);

			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				foreach (ToolStripMenuItem groupDropDownItem in dropDownItem.DropDownItems)
				{
					xml.PutSetting(XMLProfileSettings.SettingType.AppSettings,
						string.Format("{0}/{1}/{2}", Name, dropDownItem.Text, groupDropDownItem.Text.Replace(" ", "")),
						groupDropDownItem.Checked);
					int ii = 0;
					foreach (ToolStripItem item in toolStripEffects.Items)
					{
						if (item.Text == groupDropDownItem.Text)
						{
							xml.PutSetting(XMLProfileSettings.SettingType.AppSettings,
								string.Format("{0}/{1}/{2}", Name, dropDownItem.Text, groupDropDownItem.Text.Replace(" ", "") + 1), ii);
							break;
						}
						ii++;
					}
				}
			}

			// Save ToolStrip Height
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/ColorLibrary", Name),
				toolStripColorLibrary.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/GradientLibrary", Name),
				toolStripGradientLibrary.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/CurveLibrary", Name),
				toolStripCurveLibrary.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/PlayBack", Name),
				toolStripPlayBack.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Edit", Name),
				toolStripEdit.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Audio", Name),
				toolStripAudio.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/View", Name),
				toolStripView.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Tools", Name),
				toolStripTools.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/File", Name),
				toolStripFile.ImageScalingSize.Height); 
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Mode", Name),
				toolStripMode.ImageScalingSize.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStrip/Alignment", Name),
				toolStripAlignment.ImageScalingSize.Height);

			// Save each Toolstrip settings
			_allToolStripItems.Clear();
			PopulateToolStripClass(false);
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<AllToolStripItems>));
			var dataWriter = XmlWriter.Create(_toolStripFilePath, xmlsettings);
			dataSer.WriteObject(dataWriter, _allToolStripItems);
			dataWriter.Close();
		}

		private void PopulateToolStripClass(bool initialLoad)
		{
			toolStripContainer.SuspendLayout();
			bool checkVisibility = false;
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					if (!toolsStripItems.Visible && !initialLoad)
					{
						toolsStripItems.Visible = true;
						checkVisibility = true;
					}
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						AllToolStripItems ts = new AllToolStripItems();
						ts.ItemName = item.ToolTipText;
						if (initialLoad)
						{
							ts.Visible = true;
						}
						else
						{
							switch (toolsStripItems.Name)
							{
								case "toolStripEffects":
									foreach (ToolStripMenuItem group in effectGroupsToolStripMenuItem.DropDownItems)
									{
										foreach (ToolStripMenuItem menuItem in group.DropDownItems)
										{
											if (ts.ItemName == menuItem.Text)
											{
												ts.Visible = menuItem.Checked;
												break;
											}
										}
									}
									break;
								default:
									ts.Visible = item.Visible;
									break;
							}

						}
						_allToolStripItems.Add(ts);
					}
					
					if (checkVisibility) toolsStripItems.Visible = false;
				}
			}
			toolStripContainer.ResumeLayout();
		}

		[Serializable]
		internal class AllToolStripItems
		{
			public string ItemName;
			public bool Visible;
		}

		#endregion

	}
}