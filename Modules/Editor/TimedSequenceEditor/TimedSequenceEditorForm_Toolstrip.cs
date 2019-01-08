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

		private void InitializeToolBars()
		{
			// Populate Effect Toolstrip Context Menu
			PopulateEffectGroupToolStrip();
			// Add Effects to Effects Toolstrip
			PopulateToolStripEffects();
			// Populate the Operations Context Menu. Don't need to add Operations to the Toolstrip as it is done during Compile time.
			PopulateOperationsGroupToolStrip();
			// Populate the Custom Context Menu. Don't need to add Custom Items to the Toolstrip as it is done during Compile time.
			PopulateCustomGroupToolStrip();
			// Load ToolStrip Items form saved file.
			Load_ToolsStripItemsFile();

			// Uses the data from the saved file to change visibility of the ToolStrip items.
			_toolStripItems.Clear();
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						_toolStripItems.Add(item);
					}
					toolsStripItems.Items.Clear();
				}
			}
			
			// This will ensure any programmable Toolstrip items added later will be visible.
			IEnumerable<ToolStripItem> newToolStripItems = _toolStripItems.Where(n => !_allToolStripItems.Select(n1 => n1.ItemName).Contains(n.ToolTipText));
			foreach (var item in newToolStripItems)
			{
				AllToolStripItems ts = new AllToolStripItems();
				if (item.Name.Contains("custom"))
				{
					ts.AssignedToolStrip = "toolStripCustom";
				}
				else if (item.Name.Contains("effects"))
				{
					ts.AssignedToolStrip = "toolStripEffect";
				}
				else if (item.Name.Contains("operations"))
				{
					ts.AssignedToolStrip = "toolStripOperations";
				}

				ts.ItemName = item.ToolTipText;
				ts.Position = 100;
				ts.Visible = true;
				_allToolStripItems.Add(ts);
			}

			foreach (ToolStripPanelRow row1 in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row1.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (AllToolStripItems it in _allToolStripItems)
					{
						foreach (ToolStripItem item in _toolStripItems)
						{
							if (item.ToolTipText == it.ItemName && toolsStripItems.Name == it.AssignedToolStrip)
							{
								item.Visible = it.Visible;
								toolsStripItems.Items.Add(item);
							}
						}
					}
				}
			}

			// Set Toolstrips Display Style. 
			ToolStripOperationsDisplayStyle(true);
			ToolStripEffectsDisplayStyle(true);
			ToolStripCustomDisplayStyle(true);
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
					if (!item.Equals(subItem) && subItem != null)
					{
						subItem.Checked = false;
					}
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
					if (!item.Equals(subItem) && subItem != null)
					{
						subItem.Checked = false;
					}
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
					if (!item.Equals(subItem) && subItem != null)
					{
						subItem.Checked = false;
					}
				}
				item.Checked = true;
				AlignTo_Threshold = item.ToString();
			}
		}
		
		// Populate the Operations toolstrip contextMenu.
		private void PopulateOperationsGroupToolStrip()
		{
			foreach (ToolStripItem item in toolStripOperations.Items)
			{
				if (item.Tag != null && (item is ToolStripButton || item is UndoButton || item is ToolStripComboBox || (item is ToolStripDropDownButton && !item.ToString().Contains("DropDown"))))
				{
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					bool menuItemExists = false;
					foreach (ToolStripMenuItem ddi in add_RemoveOperationsToolStripMenuItem.DropDownItems)
					{
						if (ddi.Text == item.Tag.ToString())
						{
							menuItemExists = true;
							break;
						}
					}

					if (menuItemExists) continue;
					tsmi.Text = item.Tag.ToString();
					tsmi.CheckOnClick = true;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Checked = item.Visible;
					tsmi.Click += ToolStripItem_Changed;
					add_RemoveOperationsToolStripMenuItem.DropDownItems.Add(tsmi);
				}
			}
		}

		private void toolStripOperations_MouseEnter(object sender, EventArgs e)
		{
			ToolStripOperationsDisplayStyle(false);
		}

		private void ToolStripOperationsDisplayStyle(bool initialLoad)
		{
			if (ModifierKeys == Keys.Alt || initialLoad)
			{
				foreach (ToolStripItem item in toolStripOperations.Items)
				{
					if (item.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText) item.DisplayStyle = ToolStripItemDisplayStyle.Image;
				}
			}
		}

		#endregion

		#region Effect ToolStrip

		// Populate the Effect groups context menu
		private void PopulateEffectGroupToolStrip()
		{
			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				foreach (
					IEffectModuleDescriptor effectDescriptor in ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
				{
					if (effectDescriptor.EffectName != "Nutcracker" && effectDescriptor.EffectGroup.ToString() == dropDownItem.Tag.ToString())
					{
						dropDownItem.DropDownItems.Add(effectDescriptor.EffectName);
					}
				}

				foreach (ToolStripMenuItem items in dropDownItem.DropDownItems)
				{
					items.Checked = true;
					items.CheckOnClick = true;
					items.Checked = dropDownItem.Visible;
					items.CheckState = CheckState.Checked;
					items.Click += ToolStripItem_Changed;
				}
			}
		}

		private void resetEffectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				dropDownItem.Checked = true;
				foreach (ToolStripMenuItem groupDropDownItem in dropDownItem.DropDownItems)
				{
					groupDropDownItem.Checked = true;
					ChangeToolStripItemVisibility(groupDropDownItem);
				}
			}

			PopulateToolStripEffects();
		}

		private void PopulateToolStripEffects()
		{
			toolStripEffects.SuspendLayout();
			toolStripEffects.Items.Clear();

			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				foreach (ToolStripMenuItem groupDropDownItem in dropDownItem.DropDownItems)
				{
					foreach (
						IEffectModuleDescriptor effectDescriptor in
						ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
					{
						if (effectDescriptor.EffectName != "Nutcracker" &&
						    effectDescriptor.EffectGroup.ToString() == dropDownItem.Tag.ToString() &&
						    groupDropDownItem.Text == effectDescriptor.EffectName)
						{
							AddEffect(effectDescriptor, dropDownItem.Checked, groupDropDownItem.Checked);
						}
					}
				}
			}

			AlignText();
			toolStripEffects.ResumeLayout();
		}
		
		private void AddEffect(IEffectModuleDescriptor effectDescriptor, bool dropDownGroupChecked, bool dropDownItemChecked)
		{
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						if (item.ToolTipText == effectDescriptor.EffectName)
						{
							return;
						}
					}
				}
			}
		
		ToolStripButton tsb = new ToolStripButton
			{
				ToolTipText = effectDescriptor.EffectName,
				Tag = effectDescriptor.TypeId,
				ImageIndex = toolStripEffects.Items.Count - 1,
				ImageKey = effectDescriptor.EffectName,
				Image = effectDescriptor.GetRepresentativeImage(),
				Text = effectDescriptor.EffectName,
				Visible = dropDownGroupChecked && dropDownItemChecked
		};
			tsb.MouseDown += tsb_MouseDown;
			tsb.MouseMove += tsb_MouseMove;
			toolStripEffects.Items.Add(tsb);
		}
		
		private void SelectNodeForDrawing(ToolStripButton selectedButton)
		{
			//This is for when Drawing effects is enabled.

			if (selectedButton.Checked) //If button is already selected then deselect and remove GUID from grid selected effect.
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
				//Ensures the one selected to be drawn is Checked
				selectedButton.Checked = true;
			}
		}

		private void AlignText()
		{
			//Adjusts location of the effect label.
			toolStripEffects.SuspendLayout();
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
				else
				{
					tsi.Text = "";
				}
				tsi.Font = newFontSize;
			}
			toolStripEffects.ResumeLayout();
		}

		private void toolStripEffects_MouseEnter(object sender, EventArgs e)
		{
			ToolStripEffectsDisplayStyle(false);
		}

		private void ToolStripEffectsDisplayStyle(bool initialLoad)
		{
			if (ModifierKeys == Keys.Alt || initialLoad)
			{
				foreach (ToolStripItem item in toolStripEffects.Items) item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
				AlignText();
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

			if (toolStripButton_DrawMode.Checked)
			{
				SelectNodeForDrawing(selectedButton);
			}
			else
			{
				selectedButton.Checked = false;
			}
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
			AlignText();
		}

		private void toolStripMenuItemEffectGroup_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
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


		// Populate the Operations toolstrip contextMenu.
		private void PopulateCustomGroupToolStrip()
		{
			foreach (ToolStripItem item in toolStripCustom.Items)
			{
				if (item.Tag != null && (item is ToolStripButton || item is UndoButton || item is ToolStripComboBox || (item is ToolStripDropDownButton && !item.ToString().Contains("DropDown"))))
				{
					ToolStripMenuItem tsmi = new ToolStripMenuItem();
					bool menuItemExists = false;
					foreach (ToolStripMenuItem ddi in add_RemoveCustomToolStripMenuItem.DropDownItems)
					{
						if (ddi.Text == item.Tag.ToString())
						{
							menuItemExists = true;
							break;
						}
					}

					if (menuItemExists) continue;
					tsmi.Text = item.Tag.ToString();
					tsmi.CheckOnClick = true;
					tsmi.CheckState = CheckState.Checked;
					tsmi.Checked = item.Visible;
					tsmi.Click += ToolStripItem_Changed;
					add_RemoveCustomToolStripMenuItem.DropDownItems.Add(tsmi);
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

		private void toolStripCustom_MouseEnter(object sender, EventArgs e)
		{
			ToolStripCustomDisplayStyle(false);
		}

		private void ToolStripCustomDisplayStyle(bool initialLoad)
		{
			if (ModifierKeys == Keys.Alt || initialLoad)
			{
				foreach (ToolStripItem item in toolStripCustom.Items)
				{
					if (item.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
					{
						item.DisplayStyle = ToolStripItemDisplayStyle.Image;
					}
				}
			}
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

		// Show/Hide selected Toolstrip item.
		private void ToolStripItem_Changed(object sender, EventArgs e)
		{
			var tsi = sender as ToolStripMenuItem;
			ChangeToolStripItemVisibility(tsi);
		}

		private void ChangeToolStripItemVisibility(ToolStripMenuItem tsi)
		{
			foreach (var row in toolStripContainer.TopToolStripPanel.Rows)
			{
				foreach (Control toolsStripControl in row.Controls)
				{
					ToolStrip toolsStripItems = toolsStripControl as ToolStrip;
					foreach (ToolStripItem item in toolsStripItems.Items)
					{
						if (item.ToolTipText != null && item.ToolTipText == tsi.Text || item.Tag != null && item.Tag.ToString() == tsi.Text)
						{
							item.Visible = tsi.Checked;
						}
					}
				}
			}
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
						if (initialLoad && toolsStripItems.Name != "toolStripOperations")
						{
							ts.Visible = true;
						}
						else
						{
							ts.Visible = item.Visible;
						}
						ts.AssignedToolStrip = toolsStripItems.Name;
						_allToolStripItems.Add(ts);
						i++;
					}
				}
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

		[Serializable]
		internal class AllToolStripItems
		{
			public string ItemName;
			public int Position;
			public bool Visible;
			public string AssignedToolStrip;
		}

		#endregion
	}
}