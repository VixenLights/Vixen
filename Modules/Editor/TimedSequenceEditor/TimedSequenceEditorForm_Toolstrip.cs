using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Vixen.Module.Effect;
using Vixen.Services;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		//Used for the Effects ToolStrip
		private ToolStripItem _mToolStripEffects;
		private bool _beginNewEffectDragDrop;
		private List<AllToolStripItems> _allToolStripItems;
		private readonly string _toolStripFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen", "ToolStripItemPositions.xml");
		private List<bool> _defaultOperationsToolStrip;
		private List<bool> _defaultCustomsToolStrip;

		private void InitializeToolBars()
		{
			int i = 0;
			_defaultOperationsToolStrip = new List<bool>();
			foreach (ToolStripItem tsi in toolStripOperations.Items)
			{
				_defaultOperationsToolStrip.Add(tsi.Visible);
				i++;
			}

			_defaultCustomsToolStrip = new List<bool>();
			foreach (ToolStripItem tsi in toolStripCustom.Items)
			{
				_defaultCustomsToolStrip.Add(tsi.Visible);
			}

			toolStripCustom.Visible = false;

			XMLProfileSettings xml = new XMLProfileSettings();
			// Custom Toolstrip View Menu setting.
			customToolStripToolStripMenuItem.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CustomToolStrip/CustomToolStrip", Name), false);
			
			// Effect Toolstrip View Menu setting.
			effectToolStripToolStripMenuItem.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EffectToolStrip/EffectToolStrip", Name), false);

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
			PopulateOperationsToolStrip_Context();
			PopulateCustomToolStrip_Context();
		}

		private void SetToolBarLayout()
		{
			// Set layout and settings for all Toolbars.
			XMLProfileSettings xml = new XMLProfileSettings();

			SuspendLayout();

			for (int i = 0; i < 3; i++)
			{
				int toolStripLocationX = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripX{1}", Name, i), 0);
				int toolStripLocationY = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripY{1}", Name, i), 0);
				string toolStripName = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolStripName{1}", Name, i), "");

				switch (toolStripName)
				{
					case "toolStripEffects":
						toolStripEffects.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripEffects.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						break;
					case "toolStripOperations":
						toolStripOperations.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripOperations.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						break;
					case "toolStripCustom":
						toolStripCustom.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						toolStripCustom.Location = new Point(toolStripLocationX, toolStripLocationY + 5);
						break;
				}
			}

			SetToolStripStartPosition();
			ResumeLayout();
			PerformLayout();
		}

		#region Main Operations ToolStrip

		private void toolStripButton_DrawMode_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.EnableDrawMode = true;
			toolStripButton_DrawMode.Checked = true;
			toolStripButton_SelectionMode.Checked = false;
		}

		private void toolStripButton_SelectionMode_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.EnableDrawMode = false;
			toolStripButton_SelectionMode.Checked = true;
			toolStripButton_DrawMode.Checked = false;
			
			// Ensure any Effect buttons in the toolstrip do not have the selected box around it.
			foreach (ToolStripItem effectButton in toolStripEffects.Items)
			{
				if (effectButton is ToolStripButton) ((ToolStripButton)effectButton).Checked = false;
			}
		}

		private void toolStripButton_DragBoxFilter_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.DragBoxFilterEnabled = toolStripButton_DragBoxFilter.Checked;
		}

		private void toolStripSplitButton_CloseGaps_ButtonClick(object sender, EventArgs e)
		{
			TimelineControl.grid.CloseGapsBetweenElements();
		}

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
			toolStripMenuItem_Loop.Checked = toolStripButton_Loop.Checked;
		}

		private void toolStripButton_SnapTo_CheckedChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_SnapTo.Checked = toolStripButton_SnapTo.Checked;
			TimelineControl.grid.EnableSnapTo = toolStripButton_SnapTo.Checked;
		}

		private void toolStripButton_IncreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed + _timingChangeDelta);
		}

		private void toolStripButton_DecreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed - _timingChangeDelta);
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
		
		// Populate the Operations toolstrip contextMenu.
		private void PopulateOperationsToolStrip_Context()
		{
			foreach (ToolStripItem item in toolStripOperations.Items)
			{
				//if (item.Tag != null && (item is ToolStripButton || item is UndoButton || item is ToolStripComboBox || (item is ToolStripDropDownButton && !item.ToString().Contains("DropDown"))))
				//{
				ToolStripMenuItem tsmi = new ToolStripMenuItem();
				bool menuItemExists = false;
				foreach (ToolStripMenuItem ddi in add_RemoveOperationsToolStripMenuItem.DropDownItems)
				{
					if (ddi.Text == item.ToolTipText)
					{
						menuItemExists = true;
						break;
					}
				}

				if (menuItemExists) continue;
				tsmi.Text = item.ToolTipText;
				tsmi.Tag = item;
				tsmi.CheckOnClick = true;
				tsmi.CheckState = CheckState.Checked;
				foreach (AllToolStripItems it in _allToolStripItems)
				{
					if (tsmi.Text == it.ItemName)
					{
						item.Visible = it.Visible;;
						tsmi.Checked = it.Visible;
					}
				}

				tsmi.Click += ToolStripOperationItem_Changed;
				add_RemoveOperationsToolStripMenuItem.DropDownItems.Add(tsmi);
			}

			toolStripOperations.SuspendLayout();
			foreach (AllToolStripItems it in _allToolStripItems)
			{
				foreach (ToolStripItem item in toolStripOperations.Items)
				{
					if (item.ToolTipText == it.ItemName) item.Visible = it.Visible;
				}
			}
			toolStripOperations.ResumeLayout();
		}

		// Show/Hide selected Toolstrip item.
		private void ToolStripOperationItem_Changed(object sender, EventArgs e)
		{
			ToolStripMenuItem tsi = sender as ToolStripMenuItem;
			foreach (ToolStripItem item in toolStripOperations.Items)
			{
				if (item.ToolTipText != null && item.ToolTipText == tsi.Text || item.Tag != null && item.Tag.ToString() == tsi.Text) item.Visible = tsi.Checked;
			}
		}

		private void resetOperationsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Resets all Custom Context menu items to checked.
			for (var i = 0; i < _defaultOperationsToolStrip.Count; i++)
			{
				toolStripOperations.Items[i].Visible = _defaultOperationsToolStrip[i];
				foreach (ToolStripMenuItem tsmi in add_RemoveOperationsToolStripMenuItem.DropDownItems)
				{
					if (tsmi.Text == toolStripOperations.Items[i].ToolTipText)
					{
						tsmi.Checked = _defaultOperationsToolStrip[i];
						break;
					}
				}
			}
		}

		#endregion

		#region Effect ToolStrip

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

			if (toolStripButton_DrawMode.Checked) SelectNodeForDrawing(selectedButton);
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

		private void toolStripMenuItem_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
		}

		private void effectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			effectToolStripToolStripMenuItem.Checked = !effectToolStripToolStripMenuItem.Checked;
		}

		private void effectToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			toolStripEffects.Visible = !toolStripEffects.Visible;
		}

		#endregion

		#region Custom ToolStrip

		// Populate the Custom toolstrip context Menu.
		private void PopulateCustomToolStrip_Context()
		{
			SortedDictionary<int, ToolStripItem> tempItems = new SortedDictionary<int, ToolStripItem>();
			foreach (ToolStripItem item in toolStripCustom.Items)
			{
				bool itemFound = false;
				ToolStripMenuItem tsmi = new ToolStripMenuItem();
				bool menuItemExists = false;
				foreach (ToolStripMenuItem ddi in add_RemoveCustomToolStripMenuItem.DropDownItems)
				{
					if (ddi.Text == item.ToolTipText)
					{
						menuItemExists = true;
						break;
					}
				}

				if (menuItemExists) continue;
				tsmi.Text = item.ToolTipText;
				tsmi.Tag = item;
				tsmi.CheckOnClick = true;
				tsmi.CheckState = CheckState.Checked;
				foreach (AllToolStripItems it in _allToolStripItems)
				{
					if (tsmi.Text == it.ItemName)
					{
						tsmi.Checked = it.Visible;
						item.Visible = it.Visible;
						tempItems.Add(it.Position, item);
						itemFound = true;
					}
				}
				tsmi.Click += ToolStripCustomItem_Changed;
				add_RemoveCustomToolStripMenuItem.DropDownItems.Add(tsmi);
				if (!itemFound) tempItems.Add(tempItems.Count, item);
			}

			toolStripCustom.SuspendLayout();
			toolStripCustom.Items.Clear();
			foreach (KeyValuePair<int, ToolStripItem> groupItem in tempItems) toolStripCustom.Items.Add(groupItem.Value);
			toolStripCustom.ResumeLayout();
		}

		// Show/Hide selected Toolstrip item.
		private void ToolStripCustomItem_Changed(object sender, EventArgs e)
		{
			var tsi = sender as ToolStripMenuItem;
			foreach (ToolStripItem item in toolStripCustom.Items)
			{
				if (item.ToolTipText != null && item.ToolTipText == tsi.Text || item.Tag != null && item.Tag.ToString() == tsi.Text) item.Visible = tsi.Checked;
			}
		}

		private void resetCustomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Resets all Custom Context menu items to checked.
			for (var i = 0; i < _defaultCustomsToolStrip.Count; i++)
			{
				toolStripCustom.Items[i].Visible = _defaultCustomsToolStrip[i];
				foreach (ToolStripMenuItem tsmi in add_RemoveCustomToolStripMenuItem.DropDownItems)
				{
					if (tsmi.Text == toolStripCustom.Items[i].ToolTipText)
					{
						tsmi.Checked = _defaultCustomsToolStrip[i];
						break;
					}
				}
			}
		}

		private void customToolStripButton_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("Start");
		}

		private void customToolStripButton_End_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("End");
		}

		private void customToolStripButton_Both_Click(object sender, EventArgs e)
		{
			AlignEffectsToNearestMarks("Both");
		}

		private void customToolStripButton_Distribute_Click(object sender, EventArgs e)
		{
			DistributeSelectedEffectsEqually();
		}

		private void customToolStripMenuItem_Click(object sender, EventArgs e)
		{
			customToolStripToolStripMenuItem.Checked = !customToolStripToolStripMenuItem.Checked;
		}

		private void customToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			toolStripCustom.Visible = !toolStripCustom.Visible;
		}

		#endregion

		#region ToolStrip Helpers

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
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					int i = 0;
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						AllToolStripItems ts = new AllToolStripItems();
						ts.ItemName = item.ToolTipText;
						ts.Position = i;
						if (initialLoad && toolsStripItems.Name == "toolStripEffects")
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
						i++;
					}
				}
			}
		}

		[Serializable]
		internal class AllToolStripItems
		{
			public string ItemName;
			public int Position;
			public bool Visible;
		}

		#endregion
	}
}