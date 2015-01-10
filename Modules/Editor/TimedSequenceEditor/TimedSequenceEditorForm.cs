using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Resources.Properties;
using NLog;
using Vixen;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module;
using Vixen.Module.App;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.State;
using VixenModules.App.ColorGradients;
using VixenModules.App.VirtualEffect;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Windows.Forms.Timer;
using VixenModules.Property.Color;


namespace VixenModules.Editor.TimedSequenceEditor
{

	public partial class TimedSequenceEditorForm : Form, IEditorUserInterface, ITiming
	{

		#region Member Variables

		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		// the sequence.
		private TimedSequence _sequence;
		
		// the program context we will be playing this sequence in: used to interact with the execution engine.
		private ISequenceContext _context;

		// the timing source this sequence will be executing against. Used to update times, etc.
		private ITiming _timingSource;

		// Delayed play countdown
		private int DelayCountDown;

		private readonly Timer _autoSaveTimer = new Timer();

		// Variables used by the add multiple effects dialog
		int am_LastEffectCount;
		TimeSpan am_LastStartTime;
		TimeSpan am_LastDuration;
		TimeSpan am_LastDurationBetween;

		// a mapping of effects in the sequence to the element that represent them in the grid.
		private Dictionary<EffectNode, Element> _effectNodeToElement;

		// a mapping of system elements to the (possibly multiple) rows that represent them in the grid.
		private Dictionary<ElementNode, List<Row>> _elementNodeToRows;

		// the default time for a sequence if one is loaded with 0 time
		private static readonly TimeSpan _defaultSequenceTime = TimeSpan.FromMinutes(1);

		// Undo manager
		private UndoManager _undoMgr;

		private TimeSpan? m_prevPlaybackStart = null;
		private TimeSpan? m_prevPlaybackEnd = null;

		private bool m_modified = false;

		private float _timingSpeed = 1;

		private float _timingChangeDelta = 0.25f;

		private static readonly DataFormats.Format _clipboardFormatName =
			DataFormats.GetFormat(typeof(TimelineElementsClipboardData).FullName);

		private VirtualEffectLibrary _virtualEffectLibrary;

		private readonly ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

		private string settingsPath;
		private string ColorCollectionsPath;

		private CurveLibrary _curveLibrary;
		private ColorGradientLibrary _colorGradientLibrary;
		private LipSyncMapLibrary _library;
		private List<ColorCollection> _ColorCollections = new List<ColorCollection>();
		
		//Used for color collections
		private static Random rnd = new Random();
		private PreCachingSequenceEngine _preCachingSequenceEngine;

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
			toolStripButton_DrawMode.Image = Resources.pencil;
			toolStripButton_DrawMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_SelectionMode.Image = Resources.cursor_arrow;
			toolStripButton_SelectionMode.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_DragBoxFilter.Image = Resources.table_select_big;
			toolStripButton_DragBoxFilter.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_IncreaseTimingSpeed.Image = Resources.plus;
			toolStripButton_IncreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButton_DecreaseTimingSpeed.Image = Resources.minus;
			toolStripButton_DecreaseTimingSpeed.DisplayStyle = ToolStripItemDisplayStyle.Image;

			foreach (ToolStripItem toolStripItem in toolStripDropDownButton_SnapToStrength.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += toolStripButtonSnapToStrength_MenuItem_Click;
				}
			}

