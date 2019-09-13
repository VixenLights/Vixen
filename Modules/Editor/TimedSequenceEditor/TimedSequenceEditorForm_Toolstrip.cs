using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Catel.Collections;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.App;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;
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
		private ToolStripStates _toolStripStates;
		private readonly string _standardToolStripFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ToolStripState.xml");
		private readonly string _profileToolStripFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", $"{VixenSystem.ProfileName}_ToolStripState.xml");
		private bool _mouseDown;

		// Libraries
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
			editToolStripButton_PasteVisibleMarks.Image = Resources.paste_marks;
			editToolStripButton_PasteInvert.Image = Resources.paste_invert;
			editToolStripButton_PasteDropDown.Image = Resources.paste_special;
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
					if (toolsStripControl.Name == toolStripEffects.Name) continue;
					if (toolsStripControl is ToolStrip toolStrip)
					{
						if (_toolStripStates.States.TryGetValue(toolStrip.Name, out var toolStripStates))
						{
							foreach (ToolStripItem item in toolStrip.Items)
							{
								if (toolStripStates.TryGetValue(item.Name, out var visible))
								{
									item.Visible = visible;
								}
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
		private void ToolStripItemState_Changed(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem tsmi)
			{
				if (tsmi.Tag is ToolStripItem menuItem)
				{
					menuItem.Visible = tsmi.Checked;
					if (menuItem.Owner is ToolStrip ts)
					{
						if (_toolStripStates.States.TryGetValue(ts.Name, out var itemStates))
						{
							if (itemStates.ContainsKey(menuItem.Name))
							{
								itemStates[menuItem.Name] = tsmi.Checked;
							}
							else
							{
								itemStates.Add(menuItem.Name, tsmi.Checked);
							}
						}
					}
				}
			}

			SetToolStripStartPosition();
		}

		// Show/Hide selected ToolBar.
		private void ToolStripItem_Changed(object sender, EventArgs e)
		{
			if (sender is ToolStripMenuItem tsi)
			{
				foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
				{
					foreach (Control toolsStripControl in row.Controls)
					{
						if (toolsStripControl is ToolStrip toolStrip)
						{
							if (toolStrip.Text != null && toolStrip.Text == tsi.Text)
							{
								toolStrip.Visible = tsi.Checked;
								break;
							}
						}
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
			if (_toolStripStates.States.TryGetValue(ts.Name, out var itemStates))
			{
				foreach (ToolStripItem item in ts.Items)
				{
					item.Visible = true;
					itemStates[item.Name] = true;
				}
			}
			else
			{
				foreach (ToolStripItem item in ts.Items)
				{
					item.Visible = true;
				}
			}
		}

		#endregion

		#region Library Toolstrips

		#region All Library Toolstrips

		private void UpdateCurrentLibrary()
		{
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					UpdateColorLibrary();
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

		private void toolStripLibraryButton_MouseEnter(object sender, EventArgs e)
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

		private void toolStripLibraryButton_MouseLeave(object sender, EventArgs e)
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
			if (sender is ToolStrip ts)
			{
				foreach (ToolStripButton button in ts.Items)
				{
					button.Checked = false;
				}
			}
			_currentToolStrip = null;
			TimelineControl.Focus();
		}

		private Rectangle _dragBoxFromToolStripMouseDown;
		
		void toolStripLibraryButton_MouseDown(object sender, MouseEventArgs e)
		{
			_selectedButton = sender as ToolStripButton;
			if (_selectedButton == null)
			{
				_dragBoxFromToolStripMouseDown = Rectangle.Empty;
				return;
			}
			_contextToolStrip = _selectedButton.Owner;
			_contextToolStrip.Focus();
			if (e.Clicks == 2)
			{
				_dragBoxFromToolStripMouseDown = Rectangle.Empty;
				toolStripMenuItemEditItem_Click(sender, null);
				return;
			}
			if (e.Button == MouseButtons.Right)
			{
				_dragBoxFromToolStripMouseDown = Rectangle.Empty;
				_mouseDown = true;
			}
			else 
			{
				SelectNodeForDrawing();
				Size dragSize = SystemInformation.DragSize;
				_dragBoxFromToolStripMouseDown = new Rectangle(new Point(e.X - (dragSize.Width /2),
					e.Y - (dragSize.Height /2)), dragSize);
			}
		}

		private void toolStripLibraryButton_MouseUp(object sender, MouseEventArgs e)
		{
			_dragBoxFromToolStripMouseDown = Rectangle.Empty;
		}

		private void toolStripLibraryButton_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {

                // If the mouse moves outside the rectangle, start the drag.
                if (_dragBoxFromToolStripMouseDown != Rectangle.Empty && 
                    !_dragBoxFromToolStripMouseDown.Contains(e.X, e.Y)) {

					_itemMove = true;
                    _dragValid = true;
                    // Proceed with the drag-and-drop, passing in the button item.                    
                    _contextToolStrip.DoDragDrop(_selectedButton.Tag, DragDropEffects.Move);
                }
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
					_selectedButton.Owner?.DoDragDrop(_selectedButton.Tag, DragDropEffects.Copy);
					break;
				case "toolStripCurveLibrary":
					Curve newCrve = new Curve((Curve)_selectedButton.Tag);
					if (link) newCrve.LibraryReferenceName = _selectedButton.Name;
					newCrve.IsCurrentLibraryCurve = false;
					_selectedButton.Owner?.DoDragDrop(newCrve, DragDropEffects.Copy);
					break;
				case "toolStripGradientLibrary":
					ColorGradient newGradient = new ColorGradient((ColorGradient)_selectedButton.Tag);
					if (link) newGradient.LibraryReferenceName = _selectedButton.Name;
					newGradient.IsCurrentLibraryGradient = false;
					_selectedButton.Owner?.DoDragDrop(newGradient, DragDropEffects.Copy);
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
			//Just let the form library handle it.
			if (_contextToolStrip.Name == toolStripColorLibrary.Name)
			{
				ColorLibraryForm.ImportColorLibrary();
			}
			else if(_contextToolStrip.Name == toolStripCurveLibrary.Name)
			{
				CurveLibraryForm.ImportCurveLibrary();
			}
			else if (_contextToolStrip.Name == toolStripGradientLibrary.Name)
			{
				GradientLibraryForm.ImportGradientLibrary();
			}
			TimelineControl.Focus();
		}

		private void toolStripMenuItemExport_Click(object sender, EventArgs e)
		{
			//Just let the form library handle it.
			if (_contextToolStrip.Name == toolStripColorLibrary.Name)
			{
				ColorLibraryForm.ExportColorLibrary();
			}
			else if(_contextToolStrip.Name == toolStripCurveLibrary.Name)
			{
				CurveLibraryForm.ExportCurveLibrary();
			}
			else if (_contextToolStrip.Name == toolStripGradientLibrary.Name)
			{
				GradientLibraryForm.ExportGradientLibrary();
			}
			TimelineControl.Focus();
		}

		#endregion

		#region All Library Contex Menus

		private void toolStripMenuItemNewItem_Click(object sender, EventArgs e)
		{
			ToolStripMenu(false, null);
		}

		private void ToolStripMenu(bool dragDrop, string newButtonName)
		{
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					ColorLibraryForm.AddNewColor();
					break;
				case "toolStripCurveLibrary":
					if (!dragDrop)
					{
						CurveLibraryForm.AddCurveToLibrary(new Curve());
					}
					break;
				case "toolStripGradientLibrary":
					if (!dragDrop)
					{
						GradientLibraryForm.AddGradientToLibrary(new ColorGradient());
					}
					break;
			}
			TimelineControl.Focus();
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

						UpdateColorLibrary();
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
			TimelineControl.Focus();
		}

		private void toolStripMenuItemDeleteItem_Click(object sender, EventArgs e)
		{
			List<ToolStripButton> removeButtons = new List<ToolStripButton>();
			switch (_contextToolStrip.Name)
			{
				case "toolStripColorLibrary":
					var colors = new List<Color>();
					foreach (ToolStripButton tsb in toolStripColorLibrary.Items)
					{
						if (tsb.Checked)
						{
							colors.Add((Color)tsb.Tag);
						}
					}
					ColorLibraryForm.RemoveColors(colors);
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
			TimelineControl.Focus();
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
			PopulateColors();
		}

		private void PopulateColorLibraryToolStrip_Context()
		{
			PopulateColors();
		}

		public void PopulateColors()
		{
			if (_currentToolStrip == null)
			{
				toolStripColorLibrary.SuspendLayout();
				toolStripColorLibrary.Items.Clear();
				var itemsState = new Dictionary<string, bool>();
				if (_toolStripStates.States.TryGetValue(toolStripGradientLibrary.Name, out var state))
				{
					itemsState = state;
				}
				foreach (Color colorItem in ColorLibraryForm.Colors)
				{
					string name = $"R: {colorItem.R} G: {colorItem.G} B: {colorItem.B}";
					ToolStripButton tsb = new ToolStripButton
					{
						ToolTipText = name,
						Tag = colorItem,
						Image = CreateColorListItem(colorItem),
						TextImageRelation = TextImageRelation.ImageAboveText,
						Name = name
					};

					tsb.MouseDown += toolStripLibraryButton_MouseDown;
					tsb.MouseUp += toolStripLibraryButton_MouseUp;
					tsb.MouseMove += toolStripLibraryButton_MouseMove;
					tsb.MouseEnter += toolStripLibraryButton_MouseEnter;
					tsb.MouseLeave += toolStripLibraryButton_MouseLeave;
					if (itemsState.TryGetValue(tsb.Name, out var visible))
					{
						tsb.Visible = visible;
					}
					toolStripColorLibrary.Items.Add(tsb);
				}
				toolStripColorLibrary.ResumeLayout();
			}
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

		private void UpdateColorLibrary()
		{
			var colors = new List<Color>();
			foreach (ToolStripButton tsb in toolStripColorLibrary.Items) colors.Add((Color)tsb.Tag);
			ColorLibraryForm.UpdateColorSet(colors);
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
			if (selectedToolStrip != null)
			{
				_contextToolStrip = selectedToolStrip;
			}
			if (e.Effect == DragDropEffects.Copy)
			{
				Color colorItem = (Color)e.Data.GetData(typeof(Color));
				ColorLibraryForm.AddColor(colorItem);
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
				var itemsState = new Dictionary<string, bool>();
				if (_toolStripStates.States.TryGetValue(toolStripCurveLibrary.Name, out var state))
				{
					itemsState = state;
				}
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

						tsb.MouseDown += toolStripLibraryButton_MouseDown;
						tsb.MouseUp += toolStripLibraryButton_MouseUp;
						tsb.MouseMove += toolStripLibraryButton_MouseMove;
						tsb.MouseEnter += toolStripLibraryButton_MouseEnter;
						tsb.MouseLeave += toolStripLibraryButton_MouseLeave;
						if (itemsState.TryGetValue(tsb.Name, out var visible))
						{
							tsb.Visible = visible;
						}
						toolStripCurveLibrary.Items.Add(tsb);
					}
				}

				toolStripCurveLibrary.ResumeLayout();
			}
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
				
				CurveLibraryForm.AddCurveToLibrary(c, false);

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

				var itemsState = new Dictionary<string, bool>();
				if (_toolStripStates.States.TryGetValue(toolStripGradientLibrary.Name, out var state))
				{
					itemsState = state;
				}
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

						tsb.MouseDown += toolStripLibraryButton_MouseDown;
						tsb.MouseUp += toolStripLibraryButton_MouseUp;
						tsb.MouseMove += toolStripLibraryButton_MouseMove;
						tsb.MouseEnter += toolStripLibraryButton_MouseEnter;
						tsb.MouseLeave += toolStripLibraryButton_MouseLeave;

						if (itemsState.TryGetValue(tsb.Name, out var visible))
						{
							tsb.Visible = visible;
						}
						toolStripGradientLibrary.Items.Add(tsb);
					}
				}

				toolStripGradientLibrary.ResumeLayout();
			}
		}

		#endregion

		#region Gradient Library Toolstrip Enter and Drop

		private void toolStripGradientLibrary_DragDrop(object sender, DragEventArgs e)
		{
			ToolStrip selectedToolStrip = sender as ToolStrip;
			if (selectedToolStrip != null) _contextToolStrip = selectedToolStrip;
			if (e.Effect == DragDropEffects.Copy)
			{
				// Add the Gradient to to the library.
				ColorGradient c = (ColorGradient)e.Data.GetData(typeof(ColorGradient));
				GradientLibraryForm.AddGradientToLibrary(c, false);
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
				Name = effectDescriptor.EffectName,
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
			tsmi.Text = tsmi.Name = effectDescriptor.EffectName;
			tsmi.CheckState = CheckState.Checked;
			tsmi.Checked = true;
			tsmi.CheckOnClick = true;
			tsmi.Image = tsb.Image;
			tsmi.Click += ToolStripEffectItem_Changed;
			tsmi.Tag = tsb;
			toolStripMenuItem.DropDownItems.Add(tsmi);
			if (_toolStripStates.States.TryGetValue(toolStripEffects.Name, out var stripItemStates))
			{
				if (stripItemStates.TryGetValue(tsmi.Name, out var visible))
				{
					tsmi.Checked = visible;
					if (!toolStripMenuItem.Checked)
					{
						tsb.Visible = false;
					}
					else if (!tsmi.Checked)
					{
						tsb.Visible = false;
					}
				}
			}
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
			if (tsi.OwnerItem is ToolStripMenuItem tso)
			{
				tso.Checked = true;
				if (!_toolStripStates.States.TryGetValue(toolStripEffects.Name, out var itemStates))
				{
					itemStates = new Dictionary<string, bool>();
					_toolStripStates.States.Add(toolStripEffects.Name, itemStates);
				
				}

				if (tsi.Tag is ToolStripItem item)
				{
					item.Visible = tsi.Checked;
					if (itemStates.ContainsKey(item.Name))
					{
						itemStates[item.Name] = tsi.Checked;
					}
					else
					{
						itemStates.Add(item.Name, tsi.Checked);
					}
				}

				toolStripMenuItemEffectGroup_Change(tso);
			}
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
			PlayPauseToggle();
		}

		private void toolStripButton_Stop_Click(object sender, EventArgs e)
		{
			StopSequence();
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
				tsmi.Tag = tsi;
				tsmi.Text = tsi.ToolTipText;
				tsmi.CheckOnClick = true;
				tsmi.CheckState = CheckState.Checked;
				tsmi.Click += ToolStripItemState_Changed;
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
			if(e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
			{
				if (_currentToolStrip is ToolStrip ts)
				{
					foreach (ToolStripButton button in ts.Items)
					{
						button.Checked = false;
					}
				}
				TimelineControl.Focus();
			}
			_currentToolStrip = null;
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
			_toolStripStates = new ToolStripStates();

			if (File.Exists(_standardToolStripFilePath))
			{
				using (FileStream reader = new FileStream(_standardToolStripFilePath, FileMode.Open, FileAccess.Read))
				{
					try
					{
						DataContractSerializer ser = new DataContractSerializer(typeof(ToolStripStates));
						_toolStripStates = (ToolStripStates)ser.ReadObject(reader);
					}
					catch (Exception e)
					{
						Logging.Error(e, "An error occured trying to restore the tool strip state.");
					}
				}

				if (File.Exists(_profileToolStripFilePath))
				{
					using (FileStream reader = new FileStream(_profileToolStripFilePath, FileMode.Open, FileAccess.Read))
					{
						try
						{
							DataContractSerializer ser = new DataContractSerializer(typeof(ToolStripStates));
							var profileStates = (ToolStripStates)ser.ReadObject(reader);
							_toolStripStates.States.AddRange(profileStates.States);
						}
						catch (Exception e)
						{
							Logging.Error(e, "An error occured trying to restore the profile tool strip state.");
						}
					}
				}
			}
			else
			{
				PopulateToolStripStates();
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

			// Save each Toolstrip item visible settings
			var xmlSettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			var profileStates = new ToolStripStates
			{
				States = _toolStripStates.States.Where(x => x.Key == toolStripGradientLibrary.Name ||
				                                            x.Key == toolStripCurveLibrary.Name ||
				                                            x.Key == toolStripColorLibrary.Name)
					.ToDictionary(val => val.Key, val => val.Value)
			};



			var regularStates = new ToolStripStates
			{
				States = _toolStripStates.States.Except(profileStates.States)
					.ToDictionary(val => val.Key, val => val.Value)
			};

			try
			{
				DataContractSerializer dataSer = new DataContractSerializer(typeof(ToolStripStates));
				var dataWriter = XmlWriter.Create(_standardToolStripFilePath, xmlSettings);
				dataSer.WriteObject(dataWriter, regularStates);
				dataWriter.Close();

				dataWriter = XmlWriter.Create(_profileToolStripFilePath, xmlSettings);
				dataSer.WriteObject(dataWriter, profileStates);
				dataWriter.Close();
			}
			catch (Exception e)
			{
				Logging.Error(e, "An error occured saving the tool strip states.");
			}
		}

		private void PopulateToolStripStates()
		{
			toolStripContainer.SuspendLayout();
			
			_toolStripStates.States.Clear();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					if (toolsStripControl is ToolStrip toolsStripItems)
					{
						var toolStripItemStates = new Dictionary<string, bool>();
						_toolStripStates.States.Add(toolsStripControl.Name, toolStripItemStates);

						foreach (ToolStripItem item in toolsStripItems.Items)
						{
							var state = new Tuple<string, bool>(item.Name, true);
						
							if (toolsStripItems.Name == toolStripEffects.Name)
							{
								foreach (ToolStripMenuItem group in effectGroupsToolStripMenuItem.DropDownItems)
								{
									foreach (ToolStripMenuItem menuItem in group.DropDownItems)
									{
										if (item.Name == menuItem.Text)
										{
											state = new Tuple<string, bool>(item.Name, menuItem.Checked);
											break;
										}
									}
								}
							}
							else {
								state = new Tuple<string, bool>(item.Name, item.Visible);
							}

							if (!toolStripItemStates.ContainsKey(item.Name))
							{
								toolStripItemStates.Add(state.Item1, state.Item2);
							}
						}
					}
				}
			}

			toolStripContainer.ResumeLayout();
		}

		[DataContract]
		internal class ToolStripStates
		{
			public ToolStripStates()
			{
				States = new Dictionary<string, Dictionary<string, bool>>();
			}

			[DataMember]
			public Dictionary<string, Dictionary<string, bool>> States { get; set; }
		}


		#endregion

	}
}