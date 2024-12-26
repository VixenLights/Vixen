using Common.Broadcast;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using Vixen.Sys.LayerMixing;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FindEffectForm : DockContent
	{
		public TimelineControl TimelineControl { get; set; }

		private readonly SequenceLayers _layerManager;
		private bool _rowEventsAdded;
		private string _searchString = string.Empty;
		private bool _findEffects = true;

		public FindEffectForm(TimelineControl timelineControl, SequenceLayers layerManager)
		{
			InitializeComponent();

			_layerManager = layerManager;
			contextMenuStrip1.Renderer = new ThemeToolStripRenderer();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			TimelineControl = timelineControl;
			timelineControl.ElementsFinishedMoving += TimelineControlOnElementsFinishedMoving;

			comboBoxFind.SelectedIndex = 0;

			Closing += FindEffectForm_Closing;
			Resize += FindEffectForm_Resize;

			// Establish automation to intercept quick keys meant for the Timeline window
			comboBoxAvailableEffect.KeyDown += Form_FindKeyDown;
			comboBoxFind.KeyDown += Form_FindKeyDown;
			listViewEffectStartTime.KeyDown += Form_FindKeyDown;
			checkBoxCollapseAllGroups.KeyDown += Form_FindKeyDown;
		}

		#region Private
		/// <summary>
		/// Intercept KeyDown event
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">Contains the event data</param>
		private void Form_FindKeyDown(object sender, KeyEventArgs e)
		{
			Broadcast.Transmit<KeyEventArgs>("KeydownSWF", e);
		}

		private void FindEffectForm_Load(object sender, EventArgs e)
		{
			ResizeColumns();
		}

		private void FindEffectForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (TimelineControl != null)
			{
				foreach (Row row in TimelineControl.Rows)
				{
					row.ElementRemoved -= RowElementAddedRemoved;
					row.ElementAdded -= RowElementAddedRemoved;
				}

				TimelineControl.ElementsFinishedMoving -= TimelineControlOnElementsFinishedMoving;
			}

			Resize -= FindEffectForm_Resize;

		}

		private void AddRowEvents()
		{
			foreach (Row row in TimelineControl.Rows)
			{
				row.ElementRemoved += RowElementAddedRemoved;
				row.ElementAdded += RowElementAddedRemoved;
			}

			_rowEventsAdded = true;
		}

		private async void RowElementAddedRemoved(object sender, ElementEventArgs e)
		{
			if (_findEffects && e.Element.EffectNode.Effect.EffectName ==
				_searchString)
			{
				if (InvokeRequired)
				{
					BeginInvoke(UpdateListView);
				}
				else
				{
					await UpdateListView();
				}
			}
		}

		private async void TimelineControlOnElementsFinishedMoving(object sender, MultiElementEventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke(UpdateListView);
			}
			else
			{
				await UpdateListView();
			}
		}
		#endregion

		#region Methods

		private void ResizeColumns()
		{
			listViewEffectStartTime.ColumnAutoSize();
			listViewEffectStartTime.SetLastColumnWidth();
		}

		private async Task UpdateListView()
		{
			if (comboBoxAvailableEffect.SelectedItem != null)
			{

				//gets the Effect data of all Effects in the sequence of the same type as per combobox selection
				var elements = new List<Element>();

				await Task.Factory.StartNew(() =>
				{
					HashSet<string> uniqueStrings = new HashSet<string>();
					foreach (Row row in TimelineControl.Rows)
					{
						foreach (Element element in row)
						{
							if (_findEffects) //0 is to find effects and 1 will find layers
							{
								//only unique effects will be added as there is no point adding the same effect just becasue it's in a different Element group
								if ((uniqueStrings.Contains(element.EffectNode.Effect.InstanceId.ToString()) ||
									 element.EffectNode.Effect.EffectName != _searchString)) continue;
								uniqueStrings.Add(element.EffectNode.Effect.InstanceId.ToString());
								elements.Add(element);
							}
							else
							{
								if ((uniqueStrings.Contains(element.EffectNode.Effect.InstanceId.ToString()) ||
									 _layerManager.GetLayer(element.EffectNode).LayerName != _searchString))
									continue;
								uniqueStrings.Add(element.EffectNode.Effect.InstanceId.ToString());
								elements.Add(element);
							}
						}
					}

					elements.Sort(); //Puts the effects into Start Time order
				});

				//Add Effect data to listview.
				listViewEffectStartTime.BeginUpdate();
				listViewEffectStartTime.Items.Clear();

				foreach (Element element in elements)
				{
					AddElementToListView(element);
				}
				ResizeColumns();
				listViewEffectStartTime.EndUpdate();


				if (!_rowEventsAdded)
				{
					AddRowEvents();
				}
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
			listViewEffectStartTime.SetLastColumnWidth();
		}

		private async void comboBoxAvailableEffect_SelectedIndexChanged(object sender, EventArgs e)
		{
			_findEffects = comboBoxFind.SelectedIndex == 0;
			_searchString = comboBoxAvailableEffect.SelectedItem.ToString();
			await UpdateListView();
		}

		private void comboBoxAvailableEffect_Click(object sender, EventArgs e)
		{
			GetAllElements();
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
