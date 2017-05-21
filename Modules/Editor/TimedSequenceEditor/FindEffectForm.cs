using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FindEffectForm : DockContent
	{
		private List<Element> _elements = new List<Element>();

		public TimelineControl TimelineControl { get; set; }

		private const string TimeFormat = @"hh\:mm\:ss\.fffffff";

		public FindEffectForm(TimelineControl timelineControl)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			contextMenuStrip1.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			TimelineControl = timelineControl;
			GetAllEffects();

			listViewEffectStartTime.BeginUpdate();
			listViewEffectStartTime.ColumnAutoSize();
			listViewEffectStartTime.SetLastColumnWidth();
			listViewEffectStartTime.EndUpdate();
		}

		private void comboBoxAvailableEffect_Click(object sender, EventArgs e)
		{
			GetAllEffects();
		}

		private void listViewEffectStartTime_UpdateListView(object sender, EventArgs e)
		{
			if (comboBoxAvailableEffect.SelectedItem != null)
			{
				//gets the Effect data of all Effects in the sequence of the same type as per combobox selection
				_elements.Clear();

				HashSet<string> uniqueStrings = new HashSet<string>();
				foreach (Row row in TimelineControl.Rows)
				{
					foreach (Element element in row)
					{
						//only unique effects will be added as there is no point adding the same effect just becasue it's in a different Element group
						if ((!uniqueStrings.Contains(element.EffectNode.Effect.InstanceId.ToString()) &&
						     element.EffectNode.Effect.EffectName == comboBoxAvailableEffect.SelectedItem.ToString()))
						{
							uniqueStrings.Add(element.EffectNode.Effect.InstanceId.ToString());
							_elements.Add(element);
						}
					}
				}

				_elements.Sort(); //Puts the effects into Start Time order

				//Add Effect data to listview.
				listViewEffectStartTime.BeginUpdate();
				listViewEffectStartTime.Items.Clear();

				foreach (Element element in _elements)
				{
					ListViewItem item = new ListViewItem();
					item.Text = element.EffectNode.Effect.TargetNodes[0].ToString();
					item.SubItems.Add(element.StartTime.ToString());
					item.Tag = element.EffectNode.Effect.InstanceId.ToString(); //GUID added so it can be used to find selected effects.
					listViewEffectStartTime.Items.Add(item);
				}

				listViewEffectStartTime.ColumnAutoSize();
				listViewEffectStartTime.SetLastColumnWidth();
				listViewEffectStartTime.EndUpdate();
			}
		}

		private void GetAllEffects()
		{
			//Locate all used Effects within the sequence and add to combobox.
			comboBoxAvailableEffect.Items.Clear();
			comboBoxAvailableEffect.BeginUpdate();
			HashSet<string> uniqueStrings = new HashSet<string>();

			foreach (Row row in TimelineControl.Rows)
				foreach (Element effect in row)
				{
					if (!uniqueStrings.Contains(effect.EffectNode.Effect.EffectName))
					{
						uniqueStrings.Add(effect.EffectNode.Effect.EffectName);
						comboBoxAvailableEffect.Items.Add(effect.EffectNode.Effect.EffectName);
					}
				}
			comboBoxAvailableEffect.Sorted = true; //sort effects in combobox
			comboBoxAvailableEffect.EndUpdate();
		}

		private void DisplaySelectedEffects()
		{
			TimelineControl.grid.ClearSelectedElements();
			TimeSpan effectStartTime = new TimeSpan();
			int rowVerticleOffset = 0;

			if (checkBoxCollapseAllGroups.Checked)
				TimelineControl.RowListMenuCollapse();

			foreach (ListViewItem currentEffect in listViewEffectStartTime.SelectedItems)
			{
				TimeSpan.TryParseExact(listViewEffectStartTime.Items[currentEffect.Index].SubItems[1].Text, TimeFormat,
					CultureInfo.InvariantCulture, out effectStartTime);
				Guid effectId = Guid.Parse(listViewEffectStartTime.Items[currentEffect.Index].Tag.ToString());

				foreach (var element in _elements)
				{
					if (element.EffectNode.Effect.InstanceId == effectId) //Compares Effect GUID and proceed when matched found.
					{
						element.Selected = true;
						foreach (Row row in TimelineControl.Rows)
						{
							if (row.ParentRow != null && row.Name == element.Row.Name)
							{
								//Make selected effect and any Parent nodes visible and Tree expanded.
								Row parent = row.ParentRow;
								do
								{
									if (element.Row.ParentRow != null)
									{
										parent.TreeOpen = true;
										parent.Visible = true;
										foreach (var childRows in parent.ChildRows)
										{
											childRows.Visible = true;
										}
										parent = parent.ParentRow;
									}
								} while (parent != null);
								element.EndUpdate();
							}
						}
						rowVerticleOffset = element.Row.DisplayTop;
						element.Row.Visible = true;
					}
				}
			}

			TimelineControl.VisibleTimeStart = effectStartTime; //Adjusts the Horixontal Start Time position so the last selected effect is visible
			TimelineControl.VerticalOffset = rowVerticleOffset; //Adjust the vertical grid position so the last selected effect is visible.
			TimelineControl.grid._SelectionChanged(); //Ensures Effect editor docker is updated with the Selected effects.
			TimelineControl.Refresh();
		}

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

		private void listViewEffectStartTime_MouseUp(object sender, MouseEventArgs e)
		{
			findAllSelectedEffectsToolStripMenuItem.Enabled = listViewEffectStartTime.SelectedItems.Count != 0;
		}

		private void findAllSelectedEffectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Will find the selected effects that you have selected. Can be multiple effects.
			DisplaySelectedEffects();
		}

		private void listViewEffectStartTime_DoubleClick(object sender, EventArgs e)
		{
			//Will find the selected effect that you double clicked on.
			DisplaySelectedEffects();
		}

		private void FindEffectForm_Resize(object sender, EventArgs e)
		{
			comboBoxAvailableEffect.Refresh(); //Ensure the combobox is redrawn to display correctly.
		}
	}
}
