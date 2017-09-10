using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FindEffectForm : DockContent
	{
		public TimelineControl TimelineControl { get; set; }

		private readonly SequenceLayers _layerManager;

		public FindEffectForm(TimelineControl timelineControl, SequenceLayers layerManager)
		{
			InitializeComponent();
			_layerManager = layerManager;
			contextMenuStrip1.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			TimelineControl = timelineControl;

			comboBoxFind.SelectedIndex = 0;

			listViewEffectStartTime.BeginUpdate();
			listViewEffectStartTime.ColumnAutoSize();
			listViewEffectStartTime.SetLastColumnWidth();
			listViewEffectStartTime.EndUpdate();
			ThemeUpdateControls.UpdateControls(this);
		}

		#region Methods

		private void UpdateListView()
		{
			if (comboBoxAvailableEffect.SelectedItem != null)
			{
				//gets the Effect data of all Effects in the sequence of the same type as per combobox selection
				var elements = new List<Element>();

				HashSet<string> uniqueStrings = new HashSet<string>();
				foreach (Row row in TimelineControl.Rows)
				{
					foreach (Element element in row)
					{
						if (comboBoxFind.SelectedIndex == 0) //0 is to find effects and 1 will find layers
						{
							//only unique effects will be added as there is no point adding the same effect just becasue it's in a different Element group
							if ((uniqueStrings.Contains(element.EffectNode.Effect.InstanceId.ToString()) ||
							     element.EffectNode.Effect.EffectName != comboBoxAvailableEffect.SelectedItem.ToString())) continue;
							uniqueStrings.Add(element.EffectNode.Effect.InstanceId.ToString());
							elements.Add(element);
						}
						else
						{
							if ((uniqueStrings.Contains(element.EffectNode.Effect.InstanceId.ToString()) ||
							     _layerManager.GetLayer(element.EffectNode).LayerName != comboBoxAvailableEffect.SelectedItem.ToString()))
								continue;
							uniqueStrings.Add(element.EffectNode.Effect.InstanceId.ToString());
							elements.Add(element);
						}
					}
				}

				elements.Sort(); //Puts the effects into Start Time order

				//Add Effect data to listview.
				listViewEffectStartTime.BeginUpdate();
				listViewEffectStartTime.Items.Clear();

				foreach (Element element in elements)
				{
					AddElementToListView(element);
				}

				listViewEffectStartTime.SetLastColumnWidth();
				listViewEffectStartTime.EndUpdate();
			}
		}

		private void AddElementToListView(Element element)
		{
			ListViewItem item = new ListViewItem();
			item.Text = element.Row.RowLabel.Name;
			item.SubItems.Add(element.StartTime.ToString());
			item.SubItems.Add(comboBoxFind.SelectedIndex == 0
				? _layerManager.GetLayer(element.EffectNode).LayerName
				: element.EffectNode.Effect.EffectName);
			item.Tag = element; 
			listViewEffectStartTime.Items.Add(item);
		}

		private void GetAllElements()
		{
			//Locate all used Effects within the sequence and add to combobox.
			comboBoxAvailableEffect.Items.Clear();
			comboBoxAvailableEffect.BeginUpdate();
			HashSet<string> uniqueStrings = new HashSet<string>();
			
			foreach (Row row in TimelineControl.Rows)
				foreach (Element effect in row)
				{
					if (comboBoxFind.SelectedIndex == 0) //0 is to find effects and 1 will find layers
					{
						if (uniqueStrings.Contains(effect.EffectNode.Effect.EffectName)) continue;
						uniqueStrings.Add(effect.EffectNode.Effect.EffectName);
						comboBoxAvailableEffect.Items.Add(effect.EffectNode.Effect.EffectName);
					}
					else
					{
						if (uniqueStrings.Contains(_layerManager.GetLayer(effect.EffectNode).LayerName)) continue;
						uniqueStrings.Add(_layerManager.GetLayer(effect.EffectNode).LayerName);
						comboBoxAvailableEffect.Items.Add(_layerManager.GetLayer(effect.EffectNode).LayerName);
					}
				}
			comboBoxAvailableEffect.Sorted = true; //sort effects in combobox
			comboBoxAvailableEffect.EndUpdate();
		}

		private void DisplaySelectedEffects()
		{
			if (checkBoxCollapseAllGroups.Checked)
				TimelineControl.RowListMenuCollapse();

			TimelineControl.grid.ClearSelectedElements();

			var selectedElements = new List<Element>();
			foreach (ListViewItem item in listViewEffectStartTime.SelectedItems)
			{
				selectedElements.Add(item.Tag as Element);
			}

			TimelineControl.grid.DisplaySelectedEffects(selectedElements);

		}

		#endregion

		#region Event

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

		private void comboBoxAvailableEffect_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateListView();
			listViewEffectStartTime.ColumnAutoSize();
			listViewEffectStartTime.Refresh();
		}

		private void comboBoxAvailableEffect_Click(object sender, EventArgs e)
		{
			GetAllElements();
		}

		private void listViewEffectStartTime_UpdateListView(object sender, EventArgs e)
		{
			UpdateListView();
		}
		#endregion

		private void comboBoxFind_SelectedIndexChanged(object sender, EventArgs e)
		{
			LayerEffectHeader.Text = comboBoxFind.SelectedIndex == 0 ? "Layers" : "Effects";
			GetAllElements();
			listViewEffectStartTime.Items.Clear();
		}
	}
}
