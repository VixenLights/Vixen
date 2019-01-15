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
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys.State;
using ZedGraph;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		#region Member Variables

		//Used for the Effects ToolStrip
		private ToolStripItem _mToolStripEffects;
		private bool _beginNewEffectDragDrop;
		private List<AllToolStripItems> _allToolStripItems;
		private readonly string _toolStripFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ToolStripItemPositions.xml");

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
			PopulateAllToolStrips();
			
		}

		private void PlayBackToolStripSetup()
		{
			toolStripPlayBack.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripEdit.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripAlignment.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripFile.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripView.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripTools.ImageScalingSize = new Size(_iconSize, _iconSize);
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
			toolStripAudio.ImageScalingSize = new Size(_iconSize, _iconSize);
			toolStripButton_AssociateAudio.Image = Resources.music;
			toolStripButton_AssociateAudio.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_IncreaseTimingSpeed.Image = Resources.plus;
			toolStripButton_IncreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_DecreaseTimingSpeed.Image = Resources.minus;
			toolStripButton_DecreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			audioToolStripButton_Audio_Devices.Image = Resources.Audio_Devices;
			audioToolStripButton_Audio_Devices.DisplayStyle = ToolStripItemDisplayStyle.Image;
			audioToolStripLabel_TimingSpeed.Image = SpeedVisualisation();
			audioToolStripLabel_TimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripAudio.MouseWheel += toolStripAudio_MouseWheel;
		}

		private void ModeToolStripSetup()
		{
			toolStripMode.ImageScalingSize = new Size(_iconSize, _iconSize);
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

			//SuspendLayout();
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
			//ResumeLayout();

			toolStripContainer.ResumeLayout();
			toolStripContainer.PerformLayout();
			//PerformLayout();

			// Add Theme to all ToolStrips.
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					toolsStripItems.Renderer = new ThemeToolStripRenderer();

					// Add all Toolbars to the Edit menu
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					tsmi.Visible = true;
					tsmi.Text = toolsStripItems.Text;
					tsmi.CheckOnClick = true;
					tsmi.Tag = toolsStripControl;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Checked = toolsStripItems.Visible;
					tsmi.Click += toolstripToolStripMenuItem_Click;
					tsmi.CheckedChanged += toolstripToolStripMenuItem_CheckedChanged;
					toolbarToolStripMenuItem.DropDownItems.Add(tsmi);
				}
			}
			SortToolStripMenuItemAlphabetically(toolbarToolStripMenuItem);
		}

