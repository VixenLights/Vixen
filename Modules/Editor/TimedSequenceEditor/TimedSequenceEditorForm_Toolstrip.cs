using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Services;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		//Used for the Effects ToolStrip
		private ToolStripItem _mToolStripEffects;
		private bool _beginNewEffectDragDrop;

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
				if (item.Tag != null && (item is ToolStripButton || item is ToolStripComboBox || (item is ToolStripDropDownButton && !item.ToString().Contains("DropDown"))))
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
					tsmi.Click += PopulateToolStripOperations;
					add_RemoveOperationsToolStripMenuItem.DropDownItems.Add(tsmi);
				}
			}
		}
		
		// Show/Hide Operations toolstrip item.
		private void PopulateToolStripOperations(object sender, EventArgs e)
		{
			var tsi = sender as ToolStripItem;
			foreach (ToolStripItem item in toolStripOperations.Items)
			{
				if (item.Tag != null && item.Tag.ToString() == tsi.Text) item.Visible = !item.Visible;
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
					IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
				{
					if (effectDesriptor.EffectName != "Nutcracker" &&
						effectDesriptor.EffectGroup.ToString() == dropDownItem.Tag.ToString())
					{
						dropDownItem.DropDownItems.Add(effectDesriptor.EffectName);
					}
				}
				foreach (ToolStripMenuItem items in dropDownItem.DropDownItems)
				{
					items.Checked = true;
					items.CheckOnClick = true;
					items.CheckState = CheckState.Checked;
					items.Click += PopulateToolStripEffects;
				}
			}
		}

		// Populate the Effects toolstrip
		private void PopulateToolStripEffects(object sender, EventArgs e)
		{
			if (sender is ToolStripDropDownItem item && item.OwnerItem is ToolStripMenuItem itemOwner) itemOwner.Checked = true;
			contextMenuStripEffect.SuspendLayout();
			PopulateToolStripEffects();
			contextMenuStripEffect.ResumeLayout();
		}

		private void PopulateToolStripEffects()
		{
			toolStripEffects.SuspendLayout();
			toolStripEffects.Items.Clear();

			foreach (ToolStripMenuItem dropDownItem in effectGroupsToolStripMenuItem.DropDownItems)
			{
				if (!dropDownItem.Checked) continue;
				foreach (ToolStripMenuItem groupDropDownItem in dropDownItem.DropDownItems)
				{
					foreach (
						IEffectModuleDescriptor effectDesriptor in
						ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
					{
						if (effectDesriptor.EffectName != "Nutcracker" &&
							effectDesriptor.EffectGroup.ToString() == dropDownItem.Tag.ToString() &&
							groupDropDownItem.Text == effectDesriptor.EffectName && groupDropDownItem.Checked) AddEffect(effectDesriptor);
					}
				}

				//Adds a toolstrip seperator to seperate effects groups. 
				ToolStripSeparator tss = new ToolStripSeparator();
				toolStripEffects.Items.Add(tss);
			}

			AlignText();
			toolStripEffects.ResumeLayout();
		}

		private void AddEffect(IEffectModuleDescriptor effectDesriptor)
		{
			ToolStripButton tsb = new ToolStripButton
			{
				ToolTipText = effectDesriptor.EffectName,
				Tag = effectDesriptor.TypeId,
				ImageIndex = toolStripEffects.Items.Count - 1,
				ImageKey = effectDesriptor.EffectName,
				Image = effectDesriptor.GetRepresentativeImage(),
				Text = effectDesriptor.EffectName
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
				else
				{
					tsi.Text = "";
				}
				tsi.Font = newFontSize;
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
			PopulateToolStripEffects();
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
	}
}