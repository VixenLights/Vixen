using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Common.Controls.Timeline;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module;
using VixenModules.Media.Audio;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using Element = Common.Controls.Timeline.Element;
using VixenModules.App.VirtualEffect;
using Vixen.Module.App;
using System.Threading.Tasks;
using System.Threading;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm : Form, IEditorUserInterface, IExecutionControl, ITiming
	{
		#region Member Variables

		// the sequence.
		private TimedSequence _sequence;

		// the program context we will be playing this sequence in: used to interact with the execution engine.
		private ISequenceContext _context;

		// the timing source this sequence will be executing against. Used to update times, etc.
		private ITiming _timingSource;

		// a mapping of effects in the sequence to the element that represent them in the grid.
		private Dictionary<EffectNode, Element> _effectNodeToElement;

		// a mapping of system elements to the (possibly multiple) rows that represent them in the grid.
		private Dictionary<ElementNode, List<Row>> _elementNodeToRows;

		// the default time for a sequence if one is loaded with 0 time
		private static TimeSpan _defaultSequenceTime = TimeSpan.FromMinutes(1);

		// Undo manager
		private Common.Controls.UndoManager _undoMgr;

		private TimeSpan? m_prevPlaybackStart = null;
		private TimeSpan? m_prevPlaybackEnd = null;

		private bool m_modified = false;

		private float _timingSpeed = 1;

		private float _timingChangeDelta = 0.25f;

		private static readonly DataFormats.Format _clipboardFormatName = DataFormats.GetFormat(typeof(TimelineElementsClipboardData).FullName);

		private VirtualEffectLibrary _virtualEffectLibrary;

		#endregion


		#region Constructor / Initialization

		public TimedSequenceEditorForm()
		{
			InitializeComponent();

			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			timelineControl.grid.RenderProgressChanged += OnRenderProgressChanged;

			timelineControl.ElementChangedRows += ElementChangedRowsHandler;
			timelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
			timelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			timelineControl.DataDropped += timelineControl_DataDropped;

			timelineControl.PlaybackCurrentTimeChanged += timelineControl_PlaybackCurrentTimeChanged;

			timelineControl.RulerClicked += timelineControl_RulerClicked;
			timelineControl.RulerBeginDragTimeRange += timelineControl_RulerBeginDragTimeRange;
			timelineControl.RulerTimeRangeDragged += timelineControl_TimeRangeDragged;

			timelineControl.SelectionChanged += TimelineControlOnSelectionChanged;
			TimeLineSequenceClipboardContentsChanged += TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			timelineControl.CursorMoved += CursorMovedHandler;
			timelineControl.ElementsSelected += timelineControl_ElementsSelected;
			timelineControl.SequenceLoading = false;

			_virtualEffectLibrary = ApplicationServices.Get<IAppModuleInstance>(VixenModules.App.VirtualEffect.VirtualEffectLibraryDescriptor.Guid) as VirtualEffectLibrary;

			LoadAvailableEffects();
			InitUndo();
			updateButtonStates();
			LoadVirtualEffects();

#if DEBUG
			ToolStripButton b = new ToolStripButton("[Debug Break]");
			b.Click += b_Click;
			toolStripOperations.Items.Add(b);
#endif
		}

#if DEBUG
		private void b_Click(object sender, EventArgs e)
		{
			//Debugger.Break();

			Debug.WriteLine("***** Effects in Sequence *****");
			foreach (var x in _sequence.SequenceData.EffectData)
				Debug.WriteLine("{0} - {1}: {2}", x.StartTime, x.EndTime, ((IEffectNode)x).Effect.InstanceId);
		}
#endif

		private void LoadVirtualEffects()
		{
			//ToolStripMenuItem menuItem = new ToolStripMenuItem(ve.Name);
			//menuItem.Tag = 0;

			if (_virtualEffectLibrary == null)
				return;

			foreach (KeyValuePair<Guid, VirtualEffect> guid in _virtualEffectLibrary) {
				ToolStripMenuItem menuItem = new ToolStripMenuItem(guid.Value.Name);
				menuItem.Tag = guid.Key;
				menuItem.Click += (sender, e) => {
					Row destination = timelineControl.SelectedRow;
					if (destination != null) {
						addNewVirtualEffectById((Guid)menuItem.Tag, destination, timelineControl.CursorPosition, TimeSpan.FromSeconds(2)); // TODO: get a proper time
					}
				};
				//addEffectToolStripMenuItem.DropDownItems.Add(menuItem);

				// Add a button to the tool strip
				ToolStripItem tsItem = new ToolStripButton(guid.Value.Name);
				tsItem.Tag = guid.Key;
				tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				tsItem.MouseMove += toolStripEffects_Item_MouseMove;
				tsItem.Click += toolStripEffects_Item_Click;
				toolStripEffects.Items.Add(tsItem);
				toolStripExVirtualEffects.Items.Add(tsItem);
			}
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
											addNewEffectById((Guid)menuItem.Tag, destination, timelineControl.CursorPosition, TimeSpan.FromSeconds(2)); // TODO: get a proper time
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
				if (_sequence.Length != value) {
					_sequence.Length = value;
				}

				if (timelineControl.TotalTime != value) {
					timelineControl.TotalTime = value;
				}

				toolStripStatusLabel_sequenceLength.Text = _sequence.Length.ToString("m\\:ss\\.fff");
			}
		}

		#endregion


		#region Saving / Loading Methods

		/// <summary>
		/// Loads all nodes (groups/elements) currently in the system as rows in the timeline control.
		/// </summary>
		private void loadSystemNodesToRows(bool clearCurrentRows = true)
		{
			timelineControl.AllowGridResize = false;
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			if (clearCurrentRows)
				timelineControl.ClearAllRows();

            foreach (ElementNode node in VixenSystem.Nodes.GetRootNodes())
            {
                addNodeAsRow(node, null);
            }
            timelineControl.LayoutRows();
            timelineControl.ResizeGrid();
		}

		void loadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            updateToolStrip4(string.Format("Please Wait. Loading: {1}", _sequence.Name, loadingWatch.Elapsed));
        }

        delegate void updateToolStrip4Delegate(string text);
        private void updateToolStrip4(string text)
		{
            if (this.InvokeRequired)
            {
                this.Invoke(new updateToolStrip4Delegate(updateToolStrip4), text);
            }
            else
            {
                this.toolStripStatusLabel4.Text = text;
                if (string.IsNullOrWhiteSpace(text))
                {
					this.Invalidate();
                    this.Enabled = true;
					this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				}
			}
		}
		System.Timers.Timer loadTimer = null;
		Stopwatch loadingWatch = null;
		private void loadSequence(Vixen.Sys.ISequence sequence)
		{
            var taskQueue = new Queue<Task>();
            
            if (loadTimer == null)
            {
				loadTimer = new System.Timers.Timer();
				loadTimer.Elapsed += loadTimer_Elapsed;
				loadTimer.Interval = 250;
			}
			loadingWatch = Stopwatch.StartNew();
			loadTimer.Enabled = true;
			timelineControl.SequenceLoading = true;

            // Let's get the window on the screen. Make it appear to instantly load.
            Invalidate(true);
            Update();

			try {
				// default the sequence to 1 minute if it's not set
				if (_sequence.Length == TimeSpan.Zero)
					_sequence.Length = _defaultSequenceTime;

				SequenceLength = _sequence.Length;

				// update our program context with this sequence
				OpenSequenceContext(sequence);

				// clear out all the old data
				loadSystemNodesToRows();
                //taskQueue.Enqueue(Task.Factory.StartNew(() => { loadSystemNodesToRows(); }));

				// load the new data: get all the commands in the sequence, and make a new element for each of them.
				_effectNodeToElement = new Dictionary<EffectNode, Element>();
                //Console.WriteLine("Loading 6: " + loadingWatch.ElapsedMilliseconds);

                // This takes quite a bit of time so queue it up
                taskQueue.Enqueue(Task.Factory.StartNew(() =>
                {
                    foreach (EffectNode node in _sequence.SequenceData.EffectData)
                    {
                        addElementForEffectNodeTPL(node);
                    }
                }));
                // Now that it is queued up, let 'er rip and start background rendering when complete.
                Task.Factory.ContinueWhenAll(taskQueue.ToArray(), completedTasks =>
                {
                    // Clear the loading toolbar
                    loadingWatch.Stop();
                    timelineControl.SequenceLoading = false;
                    loadTimer.Enabled = false;
                    updateToolStrip4(string.Empty);
                    timelineControl.grid.StartBackgroundRendering();
                    Console.WriteLine("Done Loading Effects");
                });

				populateGridWithMarks();

				var t2 = Task.Factory.StartNew(() => populateWaveformAudio());

				//This path is followed for new and existing sequences so we need to determine which we have and set modified accordingly.
				//Added logic to determine if the sequence has a filepath to set modified JU 8/1/2012. 

                _SetTimingToolStripEnabledState();

				if (String.IsNullOrEmpty(_sequence.FilePath)) {
					sequenceModified();
				} else {
					sequenceNotModified();
				}

				VixenSystem.Logging.Debug(string.Format("Sequence {0} took {1} to load. ", sequence.Name, loadingWatch.Elapsed));
			} catch (Exception ee) {
				VixenSystem.Logging.Error("Error loading sequence.", ee);
			}
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
					// Updated to use the OS SaveFileDialog functionality 8/1/2012 JU
					// Edit this type to be the more generic type to support importing into timed sequnces 12 FEB 2013 - JEMA
					EditorModuleDescriptorBase descriptor = ((OwnerModule.Descriptor) as EditorModuleDescriptorBase);
					saveFileDialog.InitialDirectory = SequenceService.SequenceDirectory;
					string filter = descriptor.TypeName + " (*" + string.Join(", *", _sequence.FileExtension) + ")|*" + string.Join("; *", _sequence.FileExtension);
					saveFileDialog.DefaultExt = _sequence.FileExtension;
					saveFileDialog.Filter = filter;
					DialogResult result = saveFileDialog.ShowDialog();
					if (result == DialogResult.OK) {
						string name = saveFileDialog.FileName;
						string extension = Path.GetExtension(saveFileDialog.FileName);

						// if the given extension isn't valid for this type, then keep the name intact and add an extension
						if (extension != _sequence.FileExtension) {
							name = name + _sequence.FileExtension;
							VixenSystem.Logging.Info("Incorrect extension provided for timed sequence, appending one.");
						}
						_sequence.Save(name);
					} else {
						//user canceled save
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

			foreach (MarkCollection mc in _sequence.MarkCollections) {
				if (mc.Enabled) {
					foreach (TimeSpan time in mc.Marks) {
						timelineControl.AddSnapTime(time, mc.Level, mc.MarkColor);
					}
				}
			}
		}

		private void populateWaveformAudio()
		{
			if (_sequence.GetAllMedia().Any()) {
				IMediaModuleInstance media = _sequence.GetAllMedia().First();
				Audio audio = media as Audio;
				timelineControl.Audio = audio;
			}
		}

		/// <summary>
		/// Called to update the title bar with the filename and saved / unsaved status
		/// </summary>
		private void setTitleBarText()
		{
            if (this.InvokeRequired)
                this.Invoke(new Vixen.Delegates.GenericDelegate(setTitleBarText));
            else
            {
                //Set sequence name in title bar based on the module name and current sequence name JU 8/1/2012
                //Made this more generic to support importing 12 FEB 2013 - JEMA
                Text = String.Format("{0} - [{1}{2}]", ((OwnerModule.Descriptor) as EditorModuleDescriptorBase).TypeName,
                                     _sequence.Name, IsModified ? " *" : "");
            }
		}

		/// <summary>Called when the sequence is modified.</summary>
		private void sequenceModified()
		{
			m_modified = true;
			setTitleBarText();
			// TODO: Other things, like enable save button, etc.
		}

		/// <summary>Called when the sequence is no longer considered modified.</summary>
		private void sequenceNotModified()
		{
			m_modified = false;
			setTitleBarText();
			// TODO: Other things, like disable save button, etc.
		}

		#endregion


		#region Event Handlers

		private void OnRenderProgressChanged(object sender, RenderElementEventArgs e)
		{
			try {
				if (!Disposing) {
                    if (e.Percent >= 0 && e.Percent <= 100)
                    {
                        toolStripProgressBar_RenderingElements.Value = e.Percent;
                    }
					if (e.Percent == 100) {
						toolStripProgressBar_RenderingElements.Visible = false;
						toolStripStatusLabel_RenderingElements.Visible = false;
					} else if (!toolStripProgressBar_RenderingElements.Visible) {
                        toolStripProgressBar_RenderingElements.Visible = true;
                        toolStripStatusLabel_RenderingElements.Visible = true;
					}
				}
			} catch {
			}
		}

		private void TimelineSequenceTimeLineSequenceClipboardContentsChanged(object sender, EventArgs eventArgs)
		{
			UpdatePasteMenuStates();
		}

		private void TimelineControlOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			toolStripButton_Copy.Enabled = toolStripButton_Cut.Enabled = timelineControl.SelectedElements.Any();
			toolStripMenuItem_Copy.Enabled = toolStripMenuItem_Cut.Enabled = timelineControl.SelectedElements.Any();
		}

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			TimedSequenceElement element = sender as TimedSequenceElement;
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
			ElementNode oldElement = e.OldRow.Tag as ElementNode;
			ElementNode newElement = e.NewRow.Tag as ElementNode;
			TimedSequenceElement movedElement = e.Element as TimedSequenceElement;
			//List<TimelineElement> targetElements = _effectNodeToElement[movedElement.EffectNode];

			// retarget the selected element from the old element it was in to the new element it was dragged to
			// TODO: there's *got* to be a better way of adding/removing a single item from an array...
			List<ElementNode> nodeList = new List<ElementNode>(movedElement.EffectNode.Effect.TargetNodes);
			if (nodeList.Contains(oldElement)) {
				nodeList.Remove(oldElement);
			} else {
				VixenSystem.Logging.Debug("TimedSequenceEditor: moving an element from " + e.OldRow.Name +
										  " to " + e.NewRow.Name + "and the effect element wasn't in the old row element!");
			}
			nodeList.Add(newElement);
			movedElement.EffectNode.Effect.TargetNodes = nodeList.ToArray();

			// now that the effect that this element has been updated to accurately reflect the change,
			// move the actual element around. It's a single element in the grid, belonging to multiple rows:
			// so find all rows that represent the old element, remove the element from them, and also find
			// all rows that represent the new element and add it to them.
			foreach (Row row in timelineControl) {
				ElementNode rowElement = row.Tag as ElementNode;

				if (rowElement == oldElement && row != e.OldRow)
					row.RemoveElement(movedElement);
				if (rowElement == newElement && row != e.NewRow)
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
			EditElements(new TimedSequenceElement[] { element });
		}

		private void EditElements(IEnumerable<TimedSequenceElement> elements)
		{
			if (elements == null)
				return;

			using (TimedSequenceEditorEffectEditor editor = new TimedSequenceEditorEffectEditor(elements.Select(x => x.EffectNode))) {
				DialogResult result = editor.ShowDialog();
                if (result == DialogResult.OK)
                {
                    foreach (Element element in elements)
                        element.Changed = true;
                    sequenceModified();
                }
			}
		}

		private void timelineControl_ElementsSelected(object sender, ElementsSelectedEventArgs e)
		{
			if (e.ElementsUnderCursor != null && e.ElementsUnderCursor.Count() > 1) {
				contextMenuStripElementSelection.Items.Clear();

				ToolStripMenuItem item;

				foreach (Element element in e.ElementsUnderCursor) {
					TimedSequenceElement tse = element as TimedSequenceElement;
					if (tse == null)
						continue;

					string name = tse.EffectNode.Effect.Descriptor.TypeName;
					name += " (" + tse.EffectNode.StartTime.ToString(@"m\:ss\.fff") + ")";
					item = new ToolStripMenuItem(name);
					item.Click += contextMenuStripElementSelectionItem_Click;
					item.Tag = tse;
					contextMenuStripElementSelection.Items.Add(item);
				}

				e.AutomaticallyHandleSelection = false;

				contextMenuStripElementSelection.Show(MousePosition);
			}
		}

		private void contextMenuStripElementSelectionItem_Click(object sender, EventArgs e)
		{
			TimedSequenceElement tse = (sender as ToolStripMenuItem).Tag as TimedSequenceElement;
			if (tse != null)
				timelineControl.SelectElement(tse);
		}

		private void timelineControl_RulerClicked(object sender, RulerClickedEventArgs e)
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: StartPointClicked to Play with null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

			if (autoPlay) {
				// Save the times for later restoration
				m_prevPlaybackStart = timelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
			} else {
				m_prevPlaybackStart = e.Time;
				m_prevPlaybackEnd = null;
			}

			// Set the timeline control
			timelineControl.PlaybackStartTime = e.Time;
			timelineControl.PlaybackEndTime = null;

			if (autoPlay) {
				_PlaySequence(e.Time, TimeSpan.MaxValue);
			} else {
				timelineControl.CursorPosition = e.Time;
			}

		}

		private void timelineControl_RulerBeginDragTimeRange(object sender, EventArgs e)
		{
			m_prevPlaybackStart = timelineControl.PlaybackStartTime;
			m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
		}

		private void timelineControl_TimeRangeDragged(object sender, ModifierKeysEventArgs e)
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: TimeRangeDragged with null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

			if (autoPlay) {
				_PlaySequence(timelineControl.PlaybackStartTime.Value, timelineControl.PlaybackEndTime.Value);
			} else {
				// We actually want to keep this range.
				m_prevPlaybackStart = timelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = timelineControl.PlaybackEndTime;
			}
		}

		#endregion

		#region Events

		//Create internal event for data being placed on clipboard as there is no outside data relevant
		//and monitoring the system clipboard gets into a bunch of not so pretty user32 api calls
		//So we will just deal with our own data. If other editors crop up that we can import data 
		//from via the clipboard, then this can be readdressed. This is mainly adding polish so we 
		//can set the enabled state of the paste menu items. JU 9/18/2012
		private static event EventHandler TimeLineSequenceClipboardContentsChanged;

		private void _TimeLineSequenceClipboardContentsChanged(EventArgs e)
		{
			if (TimeLineSequenceClipboardContentsChanged != null) {
				TimeLineSequenceClipboardContentsChanged(this, null);
			}
		}

		#endregion

		#region Sequence actions (play, pause, etc.)

		private void OpenSequenceContext(Vixen.Sys.ISequence sequence)
		{
			if (_context != null) {
				CloseSequenceContext();
			}
			//_context = (ProgramContext)VixenSystem.Contexts.CreateContext(Sequence);
			//_context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.ContextLevelCaching), Sequence);
            _context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), Sequence);
            if (_context == null)
            {
				MessageBox.Show("Unable to play this sequence.  See error log for details.");
				return;
			}
			timelineControl.grid.Context = _context;
			_context.SequenceStarted += context_SequenceStarted;
			_context.SequenceEnded += context_SequenceEnded;
			//_context.ProgramEnded += _context_ProgramEnded;
			_context.ContextEnded += context_ContextEnded;

			updateButtonStates();
		}


		private void CloseSequenceContext()
		{
			_context.SequenceStarted -= context_SequenceStarted;
			_context.SequenceEnded -= context_SequenceEnded;
			//_context.ProgramEnded -= _context_ProgramEnded;
			_context.ContextEnded -= context_ContextEnded;

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
				start = _TimingSource.Position;
				end = TimeSpan.MaxValue;
				updateButtonStates(); // context provides no notification to/from pause state.
			} else {
				start = timelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				end = timelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
			}
			_PlaySequence(start, end);
		}

		public void PlaySequenceFrom(TimeSpan StartTime)
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused) {
				// continue execution from previous location.
				start = _TimingSource.Position;
				end = TimeSpan.MaxValue;
				updateButtonStates(); // context provides no notification to/from pause state.
			} else {
				start = StartTime;
				end = timelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
				if (start >= end) {
					start = timelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				}
			}
			_PlaySequence(start, end);
		}

		public void PauseSequence()
		{
			if (_context == null) {
				VixenSystem.Logging.Error("TimedSequenceEditor: attempt to Pause with null context!");
				return;
			}

			_context.Pause();
			updateButtonStates(); // context provides no notification to/from pause state.
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
			_TimingSource = e.TimingSource;
			updateButtonStates();
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
			timerPlaying.Stop();
			_TimingSource = null;
		}

		protected void context_ContextEnded(object sender, EventArgs e)
		{
			updateButtonStates();

			timelineControl.PlaybackStartTime = m_prevPlaybackStart;
			timelineControl.PlaybackEndTime = m_prevPlaybackEnd;
			timelineControl.PlaybackCurrentTime = null;
		}

		protected void timerPlaying_Tick(object sender, EventArgs e)
		{
			if (_TimingSource != null) {
				timelineControl.PlaybackCurrentTime = _TimingSource.Position;
			}
		}

		private void timelineControl_PlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			if (timelineControl.PlaybackCurrentTime.HasValue)
				toolStripStatusLabel_currentTime.Text = timelineControl.PlaybackCurrentTime.Value.ToString("m\\:ss\\.fff");
			else
				toolStripStatusLabel_currentTime.Text = String.Empty;
		}

		private void CursorMovedHandler(object sender, EventArgs e)
		{
			toolStripStatusLabel_currentTime.Text = (e as TimeSpanEventArgs).Time.ToString("m\\:ss\\.fff");
		}

		private void UpdatePasteMenuStates()
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject != null) {
				toolStripButton_Paste.Enabled = toolStripMenuItem_Paste.Enabled = dataObject.GetDataPresent(_clipboardFormatName.Name);
			}

		}

		private void updateButtonStates()
		{
            if (this.InvokeRequired)
                this.Invoke(new Vixen.Delegates.GenericDelegate(updateButtonStates));
            else
            {
                if (_context == null)
                {
                    toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
                    toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
                    toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
                    return;
                }

                if (_context.IsRunning)
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
			//return addElementForEffectNode(node);
			return addElementForEffectNodeTPL(node);
		}


		public void RemoveEffectNodeAndElement(EffectNode node)
		{
			//Debug.WriteLine("{0}   RemoveEffectNodeAndElement(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, node.Effect.InstanceId);

			// Lookup this effect node's Timeline Element
			TimedSequenceElement tse = (TimedSequenceElement)_effectNodeToElement[node];

			foreach (Row row in timelineControl) // Remove the element from all rows
				row.RemoveElement(tse);

			// TODO: Unnecessary?
			tse.ContentChanged -= ElementContentChangedHandler; // Unregister event handlers
			tse.TimeChanged -= ElementTimeChangedHandler;

			_effectNodeToElement.Remove(node); // Remove the effect node from the map
			_sequence.RemoveData(node); // Remove the effect node from sequence
		}



		/// <summary>
		/// Creates a new effect instance, and adds it to the sequence and timelineControl.
		/// </summary>
		/// <param name="effectId">The GUID of the effect module to instantiate</param>
		/// <param name="row">The Common.Controls.Timeline.Row to add the effect to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		private void addNewEffectById(Guid effectId, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			//Debug.WriteLine("{0}   addNewEffectById({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectId);
			// get a new instance of this effect, populate it, and make a node for it

			//test if guid is in VirtualLibrary, otherwise proceed as normal
			if (_virtualEffectLibrary != null && _virtualEffectLibrary.ContainsEffect(effectId)) {
				addNewVirtualEffectById(effectId, row, startTime, timeSpan);
			} else {
				IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(effectId);
				addEffectInstance(effect, row, startTime, timeSpan);
			}
		}

		private void addNewVirtualEffectById(Guid effectId, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			if (_virtualEffectLibrary == null)
				return;

			//Debug.WriteLine("{0}   addNewEffectById({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectId);
			// get a new instance of this effect, populate it, and make a node for it
			VirtualEffect virtualEffect = _virtualEffectLibrary.GetVirtualEffect(effectId);
			IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(virtualEffect.EffectGuid);
			effect.ParameterValues = virtualEffect.VirtualParams;
			addEffectInstance(effect, row, startTime, timeSpan);
		}

		/// <summary>
		/// Wraps an effect instance in an EffectNode, adds it to the sequence, and an associated element to the timeline control.
		/// </summary>
		/// <param name="effectInstance">Effect instance</param>
		/// <param name="row">Common.Controls.Timeline.Row to add the effect instance to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		private void addEffectInstance(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime, TimeSpan timeSpan)
		{
			try {
				//Debug.WriteLine("{0}   addEffectInstance(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectInstance.InstanceId);

				// get the target element
				ElementNode targetNode = (ElementNode)row.Tag;

				// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
				effectInstance.TargetNodes = new ElementNode[] { targetNode };
				effectInstance.TimeSpan = timeSpan;
				EffectNode effectNode = new EffectNode(effectInstance, startTime);

				// put it in the sequence and in the timeline display
				TimedSequenceElement newElement = AddEffectNode(effectNode);
				sequenceModified();

				var act = new EffectsAddedUndoAction(this, new EffectNode[] { effectNode });
				_undoMgr.AddUndoAction(act);

			} catch (Exception ex) {
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " + ((row == null) ? "<null>" : row.Name);
				VixenSystem.Logging.Error(msg, ex);
			}
		}


		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for the given EffectNode.
		/// Will add a single TimedSequenceElement to in each row that each targeted element of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="node">The EffectNode to make element(s) in the grid for.</param>
		private TimedSequenceElement addElementForEffectNode(EffectNode node)
		{
			TimedSequenceElement element = new TimedSequenceElement(node);
			element.ContentChanged += ElementContentChangedHandler;
			element.TimeChanged += ElementTimeChangedHandler;

			// for the effect, make a single element and add it to every row that represents its target elements

			foreach (ElementNode target in node.Effect.TargetNodes) {
				if (_elementNodeToRows.ContainsKey(target)) {
					// Add the element to each row that represents the element this command is in.
					foreach (Row row in _elementNodeToRows[target]) {
						if (!_effectNodeToElement.ContainsKey(node))
							_effectNodeToElement[node] = element;
						//else
						//    VixenSystem.Logging.Debug("TimedSequenceEditor: Making a new element, but the map already has one!");
						//Render this effect now to get it into the cache.
						//element.EffectNode.Effect.Render();
						row.AddElement(element);
					}
				} else {
					// we don't have a row for the element this effect is referencing; most likely, the row has
					// been deleted, or we're opening someone else's sequence, etc. Big fat TODO: here for that, then.
					// dunno what we want to do: prompt to add new elements for them? map them to others? etc.
					string message = "No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
					MessageBox.Show(message);
					VixenSystem.Logging.Error(message);
				}
			}

			return element;
		}
        private TimedSequenceElement addElementForEffectNodeTPL(EffectNode node)
        {
            TimedSequenceElement element = new TimedSequenceElement(node);
            element.ContentChanged += ElementContentChangedHandler;
            element.TimeChanged += ElementTimeChangedHandler;

            // for the effect, make a single element and add it to every row that represents its target elements
            node.Effect.TargetNodes.AsParallel().WithCancellation(cancellationTokenSource.Token).ForAll(target =>
            {
                if (_elementNodeToRows.ContainsKey(target))
                {
                    // Add the element to each row that represents the element this command is in.
                    foreach (Row row in _elementNodeToRows[target])
                    {
                        if (!_effectNodeToElement.ContainsKey(node))
                            _effectNodeToElement[node] = element;
                        //else
                        //    VixenSystem.Logging.Debug("TimedSequenceEditor: Making a new element, but the map already has one!");
                        //Render this effect now to get it into the cache.
                        //element.EffectNode.Effect.Render();
                        row.AddElement(element);
                    }
                }
                else
                {
                    // we don't have a row for the element this effect is referencing; most likely, the row has
                    // been deleted, or we're opening someone else's sequence, etc. Big fat TODO: here for that, then.
                    // dunno what we want to do: prompt to add new elements for them? map them to others? etc.
                    string message = "No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
                    MessageBox.Show(message);
                    VixenSystem.Logging.Error(message);
                }
            });

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
			foreach (TimedSequenceElement elem in selected) {
				RemoveEffectNodeAndElement(elem.EffectNode);
			}

			sequenceModified();
		}


		/// <summary>
		/// Adds a single given element node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
        int doEventsCounter = 0;
		private void addNodeAsRow(ElementNode node, Row parentRow)
		{
			// made the new row from the given node and add it to the control.
			TimedSequenceRowLabel label = new TimedSequenceRowLabel();
			label.Name = node.Name;
			Row newRow = timelineControl.AddRow(label, parentRow, 32);
            newRow.ElementRemoved += ElementRemovedFromRowHandler;
			newRow.ElementAdded += ElementAddedToRowHandler;

			// Tag it with the node it refers to, and take note of which row the given element node will refer to.
			newRow.Tag = node;
            if (_elementNodeToRows.ContainsKey(node))
                _elementNodeToRows[node].Add(newRow);
            else
                _elementNodeToRows[node] = new List<Row> { newRow };

            // This slows the load down just a little, but it
            // allows the update of the load timer on the bottom of the 
            // screen so Vixen doesn't appear to be locked up for very large sequences
            if (doEventsCounter % 600 == 0)
                Application.DoEvents();
            doEventsCounter++;

			// iterate through all if its children, adding them as needed
            foreach (ElementNode child in node.Children)
            {
				addNodeAsRow(child, newRow);
			}
		}

		#endregion


		#region Effect Drag/Drop

		// http://sagistech.blogspot.com/2010/03/dodragdrop-prevent-doubleclick-event.html
		private bool _beginDragDrop;

		private void toolStripEffects_Item_MouseDown(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
				_beginDragDrop = true;
			else
				_beginDragDrop = false;
		}

		private void toolStripEffects_Item_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && _beginDragDrop) {
				_beginDragDrop = false;
				ToolStripItem item = sender as ToolStripItem;
				DataObject data = new DataObject(DataFormats.Serializable, item.Tag);
				item.GetCurrentParent().DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void toolStripEffects_Item_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Currently, you must drag this item to the grid below to place an effect.");
		}

		private void timelineControl_DataDropped(object sender, TimelineDropEventArgs e)
		{
			Guid effectGuid = (Guid)e.Data.GetData(DataFormats.Serializable);
			TimeSpan duration = TimeSpan.FromSeconds(2.0); // TODO: need a default value here. I suggest a per-effect default.
			TimeSpan startTime = Util.Min(e.Time, (_sequence.Length - duration)); // Ensure the element is inside the grid.
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
						timelineControl.VerticalOffset = int.MaxValue; // a bit iffy, but we know that the grid caps it to what's visible
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
					if (!_context.IsRunning)
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
			if (!timelineControl.SelectedElements.Any())
				return;

			TimelineElementsClipboardData result = new TimelineElementsClipboardData() {
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

					TimelineElementsClipboardData.EffectModelCandidate modelCandidate =
						new TimelineElementsClipboardData.EffectModelCandidate(((TimedSequenceElement)elem).EffectNode.Effect) {
							Duration = elem.Duration,
							StartTime = elem.StartTime
						};
					result.EffectModelCandidates.Add(modelCandidate, relativeVisibleRow);

					if (elem.StartTime < result.EarliestStartTime)
						result.EarliestStartTime = elem.StartTime;

					if (cutElements) {
						row.RemoveElement(elem);
						_sequence.RemoveData(((TimedSequenceElement)elem).EffectNode);
						sequenceModified();
					}

				}
				rownum++;
			}

			IDataObject dataObject = new DataObject(_clipboardFormatName);
			dataObject.SetData(result);
			Clipboard.SetDataObject(dataObject, false);
			_TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);

		}

		private void ClipboardCut()
		{
			ClipboardAddData(true);

		}

		private void ClipboardCopy()
		{
			ClipboardAddData(false);
		}

		public int ClipboardPaste(TimeSpan pasteTime)
		{
			int result = 0;
			TimelineElementsClipboardData data = null;
			IDataObject dataObject = Clipboard.GetDataObject();

			if (dataObject == null)
				return result;

			if (dataObject.GetDataPresent(_clipboardFormatName.Name)) {
				data = dataObject.GetData(_clipboardFormatName.Name) as TimelineElementsClipboardData;
			}

			if (data == null)
				return result;

			Row targetRow = timelineControl.SelectedRow ?? timelineControl.TopVisibleRow;

			List<Row> visibleRows = new List<Row>(timelineControl.VisibleRows);
			int topTargetRoxIndex = visibleRows.IndexOf(targetRow);

			foreach (KeyValuePair<TimelineElementsClipboardData.EffectModelCandidate, int> kvp in data.EffectModelCandidates) {
				TimelineElementsClipboardData.EffectModelCandidate effectModelCandidate = kvp.Key as TimelineElementsClipboardData.EffectModelCandidate;
				int relativeRow = kvp.Value;

				int targetRowIndex = topTargetRoxIndex + relativeRow;
				TimeSpan targetTime = effectModelCandidate.StartTime - data.EarliestStartTime + pasteTime;

				if (targetRowIndex >= visibleRows.Count)
					continue;

				//Make a new effect and populate it with the detail data from the clipboard
				IEffectModuleInstance newEffect = ApplicationServices.Get<IEffectModuleInstance>(effectModelCandidate.TypeId);
				newEffect.ModuleData = effectModelCandidate.GetEffectData();
				addEffectInstance(newEffect, visibleRows[targetRowIndex], targetTime, effectModelCandidate.Duration);
				result++;
			}

			return result;
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
			ClipboardPaste(timelineControl.CursorPosition);
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
				EditElements(timelineControl.SelectedElements.Cast<TimedSequenceElement>());
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
			foreach (IMediaModuleInstance module in _sequence.GetAllMedia()) {
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
				IMediaModuleInstance newInstance = _sequence.AddMedia(openFileDialog.FileName);
				if (newInstance == null) {
					MessageBox.Show("The selected file is not a supported type.");
					return;
				}

				// we're going ahead and adding the new audio, so remove any of the old ones we found earlier
				foreach (IMediaModuleInstance module in modulesToRemove) {
					_sequence.RemoveMedia(module);
				}
				//Remove any associated audio from the timeline.
				timelineControl.Audio = null;

				TimeSpan length = TimeSpan.Zero;
				if (newInstance is VixenModules.Media.Audio.Audio) {
					length = (newInstance as VixenModules.Media.Audio.Audio).MediaDuration;
					timelineControl.Audio = newInstance as VixenModules.Media.Audio.Audio;
				}

				_UpdateTimingSourceToSelectedMedia();

				if (length != TimeSpan.Zero) {
					if (_sequence.Length == _defaultSequenceTime) {
						SequenceLength = length;
					} else {
						if (MessageBox.Show("Do you want to resize the sequence to the size of the audio?",
										   "Resize sequence?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes) {
							SequenceLength = length;
						}
					}
				}

				sequenceModified();
			}
		}

		//*** only do this if the user agrees to do it
		private void _UpdateTimingSourceToSelectedMedia()
		{
			//This sucks so bad, I am so sorry.  Magic strings and everything, good god.
			TimingProviders timingProviders = new TimingProviders(_sequence);
			string[] mediaTimingSources;

			try {
				mediaTimingSources = timingProviders.GetAvailableTimingSources("Media");
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}

			if (mediaTimingSources.Length > 0) {
				SelectedTimingProvider mediaTimingProvider = new SelectedTimingProvider("Media", mediaTimingSources.First());
				_sequence.SelectedTimingProvider = mediaTimingProvider;
				_SetTimingToolStripEnabledState();
			}
		}

		private void toolStripMenuItem_MarkManager_Click(object sender, EventArgs e)
		{
			MarkManager manager = new MarkManager(new List<MarkCollection>(_sequence.MarkCollections), this, this, this);
			if (manager.ShowDialog() == DialogResult.OK) {
				_sequence.MarkCollections = manager.MarkCollections;
				populateGridWithMarks();
				sequenceModified();
			}
		}

		private void modifySequenceLengthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string oldLength = _sequence.Length.ToString("m\\:ss\\.fff");
			Common.Controls.TextDialog prompt = new Common.Controls.TextDialog("Enter new sequence length:", "Sequence Length", oldLength, true);

			do {
				if (prompt.ShowDialog() != DialogResult.OK)
					break;

				TimeSpan time;
				bool success = TimeSpan.TryParseExact(prompt.Response, TimeFormats.PositiveFormats, null, out time);
				if (success) {
					SequenceLength = time;
					break;
				} else {
					MessageBox.Show("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time");
				}
			} while (true);
		}

		#endregion

		#endregion


		#region Toolbar buttons

		private void toolStripButton_Start_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
			timelineControl.PlaybackStartTime = TimeSpan.Zero;
			timelineControl.VisibleTimeStart = TimeSpan.Zero;
		}

		private void toolStripButton_End_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
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
			_undoMgr = new Common.Controls.UndoManager();
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

		private void undoButton_ItemChosen(object sender, Common.Controls.UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Undo(e.NumItems);
		}

		private void redoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Redo();
		}

		private void redoButton_ItemChosen(object sender, Common.Controls.UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Redo(e.NumItems);
		}


		private void _undoMgr_UndoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumUndoable == 0) {
				undoButton.Enabled = false;
				return;
			}

			undoButton.Enabled = true;
			undoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.UndoActions)
				undoButton.UndoItems.Add(act.Description);
		}

		private void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumRedoable == 0) {
				redoButton.Enabled = false;
				return;
			}

			redoButton.Enabled = true;
			redoButton.UndoItems.Clear();
			foreach (var act in _undoMgr.RedoActions)
				redoButton.UndoItems.Add(act.Description);
		}


		private void timelineControl_ElementsMovedNew(object sender, ElementsChangedTimesEventArgs e)
		{
			var action = new ElementsTimeChangedUndoAction(e.PreviousTimes, e.Type);
			_undoMgr.AddUndoAction(action);
		}

		#endregion


		#region IEditorUserInterface implementation

		public bool IsModified
		{
			get { return m_modified; }
		}

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

				if (value is TimedSequence)
					_sequence = (TimedSequence)value;
				else {
					throw new NotImplementedException("Cannot use sequence type with a Timed Sequence Editor");
				}
				//loadSequence(value); 
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
			get { return _TimingSource.Position; }
			set { }
		}

		public bool SupportsVariableSpeeds
		{
			get { return false; }
		}

		public float Speed
		{
			get { return _timingSpeed; }
			set { _SetTimingSpeed(value); }
		}

		public bool positionHasValue
		{
			get { return timelineControl.PlaybackCurrentTime.HasValue; }
		}

		#endregion

		private void toolStripButton_IncreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed + _timingChangeDelta);
		}

		private void toolStripButton_DecreaseTimingSpeed_Click(object sender, EventArgs e)
		{
			_SetTimingSpeed(_timingSpeed - _timingChangeDelta);
		}

		private void _SetTimingSpeed(float speed)
		{
			if (speed <= 0) throw new InvalidOperationException("Cannot have a speed of 0 or less.");

			_timingSpeed = speed;

			// If they haven't executed the sequence yet, the timing source member will not yet be set.
			if (_TimingSource != null) {
				_TimingSource.Speed = _timingSpeed;
			}

			_UpdateTimingSpeedDisplay();
			toolStripButton_DecreaseTimingSpeed.Enabled = _timingSpeed > _timingChangeDelta;
		}

		private void _UpdateTimingSpeedDisplay()
		{
			toolStripLabel_TimingSpeed.Text = _timingSpeed.ToString("p0");
		}

		private void _SetTimingToolStripEnabledState()
		{
            if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericDelegate(_SetTimingToolStripEnabledState));
			else {
                ITiming timingSource = _sequence.GetTiming();
				toolStripTiming.Enabled = timingSource != null && timingSource.SupportsVariableSpeeds;
			}
		}

		private void _PlaySequence(TimeSpan rangeStart, TimeSpan rangeEnd)
		{
			if (_context.IsRunning && _context.IsPaused) {
				_context.Resume();
				updateButtonStates();
			} else {
				_context.Play(rangeStart, rangeEnd);
			}

			//_SetTimingSpeed(_timingSpeed);
		}

		private ITiming _TimingSource
		{
			get { return _timingSource; }
			set
			{
				_timingSource = value;

				if (value == null) return;

				if (_timingSource.SupportsVariableSpeeds) {
					_timingSource.Speed = _timingSpeed;
					toolStripTiming.Enabled = true;
				} else {
					_UpdateTimingSpeedDisplay();
					toolStripTiming.Enabled = false;
				}
			}
		}

		private void toolStripButtonVirtualEffectsAdd_Click(object sender, EventArgs e)
		{
			IEnumerable<Element> selectedElements = timelineControl.SelectedElements;
			switch (selectedElements.Count()) {
				case 0:
					MessageBox.Show("Please select an element to save.");
					break;
				case 1:
					TimedSequenceElement tse = (TimedSequenceElement)selectedElements.ElementAt(0);
					saveVirtualEffect(tse.EffectNode.Effect);
					break;
				default:
					MessageBox.Show("Please select only 1 element to save.");
					break;
			}
		}

		private void toolStripButtonVirtualEffectsRemove_Click(object sender, EventArgs e)
		{
			if (_virtualEffectLibrary == null)
				return;

			VirtualEffectRemoveDialog dialog = new VirtualEffectRemoveDialog(_virtualEffectLibrary);

			if (dialog.ShowDialog() == DialogResult.OK) {
				toolStripExVirtualEffects_Clear();
				foreach (Guid g in dialog.virtualEffectsToRemove) {
					_virtualEffectLibrary.removeEffect(g);
				}
				LoadVirtualEffects();
			}
		}

		private void toolStripExVirtualEffects_Clear()
		{
			if (_virtualEffectLibrary == null)
				return;

			List<ToolStripItem> addList = new List<ToolStripItem>();
			List<ToolStripItem> removeList = new List<ToolStripItem>();
			foreach (ToolStripItem tsItem in toolStripExVirtualEffects.Items) {
				if (tsItem.Tag != null && (_virtualEffectLibrary.ContainsEffect((Guid)tsItem.Tag))) {
					removeList.Add(tsItem);
				}
			}
			foreach (ToolStripItem tsItem in removeList) {
				toolStripExVirtualEffects.Items.Remove(tsItem);
			}
		}

		private void saveVirtualEffect(IEffectModuleInstance moduleInstance)
		{
			if (_virtualEffectLibrary == null)
				return;

			VirtualEffectNameDialog dialog = new VirtualEffectNameDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				_virtualEffectLibrary.addEffect(Guid.NewGuid(), dialog.effectName, moduleInstance.TypeId, moduleInstance.ParameterValues);
				toolStripExVirtualEffects_Clear();
				LoadVirtualEffects();
			}

		}
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		Task loadingTask = null;
		private void TimedSequenceEditorForm_Shown(object sender, EventArgs e)
		{
			var token = cancellationTokenSource.Token;
			this.Enabled = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			//loadingTask = Task.Factory.StartNew(() => loadSequence(_sequence), token);
			loadSequence(_sequence);
		}
	}

	[Serializable]
	internal class TimelineElementsClipboardData
	{
		public TimelineElementsClipboardData()
		{
			EffectModelCandidates = new Dictionary<EffectModelCandidate, int>();
		}

		// a collection of elements and the number of rows they were below the top visible element when
		// this data was generated and placed on the clipboard.
		public Dictionary<EffectModelCandidate, int> EffectModelCandidates { get; set; }

		public int FirstVisibleRow
		{
			get;
			set;
		}

		public TimeSpan EarliestStartTime { get; set; }

		/// <summary>
		/// Class to hold effect data to allow it to be placed on the clipboard and be reconstructed when later pasted
		/// </summary>
		[Serializable]
		public class EffectModelCandidate
		{
			private readonly Type _moduleDataClass;
			private readonly MemoryStream _effectData;

			public EffectModelCandidate(IEffectModuleInstance effect)
			{
				_moduleDataClass = effect.Descriptor.ModuleDataClass;
				DataContractSerializer ds = new DataContractSerializer(_moduleDataClass);

				TypeId = effect.Descriptor.TypeId;
				_effectData = new MemoryStream();
				using (XmlDictionaryWriter w = XmlDictionaryWriter.CreateBinaryWriter(_effectData))
					ds.WriteObject(w, effect.ModuleData);
			}

			public TimeSpan StartTime { get; set; }
			public TimeSpan Duration { get; set; }
			public Guid TypeId { get; private set; }

			public IModuleDataModel GetEffectData()
			{
				DataContractSerializer ds = new DataContractSerializer(_moduleDataClass);
				MemoryStream effectDataIn = new MemoryStream(_effectData.ToArray());
				using (XmlDictionaryReader r = XmlDictionaryReader.CreateBinaryReader(effectDataIn, XmlDictionaryReaderQuotas.Max))
					return (IModuleDataModel)ds.ReadObject(r);

			}

		}
	}

	public class TimeFormats
	{
		private static readonly string[] _positiveFormats = new string[] {
			@"m\:ss", @"m\:ss\.f", @"m\:ss\.ff", @"m\:ss\.fff", 
			@"\:ss", @"\:ss\.f", @"\:ss\.ff", @"\:ss\.fff", 
			@"%s", @"s\.f", @"s\.ff", @"s\.fff", 
		};

		private static readonly string[] _negativeFormats = new string[] {
			@"\-m\:ss", @"\-m\:ss\.f", @"\-m\:ss\.ff", @"\-m\:ss\.fff", 
			@"\-\:ss", @"\-\:ss\.f", @"\-\:ss\.ff", @"\-\:ss\.fff", 
			@"\-%s", @"\-s\.f", @"\-s\.ff", @"\-s\.fff", 
		};

		public static string[] AllFormats
		{
			get { return _negativeFormats.Concat(_positiveFormats).ToArray(); }
		}

		public static string[] PositiveFormats
		{
			get { return _positiveFormats; }
		}

		public static string[] NegativeFormats
		{
			get { return _negativeFormats; }
		}
	}
}

