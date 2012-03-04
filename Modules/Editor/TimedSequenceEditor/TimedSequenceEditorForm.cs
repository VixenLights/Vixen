using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonElements.Timeline;
using Vixen.Execution;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Sys;
using VixenModules.Sequence.Timed;

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

		private TimeSpan? m_prevPlaybackStart = null;
		private TimeSpan? m_prevPlaybackEnd = null;

		private bool m_modified = false;

		#endregion


		#region Constructor / Initialization

		public TimedSequenceEditorForm()
		{
			InitializeComponent();

			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_channelNodeToRows = new Dictionary<ChannelNode, List<Row>>();

			timelineControl.ElementChangedRows += ElementChangedRowsHandler;
			timelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
			timelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			timelineControl.DataDropped += timelineControl_DataDropped;

			timelineControl.PlaybackCurrentTimeChanged += timelineControl_PlaybackCurrentTimeChanged;

			timelineControl.RulerClicked += timelineControl_RulerClicked;
			timelineControl.RulerBeginDragTimeRange += timelineControl_RulerBeginDragTimeRange;
			timelineControl.RulerTimeRangeDragged += timelineControl_TimeRangeDragged;

			LoadAvailableEffects();
            InitUndo();
			updateButtonStates();

#if DEBUG
			ToolStripButton b = new ToolStripButton("[Debug Break]");
			b.Click += b_Click;
			toolStripOperations.Items.Add(b);
#endif
		}

#if DEBUG
		void b_Click(object sender, EventArgs e)
		{
			//Debugger.Break();

			Debug.WriteLine("***** Effects in Sequence *****");
			foreach (var x in _sequence.GetData())
				Debug.WriteLine("{0} - {1}: {2}", x.StartTime, x.EndTime, x.Effect.InstanceId);
		}
