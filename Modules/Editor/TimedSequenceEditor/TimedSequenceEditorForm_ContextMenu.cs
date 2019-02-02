using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using Vixen.Marks;
using Vixen.Module.Effect;
using Vixen.Services;
using VixenModules.Sequence.Timed;
using Element = Common.Controls.Timeline.Element;
using MarkCollection = VixenModules.App.Marks.MarkCollection;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{

		private void AddContextCollectionsMenu()
		{
			ToolStripMenuItem contextMenuItemCollections = new ToolStripMenuItem("Collections") { Image = Resources.collection };

			if ((TimelineControl.SelectedElements.Count() > 1 
				|| TimelineControl.SelectedElements.Count() == 1 && SupportsColorLists(TimelineControl.SelectedElements.FirstOrDefault())) 
				&& _colorCollections.Any())
			{
				ToolStripMenuItem contextMenuItemColorCollections = new ToolStripMenuItem("Colors") { Image = Resources.colors };
				ToolStripMenuItem contextMenuItemRandomColors = new ToolStripMenuItem("Random") {Image = Resources.randomColors };
				ToolStripMenuItem contextMenuItemSequentialColors = new ToolStripMenuItem("Sequential") { Image = Resources.sequentialColors };

				contextMenuItemCollections.DropDown.Items.Add(contextMenuItemColorCollections);
				contextMenuItemColorCollections.DropDown.Items.Add(contextMenuItemRandomColors);
				contextMenuItemColorCollections.DropDown.Items.Add(contextMenuItemSequentialColors);

				foreach (ColorCollection collection in _colorCollections)
				{
					if (collection.Color.Any())
					{
						ToolStripMenuItem contextMenuItemRandomColorItem = new ToolStripMenuItem(collection.Name);
						contextMenuItemRandomColorItem.ToolTipText = collection.Description;
						contextMenuItemRandomColorItem.Click += (mySender, myE) => ApplyColorCollection(collection, true);
						contextMenuItemRandomColors.DropDown.Items.Add(contextMenuItemRandomColorItem);

						ToolStripMenuItem contextMenuItemSequentialColorItem = new ToolStripMenuItem(collection.Name);
						contextMenuItemSequentialColorItem.ToolTipText = collection.Description;
						contextMenuItemSequentialColorItem.Click += (mySender, myE) => ApplyColorCollection(collection, false);
						contextMenuItemSequentialColors.DropDown.Items.Add(contextMenuItemSequentialColorItem);	
					}
				}

				if (contextMenuItemCollections.DropDownItems.Count > 0)
				{
					_contextMenuStrip.Items.Add("-");
					_contextMenuStrip.Items.Add(contextMenuItemCollections);
				}
			}
		}

		private void timelineControl_ContextSelected(object sender, ContextSelectedEventArgs e)
		{
			_contextMenuStrip.Items.Clear();

			Element element = e.ElementsUnderCursor.FirstOrDefault();
			TimedSequenceElement tse = element as TimedSequenceElement;

			#region Add Effect

			ToolStripMenuItem contextMenuItemAddEffect = new ToolStripMenuItem("Add Effect(s)") {Image = Resources.effects};
			IEnumerable<IEffectModuleDescriptor> effectDesriptors =
				ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>()
					.Cast<IEffectModuleDescriptor>()
					.OrderBy(x => x.EffectGroup)
					.ThenBy(n => n.EffectName);
			EffectGroups group = effectDesriptors.First().EffectGroup;
			foreach (IEffectModuleDescriptor effectDesriptor in effectDesriptors)
			{
				if (effectDesriptor.EffectName == "Nutcracker") continue; //Remove this when the Nutcracker module is removed
				if (effectDesriptor.EffectGroup != group)
				{
					ToolStripSeparator seperator = new ToolStripSeparator();
					contextMenuItemAddEffect.DropDownItems.Add(seperator);
					group = effectDesriptor.EffectGroup;
				}
				// Add an entry to the menu
				ToolStripMenuItem contextMenuItemEffect = new ToolStripMenuItem(effectDesriptor.EffectName);
				contextMenuItemEffect.Image = effectDesriptor.GetRepresentativeImage();
				contextMenuItemEffect.Tag = effectDesriptor.TypeId;
				contextMenuItemEffect.ToolTipText = @"Use Shift key to add multiple effects of the same type.";
				contextMenuItemEffect.Click += (mySender, myE) =>
				{
					if (e.Row != null)
					{
						//add multiple
						if (ModifierKeys == Keys.Shift || ModifierKeys == (Keys.Shift | Keys.Control))
						{
							AddMultipleEffects(e.GridTime, effectDesriptor.EffectName, (Guid) contextMenuItemEffect.Tag, e.Row);
						}
						else //add single
							AddNewEffectById((Guid) contextMenuItemEffect.Tag, e.Row, e.GridTime, GetDefaultEffectDuration(e.GridTime), true);
					}
				};

				contextMenuItemAddEffect.DropDownItems.Add(contextMenuItemEffect);
			}

			_contextMenuStrip.Items.Add(contextMenuItemAddEffect);

			#endregion

			#region Layer Section

			ConfigureLayerMenu(e);

			#endregion
		
			#region Effect Alignment Section
			
			ToolStripMenuItem contextMenuItemAlignment = new ToolStripMenuItem("Alignment")
			{
				Enabled = TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements),
				Image = Resources.alignment
			};
			//Disables the Alignment menu if too many effects are selected in a row.
			if (!contextMenuItemAlignment.Enabled)
			{
				contextMenuItemAlignment.ToolTipText = @"Disabled, maximum selected effects per row is 32.";
			}

			ToolStripMenuItem contextMenuItemAlignStart = new ToolStripMenuItem("Align Start Times")
			{
				ToolTipText = @"Holding shift will align the start times, while holding duration.",
				Image = Resources.alignStart
			};
			contextMenuItemAlignStart.Click +=
				(mySender, myE) =>
					TimelineControl.grid.AlignElementStartTimes(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
			contextMenuItemAlignStart.ShortcutKeyDisplayString = @"(Shift)+S";

			ToolStripMenuItem contextMenuItemAlignEnd = new ToolStripMenuItem("Align End Times")
			{
				ToolTipText = @"Holding shift will align the end times, while holding duration.",
				Image = Resources.alignEnd
			};
			contextMenuItemAlignEnd.Click +=
				(mySender, myE) =>
					TimelineControl.grid.AlignElementEndTimes(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
			contextMenuItemAlignEnd.ShortcutKeyDisplayString = @"(Shift)+E";

			ToolStripMenuItem contextMenuItemAlignBoth = new ToolStripMenuItem("Align Both Times") {Image = Resources.alignBoth};
			contextMenuItemAlignBoth.Click +=
				(mySender, myE) => TimelineControl.grid.AlignElementStartEndTimes(TimelineControl.SelectedElements, element);
			contextMenuItemAlignBoth.ShortcutKeyDisplayString = @"B";

			ToolStripMenuItem contextMenuItemMatchDuration = new ToolStripMenuItem("Match Duration")
			{
				ToolTipText =
					@"Holding shift will hold the effects end time and adjust the start time, by default the end time is adjusted.",
				Image = Resources.matchDuration
			};
			contextMenuItemMatchDuration.Click +=
				(mySender, myE) =>
					TimelineControl.grid.AlignElementDurations(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
			contextMenuItemMatchDuration.ShortcutKeyDisplayString = @"(Shift)";

			ToolStripMenuItem contextMenuItemAlignStartToEnd = new ToolStripMenuItem("Align Start to End")
			{
				ToolTipText =
					@"Holding shift will hold the effects end time and only adjust the start time, by default the entire effect is moved.",
				Image = Resources.alignStartEnd
			};
			contextMenuItemAlignStartToEnd.Click +=
				(mySender, myE) =>
					TimelineControl.grid.AlignElementStartToEndTimes(TimelineControl.SelectedElements, element,
						ModifierKeys == Keys.Shift);
			contextMenuItemAlignStartToEnd.ShortcutKeyDisplayString = @"(Shift)";

			ToolStripMenuItem contextMenuItemAlignEndToStart = new ToolStripMenuItem("Align End to Start")
			{
				ToolTipText =
					@"Holding shift will hold the effects start time and only adjust the end time, by default the entire effect is moved.",
				Image = Resources.alignStartEnd
			};
			contextMenuItemAlignEndToStart.Click +=
				(mySender, myE) =>
					TimelineControl.grid.AlignElementEndToStartTime(TimelineControl.SelectedElements, element,
						ModifierKeys == Keys.Shift);
			contextMenuItemAlignEndToStart.ShortcutKeyDisplayString = @"(Shift)";			

			ToolStripMenuItem contextMenuItemDistDialog = new ToolStripMenuItem("Distribute Effects")
			{
				Image = Resources.distribute
			};
			contextMenuItemDistDialog.Click += (mySender, myE) => DistributeSelectedEffects();

			ToolStripMenuItem contextMenuItemAlignCenter = new ToolStripMenuItem("Align Centerpoints")
			{
				Image = Resources.alignCenter
			};
			contextMenuItemAlignCenter.Click +=
				(mySender, myE) => TimelineControl.grid.AlignElementCenters(TimelineControl.SelectedElements, element);

			ToolStripMenuItem contextMenuItemDistributeEqually = new ToolStripMenuItem("Distribute Equally")
			{
				ToolTipText =
					@"This will stair step the selected elements, starting with the element that has the earlier start mouseLocation on the time line.",
				Image = Resources.distribute
			};
			contextMenuItemDistributeEqually.Click += (mySender, myE) => DistributeSelectedEffectsEqually();

			ToolStripMenuItem contextMenuItemAlignStartToMark = new ToolStripMenuItem("Align Start to nearest mark")
			{
				Image = Resources.alignStartMark
			};
			contextMenuItemAlignStartToMark.Click += (mySender, myE) => AlignEffectsToNearestMarks("Start");
			contextMenuItemAlignStartToMark.ShortcutKeyDisplayString = @"Ctrl+Shift+S";

			ToolStripMenuItem contextMenuItemAlignEndToMark = new ToolStripMenuItem("Align End to nearest mark")
			{
				Image = Resources.alignEndMark
			};
			contextMenuItemAlignEndToMark.Click += (mySender, myE) => AlignEffectsToNearestMarks("End");
			contextMenuItemAlignEndToMark.ShortcutKeyDisplayString = @"Ctrl+Shift+E";

			ToolStripMenuItem contextMenuItemAlignBothToMark = new ToolStripMenuItem("Align Both to nearest mark")
			{
				Image = Resources.alignBothMark
			};
			contextMenuItemAlignBothToMark.Click += (mySender, myE) => AlignEffectsToNearestMarks("Both");
			contextMenuItemAlignBothToMark.ShortcutKeyDisplayString = @"Ctrl+Shift+B";

			_contextMenuStrip.Items.Add(contextMenuItemAlignment);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignStart);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignEnd);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignBoth);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignCenter);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemMatchDuration);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignStartToEnd);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignEndToStart);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemDistributeEqually);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemDistDialog);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignStartToMark);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignEndToMark);
			contextMenuItemAlignment.DropDown.Items.Add(contextMenuItemAlignBothToMark);

			if (TimelineControl.SelectedElements.Count() > 1 || TimelineControl.SelectedElements.Any() && element != null && !element.Selected)
			{
				contextMenuItemDistributeEqually.Enabled = true;
				contextMenuItemDistDialog.Enabled = true;
				contextMenuItemAlignStart.Enabled = true;
				contextMenuItemAlignEnd.Enabled = true;
				contextMenuItemAlignBoth.Enabled = true;
				contextMenuItemAlignCenter.Enabled = true;
				contextMenuItemMatchDuration.Enabled = true;
				contextMenuItemAlignEndToStart.Enabled = true;
				contextMenuItemAlignStartToEnd.Enabled = true;
				contextMenuItemAlignment.Enabled = true;
				contextMenuItemAlignment.ToolTipText = string.Empty;
			}
			else
			{
				contextMenuItemDistributeEqually.Enabled = false;
				contextMenuItemDistDialog.Enabled = false;
				contextMenuItemAlignStart.Enabled = false;
				contextMenuItemAlignEnd.Enabled = false;
				contextMenuItemAlignBoth.Enabled = false;
				contextMenuItemAlignCenter.Enabled = false;
				contextMenuItemMatchDuration.Enabled = false;
				contextMenuItemAlignEndToStart.Enabled = false;
				contextMenuItemAlignStartToEnd.Enabled = false;
				contextMenuItemAlignment.Enabled = false;
				if (TimelineControl.SelectedElements.Count() == 1)
				{
					contextMenuItemAlignment.ToolTipText =
						@"Select more then one effect or ensure you have Marks added to enable the Alignment feature.";
				}
				else
				{
					contextMenuItemAlignment.ToolTipText = @"Select more then one effect to enable the Alignment feature.";
				}
			}

			contextMenuItemAlignStartToMark.Enabled = false;
			contextMenuItemAlignEndToMark.Enabled = false;
			contextMenuItemAlignBothToMark.Enabled = false;

			foreach (MarkCollection mc in _sequence.LabeledMarkCollections)
			{
				if (mc.Marks.Any())
				{
					contextMenuItemAlignStartToMark.Enabled = true;
					contextMenuItemAlignEndToMark.Enabled = true;
					contextMenuItemAlignBothToMark.Enabled = true;
					contextMenuItemAlignment.Enabled = true;
					contextMenuItemAlignment.ToolTipText = string.Empty;
					break;
				}
			}
			#endregion

			#region Effect Manipulation Section
			if (tse != null)
			{
				ToolStripMenuItem contextMenuItemManipulation = new ToolStripMenuItem("Manipulation");
				ToolStripMenuItem contextMenuItemManipulateDivide = new ToolStripMenuItem("Divide at cursor")
				{
					Image = Resources.divide
				};
				contextMenuItemManipulateDivide.Click += (mySender, myE) =>
				{
					if (TimelineControl.SelectedElements.Any())
					{
						TimelineControl.grid.SplitElementsAtTime(
							TimelineControl.SelectedElements.Where(elem => elem.StartTime < e.GridTime && elem.EndTime > e.GridTime)
								.ToList(), e.GridTime);
					}
					else
					{
						TimelineControl.grid.SplitElementsAtTime(new List<Element> {element}, e.GridTime);
					}
				};

				ToolStripMenuItem contextMenuItemManipulationClone = new ToolStripMenuItem("Clone") {Image = Resources.page_copy};
				contextMenuItemManipulationClone.Click += (mySender, myE) =>
				{
					if (TimelineControl.SelectedElements.Any())
					{
						CloneElements(TimelineControl.SelectedElements ?? new List<Element> {element});
					}
					else
					{
						CloneElements(new List<Element> {element});
					}
				};

				ToolStripMenuItem contextMenuItemManipulationCloneToOther = new ToolStripMenuItem("Clone to selected effects")
				{
					Image = Resources.copySelect
				};
				contextMenuItemManipulationCloneToOther.Click += (mySender, myE) =>
				{
					if (TimelineControl.SelectedElements.Any(elem => elem.EffectNode.Effect.TypeId != element.EffectNode.Effect.TypeId))
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm(string.Format(
							"Some of the selected effects are not of the same type, only effects of {0} type will be modified.",
							element.EffectNode.Effect.EffectName), @"Multiple type effect selected", false, true);
						messageBox.ShowDialog();
						if (messageBox.DialogResult == DialogResult.Cancel) return;
					}

					foreach (
						Element elem in
							TimelineControl.SelectedElements.Where(elem => elem != element)
								.Where(elem => elem.EffectNode.Effect.TypeId == element.EffectNode.Effect.TypeId))
					{
						elem.EffectNode.Effect.ParameterValues = element.EffectNode.Effect.ParameterValues;
						elem.RenderElement();
					}
				};
				contextMenuItemManipulationCloneToOther.Enabled = (TimelineControl.SelectedElements.Count() > 2);

				_contextMenuStrip.Items.Add(contextMenuItemManipulation);
				contextMenuItemManipulation.DropDown.Items.Add(contextMenuItemManipulateDivide);
				contextMenuItemManipulation.DropDown.Items.Add(contextMenuItemManipulationClone);
				contextMenuItemManipulation.DropDown.Items.Add(contextMenuItemManipulationCloneToOther);

				ToolStripMenuItem contextMenuItemEditTime = new ToolStripMenuItem("Edit Time") {Image = Resources.clock_edit};
				contextMenuItemEditTime.Click += (mySender, myE) =>
				{
					EffectTimeEditor editor = new EffectTimeEditor(tse.EffectNode.StartTime, tse.EffectNode.TimeSpan, SequenceLength);
					if (editor.ShowDialog(this) != DialogResult.OK) return;

					if (TimelineControl.SelectedElements.Any())
					{
						var elementsToMove = TimelineControl.SelectedElements.ToDictionary(elem => elem,
							elem => new Tuple<TimeSpan, TimeSpan>(editor.Start, editor.Start + editor.Duration));
						TimelineControl.grid.MoveResizeElements(elementsToMove);
					}
					else
					{
						TimelineControl.grid.MoveResizeElement(element, editor.Start, editor.Duration);
					}
				};
				//Why do we set .Tag ?
				contextMenuItemEditTime.Tag = tse;
				contextMenuItemEditTime.Enabled = TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements);
				if (!contextMenuItemEditTime.Enabled)
					contextMenuItemEditTime.ToolTipText = @"Disabled, maximum selected effects per row is 32.";
				_contextMenuStrip.Items.Add(contextMenuItemEditTime);
			}
			#endregion

			#region Cut Copy Paste Section

			_contextMenuStrip.Items.Add("-");

			ToolStripMenuItem contextMenuItemCopy = new ToolStripMenuItem("Copy", null, toolStripMenuItem_Copy_Click)
			{
				ShortcutKeyDisplayString = @"Ctrl+C",
				Image = Resources.page_copy
			};
			ToolStripMenuItem contextMenuItemCut = new ToolStripMenuItem("Cut", null, toolStripMenuItem_Cut_Click)
			{
				ShortcutKeyDisplayString = @"Ctrl+X",
				Image = Resources.cut
			};
			contextMenuItemCopy.Enabled = contextMenuItemCut.Enabled = TimelineControl.SelectedElements.Any();

			// Gets Clipboard Count to be used with some of the Pasting options.
			int clipboardCount = GetClipboardCount();
			ToolStripMenuItem contextMenuItemPaste = new ToolStripMenuItem("Paste", null, toolStripMenuItem_Paste_Click)
			{
				ShortcutKeyDisplayString = @"Ctrl+V", Image = Resources.page_white_paste,
				Enabled = clipboardCount > 0
			};
			
			// Checks if there are any visible marks that are past the mouse click position.
			bool visibleMarks = GetMarksPresent();

			ToolStripMenuItem contextMenuItemPasteToMarks = new ToolStripMenuItem("Paste - Visible Mark/s", null, toolStripMenuItem_PasteToMarks_Click)
			{
				Image = Resources.paste_marks,
				Enabled = visibleMarks && clipboardCount > 0
			};

			ToolStripMenuItem contextMenuItemPasteInvert = new ToolStripMenuItem("Paste - Invert", null, toolStripMenuItem_PasteInvert_Click)
			{
				Image = Resources.paste_invert,
				Enabled = clipboardCount > 1
			};

			_contextMenuStrip.Items.AddRange(new ToolStripItem[] {contextMenuItemCut, contextMenuItemCopy, contextMenuItemPaste});

			bool pasteSpecialSubMenu = clipboardCount > 1 && visibleMarks;

			if (pasteSpecialSubMenu)
			{
				ToolStripMenuItem contextMenuItemPasteSpecial = new ToolStripMenuItem("Paste Special")
				{
					Image = Resources.paste_special,
					Enabled = true
				};
				_contextMenuStrip.Items.Add(contextMenuItemPasteSpecial);
				contextMenuItemPasteSpecial.DropDownItems.AddRange(new ToolStripItem[] { contextMenuItemPasteInvert, contextMenuItemPasteToMarks });
			}
			else if (visibleMarks && clipboardCount > 0)
			{
				_contextMenuStrip.Items.AddRange(new ToolStripItem[] { contextMenuItemPasteToMarks });
			}
			else if (clipboardCount > 1)
			{
				_contextMenuStrip.Items.AddRange(new ToolStripItem[] { contextMenuItemPasteInvert });
			}

			if (TimelineControl.SelectedElements.Any())
			{
				//Add Delete/Collections
				ToolStripMenuItem contextMenuItemDelete = new ToolStripMenuItem("Delete Effect(s)", null,
					toolStripMenuItem_deleteElements_Click) {ShortcutKeyDisplayString = @"Del", Image = Resources.delete};
				_contextMenuStrip.Items.Add(contextMenuItemDelete);
				AddContextCollectionsMenu();
			}

			#endregion
			
			#region Mark Section

			ToolStripMenuItem contextMenuItemAddMark = new ToolStripMenuItem("Add Marks to Effects")
				{ Image = Resources.marks };
			contextMenuItemAddMark.Click += (mySender, myE) => AddMarksToSelectedEffects();
			_contextMenuStrip.Items.Add(contextMenuItemAddMark);
			
			#endregion

			e.AutomaticallyHandleSelection = false;
			_contextMenuStrip.Show(MousePosition);
		}
		
	}
}
