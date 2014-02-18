using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using Common.Controls.Timeline;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module;
using VixenModules.App.Curves;
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
using Common.Resources.Properties;
using Common.Controls;
using WeifenLuo.WinFormsUI.Docking;
using Vixen.Sys.State.Execution;

namespace VixenModules.Editor.TimedSequenceEditor
{

	public partial class TimedSequenceEditorForm : Form, IEditorUserInterface, IExecutionControl, ITiming
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		#region Member Variables

		// the sequence.
		private TimedSequence _sequence;
		
		// the program context we will be playing this sequence in: used to interact with the execution engine.
		private ISequenceContext _context;

		// the timing source this sequence will be executing against. Used to update times, etc.
		private ITiming _timingSource;

		// Delayed play countdown
		private int DelayCountDown;

		// Did the user use the stop button
		private bool stoppedByUser = false;

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

		private static readonly DataFormats.Format _clipboardFormatName =
			DataFormats.GetFormat(typeof(TimelineElementsClipboardData).FullName);

		private VirtualEffectLibrary _virtualEffectLibrary;

		private ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

		private string settingsPath;

		#endregion

		#region Constructor / Initialization

		public TimedSequenceEditorForm()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			toolStripButton_Start.Image = Resources.control_start_blue;
			toolStripButton_Start.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Play.Image = Resources.control_play_blue;
			toolStripButton_Play.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Stop.Image = Resources.control_stop_blue;
			toolStripButton_Stop.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Pause.Image = Resources.control_pause_blue;
			toolStripButton_Pause.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_End.Image = Resources.control_end_blue;
			toolStripButton_End.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Loop.Image = Resources.arrow_repeat;
			toolStripButton_Loop.DisplayStyle = ToolStripItemDisplayStyle.Image;
			undoButton.Image = Resources.arrow_undo;
			undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.Image = Resources.arrow_redo;
			redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			redoButton.ButtonType = UndoButtonType.RedoButton;
			toolStripButton_Cut.Image = Resources.cut;
			toolStripButton_Cut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Copy.Image = Resources.page_white_copy;
			toolStripButton_Copy.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_Paste.Image = Resources.page_white_paste;
			toolStripButton_Paste.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_AssociateAudio.Image = Resources.music;
			toolStripButton_AssociateAudio.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_MarkManager.Image = Resources.timeline_marker;
			toolStripButton_MarkManager.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_ZoomTimeIn.Image = Resources.zoom_in;
			toolStripButton_ZoomTimeIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_ZoomTimeOut.Image = Resources.zoom_out;
			toolStripButton_ZoomTimeOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_SnapTo.Image = Resources.magnet;
			toolStripButton_SnapTo.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_IncreaseTimingSpeed.Image = Resources.plus;
			toolStripButton_IncreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_DecreaseTimingSpeed.Image = Resources.minus;
			toolStripButton_DecreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;

			Execution.ExecutionStateChanged += OnExecutionStateChanged;
		}

		private IDockContent DockingPanels_GetContentFromPersistString(string persistString)
		{
			if (persistString == typeof(Form_Effects).ToString())
				return EffectsForm;
			else if (persistString == typeof(Form_Grid).ToString())
				return GridForm;
			else if (persistString == typeof(Form_Marks).ToString())
				return MarksForm;
			else
			{
				throw new NotImplementedException("Unable to find docking window type.");
			}
		}

		private void TimedSequenceEditorForm_Load(object sender, EventArgs e)
		{
			settingsPath =
				System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "Vixen",
							   "TimedSequenceEditorForm.xml");
			if (System.IO.File.Exists(settingsPath))
			{
				dockPanel.LoadFromXml(settingsPath, new DeserializeDockContent(DockingPanels_GetContentFromPersistString));
			}
			else
			{
				GridForm.Show(dockPanel);
				MarksForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
				EffectsForm.Show(dockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
			}

			XMLProfileSettings xml = new XMLProfileSettings();
			dockPanel.DockLeftPortion = xml.GetSetting(string.Format("{0}/DockLeftPortion", this.Name), 150);
			dockPanel.DockRightPortion = xml.GetSetting(string.Format("{0}/DockLeftPortion", this.Name), 150);
			xml = null;

			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			TimelineControl.grid.RenderProgressChanged += OnRenderProgressChanged;

			TimelineControl.ElementChangedRows += ElementChangedRowsHandler;
			TimelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
			TimelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			TimelineControl.DataDropped += timelineControl_DataDropped;

			TimelineControl.PlaybackCurrentTimeChanged += timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked += timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange += timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged += timelineControl_TimeRangeDragged;

			TimelineControl.MarkMoved += timelineControl_MarkMoved;
			TimelineControl.DeleteMark += timelineControl_DeleteMark;

			MarksForm.MarkCollectionChecked += MarkCollection_Checked;
			MarksForm.EditMarkCollection += MarkCollection_Edit;
			MarksForm.ChangedMarkCollection += MarkCollection_Changed;

			TimelineControl.SelectionChanged += TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown += TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged += TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			TimelineControl.CursorMoved += CursorMovedHandler;
			TimelineControl.ElementsSelected += timelineControl_ElementsSelected;
			TimelineControl.ContextSelected += timelineControl_ContextSelected;
			TimelineControl.SequenceLoading = false;

			_virtualEffectLibrary =
				ApplicationServices.Get<IAppModuleInstance>(VixenModules.App.VirtualEffect.VirtualEffectLibraryDescriptor.Guid) as
				VirtualEffectLibrary;

			LoadAvailableEffects();
			InitUndo();
			updateButtonStates();
			UpdatePasteMenuStates();
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
				Debug.WriteLine("{0} - {1}: {2}", x.StartTime, x.EndTime, ((IEffectNode) x).Effect.InstanceId);
		}