#endif


		private void LoadAvailableEffects()
		{
			foreach (IEffectModuleDescriptor effectDesriptor in ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>()) {
				// Add an entry to the menu
				ToolStripMenuItem menuItem = new ToolStripMenuItem(effectDesriptor.EffectName);
				menuItem.Tag = effectDesriptor.TypeId;
				menuItem.Click += (sender, e) => {
					Row destination = timelineControl.SelectedRow;
					if (destination != null) {
						addNewEffectById((Guid)menuItem.Tag, destination, timelineControl.CursorPosition, TimeSpan.FromSeconds(2));		// TODO: get a proper time
					}
				};
				addEffectToolStripMenuItem.DropDownItems.Add(menuItem);

				// Add a button to the tool strip
				ToolStripItem tsItem = new ToolStripButton(effectDesriptor.EffectName);
				tsItem.Tag = effectDesriptor.TypeId;
				tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				tsItem.MouseMove += toolStripEffects_Item_MouseMove;
				tsItem.Click += toolStripEffects_Item_Click;
				toolStripEffects.Items.Add(tsItem);
			}
		}

		#endregion

		#region Private Properties

		private TimeSpan SequenceLength
		{
			get { return _sequence.Length; }
			set
			{
				if (_sequence.Length != value)
				{
					_sequence.Length = value;
				}

				if (timelineControl.TotalTime != value)
				{
					timelineControl.TotalTime = value;
				}

				toolStripStatusLabel_sequenceLength.Text = _sequence.Length.ToString("m\\:ss\\.fff");
			}
		}

		#endregion


		#region Saving / Loading Methods

		/// <summary>
		/// Loads all nodes (groups/channels) currently in the system as rows in the timeline control.
		/// </summary>
		private void loadSystemNodesToRows(bool clearCurrentRows = true)
		{
			_channelNodeToRows = new Dictionary<ChannelNode, List<Row>>();

			if (clearCurrentRows)
				timelineControl.ClearAllRows();

			foreach(ChannelNode node in VixenSystem.Nodes.GetRootNodes()) {
				addNodeAsRow(node, null);
			}
		}

		private void loadSequence(Vixen.Sys.ISequence sequence)
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

			SequenceLength = _sequence.Length;

			// update our program context with this sequence
			OpenSequenceContext(sequence);

			// clear out all the old data
			loadSystemNodesToRows();

			// load the new data: get all the commands in the sequence, and make a new element for each of them.
			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			foreach (EffectNode node in _sequence.Data.GetMainStreamData()) {
				addElementForEffectNode(node);
			}

			populateGridWithMarks();

			sequenceModified();
		}

		/// <summary>
		/// Saves the current sequence to a file. May prompt for a file name to save the sequence to if needed.
		/// </summary>
		/// <param name="filePath">The filename to save the sequence to. If null, the filename in the sequence will be used.
		/// If that is also null, the user will be prompted for a filename.</param>
		/// <param name="forcePrompt">If true, the user will always be prompted for a filename to save the sequence to.</param>
		private void saveSequence(string filePath = null, bool forcePrompt = false)
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

			sequenceNotModified();
		}

		#endregion


		#region Other Private Methods

		private void populateGridWithMarks()
		{
			timelineControl.ClearAllSnapTimes();

			foreach (MarkCollection mc in _sequence.MarkCollections)
			{
				if (mc.Enabled)
				{
					foreach (TimeSpan time in mc.Marks)
					{
						timelineControl.AddSnapTime(time, mc.Level, mc.MarkColor);
					}
				}
			}
		}

		/// <summary>Called when the sequence is modified.</summary>
		private void sequenceModified()
		{
			m_modified = true;
			// TODO: Other things, like enable save button, etc.
		}

		/// <summary>Called when the sequence is no longer considered modified.</summary>
		private void sequenceNotModified()
		{
			m_modified = false;
			// TODO: Other things, like disable save button, etc.
		}

		#endregion


		#region Event Handlers

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			TimedSequenceElement element = sender as TimedSequenceElement;
			// TODO: I'm not sure if we will need to do anything here; if we are updating effect details,
			// will the EffectEditors configure the EffectNode object directly?
			sequenceModified();
		}

		protected void ElementTimeChangedHandler(object sender, EventArgs e)
		{
			sequenceModified();
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

			sequenceModified();
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
					sequenceModified();
			}
		}

		private void savePlaybackPositions()
		{
				_originalCursorPositionBeforePlayback = timelineControl.CursorPosition;
				m_prevPlaybackStart = timelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
		}


		void timelineControl_RulerClicked(object sender, RulerClickedEventArgs e)
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: StartPointClicked to Play with null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);
			
			if (autoPlay)
			{
				// Save the times for later restoration
				m_prevPlaybackStart = timelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
			}

			// Set the timeline control
			timelineControl.PlaybackStartTime = e.Time;
			timelineControl.PlaybackEndTime = null;

			if (autoPlay)
			{
				_context.Play(e.Time, TimeSpan.MaxValue);
			}
			else
			{
				timelineControl.CursorPosition = e.Time;
			}

		}

		void timelineControl_RulerBeginDragTimeRange(object sender, EventArgs e)
		{
			m_prevPlaybackStart = timelineControl.PlaybackStartTime;
			m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
		}

		void timelineControl_TimeRangeDragged(object sender, ModifierKeysEventArgs e)
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: TimeRangeDragged with null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

			if (autoPlay)
			{
				_context.Play(timelineControl.PlaybackStartTime.Value, timelineControl.PlaybackEndTime.Value);
			}
			else
			{
				// We actually want to keep this range.
				m_prevPlaybackStart = timelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
			}
		}

		#endregion


		#region Sequence actions (play, pause, etc.)

		private void OpenSequenceContext(Vixen.Sys.ISequence sequence)
		{
			if (_context != null) {
				CloseSequenceContext();
			}
			_context = (ProgramContext)VixenSystem.Contexts.CreateContext(Sequence);
			_context.SequenceStarted += context_SequenceStarted;
			_context.SequenceEnded += context_SequenceEnded;
			_context.ProgramEnded += _context_ProgramEnded;

			updateButtonStates();
		}


		private void CloseSequenceContext()
		{
			_context.SequenceStarted -= context_SequenceStarted;
			_context.SequenceEnded -= context_SequenceEnded;
			_context.ProgramEnded -= _context_ProgramEnded;
			VixenSystem.Contexts.ReleaseContext(_context);
			updateButtonStates();
		}

		public void PlaySequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused) {
				// continue execution from previous location.
				start = _timingSource.Position;
				end = TimeSpan.MaxValue;
				updateButtonStates();	// context provides no notification to/from pause state.
			} else {
				start = timelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				end = timelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
			}
			_context.Play(start, end);
		}

		public void PauseSequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Pause with null context!");
				return;
			}

			_context.Pause();
			updateButtonStates();	// context provides no notification to/from pause state.
		}

		public void StopSequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Stop with null context!");
				return;
			}

			_context.Stop();
			// button states updated by event handler.
		}

		protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
		{
			timerPlaying.Start();
			_timingSource = e.TimingSource;
			updateButtonStates();
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
			timerPlaying.Stop();
			_timingSource = null;
		}

		protected void _context_ProgramEnded(object sender, ProgramEventArgs e)
		{
			updateButtonStates();

			timelineControl.PlaybackStartTime = m_prevPlaybackStart;
			timelineControl.PlaybackEndTime = m_prevPlaybackEnd;
			timelineControl.PlaybackCurrentTime = null;
		}

		protected void timerPlaying_Tick(object sender, EventArgs e)
		{
			if (_timingSource != null) {
				timelineControl.PlaybackCurrentTime = _timingSource.Position;
			}
		}

		private void timelineControl_PlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			if (timelineControl.PlaybackCurrentTime.HasValue)
				toolStripStatusLabel_currentTime.Text = timelineControl.PlaybackCurrentTime.Value.ToString("m\\:ss\\.fff");
			else
				toolStripStatusLabel_currentTime.Text = String.Empty;
		}

		private void updateButtonStates()
		{
			if (_context == null)
			{
				toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
				toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
				toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
				return;
			}

			if (_context.IsPlaying)
			{
				if (_context.IsPaused)
				{
					toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
					toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
				}
				else
				{
					toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
					toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = true;
				}
				toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = true;
			}
			else // Stopped
			{
				toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
				toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
				toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
			}
		}

		#endregion


		#region Sequence / TimelineControl relationship management

		/// <summary>
		/// Adds an EffectNode to the sequence and the TimelineControl.
		/// </summary>
		/// <param name="node"></param>
		/// <returns>The TimedSequenceElement created and added to the TimelineControl.</returns>
		public TimedSequenceElement AddEffectNode(EffectNode node)
		{
			//Debug.WriteLine("{0}   AddEffectNode({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, node.Effect.InstanceId);
			_sequence.InsertData(node);
			return addElementForEffectNode(node);
		}


		public void RemoveEffectNodeAndElement(EffectNode node)
		{
			//Debug.WriteLine("{0}   RemoveEffectNodeAndElement(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, node.Effect.InstanceId);

			// Lookup this effect node's Timeline Element
			TimedSequenceElement tse = (TimedSequenceElement)_effectNodeToElement[node];

			foreach (Row row in timelineControl)					// Remove the element from all rows
				row.RemoveElement(tse);

			// TODO: Unnecessary?
			tse.ContentChanged -= ElementContentChangedHandler;		// Unregister event handlers
			tse.TimeChanged -= ElementTimeChangedHandler;

			_effectNodeToElement.Remove(node);						// Remove the effect node from the map
			_sequence.RemoveData(node);								// Remove the effect node from sequence
		}



		/// <summary>
		/// Creates a new effect instance, and adds it to the sequence and timelineControl.
		/// </summary>
		/// <param name="effectId">The GUID of the effect module to instantiate</param>
		/// <param name="row">The CommonElements.Timeline.Row to add the effect to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		private void addNewEffectById(Guid effectId, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			//Debug.WriteLine("{0}   addNewEffectById({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectId);
			// get a new instance of this effect, populate it, and make a node for it
			IEffectModuleInstance effect = Vixen.Sys.ApplicationServices.Get<IEffectModuleInstance>(effectId);
			addEffectInstance(effect, row, startTime, timeSpan);
		}

		/// <summary>
		/// Wraps an effect instance in an EffectNode, adds it to the sequence, and an associated element to the timeline control.
		/// </summary>
		/// <param name="effectInstance">Effect instance</param>
		/// <param name="row">CommonElements.Timeline.Row to add the effect instance to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		private void addEffectInstance(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			try {
				//Debug.WriteLine("{0}   addEffectInstance(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectInstance.InstanceId);

				// get the target channel
				ChannelNode targetNode = (ChannelNode)row.Tag;

				// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
				effectInstance.TargetNodes = new ChannelNode[] { targetNode };
				effectInstance.TimeSpan = timeSpan;
				EffectNode effectNode = new EffectNode(effectInstance, startTime);

				// put it in the sequence and in the timeline display
				TimedSequenceElement newElement = AddEffectNode(effectNode);
				sequenceModified();

				var act = new EffectsAddedUndoAction(this, new EffectNode[] { effectNode });
				_undoMgr.AddUndoAction(act);

			} catch (Exception ex) {
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " + row.Name;
				VixenSystem.Logging.Error(msg, ex);
				MessageBox.Show(msg + ":\n" + ex.Message);
			}
		}


		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for the given EffectNode.
		/// Will add a single TimedSequenceElement to in each row that each targeted channel of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="node">The EffectNode to make element(s) in the grid for.</param>
		private TimedSequenceElement addElementForEffectNode(EffectNode node)
		{
			TimedSequenceElement element = new TimedSequenceElement(node);
			element.ContentChanged += ElementContentChangedHandler;
			element.TimeChanged += ElementTimeChangedHandler;

			// for the effect, make a single element and add it to every row that represents its target channels
			foreach (ChannelNode target in node.Effect.TargetNodes)
			{
				if (_channelNodeToRows.ContainsKey(target))
				{
					// Add the element to each row that represents the channel this command is in.
					foreach (Row row in _channelNodeToRows[target])
					{
						if (!_effectNodeToElement.ContainsKey(node))
							_effectNodeToElement[node] = element;
						else
							VixenSystem.Logging.Debug("TimedSequenceEditor: Making a new element, but the map already has one!");

						row.AddElement(element);
					}
				}
				else
				{
					// we don't have a row for the channel this effect is referencing; most likely, the row has
					// been deleted, or we're opening someone else's sequence, etc. Big fat TODO: here for that, then.
					// dunno what we want to do: prompt to add new channels for them? map them to others? etc.
					string message = "No Timeline.Row is associated with a target ChannelNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
					MessageBox.Show(message);
					VixenSystem.Logging.Error(message);
				}
			}

			return element;
		}


		private void removeSelectedElements()
		{
			Element[] selected = timelineControl.SelectedElements.ToArray();

			if (selected.Length == 0)
				return;

			// Add the undo action
			var action = new EffectsRemovedUndoAction(this,
				selected.Cast<TimedSequenceElement>().Select(x => x.EffectNode)
				);
			_undoMgr.AddUndoAction(action);

			// Remove the elements (sequence and GUI)
			foreach (TimedSequenceElement elem in selected)
			{
				RemoveEffectNodeAndElement(elem.EffectNode);
			}

			sequenceModified();
		}


		/// <summary>
		/// Adds a single given channel node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
		private void addNodeAsRow(ChannelNode node, Row parentRow)
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
			foreach (ChannelNode child in node.Children)
			{
				addNodeAsRow(child, newRow);
			}
		}

		#endregion


		#region Effect Drag/Drop

		// http://sagistech.blogspot.com/2010/03/dodragdrop-prevent-doubleclick-event.html
		private bool _beginDragDrop;

		void toolStripEffects_Item_MouseDown(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
				_beginDragDrop = true;
			else
				_beginDragDrop = false;
		}

		void toolStripEffects_Item_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && _beginDragDrop)
			{
				_beginDragDrop = false;
				ToolStripItem item = sender as ToolStripItem;
				DataObject data = new DataObject(DataFormats.Serializable, item.Tag);
				item.GetCurrentParent().DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		void toolStripEffects_Item_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Currently, you must drag this item to the grid below to place an effect.");
		}

		void timelineControl_DataDropped(object sender, TimelineDropEventArgs e)
		{
			Guid effectGuid = (Guid)e.Data.GetData(DataFormats.Serializable);
			TimeSpan duration = TimeSpan.FromSeconds(2.0);	// TODO: need a default value here. I suggest a per-effect default.
            TimeSpan startTime = Util.Min(e.Time, (_sequence.Length - duration));   // Ensure the element is inside the grid.
            addNewEffectById(effectGuid, e.Row, startTime, duration);
		}

		#endregion


		#region Overridden form functions (On___)

		protected override void OnKeyDown(KeyEventArgs e)
		{
			// do anything special we want to here: keyboard shortcuts that are in
			// the menu will be handled by them instead.
			switch (e.KeyCode) {
				case Keys.Home:
					if (e.Control)
						timelineControl.VisibleTimeStart = TimeSpan.Zero;
					else
						timelineControl.VerticalOffset = 0;
					break;

				case Keys.End:
					if (e.Control)
						timelineControl.VisibleTimeStart = timelineControl.TotalTime - timelineControl.VisibleTimeSpan;
					else
						timelineControl.VerticalOffset = int.MaxValue;	// a bit iffy, but we know that the grid caps it to what's visible
					break;

				case Keys.PageUp:
					if (e.Control)
						timelineControl.VisibleTimeStart -= timelineControl.VisibleTimeSpan.Scale(0.5);
					else
						timelineControl.VerticalOffset -= (timelineControl.VisibleHeight / 2);
					break;

				case Keys.PageDown:
					if (e.Control)
						timelineControl.VisibleTimeStart += timelineControl.VisibleTimeSpan.Scale(0.5);
					else
						timelineControl.VerticalOffset += (timelineControl.VisibleHeight / 2);
					break;

				case Keys.Space:
					if (!_context.IsPlaying)
						PlaySequence();
					else {
						if (_context.IsPaused)
							PlaySequence();
						else
							StopSequence();
					}
					break;

				case Keys.Left:
					if (e.Control)
						timelineControl.MoveSelectedElementsByTime(timelineControl.TimePerPixel.Scale(-2));
					break;

				case Keys.Right:
					if (e.Control)
						timelineControl.MoveSelectedElementsByTime(timelineControl.TimePerPixel.Scale(2));
					break;

				default:
					break;
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			VixenSystem.Contexts.ReleaseContext(_context);
		}

		#endregion

		#region Clipboard

		private void ClipboardAddData(bool cutElements)
		{
			if (timelineControl.SelectedElements.Count() <= 0)
				return;

			TimelineElementsClipboardData result = new TimelineElementsClipboardData()
			{
				FirstVisibleRow = -1,
				EarliestStartTime = TimeSpan.MaxValue,
			};
			
			int rownum = 0;
			foreach (Row row in timelineControl.VisibleRows) {
				// Since removals may happen during enumeration, make a copy with ToArray().
				foreach (Element elem in row.SelectedElements.ToArray()) {
					if (result.FirstVisibleRow == -1)
						result.FirstVisibleRow = rownum;

					int relativeVisibleRow = rownum - result.FirstVisibleRow;
					
					if (result.Elements.ContainsKey(elem))
						continue;
					result.Elements.Add(elem, relativeVisibleRow);

					if (elem.StartTime < result.EarliestStartTime)
						result.EarliestStartTime = elem.StartTime;

					if (cutElements) {
						row.RemoveElement(elem);
						_sequence.RemoveData(((TimedSequenceElement)elem).EffectNode);
					}
				}
				rownum++;
			}

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
			sequenceModified();
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
				IEffectModuleInstance newEffect = (IEffectModuleInstance)elem.EffectNode.Effect.Clone();
				addEffectInstance(newEffect, visibleRows[targetRowIndex], targetTime, elem.Duration);
			}
		}

		#endregion


		#region Menu Bar

		#region Sequence Menu

		private void toolStripMenuItem_Save_Click(object sender, EventArgs e)
		{
			saveSequence();
		}

		private void toolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
		{
			saveSequence(null, true);
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

		#region Edit Menu

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

		private void toolStripMenuItem_deleteElements_Click(object sender, EventArgs e)
		{
			removeSelectedElements();
		}

		private void selectAllElementsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timelineControl.SelectAllElements();
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

		#endregion

		#region View Menu

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

		#endregion

		#region Tools Menu

		private void toolStripMenuItem_associateAudio_Click(object sender, EventArgs e)
		{
			// for now, only allow a single Audio type media to be assocated. If they want to add another, confirm and remove it.
			HashSet<IMediaModuleInstance> modulesToRemove = new HashSet<IMediaModuleInstance>();
			foreach (IMediaModuleInstance module in _sequence.GetAllMedia())
			{
				if (module is VixenModules.Media.Audio.Audio)
				{
					modulesToRemove.Add(module);
				}
			}

			if (modulesToRemove.Count > 0)
			{
				DialogResult result = MessageBox.Show("Only one audio file can be associated with a sequence at a time. If you choose another, " +
						"the first will be removed. Continue?", "Remove existing audio?", MessageBoxButtons.YesNoCancel);
				if (result != System.Windows.Forms.DialogResult.Yes)
					return;
			}

			// TODO: we need to be able to get the support file types, to filter the openFileDialog properly, but it's not
			// immediately obvious how to get that; for now, just let it open any file type and complain if it's wrong

			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				IMediaModuleInstance newInstance = _sequence.AddMedia(openFileDialog.FileName);
				if (newInstance == null)
				{
					MessageBox.Show("The selected file is not a supported type.");
					return;
				}

				// we're going ahead and adding the new audio, so remove any of the old ones we found earlier
				foreach (IMediaModuleInstance module in modulesToRemove)
				{
					_sequence.RemoveMedia(module);
				}

				TimeSpan length = TimeSpan.Zero;
				if (newInstance is VixenModules.Media.Audio.Audio)
				{
					length = (newInstance as VixenModules.Media.Audio.Audio).MediaDuration;
				}

				if (length != TimeSpan.Zero)
				{
					if (_sequence.Length == _defaultSequenceTime)
					{
						SequenceLength = length;
					}
					else
					{
						if (MessageBox.Show("Do you want to resize the sequence to the size of the audio?",
							"Resize sequence?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
						{
							SequenceLength = length;
						}
					}
				}

				sequenceModified();
			}
		}

		private void toolStripMenuItem_MarkManager_Click(object sender, EventArgs e)
		{
			MarkManager manager = new MarkManager(new List<MarkCollection>(_sequence.MarkCollections), this, this);
			if (manager.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				_sequence.MarkCollections = manager.MarkCollections;
				populateGridWithMarks();
				sequenceModified();
			}
		}

		private void modifySequenceLengthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string oldLength = _sequence.Length.ToString("m\\:ss\\.fff");
			CommonElements.TextDialog prompt = new CommonElements.TextDialog("Enter new sequence length:", "Sequence Length", oldLength, true);

            do {
                if (prompt.ShowDialog() != DialogResult.OK)
                    break;
                
                TimeSpan time;
                bool success = TimeSpan.TryParseExact(prompt.Response, TimeFormats.Formats, null, out time);
                if (success) {
                    SequenceLength = time;
                    break;
                }
                else {
                    MessageBox.Show("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time");
                }
            } while (true);
		}

		#endregion

		#endregion


		#region Toolbar buttons

		private void toolStripButton_Start_Click(object sender, EventArgs e)
		{
			timelineControl.PlaybackStartTime = TimeSpan.Zero;
			timelineControl.VisibleTimeStart = TimeSpan.Zero;
		}

		private void toolStripButton_End_Click(object sender, EventArgs e)
		{
			timelineControl.PlaybackStartTime = _sequence.Length;
			timelineControl.VisibleTimeStart = timelineControl.TotalTime - timelineControl.VisibleTimeSpan;
		}

		private void playToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PlaySequence();
		}

		private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PauseSequence();
		}

		private void stopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StopSequence();
		}

		#endregion


		#region Undo

		private void InitUndo()
		{
			_undoMgr = new CommonElements.UndoManager();
			_undoMgr.UndoItemsChanged += _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged += _undoMgr_RedoItemsChanged;

			undoButton.Enabled = false;
			undoButton.ItemChosen += undoButton_ItemChosen;

			redoButton.Enabled = false;
			redoButton.ItemChosen += redoButton_ItemChosen;
		}


		private void undoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Undo();
		}

		private void undoButton_ItemChosen(object sender, CommonElements.UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Undo(e.NumItems);
		}

		private void redoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Redo();
		}

		private void redoButton_ItemChosen(object sender, CommonElements.UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Redo(e.NumItems);
		}



		void _undoMgr_UndoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumUndoable == 0)
			{
				undoButton.Enabled = false;
				return;
			}

			undoButton.Enabled = true;
			undoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.UndoActions)
				undoButton.UndoItems.Add(act.Description);
		}

		void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumRedoable == 0)
			{
				redoButton.Enabled = false;
				return;
			}

			redoButton.Enabled = true;
			redoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.RedoActions)
				redoButton.UndoItems.Add(act.Description);
		}


		void timelineControl_ElementsMovedNew(object sender, ElementsChangedTimesEventArgs e)
		{
			var action = new ElementsTimeChangedUndoAction(e.PreviousTimes, e.Type);
			_undoMgr.AddUndoAction(action);
		}

		#endregion


		#region IEditorUserInterface implementation

		public bool IsModified { get { return m_modified; } }

		public void RefreshSequence()
		{
			Sequence = Sequence;
		}

		public void Save(string filePath = null)
		{
			saveSequence(filePath);
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
				loadSequence(value);
			}
		}

		public IEditorModuleInstance OwnerModule { get; set; }

		void IEditorUserInterface.Start()
		{
			Show();
		}

		#endregion

		#region IExecutionControl and ITiming implementation - beat tapping

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

		public bool SupportsVariableSpeeds
		{
			get { return false; }
		}

		public float Speed
		{
			get { return 1; } // 1 = 100%
			set { throw new NotSupportedException(); }
		}

		#endregion

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

	public class TimeFormats
	{
		private static string[] _formats = new string[] {
			@"m\:ss", @"m\:ss\.f", @"m\:ss\.ff", @"m\:ss\.fff", 
			@"\:ss", @"\:ss\.f", @"\:ss\.ff", @"\:ss\.fff", 
			@"%s", @"s\.f", @"s\.ff", @"s\.fff", 
		};

		public static string[] Formats
		{
			get { return _formats; }
		}
	}
}

