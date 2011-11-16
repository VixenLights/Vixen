using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Execution;
using Vixen.Sys;
using Vixen.Module.Editor;
using Vixen.Module.Sequence;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Module.Property;
using Vixen.Module.Timing;
using Vixen.Module.Media;
using CommonElements.Timeline;
using VixenModules.Sequence.Timed;
using System.Diagnostics;
using System.IO;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm : Form, IEditorUserInterface, IExecutionControl, ITiming
	{
		#region Member Variables

		// the sequence.
		private TimedSequence _sequence;

		// the program context we will be playing this sequence in: used to interact with the execution engine.
		private ProgramContext _context;

		// the timing source this sequence will be executing against. Used to update times, etc.
		private ITiming _timingSource;

		// a mapping of effects in the sequence to the element that represent them in the grid.
		private Dictionary<EffectNode, Element> _effectNodeToElement;

		// a mapping of system channels to the (possibly multiple) rows that represent them in the grid.
		private Dictionary<ChannelNode, List<Row>> _channelNodeToRows;

		// the time that was originally marked with the cursor before playback started; this is so
		// we can move the cursor to represent the current playing time, and still return to where it was.
		private TimeSpan _originalCursorPositionBeforePlayback;

		// the default time for a sequence if one is loaded with 0 time
		private static TimeSpan _defaultSequenceTime = TimeSpan.FromMinutes(1);

		// our fake, dodgy clipboard. TODO: fix using the real one, it doesn't seem to work.
		private TimelineElementsClipboardData _clipboard;

		// Undo manager
        private CommonElements.UndoManager _undoMgr;

		#endregion



        #region Undo

        private void InitUndo()
        {
            _undoMgr = new CommonElements.UndoManager();
            _undoMgr.UndoItemsChanged += _undoMgr_UndoItemsChanged;
            _undoMgr.RedoItemsChanged += _undoMgr_RedoItemsChanged;

            splitButton_Undo.Enabled = false;
            splitButton_Redo.Enabled = false;
        }

        private void splitButton_Undo_ButtonClick(object sender, EventArgs e)
        {
            _undoMgr.Undo();
        }

        private void splitButton_Redo_ButtonClick(object sender, EventArgs e)
        {
            _undoMgr.Redo();
        }



        void _undoMgr_UndoItemsChanged(object sender, EventArgs e)
        {
            if (_undoMgr.NumUndoable == 0)
            {
                splitButton_Undo.Enabled = false;
                return;
            }

            splitButton_Undo.Enabled = true;
            splitButton_Undo.DropDownItems.Clear();
            foreach (var act in _undoMgr.UndoActions)
            {
                splitButton_Undo.DropDownItems.Add(act.Description);
            }
            
        }

        void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
        {
            if (_undoMgr.NumRedoable == 0)
            {
                splitButton_Redo.Enabled = false;
                return;
            }

            splitButton_Redo.Enabled = true;
            splitButton_Redo.DropDownItems.Clear();
            foreach (var act in _undoMgr.RedoActions)
            {
                splitButton_Redo.DropDownItems.Add(act.Description);
            }
        }


        void timelineControl_ElementsMovedNew(object sender, ElementsChangedTimesEventArgs e)
        {
            var action = new ElementsTimeChangedUndoAction(e.PreviousTimes, e.Type);
            _undoMgr.AddUndoAction(action);
        }

        #endregion



        public TimedSequenceEditorForm()
		{
			InitializeComponent();

			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_channelNodeToRows = new Dictionary<ChannelNode, List<Row>>();

			timelineControl.ElementChangedRows += ElementChangedRowsHandler;
			timelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			timelineControl.CursorMoved += timelineControl_CursorMoved;

			LoadAvailableEffects();

			// JRR drag/drop
			timelineControl.DataDropped += timelineControl_DataDropped;

            InitUndo();

            timelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
		}


		private void LoadAvailableEffects()
		{
			foreach (IEffectModuleDescriptor effectDesriptor in ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>()) {
				// Add an entry to the menu
				ToolStripMenuItem menuItem = new ToolStripMenuItem(effectDesriptor.EffectName);
				menuItem.Tag = effectDesriptor.TypeId;
				menuItem.Click += (sender, e) => {
					Row destination = timelineControl.SelectedRow;
					if (destination != null) {
						addNewEffect((Guid)menuItem.Tag, destination, timelineControl.CursorPosition, TimeSpan.FromSeconds(2));		// TODO: get a proper time
					}
				};
				addEffectToolStripMenuItem.DropDownItems.Add(menuItem);

				// Add a button to the tool strip
				ToolStripItem tsItem = new ToolStripButton(effectDesriptor.EffectName);
				tsItem.Tag = effectDesriptor.TypeId;
				tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				toolStripEffects.Items.Add(tsItem);
			}
		}


	
		#region IEditorUserInterface implementation

		// TODO: what is this supposed to be for?
		public EditorValues EditorValues
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsModified { get; private set; }

		public void RefreshSequence()
		{
			Sequence = Sequence;
		}

		public void Save(string filePath = null)
		{
			SaveSequence(filePath);
		}

		public ISelection Selection
		{
			get { throw new NotImplementedException(); }
		}

		public Vixen.Sys.ISequence Sequence
		{
			get { return _sequence; }
			set
			{
				LoadSequence(value);
			}
		}

		public IEditorModuleInstance OwnerModule { get; set; }

		void IEditorUserInterface.Start()
		{
			Show();
		}

		#endregion


		#region Saving / Loading Methods

		/// <summary>
		/// Loads all nodes (groups/channels) currently in the system as rows in the timeline control.
		/// </summary>
		private void LoadSystemNodesToRows(bool clearCurrentRows = true)
		{
			_channelNodeToRows = new Dictionary<ChannelNode, List<Row>>();

			if (clearCurrentRows)
				timelineControl.ClearAllRows();

			foreach(ChannelNode node in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeAsRow(node, null);
			}
		}

		/// <summary>
		/// Adds a single given channel node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
		private void AddNodeAsRow(ChannelNode node, Row parentRow)
		{
			// made the new row from the given node and add it to the control.
			TimedSequenceRowLabel label = new TimedSequenceRowLabel();
			label.Name = node.Name;
			Row newRow = timelineControl.AddRow(label, parentRow, 32);
			newRow.ElementRemoved += ElementRemovedFromRowHandler;
			newRow.ElementAdded += ElementAddedToRowHandler;

			// Tag it with the node it refers to, and take note of which row the given channel node will refer to.
			newRow.Tag = node;
			if (_channelNodeToRows.ContainsKey(node))
				_channelNodeToRows[node].Add(newRow);
			else
				_channelNodeToRows[node] = new List<Row> { newRow };

			// iterate through all if its children, adding them as needed
			foreach (ChannelNode child in node.Children) {
				AddNodeAsRow(child, newRow);
			}
		}

		private void LoadSequence(Vixen.Sys.ISequence sequence)
		{
			// check if it's the right type of sequence that we know how to deal with. If not, complain bitterly.
			if (sequence is TimedSequence)
				_sequence = (TimedSequence)sequence;
			else {
				throw new NotImplementedException("Cannot use sequence type with a Timed Sequence Editor");
			}

			// default the sequence to 1 minute if it's not set
			if (_sequence.Length == TimeSpan.Zero)
				_sequence.Length = _defaultSequenceTime;

			SetSequenceLength(_sequence.Length);

			// update our program context with this sequence
			OpenSequenceContext(sequence);

			// clear out all the old data
			LoadSystemNodesToRows();

			// load the new data: get all the commands in the sequence, and make a new element for each of them.
			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			foreach (EffectNode node in _sequence.Data.GetEffects()) {
				AddElementForEffectNode(node);
			}

			PopulateGridWithMarks();

			IsModified = false;
		}

		private void SetSequenceLength(TimeSpan length)
		{
			if (_sequence.Length != length) {
				_sequence.Length = length;
			}

			if (timelineControl.TotalTime != length) {
				timelineControl.TotalTime = length;
			}
		}

		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for the given EffectNode.
		/// Will add a single TimedSequenceElement to in each row that each targeted channel of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="node">The EffectNode to make element(s) in the grid for.</param>
		private void AddElementForEffectNode(EffectNode node)
		{
			TimedSequenceElement element = null;
			// for the effect, make a single element and add it to every row that represents its target channels
			foreach (ChannelNode target in node.Effect.TargetNodes) {
				if (_channelNodeToRows.ContainsKey(target)) {
					List<Row> targetRows = _channelNodeToRows[target];
					// make a new element for each row that represents the channel this command is in.
					foreach (Row row in targetRows) {
						if (element == null) {
							element = new TimedSequenceElement(node);
							element.ContentChanged += ElementContentChangedHandler;
							element.TimeChanged += ElementTimeChangedHandler;

							if (!_effectNodeToElement.ContainsKey(node))
								_effectNodeToElement[node] = element;
							else
								VixenSystem.Logging.Debug("TimedSequenceEditor: Making a new element, but the map already has one!");
						}

						row.AddElement(element);
					}
				} else {
					// we don't have a row for the channel this effect is referencing; most likely, the row has
					// been deleted, or we're opening someone else's sequence, etc. Big fat TODO: here for that, then.
					// dunno what we want to do: prompt to add new channels for them? map them to others? etc.
				}
			}
		}

		/// <summary>
		/// Removes the TimedSequenceElement in the TimelineControl that refer to the given effect node.
		/// </summary>
		/// <param name="node">The EffectNode to purge the element in the grid for.</param>
		private void RemoveElementForEffectNode(EffectNode node)
		{
			Element element = _effectNodeToElement[node];
			// iterate through all rows, trying to remove the element that the given effect is represented by
			foreach (Row row in timelineControl) {
				if (row.ContainsElement(element)) {
					row.RemoveElement(element);
				}
			}

			element.ContentChanged -= ElementContentChangedHandler;
			element.TimeChanged -= ElementTimeChangedHandler;
			_effectNodeToElement.Remove(node);
		}

		/// <summary>
		/// Removes the given TimedSequenceElement in the TimelineControl.
		/// </summary>
		/// <param name="element">The element to remove.</param>
		private void RemoveElement(TimedSequenceElement element)
		{
			if (element != null)
				RemoveElementForEffectNode(element.EffectNode);
		}

		/// <summary>
		/// Removes the given TimedSequenceElements in the TimelineControl.
		/// </summary>
		/// <param name="element">The elements to remove.</param>
		private void RemoveElements(IEnumerable<TimedSequenceElement> elements)
		{
			foreach (TimedSequenceElement element in elements.ToArray())
				RemoveElement(element);
		}

		/// <summary>
		/// Saves the current sequence to a file. May prompt for a file name to save the sequence to if needed.
		/// </summary>
		/// <param name="filePath">The filename to save the sequence to. If null, the filename in the sequence will be used.
		/// If that is also null, the user will be prompted for a filename.</param>
		/// <param name="forcePrompt">If true, the user will always be prompted for a filename to save the sequence to.</param>
		private void SaveSequence(string filePath = null, bool forcePrompt = false)
		{
			if (_sequence == null) {
				VixenSystem.Logging.Error("Trying to save a sequence that is null!");
			}

			if (filePath == null | forcePrompt) {
				if (_sequence.FilePath.Trim() == "" || forcePrompt) {
					CommonElements.TextDialog prompt = new CommonElements.TextDialog("Please enter a sequence name:");
					prompt.ShowDialog();
					string extension = Path.GetExtension(prompt.Response);
					string name = Path.GetFileNameWithoutExtension(prompt.Response);

					if (name.Trim() != "") {
						// if the given extension isn't valid for this type, then keep the name intact and add an extension
						// TODO: should we pick one type? Should an editor even be able to edit multiple file types? etc...
						if (!((OwnerModule.Descriptor) as TimedSequenceEditorDescriptor).FileExtensions.Contains(extension)) {
							name = name + extension;
							extension = ((OwnerModule.Descriptor) as TimedSequenceEditorDescriptor).FileExtensions.First();
						}

						_sequence.Save(name + extension);
					} else {
						VixenSystem.Logging.Info("No name given for sequence on save, not saving.");
						return;
					}
				} else {
					_sequence.Save();
				}
			} else {
				_sequence.Save(filePath);
			}

			IsModified = false;
		}

		private void PopulateGridWithMarks()
		{
			timelineControl.ClearAllSnapTimes();

			foreach (MarkCollection mc in _sequence.MarkCollections) {
				if (mc.Enabled) {
					foreach (TimeSpan time in mc.Marks) {
						timelineControl.AddSnapTime(time, mc.Level, mc.MarkColor);
					}
				}
			}
		}

		#endregion


		#region Event Handlers

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			TimedSequenceElement element = sender as TimedSequenceElement;
			// TODO: I'm not sure if we will need to do anything here; if we are updating effect details,
			// will the EffectEditors configure the EffectNode object directly?
			IsModified = true;
		}

		protected void ElementTimeChangedHandler(object sender, EventArgs e)
		{
			//_undoMgr.AddUndoAction(new ElementTimeChangedUndoAction(

			IsModified = true;
		}

		protected void ElementRemovedFromRowHandler(object sender, ElementEventArgs e)
		{
			// not currently used
		}

		protected void ElementAddedToRowHandler(object sender, ElementEventArgs e)
		{
			// not currently used
		}

		protected void ElementChangedRowsHandler(object sender, ElementRowChangeEventArgs e)
		{
			ChannelNode oldChannel = e.OldRow.Tag as ChannelNode;
			ChannelNode newChannel = e.NewRow.Tag as ChannelNode;
			TimedSequenceElement movedElement = e.Element as TimedSequenceElement;
			//List<TimelineElement> targetElements = _effectNodeToElement[movedElement.EffectNode];

			// retarget the selected element from the old channel it was in to the new channel it was dragged to
			// TODO: there's *got* to be a better way of adding/removing a single item from an array...
			List<ChannelNode> nodeList = new List<ChannelNode>(movedElement.EffectNode.Effect.TargetNodes);
			if (nodeList.Contains(oldChannel)) {
				nodeList.Remove(oldChannel);
			} else {
				VixenSystem.Logging.Debug("TimedSequenceEditor: moving an element from " + e.OldRow.Name +
					" to " + e.NewRow.Name + "and the effect element wasn't in the old row channel!");
			}
			nodeList.Add(newChannel);
			movedElement.EffectNode.Effect.TargetNodes = nodeList.ToArray();

			// now that the effect that this element has been updated to accurately reflect the change,
			// move the actual element around. It's a single element in the grid, belonging to multiple rows:
			// so find all rows that represent the old channel, remove the element from them, and also find
			// all rows that represent the new channel and add it to them.
			foreach (Row row in timelineControl) {
				ChannelNode rowChannel = row.Tag as ChannelNode;

				if (rowChannel == oldChannel && row != e.OldRow)
					row.RemoveElement(movedElement);
				if (rowChannel == newChannel && row != e.NewRow)
					row.AddElement(movedElement);
			}

			IsModified = true;
		}

		protected void ElementDoubleClickedHandler(object sender, ElementEventArgs e)
		{
			TimedSequenceElement element = e.Element as TimedSequenceElement;

			if (element.EffectNode == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: Element double-clicked, and it doesn't have an associated effect!");
				return;
			}

			EditElement(element);
		}

		private void EditElement(TimedSequenceElement element)
		{
			if (element == null)
				return;

			using (TimedSequenceEditorEffectEditor editor = new TimedSequenceEditorEffectEditor(element.EffectNode)) {
				DialogResult result = editor.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
					IsModified = true;
			}
		}

		#endregion


		#region Sequence actions (play, pause, etc.)

		private void OpenSequenceContext(Vixen.Sys.ISequence sequence)
		{
			if (_context != null) {
				CloseSequenceContext();
			}
			_context = Execution.CreateContext(Sequence);
			_context.SequenceStarted += context_SequenceStarted;
			_context.SequenceEnded += context_SequenceEnded;
		}

		private void CloseSequenceContext()
		{
			_context.SequenceStarted -= context_SequenceStarted;
			_context.SequenceEnded -= context_SequenceEnded;
			Execution.ReleaseContext(_context);
		}

		public void PlaySequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			if (_context.IsPaused) {
				_context.Play(_timingSource.Position, TimeSpan.MaxValue);
			} else {
				_originalCursorPositionBeforePlayback = timelineControl.CursorPosition;
				_context.Play(timelineControl.CursorPosition, TimeSpan.MaxValue);
			}
		}

		public void PauseSequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			_context.Pause();
		}

		public void StopSequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			_context.Stop();

			timelineControl.CursorPosition = _originalCursorPositionBeforePlayback;
		}

		protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
		{
			timerPlaying.Start();
			_timingSource = e.TimingSource;
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
			timerPlaying.Stop();
			_timingSource = null;
		}

		protected void timerPlaying_Tick(object sender, EventArgs e)
		{
			if (_timingSource != null) {
				timelineControl.CursorPosition = _timingSource.Position;
			}
		}

		protected void timelineControl_CursorMoved(object sender, EventArgs e)
		{
			toolStripStatusLabel_currentTime.Text = timelineControl.CursorPosition.ToString("m\\:ss\\.fff");
			EnsureCursorIsVisible();
		}

		private void EnsureCursorIsVisible()
		{
			// check if the cursor position would be over 90% of the grid width: if so, scroll the grid so it would be at 50%
			// TODO: this is probably going to look a bit messy and jerky.
			if (timelineControl.CursorPosition > timelineControl.VisibleTimeStart + TimeSpan.FromMilliseconds(timelineControl.VisibleTimeSpan.TotalMilliseconds * 0.9)) {
				timelineControl.VisibleTimeStart = TimeSpan.FromMilliseconds(timelineControl.CursorPosition.TotalMilliseconds - (timelineControl.VisibleTimeSpan.TotalMilliseconds * 0.5));
			}

			if (timelineControl.CursorPosition < timelineControl.VisibleTimeStart) {
				timelineControl.VisibleTimeStart = TimeSpan.FromMilliseconds(timelineControl.CursorPosition.TotalMilliseconds - (timelineControl.VisibleTimeSpan.TotalMilliseconds * 0.2));
			}

			RestrictVisibleTimeToSequenceLength();
		}

		private void RestrictVisibleTimeToSequenceLength()
		{
			if (timelineControl.VisibleTimeStart < TimeSpan.FromSeconds(0))
				timelineControl.VisibleTimeStart = TimeSpan.FromSeconds(0);

			if (timelineControl.VisibleTimeEnd > _sequence.Length)
				timelineControl.VisibleTimeStart = _sequence.Length - timelineControl.VisibleTimeSpan;
		}


		// implementation of IExecutionControl and ITiming interfaces, for the beat track tapping.
		void IExecutionControl.Resume()
		{
			PlaySequence();
		}

		void IExecutionControl.Start()
		{
			PlaySequence();
		}

		void IExecutionControl.Pause()
		{
			PauseSequence();
		}

		void IExecutionControl.Stop()
		{
			StopSequence();
		}

		TimeSpan ITiming.Position
		{
			get { return _timingSource.Position; }
			set { }
		}

		public bool SupportsVariableSpeeds {
			get { return false; }
		}

		public float Speed {
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}

		#endregion




		#region Tool Strip Menu Items

		private void toolStripMenuItem_Save_Click(object sender, EventArgs e)
		{
			SaveSequence();
		}

		private void toolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
		{
			SaveSequence(null, true);
		}

		private void toolStripMenuItem_Close_Click(object sender, EventArgs e)
		{
			Close();
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


		#endregion


		/// <summary>
		/// Adds an effect to the sequence and timelineControl.
		/// </summary>
		private void addNewEffect(Guid effectId, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			// get a new instance of this effect, populate it, and make a node for it
			IEffectModuleInstance effect = Vixen.Sys.ApplicationServices.Get<IEffectModuleInstance>(effectId);
			addNewEffect(effect, row, startTime, timeSpan);
		}

		private void addNewEffect(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			try {
				// make sure we don't have a null effect given
				if (effectInstance == null) {
					VixenSystem.Logging.Error("TimedSequenceEditor: addNewEffect was given a null effect instance.");
					return;
				}

				// get the target channel
				ChannelNode targetNode = (ChannelNode)row.Tag;

				// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
				effectInstance.TargetNodes = new ChannelNode[] { targetNode };
				effectInstance.TimeSpan = timeSpan;
				EffectNode effectNode = new EffectNode(effectInstance, startTime);

				// put it in the sequence and in the timeline display
				_sequence.InsertData(effectNode);
				AddElementForEffectNode(effectNode);
				IsModified = true;
			} catch (Exception ex) {
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " + row.Name;
				VixenSystem.Logging.Error(msg, ex);
				MessageBox.Show(msg + ":\n" + ex.Message);
			}
		}



		#region Effect Drag/Drop

		void toolStripEffects_Item_MouseDown(object sender, MouseEventArgs e)
		{
			ToolStripItem item = sender as ToolStripItem;

			DataObject data = new DataObject(DataFormats.Serializable, item.Tag);
			item.GetCurrentParent().DoDragDrop(data, DragDropEffects.Copy);
		}

		void timelineControl_DataDropped(object sender, TimelineDropEventArgs e)
		{
			Guid effectGuid = (Guid)e.Data.GetData(DataFormats.Serializable);
			TimeSpan timeSpan = TimeSpan.FromSeconds(2.0);	// TODO: need a default value here. I suggest a per-effect default.
			addNewEffect(effectGuid, e.Row, e.Time, timeSpan);
		}

		#endregion

		private void TimedSequenceEditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			Execution.ReleaseContext(_context);
		}

		private void toolStripMenuItem_MarkManager_Click(object sender, EventArgs e)
		{
			MarkManager manager = new MarkManager(new List<MarkCollection>(_sequence.MarkCollections), this, this);
			if (manager.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				_sequence.MarkCollections = manager.MarkCollections;
				PopulateGridWithMarks();
				IsModified = true;
			}
		}

		private void toolStripMenuItem_associateAudio_Click(object sender, EventArgs e)
		{
			// for now, only allow a single Audio type media to be assocated. If they want to add another, confirm and remove it.
			HashSet<IMediaModuleInstance> modulesToRemove = new HashSet<IMediaModuleInstance>();
			foreach (IMediaModuleInstance module in _sequence.Media) {
				if (module is VixenModules.Media.Audio.Audio) {
					modulesToRemove.Add(module);
				}
			}

			if (modulesToRemove.Count > 0) {
				DialogResult result = MessageBox.Show("Only one audio file can be associated with a sequence at a time. If you choose another, " +
						"the first will be removed. Continue?", "Remove existing audio?", MessageBoxButtons.YesNoCancel);
				if (result != System.Windows.Forms.DialogResult.Yes)
					return;
			}

			// TODO: we need to be able to get the support file types, to filter the openFileDialog properly, but it's not
			// immediately obvious how to get that; for now, just let it open any file type and complain if it's wrong

			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				IMediaModuleInstance newInstance = _sequence.Media.Add(openFileDialog.FileName);
				if (newInstance == null) {
					MessageBox.Show("The selected file is not a supported type.");
					return;
				}

				// we're going ahead and adding the new audio, so remove any of the old ones we found earlier
				foreach (IMediaModuleInstance module in modulesToRemove) {
					_sequence.Media.Remove(module);
				}

				TimeSpan length = TimeSpan.Zero;
				if (newInstance is VixenModules.Media.Audio.Audio) {
					 length = (newInstance as VixenModules.Media.Audio.Audio).MediaDuration;
				}

				if (length != TimeSpan.Zero) {
					if (_sequence.Length == _defaultSequenceTime) {
						SetSequenceLength(length);
					} else {
						if (MessageBox.Show("Do you want to resize the sequence to the size of the audio?",
							"Resize sequence?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes) {
							SetSequenceLength(length);
						}
					}
				}

				IsModified = true;
			}
		}

		private void TimedSequenceEditorForm_KeyDown(object sender, KeyEventArgs e)
		{
			// do anything special we want to here: keyboard shortcuts that are in
			// the menu will be handled by them instead.
			switch (e.KeyCode) {
				default:
					break;
			}

		}

		private void ClipboardAddData(bool cutElements)
		{
			if (timelineControl.SelectedElements.Count() <= 0)
				return;

			TimelineElementsClipboardData result = new TimelineElementsClipboardData();

			int relativeVisibleRow = 0;
			int firstVisibleRow = 0;
			int i = 0;
			TimeSpan earliestTime = TimeSpan.MaxValue;
			bool foundFirstRow = false;

			foreach (Row row in timelineControl.VisibleRows) {
				foreach (Element elem in row.SelectedElements.ToArray()) {
					if (!foundFirstRow) {
						firstVisibleRow = i;
						result.FirstVisibleRow = firstVisibleRow;
						foundFirstRow = true;
					}

					relativeVisibleRow = i - firstVisibleRow;
					result.Elements.Add(elem, relativeVisibleRow);

					if (elem.StartTime < earliestTime)
						earliestTime = elem.StartTime;

					if (cutElements) {
						row.RemoveElement(elem);
					}
				}
				i++;
			}

			result.EarliestStartTime = earliestTime;

			_clipboard = result;

			// screw the clipboard. Can't get this shit working.
			//DataFormats.Format format = DataFormats.GetFormat(typeof(TimelineElementsClipboardData).FullName);
			//IDataObject dataObject = new DataObject();
			//dataObject.SetData(format.Name, false, result);
			//Clipboard.SetDataObject(dataObject, false);
			//Clipboard.SetData("TimedSequenceEditorElements", result);
		}

		private void ClipboardCut()
		{
			ClipboardAddData(true);
			IsModified = true;
		}

		private void ClipboardCopy()
		{
			ClipboardAddData(false);
		}

		private void ClipboardPaste()
		{

			TimelineElementsClipboardData data = _clipboard;

			// screw the clipboard. Can't get this shit working.
			//IDataObject dataObject = Clipboard.GetDataObject();
			//string format = typeof(TimelineElementsClipboardData).FullName;

			//if (dataObject.GetDataPresent(format)) {
			//    data = dataObject.GetData(format) as TimelineElementsClipboardData;
			//}
			////TimelineElementsClipboardData data = Clipboard.GetData("TimedSequenceEditorElements") as TimelineElementsClipboardData;

			if (data == null)
				return;

			Row targetRow = timelineControl.SelectedRow ?? timelineControl.TopVisibleRow;
			TimeSpan cursorTime = timelineControl.CursorPosition;

			List<Row> visibleRows = new List<Row>(timelineControl.VisibleRows);
			int topTargetRoxIndex = visibleRows.IndexOf(targetRow);

			foreach (KeyValuePair<Element, int> kvp in data.Elements) {
				TimedSequenceElement elem = kvp.Key as TimedSequenceElement;
				int relativeRow = kvp.Value;

				int targetRowIndex = topTargetRoxIndex + relativeRow;
				TimeSpan targetTime = elem.StartTime - data.EarliestStartTime + cursorTime;

				if (targetRowIndex >= visibleRows.Count)
					continue;

				// clone the effect, and make a new effect node for it
				IEffectModuleInstance newEffect = elem.EffectNode.Effect.Clone() as IEffectModuleInstance;
				addNewEffect(newEffect, visibleRows[targetRowIndex], targetTime, elem.Duration);
			}
		}

		private void toolStripMenuItem_Cut_Click(object sender, EventArgs e)
		{
			ClipboardCut();
		}

		private void toolStripMenuItem_Copy_Click(object sender, EventArgs e)
		{
			ClipboardCopy();
		}

		private void toolStripMenuItem_Paste_Click(object sender, EventArgs e)
		{
			ClipboardPaste();
		}

		private void toolStripMenuItem_EditEffect_Click(object sender, EventArgs e)
		{
			if (timelineControl.SelectedElements.Count() > 0) {
				EditElement(timelineControl.SelectedElements.First() as TimedSequenceElement);
			}
		}

		// this seems to break the keyboard shortcuts; the key shortcuts don't get enabled again
		// until the menu is dropped down, which is annoying. These really should be enabled/disabled
		// on select of elements, but that's too annoying for now...
		//private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		//{
		//    toolStripMenuItem_EditEffect.Enabled = timelineControl.SelectedElements.Count() > 0;
		//    toolStripMenuItem_Cut.Enabled = timelineControl.SelectedElements.Count() > 0;
		//    toolStripMenuItem_Copy.Enabled = timelineControl.SelectedElements.Count() > 0;
		//    toolStripMenuItem_Paste.Enabled = _clipboard != null;		//TODO: fix this when clipboard fixed
		//}

		private void toolStripMenuItem_zoomTimeIn_Click(object sender, EventArgs e)
		{
			timelineControl.Zoom(0.8);
		}

		private void toolStripMenuItem_zoomTimeOut_Click(object sender, EventArgs e)
		{
			timelineControl.Zoom(1.25);
		}

		private void toolStripMenuItem_zoomRowsIn_Click(object sender, EventArgs e)
		{
			timelineControl.ZoomRows(1.25);
		}

		private void toolStripMenuItem_zoomRowsOut_Click(object sender, EventArgs e)
		{
			timelineControl.ZoomRows(0.8);
		}

		private void toolStripMenuItem_deleteElements_Click(object sender, EventArgs e)
		{
			if (timelineControl.SelectedElements.Count() > 0)
				IsModified = true;

			RemoveElements(timelineControl.SelectedElements.Cast<TimedSequenceElement>());
		}

		private void toolStripButton_Start_Click(object sender, EventArgs e)
		{
			timelineControl.CursorPosition = TimeSpan.FromSeconds(0);
		}

		private void toolStripButton_End_Click(object sender, EventArgs e)
		{
			timelineControl.CursorPosition = _sequence.Length;
		}

		private void TimedSequenceEditorForm_Resize(object sender, EventArgs e)
		{
			RestrictVisibleTimeToSequenceLength();
		}




	}

	[Serializable]
	public class TimelineElementsClipboardData
	{
		public TimelineElementsClipboardData()
		{
			Elements = new Dictionary<Element, int>();
		}

		// a collection of elements and the number of rows they were below the top visible element when
		// this data was generated and placed on the clipboard.
		public Dictionary<Element, int> Elements;
		
		public int FirstVisibleRow;

		public TimeSpan EarliestStartTime;
	}
}