#endregion

		#region All Toolstrips Initialization (Except Effects)

		private void PopulateAllToolStrips()
		{
			toolStripContainer.SuspendLayout();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					if (toolsStripItems.Name == "toolStripEffects") continue; // Skip the Effects Toolstrip
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

		#region All Toolstrip Events (Except Effects)

		// Show/Hide selected Toolstrip item.
		private void ToolStripAllItem_Changed(object sender, EventArgs e)
		{
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

			toolbarToolStripMenuItem.DropDownItems.Clear();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;

					// Add all Toolbars to the Edit menu
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					tsmi.Visible = true;
					tsmi.Text = toolsStripItems.Text;
					tsmi.CheckOnClick = true;
					tsmi.Tag = toolsStripControl;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Checked = toolsStripItems.Visible;
					tsmi.Click += toolstripToolStripMenuItem_Click;
					tsmi.CheckedChanged += toolstripToolStripMenuItem_CheckedChanged;
					toolbarToolStripMenuItem.DropDownItems.Add(tsmi);
				}
			}
			SortToolStripMenuItemAlphabetically(toolbarToolStripMenuItem);
		}

		// Generate Context Menu
		private void contextMenuStripAll_Opening(object sender, CancelEventArgs e)
		{
			// Add Toolstrip items to Context menu
			resetToolStripMenuItem.Visible = true;
			add_RemoveContextToolStripMenuItem.Visible = true;
			ContextMenuStrip tsm = sender as ContextMenuStrip;
			if (tsm.SourceControl.Parent.Name != "toolStripContainer")
			{
				add_RemoveContextToolStripMenuItem.DropDownItems.Clear();
				ToolStrip ts = tsm.SourceControl as ToolStrip;
				foreach (ToolStripItem tsi in ts.Items)
				{
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					tsmi.Text = tsi.ToolTipText;
					tsmi.CheckOnClick = true;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Click += ToolStripAllItem_Changed;
					tsmi.Checked = tsi.Visible;
					add_RemoveContextToolStripMenuItem.DropDownItems.Add(tsmi);
				}

				resetToolStripMenuItem.Tag = ts;
			}
			else
			{
				resetToolStripMenuItem.Visible = false;
				add_RemoveContextToolStripMenuItem.Visible = false;
			}

			// Add Toolbar list to Context menu
			toolbarsToolStripMenuItem.DropDownItems.Clear();
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
					toolbarsToolStripMenuItem.DropDownItems.Add(tsmi);
				}
			}
			SortToolStripMenuItemAlphabetically(toolbarsToolStripMenuItem);
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
					toolStripEffects.ImageScalingSize = new Size(_iconSize, _iconSize);
					break;
				case 1:
					alignText = TextImageRelation.TextAboveImage;
					toolStripEffects.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
					break;
				case 2:
					alignText = TextImageRelation.ImageAboveText;
					toolStripEffects.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
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
			toolbarsToolStripMenuItem_Effect.DropDownItems.Clear();
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
					toolbarsToolStripMenuItem_Effect.DropDownItems.Add(tsmi);
				}
			}
			SortToolStripMenuItemAlphabetically(toolbarsToolStripMenuItem_Effect);
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
					tsi.Checked = !tsi.Checked;
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

		private Bitmap SpeedVisualisation()
		{
			// Set Color based on Speed value.
			HSBColor mainColor = new HSBColor(120 - (int)(20.0f / 4.0f * _timingSpeed * 6), 255, 255);

			// Set Colors to use.
			Pen line = new Pen(ThemeColorTable.ForeColor, 3);
			Pen arch = new Pen(mainColor, 5);
			SolidBrush brush = new SolidBrush(mainColor);

			// Create points that define the line start and end location.
			int needleLength = 19;
			double needleAngle = (Math.PI / 180) * (350 - (160 - (int)(20.0f / 4.0f * _timingSpeed * 8)));
			Point point1 = new Point(16, 32);
			Point point2 = new Point((int)(16 + Math.Cos(needleAngle) * needleLength), (int)(32 + Math.Sin(needleAngle) * needleLength));
			
			// Create Bitmap and draw the image.
			Bitmap bmp = new Bitmap(32, 32);
			using (var graphics = Graphics.FromImage(bmp))
			{
				// Draw Speed value.
				using (Font arialFont = new Font("Arial", 10, FontStyle.Bold))
				{
					int speedValueXLocation = _timingSpeed >= 0.97f ? 3 : 6; // Adjust the start location of the Speed text if over 100 as the text is longer then values under 100.
					graphics.DrawString(((int)((float)Math.Round(_timingSpeed, 1) * 100)).ToString(), arialFont, brush,
						new PointF(speedValueXLocation, 0));
				}

				// Draw arc.
				Rectangle rect = new Rectangle(3, 16, 25, 28);
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
						_SetTimingSpeed((float)Math.Round(_timingSpeed, 1) + 0.2f);
					}
					else
					{
						_SetTimingSpeed((float)Math.Round(_timingSpeed, 1) - 0.2f);
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

		#region ToolStrip Helpers
		private void toolstripToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			//menuItem.Checked = !menuItem.Checked;
		}

		private void toolstripToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			ToolStrip toolstrip = menuItem.Tag as ToolStrip;
			toolstrip.Visible = !toolstrip.Visible;
		}

		private void toolStripMenuItem_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
			SetToolStripStartPosition();
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
	}
}