			Execution.ExecutionStateChanged += OnExecutionStateChanged;
			_autoSaveTimer.Tick += AutoSaveEventProcessor;
		}

		private IDockContent DockingPanels_GetContentFromPersistString(string persistString)
		{
			if (persistString == typeof(Form_Effects).ToString())
				return EffectsForm;
			else if (persistString == typeof(Form_Grid).ToString())
				return GridForm;
			else if (persistString == typeof(Form_Marks).ToString())
				return MarksForm;
			else if (persistString == typeof(Form_ToolPalette).ToString())
				return ToolsForm;
			else
			{
				throw new NotImplementedException("Unable to find docking window type.");
			}
		}

		private void TimedSequenceEditorForm_Load(object sender, EventArgs e)
		{
			settingsPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen",
							   "TimedSequenceEditorForm.xml");
			ColorCollectionsPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen",
							   "ColorCollections.xml");

			if (File.Exists(settingsPath))
			{
				dockPanel.LoadFromXml(settingsPath, new DeserializeDockContent(DockingPanels_GetContentFromPersistString));
			}
			else
			{
				GridForm.Show(dockPanel);
				ToolsForm.Show(dockPanel, DockState.DockLeft);
				MarksForm.Show(dockPanel, DockState.DockLeft);
				EffectsForm.Show(dockPanel, DockState.DockLeft);
			}

			XMLProfileSettings xml = new XMLProfileSettings();
			
			//Get preferences
			_autoSaveTimer.Interval = xml.GetSetting(XMLProfileSettings.SettingType.Preferences, string.Format("{0}/AutoSaveInterval", Name), 300000);

			//Restore App Settings
			dockPanel.DockLeftPortion = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockLeftPortion", Name), 150);
			dockPanel.DockRightPortion = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockRightPortion", Name), 150);
			autoSaveToolStripMenuItem.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AutoSaveEnabled", Name), true);
			toolStripButton_SnapTo.Checked = toolStripMenuItem_SnapTo.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapToSelected", Name), true);
			PopulateSnapStrength(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings,	string.Format("{0}/SnapStrength", Name), 2));
			toolStripMenuItem_ResizeIndicator.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorEnabled", Name),false);
			toolStripButton_DrawMode.Checked = TimelineControl.grid.EnableDrawMode = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DrawModeSelected", Name), false);
			toolStripButton_SelectionMode.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SelectionModeSelected", Name), true);
			ToolsForm.LinkCurves = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolPaletteLinkCurves", Name), false);
			ToolsForm.LinkGradients = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolPaletteLinkGradients", Name), false);
			cADStyleSelectionBoxToolStripMenuItem.Checked = TimelineControl.grid.aCadStyleSelectionBox = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CadStyleSelectionBox", Name), false);
			CheckRiColorMenuItem(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorColor", Name), "Red"));

			foreach (ToolStripItem toolStripItem in toolStripDropDownButton_SnapToStrength.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					if(TimelineControl.grid.SnapStrength.Equals(Convert.ToInt32(toolStripMenuItem.Tag)))
					{
						toolStripMenuItem.PerformClick();
						break;
					}
				}
			}

			WindowState = FormWindowState.Normal;

			if (xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name),
				"Normal").Equals("Maximized"))
			{
				WindowState = FormWindowState.Maximized;
			}
			else
			{
				var desktopBounds = new Rectangle(new Point(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X),
								xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y)),new Size(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name), Size.Width),
							xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name), Size.Height)));
				if (IsVisibleOnAnyScreen(desktopBounds))
				{
					DesktopBounds = desktopBounds;
				}
			}
			
			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			TimelineControl.grid.RenderProgressChanged += OnRenderProgressChanged;

			TimelineControl.ElementChangedRows += ElementChangedRowsHandler;
			TimelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
			TimelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			TimelineControl.DataDropped += timelineControl_DataDropped;
			TimelineControl.ColorDropped += timelineControl_ColorDropped;
			TimelineControl.CurveDropped += timelineControl_CurveDropped;
			TimelineControl.GradientDropped += timelineControl_GradientDropped;

			TimelineControl.PlaybackCurrentTimeChanged += timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked += timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange += timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged += timelineControl_TimeRangeDragged;

			TimelineControl.MarkMoved += timelineControl_MarkMoved;
			TimelineControl.DeleteMark += timelineControl_DeleteMark;

			EffectsForm.EscapeDrawMode += EscapeDrawMode;

			MarksForm.MarkCollectionChecked += MarkCollection_Checked;
			MarksForm.EditMarkCollection += MarkCollection_Edit;
			MarksForm.ChangedMarkCollection += MarkCollection_Changed;

			ToolsForm.StartColorDrag += ToolPalette_ColorDrag;
			ToolsForm.StartCurveDrag += ToolPalette_CurveDrag;
			ToolsForm.StartGradientDrag += ToolPalette_GradientDrag;

			TimelineControl.SelectionChanged += TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown += TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged += TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			TimelineControl.CursorMoved += CursorMovedHandler;
			TimelineControl.ElementsSelected += timelineControl_ElementsSelected;
			TimelineControl.ContextSelected += timelineControl_ContextSelected;
			TimelineControl.SequenceLoading = false;
			TimelineControl.TimePerPixelChanged += TimelineControl_TimePerPixelChanged;
			TimelineControl.grid.SelectedElementsCloneDelegate = CloneElements;
			TimelineControl.grid.StartDrawMode += DrawElement;

			_virtualEffectLibrary =
				ApplicationServices.Get<IAppModuleInstance>(VirtualEffectLibraryDescriptor.Guid) as
				VirtualEffectLibrary;

			_curveLibrary = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;
			if (_curveLibrary != null)
			{
				_curveLibrary.CurveChanged += CurveLibrary_CurveChanged;
			}

			_colorGradientLibrary = ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;
			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientChanged += ColorGradientLibrary_CurveChanged;	
			}

			LoadAvailableEffects();
			PopulateDragBoxFilterDropDown();
			InitUndo();
			updateButtonStates();
			UpdatePasteMenuStates();
			LoadVirtualEffects();
			LoadColorCollections();

            _library = ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;

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
		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(rect));
		}


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
			TimelineControl.ColorDropped -= timelineControl_ColorDropped;
			TimelineControl.CurveDropped -= timelineControl_CurveDropped;
			TimelineControl.GradientDropped -= timelineControl_GradientDropped;

			TimelineControl.PlaybackCurrentTimeChanged -= timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked -= timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange -= timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged -= timelineControl_TimeRangeDragged;
			TimelineControl.MarkMoved -= timelineControl_MarkMoved;
			TimelineControl.DeleteMark -= timelineControl_DeleteMark;

			EffectsForm.EscapeDrawMode -= EscapeDrawMode;
			EffectsForm.Dispose();

			MarksForm.EditMarkCollection -= MarkCollection_Edit;
			MarksForm.MarkCollectionChecked -= MarkCollection_Checked;
			MarksForm.ChangedMarkCollection -= MarkCollection_Changed;
			MarksForm.Dispose();

			ToolsForm.StartColorDrag -= ToolPalette_ColorDrag;
			ToolsForm.StartCurveDrag -= ToolPalette_CurveDrag;
			ToolsForm.StartGradientDrag -= ToolPalette_GradientDrag;
			ToolsForm.Dispose();

			TimelineControl.SelectionChanged -= TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown -= TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged -= TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			TimelineControl.CursorMoved -= CursorMovedHandler;
			TimelineControl.ElementsSelected -= timelineControl_ElementsSelected;
			TimelineControl.ContextSelected -= timelineControl_ContextSelected;
			TimelineControl.TimePerPixelChanged -= TimelineControl_TimePerPixelChanged;
			TimelineControl.DataDropped -= timelineControl_DataDropped;

			Execution.ExecutionStateChanged -= OnExecutionStateChanged;
			_autoSaveTimer.Stop();
			_autoSaveTimer.Tick -= AutoSaveEventProcessor;

			if (_curveLibrary != null)
			{
				_curveLibrary.CurveChanged -= CurveLibrary_CurveChanged;
			}

			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientChanged -= ColorGradientLibrary_CurveChanged;
			}

			//GRRR - make the color collections a library at some point

			foreach (ToolStripItem toolStripItem in toolStripDropDownButton_SnapToStrength.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click -= toolStripButtonSnapToStrength_MenuItem_Click;
				}
			}

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
			
		}

		private void ToolPalette_ColorDrag(object sender, EventArgs e)
		{
			TimelineControl.grid.isColorDrop = true;
		}

		private void ToolPalette_CurveDrag(object sender, EventArgs e)
		{
			TimelineControl.grid.isCurveDrop = true;
		}

		private void ToolPalette_GradientDrag(object sender, EventArgs e)
		{
			TimelineControl.grid.isGradientDrop = true;
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

		private Form_ToolPalette _toolPaletteForm = null;
		public Form_ToolPalette ToolsForm
		{
			get
			{
				if (_toolPaletteForm != null && !_toolPaletteForm.IsDisposed)
				{
					return _toolPaletteForm;
				}
				else
				{
					_toolPaletteForm = new Form_ToolPalette(TimelineControl);
					return _toolPaletteForm;
				}
			}
		}

		private void MarkCollection_Checked(object sender, MarkCollectionArgs e)
		{
			PopulateMarkSnapTimes();
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
			}
		}

		private void PopulateDragBoxFilterDropDown()
		{
			ToolStripMenuItem dbfInvertMenuItem = new ToolStripMenuItem("Invert Selection");
			dbfInvertMenuItem.ShortcutKeys = Keys.Control | Keys.I;
			dbfInvertMenuItem.ShowShortcutKeys = true;
			dbfInvertMenuItem.MouseUp += (sender, e) =>
				{
					//Shortcut key counts as a .Click, so this goes here...
					toolStripDropDownButton_DragBoxFilter.ShowDropDown();
				};
			dbfInvertMenuItem.Click += (sender, e) =>
				{
					foreach (ToolStripMenuItem mnuItem in toolStripDropDownButton_DragBoxFilter.DropDownItems)
					{
						mnuItem.Checked = (mnuItem.Checked ? false : true);
					}
				};
			toolStripDropDownButton_DragBoxFilter.DropDownItems.Add(dbfInvertMenuItem);

			foreach (IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				//Populate Drag Box Filter drop down with effect types
				ToolStripMenuItem dbfMenuItem = new ToolStripMenuItem(effectDesriptor.EffectName, effectDesriptor.GetRepresentativeImage(48, 48));
				dbfMenuItem.CheckOnClick = true;
				dbfMenuItem.CheckStateChanged += (sender, e) =>
					{
						//OK, now I don't remember why I put this here, I think to make sure the list is updated when using the invert
						if (dbfMenuItem.Checked) TimelineControl.grid.DragBoxFilterTypes.Add(effectDesriptor.TypeId);
						else TimelineControl.grid.DragBoxFilterTypes.Remove(effectDesriptor.TypeId);
						//Either way...(the user is getting ready to use the filter)
						toolStripButton_DragBoxFilter.Checked = true;
					};
				dbfMenuItem.Click += (sender, e) =>
					{
						toolStripDropDownButton_DragBoxFilter.ShowDropDown();
					};
				toolStripDropDownButton_DragBoxFilter.DropDownItems.Add(dbfMenuItem);
			}
		}

		private void LoadAvailableEffects()
		{
			foreach (IEffectModuleDescriptor effectDesriptor in
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

		private void loadTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			updateToolStrip4(string.Format("Please Wait. Loading: {0}", loadingWatch.Elapsed));
		}

		private delegate void updateToolStrip4Delegate(string text);

		private void updateToolStrip4(string text)
		{
			if (InvokeRequired)
			{
				Invoke(new updateToolStrip4Delegate(updateToolStrip4), text);
			}
			else
			{
				toolStripStatusLabel4.Text = text;
				if (string.IsNullOrWhiteSpace(text))
				{
					Invalidate();
					Enabled = true;
					FormBorderStyle = FormBorderStyle.Sizable;
				}
			}
		}

		private System.Timers.Timer loadTimer = null;
		private Stopwatch loadingWatch = null;

		private void loadSequence(ISequence sequence)
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
				setTitleBarText();

				// update our program context with this sequence
				OpenSequenceContext(sequence);

				// clear out all the old data
				loadSystemNodesToRows();

				// load the new data: get all the commands in the sequence, and make a new element for each of them.
				_effectNodeToElement = new Dictionary<EffectNode, Element>();

				TimelineControl.grid.SuppressInvalidate = true; //Hold off invalidating the grid while we bulk load.
				TimelineControl.grid.SupressRendering = true; //Hold off rendering while we load elements. 
				// This takes quite a bit of time so queue it up
				taskQueue.Enqueue(Task.Factory.StartNew(() => addElementsForEffectNodes(_sequence.SequenceData.EffectData)));
				

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

				//This path is followed for new and existing sequences so we need to determine which we have and set modified accordingly.
				//Added logic to determine if the sequence has a filepath to set modified JU 8/1/2012. 
				PopulateAudioDropdown();
				_SetTimingToolStripEnabledState();

				if (String.IsNullOrEmpty(_sequence.FilePath))
				{
					sequenceModified();
				}
				else
				{
					sequenceNotModified();
				}

				MarksForm.Sequence = _sequence;
				MarksForm.PopulateMarkCollectionsList(null);
				PopulateMarkSnapTimes();

				if (_sequence.TimePerPixel > TimeSpan.Zero )
				{
					TimelineControl.TimePerPixel = _sequence.TimePerPixel;	
				}

				
				
				Logging.Debug(string.Format("Sequence {0} took {1} to load. ", sequence.Name, loadingWatch.Elapsed));
			}
			catch (Exception ee)
			{
				Logging.Error("Error loading sequence.", ee);
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

		private void SaveColorCollections()
		{
			var xmlsettings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof(List<ColorCollection>));
			var dataWriter = XmlWriter.Create(ColorCollectionsPath, xmlsettings);
			dataSer.WriteObject(dataWriter, _ColorCollections);
			dataWriter.Close();
		}

		private void LoadColorCollections()
		{
			if (System.IO.File.Exists(ColorCollectionsPath))
			{
				using (FileStream reader = new FileStream(ColorCollectionsPath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<ColorCollection>));
					_ColorCollections = (List<ColorCollection>)ser.ReadObject(reader);
				}
			}
		}

		#endregion

		#region Other Private Methods

		private void SetAutoSave()
		{
			if (autoSaveToolStripMenuItem.Checked && IsModified)
			{
				_autoSaveTimer.Start();
			}
			else
			{
				_autoSaveTimer.Stop();
			}
		}

		/// <summary>
		/// Populates the mark snaptimes in the grid.
		/// </summary>
		private void PopulateMarkSnapTimes()
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

		private void PopulateSnapStrength(int strength)
		{
			TimelineControl.grid.SnapStrength = strength;
			TimelineControl.ruler.SnapStrength = strength;
		}

		private void PopulateAudioDropdown()
		{
			if (InvokeRequired)
			{
				Invoke(new Delegates.GenericDelegate(PopulateAudioDropdown));
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
						PopulateWaveformAudio();
					}
				}
			}
		}

		private void PopulateWaveformAudio()
		{
			if (_sequence.GetAllMedia().Any())
			{
				IMediaModuleInstance media = _sequence.GetAllMedia().First();
				Audio audio = media as Audio;
				toolStripMenuItem_removeAudio.Enabled = true;
				if (audio.MediaExists)
				{
					TimelineControl.Audio = audio;	
				} else
				{
					string message = String.Format("Audio file not found on the path:\n\n {0}\n\nPlease Check your settings/path.",
						audio.MediaFilePath);
					Logging.Warn(message);
					MessageBox.Show(message, @"Missing audio file");
				}
				
			}
			
		}

		/// <summary>
		/// Called to update the title bar with the filename and saved / unsaved status
		/// </summary>
		private void setTitleBarText()
		{
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(setTitleBarText));
			else
			{
				//Set sequence name in title bar based on the module name and current sequence name JU 8/1/2012
				//Made this more generic to support importing 12 FEB 2013 - JEMA
				Text = String.Format("{0} - [{1}{2}]", ((OwnerModule.Descriptor) as EditorModuleDescriptorBase).TypeName,
					String.IsNullOrEmpty(_sequence.Name)?"Untitled":_sequence.Name, IsModified ? " *" : "");
			}
		}

		/// <summary>Called when the sequence is modified.</summary>
		private void sequenceModified()
		{
			if (!m_modified)
			{
				m_modified = true;
				setTitleBarText();
				SetAutoSave();
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
				SetAutoSave();
				// TODO: Other things, like disable save button, etc.	
			}
			
		}

		#endregion

		#region Library Application Private Methods

		//Switch statements are the best method at this time for these methods

		private void ApplyColorGradientToEffects(String LibraryReferenceName, ColorGradient colorGradient)
		{
			bool strayElement = false;

			foreach (Element elem in TimelineControl.SelectedElements)
			{
				ColorGradient newColorGradient = new ColorGradient(colorGradient);
				newColorGradient.LibraryReferenceName = LibraryReferenceName;
				newColorGradient.IsCurrentLibraryGradient = false;

				string effectName = elem.EffectNode.Effect.EffectName;
				object[] parms = new object[elem.EffectNode.Effect.ParameterValues.Count()];
				Array.Copy(elem.EffectNode.Effect.ParameterValues, parms, parms.Count());

				switch (effectName)
				{
					case "Alternating":
					case "Candle Flicker":
					case "Custom Value":
					case "LipSync":
					case "Nutcracker":
					case "Set Level":
					case "Launcher":
					case "RDS":
						strayElement = true;
						break;
					case "Pulse":
						parms[1] = newColorGradient;
						break;
					case "Chase":
						parms[0] = 1; //GradientThroughWholeEffect;
						parms[4] = newColorGradient;
						break;
					case "Spin":
						parms[2] = 1; //GradientThroughWholeEffect;
						parms[10] = newColorGradient;
						break;
					case "Twinkle":
						parms[7] = 1; //GradientThroughWholeEffect;
						parms[9] = newColorGradient;
						break;
					case "Wipe":
						parms[0] = newColorGradient;
						break;
				}

				elem.EffectNode.Effect.ParameterValues = parms;

				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
			if (strayElement) MessageBox.Show("One or more effects were selected that do not support color gradients.\nAll effects that do were updated.");
		}

		private void ApplyCurveToEffects(String LibraryReferenceName, Curve curve)
		{
			bool strayElement = false;

			foreach (Element elem in TimelineControl.SelectedElements)
			{
				Curve newCurve = new Curve(curve);
				newCurve.LibraryReferenceName = LibraryReferenceName;
				newCurve.IsCurrentLibraryCurve = false;

				string effectName = elem.EffectNode.Effect.EffectName;
				object[] parms = new object[elem.EffectNode.Effect.ParameterValues.Count()];
				Array.Copy(elem.EffectNode.Effect.ParameterValues, parms, parms.Count());

				switch (effectName)
				{
					case "Alternating":
					case "Candle Flicker":
					case "Custom Value":
					case "LipSync":
					case "Nutcracker":
					case "Set Level":
					case "Twinkle":
					case "Launcher":
					case "RDS":
						strayElement = true;
						break;
					case "Pulse":
						parms[0] = newCurve;
						break;
					case "Chase":
						parms[5] = newCurve;
						break;
					case "Spin":
						parms[11] = newCurve;
						break;
					case "Wipe":
						parms[2] = newCurve;
						break;
				}

				elem.EffectNode.Effect.ParameterValues = parms;

				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
			if (strayElement) MessageBox.Show("One or more effects were selected that do not support curves.\nAll effects that do were updated.");
		}

		private void ApplyColorCollection(ColorCollection Collection, bool RandomOrder)
		{
			if (!Collection.Color.Any())
				return;

			bool strayElement = false;
			Color thisColor, thisColor2 = Color.White;
			int iPos = 0;

			foreach (Element elem in TimelineControl.SelectedElements)
			{
				string effectName = elem.EffectNode.Effect.EffectName;
				object[] parms = new object[elem.EffectNode.Effect.ParameterValues.Count()];
				List<Color> validColors = new List<Color>();

				Array.Copy(elem.EffectNode.Effect.ParameterValues, parms, parms.Count());
				validColors.AddRange(elem.EffectNode.Effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));

				if (RandomOrder)
				{
					int r1 = rnd.Next(Collection.Color.Count());
					int r2 = rnd.Next(Collection.Color.Count());

					int n = 0;
					while (r1 == r2 && n <= 5)
					{
						r2 = rnd.Next(Collection.Color.Count());
						n++;
					}

					thisColor = Collection.Color[r1];
					thisColor2 = Collection.Color[r2];
				}
				else
				{
					if (iPos == Collection.Color.Count()) { iPos = 0; }
					thisColor = Collection.Color[iPos];
					iPos++;
					if (effectName == "Alternating")
					{
						thisColor2 = Collection.Color[iPos];
						iPos++;
					}
				}

				if (validColors.Any() && !validColors.Contains(thisColor)) { thisColor = validColors[rnd.Next(validColors.Count())]; }

				if (effectName == "Alternate")
				{
					if (validColors.Any() && !validColors.Contains(thisColor2)) { thisColor2 = validColors[rnd.Next(validColors.Count())]; }

					int n2 = 0;
					while (thisColor2 == thisColor && n2 <= 5)
					{
						thisColor2 = validColors[rnd.Next(validColors.Count())];
						n2++;
					}
				}

				switch (effectName)
				{
					case "Candle Flicker":
					case "LipSync":
					case "Nutcracker":
					case "Launcher":
					case "RDS":
						strayElement = true;
						break;
					case "Custom Value":
						//Disabled until we fix the custom value null reference errors - not related to this.
						//parms[0] = 4; //Set it to a type of color value
						//parms[5] = thisColor;
						strayElement = true;
						break;
					case "Alternating":
						parms[1] = thisColor;
						parms[3] = thisColor2;
						parms[8] = parms[9] = true;
						break;
					case "Set Level":
						parms[1] = thisColor;
						break;
					case "Pulse":
						parms[1] = new ColorGradient(thisColor);
						break;
					case "Chase":
						parms[0] = 0; // StaticColor
						parms[3] = thisColor;
						break;
					case "Spin":
						parms[2] = 0; // StaticColor
						parms[9] = thisColor;
						break;
					case "Twinkle":
						parms[7] = 0; // StaticColor
						parms[8] = thisColor;
						break;
					case "Wipe":
						parms[0] = new ColorGradient(thisColor);
						break;
				}

				elem.EffectNode.Effect.ParameterValues = parms;
				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
			if (strayElement) MessageBox.Show("One or more effects were selected that do not support curves.\nAll effects that do were updated.");
		}

		#endregion

		#region Event Handlers

		private void CurveLibrary_CurveChanged(object sender, EventArgs e)
		{
			CheckAndRenderDirtyElements();
		}

		private void ColorGradientLibrary_CurveChanged(object sender, EventArgs e)
		{
			CheckAndRenderDirtyElements();
		}

		private void AutoSaveEventProcessor(object sender, EventArgs e)
		{
			_autoSaveTimer.Stop();
			if (IsModified)
			{
				Save();
			}
		}

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
			catch(Exception ex)
			{
				Logging.Error("TimedSequenceEditor: Error updating rendering progress indicator.",ex);
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

		//Sorry about this, was the only way I could find to handle the escape press if
		//the effects tree still had focus. Because... someone will do this......
		protected void EscapeDrawMode(object sender, EventArgs e)
		{
			EffectsForm.DeselectAllNodes();
			TimelineControl.grid.EnableDrawMode = false;
			toolStripButton_DrawMode.Checked = false;
			toolStripButton_SelectionMode.Checked = true;
		}

		protected void DrawElement(object sender, DrawElementEventArgs e)
		{
				//Make sure we have enough of an effect to show up
				if (e.Duration > TimeSpan.FromSeconds(.010))
				{
					var newEffects = new List<EffectNode>();
					foreach (Row drawingRow in e.Rows)
					{
						var newEffect = ApplicationServices.Get<IEffectModuleInstance>(e.Guid);
						try
						{
							newEffects.Add(CreateEffectNode(newEffect, drawingRow, e.StartTime,e.Duration));
						}
						catch (Exception ex)
						{
							string msg = "TimedSequenceEditor DrawMultipleElements: error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
											((drawingRow == null) ? "<null>" : drawingRow.Name);
							Logging.Error(msg, ex);
						}
					}
					AddEffectNodes(newEffects);
					sequenceModified();
					var act = new EffectsAddedUndoAction(this, newEffects);
					_undoMgr.AddUndoAction(act);
					SelectEffectNodes(newEffects);
				}
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
			EditElements(new[] { element });
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

		private void TimelineControl_TimePerPixelChanged(object sender, EventArgs e)
		{
			if (_sequence.TimePerPixel != TimelineControl.TimePerPixel)
			{
				_sequence.TimePerPixel = TimelineControl.TimePerPixel;
				sequenceModified();	
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
				menuItem.ToolTipText = "Use Shift key to add multiple effects of the same type.";
				menuItem.Click += (mySender, myE) =>
				{
					if (e.Row != null)
					{
						//Modified 7-9-2014 J. Bolding - Changed so that the multiple element addition is wrapped into one action by the undo/redo engine.
						if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == (Keys.Shift | Keys.Control))
						{
							//add multiple here
							AddMultipleEffects(e.GridTime,effectDesriptor.EffectName,(Guid)menuItem.Tag,e.Row);
						}
						else
							addNewEffectById((Guid)menuItem.Tag, e.Row, e.GridTime, TimeSpan.FromSeconds(2));
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
					//Disables the Alignment menu if too many effects are selected in a row.
					itemAlignment.Enabled = TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements);
					if (!itemAlignment.Enabled)
					{
						itemAlignment.ToolTipText = "Disabled, maximum selected effects per row is 4.";
					}

					ToolStripMenuItem itemAlignStart = new ToolStripMenuItem("Align Start Times (shift)");
					itemAlignStart.ToolTipText = "Holding shift will align the start times, while holding duration.";
					itemAlignStart.Click += (mySender, myE) => TimelineControl.grid.AlignElementStartTimes(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);

					ToolStripMenuItem itemAlignEnd = new ToolStripMenuItem("Align End Times (shift)");
					itemAlignEnd.ToolTipText = "Holding shift will align the end times, while holding duration.";
					itemAlignEnd.Click += (mySender, myE) => TimelineControl.grid.AlignElementEndTimes(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);

					ToolStripMenuItem itemAlignBoth = new ToolStripMenuItem("Align Both Times");
					itemAlignBoth.Click += (mySender, myE) => TimelineControl.grid.AlignElementStartEndTimes(TimelineControl.SelectedElements, element);

					ToolStripMenuItem itemMatchDuration = new ToolStripMenuItem("Match Duration (shift)");
					itemMatchDuration.ToolTipText = "Holding shift will hold the effects end time and adjust the start time, by default the end time is adjusted.";
					itemMatchDuration.Click += (mySender, myE) => TimelineControl.grid.AlignElementDurations(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
					
					ToolStripMenuItem itemAlignStartToEnd = new ToolStripMenuItem("Align Start to End");
					itemAlignStartToEnd.Click += (mySender, myE) => TimelineControl.grid.AlignElementStartToEndTimes(TimelineControl.SelectedElements, element);
					
					ToolStripMenuItem itemAlignEndToStart = new ToolStripMenuItem("Align End to Start");
					itemAlignEndToStart.Click += (mySender, myE) => TimelineControl.grid.AlignElementEndToStartTime(TimelineControl.SelectedElements, element);
					
					ToolStripMenuItem itemDistDialog = new ToolStripMenuItem("Distribute Effects");
					itemDistDialog.Click += (mySender, myE) => DistributeSelectedEffects();

					ToolStripMenuItem itemAlignCenter = new ToolStripMenuItem("Align Centerpoints");
					itemAlignCenter.Click += (mySender, myE) => TimelineControl.grid.AlignElementCenters(TimelineControl.SelectedElements, element);

					ToolStripMenuItem itemDistributeEqually = new ToolStripMenuItem("Distribute Equally");
					itemDistributeEqually.ToolTipText = "This will stair step the selected elements, starting with the element that has the earlier start point on the time line.";
					itemDistributeEqually.Click += (mySender, myE) => DistributeSelectedEffectsEqually();

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
				//Add Edit/Delete/Collections
				contextMenuStrip.Items.Add("-");
				contextMenuStrip.Items.Add(toolStripMenuItem_EditEffect);
				contextMenuStrip.Items.Add(toolStripMenuItem_deleteElements);
				contextMenuStrip.Items.Add("-");

				AddContextCollectionsMenu();

			}

			e.AutomaticallyHandleSelection = false;

			contextMenuStrip.Show(MousePosition);
		}

		private void AddContextCollectionsMenu()
		{
			ToolStripMenuItem itemCollections = new ToolStripMenuItem("Collections");
			contextMenuStrip.Items.Add(itemCollections);

			if (TimelineControl.SelectedElements.Count() > 1 && _ColorCollections.Any())
			{
				ToolStripMenuItem itemColorCollections = new ToolStripMenuItem("Colors");
				ToolStripMenuItem itemRandomColors = new ToolStripMenuItem("Random");
				ToolStripMenuItem itemSequentialColors = new ToolStripMenuItem("Sequential");

				itemCollections.DropDown.Items.Add(itemColorCollections);
				itemColorCollections.DropDown.Items.Add(itemRandomColors);
				itemColorCollections.DropDown.Items.Add(itemSequentialColors);

				foreach (ColorCollection Collection in _ColorCollections)
				{
					if (Collection.Color.Any())
					{
						ToolStripMenuItem menuRandomColorItem = new ToolStripMenuItem(Collection.Name);
						menuRandomColorItem.ToolTipText = Collection.Description;
						menuRandomColorItem.Click += (mySender, myE) => ApplyColorCollection(Collection, true);
						itemRandomColors.DropDown.Items.Add(menuRandomColorItem);
						
						ToolStripMenuItem menuSequentialColorItem = new ToolStripMenuItem(Collection.Name);
						menuSequentialColorItem.ToolTipText = Collection.Description;
						menuSequentialColorItem.Click += (mySender, myE) => ApplyColorCollection(Collection, false);
						itemSequentialColors.DropDown.Items.Add(menuSequentialColorItem);	
					}
				}
			}

			if (_curveLibrary.Any())
			{
				ToolStripMenuItem itemCurveLibrary = new ToolStripMenuItem("Curves");
				itemCollections.DropDown.Items.Add(itemCurveLibrary);
				foreach (KeyValuePair<string, Curve> curve in _curveLibrary)
				{
					ToolStripMenuItem itemCurve = new ToolStripMenuItem(curve.Key);
					itemCurve.Click += (mySender, myE) => ApplyCurveToEffects(curve.Key, curve.Value);
					itemCurveLibrary.DropDown.Items.Add(itemCurve);
				}
			}

			if (_colorGradientLibrary.Any())
			{
				ToolStripMenuItem itemColorGradientLibrary = new ToolStripMenuItem("Color Gradient");
				itemCollections.DropDown.Items.Add(itemColorGradientLibrary);
				foreach (KeyValuePair<string, ColorGradient> colorGradient in _colorGradientLibrary)
				{
					ToolStripMenuItem itemColorGradient = new ToolStripMenuItem(colorGradient.Key);
					itemColorGradient.Click += (mySender, myE) => ApplyColorGradientToEffects(colorGradient.Key, colorGradient.Value);
					itemColorGradientLibrary.DropDown.Items.Add(itemColorGradient);
				}
			}
		}

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
		}


		private void toolStripMenuItem_ResizeIndicator_CheckStateChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.ResizeIndicator_Enabled = toolStripMenuItem_ResizeIndicator.Checked;
		}


		private void CheckRiColorMenuItem(string color)
		{
			TimelineControl.grid.ResizeIndicator_Color = color;
			toolStripMenuItem_RIColor_Blue.Checked = color == "Blue";
			toolStripMenuItem_RIColor_Yellow.Checked = color == "Yellow";
			toolStripMenuItem_RIColor_Green.Checked = color == "Green";
			toolStripMenuItem_RIColor_White.Checked = color == "White";
			toolStripMenuItem_RIColor_Red.Checked = color == "Red";
		}
		private void toolStripMenuItem_RIColor_Blue_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Blue");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Yellow_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Yellow");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Green_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Green");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_White_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("White");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Red_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Red");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripButton_DragBoxFilter_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.DragBoxFilterEnabled = toolStripButton_DragBoxFilter.Checked;
		}

		private void ColorCollectionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ColorCollectionLibrary_Form RCCF = new ColorCollectionLibrary_Form(new List<ColorCollection>(_ColorCollections));
			if (RCCF.ShowDialog() == DialogResult.OK)
			{
				_ColorCollections = RCCF.ColorCollections;
				SaveColorCollections();
			}
			else
			{
				LoadColorCollections();
			}
		}

		private void AddMultipleEffects(TimeSpan StartTime, String EffectName, Guid EffectID, Row Row)
		{
			var eDialog = new Form_AddMultipleEffects();
			if (Control.ModifierKeys == (Keys.Shift | Keys.Control) && am_LastEffectCount > 0)
			{
				eDialog.EffectCount = am_LastEffectCount;
				eDialog.StartTime = am_LastStartTime;
				eDialog.Duration = am_LastDuration;
				eDialog.DurationBetween = am_LastDurationBetween;
			}
			else
			{
				eDialog.EffectCount = 2;
				eDialog.StartTime = StartTime;
				eDialog.Duration = TimeSpan.FromSeconds(2);
				eDialog.DurationBetween = TimeSpan.FromSeconds(2);
			}
			eDialog.EffectName = EffectName;
			eDialog.SequenceLength = eDialog.EndTime = SequenceLength;
			eDialog.MarkCollections = _sequence.MarkCollections;
			eDialog.ShowDialog();

			if (eDialog.DialogResult == DialogResult.OK)
			{
				am_LastEffectCount = eDialog.EffectCount;
				am_LastStartTime = eDialog.StartTime;
				am_LastDuration = eDialog.Duration;
				am_LastDurationBetween = eDialog.DurationBetween;

				var newEffects = new List<EffectNode>();

				if (eDialog.AlignToBeatMarks)
				{
					newEffects = AddEffectsToBeatMarks(eDialog.CheckedMarks, eDialog.EffectCount, EffectID, eDialog.StartTime, eDialog.Duration, Row, eDialog.FillDuration, eDialog.SkipEOBeat);
				}
				else
				{
					TimeSpan NextStartTime = eDialog.StartTime;
					for (int i = 0; i < eDialog.EffectCount; i++)
					{
						if (NextStartTime + eDialog.Duration > SequenceLength)
						{
							//if something went wrong in the forms calculations
							break;
						}
						else
						{
							var newEffect = ApplicationServices.Get<IEffectModuleInstance>(EffectID);
							try
							{
								newEffects.Add(CreateEffectNode(newEffect, Row, NextStartTime, eDialog.Duration));
								NextStartTime = NextStartTime + eDialog.Duration + eDialog.DurationBetween;
							}
							catch (Exception ex)
							{
								string msg = "TimedSequenceEditor AddMultipleElements: error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
											 ((Row == null) ? "<null>" : Row.Name);
								Logging.Error(msg, ex);
							}
						}
					}
					AddEffectNodes(newEffects);
					sequenceModified();
					var act = new EffectsAddedUndoAction(this, newEffects);
					_undoMgr.AddUndoAction(act);
				}
				if (newEffects.Count > 0)
				{
					if (eDialog.SelectEffects || eDialog.EditEffects) SelectEffectNodes(newEffects);
					if (eDialog.EditEffects && TimelineControl.SelectedElements.Any())
					{
						EditElements(TimelineControl.SelectedElements.Cast<TimedSequenceElement>());
					}

				}
			}
		}
		private List<EffectNode> AddEffectsToBeatMarks(ListView.CheckedListViewItemCollection CheckedMarks, int EffectCount, Guid EffectGuid, TimeSpan StartTime, TimeSpan Duration, Row Row, Boolean FillDuration, Boolean SkipEOBeat)
		{
			List<TimeSpan> Times = new List<TimeSpan>();
			bool SkipThisBeat = false;

			foreach (ListViewItem ListItem in CheckedMarks)
			{
				foreach (MarkCollection MCItem in _sequence.MarkCollections)
				{
					if (MCItem.Name == ListItem.Text)
					{
						foreach (TimeSpan Mark in MCItem.Marks)
						{
							if (Mark >= StartTime) Times.Add(Mark);
						}
					}
				}
			}

			Times.Sort();

			var newEffects = new List<EffectNode>();

			if (Times.Count > 0)
			{
				foreach (TimeSpan Mark in Times)
				{
					if (newEffects.Count < EffectCount)
					{
						if (!SkipEOBeat || (SkipEOBeat && !SkipThisBeat))
						{
							var newEffect = ApplicationServices.Get<IEffectModuleInstance>(EffectGuid);
							try
							{
								if (FillDuration)
								{
									if (Times.IndexOf(Mark) == Times.Count - 1) //The dialog hanles this, but just to make sure
										break; //We're done -- There are no more marks to fill, don't create it
									Duration = Times[Times.IndexOf(Mark) + 1] - Mark;
									if (Duration < TimeSpan.FromSeconds(.01)) Duration = TimeSpan.FromSeconds(.01);
								}
								newEffects.Add(CreateEffectNode(newEffect, Row, Mark, Duration));
							}
							catch (Exception ex)
							{
								string msg = "TimedSequenceEditor AddMultipleElements: error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
											 ((Row == null) ? "<null>" : Row.Name);
								Logging.Error(msg, ex);
							}
						}
						SkipThisBeat = (SkipThisBeat ? false : true);
					}
					else
						break; //We're done creating, we've matched counts
				}

				AddEffectNodes(newEffects);
				sequenceModified();
				var act = new EffectsAddedUndoAction(this, newEffects);
				_undoMgr.AddUndoAction(act);
			}

			return newEffects;
		}

		private void DistributeSelectedEffectsEqually()
		{
			if (!TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements))
			{
				MessageBox.Show(TimelineControl.grid.alignmentHelperWarning);
				return;
			}

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
			var effectDuration = totalDuration.TotalSeconds/totalElements;
			TimeSpan effectTS = TimeSpan.FromSeconds(effectDuration);
			//var msgString = string.Format("Total Elements: {0}\n Start Time: {1}\n End Time: {2}\n Total Duration: {3}\n Effect Duration: {4}\n TimeSpan Duration: {5}\n Start at last element: {6}", totalElements,startTime,endTime,totalDuration,effectDuration, effectTS.TotalSeconds, startAtLastElement);
			//MessageBox.Show(msgString);
			//Sanity Check - Keep effects from becoming less than minimum.
			if (effectDuration < .001)
			{
				MessageBox.Show(
					string.Format(
						"Unable to complete request. The resulting duration would fall below 1 millisecond.\nCalculated duration: {0}",
						effectDuration), "Warning", MessageBoxButtons.OK);
				return;
			}

			var elementsToDistribute = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			if (!startAtLastElement)
			{
				//Lets move the first one
				elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(0),
							new Tuple<TimeSpan, TimeSpan>(startTime, startTime + effectTS));
				for (int i = 1; i <= totalElements - 1; i++)
				{
					var thisStartTime = elementsToDistribute.Last().Value.Item2;
					elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i), new Tuple<TimeSpan, TimeSpan>(thisStartTime, thisStartTime + effectTS));
				}
			}
			else
			{
				//Lets move the first(last) one
				elementsToDistribute.Add(TimelineControl.SelectedElements.Last(), new Tuple<TimeSpan, TimeSpan>(startTime, startTime + effectTS));
				for (int i = totalElements - 2; i >= 0; i--)
				{
					var thisStartTime = elementsToDistribute.Last().Value.Item2; 
					elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i), new Tuple<TimeSpan, TimeSpan>(thisStartTime, thisStartTime + effectTS));
				}
			}

			if (elementsToDistribute.Any())
			{
				TimelineControl.grid.MoveResizeElements(elementsToDistribute, ElementMoveType.Distribute);
			}
		}

		private void DistributeSelectedEffects()
		{
			if (!TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements))
			{
				MessageBox.Show(TimelineControl.grid.alignmentHelperWarning);
				return;
			}

			var startTime = TimelineControl.SelectedElements.First().StartTime;
			var endTime = TimelineControl.SelectedElements.Last().EndTime;
			if (startTime > endTime)
			{
				startTime = TimelineControl.SelectedElements.Last().StartTime;
				endTime = TimelineControl.SelectedElements.First().EndTime;
			}
			var dDialog = new EffectDistributionDialog();
			var elementCount = TimelineControl.SelectedElements.Count();

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
				TimeSpan duration = endTime - startTime;
				double offset = duration.TotalSeconds/elementCount;

				var elementsToDistribute = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
				if (dDialog.StartWithFirst)
				{
					//We start with the first effect
					for (int i = 0; i <= elementCount - 1; i++)
					{
						double thisStartTime = startTime.TotalSeconds;
						double thisEndTime = thisStartTime + offset;
						//Generic placement of starttime eq to prev end time
						if (i > 0)
							thisStartTime = elementsToDistribute.Last().Value.Item2.TotalSeconds;
						//Determine Start time
						if (i > 0 && dDialog.RadioEffectPlacementOverlap)
							thisStartTime = thisStartTime - Convert.ToDouble(dDialog.EffectPlacementOverlap.TotalSeconds);
						if (i > 0 && dDialog.RadioPlacementSpacedDuration)
							thisStartTime = thisStartTime + Convert.ToDouble(dDialog.SpacedPlacementDuration.TotalSeconds);
						if (dDialog.RadioDoNotChangeDuration && !dDialog.RadioEffectPlacementOverlap &&
						    !dDialog.RadioPlacementSpacedDuration)
							thisStartTime = startTime.TotalSeconds + (offset*i);
						//Determine End time
						if (dDialog.RadioEqualDuration)
							thisEndTime = thisStartTime + offset;
						if (dDialog.RadioDoNotChangeDuration)
							thisEndTime = thisStartTime + TimelineControl.SelectedElements.ElementAt(i).Duration.TotalSeconds;
						if (dDialog.RadioSpecifiedDuration)
							thisEndTime = thisStartTime + Convert.ToDouble(dDialog.SpecifiedEffectDuration.TotalSeconds);
						elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i),
							new Tuple<TimeSpan, TimeSpan>(TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisEndTime)));
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
							thisStartTime = elementsToDistribute.Last().Value.Item2.TotalSeconds;
						//Determine Start time
						if (i < elementCount - 1 && dDialog.RadioEffectPlacementOverlap)
							thisStartTime = thisStartTime - Convert.ToDouble(dDialog.EffectPlacementOverlap.TotalSeconds);
						if (i < elementCount - 1 && dDialog.RadioPlacementSpacedDuration)
							thisStartTime = thisStartTime + Convert.ToDouble(dDialog.SpacedPlacementDuration.TotalSeconds);
						if (dDialog.RadioDoNotChangeDuration && !dDialog.RadioEffectPlacementOverlap &&
						    !dDialog.RadioPlacementSpacedDuration)
							thisStartTime = startTime.TotalSeconds + (offset*placeCount);
						//Determine End time
						if (dDialog.RadioEqualDuration)
							thisEndTime = thisStartTime + offset;
						if (dDialog.RadioDoNotChangeDuration)
							thisEndTime = thisStartTime + TimelineControl.SelectedElements.ElementAt(i).Duration.TotalSeconds;
						if (dDialog.RadioSpecifiedDuration)
							thisEndTime = thisStartTime + Convert.ToDouble(dDialog.SpecifiedEffectDuration.TotalSeconds);
						elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i),
							new Tuple<TimeSpan, TimeSpan>(TimeSpan.FromSeconds(thisStartTime), TimeSpan.FromSeconds(thisEndTime)));
						placeCount++;
					}
				}
				if (elementsToDistribute.Any())
				{
					TimelineControl.grid.MoveResizeElements(elementsToDistribute, ElementMoveType.Distribute);
				}
			}
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

			if (e.Button == MouseButtons.Left)
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
			else if (e.Button == MouseButtons.Right)
			{
				MarkCollection mc = null;
				if (_sequence.MarkCollections.Count == 0)
				{
					if (MessageBox.Show("Marks are stored in Mark Collections. There are no mark collections available to store this mark. Would you like to create a new one?", "Creat a Mark Collection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						mc = GetOrAddNewMarkCollection(Color.White, "Default Marks");
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
					PopulateMarkSnapTimes();
					sequenceModified();
				}
			}
		}

		private MarkCollection GetOrAddNewMarkCollection(Color color, string name = "New Collection")
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
			PopulateMarkSnapTimes();
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
			PopulateMarkSnapTimes();
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

		private void OpenSequenceContext(ISequence sequence)
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
				Logging.Error(@"Null context when attempting to play sequence.");
				MessageBox.Show(@"Unable to play this sequence.  See error log for details.");
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
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(updateButtonStates));
			else
			{
				if (_context == null)
				{
					toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
					toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
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
					EffectsForm.Enabled = false;
				}
				else // Stopped
				{
					toolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
					toolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					toolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
					EffectsForm.Enabled = true;
				}
			}
		}

		#endregion

		#region Sequence / TimelineControl relationship management

		private List<Element> CloneElements(IEnumerable<Element> elements)
		{
			var newElements = new List<Element>();
			foreach (var element in elements)
			{
				var newEffect = ApplicationServices.Get<IEffectModuleInstance>(element.EffectNode.Effect.TypeId);
				newEffect.ModuleData = element.EffectNode.Effect.ModuleData.Clone();
				
				try
				{
					// get the target element
					var targetNode = (ElementNode)element.Row.Tag;

					// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
					newEffect.TargetNodes = new[] { targetNode };
					newEffect.TimeSpan = element.Duration;
					var effectNode = new EffectNode(newEffect, element.StartTime);

					// put it in the sequence and in the timeline display
					newElements.Add(AddEffectNode(effectNode));
					

				} catch (Exception ex)
				{
					string msg = "TimedSequenceEditor CloneElements: error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
								 ((element.Row == null) ? "<null>" : element.Row.Name);
					Logging.Error(msg, ex);
				}
			}

			sequenceModified();

			//Add elements as a group to undo
			var act = new EffectsAddedUndoAction(this, newElements.Select(x => x.EffectNode).ToArray());
			_undoMgr.AddUndoAction(act);

			return newElements;
		}

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

		/// <summary>
		/// Adds multiple EffectNodes to the sequence and the TimelineControl.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns>A List of the TimedSequenceElements created and added to the TimelineControl.</returns>
		public List<TimedSequenceElement> AddEffectNodes(IEnumerable<EffectNode> nodes)
		{
			return nodes.Select(AddEffectNode).ToList();
		}

		/// <summary>
		/// Selects the given effects given in an EffectNode list
		/// </summary>
		///<param name="nodes"></param>
		private void SelectEffectNodes(IEnumerable<EffectNode> nodes)
		{
			TimelineControl.grid.ClearSelectedElements();

			foreach (EffectNode element in nodes)
			{
				TimedSequenceElement tse = (TimedSequenceElement)_effectNodeToElement[element];
				tse.Selected = true;
			}
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
		/// Adds a Undo record for the add as well.
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

				if ((startTime + timeSpan) > SequenceLength)
				{
					timeSpan = SequenceLength - startTime;
				}

				var effectNode = CreateEffectNode(effectInstance, row, startTime, timeSpan);

				// put it in the sequence and in the timeline display
				AddEffectNode(effectNode);
				sequenceModified();

				var act = new EffectsAddedUndoAction(this, new[] { effectNode });
				_undoMgr.AddUndoAction(act);
			}
			catch (Exception ex)
			{
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " +
							 ((row == null) ? "<null>" : row.Name);
				Logging.Error(msg, ex);
			}
		}

		private static EffectNode CreateEffectNode(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime,
			TimeSpan timeSpan)
		{
			// get the target element
			var targetNode = (ElementNode) row.Tag;

			// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
			effectInstance.TargetNodes = new[] {targetNode};
			effectInstance.TimeSpan = timeSpan;
			return new EffectNode(effectInstance, startTime);
	
		}


		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for each of the given EffectNodes in the list.
		/// Uses bulk loading feature of Row
		/// Will add a single TimedSequenceElement to in each row that each targeted element of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="nodes">The EffectNode to make element(s) in the grid for.</param>
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
						const string message = "No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
						Logging.Error(message);
						MessageBox.Show(message);
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
									const string message = "No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
									Logging.Error(message);
									MessageBox.Show(message);
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

		/// <summary>
		/// Checks all elements and if they are dirty they are placed in the render queue
		/// </summary>
		private void CheckAndRenderDirtyElements()
		{
			TimelineControl.Rows.AsParallel().WithCancellation(cancellationTokenSource.Token).ForAll(target =>
			{
				foreach (Element elem in target)
				{
					if (elem.EffectNode.Effect.IsDirty)
					{
						TimelineControl.grid.RenderElement(elem);
					}
				}
			});
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

		private void timelineControl_DataDropped(object sender, TimelineDropEventArgs e)
		{
			Guid effectGuid = (Guid)e.Data.GetData(DataFormats.Serializable);
			TimeSpan duration = TimeSpan.FromSeconds(2.0); // TODO: need a default value here. I suggest a per-effect default.
			TimeSpan startTime = Util.Min(e.Time, (_sequence.Length - duration)); // Ensure the element is inside the grid.
			addNewEffectById(effectGuid, e.Row, startTime, duration);
		}


		private void timelineControl_ColorDropped(object sender, ToolDropEventArgs e)
		{
			List<Element> elementList = new List<Element>();

			if (e.Element.Selected)
				elementList = TimelineControl.SelectedElements.ToList();
			else
				elementList.Add(e.Element);

			Color color = (Color)e.Data.GetData(typeof(Color));
			foreach (Element elem in elementList)
			{
				object[] parms = elem.EffectNode.Effect.ParameterValues;
				switch (elem.EffectNode.Effect.EffectName)
				{
					case "Alternating":
						if (e.MouseButton == MouseButtons.Right || Control.ModifierKeys.HasFlag(Keys.Control))
						{
							parms[3] = color;
							parms[9] = true;
						}
						else
						{
							parms[1] = color;
							parms[8] = true;
						}
						break;
					case "Set Level":
						parms[1] = color;
						break;
					case "Pulse":
						parms[1] = new ColorGradient(color);
						break;
					case "Chase":
						parms[0] = 0; // StaticColor
						parms[3] = color;
						break;
					case "Spin":
						parms[2] = 0; // StaticColor
						parms[9] = color;
						break;
					case "Twinkle":
						parms[7] = 0; // StaticColor
						parms[8] = color;
						break;
					case "Wipe":
						parms[0] = new ColorGradient(color);
						break;
				}
				//TODO:
				//this would be a good place to build a list of target elements, and the new parameters
				//list could be passed to a ModifyElements method to do the work in "one step"
				elem.EffectNode.Effect.ParameterValues = parms;
				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
		}

		private void timelineControl_CurveDropped(object sender, ToolDropEventArgs e)
		{
			List<Element> elementList = new List<Element>();

			if (e.Element.Selected)
				elementList = TimelineControl.SelectedElements.ToList();
			else
				elementList.Add(e.Element);

			Curve droppedCurve = _curveLibrary.GetCurve(e.Data.GetData(DataFormats.StringFormat).ToString());
			foreach (Element elem in elementList)
			{
				Curve curve = new Curve(droppedCurve);

				if (ToolsForm.LinkCurves)
				{
					curve.LibraryReferenceName = e.Data.GetData(DataFormats.StringFormat).ToString();
				}
				else
				{
					curve.LibraryReferenceName = string.Empty;
					curve.UnlinkFromLibraryCurve();
				}

				curve.IsCurrentLibraryCurve = false;

				object[] parms = elem.EffectNode.Effect.ParameterValues;
				switch (elem.EffectNode.Effect.EffectName)
				{
					case "Alternating":
						if (e.MouseButton == MouseButtons.Right || Control.ModifierKeys.HasFlag(Keys.Control))
						{
							parms[13] = curve;
						}
						else
						{
							parms[12] = curve;
						}
						break;
					case "Pulse":
						parms[0] = curve;
						break;
					case "Chase":
						parms[5] = curve;
						break;
					case "Spin":
						parms[11] = curve;
						break;
					case "Wipe":
						parms[2] = curve;
						break;
				}
				elem.EffectNode.Effect.ParameterValues = parms;
				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
		}

		private void timelineControl_GradientDropped(object sender, ToolDropEventArgs e)
		{
			List<Element> elementList = new List<Element>();

			if (e.Element.Selected)
				elementList = TimelineControl.SelectedElements.ToList();
			else
				elementList.Add(e.Element);

			ColorGradient droppedGradient = _colorGradientLibrary.GetColorGradient(e.Data.GetData(DataFormats.StringFormat).ToString());
			foreach (Element elem in elementList)
			{
				ColorGradient gradient = new ColorGradient(droppedGradient);

				if (ToolsForm.LinkGradients)
				{
					gradient.LibraryReferenceName = e.Data.GetData(DataFormats.StringFormat).ToString();
				}
				else
				{
					gradient.LibraryReferenceName = string.Empty;
					gradient.UnlinkFromLibrary();
				}

				gradient.IsCurrentLibraryGradient = false;

				object[] parms = elem.EffectNode.Effect.ParameterValues;
				switch (elem.EffectNode.Effect.EffectName)
				{
					case "Alternating":
						if (e.MouseButton == MouseButtons.Right || Control.ModifierKeys.HasFlag(Keys.Control))
						{
							parms[9] = false;
							parms[11] = gradient;
						}
						else
						{
							parms[8] = false;
							parms[10] = gradient;
						}
						break;
					case "Pulse":
						parms[1] = gradient;
						break;
					case "Chase":
						parms[0] = ToolsForm.GradientHandling;
						parms[4] = gradient;
						break;
					case "Spin":
						parms[2] = ToolsForm.GradientHandling;
						parms[10] = gradient;
						break;
					case "Twinkle":
						parms[7] = ToolsForm.GradientHandling;
						parms[9] = gradient;
						break;
					case "Wipe":
						parms[0] = gradient;
						break;
				}
				elem.EffectNode.Effect.ParameterValues = parms;
				TimelineControl.grid.RenderElement(elem);
			}
			sequenceModified();
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
				case Keys.Up:
					EffectsForm.MoveNodeSelection("up");
					break;
				case Keys.Down:
					EffectsForm.MoveNodeSelection("down");
					break;
				//case Keys.Escape:
					//EffectsForm.DeselectAllNodes();
					//toolStripButton_DrawMode.Checked = false;
					//toolStripButton_SelectionMode.Checked = true;
					//break;
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

				case Keys.Up:
					EffectsForm.MoveNodeSelection("up");
					break;

				case Keys.Down:
					EffectsForm.MoveNodeSelection("down");
					break;

				case Keys.Escape:
					if (TimelineControl.grid._beginEffectDraw) //If we are drawing, prevent escape
						return;
					EffectsForm.DeselectAllNodes();
					TimelineControl.grid.EnableDrawMode = false;
					toolStripButton_DrawMode.Checked = false;
					toolStripButton_SelectionMode.Checked = true;
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
				case Keys.Z:
					if (e.Control)
					{
						if (_undoMgr.NumUndoable > 0)
						{
							_undoMgr.Undo();	
						}
					}
					break;
				case Keys.Y:
					if (e.Control)
					{
						if (_undoMgr.NumRedoable > 0)
						{
							_undoMgr.Redo();
						}
					}
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
			var affectedElements = new List<Element>();
			foreach (Row row in TimelineControl.VisibleRows)
			{
				// Since removals may happen during enumeration, make a copy with ToArray().
				
				affectedElements.AddRange(row.SelectedElements);
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
			if (cutElements)
			{
				var act = new EffectsCutUndoAction(this, affectedElements.Select(x => x.EffectNode));
				_undoMgr.AddUndoAction(act);	
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

		/// <summary>
		/// Pastes the clipboard data starting at the given time. If pasting to a SelectedRow, the time passed should be TimeSpan.Zero
		/// </summary>
		/// <param name="pasteTime"></param>
		/// <returns></returns>
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
			TimeSpan offset = pasteTime == TimeSpan.Zero ? TimeSpan.Zero : data.EarliestStartTime;
			Row targetRow = TimelineControl.SelectedRow ?? TimelineControl.ActiveRow ?? TimelineControl.TopVisibleRow;
			List<Row> visibleRows = new List<Row>(TimelineControl.VisibleRows);
			int topTargetRoxIndex = visibleRows.IndexOf(targetRow);
			List<EffectNode> nodesToAdd = new List<EffectNode>();
			foreach (KeyValuePair<TimelineElementsClipboardData.EffectModelCandidate, int> kvp in data.EffectModelCandidates)
			{
				TimelineElementsClipboardData.EffectModelCandidate effectModelCandidate = kvp.Key;
				int relativeRow = kvp.Value;

				int targetRowIndex = topTargetRoxIndex + relativeRow;
				TimeSpan targetTime = effectModelCandidate.StartTime - offset + pasteTime;
				if (targetTime > TimelineControl.grid.TotalTime)
				{
					continue;
				}
				if (targetTime + effectModelCandidate.Duration > TimelineControl.grid.TotalTime)
				{
					//Shorten to fit.
					effectModelCandidate.Duration = TimelineControl.grid.TotalTime - targetTime;
				}
				if (targetRowIndex >= visibleRows.Count)
					continue;

				//Make a new effect and populate it with the detail data from the clipboard
				var newEffect = ApplicationServices.Get<IEffectModuleInstance>(effectModelCandidate.TypeId);
				newEffect.ModuleData = effectModelCandidate.GetEffectData();
				
				nodesToAdd.Add(CreateEffectNode(newEffect, visibleRows[targetRowIndex], targetTime, effectModelCandidate.Duration));
				result++;
			}

			// put it in the sequence and in the timeline display
			List<TimedSequenceElement> elements = AddEffectNodes(nodesToAdd);
			sequenceModified();

			var act = new EffectsPastedUndoAction(this, elements.Select(x => x.EffectNode));
			_undoMgr.AddUndoAction(act);

			return result;
		}

		#endregion

		#region Menu Bar

		#region Sequence Menu

		private void toolStripMenuItem_Save_Click(object sender, EventArgs e)
		{
			saveSequence();
		}

		private void toolStripMenuItem_AutoSave_Click(object sender, EventArgs e)
		{
			SetAutoSave();
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
			Row targetRow = TimelineControl.SelectedRow ?? TimelineControl.ActiveRow ?? TimelineControl.TopVisibleRow;
			ClipboardPaste(targetRow.Selected ? TimeSpan.Zero : TimelineControl.CursorPosition);
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
				if (module is Audio)
				{
					modulesToRemove.Add(module);
				}
			}

			if (modulesToRemove.Count > 0)
			{
				DialogResult result =
					MessageBox.Show(@"Are you sure you want to remove the audio association?", @"Remove existing audio?", MessageBoxButtons.YesNoCancel);
				if (result != DialogResult.Yes)
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
				if (module is Audio)
				{
					modulesToRemove.Add(module);
				}
			}

			if (modulesToRemove.Count > 0)
			{
				DialogResult result =
					MessageBox.Show(@"Only one audio file can be associated with a sequence at a time. If you choose another, " +
									@"the first will be removed. Continue?", @"Remove existing audio?", MessageBoxButtons.YesNoCancel);
				if (result != DialogResult.Yes)
					return;
			}

			// TODO: we need to be able to get the support file types, to filter the openFileDialog properly, but it's not
			// immediately obvious how to get that; for now, just let it open any file type and complain if it's wrong

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				IMediaModuleInstance newInstance = _sequence.AddMedia(openFileDialog.FileName);
				if (newInstance == null)
				{
					Logging.Warn(string.Format("Unsupported audio file {0}", openFileDialog.FileName));
					MessageBox.Show(@"The selected file is not a supported type.");
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
				if (newInstance is Audio)
				{
					length = (newInstance as Audio).MediaDuration;
					TimelineControl.Audio = newInstance as Audio;
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
						if (MessageBox.Show(@"Do you want to resize the sequence to the size of the audio?",
											@"Resize sequence?", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
			TextDialog prompt = new TextDialog("Enter new sequence length:", "Sequence Length",
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
					sequenceModified();
					break;
				}
				else
				{
					MessageBox.Show(@"Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'",
									@"Error parsing time");
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
				//We have to re-subscribe to the event handlers
				EffectsForm.EscapeDrawMode += EscapeDrawMode;
			}
			else
			{
				MarksForm.Close();
			}
		}

		private void toolWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ToolsForm.DockState == DockState.Unknown)
			{
				DockState dockState = ToolsForm.DockState;
				dockState = DockState.DockLeft;
				if (dockState == DockState.Unknown) dockState = DockState.DockLeft;
				ToolsForm.Show(dockPanel, dockState);
				//We have to re-subscribe to the event handlers
				ToolsForm.StartColorDrag += ToolPalette_ColorDrag;
				ToolsForm.StartCurveDrag += ToolPalette_CurveDrag;
				ToolsForm.StartGradientDrag += ToolPalette_GradientDrag;
			}
			else
			{
				ToolsForm.Close();
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
					MessageBox.Show(@"Please select an element to save.");
					break;
				case 1:
					TimedSequenceElement tse = (TimedSequenceElement)selectedElements.ElementAt(0);
					saveVirtualEffect(tse.EffectNode.Effect);
					break;
				default:
					MessageBox.Show(@"Please select only 1 element to save.");
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

		private void toolStripButtonSnapToStrength_MenuItem_Click(object sender, EventArgs e)
		{

			ToolStripMenuItem item = sender as ToolStripMenuItem;
			if (!item.Checked)
			{
				foreach (ToolStripMenuItem subItem in item.Owner.Items)
				{
					if (!item.Equals(subItem) && subItem != null)
					{
						subItem.Checked = false;
					}
				}
				item.Checked = true;
				TimelineControl.ruler.SnapStrength = TimelineControl.grid.SnapStrength = Convert.ToInt32(item.Tag);
				PopulateMarkSnapTimes();
				
			} 
			
			// clicking the currently checked one--do not uncheck it
			
		}

		#endregion

		#region Undo

		private void InitUndo()
		{
			_undoMgr = new UndoManager();
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

		private void undoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
		{
			_undoMgr.Undo(e.NumItems);
		}

		private void redoButton_ButtonClick(object sender, EventArgs e)
		{
			_undoMgr.Redo();
		}

		private void redoButton_ItemChosen(object sender, UndoMultipleItemsEventArgs e)
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

		public void SwapPlaces(Dictionary<Element, ElementTimeInfo> changedElements)
		{
			TimelineControl.grid.SwapElementPlacement(changedElements);
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

		public ISequence Sequence
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
			MarksForm.Close();
			EffectsForm.Close();

			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockLeftPortion", Name), (int)dockPanel.DockLeftPortion);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockRightPortion", Name), (int)dockPanel.DockLeftPortion);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AutoSaveEnabled", Name), autoSaveToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DrawModeSelected", Name), toolStripButton_DrawMode.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SelectionModeSelected", Name), toolStripButton_SelectionMode.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapToSelected", Name), toolStripButton_SnapTo.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name), Size.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name), Size.Width);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), WindowState.ToString());
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapStrength", Name), TimelineControl.grid.SnapStrength);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorEnabled", Name), TimelineControl.grid.ResizeIndicator_Enabled);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CadStyleSelectionBox", Name), cADStyleSelectionBoxToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorColor", Name), TimelineControl.grid.ResizeIndicator_Color);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolPaletteLinkCurves", Name), ToolsForm.LinkCurves);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ToolPaletteLinkGradients", Name), ToolsForm.LinkGradients);

			ToolsForm.Close();

			//These are only saved in options
			//xml.PutPreference(string.Format("{0}/AutoSaveInterval", Name), _autoSaveTimer.Interval);

			//Clean up any old locations from before we organized the settings.
			xml.RemoveNode("StandardNudge");
			xml.RemoveNode("SuperNudge");
			xml.RemoveNode(Name);

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
				PopulateMarkSnapTimes();
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
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(_SetTimingToolStripEnabledState));
			else
			{
				ITiming timingSource = _sequence.GetTiming();
				toolStripButton_IncreaseTimingSpeed.Enabled =
					toolStripButton_DecreaseTimingSpeed.Enabled =
					toolStripLabel_TimingSpeed.Enabled = toolStripLabel_TimingSpeedLabel.Enabled =
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
				if (toolStripButton_Loop.Checked)
				{
					_context.PlayLoop(rangeStart, rangeEnd);
				}
				else
				{
					_context.Play(rangeStart, rangeEnd);	
				}
				
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
					toolStripButton_IncreaseTimingSpeed.Enabled =
							toolStripLabel_TimingSpeed.Enabled = toolStripLabel_TimingSpeedLabel.Enabled = true;
					toolStripButton_DecreaseTimingSpeed.Enabled = toolStripButton_DecreaseTimingSpeed.Enabled = _timingSpeed > _timingChangeDelta; ;

				}
				else
				{
					_UpdateTimingSpeedDisplay();
					toolStripButton_IncreaseTimingSpeed.Enabled =
				   toolStripButton_DecreaseTimingSpeed.Enabled =
				   toolStripLabel_TimingSpeed.Enabled = toolStripLabel_TimingSpeedLabel.Enabled = false;
				}
			}
		}

		private void toolStripExVirtualEffects_Clear()
		{
			if (_virtualEffectLibrary == null)
				return;

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

		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private readonly Task loadingTask = null;

		private void TimedSequenceEditorForm_Shown(object sender, EventArgs e)
		{
			Enabled = false;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			//loadingTask = Task.Factory.StartNew(() => loadSequence(_sequence), token);
			loadSequence(_sequence);
		}

		private void cboAudioDevices_TextChanged(object sender, EventArgs e)
		{
			Variables.SelectedAudioDeviceIndex = cboAudioDevices.SelectedIndex;
		}

		private void cboAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			Variables.SelectedAudioDeviceIndex = cboAudioDevices.SelectedIndex;
		}

		private void menuStrip_MenuActivate(object sender, EventArgs e)
		{
			effectWindowToolStripMenuItem.Checked = (EffectsForm.DockState != DockState.Unknown);
			markWindowToolStripMenuItem.Checked = (MarksForm.DockState != DockState.Unknown);
			toolWindowToolStripMenuItem.Checked = (ToolsForm.DockState != DockState.Unknown);
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

		private void curveEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selector = new CurveLibrarySelector{DoubleClickMode = CurveLibrarySelector.Mode.Edit};
			selector.ShowDialog();
		}

		private void colorGradientToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selector = new ColorGradientLibrarySelector{DoubleClickMode = ColorGradientLibrarySelector.Mode.Edit};
			selector.ShowDialog();
		}

        private void editMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LipSyncMapSelector mapSelector = new LipSyncMapSelector();
            DialogResult dr = mapSelector.ShowDialog();
            if (mapSelector.Changed == true)
            {
                mapSelector.Changed = false;
                sequenceModified();
                resetLipSyncNodes();
                VixenSystem.SaveSystemConfig();
	        }
        }

        private void setDefaultMap_Click(object sender,EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            if (!_library.DefaultMappingName.Equals(menu.Text))
            {
                _library.DefaultMappingName = menu.Text; 
                sequenceModified();
            }
            
        }

        private void resetLipSyncNodes()
        {
            foreach (Row row in TimelineControl.Rows)
            {
                for (int j = 0; j < row.ElementCount; j++)
                {
                    Element elem = row.ElementAt(j);
					IEffectModuleInstance effect = elem.EffectNode.Effect;
					if (effect.GetType() == typeof(LipSync))
					{
						((LipSync)effect).MakeDirty();
					}

                }
            }

            TimelineControl.grid.RenderAllRows();
        }

        private void defaultMapToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string defaultText = _library.DefaultMappingName;
            this.defaultMapToolStripMenuItem.DropDownItems.Clear();
            
            foreach (LipSyncMapData mapping in _library.Library.Values)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(mapping.LibraryReferenceName);
                menuItem.Click += setDefaultMap_Click;
                menuItem.Checked = _library.IsDefaultMapping(mapping.LibraryReferenceName);
                this.defaultMapToolStripMenuItem.DropDownItems.Add(menuItem);
            }            
        }

        private void papagayoImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName;
            PapagayoDoc papagayoFile = new PapagayoDoc();
            FileDialog openDialog = new OpenFileDialog();

            openDialog.Filter = "Papagayo files (*.pgo)|*.pgo|All files (*.*)|*.*";
            openDialog.FilterIndex = 1;
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fileName = openDialog.FileName;
            papagayoFile.Load(fileName);

            TimelineElementsClipboardData result = new TimelineElementsClipboardData()
            {
                FirstVisibleRow = -1,
                EarliestStartTime = TimeSpan.MaxValue,
            };

            result.FirstVisibleRow = 0;

            int rownum = 0;
            foreach (string voice in papagayoFile.VoiceList)
            {
                List<PapagayoPhoneme> phonemes = papagayoFile.PhonemeList(voice);

                if (phonemes.Count > 0)
                {

                    foreach (PapagayoPhoneme phoneme in phonemes)
                    {
                        if (phoneme.DurationMS == 0.0)
                        {
                            continue;
                        }

                        IEffectModuleInstance effect =
                            ApplicationServices.Get<IEffectModuleInstance>(new LipSyncDescriptor().TypeId);

                        ((LipSync)effect).StaticPhoneme = phoneme.TypeName.ToUpper();
                        ((LipSync)effect).LyricData = phoneme.LyricData;

                        TimeSpan startTime = TimeSpan.FromMilliseconds(phoneme.StartMS);
                        TimelineElementsClipboardData.EffectModelCandidate modelCandidate =
                              new TimelineElementsClipboardData.EffectModelCandidate(effect)
                              {
                                  Duration = TimeSpan.FromMilliseconds(phoneme.DurationMS - 1),
                                  StartTime = startTime,
                              };

                        result.EffectModelCandidates.Add(modelCandidate, rownum);
                        if (startTime < result.EarliestStartTime)
                            result.EarliestStartTime = startTime;

                        effect.Render();

                    }
                    
                    IDataObject dataObject = new DataObject(_clipboardFormatName);
                    dataObject.SetData(result);
                    Clipboard.SetDataObject(dataObject, true);
                    _TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);
                    sequenceModified();

                }
                rownum++;
            }
            
            string displayStr = rownum + " Voices imported to clipboard as seperate rows\n\n";
            
            int j = 1;
            foreach (string voiceStr in papagayoFile.VoiceList)
            {
                displayStr += "Row #" + j +" - " + voiceStr + "\n";
                j++;
            }
            
            MessageBox.Show(displayStr, "Papagayo Import", MessageBoxButtons.OK);
        }

        private void textConverterHandler(object sender, NewTranslationEventArgs args)
        {
            TimelineElementsClipboardData result = new TimelineElementsClipboardData()
            {
                FirstVisibleRow = -1,
                EarliestStartTime = TimeSpan.MaxValue,
            };

            if (args.PhonemeData.Count > 0)
            {

                foreach (LipSyncConvertData data in args.PhonemeData)
                {
                    if (data.Duration.Ticks == 0)
                    {
                        continue;
                    }

                    IEffectModuleInstance effect =
                        ApplicationServices.Get<IEffectModuleInstance>(new LipSyncDescriptor().TypeId);

                    ((LipSync)effect).StaticPhoneme = data.Phoneme.ToString().ToUpper();
                    ((LipSync)effect).LyricData = data.LyricData;

                    TimelineElementsClipboardData.EffectModelCandidate modelCandidate =
                          new TimelineElementsClipboardData.EffectModelCandidate(effect)
                          {
                              Duration = data.Duration,
                              StartTime = data.StartOffset
                          };

                    result.EffectModelCandidates.Add(modelCandidate, 0);
                    if (data.StartOffset < result.EarliestStartTime)
                        result.EarliestStartTime = data.StartOffset;

                    effect.Render();

                }

                IDataObject dataObject = new DataObject(_clipboardFormatName);
                dataObject.SetData(result);
                Clipboard.SetDataObject(dataObject, true);
                _TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);

                int pasted = 0;

                if (args.Placement == TranslatePlacement.Cursor)
                {
                    args.FirstMark += TimelineControl.grid.CursorPosition;
                }

                if (args.Placement != TranslatePlacement.Clipboard)
                {
                    pasted = ClipboardPaste((TimeSpan)args.FirstMark);
                }

                if (pasted == 0)
                {
                    MessageBox.Show("Conversion Complete and copied to Clipboard \n Paste at first Mark offset", "Convert Text", MessageBoxButtons.OK);
                }

                sequenceModified();

            }
        }

        private void translateFailureHandler(object sender, TranslateFailureEventArgs args)
        {
            LipSyncTextConvertFailForm failForm = new LipSyncTextConvertFailForm();
            failForm.errorLabel.Text = "Unable to find mapping for "  + args.FailedWord + Environment.NewLine +
                "Please map using buttons below";
            DialogResult dr = failForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                LipSyncTextConvert.AddUserMaping(args.FailedWord + " " + failForm.TranslatedString);
            }
        }

        private void textConverterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LipSyncTextConvertForm.Active == false)
            {
                LipSyncTextConvertForm textConverter = new LipSyncTextConvertForm();
                textConverter.NewTranslation += new EventHandler<NewTranslationEventArgs>(textConverterHandler);
                textConverter.TranslateFailure += new EventHandler<TranslateFailureEventArgs>(translateFailureHandler);
                textConverter.MarkCollections = _sequence.MarkCollections;
                textConverter.Show(this);
            }
        }

        private void lipSyncMappingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.changeMapToolStripMenuItem.Enabled =
             (_library.Library.Count > 1) &&
             (TimelineControl.SelectedElements.Any(effect => effect.EffectNode.Effect.GetType() == typeof(LipSync)));
        }


        private void changeMapToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string defaultText = _library.DefaultMappingName;
            this.changeMapToolStripMenuItem.DropDownItems.Clear();

            foreach (LipSyncMapData mapping in _library.Library.Values)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(mapping.LibraryReferenceName);
                menuItem.Click += changeMappings_Click;
                this.changeMapToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void changeMappings_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripSender = (ToolStripMenuItem)sender;

            TimelineControl.SelectedElements.ToList().ForEach(delegate(Element element)
            {
                if (element.EffectNode.Effect.GetType() == typeof(LipSync))
                {
                    ((LipSync)element.EffectNode.Effect).PhonemeMapping =  toolStripSender.Text;
                    resetLipSyncNodes();
                }
            });

        }

		private void helpDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.vixenlights.com/vixen-3-documentation/sequencer/");
		}
		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
            ExportDialog ed = new ExportDialog(Sequence);
            ed.ShowDialog();
		}

		private void bulkEffectMoveToolStripMenuItem_Click(object sender, EventArgs e)
		{

			var dialog = new BulkEffectMoveForm(TimelineControl.grid.CursorPosition);
			using (dialog)
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					TimelineControl.grid.MoveElementsInRangeByTime(dialog.Start, dialog.End, dialog.IsForward?dialog.Offset:-dialog.Offset);
				}
			}
		}

		private void cADStyleSelectionBoxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.aCadStyleSelectionBox = cADStyleSelectionBoxToolStripMenuItem.Checked;
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