#endif
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (loadingTask != null && !loadingTask.IsCompleted && !loadingTask.IsFaulted && !loadingTask.IsCanceled)
			{
				cancellationTokenSource.Cancel();
			}


			//TimelineControl.grid.RenderProgressChanged -= OnRenderProgressChanged;

			TimelineControl.ElementChangedRows -= ElementChangedRowsHandler;
			TimelineControl.ElementsMovedNew -= timelineControl_ElementsMovedNew;
			TimelineControl.ElementDoubleClicked -= ElementDoubleClickedHandler;
			TimelineControl.DataDropped -= timelineControl_DataDropped;

			TimelineControl.PlaybackCurrentTimeChanged -= timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked -= timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange -= timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged -= timelineControl_TimeRangeDragged;
			TimelineControl.MarkMoved -= timelineControl_MarkMoved;
			TimelineControl.DeleteMark -= timelineControl_DeleteMark;

			MarksForm.EditMarkCollection -= MarkCollection_Edit;
			MarksForm.MarkCollectionChecked -= MarkCollection_Checked;
			MarksForm.ChangedMarkCollection -= MarkCollection_Changed;
			MarksForm.Dispose();

			TimelineControl.SelectionChanged -= TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown -= TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged -= TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			TimelineControl.CursorMoved -= CursorMovedHandler;
			TimelineControl.ElementsSelected -= timelineControl_ElementsSelected;
			TimelineControl.ContextSelected -= timelineControl_ContextSelected;

			Execution.ExecutionStateChanged -= OnExecutionStateChanged;

			EffectsForm.Dispose();
			
			//;
			if (disposing && (components != null))
			{
				components.Dispose();
				TimelineControl.Dispose();
				GridForm.Dispose();

			}
			if (_effectNodeToElement != null)
			{
				_effectNodeToElement.Clear();
				_effectNodeToElement = null;
			}
			if (_elementNodeToRows != null)
			{
				_elementNodeToRows.Clear();
				_elementNodeToRows = null;
			}
			if (_sequence != null)
			{
				_sequence.Dispose();
				_sequence = null;
			}

			dockPanel.Dispose();

			base.Dispose(disposing);
			GC.Collect();
		}

		private Form_Effects _effectsForm = null;
		public Form_Effects EffectsForm
		{
			get
			{
				if (_effectsForm != null && !_effectsForm.IsDisposed)
				{
					return _effectsForm;
				}
				else
				{
					_effectsForm = new Form_Effects(TimelineControl);
					return _effectsForm;
				}
			}
		}

		private Form_Marks _marksForm = null;
		public Form_Marks MarksForm
		{
			get
			{
				if (_marksForm != null && !_marksForm.IsDisposed)
				{
					return _marksForm;
				}
				else
				{
					_marksForm = new Form_Marks(TimelineControl);
					return _marksForm;
				}
			}
		}

		private void MarkCollection_Checked(object sender, MarkCollectionArgs e)
		{
			populateGridWithMarks();
		}

		private void MarkCollection_Edit(Object sender, EventArgs e)
		{
			ShowMarkManager();
		}

		private void MarkCollection_Changed(Object sender, MarkCollectionArgs e)
		{
			sequenceModified();
		}

		private Form_Grid _gridForm = null;
		public Form_Grid GridForm
		{
			get
			{
				return _gridForm != null ? _gridForm : _gridForm = new Form_Grid();
			}
		}

		public TimelineControl TimelineControl
		{
			get
			{
				return _gridForm.TimelineControl;
			}
		}

		private void LoadVirtualEffects()
		{
			//ToolStripMenuItem menuItem = new ToolStripMenuItem(ve.Name);
			//menuItem.Tag = 0;

			if (_virtualEffectLibrary == null)
				return;

			foreach (KeyValuePair<Guid, VirtualEffect> guid in _virtualEffectLibrary)
			{
				ToolStripMenuItem menuItem = new ToolStripMenuItem(guid.Value.Name);
				menuItem.Tag = guid.Key;
				menuItem.Click += (sender, e) =>
									{
										Row destination = TimelineControl.ActiveRow ?? TimelineControl.SelectedRow;
										if (destination != null)
										{
											addNewVirtualEffectById((Guid)menuItem.Tag, destination, TimelineControl.CursorPosition,
																	TimeSpan.FromSeconds(2)); // TODO: get a proper time
										}
									};
				//addEffectToolStripMenuItem.DropDownItems.Add(menuItem);

				// Add a button to the tool strip
				//ToolStripItem tsItem = new ToolStripButton(guid.Value.Name);
				//tsItem.Tag = guid.Key;
				//tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				//tsItem.MouseMove += toolStripEffects_Item_MouseMove;
				//tsItem.Click += toolStripEffects_Item_Click;

				//toolStripEffects.Items.Add(tsItem);
				//toolStripExVirtualEffects.Items.Add(tsItem);
			}
		}

		private void LoadAvailableEffects()
		{
			foreach (
				IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				// Add an entry to the menu
				ToolStripMenuItem menuItem = new ToolStripMenuItem(effectDesriptor.EffectName);
				menuItem.Tag = effectDesriptor.TypeId;
				menuItem.Click += (sender, e) =>
									{
										Row destination = TimelineControl.ActiveRow ?? TimelineControl.SelectedRow;
										if (destination != null)
										{
											addNewEffectById((Guid)menuItem.Tag, destination, TimelineControl.CursorPosition,
															 TimeSpan.FromSeconds(2)); // TODO: get a proper time
										}
									};
				addEffectToolStripMenuItem.DropDownItems.Add(menuItem);

				// Add a button to the tool strip
				//ToolStripItem tsItem = new ToolStripButton(effectDesriptor.EffectName);
				//tsItem.Tag = effectDesriptor.TypeId;
				//tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				//tsItem.MouseMove += toolStripEffects_Item_MouseMove;
				//tsItem.Click += toolStripEffects_Item_Click;
				//tsItem.Image = effectDesriptor.GetRepresentativeImage(48, 48);

				//toolStripEffects.Items.Add(tsItem);
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

				if (TimelineControl.TotalTime != value)
				{
					TimelineControl.TotalTime = value;
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
			TimelineControl.AllowGridResize = false;
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			if (clearCurrentRows)
				TimelineControl.ClearAllRows();

			TimelineControl.EnableDisableHandlers(false);
			foreach (ElementNode node in VixenSystem.Nodes.GetRootNodes())
			{
				addNodeAsRow(node, null);
			}
			TimelineControl.EnableDisableHandlers(true);

			TimelineControl.LayoutRows();
			TimelineControl.ResizeGrid();
		}

		private void loadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			updateToolStrip4(string.Format("Please Wait. Loading: {1}", _sequence.Name, loadingWatch.Elapsed));
		}

		private delegate void updateToolStrip4Delegate(string text);

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

		private System.Timers.Timer loadTimer = null;
		private Stopwatch loadingWatch = null;

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
			TimelineControl.SequenceLoading = true;

			// Let's get the window on the screen. Make it appear to instantly load.
			Invalidate(true);
			Update();

			try
			{
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

				TimelineControl.grid.SuppressInvalidate = true; //Hold off invalidating the grid while we bulk load.
				TimelineControl.grid.SupressRendering = true; //Hold off rendering while we load elements. 
				// This takes quite a bit of time so queue it up
				taskQueue.Enqueue(Task.Factory.StartNew(() =>
															{
																addElementsForEffectNodes(_sequence.SequenceData.EffectData);
															}));
				// Now that it is queued up, let 'er rip and start background rendering when complete.
				Task.Factory.ContinueWhenAll(taskQueue.ToArray(), completedTasks =>
																	{
																		// Clear the loading toolbar
																		loadingWatch.Stop();
																		TimelineControl.SequenceLoading = false;
																		loadTimer.Enabled = false;
																		updateToolStrip4(string.Empty);
																		TimelineControl.grid.SupressRendering = false;
																		TimelineControl.grid.SuppressInvalidate = false;
																		TimelineControl.grid.RenderAllRows();
																	});

				populateGridWithMarks();

				var t2 = Task.Factory.StartNew(() => populateWaveformAudio());

				//This path is followed for new and existing sequences so we need to determine which we have and set modified accordingly.
				//Added logic to determine if the sequence has a filepath to set modified JU 8/1/2012. 

				_SetTimingToolStripEnabledState();

				if (String.IsNullOrEmpty(_sequence.FilePath))
				{
					sequenceModified();
				}
				else
				{
					sequenceNotModified();
				}
				PopulateAudioDropdown();

				MarksForm.Sequence = Sequence as TimedSequence;
				MarksForm.PopulateMarkCollectionsList(null);

				Logging.Debug(string.Format("Sequence {0} took {1} to load. ", sequence.Name, loadingWatch.Elapsed));
			}
			catch (Exception ee)
			{
				Logging.ErrorException("Error loading sequence.", ee);
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
			if (_sequence == null)
			{
				Logging.Error("Trying to save a sequence that is null!");
			}

			if (filePath == null | forcePrompt)
			{
				if (_sequence.FilePath.Trim() == "" || forcePrompt)
				{
					// Updated to use the OS SaveFileDialog functionality 8/1/2012 JU
					// Edit this type to be the more generic type to support importing into timed sequnces 12 FEB 2013 - JEMA
					EditorModuleDescriptorBase descriptor = ((OwnerModule.Descriptor) as EditorModuleDescriptorBase);
					saveFileDialog.InitialDirectory = SequenceService.SequenceDirectory;
					string filter = descriptor.TypeName + " (*" + string.Join(", *", _sequence.FileExtension) + ")|*" +
									string.Join("; *", _sequence.FileExtension);
					saveFileDialog.DefaultExt = _sequence.FileExtension;
					saveFileDialog.Filter = filter;
					DialogResult result = saveFileDialog.ShowDialog();
					if (result == DialogResult.OK)
					{
						string name = saveFileDialog.FileName;
						string extension = Path.GetExtension(saveFileDialog.FileName);

						// if the given extension isn't valid for this type, then keep the name intact and add an extension
						if (extension != _sequence.FileExtension)
						{
							name = name + _sequence.FileExtension;
							Logging.Info("Incorrect extension provided for timed sequence, appending one.");
						}
						_sequence.Save(name);
					}
					else
					{
						//user canceled save
						return;
					}
				}
				else
				{
					_sequence.Save();
				}
			}
			else
			{
				_sequence.Save(filePath);
			}

			sequenceNotModified();
		}

		#endregion

		#region Other Private Methods

		private void populateGridWithMarks()
		{
			TimelineControl.ClearAllSnapTimes();

			foreach (MarkCollection mc in _sequence.MarkCollections)
			{
				if (mc.Enabled)
				{
					foreach (TimeSpan time in mc.Marks)
					{
						TimelineControl.AddSnapTime(time, mc.Level, mc.MarkColor);
					}
				}
			}
		}
		private void PopulateAudioDropdown()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Vixen.Delegates.GenericDelegate(PopulateAudioDropdown));
			}
			else
			{
				using (var fmod = new FmodInstance())
				{
					cboAudioDevices.Items.Clear();
					fmod.AudioDevices.OrderBy(a => a.Item1).Select(b => b.Item2).ToList().ForEach(device =>
					{
						cboAudioDevices.Items.Add(device);
					});
					if (cboAudioDevices.Items.Count > 0)
					{
						cboAudioDevices.SelectedIndex = 0;
					}
				}
			}
		}
		private void populateWaveformAudio()
		{
			if (_sequence.GetAllMedia().Any())
			{
				IMediaModuleInstance media = _sequence.GetAllMedia().First();
				Audio audio = media as Audio;

				if (audio.MediaExists)
				{
					TimelineControl.Audio = audio;
					toolStripMenuItem_removeAudio.Enabled = true;
					PopulateAudioDropdown();
				}
				else
				{
					string message = String.Format("Audio file not found on the path:\n\n {0}\n\nPlease Check your settings/path.", audio.MediaFilePath);
					MessageBox.Show(message, "Missing audio file");
				}


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
			if (!m_modified)
			{
				m_modified = true;
				setTitleBarText();
				// TODO: Other things, like enable save button, etc.	
			}
			
		}

		/// <summary>Called when the sequence is no longer considered modified.</summary>
		private void sequenceNotModified()
		{
			if (m_modified)
			{
				m_modified = false;
				setTitleBarText();
				// TODO: Other things, like disable save button, etc.	
			}
			
		}

		#endregion

		#region Event Handlers

		private void OnRenderProgressChanged(object sender, RenderElementEventArgs e)
		{
			try
			{
				if (!Disposing)
				{
					if (e.Percent >= 0 && e.Percent <= 100)
					{
						toolStripProgressBar_RenderingElements.Value = e.Percent;
					}
					if (e.Percent == 100)
					{
						toolStripProgressBar_RenderingElements.Visible = false;
						toolStripStatusLabel_RenderingElements.Visible = false;
					}
					else if (!toolStripProgressBar_RenderingElements.Visible)
					{
						toolStripProgressBar_RenderingElements.Visible = true;
						toolStripStatusLabel_RenderingElements.Visible = true;
					}
				}
			}
			catch
			{
			}
		}

		private void TimelineSequenceTimeLineSequenceClipboardContentsChanged(object sender, EventArgs eventArgs)
		{
			UpdatePasteMenuStates();
		}

		private void TimelineControlOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			toolStripButton_Copy.Enabled = toolStripButton_Cut.Enabled = TimelineControl.SelectedElements.Any();
			toolStripMenuItem_Copy.Enabled = toolStripMenuItem_Cut.Enabled = TimelineControl.SelectedElements.Any();
		}

		private void TimelineControl_MouseDown(object sender, MouseEventArgs e)
		{
			TimelineControl.ruler.ClearSelectedMarks();
			Invalidate(true);
		}

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			TimedSequenceElement element = sender as TimedSequenceElement;
			TimelineControl.grid.RenderElement(element);
			sequenceModified();
		}

		protected void ElementTimeChangedHandler(object sender, EventArgs e)
		{
			//TimedSequenceElement element = sender as TimedSequenceElement;
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
	
			movedElement.TargetNodes = new[]{newElement};

			// now that the effect that this element has been updated to accurately reflect the change,
			// move the actual element around. It's a single element in the grid, belonging to multiple rows:
			// so find all rows that represent the old element, remove the element from them, and also find
			// all rows that represent the new element and add it to them.
			foreach (Row row in TimelineControl)
			{
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

			if (element.EffectNode == null)
			{
				Logging.Error("TimedSequenceEditor: Element double-clicked, and it doesn't have an associated effect!");
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

			using (
				TimedSequenceEditorEffectEditor editor = new TimedSequenceEditorEffectEditor(elements.Select(x => x.EffectNode)))
			{
				DialogResult result = editor.ShowDialog();
				if (result == DialogResult.OK)
				{
					foreach (Element element in elements)
					{
						TimelineControl.grid.RenderElement(element);
					}
					sequenceModified();
				}
			}
		}

		private void timelineControl_ContextSelected(object sender, ContextSelectedEventArgs e)
		{

			contextMenuStrip.Items.Clear();

			ToolStripMenuItem addEffectItem = new ToolStripMenuItem("Add Effect");

			foreach (
				IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				// Add an entry to the menu
				ToolStripMenuItem menuItem = new ToolStripMenuItem(effectDesriptor.EffectName);
				menuItem.Tag = effectDesriptor.TypeId;
				menuItem.Click += (mySender, myE) =>
				{
					if (e.Row != null)
					{
						addNewEffectById((Guid)menuItem.Tag, e.Row, e.GridTime,
										 TimeSpan.FromSeconds(2));
					}
				};

				addEffectItem.DropDownItems.Add(menuItem);
			}

			contextMenuStrip.Items.Add(addEffectItem);

			if (e.ElementsUnderCursor != null && e.ElementsUnderCursor.Count() == 1)
			{

				Element element = e.ElementsUnderCursor.FirstOrDefault();

				TimedSequenceElement tse = element as TimedSequenceElement;

				if (TimelineControl.SelectedElements.Count() > 1)
				{

					ToolStripMenuItem itemAlignment = new ToolStripMenuItem("Alignment");
					ToolStripMenuItem itemAlignStart = new ToolStripMenuItem("Align Start Times (shift)");
					itemAlignStart.ToolTipText = "Holding shift will align the start times, while holding duration.";
					itemAlignStart.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.StartTime == element.StartTime) continue;
							//If elements end time is before or the same as the reference start time, just move the element, otherwise element becomes invalid
							if (selectedElement.EndTime < element.StartTime || selectedElement.EndTime == element.StartTime)
							{
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.StartTime, element.StartTime + selectedElement.Duration);
								continue;
							}
							if (Control.ModifierKeys == Keys.Shift)
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.StartTime, element.StartTime + selectedElement.Duration);
							else
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.StartTime, selectedElement.EndTime);
						}
					};

					ToolStripMenuItem itemAlignEnd = new ToolStripMenuItem("Align End Times (shift)");
					itemAlignEnd.ToolTipText = "Holding shift will align the end times, while holding duration.";
					itemAlignEnd.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.EndTime == element.EndTime) continue;
							//If elements start time is after or the same as the reference end time, just move the element, otherwise element becomes invalid
							if (selectedElement.StartTime > element.EndTime || selectedElement.StartTime == element.EndTime)
							{
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.EndTime - selectedElement.Duration, element.EndTime);
								continue;
							}
							if (Control.ModifierKeys == Keys.Shift)
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.EndTime - selectedElement.Duration, element.EndTime);
							else
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, selectedElement.StartTime, element.EndTime);
						}
					};

					ToolStripMenuItem itemAlignBoth = new ToolStripMenuItem("Align Both Times");
					itemAlignBoth.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.StartTime == element.StartTime && selectedElement.EndTime == element.EndTime) continue;
							TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.StartTime, element.EndTime);
						}
					};

					ToolStripMenuItem itemMatchDuration = new ToolStripMenuItem("Match Duration (shift)");
					itemMatchDuration.ToolTipText = "Holding shift will hold the effects end time and adjust the start time, by default the end time is adjusted.";
					itemMatchDuration.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.Duration == element.Duration) continue;
							if (Control.ModifierKeys == Keys.Shift)
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, selectedElement.EndTime - element.Duration, selectedElement.EndTime);
							else
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, selectedElement.StartTime, selectedElement.StartTime + element.Duration);
						}
					};
					ToolStripMenuItem itemAlignStartToEnd = new ToolStripMenuItem("Align Start to End");
					itemAlignStartToEnd.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.EndTime == element.EndTime) continue;
							//Need to make sure element is not moved beyond time, if going to do so we need to adjust duration while moving otherwise element becomes invalid and not clickable
							if ((element.EndTime + selectedElement.Duration) > TimelineControl.TotalTime)
							{
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.EndTime, TimelineControl.TotalTime);
								continue;
							}
							//if the end time is going to be before the start time, we should just move the selectedelement
							if (element.EndTime > (selectedElement.StartTime + selectedElement.Duration))
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.EndTime, element.EndTime + selectedElement.Duration);
							else
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.EndTime, selectedElement.EndTime);
						}
					};
					ToolStripMenuItem itemAlignEndToStart = new ToolStripMenuItem("Align End to Start");
					itemAlignEndToStart.Click += (mySender, myE) =>
					{

						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.StartTime == element.StartTime) continue;
							//if the start time is going to be after the end time, we should just move the selectedelement
							//We don't need to wory about making sure the element will not go before 0, it works properly as it is.
							if (element.StartTime < (selectedElement.StartTime + selectedElement.Duration))
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, element.StartTime - selectedElement.Duration, element.StartTime);
							else
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, selectedElement.StartTime, element.StartTime);
							//In the event that the start time would have been moved to before 0, lets double check and make sure we are aligned
							if (selectedElement.EndTime > element.StartTime)
								TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, selectedElement.StartTime, element.StartTime);
						}
					};
					ToolStripMenuItem itemDistDialog = new ToolStripMenuItem("Distribute Effects");
					itemDistDialog.Click += (mySender, myE) =>
					{
						var startTime = TimelineControl.SelectedElements.First().StartTime;
						var endTime = TimelineControl.SelectedElements.Last().EndTime;
						if (startTime > endTime)
						{
							startTime = TimelineControl.SelectedElements.Last().StartTime;
							endTime = TimelineControl.SelectedElements.First().EndTime;
						}
						var duration = endTime - startTime;
						var dDialog = new EffectDistributionDialog();
						var elementCount = TimelineControl.SelectedElements.Count();
						var offset = duration.TotalSeconds / elementCount;

						dDialog.ElementCount = elementCount.ToString();
						dDialog.StartTime = startTime;
						dDialog.EndTime = endTime;
						dDialog.RadioEqualDuration = true;
						dDialog.RadioStairStep = true;
						dDialog.StartWithFirst = true;
						dDialog.ShowDialog();
						if (dDialog.DialogResult == DialogResult.OK)
						{
							startTime = dDialog.StartTime;
							endTime = dDialog.EndTime;
							duration = endTime - startTime; //TimeSpan.Parse(dDialog.Duration);
							offset = duration.TotalSeconds / elementCount;
							var effectDuratin = dDialog.SpecifiedEffectDuration;
							var effectSpacing = dDialog.SpacedPlacementDuration;

							if (dDialog.StartWithFirst)
							{
								//We start with the first effect
								for (int i = 0; i <= elementCount - 1; i++)
								{
									var thisStartTime = startTime.TotalSeconds;
									var thisEndTime = thisStartTime + offset;
									//Generic placement of starttime eq to prev end time
									if (i > 0)
										thisStartTime = TimelineControl.SelectedElements.ElementAt(i - 1).EndTime.TotalSeconds;
									//Determine Start time
									if (i > 0 && dDialog.RadioEffectPlacementOverlap)
										thisStartTime = thisStartTime - Convert.ToDouble(dDialog.EffectPlacementOverlap.TotalSeconds);
									if (i >0 && dDialog.RadioPlacementSpacedDuration)
										thisStartTime = thisStartTime + Convert.ToDouble(dDialog.SpacedPlacementDuration.TotalSeconds);
									if (dDialog.RadioDoNotChangeDuration && !dDialog.RadioEffectPlacementOverlap && !dDialog.RadioPlacementSpacedDuration)
										thisStartTime = startTime.TotalSeconds + (offset * i);
									//Determine End time
									if (dDialog.RadioEqualDuration)
										thisEndTime = thisStartTime + offset;
									if (dDialog.RadioDoNotChangeDuration)
										thisEndTime = thisStartTime + TimelineControl.SelectedElements.ElementAt(i).Duration.TotalSeconds;
									if (dDialog.RadioSpecifiedDuration)
										thisEndTime = thisStartTime + Convert.ToDouble(dDialog.SpecifiedEffectDuration.TotalSeconds);
									TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.ElementAt(i), TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisEndTime));
								}
							}
							if (dDialog.StartWithLast)
							{
								//We start with the last effect
								int placeCount = 0;
								for (int i = elementCount - 1; i >= 0; i--)
								{
									var thisStartTime = startTime.TotalSeconds;
									var thisEndTime = thisStartTime + offset;
									//Generic placement of starttime eq to prev end time
									if (i < elementCount - 1)
										thisStartTime = TimelineControl.SelectedElements.ElementAt(i + 1).EndTime.TotalSeconds;
									//Determine Start time
									if (i < elementCount - 1 && dDialog.RadioEffectPlacementOverlap)
										thisStartTime = thisStartTime - Convert.ToDouble(dDialog.EffectPlacementOverlap.TotalSeconds);
									if (i < elementCount - 1 && dDialog.RadioPlacementSpacedDuration)
										thisStartTime = thisStartTime + Convert.ToDouble(dDialog.SpacedPlacementDuration.TotalSeconds);
									if (dDialog.RadioDoNotChangeDuration && !dDialog.RadioEffectPlacementOverlap && !dDialog.RadioPlacementSpacedDuration)
										thisStartTime = startTime.TotalSeconds + (offset * placeCount);
									//Determine End time
									if (dDialog.RadioEqualDuration)
										thisEndTime = thisStartTime + offset;
									if (dDialog.RadioDoNotChangeDuration)
										thisEndTime = thisStartTime + TimelineControl.SelectedElements.ElementAt(i).Duration.TotalSeconds;
									if (dDialog.RadioSpecifiedDuration)
										thisEndTime = thisStartTime + Convert.ToDouble(dDialog.SpecifiedEffectDuration.TotalSeconds);
									TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.ElementAt(i), TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisEndTime));
									placeCount++;
								}
							}
						}
					};
					ToolStripMenuItem itemAlignCenter = new ToolStripMenuItem("Align Centerpoints");
					itemAlignCenter.Click += (mySender, myE) =>
					{
						var centerPoint = element.StartTime.TotalSeconds + (element.Duration.TotalSeconds / 2);
						foreach (Element selectedElement in TimelineControl.SelectedElements)
						{
							if (selectedElement.StartTime == element.StartTime) continue;
							var thisStartTime = centerPoint - (selectedElement.Duration.TotalSeconds / 2);
							TimelineControl.grid.MoveResizeElementByStartEnd(selectedElement, TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisStartTime) + selectedElement.Duration);
						}

					};
					ToolStripMenuItem itemDistributeEqually = new ToolStripMenuItem("Distribute Equally");
					itemDistributeEqually.ToolTipText = "This will stair step the selected elements, starting with the element that has the earlier start point on the time line.";
					itemDistributeEqually.Click += (mySender, myE) =>
					{
						//Before we do anything lets make sure there is time to work with
						//I don't remember why I put this here, for now its commented out until its verified that its not needed, then it will be removed
						//if (TimelineControl.SelectedElements.First().EndTime == TimelineControl.SelectedElements.Last().EndTime)
						//{
						//	MessageBox.Show("The first and last effect cannot have the same end time.", "Warning", MessageBoxButtons.OK);
						//	return;
						//}
						bool startAtLastElement = false;
						var totalElements = TimelineControl.SelectedElements.Count();
						var startTime = TimelineControl.SelectedElements.First().StartTime;
						var endTime = TimelineControl.SelectedElements.Last().EndTime;
						if (TimelineControl.SelectedElements.First().StartTime > TimelineControl.SelectedElements.Last().StartTime)
						{
							startAtLastElement = true;
							startTime = TimelineControl.SelectedElements.Last().StartTime;
							endTime = TimelineControl.SelectedElements.First().EndTime;
						}
						var totalDuration = endTime - startTime;
						var effectDuration = totalDuration.TotalSeconds / totalElements;
						TimeSpan effectTS = TimeSpan.FromSeconds(effectDuration);
						//var msgString = string.Format("Total Elements: {0}\n Start Time: {1}\n End Time: {2}\n Total Duration: {3}\n Effect Duration: {4}\n TimeSpan Duration: {5}\n Start at last element: {6}", totalElements,startTime,endTime,totalDuration,effectDuration, effectTS.TotalSeconds, startAtLastElement);
						//MessageBox.Show(msgString);
						//Sanity Check - Keep effects from becoming less than minimum.
						if (effectDuration < .01)
						{
							MessageBox.Show(string.Format("Unable to complete request. The resulting duration would fall below .01 seconds.\nCalculated duration: {0}", effectDuration), "Warning", MessageBoxButtons.OK);
							return;
						}
						if (!startAtLastElement)
						{
							//Lets move the first one
							TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.ElementAt(0), startTime, startTime + effectTS);
							for (int i = 1; i <= totalElements - 1; i++)
							{
								var thisStartTime = TimelineControl.SelectedElements.ElementAt(i - 1).EndTime;
								TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.ElementAt(i), thisStartTime, thisStartTime + effectTS);
							}
						}
						else
						{
							//Lets move the first(last) one
							TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.Last(), startTime, startTime + effectTS);
							for (int i = totalElements - 2; i >= 0; i--)
							{
								var thisStartTime = TimelineControl.SelectedElements.ElementAt(i + 1).EndTime;
								TimelineControl.grid.MoveResizeElementByStartEnd(TimelineControl.SelectedElements.ElementAt(i), thisStartTime, thisStartTime + effectTS);
							}
						}
					};

					contextMenuStrip.Items.Add(itemAlignment);
					itemAlignment.DropDown.Items.Add(itemAlignStart);
					itemAlignment.DropDown.Items.Add(itemAlignEnd);
					itemAlignment.DropDown.Items.Add(itemAlignBoth);
					itemAlignment.DropDown.Items.Add(itemAlignCenter);
					itemAlignment.DropDown.Items.Add(itemMatchDuration);
					itemAlignment.DropDown.Items.Add(itemAlignStartToEnd);
					itemAlignment.DropDown.Items.Add(itemAlignEndToStart);
					itemAlignment.DropDown.Items.Add(itemDistributeEqually);
					itemAlignment.DropDown.Items.Add(itemDistDialog);
				}

				if (tse != null)
				{
					ToolStripMenuItem item = new ToolStripMenuItem("Edit Time");
					item.Click += (mySender, myE) =>
					{
						EffectTimeEditor editor = new EffectTimeEditor(tse.EffectNode.StartTime, tse.EffectNode.TimeSpan);
						if (editor.ShowDialog(this) == DialogResult.OK)
						{
							TimelineControl.grid.MoveResizeElement(element, editor.Start, editor.Duration);
						}
					};
					item.Tag = tse;
					contextMenuStrip.Items.Add(item);

				}

			}



			//Add Copy/Cut/paste section
			contextMenuStrip.Items.Add("-");
			contextMenuStrip.Items.Add(toolStripMenuItem_Copy);
			contextMenuStrip.Items.Add(toolStripMenuItem_Cut);
			contextMenuStrip.Items.Add(toolStripMenuItem_Paste);
			if (TimelineControl.SelectedElements.Any())
			{
				//Add Edit delete
				contextMenuStrip.Items.Add("-");
				contextMenuStrip.Items.Add(toolStripMenuItem_EditEffect);
				contextMenuStrip.Items.Add(toolStripMenuItem_deleteElements);
			}

			e.AutomaticallyHandleSelection = false;

			contextMenuStrip.Show(MousePosition);
		}

		private void timelineControl_ElementsSelected(object sender, ElementsSelectedEventArgs e)
		{
			if (e.ElementsUnderCursor != null && e.ElementsUnderCursor.Count() > 1)
			{
				contextMenuStripElementSelection.Items.Clear();

				ToolStripMenuItem item;

				foreach (Element element in e.ElementsUnderCursor)
				{
					TimedSequenceElement tse = element as TimedSequenceElement;
					if (tse == null)
						continue;

					string name = tse.EffectNode.Effect.Descriptor.TypeName;
					name += string.Format(" ({0:m\\:ss\\.fff})", tse.EffectNode.StartTime);
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
				TimelineControl.SelectElement(tse);
		}

		private void timelineControl_RulerClicked(object sender, RulerClickedEventArgs e)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: StartPointClicked to Play with null context!");
				return;
			}

			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

				if (autoPlay)
				{
					// Save the times for later restoration
					m_prevPlaybackStart = TimelineControl.PlaybackStartTime;
					m_prevPlaybackEnd = TimelineControl.PlaybackEndTime;
				}
				else
				{
					m_prevPlaybackStart = e.Time;
					m_prevPlaybackEnd = null;
				}

				// Set the timeline control
				TimelineControl.PlaybackStartTime = e.Time;
				TimelineControl.PlaybackEndTime = null;

				if (autoPlay)
				{
					_PlaySequence(e.Time, TimeSpan.MaxValue);
				}
				else
				{
					TimelineControl.CursorPosition = e.Time;
				}
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				MarkCollection mc = null;
				if (_sequence.MarkCollections.Count == 0)
				{
					if (MessageBox.Show("Marks are stored in Mark Collections. There are no mark collections available to store this mark. Would you like to create a new one?", "Creat a Mark Collection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						mc = GetOrAddNewMarkCollection(System.Drawing.Color.White, "Default Marks");
						MarksForm.PopulateMarkCollectionsList(mc);
					}
				}
				else
				{
					mc = MarksForm.SelectedMarkCollection;
					if (mc == null)
					{
						MessageBox.Show("Please select a mark collection in the Mark Manager window before adding a new mark to the timeline.", "New Mark", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
					}
				}
				if (mc != null)
				{
					mc.Marks.Add(e.Time);
					populateGridWithMarks();
					sequenceModified();
				}
			}
		}

		private MarkCollection GetOrAddNewMarkCollection(System.Drawing.Color color, string name = "New Collection")
		{
			MarkCollection mc = null;
			foreach (MarkCollection mCollection in _sequence.MarkCollections)
			{
				if (mCollection.Name == name)
				{
					mc = mCollection;
					break;
				}
			}
			if (mc == null)
			{
				MarkCollection newCollection = new MarkCollection();
				newCollection.Name = name;
				newCollection.MarkColor = color;
				_sequence.MarkCollections.Add(newCollection);
				mc = newCollection;
				sequenceModified();
			}

			return mc;
		}

		private void timelineControl_MarkMoved(object sender, MarkMovedEventArgs e)
		{
			foreach (MarkCollection mc in _sequence.MarkCollections)
			{
				if (/*e.SnapDetails.SnapColor == mc.MarkColor && */e.SnapDetails.SnapLevel == mc.Level)
				{
					if (mc.Marks.Contains(e.OriginalMark))
					{
						mc.Marks.Remove(e.OriginalMark);
						mc.Marks.Add(e.NewMark);
					}
				}
			}
			populateGridWithMarks();
			sequenceModified();
		}

		private void timelineControl_DeleteMark(object sender, DeleteMarkEventArgs e)
		{
			foreach (MarkCollection mc in _sequence.MarkCollections)
			{
				if (mc.Marks.Contains(e.Mark))
				{
					mc.Marks.Remove(e.Mark);
				}
			}
			populateGridWithMarks();
			sequenceModified();
		}

		private void timelineControl_RulerBeginDragTimeRange(object sender, EventArgs e)
		{
			m_prevPlaybackStart = TimelineControl.PlaybackStartTime;
			m_prevPlaybackEnd = TimelineControl.PlaybackEndTime;
		}

		private void timelineControl_TimeRangeDragged(object sender, ModifierKeysEventArgs e)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: TimeRangeDragged with null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

			if (autoPlay)
			{
				_PlaySequence(TimelineControl.PlaybackStartTime.Value, TimelineControl.PlaybackEndTime.Value);
			}
			else
			{
				// We actually want to keep this range.
				m_prevPlaybackStart = TimelineControl.PlaybackStartTime;
				m_prevPlaybackEnd = TimelineControl.PlaybackEndTime;
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
			if (TimeLineSequenceClipboardContentsChanged != null)
			{
				TimeLineSequenceClipboardContentsChanged(this, null);
			}
		}

		#endregion

		#region Sequence actions (play, pause, etc.)

		private void OnExecutionStateChanged(object sender, EventArgs e)
		{
			Console.WriteLine("tse: state changed: " + Execution.State);
			if (Execution.State.Equals("Closing"))
			{
				if (_context != null)
					CloseSequenceContext();
				_context = null;
			}
			else if (Execution.State.Equals("Open"))
			{
				OpenSequenceContext(_sequence);
			}
		}

		private void OpenSequenceContext(Vixen.Sys.ISequence sequence)
		{
			if (_context != null)
			{
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
			TimelineControl.grid.Context = _context;
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
			stoppedByUser = false;
			//MessageBox.Show("Call to play sequence");
			if (delayOffToolStripMenuItem.Checked == false && timerPostponePlay.Enabled == false && toolStripButton_Stop.Enabled == false)
			{
				//MessageBox.Show("Starting delay");
				DelayCountDown = (timerPostponePlay.Interval / 1000);
				timerPostponePlay.Enabled = timerDelayCountdown.Enabled = true;
				toolStripButton_Play.Image = Resources.hourglass;
				//The Looping stuff kinda broke this, but we need to do this for consistency
				toolStripButton_Play.Enabled = true;
				playToolStripMenuItem.Enabled = EffectsForm.Enabled = false;
				toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = true;
			}

			if (timerPostponePlay.Enabled)
			{
				//We are waiting for a delayed start, ignore the play button
				return;
			}

			//Make sure the blue play icon is used & dissappear the delay countdown
			toolStripButton_Play.Image = Resources.control_play_blue;
			toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;

			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused)
			{
				// continue execution from previous location.
				start = _TimingSource.Position;
				end = TimeSpan.MaxValue;
				updateButtonStates(); // context provides no notification to/from pause state.
			}
			else
			{
				start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
			}
			_PlaySequence(start, end);
		}

		public void PlaySequenceFrom(TimeSpan StartTime)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: attempt to Play with null context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused)
			{
				// continue execution from previous location.
				start = _TimingSource.Position;
				end = TimeSpan.MaxValue;
				updateButtonStates(); // context provides no notification to/from pause state.
			}
			else
			{
				start = StartTime;
				end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
				if (start >= end)
				{
					start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				}
			}
			_PlaySequence(start, end);
		}

		public void PauseSequence()
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: attempt to Pause with null context!");
				return;
			}

			_context.Pause();
			updateButtonStates(); // context provides no notification to/from pause state.
		}

		public void StopSequence()
		{
			if (delayOffToolStripMenuItem.Checked != true)
			{
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
				toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", timerPostponePlay.Interval / 1000);
			}

			if (timerPostponePlay.Enabled)
			{
				timerPostponePlay.Enabled = timerDelayCountdown.Enabled = false;
				toolStripButton_Play.Image = Resources.control_play_blue;
				toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = EffectsForm.Enabled = true;
				toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
				//We are stopping the delay, there is no context, so get out of here to avoid false entry into error log
				return;
			}

			if (timerLoop.Enabled == true)
			{
				//There is no context, the user has pressed the stop button between loops, stop the timer and get out
				//We also need to update the state of the buttons
				timerLoop.Enabled = false;
				updateButtonStates();
				return;
			}


			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: attempt to Stop with null context!");
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
			//This is for the delayed play options
			if (delayOffToolStripMenuItem.Checked == false)
			{
				//MessageBox.Show("SHOWING STATUS BAR");
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
				toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", timerPostponePlay.Interval / 1000);
			}

			timerPlaying.Stop();
			_TimingSource = null;
		}

		protected void context_ContextEnded(object sender, EventArgs e)
		{
			updateButtonStates();

			TimelineControl.PlaybackStartTime = m_prevPlaybackStart;
			TimelineControl.PlaybackEndTime = m_prevPlaybackEnd;
			TimelineControl.PlaybackCurrentTime = null;

			if (toolStripButton_Loop.Checked && stoppedByUser == false)
				timerLoop.Enabled = true;
		}

		protected void timerPlaying_Tick(object sender, EventArgs e)
		{
			if (_TimingSource != null)
			{
				TimelineControl.PlaybackCurrentTime = _TimingSource.Position;
			}
		}

		private void timelineControl_PlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			if (TimelineControl.PlaybackCurrentTime.HasValue)
				toolStripStatusLabel_currentTime.Text = TimelineControl.PlaybackCurrentTime.Value.ToString("m\\:ss\\.fff");
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
			if (dataObject != null)
			{
				toolStripButton_Paste.Enabled =
					toolStripMenuItem_Paste.Enabled = dataObject.GetDataPresent(_clipboardFormatName.Name);
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
					//toolStripEffects.Enabled = false;
					EffectsForm.Enabled = false;
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
					//toolStripEffects.Enabled = false;
					EffectsForm.Enabled = false;
				}
				else // Stopped
				{
					//We are looping...this keeps the Play button and EffectsForm from blinking between loops, the other buttons should already be in proper state
					if (toolStripButton_Loop.Checked && stoppedByUser == false)
					{
						//We dont need a pause button between loops, it only gives the user the right to think they should be able to use it if its on
						toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
						return;
					}
					toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
					toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
					//toolStripEffects.Enabled = true;
					EffectsForm.Enabled = true;
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

			foreach (Row row in TimelineControl) // Remove the element from all rows
				row.RemoveElement(tse);

			// TODO: Unnecessary?
			tse.ContentChanged -= ElementContentChangedHandler; // Unregister event handlers
			tse.TimeChanged -= ElementTimeChangedHandler;

			_effectNodeToElement.Remove(node); // Remove the effect node from the map
			_sequence.RemoveData(node); // Remove the effect node from sequence
		}


		/// <summary>
		/// Creates a new effect instance, and adds it to the sequence and TimelineControl.
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
			if (_virtualEffectLibrary != null && _virtualEffectLibrary.ContainsEffect(effectId))
			{
				addNewVirtualEffectById(effectId, row, startTime, timeSpan);
			}
			else
			{
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
			try
			{
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
			}
			catch (Exception ex)
			{
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " +
							 ((row == null) ? "<null>" : row.Name);
				Logging.ErrorException(msg, ex);
			}
		}




		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for each of the given EffectNodes in the list.
		/// Uses bulk loading feature of Row
		/// Will add a single TimedSequenceElement to in each row that each targeted element of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="node">The EffectNode to make element(s) in the grid for.</param>
		private void addElementsForEffectNodes(IEnumerable<IDataNode> nodes)
		{
			Dictionary<Row, List<Element>> rowMap =
			_elementNodeToRows.SelectMany(x => x.Value).ToList().ToDictionary(x => x, x => new List<Element>());

			foreach (EffectNode node in nodes)
			{
				TimedSequenceElement element = setupNewElementFromNode(node);
				foreach (ElementNode target in node.Effect.TargetNodes)
				{
					if (_elementNodeToRows.ContainsKey(target))
					{
						// Add the element to each row that represents the element this command is in.
						foreach (Row row in _elementNodeToRows[target])
						{
							if (!_effectNodeToElement.ContainsKey(node))
							{
								_effectNodeToElement[node] = element;
							}
							rowMap[row].Add(element);

						}
					}
					else
					{
						// we don't have a row for the element this effect is referencing; most likely, the row has
						// been deleted, or we're opening someone else's sequence, etc. Big fat TODO: here for that, then.
						// dunno what we want to do: prompt to add new elements for them? map them to others? etc.
						string message = "No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
						MessageBox.Show(message);
						Logging.Error(message);
					}
				}
			}

			foreach (KeyValuePair<Row, List<Element>> row in rowMap)
			{
				row.Key.AddBulkElements(row.Value);
			}

		}

		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for the given EffectNode.
		/// Will add a single TimedSequenceElement to in each row that each targeted element of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="node">The EffectNode to make element(s) in the grid for.</param>
		private TimedSequenceElement addElementForEffectNodeTPL(EffectNode node)
		{
			TimedSequenceElement element = setupNewElementFromNode(node);

			// for the effect, make a single element and add it to every row that represents its target elements
			node.Effect.TargetNodes.AsParallel().WithCancellation(cancellationTokenSource.Token)
				.ForAll(target =>
							{
								if (_elementNodeToRows.ContainsKey(target))
								{
									// Add the element to each row that represents the element this command is in.
									foreach (Row row in _elementNodeToRows[target])
									{
										if (!_effectNodeToElement.ContainsKey(node))
										{
											_effectNodeToElement[node] = element;
										}
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
									Logging.Error(message);
								}
							});
			TimelineControl.grid.RenderElement(element);
			return element;
		}

		private TimedSequenceElement setupNewElementFromNode(EffectNode node)
		{
			TimedSequenceElement element = new TimedSequenceElement(node);
			element.ContentChanged += ElementContentChangedHandler;
			element.TimeChanged += ElementTimeChangedHandler;
			return element;
		}

		private void removeSelectedElements()
		{
			Element[] selected = TimelineControl.SelectedElements.ToArray();

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
		/// Adds a single given element node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
		private int doEventsCounter = 0;

		private void addNodeAsRow(ElementNode node, Row parentRow)
		{
			// made the new row from the given node and add it to the control.
			TimedSequenceRowLabel label = new TimedSequenceRowLabel();
			label.Name = node.Name;
			Row newRow = TimelineControl.AddRow(label, parentRow, 32);
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
			if ((e.Button == MouseButtons.Left) && _beginDragDrop)
			{
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

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
					TimelineControl.ruler.NudgeMark(-TimelineControl.ruler.StandardNudgeTime);
					break;
				case (Keys.Left | Keys.Shift):
					TimelineControl.ruler.NudgeMark(-TimelineControl.ruler.SuperNudgeTime);
					break;
				case Keys.Right:
					TimelineControl.ruler.NudgeMark(TimelineControl.ruler.StandardNudgeTime);
					break;
				case (Keys.Right | Keys.Shift):
					TimelineControl.ruler.NudgeMark(TimelineControl.ruler.SuperNudgeTime);
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			// do anything special we want to here: keyboard shortcuts that are in
			// the menu will be handled by them instead.
			switch (e.KeyCode)
			{
				//case Keys.Delete:
				//	TimelineControl.ruler.DeleteSelectedMarks();
				//	break;
				case Keys.Home:
					if (e.Control)
						TimelineControl.VisibleTimeStart = TimeSpan.Zero;
					else
						TimelineControl.VerticalOffset = 0;
					break;

				case Keys.End:
					if (e.Control)
						TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;
					else
						TimelineControl.VerticalOffset = int.MaxValue; // a bit iffy, but we know that the grid caps it to what's visible
					break;

				case Keys.PageUp:
					if (e.Control)
						TimelineControl.VisibleTimeStart -= TimelineControl.VisibleTimeSpan.Scale(0.5);
					else
						TimelineControl.VerticalOffset -= (TimelineControl.VisibleHeight / 2);
					break;

				case Keys.PageDown:
					if (e.Control)
						TimelineControl.VisibleTimeStart += TimelineControl.VisibleTimeSpan.Scale(0.5);
					else
						TimelineControl.VerticalOffset += (TimelineControl.VisibleHeight / 2);
					break;

				case Keys.Space:
					if (!_context.IsRunning)
						PlaySequence();
					else
					{
						if (_context.IsPaused)
							PlaySequence();
						else
							stoppedByUser = true;
							StopSequence();
					}
					break;

				case Keys.Left:
					if (e.Control)
						TimelineControl.MoveSelectedElementsByTime(TimelineControl.TimePerPixel.Scale(-2));
					break;

				case Keys.Right:
					if (e.Control)
						TimelineControl.MoveSelectedElementsByTime(TimelineControl.TimePerPixel.Scale(2));
					break;

				case Keys.OemMinus:
					if (e.Control && e.Shift)
						TimelineControl.ZoomRows(.8);
					else if (e.Control)
						TimelineControl.Zoom(1.25);
					break;

				case Keys.Oemplus:
					if (e.Control && e.Shift)
						TimelineControl.ZoomRows(1.25);
					else if (e.Control)
						TimelineControl.Zoom(.8);
					break;

				default:
					break;
			}
			// Prevents sending keystrokes to child controls. 
			// This was causing serious slowdowns if random keys were pressed.
			//e.SuppressKeyPress = true;
			base.OnKeyDown(e);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			VixenSystem.Contexts.ReleaseContext(_context);
		}

		#endregion

		#region Clipboard

		private void ClipboardAddData(bool cutElements)
		{
			if (!TimelineControl.SelectedElements.Any())
				return;

			TimelineElementsClipboardData result = new TimelineElementsClipboardData()
													{
														FirstVisibleRow = -1,
														EarliestStartTime = TimeSpan.MaxValue,
													};

			int rownum = 0;
			foreach (Row row in TimelineControl.VisibleRows)
			{
				// Since removals may happen during enumeration, make a copy with ToArray().
				foreach (Element elem in row.SelectedElements.ToArray())
				{
					if (result.FirstVisibleRow == -1)
						result.FirstVisibleRow = rownum;

					int relativeVisibleRow = rownum - result.FirstVisibleRow;

					TimelineElementsClipboardData.EffectModelCandidate modelCandidate =
						new TimelineElementsClipboardData.EffectModelCandidate(((TimedSequenceElement)elem).EffectNode.Effect)
							{
								Duration = elem.Duration,
								StartTime = elem.StartTime
							};
					result.EffectModelCandidates.Add(modelCandidate, relativeVisibleRow);

					if (elem.StartTime < result.EarliestStartTime)
						result.EarliestStartTime = elem.StartTime;

					if (cutElements)
					{
						row.RemoveElement(elem);
						_sequence.RemoveData(((TimedSequenceElement)elem).EffectNode);
						sequenceModified();
					}
				}
				rownum++;
			}

			IDataObject dataObject = new DataObject(_clipboardFormatName);
			dataObject.SetData(result);
			Clipboard.SetDataObject(dataObject, true);
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

			if (dataObject.GetDataPresent(_clipboardFormatName.Name))
			{
				data = dataObject.GetData(_clipboardFormatName.Name) as TimelineElementsClipboardData;
			}

			if (data == null)
				return result;
			TimeSpan offset = data.EarliestStartTime;
			Row targetRow = TimelineControl.SelectedRow ?? TimelineControl.ActiveRow ?? TimelineControl.TopVisibleRow;
			if (targetRow.Selected)
			{
				//Full row is selected, so paste as is from the beginning not the cursor position
				pasteTime = TimeSpan.Zero;
				//We don't need to offset, just place them where they start
				offset = TimeSpan.Zero;
			}
			List<Row> visibleRows = new List<Row>(TimelineControl.VisibleRows);
			int topTargetRoxIndex = visibleRows.IndexOf(targetRow);

			foreach (KeyValuePair<TimelineElementsClipboardData.EffectModelCandidate, int> kvp in data.EffectModelCandidates)
			{
				TimelineElementsClipboardData.EffectModelCandidate effectModelCandidate =
					kvp.Key as TimelineElementsClipboardData.EffectModelCandidate;
				int relativeRow = kvp.Value;

				int targetRowIndex = topTargetRoxIndex + relativeRow;
				TimeSpan targetTime = effectModelCandidate.StartTime - offset + pasteTime;
				if (targetTime > TimelineControl.grid.TotalTime)
				{
					continue;
				}
				else if (targetTime + effectModelCandidate.Duration > TimelineControl.grid.TotalTime)
				{
					//Shorten to fit.
					effectModelCandidate.Duration = TimelineControl.grid.TotalTime - targetTime;
				}
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
			stoppedByUser = false;
			StopSequence();
		}

		private void toolStripMenuItem_Loop_CheckedChanged(object sender, EventArgs e)
		{
			toolStripButton_Loop.Checked = toolStripMenuItem_Loop.Checked;
			if (toolStripButton_Loop.Checked && delayOffToolStripMenuItem.Checked != true)
			{
				//No way, we're not doing both! Turn off the delay.
				foreach (ToolStripMenuItem item in playOptionsToolStripMenuItem.DropDownItems)
				{
					item.Checked = false;
				}
				delayOffToolStripMenuItem.Checked = true;
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;
			}
		}

		private void delayOffToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 100;
			clearDelayPlayItemChecks();
			delayOffToolStripMenuItem.Checked = true;
			toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;
			toolStripButton_Play.ToolTipText = "Play F5";
		}

		private void delay5SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 5000;
			clearDelayPlayItemChecks();
			delay5SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = "5 Seconds";
		}

		private void delay10SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 10000;
			clearDelayPlayItemChecks();
			delay10SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = "10 Seconds";
		}

		private void delay20SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 20000;
			clearDelayPlayItemChecks();
			delay20SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = "20 Seconds";
		}

		private void delay30SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 30000;
			clearDelayPlayItemChecks();
			delay30SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = "30 Seconds";
		}

		private void delay60SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 60000;
			clearDelayPlayItemChecks();
			delay60SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = "60 Seconds";
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
			ClipboardPaste(TimelineControl.CursorPosition);
		}

		private void toolStripMenuItem_deleteElements_Click(object sender, EventArgs e)
		{
			if (TimelineControl.ruler.selectedMarks.Any())
			{
				TimelineControl.ruler.DeleteSelectedMarks();
			}
			else
				removeSelectedElements();
			{
				removeSelectedElements();
			}
		}

		private void selectAllElementsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.SelectAllElements();
		}

		private void toolStripMenuItem_EditEffect_Click(object sender, EventArgs e)
		{
			if (TimelineControl.SelectedElements.Any())
			{
				EditElements(TimelineControl.SelectedElements.Cast<TimedSequenceElement>());
			}
		}

		private void toolStripMenuItem_SnapTo_CheckedChanged(object sender, EventArgs e)
		{
			toolStripButton_SnapTo.Checked = toolStripMenuItem_SnapTo.Checked;
			TimelineControl.grid.EnableSnapTo = toolStripMenuItem_SnapTo.Checked;
		}

		// this seems to break the keyboard shortcuts; the key shortcuts don't get enabled again
		// until the menu is dropped down, which is annoying. These really should be enabled/disabled
		// on select of elements, but that's too annoying for now...
		//private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		//{
		//    toolStripMenuItem_EditEffect.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Cut.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Copy.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Paste.Enabled = _clipboard != null;		//TODO: fix this when clipboard fixed
		//}

		#endregion

		#region View Menu

		private void toolStripMenuItem_zoomTimeIn_Click(object sender, EventArgs e)
		{
			TimelineControl.Zoom(0.8);
		}

		private void toolStripMenuItem_zoomTimeOut_Click(object sender, EventArgs e)
		{
			TimelineControl.Zoom(1.25);
		}

		private void toolStripMenuItem_zoomRowsIn_Click(object sender, EventArgs e)
		{
			TimelineControl.ZoomRows(1.25);
		}

		private void toolStripMenuItem_zoomRowsOut_Click(object sender, EventArgs e)
		{
			TimelineControl.ZoomRows(0.8);
		}

		#endregion

		#region Tools Menu

		private void toolStripMenuItem_removeAudio_Click(object sender, EventArgs e)
		{
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
				DialogResult result =
					MessageBox.Show("Are you sure you want to remove the audio association?", "Remove existing audio?", MessageBoxButtons.YesNoCancel);
				if (result != System.Windows.Forms.DialogResult.Yes)
					return;
			}

			// we're going ahead and adding the new audio, so remove any of the old ones we found earlier
			foreach (IMediaModuleInstance module in modulesToRemove)
			{
				_sequence.RemoveMedia(module);
			}
			//Remove any associated audio from the timeline.
			TimelineControl.Audio = null;

			//Disable the menu item
			toolStripMenuItem_removeAudio.Enabled = false;

			sequenceModified();

		}

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
				DialogResult result =
					MessageBox.Show("Only one audio file can be associated with a sequence at a time. If you choose another, " +
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
				//Remove any associated audio from the timeline.
				TimelineControl.Audio = null;

				TimeSpan length = TimeSpan.Zero;
				if (newInstance is VixenModules.Media.Audio.Audio)
				{
					length = (newInstance as VixenModules.Media.Audio.Audio).MediaDuration;
					TimelineControl.Audio = newInstance as VixenModules.Media.Audio.Audio;
				}

				_UpdateTimingSourceToSelectedMedia();

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

				toolStripMenuItem_removeAudio.Enabled = true;

				sequenceModified();
			}
		}

		private void toolStripMenuItem_MarkManager_Click(object sender, EventArgs e)
		{
			ShowMarkManager();
		}

		private void modifySequenceLengthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string oldLength = _sequence.Length.ToString("m\\:ss\\.fff");
			Common.Controls.TextDialog prompt = new Common.Controls.TextDialog("Enter new sequence length:", "Sequence Length",
																			   oldLength, true);

			do
			{
				if (prompt.ShowDialog() != DialogResult.OK)
					break;

				TimeSpan time;
				bool success = TimeSpan.TryParseExact(prompt.Response, TimeFormats.PositiveFormats, null, out time);
				if (success)
				{
					SequenceLength = time;
					break;
				}
				else
				{
					MessageBox.Show("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'",
									"Error parsing time");
				}
			} while (true);
		}

		private void effectWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (EffectsForm.DockState == DockState.Unknown)
			{
				DockState dockState = EffectsForm.DockState;
				if (dockState == DockState.Unknown) dockState = DockState.DockLeft;
				EffectsForm.Show(dockPanel, dockState);
			}
			else
			{
				EffectsForm.Close();
			}
		}

		private void markWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MarksForm.DockState == DockState.Unknown)
			{
				DockState dockState = MarksForm.DockState;
				dockState = DockState.DockLeft;
				if (dockState == DockState.Unknown) dockState = DockState.DockLeft;
				MarksForm.Show(dockPanel, dockState);
			}
			else
			{
				MarksForm.Close();
			}
		}


		#endregion

		#endregion

		#region Toolbar buttons

		private void toolStripButton_Start_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
			TimelineControl.PlaybackStartTime = m_prevPlaybackStart = TimeSpan.Zero;
			TimelineControl.VisibleTimeStart = TimeSpan.Zero;
		}

		private void toolStripButton_Play_Click(object sender, EventArgs e)
		{
			PlaySequence();
		}

		private void toolStripButton_Stop_Click(object sender, EventArgs e)
		{
			stoppedByUser = true;
			StopSequence();
		}

		private void toolStripButton_Pause_Click(object sender, EventArgs e)
		{
			PauseSequence();
		}

		private void toolStripButton_End_Click(object sender, EventArgs e)
		{
			//TODO: JEMA - Check to see if this is functioning properly.
			TimelineControl.PlaybackStartTime = m_prevPlaybackEnd = _sequence.Length;

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

		private void toolStripButtonVirtualEffectsAdd_Click(object sender, EventArgs e)
		{
			IEnumerable<Element> selectedElements = TimelineControl.SelectedElements;
			switch (selectedElements.Count())
			{
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

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				toolStripExVirtualEffects_Clear();
				foreach (Guid g in dialog.virtualEffectsToRemove)
				{
					_virtualEffectLibrary.removeEffect(g);
				}
				LoadVirtualEffects();
			}
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

		private void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
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


		private void timelineControl_ElementsMovedNew(object sender, ElementsChangedTimesEventArgs e)
		{
			var action = new ElementsTimeChangedUndoAction(this, e.PreviousTimes, e.Type);
			_undoMgr.AddUndoAction(action);
		}

		public void SwapTimes(Dictionary<Element, ElementTimeInfo> changedElements)
		{
			foreach (KeyValuePair<Element, ElementTimeInfo> e in changedElements)
			{
				// Key is reference to actual element. Value is class with its times before move.
				// Swap the element's times with the saved times from before the move, so we can restore them later in redo.
				ElementTimeInfo.SwapTimes(e.Key, e.Value);
				TimelineControl.grid.RenderElement(e.Key);
			}
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
				{
					_sequence = (TimedSequence)value;
				}
				else
				{
					throw new NotImplementedException("Cannot use sequence type with a Timed Sequence Editor");
				}
				//loadSequence(value); 
			}
		}

		public IEditorModuleInstance OwnerModule { get; set; }

		void IEditorUserInterface.StartEditor()
		{
			Show();
		}

		void IEditorUserInterface.CloseEditor()
		{
			Close();
		}

		void IEditorUserInterface.EditorClosing()
		{

			dockPanel.SaveAsXml(settingsPath);
			XMLProfileSettings xml = new XMLProfileSettings();
			xml.PutSetting(string.Format("{0}/DockLeftPortion", this.Name), (int)dockPanel.DockLeftPortion);
			xml.PutSetting(string.Format("{0}/DockRihtPortion", this.Name), (int)dockPanel.DockLeftPortion);
			xml = null;
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
			get { return TimelineControl.PlaybackCurrentTime.HasValue; }
		}

		#endregion

		//*** only do this if the user agrees to do it
		private void _UpdateTimingSourceToSelectedMedia()
		{
			//This sucks so bad, I am so sorry.  Magic strings and everything, good god.
			TimingProviders timingProviders = new TimingProviders(_sequence);
			string[] mediaTimingSources;

			try
			{
				mediaTimingSources = timingProviders.GetAvailableTimingSources("Media");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}

			if (mediaTimingSources.Length > 0)
			{
				SelectedTimingProvider mediaTimingProvider = new SelectedTimingProvider("Media", mediaTimingSources.First());
				_sequence.SelectedTimingProvider = mediaTimingProvider;
				_SetTimingToolStripEnabledState();
			}
		}

		private void ShowMarkManager()
		{
			MarkManager manager = new MarkManager(new List<MarkCollection>(_sequence.MarkCollections), this, this, this);
			if (manager.ShowDialog() == DialogResult.OK)
			{
				_sequence.MarkCollections = manager.MarkCollections;
				populateGridWithMarks();
				sequenceModified();
				MarksForm.PopulateMarkCollectionsList(null);
			}
		}
	
		private void _SetTimingSpeed(float speed)
		{
			if (speed <= 0) throw new InvalidOperationException("Cannot have a speed of 0 or less.");

			_timingSpeed = speed;

			// If they haven't executed the sequence yet, the timing source member will not yet be set.
			if (_TimingSource != null)
			{
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
			else
			{
				ITiming timingSource = _sequence.GetTiming();
				this.toolStripButton_IncreaseTimingSpeed.Enabled =
					this.toolStripButton_DecreaseTimingSpeed.Enabled =
					this.toolStripLabel_TimingSpeed.Enabled = this.toolStripLabel_TimingSpeedLabel.Enabled =
				   timingSource != null && timingSource.SupportsVariableSpeeds;

			}
		}

		private void _PlaySequence(TimeSpan rangeStart, TimeSpan rangeEnd)
		{
			if (_context.IsRunning && _context.IsPaused)
			{
				_context.Resume();
				updateButtonStates();
			}
			else
			{
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

				if (_timingSource.SupportsVariableSpeeds)
				{
					_timingSource.Speed = _timingSpeed;
					this.toolStripButton_IncreaseTimingSpeed.Enabled =
						this.toolStripButton_DecreaseTimingSpeed.Enabled =
							this.toolStripLabel_TimingSpeed.Enabled = this.toolStripLabel_TimingSpeedLabel.Enabled = true;

				}
				else
				{
					_UpdateTimingSpeedDisplay();
					this.toolStripButton_IncreaseTimingSpeed.Enabled =
				   this.toolStripButton_DecreaseTimingSpeed.Enabled =
				   this.toolStripLabel_TimingSpeed.Enabled = this.toolStripLabel_TimingSpeedLabel.Enabled = false;
				}
			}
		}

		private void toolStripExVirtualEffects_Clear()
		{
			if (_virtualEffectLibrary == null)
				return;

			List<ToolStripItem> addList = new List<ToolStripItem>();
			List<ToolStripItem> removeList = new List<ToolStripItem>();
			foreach (ToolStripItem tsItem in toolStripExVirtualEffects.Items)
			{
				if (tsItem.Tag != null && (_virtualEffectLibrary.ContainsEffect((Guid)tsItem.Tag)))
				{
					removeList.Add(tsItem);
				}
			}
			foreach (ToolStripItem tsItem in removeList)
			{
				toolStripExVirtualEffects.Items.Remove(tsItem);
			}
		}

		private void saveVirtualEffect(IEffectModuleInstance moduleInstance)
		{
			if (_virtualEffectLibrary == null)
				return;

			VirtualEffectNameDialog dialog = new VirtualEffectNameDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_virtualEffectLibrary.addEffect(Guid.NewGuid(), dialog.effectName, moduleInstance.TypeId,
												moduleInstance.ParameterValues);
				toolStripExVirtualEffects_Clear();
				LoadVirtualEffects();
			}
		}

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private Task loadingTask = null;

		private void TimedSequenceEditorForm_Shown(object sender, EventArgs e)
		{
			var token = cancellationTokenSource.Token;
			this.Enabled = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			//loadingTask = Task.Factory.StartNew(() => loadSequence(_sequence), token);
			loadSequence(_sequence);
		}

		private void cboAudioDevices_TextChanged(object sender, EventArgs e)
		{
			Vixen.Sys.State.Variables.SelectedAudioDeviceIndex = cboAudioDevices.SelectedIndex;
		}

		private void cboAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			Vixen.Sys.State.Variables.SelectedAudioDeviceIndex = cboAudioDevices.SelectedIndex;
		}

		private void menuStrip_MenuActivate(object sender, EventArgs e)
		{
			effectWindowToolStripMenuItem.Checked = (EffectsForm.DockState != DockState.Unknown);
			markWindowToolStripMenuItem.Checked = (MarksForm.DockState != DockState.Unknown);
		}

		private void timerPostponePlay_Tick(object sender, EventArgs e)
		{
			timerPostponePlay.Enabled = timerDelayCountdown.Enabled = false;
			PlaySequence();
		}

		private void clearDelayPlayItemChecks()
		{
			//Make sure Looping is not enabled
			toolStripButton_Loop.Checked = toolStripMenuItem_Loop.Checked = false;
			foreach (ToolStripMenuItem item in playOptionsToolStripMenuItem.DropDownItems)
			{
				item.Checked = false;
			}
		}

		private void timerDelayCountdown_Tick(object sender, EventArgs e)
		{
			DelayCountDown--;
			toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", DelayCountDown);
		}

		private void timerLoop_Tick(object sender, EventArgs e)
		{
			timerLoop.Enabled = false;
			PlaySequence();
		}

		private void curveEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selector = new CurveLibrarySelector();
			selector.ShowDialog();
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

		public int FirstVisibleRow { get; set; }

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
					return (IModuleDataModel) ds.ReadObject(r);
			}
		}
	}

	public class TimeFormats
	{
		private static readonly string[] _positiveFormats = new string[]
		                                                    	{
		                                                    		@"m\:ss", @"m\:ss\.f", @"m\:ss\.ff", @"m\:ss\.fff",
		                                                    		@"\:ss", @"\:ss\.f", @"\:ss\.ff", @"\:ss\.fff",
		                                                    		@"%s", @"s\.f", @"s\.ff", @"s\.fff",
		                                                    	};

		private static readonly string[] _negativeFormats = new string[]
		                                                    	{
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