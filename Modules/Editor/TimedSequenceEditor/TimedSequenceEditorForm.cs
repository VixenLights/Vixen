using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using Vixen;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Marks;
using Vixen.Module.App;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Picture;
using VixenModules.Effect.Shapes;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using Vixen.Sys.State;
using VixenModules.App.ColorGradients;
using VixenModules.App.Marks;
using VixenModules.Editor.EffectEditor;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Windows.Forms.Timer;
using VixenModules.Property.Color;
using DockPanel = WeifenLuo.WinFormsUI.Docking.DockPanel;
using ListView = System.Windows.Forms.ListView;
using ListViewItem = System.Windows.Forms.ListViewItem;
using MarkCollection = VixenModules.App.Marks.MarkCollection;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
using Size = System.Drawing.Size;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm : BaseForm, IEditorUserInterface, ITiming
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
		private int _delayCountDown;

		private readonly Timer _autoSaveTimer = new Timer();

		// Variables used by the add multiple effects dialog
		private int _amLastEffectCount;
		private TimeSpan _amLastStartTime;
		private TimeSpan _amLastDuration;
		private TimeSpan _amLastDurationBetween;

		// a mapping of effects in the sequence to the element that represent them in the grid.
		private Dictionary<EffectNode, Element> _effectNodeToElement;

		// a mapping of system elements to the (possibly multiple) rows that represent them in the grid.
		private Dictionary<ElementNode, List<Row>> _elementNodeToRows;

		// the default time for a sequence if one is loaded with 0 time
		private static readonly TimeSpan DefaultSequenceTime = TimeSpan.FromMinutes(1);

		// Undo manager
		private UndoManager _undoMgr;

		private TimeSpan? _mPrevPlaybackStart;
		private TimeSpan? _mPrevPlaybackEnd;

		private bool _mModified;
		private bool _editorStateModified;

		private float _timingSpeed = 1;

		private float _timingChangeDelta = 0.1f;

		private static readonly DataFormats.Format ClipboardFormatName =
			DataFormats.GetFormat(typeof (TimelineElementsClipboardData).FullName);


		private readonly ContextMenuStrip _contextMenuStrip = new ContextMenuStrip();

		private string _settingsPath;
		private string _colorCollectionsPath;

		private CurveLibrary _curveLibrary;
		private ColorGradientLibrary _colorGradientLibrary;
		private LipSyncMapLibrary _library;
		private List<ColorCollection> _colorCollections = new List<ColorCollection>();

		//Used for color collections
		private static Random rnd = new Random();
		private PreCachingSequenceEngine _preCachingSequenceEngine;

		//Used for setting a mouse location to do repeat actions on.
		private Point _mouseOriginalPoint = new Point(0,0);

		//Used for the Align Effects to the nearest Mark.
		private string AlignTo_Threshold;

		private readonly double _scaleFactor = 1;
		private bool _suppressModifiedEvents;

		//for external clipboard events.
		IntPtr _clipboardViewerNext;
		
		private readonly TimeLineGlobalEventManager _timeLineGlobalEventManager;
		private readonly TimeLineGlobalStateManager _timeLineGlobalStateManager;

		//List to hold removed nodes so we can clean them up later. Due to how the undo works, nodes are sticky and 
		//live on past removal so they can can be added back
		//TODO Fix that stickyness so this can go away
		private List<EffectNode> _removedNodes = new List<EffectNode>();

		private int _iconSize;
		private int _toolStripImageSize;

		#endregion

		#region Constructor / Initialization

		public TimedSequenceEditorForm()
		{
			InitializeComponent();
			_scaleFactor = ScalingTools.GetScaleFactor();
			menuStrip.Renderer = new ThemeToolStripRenderer();
			
			_contextMenuStrip.Renderer = new ThemeToolStripRenderer();
			contextMenuStripEffect.Renderer = new ThemeToolStripRenderer();
			contextMenuStripLibraries.Renderer = new ThemeToolStripRenderer();
			contextMenuStripAll.Renderer = new ThemeToolStripRenderer();
			int imageSize = (int)(16 * _scaleFactor);
			_contextMenuStrip.ImageScalingSize = new Size(imageSize, imageSize);
			statusStrip.Renderer = new ThemeToolStripRenderer();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			var theme = new VS2015DarkTheme();
			dockPanel.Theme = theme;

			Icon = Resources.Icon_Vixen3;
			_iconSize = (int) (28*_scaleFactor);
			_toolStripImageSize = (int)(16 * _scaleFactor);
			toolStripEffects.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			toolStripColorLibrary.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			toolStripCurveLibrary.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);
			toolStripGradientLibrary.ImageScalingSize = new Size(_toolStripImageSize, _toolStripImageSize);

			foreach (ToolStripItem toolStripItem in modeToolStripDropDownButton_SnapToStrength.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += toolStripButtonSnapToStrength_MenuItem_Click;
				}
			}

			foreach (ToolStripItem toolStripItem in alignmentToolStripDropDownButton_CloseGaps.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += toolStripButtonCloseGapStrength_MenuItem_Click;
				}
			}

			foreach (ToolStripItem toolStripItem in alignmentToolStripDropDownButton_AlignTo.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					toolStripMenuItem.Click += toolStripDropDownButtonAlignToStrength_MenuItem_Click;
				}
			}

			effectGroupsToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			basicToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			pixelToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			deviceToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			add_RemoveContextToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			add_RemoveLibraryToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			toolbarsToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;
			toolbarsToolStripMenuItem_Effect.DropDown.Closing += toolStripMenuItem_Closing;
			toolbarToolStripMenuItem.DropDown.Closing += toolStripMenuItem_Closing;

			PerformAutoScale();
			Execution.ExecutionStateChanged += OnExecutionStateChanged;
			_autoSaveTimer.Tick += AutoSaveEventProcessor;

			//So we can be aware of mark changes.
			_timeLineGlobalEventManager = TimeLineGlobalEventManager.Manager;
			_timeLineGlobalStateManager = TimeLineGlobalStateManager.Manager;
		}

		private IDockContent DockingPanels_GetContentFromPersistString(string persistString)
		{
			if (persistString == typeof (Form_Effects).ToString())
				return EffectsForm;
			if (persistString == typeof (Form_Grid).ToString())
				return GridForm;
			if (persistString == typeof (Form_Marks).ToString())
				return MarksForm;
			if (persistString == typeof(Form_ColorLibrary).ToString())
				return ColorLibraryForm;
			if (persistString == typeof(Form_CurveLibrary).ToString())
				return CurveLibraryForm;
			if (persistString == typeof(Form_GradientLibrary).ToString())
				return GradientLibraryForm;
			if (persistString == typeof (FindEffectForm).ToString())
				return FindEffects;
			if (persistString == typeof (FormEffectEditor).ToString())
				return EffectEditorForm;
			if (persistString == typeof (LayerEditor).ToString())
				return LayerEditor;
			if (persistString == "VixenModules.Editor.TimedSequenceEditor.Form_ToolPalette")
				return null;

			//Else
			throw new NotImplementedException("Unable to find docking window type: " + persistString);
		}

		private void TimedSequenceEditorForm_Load(object sender, EventArgs e)
		{
			RegisterClipboardViewer();
			_settingsPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen",
					"TimedSequenceEditorForm.xml");
			_colorCollectionsPath =
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Vixen",
					"ColorCollections.xml");

			if (File.Exists(_settingsPath))
			{
				try
				{
					//Try to load the dock settings fro ma file. Somehow users manage to corrupt this file, so if it can be used
					//Then just reconfigure to the defaults.
					dockPanel.LoadFromXml(_settingsPath, DockingPanels_GetContentFromPersistString);
				}
				catch (Exception ex)
				{
					DestroyAndRecreateDockPanel(ex);
				}
			}
			else
			{
				SetDockDefaults();
			}

			if (GridForm.IsHidden)
			{
				GridForm.DockState = DockState.Document;
			}

			//Check to see if the timeline is undocked and in some small size that might be lost or hard to find. 
			if (GridForm.Width < 200 && GridForm.DockState == DockState.Float)
			{
				//Set a reasonable float size and dock us back in the main form where we belong.
				//This will also help ensure we can set the splitter location later
				GridForm.Pane.FloatWindow.Size = new Size(300, 300);
				GridForm.DockState = DockState.Document;
			}

			if (EffectEditorForm.DockState == DockState.Unknown)
			{
				EffectEditorForm.Show(dockPanel, DockState.DockRight);
			}
				
			if (LayerEditor.DockState == DockState.Unknown)
			{
				LayerEditor.Show(dockPanel, DockState.DockRight);
			}

			XMLProfileSettings xml = new XMLProfileSettings();

			//Get preferences
			_autoSaveTimer.Interval = xml.GetSetting(XMLProfileSettings.SettingType.Preferences, string.Format("{0}/AutoSaveInterval", Name), 300000);

			//Restore App Settings
			dockPanel.DockLeftPortion = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockLeftPortion", Name), 150);
			dockPanel.DockRightPortion = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockRightPortion", Name), 150);
			fileToolStripButton_AutoSave.Checked = autoSaveToolStripMenuItem.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AutoSaveEnabled", Name), true);
			modeToolStripButton_SnapTo.Checked = toolStripMenuItem_SnapTo.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapToSelected", Name), true);
			PopulateSnapStrength(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapStrength", Name), 2));
			TimelineControl.grid.CloseGap_Threshold = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CloseGapThreshold", Name), ".100");
			AlignTo_Threshold = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlignToThreshold", Name), ".800");
			toolStripMenuItem_ResizeIndicator.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorEnabled", Name), false);
			modeToolStripButton_DrawMode.Checked = TimelineControl.grid.EnableDrawMode = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DrawModeSelected", Name), false);
			modeToolStripButton_SelectionMode.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SelectionModeSelected", Name), true);
			CurveLibraryForm.LinkCurves = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLinkCurves", Name), false);
			GradientLibraryForm.LinkGradients = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLinkGradients", Name), false);
			cADStyleSelectionBoxToolStripMenuItem.Checked = TimelineControl.grid.aCadStyleSelectionBox = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CadStyleSelectionBox", Name), false);
			CheckRiColorMenuItem(xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorColor", Name), "Red"));
			zoomUnderMousePositionToolStripMenuItem.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ZoomUnderMousePosition", Name), false);
			TimelineControl.waveform.Height = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WaveFormHeight", Name), 50);
			TimelineControl.ruler.Height = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/RulerHeight", Name), 50);
			TimelineControl.AddMarks(_sequence.LabeledMarkCollections);
			
			_curveLibrary = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;
			if (_curveLibrary != null)
			{
				_curveLibrary.CurvesChanged += CurveLibrary_CurvesChanged;
			}

			_colorGradientLibrary =
				ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;
			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientsChanged += ColorGradientsLibrary_CurveChanged;
			}

			// Setup Toolbars and Toolstrip context menus.
			InitializeToolBars();

			foreach (ToolStripItem toolStripItem in modeToolStripDropDownButton_SnapToStrength.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					if (TimelineControl.grid.SnapStrength.Equals(Convert.ToInt32(toolStripMenuItem.Tag)))
					{
						toolStripMenuItem.PerformClick();
						break;
					}
				}
			}

			foreach (ToolStripItem toolStripItem in alignmentToolStripDropDownButton_CloseGaps.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					if (TimelineControl.grid.CloseGap_Threshold.Equals(toolStripMenuItem.Tag))
					{
						toolStripMenuItem.PerformClick();
						break;
					}
				}
			}

			foreach (ToolStripItem toolStripItem in alignmentToolStripDropDownButton_AlignTo.DropDownItems)
			{
				var toolStripMenuItem = toolStripItem as ToolStripMenuItem;
				if (toolStripMenuItem != null)
				{
					if (AlignTo_Threshold.Equals(toolStripMenuItem.ToString()))
					{
						toolStripMenuItem.PerformClick();
						break;
					}
				}
			}

			WindowState = FormWindowState.Normal;

			var width = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name),
				Size.Width);
			var height = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name),
				Size.Height);

			//ensure our sizes are at least a minimum 640x480
			if (width < 640)
			{
				width = 640;
			}
			if (height < 480)
			{
				height = 480;
			}

			var desktopBounds =
				new Rectangle(
					new Point(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y)),
					new Size(
						width,
						height));



			var windowState =
					xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), "Normal");
			
			if (windowState.Equals("Maximized") && IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopLocation = desktopBounds.Location;
				WindowState = FormWindowState.Maximized;
			}
			else if (AreCornersVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopBounds = desktopBounds;
			}
			else
			{
				// this resets the upper left corner of the window to windows standards
				StartPosition = FormStartPosition.WindowsDefaultLocation;

				if (windowState.Equals("Minimized"))
				{
					//Somehow we were closed in a minimized state. All bets are off, so put use back in some sensible default.
					WindowState = FormWindowState.Normal;
					// this resets the upper left corner of the window to windows standards
					StartPosition = FormStartPosition.WindowsDefaultLocation;
					Size = new Size(800, 600);
				}
				else
				{
					// we can still apply the saved size
					Size = new Size(desktopBounds.Width, desktopBounds.Height);
				}
			}

			_effectNodeToElement = new Dictionary<EffectNode, Element>();
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			TimelineControl.grid.RenderProgressChanged += OnRenderProgressChanged;

			TimelineControl.ElementChangedRows += ElementChangedRowsHandler;
			TimelineControl.ElementsMovedNew += timelineControl_ElementsMovedNew;
			TimelineControl.ElementDoubleClicked += ElementDoubleClickedHandler;
			//TimelineControl.DataDropped += timelineControl_DataDropped;
			
			TimelineControl.PlaybackCurrentTimeChanged += timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked += timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange += timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged += timelineControl_TimeRangeDragged;

			_timeLineGlobalEventManager.MarksMoving += TimeLineGlobalMoving;
			_timeLineGlobalEventManager.MarksMoved += TimeLineGlobalMoved;
			_timeLineGlobalEventManager.DeleteMark += TimeLineGlobalDeleted;
			_timeLineGlobalEventManager.MarksPasted += TimeLineGlobalEventManagerOnMarksPasted;
			_timeLineGlobalEventManager.MarksTextChanged += TimeLineGlobalTextChanged;
			_timeLineGlobalEventManager.PhonemeBreakdownAction += PhonemeBreakdownAction;
			_timeLineGlobalEventManager.PlayRangeAction += TimeLineGlobalEventManagerOnPlayRangeAction;

			TimelineControl.SelectionChanged += TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown += TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged += TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			_timeLineGlobalEventManager.CursorMoved += CursorMovedHandler;
			TimelineControl.ElementsSelected += timelineControl_ElementsSelected;
			TimelineControl.ContextSelected += timelineControl_ContextSelected;
			TimelineControl.SequenceLoading = false;
			TimelineControl.TimePerPixelChanged += TimelineControl_TimePerPixelChanged;
			TimelineControl.VisibleTimeStartChanged += TimelineControl_VisibleTimeStartChanged;
			TimelineControl.grid.SelectedElementsCloneDelegate = CloneElements;
			TimelineControl.grid.StartDrawMode += DrawElement;
			TimelineControl.grid.DragOver += TimelineControlGrid_DragOver;
			TimelineControl.grid.DragEnter += TimelineControlGrid_DragEnter;
			TimelineControl.grid.DragDrop += TimelineControlGrid_DragDrop;
			Row.RowHeightChanged += TimeLineControl_Row_RowHeightChanged;

			LoadAvailableEffects();
			PopulateDragBoxFilterDropDown();
			InitUndo();
			UpdateButtonStates();
			UpdatePasteMenuStates();
			LoadColorCollections();
			
			toolBarsToolStripMenuItemLibraries.DropDown.Closing += toolStripMenuItem_Closing;

			_library = ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;
			Cursor.Current = Cursors.Default;

			var splitterDistance = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SplitterDistance", Name), (int)(TimelineControl.DefaultSplitterDistance * _scaleFactor));

			if (splitterDistance > 0 && TimelineControl.splitContainer.Width > splitterDistance)
			{
				TimelineControl.splitContainer.SplitterDistance = splitterDistance;
			}
			else
			{
				var distance = (int) (TimelineControl.DefaultSplitterDistance * _scaleFactor);
				if (TimelineControl.splitContainer.Width > distance)
				{
					TimelineControl.splitContainer.SplitterDistance = distance;
				}
			}

			if (_sequence.DefaultPlaybackEndTime != TimeSpan.Zero)
			{
				_mPrevPlaybackStart = TimelineControl.PlaybackStartTime = _sequence.DefaultPlaybackStartTime;
				_mPrevPlaybackEnd = TimelineControl.PlaybackEndTime = _sequence.DefaultPlaybackEndTime;
			}

			// Adjusts Toolbars layout as per saved settings.
			SetToolBarLayout();

#if DEBUG
//ToolStripButton b = new ToolStripButton("[Debug Break]");
//b.Click += b_Click;
//toolStripOperations.Items.Add(b);
#endif
		}

		private void DestroyAndRecreateDockPanel(Exception ex)
		{
			toolStripContainer.ContentPanel.Controls.Remove(dockPanel);
			dockPanel = new DockPanel();
			dockPanel.BackColor = ThemeColorTable.BackgroundColor;
			dockPanel.Dock = DockStyle.Fill;
			dockPanel.DockBackColor = ThemeColorTable.BackgroundColor;
			dockPanel.DockLeftPortion = 200D;
			dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
			dockPanel.Location = new Point(0, 0);
			dockPanel.Margin = new Padding(4);
			dockPanel.Name = "dockPanel";
			dockPanel.Size = new Size(1579, 630);
			toolStripContainer.ContentPanel.Controls.Add(dockPanel);
			Logging.Error(ex, "Error loading dock panel config. Restoring to the default.");

			var theme = new VS2015DarkTheme();
			dockPanel.Theme = theme;

			if (_gridForm != null)
			{
				_gridForm.Dispose();
				_gridForm = null;
			}
			if (_colorLibraryForm != null)
			{
				_colorLibraryForm.Dispose();
				_colorLibraryForm = null;
			}
			if (_curveLibraryForm != null)
			{
				_curveLibraryForm.Dispose();
				_curveLibraryForm = null;
			}
			if (_gradientLibraryForm != null)
			{
				_gradientLibraryForm.Dispose();
				_gradientLibraryForm = null;
			}
			if (_marksForm != null)
			{
				_marksForm.Dispose();
				_marksForm = null;
			}
			if (_layerEditor != null)
			{
				_layerEditor.Dispose();
				_layerEditor = null;
			}
			if (_effectEditorForm != null)
			{
				_effectEditorForm.Dispose();
				_effectEditorForm = null;
			}
			if (_effectsForm != null)
			{
				_effectsForm.Dispose();
				_effectsForm = null;
			}
			if (_findEffects != null)
			{
				_findEffects.Dispose();
				_findEffects = null;
			}
			SetDockDefaults();
		}

		private void SetDockDefaults()
		{
			GridForm.Show(dockPanel, DockState.Document);
			ColorLibraryForm.Show(dockPanel, DockState.DockRight);
			CurveLibraryForm.Show(dockPanel, DockState.DockRight);
			GradientLibraryForm.Show(dockPanel, DockState.DockRight);
			MarksForm.Show(dockPanel, DockState.DockRight);
			LayerEditor.Show(dockPanel, DockState.DockRight);
			FindEffects.Show(dockPanel, DockState.DockRight);
			EffectsForm.Show(dockPanel, DockState.DockLeft);
		}


#if DEBUG
#endif

		private bool AreCornersVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location)) ||
				Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(new Point(rect.Top, rect.Right)));
		}

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
			if (_loadingTask != null && !_loadingTask.IsCompleted && !_loadingTask.IsFaulted && !_loadingTask.IsCanceled)
			{
				_cancellationTokenSource.Cancel();
			}

			foreach (var node in _removedNodes)
			{
				//Dispose any nodes that where removed
				if (!_effectNodeToElement.ContainsKey(node))
				{
					node.Effect.Dispose();
				}
			}

			foreach (var node in _effectNodeToElement.Keys)
			{
				node.Effect.Dispose();
			}

			

			//TimelineControl.grid.RenderProgressChanged -= OnRenderProgressChanged;

			TimelineControl.ElementChangedRows -= ElementChangedRowsHandler;
			TimelineControl.ElementsMovedNew -= timelineControl_ElementsMovedNew;
			TimelineControl.ElementDoubleClicked -= ElementDoubleClickedHandler;
			//TimelineControl.DataDropped -= timelineControl_DataDropped;
			TimelineControl.grid.DragOver -= TimelineControlGrid_DragOver;
			TimelineControl.grid.DragEnter -= TimelineControlGrid_DragEnter;
			TimelineControl.grid.DragDrop -= TimelineControlGrid_DragDrop;

			TimelineControl.PlaybackCurrentTimeChanged -= timelineControl_PlaybackCurrentTimeChanged;

			TimelineControl.RulerClicked -= timelineControl_RulerClicked;
			TimelineControl.RulerBeginDragTimeRange -= timelineControl_RulerBeginDragTimeRange;
			TimelineControl.RulerTimeRangeDragged -= timelineControl_TimeRangeDragged;
			_timeLineGlobalEventManager.MarksMoved -= TimeLineGlobalMoved;
			_timeLineGlobalEventManager.DeleteMark -= TimeLineGlobalDeleted;
			_timeLineGlobalEventManager.MarksPasted -= TimeLineGlobalEventManagerOnMarksPasted;
			_timeLineGlobalEventManager.MarksMoving -= TimeLineGlobalMoving;
			_timeLineGlobalEventManager.MarksTextChanged -= TimeLineGlobalTextChanged;
			_timeLineGlobalEventManager.PhonemeBreakdownAction -= PhonemeBreakdownAction;
			_timeLineGlobalEventManager.PlayRangeAction -= TimeLineGlobalEventManagerOnPlayRangeAction;

			if (_sequence != null && _sequence.LabeledMarkCollections != null)
			{
				_sequence.LabeledMarkCollections.CollectionChanged -= LabeledMarkCollections_CollectionChanged;
				RemoveMarkCollectionHandlers(_sequence.LabeledMarkCollections);
			}

			if (_effectsForm != null && !_effectsForm.IsDisposed)
			{
				EffectsForm.Dispose();
			}

			if (_effectEditorForm != null && !_effectEditorForm.IsDisposed)
			{
				EffectEditorForm.Dispose();
			}

			if (_marksForm != null && !_marksForm.IsDisposed)
			{
				_marksForm.Dispose();	
			}

			if (_colorLibraryForm != null && !_colorLibraryForm.IsDisposed)
			{
				ColorLibraryForm.Dispose();
			}

			if (_curveLibraryForm != null && !_curveLibraryForm.IsDisposed)
			{
				CurveLibraryForm.Dispose();
			}

			if (_gradientLibraryForm != null && !_gradientLibraryForm.IsDisposed)
			{
				GradientLibraryForm.Dispose();
			}

			if (_layerEditor != null && !_layerEditor.IsDisposed)
			{
				_layerEditor.Dispose();
			}

			if (_findEffects != null && !_findEffects.IsDisposed)
			{
				_findEffects.Dispose();
			}

			TimelineControl.SelectionChanged -= TimelineControlOnSelectionChanged;
			TimelineControl.grid.MouseDown -= TimelineControl_MouseDown;
			TimeLineSequenceClipboardContentsChanged -= TimelineSequenceTimeLineSequenceClipboardContentsChanged;
			_timeLineGlobalEventManager.CursorMoved -= CursorMovedHandler;
			TimelineControl.ElementsSelected -= timelineControl_ElementsSelected;
			TimelineControl.ContextSelected -= timelineControl_ContextSelected;
			TimelineControl.TimePerPixelChanged -= TimelineControl_TimePerPixelChanged;
			TimelineControl.VisibleTimeStartChanged -= TimelineControl_VisibleTimeStartChanged;
			Row.RowHeightChanged -= TimeLineControl_Row_RowHeightChanged;
			effectGroupsToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			basicToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing; 
			pixelToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing; 
			deviceToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			add_RemoveContextToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			add_RemoveLibraryToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			toolbarsToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			toolbarsToolStripMenuItem_Effect.DropDown.Closing -= toolStripMenuItem_Closing;
			toolbarToolStripMenuItem.DropDown.Closing -= toolStripMenuItem_Closing;
			ColorLibraryForm.SelectionChanged -= Populate_Colors;
			CurveLibraryForm.SelectionChanged -= Populate_Curves;
			GradientLibraryForm.SelectionChanged -= Populate_Gradients;
			toolBarsToolStripMenuItemLibraries.DropDown.Closing -= toolStripMenuItem_Closing;
			//TimelineControl.DataDropped -= timelineControl_DataDropped;

			Execution.ExecutionStateChanged -= OnExecutionStateChanged;
			_autoSaveTimer.Stop();
			_autoSaveTimer.Tick -= AutoSaveEventProcessor;

			if (_curveLibrary != null)
			{
				_curveLibrary.CurvesChanged -= CurveLibrary_CurvesChanged;
			}

			if (_colorGradientLibrary != null)
			{
				_colorGradientLibrary.GradientsChanged -= ColorGradientsLibrary_CurveChanged;
			}

			//GRRR - make the color collections a library at some mouseLocation

			foreach (ToolStripItem toolStripItem in modeToolStripDropDownButton_SnapToStrength.DropDownItems)
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
				RemoveMarkCollectionHandlers(_sequence.LabeledMarkCollections);
				_sequence.Dispose();
				_sequence = null;
			}

			dockPanel.Dispose();

			base.Dispose(disposing);
			
		}

		private void TimelineControlGrid_DragDrop(object sender, DragEventArgs e)
		{
			//Checks to see if drag items are of a Filetype, dragged from Windows Explorer.
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] filePaths = e.Data.GetData(DataFormats.FileDrop) as string[]; //Stores path of all selected files.
				if (filePaths != null)
				{
					HandleFileDrop(filePaths);
				}
			}
			else
			{
				Point p = new Point(e.X, e.Y);
				//Check for effect drop
				if (e.Data.GetDataPresent(typeof (Guid)))
				{
					Guid g = (Guid) e.Data.GetData(typeof (Guid));
					EffectDropped(g, TimelineControl.grid.TimeAtPosition(p), TimelineControl.grid.RowAtPosition(p));
				}

				//Everything else applies to a element
				Element element = TimelineControl.grid.ElementAtPosition(p);
				if (element != null)
				{
					if (e.Data.GetDataPresent(typeof (ColorGradient)))
					{
						ColorGradient cg = (ColorGradient) e.Data.GetData(typeof (ColorGradient));
						HandleGradientDrop(element, cg);
					}
					else if (e.Data.GetDataPresent(typeof (Curve)))
					{
						Curve curve = (Curve) e.Data.GetData(typeof (Curve));
						HandleCurveDrop(element, curve);
					}
					else if (e.Data.GetDataPresent(typeof (Color)))
					{
						Color color = (Color) e.Data.GetData(typeof (Color));
						HandleColorDrop(element, color);
					}

				}
			}

		}

		private void TimelineControlGrid_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = IsValidDataObject(e.Data, new Point(e.X, e.Y));
		}

		private void TimelineControlGrid_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = IsValidDataObject(e.Data, new Point(e.X, e.Y));
		}

		private DragDropEffects IsValidDataObject(IDataObject dataObject, Point mouseLocation)
		{
			if (dataObject.GetDataPresent(typeof(Guid)) && TimelineControl.grid.RowAtPosition(mouseLocation)!=null)
			{
				return DragDropEffects.Copy;
			}
			Element element = TimelineControl.grid.ElementAtPosition(mouseLocation);
			if (element != null)
			{

				var propertyData = MetadataRepository.GetProperties(element.EffectNode.Effect);
				if (dataObject.GetDataPresent(typeof(Color)) &&
					propertyData.Any(x => (x.PropertyType == typeof(Color) || x.PropertyType == typeof(ColorGradient) || x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable))
				{
					var discreteColors = GetDiscreteColors(element.EffectNode.Effect);
					if (discreteColors.Any())
					{
						var c = (Color)dataObject.GetData(typeof(Color));
						if (!discreteColors.Contains(c))
						{
							return DragDropEffects.None;
						}
					}
					
					return DragDropEffects.Copy;
				}
				if (dataObject.GetDataPresent(typeof(ColorGradient)) &&
					propertyData.Any(x => (x.PropertyType == typeof(ColorGradient) || x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable))
				{
					var discreteColors = GetDiscreteColors(element.EffectNode.Effect);
					if (discreteColors.Any())
					{
						var c = (ColorGradient)dataObject.GetData(typeof(ColorGradient));
						var colors = c.Colors.Select(x => x.Color.ToRGB().ToArgb());
						if (!discreteColors.IsSupersetOf(colors))
						{
							return DragDropEffects.None;
						}
					}
					return DragDropEffects.Copy;
				}
				if (dataObject.GetDataPresent(typeof(Curve)) &&
					propertyData.Any(x => (x.PropertyType == typeof(Curve) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable))
				{
					return DragDropEffects.Copy;
				}
			}

			return DragDropEffects.None;	
		}

		private HashSet<Color> GetDiscreteColors(IEffect effect)
		{
			var validColors = new HashSet<Color>();
			validColors.AddRange(effect.TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
	
			return validColors;
		}

		private Form_Effects _effectsForm;

		private Form_Effects EffectsForm
		{
			get
			{
				if (_effectsForm != null && !_effectsForm.IsDisposed)
				{
					return _effectsForm;
				}
				
				_effectsForm = new Form_Effects(TimelineControl);
				_effectsForm.EscapeDrawMode += EscapeDrawMode;
				_effectsForm.Closing += _effectsForm_Closing;
				return _effectsForm;
			}
		}

		
		private Form_Marks _marksForm;

		private Form_Marks MarksForm
		{
			get
			{
				if (_marksForm != null && !_marksForm.IsDisposed)
				{
					return _marksForm;
				}

				_marksForm = new Form_Marks(_sequence);
				//_marksForm.PopulateMarkCollectionsList(null);
				//_marksForm.MarkCollectionChecked += MarkCollection_Checked;
				//_marksForm.EditMarkCollection += MarkCollection_Edit;
				//_marksForm.ChangedMarkCollection += MarkCollection_Changed;
				_marksForm.Closing += _marksForm_Closing;

				return _marksForm;
			}
		}

		private LayerEditor _layerEditor;

		private LayerEditor LayerEditor
		{
			get
			{
				if (_layerEditor != null && !_layerEditor.IsDisposed)
				{
					return _layerEditor;
				}

				_layerEditor = new LayerEditor(_sequence.SequenceLayers);
				_layerEditor.LayersChanged += LayerEditorLayersChanged;
				_layerEditor.Closing +=LayerEditorOnClosing;

				return _layerEditor;
			}
		}

		private Form_ColorLibrary _colorLibraryForm;

		private Form_ColorLibrary ColorLibraryForm
		{
			get
			{
				if (_colorLibraryForm != null && !_colorLibraryForm.IsDisposed)
				{
					return _colorLibraryForm;
				}

				_colorLibraryForm = new Form_ColorLibrary(TimelineControl);
				ColorLibraryForm.SelectionChanged += Populate_Colors;
				return _colorLibraryForm;
			}
		}

		private Form_CurveLibrary _curveLibraryForm;

		private Form_CurveLibrary CurveLibraryForm
		{
			get
			{
				if (_curveLibraryForm != null && !_curveLibraryForm.IsDisposed)
				{
					return _curveLibraryForm;
				}

				_curveLibraryForm = new Form_CurveLibrary(TimelineControl);
				CurveLibraryForm.SelectionChanged += Populate_Curves;
				return _curveLibraryForm;
			}
		}

		private Form_GradientLibrary _gradientLibraryForm;

		private Form_GradientLibrary GradientLibraryForm
		{
			get
			{
				if (_gradientLibraryForm != null && !_gradientLibraryForm.IsDisposed)
				{
					return _gradientLibraryForm;
				}

				_gradientLibraryForm = new Form_GradientLibrary(TimelineControl);
				GradientLibraryForm.SelectionChanged += Populate_Gradients;
				return _gradientLibraryForm;
			}
		}

		private FormEffectEditor _effectEditorForm;

		private FormEffectEditor EffectEditorForm
		{
			get
			{
				if (_effectEditorForm != null && !_effectEditorForm.IsDisposed)
				{
					return _effectEditorForm;
				}

				_effectEditorForm = new FormEffectEditor(this);
				return _effectEditorForm;
			}
		}

		private FindEffectForm _findEffects;

		private FindEffectForm FindEffects
		{
			get
			{
				if (_findEffects != null && !_findEffects.IsDisposed)
				{
					return _findEffects;
				}

				_findEffects = new FindEffectForm(TimelineControl, Sequence.GetSequenceLayerManager());

				return _findEffects;
			}
		}

		private void _marksForm_Closing(object sender, CancelEventArgs e)
		{
			//MarksForm.MarkCollectionChecked -= MarkCollection_Checked;
			//MarksForm.EditMarkCollection -= MarkCollection_Edit;
			//MarksForm.ChangedMarkCollection -= MarkCollection_Changed;
			MarksForm.Closing -= _marksForm_Closing;
		}

		private void _effectsForm_Closing(object sender, CancelEventArgs e)
		{
			EffectsForm.EscapeDrawMode -= EscapeDrawMode;
			EffectsForm.Closing -= _effectsForm_Closing;
		}

		private void LayerEditorOnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			LayerEditor.Closing -= LayerEditorOnClosing;
		}

		private void LayerEditorLayersChanged(object sender, EventArgs e)
		{
			SequenceModified();
		}

		private Form_Grid _gridForm;

		private Form_Grid GridForm
		{
			get { return _gridForm ?? (_gridForm = new Form_Grid()); }
		}

		internal TimelineControl TimelineControl
		{
			get { return _gridForm?.TimelineControl; }
		}

		private void PopulateDragBoxFilterDropDown()
		{
			ToolStripMenuItem dbfInvertMenuItem = new ToolStripMenuItem("Invert Selection")
			{
				ShortcutKeys = Keys.Control | Keys.I,
				ShowShortcutKeys = true
			};
			dbfInvertMenuItem.MouseUp += (sender, e) => modeToolStripDropDownButton_DragBoxFilter.ShowDropDown();
			dbfInvertMenuItem.Click += (sender, e) =>
			{
				foreach (ToolStripMenuItem mnuItem in modeToolStripDropDownButton_DragBoxFilter.DropDownItems)
				{
					mnuItem.Checked = (!mnuItem.Checked);
				}
			};
			modeToolStripDropDownButton_DragBoxFilter.DropDownItems.Add(dbfInvertMenuItem);

			foreach (IEffectModuleDescriptor effectDesriptor in
				ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				if (effectDesriptor.EffectName == "Nutcracker") continue; //Remove this when the Nutcracker module is removed
				//Populate Drag Box Filter drop down with effect types
				ToolStripMenuItem dbfMenuItem = new ToolStripMenuItem(effectDesriptor.EffectName,
					effectDesriptor.GetRepresentativeImage());
				dbfMenuItem.CheckOnClick = true;
				dbfMenuItem.CheckStateChanged += (sender, e) =>
				{
					//OK, now I don't remember why I put this here, I think to make sure the list is updated when using the invert
					if (dbfMenuItem.Checked) TimelineControl.grid.DragBoxFilterTypes.Add(effectDesriptor.TypeId);
					else TimelineControl.grid.DragBoxFilterTypes.Remove(effectDesriptor.TypeId);
					//Either way...(the user is getting ready to use the filter)
					modeToolStripButton_DragBoxFilter.Checked = true;
				};
				dbfMenuItem.Click += (sender, e) => modeToolStripDropDownButton_DragBoxFilter.ShowDropDown();
				modeToolStripDropDownButton_DragBoxFilter.DropDownItems.Add(dbfMenuItem);
			}
		}

		private void LoadAvailableEffects()
		{
			foreach (IEffectModuleDescriptor effectDesriptor in
				ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				if (effectDesriptor.EffectName == "Nutcracker") continue; //Remove this when the Nutcracker module is removed
				// Add an entry to the menu
				ToolStripMenuItem menuItem = new ToolStripMenuItem(effectDesriptor.EffectName) {Tag = effectDesriptor.TypeId};
				menuItem.Click += (sender, e) =>
				{
					Row destination = TimelineControl.ActiveRow ?? TimelineControl.SelectedRow;
					if (destination != null)
					{
						AddNewEffectById((Guid) menuItem.Tag, destination, _timeLineGlobalStateManager.CursorPosition,
							GetDefaultEffectDuration(_timeLineGlobalStateManager.CursorPosition), true); // TODO: get a proper time
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
		private void LoadSystemNodesToRows(bool clearCurrentRows = true)
		{
			TimelineControl.AllowGridResize = false;
			_elementNodeToRows = new Dictionary<ElementNode, List<Row>>();

			_suppressModifiedEvents = true;
			//Scale our default pixel height for the rows
			if (_sequence.DefaultRowHeight != 0)
			{
			TimelineControl.rowHeight = _sequence.DefaultRowHeight;
			}
			else
			{
				TimelineControl.rowHeight = (int)(TimelineControl.DefaultRowHeight * _scaleFactor);
			}

			if (clearCurrentRows)
				TimelineControl.ClearAllRows();

			TimelineControl.EnableDisableHandlers(false);
			foreach (ElementNode node in VixenSystem.Nodes.GetRootNodes())
			{
				AddNodeAsRow(node, null);
			}

			var rowSettings = _sequence.RowSettings;

			//Adjusts Row heights based on saved row height settings.
			//if (_sequence.RowHeightSettings != null)
			//{
			//	_suppressModifiedEvents = true;
			//	foreach (RowHeightSetting rowSettings in _sequence.RowHeightSettings)
			//	{
			//		foreach (Row row in TimelineControl.Rows)
			//		{
			//			if (row.Name == rowSettings.RowName)
			//			{
			//				row.Height = rowSettings.RowHeight;
			//			}
			//		}
			//	}

			//	_suppressModifiedEvents = false;
			//}

			//Expand groups based on save settings
			foreach (Row row in TimelineControl.Rows)
			{
				RowSetting rowSetting;
				if(rowSettings.TryGetValue(row.TreeId(), out rowSetting))
				{
					row.TreeOpen = rowSetting.Expanded;
					row.Height = rowSetting.RowHeight;
					row.Visible = rowSetting.Visible;
					//if (row.ParentRow != null)
					//{
					//	row.TreeOpen = true;
					//	row.Visible = true;
					//}
				}
			}
			_suppressModifiedEvents = false;
			TimelineControl.EnableDisableHandlers();

			TimelineControl.LayoutRows();
			TimelineControl.ResizeGrid();
		}

		private void loadTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			UpdateToolStrip4(string.Format("Please Wait. Loading: {0}", _loadingWatch.Elapsed));
		}

		private delegate void UpdateToolStrip4Delegate(string text, int timeout = 0);

		private Timer clearStatusTimer = new Timer();

		/// <summary>
		/// Changes the text of the Status bar, with optional timeOut to clear the text after x seconds.
		/// Default timeOut is 0, the text will stay set indefinitly.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="timeOut"></param>
		private void UpdateToolStrip4(string text, int timeOut = 0)
		{
			if (InvokeRequired)
			{
				Invoke(new UpdateToolStrip4Delegate(UpdateToolStrip4), text, timeOut);
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

			if (timeOut > 0)
			{
				clearStatusTimer.Interval = timeOut*1000;
				clearStatusTimer.Tick += clearStatusTimer_Tick;
				clearStatusTimer.Start();
			}
		}

		private void clearStatusTimer_Tick(object sender, EventArgs e)
		{
			clearStatusTimer.Stop();
			clearStatusTimer.Tick -= clearStatusTimer_Tick;
			UpdateToolStrip4(string.Empty);
		}

		private System.Timers.Timer _loadTimer;
		private Stopwatch _loadingWatch;

		private void LoadSequence(ISequence sequence)
		{
			var taskQueue = new Queue<Task>();

			if (_loadTimer == null)
			{
				_loadTimer = new System.Timers.Timer();
				_loadTimer.Elapsed += loadTimer_Elapsed;
				_loadTimer.Interval = 250;
			}
			_loadingWatch = Stopwatch.StartNew();
			_loadTimer.Enabled = true;
			TimelineControl.SequenceLoading = true;

			// Let's get the window on the screen. Make it appear to instantly load.
			Invalidate(true);
			Update();

			try
			{
				// default the sequence to 1 minute if it's not set
				if (_sequence.Length == TimeSpan.Zero)
					_sequence.Length = DefaultSequenceTime;

				SequenceLength = _sequence.Length;
				SetTitleBarText();

				// update our program context with this sequence
				OpenSequenceContext(sequence);

				// clear out all the old data
				LoadSystemNodesToRows();

				// load the new data: get all the commands in the sequence, and make a new element for each of them.
				_effectNodeToElement = new Dictionary<EffectNode, Element>();

				TimelineControl.grid.SuppressInvalidate = true; //Hold off invalidating the grid while we bulk load.
				TimelineControl.grid.SupressRendering = true; //Hold off rendering while we load elements. 
				// This takes quite a bit of time so queue it up
				//The sequence loader now adds the media to the effects on sequences it loads so we don't have to do it here.
				taskQueue.Enqueue(Task.Factory.StartNew(() => AddElementsForEffectNodes(_sequence.SequenceData.EffectData, false)));

				// Now that it is queued up, let 'er rip and start background rendering when complete.
				Task.Factory.ContinueWhenAll(taskQueue.ToArray(), completedTasks =>
				{
					// Clear the loading toolbar
					_loadingWatch.Stop();
					TimelineControl.SequenceLoading = false;
					_loadTimer.Enabled = false;
					UpdateToolStrip4(string.Empty);
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
					SequenceModified();
				}
				else
				{
					SequenceNotModified();
				}

				if (_sequence.TimePerPixel > TimeSpan.Zero)
				{
					TimelineControl.TimePerPixel = _sequence.TimePerPixel;
				}

				if (_sequence.VisibleTimeStart > TimeSpan.Zero)
				{
					TimelineControl.VisibleTimeStart = _sequence.VisibleTimeStart;
				}

				TimelineControl.grid.SequenceLayers = Sequence.GetSequenceLayerManager();

				Logging.Debug("Sequence {0} took {1} to load.", sequence.Name, _loadingWatch.Elapsed);
			}
			catch (Exception ee)
			{
				Logging.Error("TimedSequenceEditor: <LoadSequence> - Error loading sequence.", ee);
			}
		}

		/// <summary>
		/// Saves the current sequence to a file. May prompt for a file name to save the sequence to if needed.
		/// </summary>
		/// <param name="filePath">The filename to save the sequence to. If null, the filename in the sequence will be used.
		/// If that is also null, the user will be prompted for a filename.</param>
		/// <param name="forcePrompt">If true, the user will always be prompted for a filename to save the sequence to.</param>
		private void SaveSequence(string filePath = null, bool forcePrompt = false)
		{
			if (_sequence != null)
			{
				if (filePath == null | forcePrompt)
				{
					if (_sequence.FilePath.Trim() == "" || forcePrompt)
					{
						// Updated to use the OS SaveFileDialog functionality 8/1/2012 JU
						// Edit this type to be the more generic type to support importing into timed sequnces 12 FEB 2013 - JEMA
						EditorModuleDescriptorBase descriptor = ((OwnerModule.Descriptor) as EditorModuleDescriptorBase);
						saveFileDialog.InitialDirectory = SequenceService.SequenceDirectory;

						//While this should never happen, ReSharper complains about it, added logging just in case.
						if (descriptor != null)
						{
							string filter = descriptor.TypeName + " (*" + string.Join(", *", _sequence.FileExtension) + ")|*" +
							                string.Join("; *", _sequence.FileExtension);
							saveFileDialog.DefaultExt = _sequence.FileExtension;
							saveFileDialog.Filter = filter;
						}
						else
						{
							Logging.Error("TimedSequenceEditor: <SaveSequence> - Save Sequence dialog filter could not be set, EditorModuleDescriptorBase is null!");
						}

						DialogResult result = saveFileDialog.ShowDialog();
						if (result == DialogResult.OK)
						{
							string name = saveFileDialog.FileName;
							string extension = Path.GetExtension(saveFileDialog.FileName);

							// if the given extension isn't valid for this type, then keep the name intact and add an extension
							if (extension != _sequence.FileExtension)
							{
								name = name + _sequence.FileExtension;
								Logging.Info("TimedSequenceEditor: <SaveSequence> - Incorrect extension provided for timed sequence, appending one.");
							}
							SaveGridRowSettings();
							_sequence.Save(name);
							SetTitleBarText();
						}
						else
						{
							//user canceled save
							return;
						}
					}
					else
					{
						SaveGridRowSettings();
						_sequence.Save();
					}
				}
				else
				{
					SaveGridRowSettings();
					_sequence.Save(filePath);
					SetTitleBarText();
				}

			}
			else
			{
				Logging.Error("TimedSequenceEditor: <SaveSequence> - Trying to save with null _sequence!");
			}
			
			SequenceNotModified();
		}

		private void SaveGridRowSettings() //Adds Row and Grid settings to _sequence to be saved. 
		{			
			//Add Default Row Height
			_sequence.DefaultRowHeight = TimelineControl.rowHeight;
			
			//Add Playback start and end time
			_sequence.DefaultPlaybackStartTime = TimelineControl.PlaybackStartTime;
			_sequence.DefaultPlaybackEndTime = TimelineControl.PlaybackEndTime;

			var rowSettings = _sequence.RowSettings;
			rowSettings.Clear();
			//Stores the settigns for rows that have been altered
			foreach (var row in TimelineControl.Rows)
			{
				if (row.Height > TimelineControl.rowHeight + 7 || row.Height < TimelineControl.rowHeight - 7) //The 7 is the buffer size and will not save the Row Height if within 7 pixels. This is if a user manual adjusts the Row to matach the others (default height) and is a small amout off.
				{
					rowSettings.Add(row.TreeId(), new RowSetting(row.Height, row.TreeOpen, row.Visible));
				}
				else if (row.TreeOpen)
				{
					rowSettings.Add(row.TreeId(), new RowSetting(row.Height, true, row.Visible));
				}
			}
		}

		private void SaveColorCollections()
		{
			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer dataSer = new DataContractSerializer(typeof (List<ColorCollection>));
			var dataWriter = XmlWriter.Create(_colorCollectionsPath, xmlsettings);
			dataSer.WriteObject(dataWriter, _colorCollections);
			dataWriter.Close();
		}

		private void LoadColorCollections()
		{
			if (File.Exists(_colorCollectionsPath))
			{
				using (FileStream reader = new FileStream(_colorCollectionsPath, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof (List<ColorCollection>));
					_colorCollections = (List<ColorCollection>) ser.ReadObject(reader);
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

		private void UpdateGridSnapTimes()
		{
			TimelineControl.grid.CreateSnapPointsFromMarks();
		}

		private void PopulateSnapStrength(int strength)
		{
			TimelineControl.grid.SnapStrength = strength;
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
					int i = 0;
					audioToolStripButton_Audio_Devices.DropDownItems.Clear();
					fmod.AudioDevices.OrderBy(a => a.Item1).Select(b => b.Item2).ToList().ForEach(device =>
					{
						ToolStripMenuItem tsmi = new ToolStripMenuItem();
						tsmi.Text = device;
						tsmi.Tag = i;
						tsmi.Click += audioDevicesToolStripMenuItem_Click;
						audioToolStripButton_Audio_Devices.DropDownItems.Add(tsmi);
						i++;
					});
					if (audioToolStripButton_Audio_Devices.DropDownItems.Count > 0)
					{
						((ToolStripMenuItem)audioToolStripButton_Audio_Devices.DropDownItems[0]).Checked = true;
						Variables.SelectedAudioDeviceIndex = 0;
						PopulateWaveformAudio();
					}
				}
			}
		}

		private delegate void ShowInvalidAudioDialogDelegate(string audioPath);

		private void ShowInvalidAudioDialog(string audioPath)
		{
			InvalidAudioPathDialog result = new InvalidAudioPathDialog(audioPath);
			result.ShowDialog(this);

			switch (result.InvalidAudioDialogResult)
			{
				case InvalidAudioDialogResult.KeepAudio:
					//Do nothing...
					break;
				case InvalidAudioDialogResult.RemoveAudio:
					RemoveAudioAssociation(false);
					break;
				case InvalidAudioDialogResult.LocateAudio:
					AddAudioAssociation(false);
					break;
			}			
		}

		private void PopulateWaveformAudio()
		{
			if (_sequence.GetAllMedia().Any())
			{
				IMediaModuleInstance media = _sequence.GetAllMedia().First();
				Audio audio = media as Audio;
				toolStripMenuItem_removeAudio.Enabled = true;
				beatBarDetectionToolStripMenuItem.Enabled = true;
				if (audio != null)
				{
					if (audio.MediaExists)
					{
						TimelineControl.Audio = audio;
						toolStripButton_AssociateAudio.ToolTipText = string.Format("Associated Audio: {0}",
							Path.GetFileName(audio.MediaFilePath));
					}
					else
					{
						//We couldn't find the audio, ask the user what to do
						//Since we are on a worker thread ...
						ShowInvalidAudioDialogDelegate dialog = ShowInvalidAudioDialog;
						BeginInvoke(dialog, audio.MediaFilePath);
					}
				}
				else
				{
					Logging.Error("TimedSequenceEditor: <PopulateWaveformAudio> - Attempting to process null audio!");
				}
			}
		}

		/// <summary>
		/// Called to update the title bar with the filename and saved / unsaved status
		/// </summary>
		private void SetTitleBarText()
		{
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(SetTitleBarText));
			else
			{
				//Set sequence name in title bar based on the module name and current sequence name JU 8/1/2012
				//Made this more generic to support importing 12 FEB 2013 - JEMA
				var editorModuleDescriptorBase = (OwnerModule.Descriptor) as EditorModuleDescriptorBase;
				if (editorModuleDescriptorBase != null)
				{
					Text = String.Format("{0} - [{1}{2}]", editorModuleDescriptorBase.TypeName,
						String.IsNullOrEmpty(_sequence.Name) ? "Untitled" : _sequence.Name, IsModified ? " *" : "");
				}
				else
				{
					Logging.Error("TimedSequenceEditor: <SetTitleBarText> - editorModuleDesciptorBase is null!!");
				}
			}
		}

		/// <summary>Called when the sequence is modified.</summary>
		private void SequenceModified()
		{
			if (_mModified) return;
			_mModified = true;
			SetTitleBarText();
			SetAutoSave();
			// TODO: Other things, like enable save button, etc.	
		}

		/// <summary>Called when the sequence is no longer considered modified.</summary>
		private void SequenceNotModified()
		{
			if (!_mModified) return;
			_mModified = false;
			SetTitleBarText();
			SetAutoSave();
			// TODO: Other things, like disable save button, etc.
		}

		/// <summary>
		/// Removes the audio association from the sequence.
		/// </summary>
		/// <param name="showWarning">pass as false to prevent MessageBox warning</param>
		private void RemoveAudioAssociation(bool showWarning = true)
		{
			HashSet<IMediaModuleInstance> modulesToRemove = new HashSet<IMediaModuleInstance>();
			foreach (IMediaModuleInstance module in _sequence.GetAllMedia())
			{
				if (module is Audio)
				{
					modulesToRemove.Add(module);
				}
			}

			if (modulesToRemove.Count > 0 && showWarning)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Are you sure you want to remove the audio association?", "Remove existing audio?", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult != DialogResult.OK)
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
			beatBarDetectionToolStripMenuItem.Enabled = false;
			toolStripButton_AssociateAudio.ToolTipText = @"Associate Audio";

			SequenceModified();
		}

		/// <summary>
		/// Adds an audio association to the sequence
		/// </summary>
		/// <param name="showWarning">pass as false to prevent MessageBox warning when audio association already exists</param>
		private void AddAudioAssociation(bool showWarning = true)
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

			if (modulesToRemove.Count > 0 && showWarning)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Only one audio file can be associated with a sequence at a time. If you choose another, " +
									@"the first will be removed. Continue?", @"Remove existing audio?", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult != DialogResult.OK)
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("The selected file is not a supported type.", @"Warning", false, false);
					messageBox.ShowDialog();
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
					//The only true check that needs to be done is the length of the sequence to the length of the audio
					//Perhaps this was put here to avoid the popup annoyance when creating a new sequence... Ill leave it here
					if (_sequence.Length == DefaultSequenceTime)
					{
						SequenceLength = length;
					}
					else if (_sequence.Length != length)
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Do you want to resize the sequence to the size of the audio?",
											@"Resize sequence?", true, false);
						messageBox.ShowDialog();
						if (messageBox.DialogResult == DialogResult.OK)
						{
							SequenceLength = length;
						}
					}
				}

				UpdateMediaOnSupportedEffects();

				toolStripMenuItem_removeAudio.Enabled = true;
				beatBarDetectionToolStripMenuItem.Enabled = true;
				toolStripButton_AssociateAudio.ToolTipText = string.Format("Associated Audio: {0}", Path.GetFileName(openFileDialog.FileName));

				SequenceModified();
			}
		}

		#endregion

		#region Library Application Private Methods

		//Switch statements are the best method at this time for these methods

		private bool SupportsColor(Element element)
		{
			if (element == null) return false;
			var propertyData = MetadataRepository.GetProperties(element.EffectNode.Effect);				
			return propertyData.Any(x => (x.PropertyType == typeof(Color) || x.PropertyType == typeof(ColorGradient) || x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);				
		}

		private bool SupportsColorLists(Element element)
		{
			if (element == null) return false;
			var propertyData = MetadataRepository.GetProperties(element.EffectNode.Effect);
			return propertyData.Any(x => (x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);
		}

		private List<Color> GetSupportedColorsFromCollection(Element element, List<Color> colors )
		{
			var discreteColors = GetDiscreteColors(element.EffectNode.Effect);
			if (discreteColors.Any())
			{
				return discreteColors.Intersect(colors).ToList();
			}
			return colors;
		} 

		private Color GetRandomColorFromCollection(List<Color> colors)
		{
			int item = rnd.Next(colors.Count());
			return colors[item];
		}

		private void ApplyColorCollection(ColorCollection collection, bool randomOrder)
		{
			if (!collection.Color.Any()) return;

			bool skipElements = false;
			int index = 0;

			foreach (Element elem in TimelineControl.SelectedElements)
			{
				if (!SupportsColor(elem))
				{
					skipElements = true;
					continue;
				}
				var colors = GetSupportedColorsFromCollection(elem, collection.Color);				
				var properties = MetadataRepository.GetProperties(elem.EffectNode.Effect).Where(x => (x.PropertyType == typeof(Color) ||
					x.PropertyType == typeof(ColorGradient) || x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);

				Dictionary<Element, Tuple<Object, PropertyDescriptor>> elementValues = new Dictionary<Element, Tuple<object, PropertyDescriptor>>();

				foreach (var propertyData in properties)
				{					
					if (propertyData.PropertyType == typeof (Color))
					{
						var color = randomOrder ? GetRandomColorFromCollection(colors) : colors[index++ % colors.Count];
						elementValues.Add(elem,
							new Tuple<object, PropertyDescriptor>(propertyData.Descriptor.GetValue(elem.EffectNode.Effect),
								propertyData.Descriptor));
						UpdateEffectProperty(propertyData.Descriptor, elem, color);
					}
					else
					{
						//The rest take a gradient.
						if (propertyData.PropertyType == typeof(ColorGradient))
						{
							var color = randomOrder ? GetRandomColorFromCollection(colors) : colors[index++ % colors.Count];
							elementValues.Add(elem,
								new Tuple<object, PropertyDescriptor>(propertyData.Descriptor.GetValue(elem.EffectNode.Effect),
									propertyData.Descriptor));
							UpdateEffectProperty(propertyData.Descriptor, elem, new ColorGradient(color));
						}
						else if (propertyData.PropertyType == typeof(List<ColorGradient>))
						{
							var gradients = propertyData.Descriptor.GetValue(elem.EffectNode.Effect) as List<ColorGradient>;
							if (gradients != null)
							{
								var newGradients = gradients.ToList();
								for (int i = 0; i < newGradients.Count; i++)
								{
									newGradients[i] =
										new ColorGradient(randomOrder ? GetRandomColorFromCollection(colors) : colors[index++ % colors.Count]);
								}
								elementValues.Add(elem,
									new Tuple<object, PropertyDescriptor>(gradients,
										propertyData.Descriptor));
								UpdateEffectProperty(propertyData.Descriptor, elem, newGradients);
							}

						}
						else if (propertyData.PropertyType == typeof(List<GradientLevelPair>))
						{
							var gradients = propertyData.Descriptor.GetValue(elem.EffectNode.Effect) as List<GradientLevelPair>;
							if (gradients != null)
							{
								var newGradients = gradients.ToList();
								for (int i = 0; i < newGradients.Count; i++)
								{
									newGradients[i] = new GradientLevelPair(new ColorGradient(randomOrder ? GetRandomColorFromCollection(colors) : colors[index++ % colors.Count]), new Curve(gradients[i].Curve));
								}
								elementValues.Add(elem,
									new Tuple<object, PropertyDescriptor>(gradients,
										propertyData.Descriptor));
								UpdateEffectProperty(propertyData.Descriptor, elem, newGradients);
							}
						}
					}
				}

				if (elementValues.Any())
				{
					var undo = new EffectsPropertyModifiedUndoAction(elementValues);
					AddEffectsModifiedToUndo(undo);
				}
			}

			if (skipElements)
			{
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("One or more effects were selected that do not support colors.\nAll effects that do were updated.",
									@"Information", true, false);
				messageBox.ShowDialog();
			}
			SequenceModified();
		}

		#endregion

		#region Event Handlers

		private void CurveLibrary_CurvesChanged(object sender, EventArgs e)
		{
			CheckAndRenderDirtyElementsAsync();
		}

		private void ColorGradientsLibrary_CurveChanged(object sender, EventArgs e)
		{
			CheckAndRenderDirtyElementsAsync();
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
			catch (Exception ex)
			{
				Logging.Error("TimedSequenceEditor: <OnRenderProgressChanged> - Error updating rendering progress indicator.", ex);
			}
		}

		/// <summary>
		/// Register this form as a Clipboard Viewer application
		/// </summary>
		private void RegisterClipboardViewer()
		{
			_clipboardViewerNext = User32.SetClipboardViewer(this.Handle);
		}

		/// <summary>
		/// Remove this form from the Clipboard Viewer list
		/// </summary>
		private void UnregisterClipboardViewer()
		{
			User32.ChangeClipboardChain(this.Handle, _clipboardViewerNext);
		}

		protected override void WndProc(ref Message m)
		{
			switch ((Msgs)m.Msg)
			{
				//
				// The WM_DRAWCLIPBOARD message is sent to the first window 
				// in the clipboard viewer chain when the content of the 
				// clipboard changes. This enables a clipboard viewer 
				// window to display the new content of the clipboard. 
				//
				case Msgs.WM_DRAWCLIPBOARD:

					Debug.WriteLine("WindowProc DRAWCLIPBOARD: " + m.Msg, "WndProc");

					if (ClipboardHasData())
					{
						_TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);
					}

					//
					// Each window that receives the WM_DRAWCLIPBOARD message 
					// must call the SendMessage function to pass the message 
					// on to the next window in the clipboard viewer chain.
					//
					User32.SendMessage(_clipboardViewerNext, m.Msg, m.WParam, m.LParam);
					break;

				//
				// The WM_CHANGECBCHAIN message is sent to the first window 
				// in the clipboard viewer chain when a window is being 
				// removed from the chain. 
				//
				case Msgs.WM_CHANGECBCHAIN:
					Debug.WriteLine("WM_CHANGECBCHAIN: lParam: " + m.LParam, "WndProc");

					// When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
					// it should call the SendMessage function to pass the message to the 
					// next window in the chain, unless the next window is the window 
					// being removed. In this case, the clipboard viewer should save 
					// the handle specified by the lParam parameter as the next window in the chain. 

					//
					// wParam is the Handle to the window being removed from 
					// the clipboard viewer chain 
					// lParam is the Handle to the next window in the chain 
					// following the window being removed. 
					if (m.WParam == _clipboardViewerNext)
					{
						//
						// If wParam is the next clipboard viewer then it
						// is being removed so update pointer to the next
						// window in the clipboard chain
						//
						_clipboardViewerNext = m.LParam;
					}
					else
					{
						User32.SendMessage(_clipboardViewerNext, m.Msg, m.WParam, m.LParam);
					}
					break;

				default:
					//
					// Let the form process the messages that we are
					// not interested in
					//
					base.WndProc(ref m);
					break;
			}
		}

		private void TimelineSequenceTimeLineSequenceClipboardContentsChanged(object sender, EventArgs eventArgs)
		{
			UpdatePasteMenuStates();
		}

		private void TimelineControlOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			editToolStripButton_Copy.Enabled = editToolStripButton_Cut.Enabled = TimelineControl.SelectedElements.Any();
			toolStripMenuItem_Copy.Enabled = toolStripMenuItem_Cut.Enabled = TimelineControl.SelectedElements.Any();
			toolStripMenuItem_deleteElements.Enabled = TimelineControl.SelectedElements.Any();
		}

		private void TimelineControl_MouseDown(object sender, MouseEventArgs e)
		{
			//TimelineControl.ruler.ClearSelectedMarks();
			if (e.Button != MouseButtons.Right)
			{
				MarksSelectionManager.Manager().ClearSelected();
				Invalidate(true);
			}
		}

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			TimedSequenceElement element = sender as TimedSequenceElement;
			TimelineControl.grid.RenderElement(element);
			SequenceModified();
		}

		protected void ElementTimeChangedHandler(object sender, EventArgs e)
		{
			//TimedSequenceElement element = sender as TimedSequenceElement;
			SequenceModified();
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

			if (movedElement != null)
			{
				movedElement.TargetNodes = new[] {newElement};

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
			}

			else
			{
				Logging.Error("TimedSequenceEditor: <ElementChangedRowsHandler> - movedElement is null!");
			}

			SequenceModified();
		}

		//Sorry about this, was the only way I could find to handle the escape press if
		//the effects tree still had focus. Because... someone will do this......
		protected void EscapeDrawMode(object sender, EventArgs e)
		{
			EffectsForm.DeselectAllNodes();
			TimelineControl.grid.EnableDrawMode = false;
			modeToolStripButton_DrawMode.Checked = false;
			modeToolStripButton_SelectionMode.Checked = true;
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
						newEffects.Add(CreateEffectNode(newEffect, drawingRow, e.StartTime, e.Duration));
					}
					catch (Exception ex)
					{
						string msg = "TimedSequenceEditor: <DrawElement> - error adding effect of type " +
						             newEffect.Descriptor.TypeId + " to row " +
						             ((drawingRow == null) ? "<null>" : drawingRow.Name);
							Logging.Error(msg, ex);
					}
				}
				AddEffectNodes(newEffects);
				SequenceModified();
				var act = new EffectsAddedUndoAction(this, newEffects);
				_undoMgr.AddUndoAction(act);
				SelectEffectNodes(newEffects);
			}
		}

		protected void ElementDoubleClickedHandler(object sender, ElementEventArgs e)
		{
			TimedSequenceElement element = e.Element as TimedSequenceElement;

			if (element == null || element.EffectNode == null)
			{
				Logging.Error("TimedSequenceEditor: <ElementDoubleClickedHandler> - Element doesn't have an associated effect!");
				return;
			}

			EditElement(element);
		}

		private void EditElement(TimedSequenceElement element)
		{
			EditElements(new[] {element});
		}

		private void EditElements(IEnumerable<TimedSequenceElement> elements, string elementType = null)
		{
			if (elements == null)
				return;

			//Restrict the pop up editor to only Nutcracker effects for now. All other effects are supported by the new
			//effect editor docker. This will be deprecated in some future version once the Nutcracker effects are converted
			//to indivudual supported effects.
			elementType = "Nutcracker";

			IEnumerable<TimedSequenceElement> editElements;

			editElements = elementType == null
				? elements
				: elements.Where(element => element.EffectNode.Effect.EffectName == elementType);

			if (!editElements.Any())
			{
				return;
			}

			using (
				TimedSequenceEditorEffectEditor editor = new TimedSequenceEditorEffectEditor(editElements.Select(x => x.EffectNode))
				)
			{
				DialogResult result = editor.ShowDialog();
				if (result == DialogResult.OK)
				{
					foreach (Element element in editElements)
					{
						//TimelineControl.grid.RenderElement(element);
						element.UpdateNotifyContentChanged();
					}
					SequenceModified();
				}
			}
		}

		private void TimeLineControl_Row_RowHeightChanged(object sender, EventArgs e)
		{
			if (!_suppressModifiedEvents)
			{
				_editorStateModified = true;
			}
		}

		private void TimelineControl_TimePerPixelChanged(object sender, EventArgs e)
		{
			if (_sequence.TimePerPixel != TimelineControl.TimePerPixel)
			{
				_sequence.TimePerPixel = TimelineControl.TimePerPixel;
				_editorStateModified = true;
			}
		}

		private void TimelineControl_VisibleTimeStartChanged(object sender, EventArgs e)
		{
			if (_sequence.VisibleTimeStart != TimelineControl.VisibleTimeStart)
			{
				_sequence.VisibleTimeStart = TimelineControl.VisibleTimeStart;
				_editorStateModified = true;
			}
		}


		private void ConfigureLayerMenu(ContextSelectedEventArgs e)
		{
			if ((e.ElementsUnderCursor != null && e.ElementsUnderCursor.Any()) || TimelineControl.SelectedElements.Any())
			{
				var layers = Sequence.GetAllLayers();
				if (layers.Count() > 1)
				{
					ToolStripMenuItem contextMenuToLayer = new ToolStripMenuItem("Layer")
					{
						Enabled = true,
						Image = Resources.layers,
						ToolTipText = @"Assign effects to a layer"
					};
					
					HashSet<Guid> layersUsed = new HashSet<Guid>();
					var sequenceLayers = Sequence.GetSequenceLayerManager();
					if (TimelineControl.SelectedElements.Any())
					{
						foreach (var selectedElement in TimelineControl.SelectedElements)
						{
							var curentLayer = sequenceLayers.GetLayer(selectedElement.EffectNode);
							if (layersUsed.Contains(curentLayer.Id) == false)
							{
								layersUsed.Add(curentLayer.Id);
								if (layersUsed.Count == sequenceLayers.Count)
								{
									break;
								}
							}
						}
					}
					else
					{
						foreach (var elementUnderCursor in e.ElementsUnderCursor)
						{
							var curentLayer = sequenceLayers.GetLayer(elementUnderCursor.EffectNode);
							if (layersUsed.Contains(curentLayer.Id) == false)
							{
								layersUsed.Add(curentLayer.Id);
								if (layersUsed.Count == sequenceLayers.Count)
								{
									break;
								}
							}
						}
					}
					Bitmap checkMarkColor;
					int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
					if (layersUsed.Count == 1)
					{
						checkMarkColor = Tools.GetIcon(Resources.check_mark, iconSize);
					}
					else
					{
						checkMarkColor = Tools.GetIcon(Resources.check_markMedium, iconSize);
					}

					
					foreach (var layer in layers.Reverse())
					{
						var item = new ToolStripMenuItem(layer.LayerName);
						item.Tag = layer;
						item.ToolTipText = layer.FilterName;

						if (layersUsed.Contains(layer.Id))
						{
							item.Image = checkMarkColor;
						}
						
						contextMenuToLayer.DropDownItems.Add(item);
						item.Click += (sender, args) =>
						{
							var el = e.ElementsUnderCursor;
							Dictionary<IEffectNode, ILayer> modifiedNodes = new Dictionary<IEffectNode, ILayer>();
							var newLayer = (ILayer) item.Tag;
							//First try to apply to selected elements
							if (TimelineControl.SelectedElements.Any())
							{
								foreach (var selectedElement in TimelineControl.SelectedElements)
								{
									var curentLayer = sequenceLayers.GetLayer(selectedElement.EffectNode);
									if (newLayer != curentLayer)
									{
										modifiedNodes.Add(selectedElement.EffectNode, curentLayer);
										sequenceLayers.AssignEffectNodeToLayer(selectedElement.EffectNode, newLayer);
									}
								}
							}
							else if (el != null && el.Any()) 
							{
								//if there are no selected elements, then try to apply to the element under the cursor
								foreach (var selectedElement in el)
								{
									var curentLayer = sequenceLayers.GetLayer(selectedElement.EffectNode);
									if (newLayer != curentLayer)
									{
										modifiedNodes.Add(selectedElement.EffectNode, curentLayer);
										sequenceLayers.AssignEffectNodeToLayer(selectedElement.EffectNode, newLayer);
									}
								}
							}
							
							if (modifiedNodes.Any())
							{
								var undo = new EffectsLayerChangedUndoAction(this, modifiedNodes);
								_undoMgr.AddUndoAction(undo);
								SequenceModified();
							}
						};
					}
					_contextMenuStrip.Items.Add(contextMenuToLayer);
				}
			}
		}

		private void AddMultipleEffects(TimeSpan startTime, String effectName, Guid effectId, Row row)
		{
			var eDialog = new Form_AddMultipleEffects();
			if (ModifierKeys == (Keys.Shift | Keys.Control) && _amLastEffectCount > 0)
			{
				eDialog.EffectCount = _amLastEffectCount;
				eDialog.StartTime = _amLastStartTime;
				eDialog.Duration = _amLastDuration;
				eDialog.DurationBetween = _amLastDurationBetween;
			}
			else
			{
				eDialog.EffectCount = 2;
				eDialog.StartTime = startTime;
				eDialog.Duration = TimeSpan.FromSeconds(2);
				eDialog.DurationBetween = TimeSpan.FromSeconds(2);
			}
			eDialog.EffectName = effectName;
			eDialog.SequenceLength = eDialog.EndTime = SequenceLength;
			eDialog.MarkCollections = _sequence.LabeledMarkCollections;
			eDialog.ShowDialog();
			if (eDialog.DialogResult == DialogResult.OK)
			{
				_amLastEffectCount = eDialog.EffectCount;
				_amLastStartTime = eDialog.StartTime;
				_amLastDuration = eDialog.Duration;
				_amLastDurationBetween = eDialog.DurationBetween;
				var newEffects = new List<EffectNode>();
				if (eDialog.AlignToBeatMarks)
				{
					newEffects = AddEffectsToBeatMarks(eDialog.CheckedMarks, eDialog.EffectCount, effectId, eDialog.StartTime, eDialog.Duration, row, eDialog.FillDuration, eDialog.SkipEOBeat);
				}
				else
				{
					TimeSpan nextStartTime = eDialog.StartTime;
					for (int i = 0; i < eDialog.EffectCount; i++)
					{
						if (nextStartTime + eDialog.Duration > SequenceLength)
						{
							//if something went wrong in the forms calculations
							break;
						}						
						var newEffect = ApplicationServices.Get<IEffectModuleInstance>(effectId);
						try
						{
							newEffects.Add(CreateEffectNode(newEffect, row, nextStartTime, eDialog.Duration));
							nextStartTime = nextStartTime + eDialog.Duration + eDialog.DurationBetween;
						}
						catch (Exception ex)
						{
							string msg = "TimedSequenceEditor: <AddMultipleEffects> - error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
							             ((row == null) ? "<null>" : row.Name);
							Logging.Error(msg, ex);
						}
					}
					AddEffectNodes(newEffects);
					SequenceModified();
					var act = new EffectsAddedUndoAction(this, newEffects);
					_undoMgr.AddUndoAction(act);
				}
				if (newEffects.Count > 0)
				{
					if (eDialog.SelectEffects) SelectEffectNodes(newEffects);
				}
			}
		}
		
		private List<EffectNode> AddEffectsToBeatMarks(ListView.CheckedListViewItemCollection checkedMarks, int effectCount, Guid effectGuid, TimeSpan startTime, TimeSpan duration, Row row, Boolean fillDuration, Boolean skipEoBeat)
		{
			bool skipThisBeat = false;
			List<IMark> times = (from ListViewItem listItem in checkedMarks from mcItem in _sequence.LabeledMarkCollections where mcItem.Name == listItem.Text from mark in mcItem.Marks where mark.StartTime >= startTime select mark).ToList();
			times.Sort();
			var newEffects = new List<EffectNode>();
			if (times.Count > 0)
			{
				foreach (Mark mark in times)
				{
					if (newEffects.Count < effectCount)
					{
						if (!skipThisBeat)
						{
							var newEffect = ApplicationServices.Get<IEffectModuleInstance>(effectGuid);
							try
							{
								if (fillDuration)
								{
									if (times.IndexOf(mark) == times.Count - 1) //The dialog hanles this, but just to make sure
										break; //We're done -- There are no more marks to fill, don't create it
									duration = times[times.IndexOf(mark) + 1].StartTime - mark.StartTime;
									if (duration < TimeSpan.FromSeconds(.01)) duration = TimeSpan.FromSeconds(.01);
								}
								newEffects.Add(CreateEffectNode(newEffect, row, mark.StartTime, duration));
							}
							catch (Exception ex)
							{
								string msg = "TimedSequenceEditor: <AddEffectsToBeatMarks> - error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
											 ((row == null) ? "<null>" : row.Name);
								Logging.Error(msg, ex);
							}
						}						
						if (skipEoBeat) skipThisBeat = (!skipThisBeat);
					}
					else
						break; //We're done creating, we've matched counts
				}
				AddEffectNodes(newEffects);
				SequenceModified();
				var act = new EffectsAddedUndoAction(this, newEffects);
				_undoMgr.AddUndoAction(act);
			}
			return newEffects;
		}

		private void DistributeSelectedEffectsEqually()
		{
			if (!TimelineControl.grid.OkToUseAlignmentHelper(TimelineControl.SelectedElements))
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				var messageBox = new MessageBoxForm(TimelineControl.grid.alignmentHelperWarning, @"", false, false);
				messageBox.ShowDialog();
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
			if (totalElements == 0) return;
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
			TimeSpan effectTs = TimeSpan.FromSeconds(effectDuration);
			//var msgString = string.Format("Total Elements: {0}\n Start Time: {1}\n End Time: {2}\n Total Duration: {3}\n Effect Duration: {4}\n TimeSpan Duration: {5}\n Start at last element: {6}", totalElements,startTime,endTime,totalDuration,effectDuration, effectTS.TotalSeconds, startAtLastElement);
			//MessageBox.Show(msgString);
			//Sanity Check - Keep effects from becoming less than minimum.
			if (effectDuration < .050)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(string.Format(
						"Unable to complete request. The resulting duration would fall below 50 milliseconds.\nCalculated duration: {0}",
						effectDuration), @"Warning", false, false);
				messageBox.ShowDialog();
				return;
			}
			var elementsToDistribute = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			if (!startAtLastElement)
			{
				//Lets move the first one
				elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(0),
							new Tuple<TimeSpan, TimeSpan>(startTime, startTime + effectTs));
				for (int i = 1; i <= totalElements - 1; i++)
				{
					var thisStartTime = elementsToDistribute.Last().Value.Item2;
					elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i), new Tuple<TimeSpan, TimeSpan>(thisStartTime, thisStartTime + effectTs));
				}
			}
			else
			{
				//Lets move the first(last) one
				elementsToDistribute.Add(TimelineControl.SelectedElements.Last(), new Tuple<TimeSpan, TimeSpan>(startTime, startTime + effectTs));
				for (int i = totalElements - 2; i >= 0; i--)
				{
					var thisStartTime = elementsToDistribute.Last().Value.Item2; 
					elementsToDistribute.Add(TimelineControl.SelectedElements.ElementAt(i), new Tuple<TimeSpan, TimeSpan>(thisStartTime, thisStartTime + effectTs));
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				var messageBox = new MessageBoxForm(TimelineControl.grid.alignmentHelperWarning, @"", false, false);
				messageBox.ShowDialog();
				return;
			}

			if (TimelineControl.SelectedElements.Count() < 2)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				var messageBox = new MessageBoxForm("Select at least two effects to distribute.", "Select more effects", false, false);
				messageBox.ShowDialog();
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

			dDialog.ElementCount = elementCount.ToString(CultureInfo.InvariantCulture);
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
				foreach (Element element in e.ElementsUnderCursor)
				{
					TimedSequenceElement tse = element as TimedSequenceElement;
					if (tse == null) continue;
					string name = tse.EffectNode.Effect.Descriptor.TypeName;
					name += string.Format(" ({0:m\\:ss\\.fff})", tse.EffectNode.StartTime);
					ToolStripMenuItem item = new ToolStripMenuItem(name);
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
			var toolStripMenuItem = sender as ToolStripMenuItem;
			if (toolStripMenuItem != null)
			{
				TimedSequenceElement tse = toolStripMenuItem.Tag as TimedSequenceElement;
				if (tse != null)
					TimelineControl.SelectElement(tse);
			}
			else
			{
				Logging.Error("TimedSequenceEditor: <contextMenuStripElementSelectionItem_Click> - toolStripMenuItem is null!");
			}
		}

		private void timelineControl_RulerClicked(object sender, RulerClickedEventArgs e)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <timelineControl_RulerCLicked> - StartPointClicked with null context!");
				return;
			}

			if (e.Button == MouseButtons.Left)
			{
				bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

				if (autoPlay)
				{
					// Save the times for later restoration
					_mPrevPlaybackStart = TimelineControl.PlaybackStartTime;
					_mPrevPlaybackEnd = TimelineControl.PlaybackEndTime;
				}
				else
				{
					_mPrevPlaybackStart = e.Time;
					_mPrevPlaybackEnd = null;
				}

				// Set the timeline control
				TimelineControl.PlaybackStartTime = e.Time;
				TimelineControl.PlaybackEndTime = null;
				SequenceModified();
				if (autoPlay)
				{
					_PlaySequence(e.Time, TimeSpan.MaxValue);
				}
				else
				{
					_timeLineGlobalStateManager.CursorPosition = e.Time;
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				AddMarkAtTime(e.Time, false, e.ModifierKeys == Keys.Control || e.ModifierKeys == (Keys.Control|Keys.Shift), e.ModifierKeys == Keys.Shift || e.ModifierKeys == (Keys.Control | Keys.Shift));
			}
		}

		private void TimeLineGlobalEventManagerOnPlayRangeAction(object sender, PlayRangeEventArgs e)
		{
			PlaySequenceBetween(e.StartTime, e.EndTime);
		}


		private void PhonemeBreakdownAction(object sender, PhonemeBreakdownEventArgs e)
		{
			Dictionary<IMark, IMarkCollection> undoMarks = new Dictionary<IMark, IMarkCollection>();
			if (e.BreakdownType == BreakdownType.Phrase)
			{
				foreach (IGrouping<IMarkCollection, IMark> markGroup in e.Marks.GroupBy(m => m.Parent))
				{
					var mc = GetOrCreatePhonemeMarkCollection(markGroup.Key, MarkCollectionType.Word).First();
					
					foreach (var mark in markGroup)
					{
						string[] words = mark.Text.Trim().Split();
						if (words.Any())
						{
							var duration = TimeSpan.FromTicks(mark.Duration.Ticks / words.Length);
							var startTime = mark.StartTime;
							foreach (var word in words)
							{
								var wordMark = new Mark(startTime)
								{
									Duration = duration,
									Text = word
								};
								startTime = startTime + duration;
								mc.AddMark(wordMark);
								undoMarks.Add(wordMark, mc);
							}
						}
					}
				}
			}
			else if (e.BreakdownType == BreakdownType.Word)
			{
				foreach (IGrouping<IMarkCollection, IMark> markGroup in e.Marks.GroupBy(m => m.Parent))
				{
					var mc = GetOrCreatePhonemeMarkCollection(markGroup.Key, MarkCollectionType.Phoneme).First();

					if (LipSyncTextConvert.StandardDictExists() == false)
					{
						var messageBox = new MessageBoxForm("Unable to find Standard Phoneme Dictionary", "Error",
							MessageBoxButtons.OK, SystemIcons.Error);
						messageBox.ShowDialog();
						return;
					}

					LipSyncTextConvert.InitDictionary();
					List<IMark> marksAdded = new List<IMark>();
					foreach (var mark in markGroup)
					{
						var cleanText = LipSyncTextConvert.RemovePunctuation(mark.Text.Trim());
						List<App.LipSyncApp.PhonemeType> phonemeList = LipSyncTextConvert.TryConvert(cleanText);
						if (!phonemeList.Any())
						{
							var value = GetUserMappingForFailedWord(cleanText);
							if (value != String.Empty)
							{
								phonemeList = LipSyncTextConvert.TryConvert(cleanText);
							}
							else
							{
								continue;
							}
						}
						var duration = TimeSpan.FromTicks(mark.Duration.Ticks / phonemeList.Count);
						var startTime = mark.StartTime;
						foreach (var phonemeType in phonemeList)
						{
							var phonemeMark = new Mark(startTime)
							{
								Duration = duration,
								Text = phonemeType.ToString()
							};
							startTime = startTime + duration;
							marksAdded.Add(phonemeMark);
							undoMarks.Add(phonemeMark, mc);
						}
					}

					mc.AddMarks(marksAdded);
				}
			}

			if (undoMarks.Any())
			{
				CheckAndRenderDirtyElementsAsync();
				var act = new MarksAddedUndoAction(this, undoMarks);
				_undoMgr.AddUndoAction(act);
			}
		}

		private List<IMarkCollection> GetOrCreatePhonemeMarkCollection(IMarkCollection parent, MarkCollectionType type)
		{
			//Find the right collection
			var linkedCollections = _sequence.LabeledMarkCollections.Where(x =>
				x.LinkedMarkCollectionId == parent.Id && x.CollectionType == type);
			List<IMarkCollection> mcList = new List<IMarkCollection>();
			if (linkedCollections.Any())
			{
				mcList.AddRange(linkedCollections);
			}
			else
			{
				var name = $"{parent.Name} {type}";
				if (parent.CollectionType != MarkCollectionType.Phrase)
				{
					//try to find the phrase parent
					var phraseParent = _sequence.LabeledMarkCollections.FirstOrDefault(x => x.Id == parent.LinkedMarkCollectionId);
					if (phraseParent != null && phraseParent.CollectionType == MarkCollectionType.Phrase)
					{
						name = $"{phraseParent.Name} {type}";
					}
				}
				var mc = GetOrAddNewMarkCollection(parent.Decorator.Color, name);
				mc.LinkedMarkCollectionId = parent.Id;
				mc.CollectionType = type;
				mc.ShowMarkBar = true;
				mcList.Add(mc);
			}

			return mcList;
		}


		private IMark AddMarkAtTime(TimeSpan time, bool suppressUndo, bool fillGap=false, bool promptForName=false)
		{
			var markName = string.Empty;
			if (promptForName)
			{
				TextDialog td = new TextDialog("Enter the Mark label text.", string.Empty);
				var result = td.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					markName = td.Response;
				}
			}
			IMark newMark = null;
			IMarkCollection mc = null;
			if (_sequence.LabeledMarkCollections.Count == 0)
			{
				if (_context.IsRunning) PauseSequence();
				var messageBox = new MessageBoxForm("Marks are stored in Mark Collections. There are no mark collections available to store this mark. Would you like to create a new one?", @"Create a Mark Collection", MessageBoxButtons.YesNo, SystemIcons.Information);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					mc = GetOrAddNewMarkCollection(Color.White, "Default Marks");
				}
			}
			else
			{
				mc = _sequence.LabeledMarkCollections.FirstOrDefault(x => x.IsDefault && x.IsVisible);
				if (mc == null)
				{
					if (_context.IsRunning) PauseSequence();
					var messageBox = new MessageBoxForm("The active mark collection is not visible on the timeline. Would you like to enable it to add the mark?", @"New Mark", MessageBoxButtons.OKCancel, SystemIcons.Error);
					var result = messageBox.ShowDialog();
					if (result == DialogResult.OK)
					{
						mc = _sequence.LabeledMarkCollections.FirstOrDefault(x => x.IsDefault);
						if (mc != null)
						{
							mc.ShowGridLines = mc.ShowMarkBar = true;
						}
					}
				}
			}
			if (mc != null && mc.Marks.All(x => x.StartTime != time))
			{

				newMark = new Mark(time);
				newMark.Text = markName;
				mc.AddMark(newMark);
				if (fillGap)
				{
					mc.FillGapTimes(newMark);
				}

				SequenceModified();
				CheckAndRenderDirtyElementsAsync();
				if (!suppressUndo)
				{
					var act = new MarksAddedUndoAction(this, newMark, mc);
					_undoMgr.AddUndoAction(act);
				}
			}

			return newMark;
		}

		private IMarkCollection GetOrAddNewMarkCollection(Color color, string name = "New Collection")
		{
			IMarkCollection mc = _sequence.LabeledMarkCollections.FirstOrDefault(mCollection => mCollection.Name == name);
			if (mc == null)
			{
				MarkCollection newCollection = new MarkCollection {Name = name};
				newCollection.Decorator.Color = color;
				if (!_sequence.LabeledMarkCollections.Any())
				{
					newCollection.IsDefault = true;
				}
				_sequence.LabeledMarkCollections.Add(newCollection);
				mc = newCollection;
				SequenceModified();
			}

			return mc;
		}

		private void TimeLineGlobalTextChanged(object sender, MarksTextChangedEventArgs e)
		{
			SequenceModified();
			CheckAndRenderDirtyElementsAsync();
		}

		private void TimeLineGlobalMoving(object sender, MarksMovingEventArgs e)
		{
			UpdateGridSnapTimes();
		}

		private void TimeLineGlobalMoved(object sender, MarksMovedEventArgs e)
		{
			_undoMgr.AddUndoAction(new MarksTimeChangedUndoAction(this, e.MoveResizeInfo, e.MoveType));
			UpdateGridSnapTimes();
			CheckAndRenderDirtyElementsAsync();
			SequenceModified();
		}

		private void TimeLineGlobalDeleted(object sender, MarksDeletedEventArgs e)
		{
			var marksDeleted = new Dictionary<IMark, IMarkCollection>();
			foreach (var mark in e.Marks)
			{
				marksDeleted.Add(mark, mark.Parent);
			}
			var act = new MarksRemovedUndoAction(this, marksDeleted);
			_undoMgr.AddUndoAction(act);
			CheckAndRenderDirtyElementsAsync();
			UpdateGridSnapTimes();
			SequenceModified();
		}

		private void TimeLineGlobalEventManagerOnMarksPasted(object sender, MarksPastedEventArgs e)
		{
			var marksPasted = new Dictionary<IMark, IMarkCollection>();
			foreach (var mark in e.Marks)
			{
				marksPasted.Add(mark, mark.Parent);
			}
			var act = new MarksAddedUndoAction(this, marksPasted);
			_undoMgr.AddUndoAction(act);
			CheckAndRenderDirtyElementsAsync();
			UpdateGridSnapTimes();
			SequenceModified();
		}


		private void timelineControl_RulerBeginDragTimeRange(object sender, EventArgs e)
		{
			_mPrevPlaybackStart = TimelineControl.PlaybackStartTime;
			_mPrevPlaybackEnd = TimelineControl.PlaybackEndTime;
		}

		private void timelineControl_TimeRangeDragged(object sender, ModifierKeysEventArgs e)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <timelineControl_TimeRangeDragged> - null context!");
				return;
			}

			bool autoPlay = e.ModifierKeys.HasFlag(Keys.Control);

			if (autoPlay)
			{
				if (TimelineControl.PlaybackStartTime != null && TimelineControl.PlaybackEndTime != null)
					_PlaySequence(TimelineControl.PlaybackStartTime.Value, TimelineControl.PlaybackEndTime.Value);
				else
				{
					Logging.Error("TimedSequenceEditor: <timelineControl_TimeRangeDragged> - On autoPlay, PlaybackStartTime or PlaybackEndTime was null!");
				}
			}
			else
			{
				// We actually want to keep this range.
				_mPrevPlaybackStart = TimelineControl.PlaybackStartTime;
				_mPrevPlaybackEnd = TimelineControl.PlaybackEndTime;
			}
		}

		private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			UpdatePasteMenuStates();
		}

		private void toolStripEdit_MouseEnter(object sender, EventArgs e)
		{
			UpdatePasteMenuStates();
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
			Console.WriteLine(@"tse: state changed: " + Execution.State);
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
				Logging.Error(@"TimedSequenceEditor: <OpenSequenceContext> - null _context when attempting to play sequence!");
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to play this sequence.  See error log for details.", @"Unable to Play", false, false);
				messageBox.ShowDialog();
				return;
			}
			TimelineControl.grid.Context = _context;
			_context.SequenceStarted += context_SequenceStarted;
			_context.SequenceEnded += context_SequenceEnded;
			//_context.ProgramEnded += _context_ProgramEnded;
			_context.ContextEnded += context_ContextEnded;

			UpdateButtonStates();
		}

		private void CloseSequenceContext()
		{
			_context.SequenceStarted -= context_SequenceStarted;
			_context.SequenceEnded -= context_SequenceEnded;
			//_context.ProgramEnded -= _context_ProgramEnded;
			_context.ContextEnded -= context_ContextEnded;

			VixenSystem.Contexts.ReleaseContext(_context);
			UpdateButtonStates();
		}

		private void PlaySequence()
		{
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			//MessageBox.Show("Call to play sequence");
			if (delayOffToolStripMenuItem.Checked == false && timerPostponePlay.Enabled == false && playBackToolStripButton_Stop.Enabled == false)
			{
				//MessageBox.Show("Starting delay");
				_delayCountDown = (timerPostponePlay.Interval / 1000);
				timerPostponePlay.Enabled = timerDelayCountdown.Enabled = true;
				playBackToolStripButton_Play.Image = Resources.hourglass;
				//The Looping stuff kinda broke this, but we need to do this for consistency
				playBackToolStripButton_Play.Enabled = true;
				playToolStripMenuItem.Enabled = false;
				playBackToolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = true;
			}

			if (timerPostponePlay.Enabled)
			{
				//We are waiting for a delayed start, ignore the play button
				return;
			}

			//Make sure the blue play icon is used & dissappear the delay countdown
			playBackToolStripButton_Play.Image = Resources.control_play_blue;
			toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;

			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <PlaySequence> - attempt to Play with null _context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused)
			{
				// continue execution from previous location.
				start = TimingSource.Position;
				end = TimeSpan.MaxValue;
				UpdateButtonStates(); // context provides no notification to/from pause state.
			}
			else
			{
				start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
			}
			_PlaySequence(start, end);
		}

		/// <summary>
		/// Plays the sequence from the specified starting mouseLocation in TimeSpan format
		/// </summary>
		/// <param name="startTime"></param>
		public void PlaySequenceFrom(TimeSpan startTime)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <PlaySequenceFrom> - attempt to Play with null _context!");
				return;
			}

			TimeSpan start, end;

			if (_context.IsPaused)
			{
				// continue execution from previous location.
				start = TimingSource.Position;
				end = TimeSpan.MaxValue;
				UpdateButtonStates(); // context provides no notification to/from pause state.
			}
			else
			{
				start = startTime;
				end = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
				if (start >= end)
				{
					start = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
				}
			}
			_PlaySequence(start, end);
		}

		public void PlaySequenceBetween(TimeSpan startTime, TimeSpan endTime)
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <PlaySequenceFrom> - attempt to Play with null _context!");
				return;
			}

			if (_context.IsPaused)
			{
				StopSequence();
			}

			if (startTime < TimeSpan.Zero)
			{
				startTime = TimelineControl.PlaybackStartTime.GetValueOrDefault(TimeSpan.Zero);
			}

			if (endTime > TimelineControl.TotalTime)
			{
				endTime = TimelineControl.PlaybackEndTime.GetValueOrDefault(TimeSpan.MaxValue);
			}
			
			_PlaySequence(startTime, endTime);
		}

		private void PauseSequence()
		{
			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <PauseSequence> - attempt to Pause with null _context!");
				return;
			}

			_context.Pause();
			UpdateButtonStates(); // context provides no notification to/from pause state.
		}

		private void StopSequence()
		{
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			if (delayOffToolStripMenuItem.Checked != true)
			{
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
				toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", timerPostponePlay.Interval / 1000);
			}

			if (timerPostponePlay.Enabled)
			{
				timerPostponePlay.Enabled = timerDelayCountdown.Enabled = false;
				playBackToolStripButton_Play.Image = Resources.control_play_blue;
				playBackToolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
				playBackToolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
				//We are stopping the delay, there is no context, so get out of here to avoid false entry into error log
				return;
			}

			if (_context == null)
			{
				Logging.Error("TimedSequenceEditor: <StopSequence> - attempt to Stop with null _context!");
				return;
			}

			_context.Stop();
			// button states updated by event handler.
		}

		protected void context_SequenceStarted(object sender, SequenceStartedEventArgs e)
		{
			timerPlaying.Start();
			TimingSource = e.TimingSource;
			UpdateButtonStates();
		}

		protected void context_SequenceEnded(object sender, SequenceEventArgs e)
		{
			//This is for the delayed play options
			if (delayOffToolStripMenuItem.Checked == false)
			{
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
				toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", timerPostponePlay.Interval / 1000);
			}
			timerPlaying.Stop();
			TimingSource = null;
		}

		protected void context_ContextEnded(object sender, EventArgs e)
		{
			UpdateButtonStates();
			TimelineControl.PlaybackStartTime = _mPrevPlaybackStart;
			TimelineControl.PlaybackEndTime = _mPrevPlaybackEnd;
			TimelineControl.PlaybackCurrentTime = null;
			EffectEditorForm.ResumePreview();
			SetPreviewsTopMost(false);
		}

		protected void timerPlaying_Tick(object sender, EventArgs e)
		{
			if (TimingSource != null)
			{
				TimelineControl.PlaybackCurrentTime = TimingSource.Position;
			}
		}

		private void timelineControl_PlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			toolStripStatusLabel_currentTime.Text = TimelineControl.PlaybackCurrentTime.HasValue ? TimelineControl.PlaybackCurrentTime.Value.ToString("m\\:ss\\.fff") : String.Empty;
		}

		private void CursorMovedHandler(object sender, EventArgs e)
		{
			var timeSpanEventArgs = e as TimeSpanEventArgs;
			if (timeSpanEventArgs != null)
				toolStripStatusLabel_currentTime.Text = timeSpanEventArgs.Time.ToString("m\\:ss\\.fff");
			else
			{
				Logging.Error("TimedSequenceEditor: <CursorMovedHandler> - timeSpanEventArgs = null!");
			}
		}

		private void UpdatePasteMenuStates()
		{
			editToolStripButton_Paste.Enabled = toolStripMenuItem_Paste.Enabled = GetClipboardCount() > 0;
			editToolStripButton_PasteVisibleMarks.Visible = toolStripMenuItem_PasteToMarks.Enabled = GetMarksPresent() && GetClipboardCount() > 0;
			editToolStripButton_PasteInvert.Visible = toolStripMenuItem_PasteInvert.Enabled = GetClipboardCount() > 1;
			editToolStripButton_PasteDropDown.Enabled = toolStripMenuItem_PasteSpecial.Enabled =
			toolStripMenuItem_PasteToMarks.Enabled || toolStripMenuItem_PasteInvert.Enabled;
		}

		private bool ClipboardHasData()
		{
			IDataObject dataObject = Clipboard.GetDataObject();
			return dataObject != null && dataObject.GetDataPresent(ClipboardFormatName.Name);
		}

		private int GetClipboardCount()
		{
			// Gets number of Effects on the clipboard, used to determine which paste options will be enabled.
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(ClipboardFormatName.Name))
			{
				if (dataObject.GetData(ClipboardFormatName.Name) is TimelineElementsClipboardData data)
					return data.EffectModelCandidates.Count;
			}
			return 0;
		}

		private bool GetMarksPresent()
		{
			// Checks if there are any visible marks that are past the mouse click position.
			bool visibleMarks = false;
			TimeSpan pasteTime = _timeLineGlobalStateManager.CursorPosition;
			foreach (var mc in _sequence.LabeledMarkCollections)
			{
				// Only continue processing visible Mark collections while no mark is found after the mouse click position.
				if (!mc.IsVisible || visibleMarks) continue;
				foreach (IMark mark in mc.Marks)
				{
					if (pasteTime > mark.StartTime) continue;
					visibleMarks = true;
					break; // We only need at least one mark past the mouse pointer to continue looping so break to save time.
				}
			}
			return visibleMarks;
		}

		private void UpdateButtonStates()
		{
			if (InvokeRequired)
				Invoke(new Delegates.GenericDelegate(UpdateButtonStates));
			else
			{
				if (_context == null)
				{
					playBackToolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
					playBackToolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					playBackToolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
					return;
				}

				if (_context.IsRunning)
				{
					if (_context.IsPaused)
					{
						playBackToolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
						playBackToolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					}
					else
					{
						playBackToolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = false;
						playBackToolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = true;
					}
					playBackToolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = true;
				}
				else // Stopped
				{
					playBackToolStripButton_Play.Enabled = playToolStripMenuItem.Enabled = true;
					playBackToolStripButton_Pause.Enabled = pauseToolStripMenuItem.Enabled = false;
					playBackToolStripButton_Stop.Enabled = stopToolStripMenuItem.Enabled = false;
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
					if (element.Row != null)
					{
						var effectNode = CreateEffectNode(newEffect, element.Row, element.StartTime, element.Duration);
						LayerManager.AssignEffectNodeToLayer(effectNode, LayerManager.GetLayer(element.EffectNode));
						// put it in the sequence and in the timeline display
						newElements.Add(AddEffectNode(effectNode));
					}
					else
					{
						Logging.Error("TimedSequenceEditor: <CloneElements> - Skipping element; element.Row is null!");
						
					}

				} catch (Exception ex)
				{
					string msg = "TimedSequenceEditor CloneElements: error adding effect of type " + newEffect.Descriptor.TypeId + " to row " +
								 ((element.Row == null) ? "<null>" : element.Row.Name);
					Logging.Error(msg, ex);
				}
			}

			SequenceModified();

			//Add elements as a group to undo
			var act = new EffectsAddedUndoAction(this, newElements.Select(x => x.EffectNode).ToArray());
			_undoMgr.AddUndoAction(act);

			return newElements;
		}

		/// <summary>
		/// Adds an EffectNode to the sequence and the TimelineControl and puts it in the given layer.
		/// </summary>
		/// <param name="node"></param>
		/// <returns>The TimedSequenceElement created and added to the TimelineControl.</returns>
		public TimedSequenceElement AddEffectNode(EffectNode node, ILayer layer)
		{
			//Debug.WriteLine("{0}   AddEffectNode({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, node.Effect.InstanceId);
			_sequence.InsertData(node);
			//return addElementForEffectNode(node);
			var element =  AddElementForEffectNodeTpl(node);
			Sequence.GetSequenceLayerManager().AssignEffectNodeToLayer(node, layer);
			return element;
		}

		/// <summary>
		/// Adds a Mark to a Mark Collection.
		/// </summary>
		/// <param name="markCollections"></param>
		public void AddMark(Dictionary<IMark, IMarkCollection> markCollections)
		{
			foreach (var mark in markCollections)
			{
				mark.Value.AddMark(mark.Key);
			}

			CheckAndRenderDirtyElementsAsync();
			SequenceModified();
		}

		/// <summary>
		/// Remove a Mark to a Mark Collection.
		/// </summary>
		/// <param name="markCollections"></param>
		public void RemoveMark(Dictionary<IMark, IMarkCollection> markCollections)
		{
			foreach (var mark in markCollections)
			{
				mark.Value.RemoveMark(mark.Key);
			}

			CheckAndRenderDirtyElementsAsync();
			SequenceModified();
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
			return AddElementForEffectNodeTpl(node);
		}

		/// <summary>
		/// Adds multiple EffectNodes to the sequence and the TimelineControl.
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns>A List of the TimedSequenceElements created and added to the TimelineControl.</returns>
		private List<TimedSequenceElement> AddEffectNodes(IEnumerable<EffectNode> nodes)
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
			TimelineControl.grid.SelectElements(nodes.Select(x => _effectNodeToElement[x]));
		}
		
		/// <summary>
		/// Removes the Effect Node and Element
		/// </summary>
		/// <param name="node"></param>
		public void RemoveEffectNodeAndElement(EffectNode node)
		{
			//Debug.WriteLine("{0}   RemoveEffectNodeAndElement(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, node.Effect.InstanceId);

			// Lookup this effect node's Timeline Element
			if (_effectNodeToElement.ContainsKey(node))
			{
				TimedSequenceElement tse = (TimedSequenceElement) _effectNodeToElement[node];
				TimelineControl.DeselectElement(tse);
				foreach (Row row in TimelineControl) // Remove the element from all rows
					row.RemoveElement(tse);

				// TODO: Unnecessary?
				tse.ContentChanged -= ElementContentChangedHandler; // Unregister event handlers
				tse.TimeChanged -= ElementTimeChangedHandler;
				
				_effectNodeToElement.Remove(node); // Remove the effect node from the map
				_sequence.RemoveData(node); // Remove the effect node from sequence
				Sequence.GetSequenceLayerManager().RemoveEffectNodeFromLayers(node);
				_removedNodes.Add(node); //Store this away so we can clean it up later
			}
			else
			{
				Logging.Error("Missing node on remove attempt in RemoveEffectNodeAndElement.");
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Node to remove not found, the editor is in a bad state! Please close the editor and restart it.", "Error", false, false);
				messageBox.ShowDialog();
			}
		}


		/// <summary>
		/// Creates a new effect instance, and adds it to the sequence and TimelineControl.
		/// </summary>
		/// <param name="effectId">The GUID of the effect module to instantiate</param>
		/// <param name="row">The Common.Controls.Timeline.Row to add the effect to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		/// <param name="select">Optional indicator to set as the sole selection in the timeline</param>
		private EffectNode AddNewEffectById(Guid effectId, Row row, TimeSpan startTime, TimeSpan timeSpan, bool select=false)
		{
			//Debug.WriteLine("{0}   addNewEffectById({1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectId);
			// get a new instance of this effect, populate it, and make a node for it

			IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(effectId);
			return AddEffectInstance(effect, row, startTime, timeSpan, select);
		}

		/// <summary>
		/// Wraps an effect instance in an EffectNode, adds it to the sequence, and an associated element to the timeline control.
		/// Adds a Undo record for the add as well.
		/// </summary>
		/// <param name="effectInstance">Effect instance</param>
		/// <param name="row">Common.Controls.Timeline.Row to add the effect instance to</param>
		/// <param name="startTime">The start time of the effect</param>
		/// <param name="timeSpan">The duration of the effect</param>
		/// <param name="select">Optional indicator to set as the sole selection in the timeline</param>
		private EffectNode AddEffectInstance(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime, TimeSpan timeSpan, bool select = false)
		{
			EffectNode effectNode = null;
			try
			{
				//Debug.WriteLine("{0}   addEffectInstance(InstanceId={1})", (int)DateTime.Now.TimeOfDay.TotalMilliseconds, effectInstance.InstanceId);

				if ((startTime + timeSpan) > SequenceLength)
				{
					timeSpan = SequenceLength - startTime;
				}

				effectNode = CreateEffectNode(effectInstance, row, startTime, timeSpan);

				// put it in the sequence and in the timeline display
				Element element = AddEffectNode(effectNode);
				if (select)
				{
					TimelineControl.grid.ClearSelectedElements();
					TimelineControl.SelectElement(element);
				}
				SequenceModified();

				var act = new EffectsAddedUndoAction(this, new[] { effectNode });
				_undoMgr.AddUndoAction(act);

			}
			catch (Exception ex)
			{
				string msg = "TimedSequenceEditor: error adding effect of type " + effectInstance.Descriptor.TypeId + " to row " +
							 ((row == null) ? "<null>" : row.Name);
				Logging.Error(msg, ex);
			}

			return effectNode;
		}

		private EffectNode CreateEffectNode(IEffectModuleInstance effectInstance, Row row, TimeSpan startTime,
			TimeSpan timeSpan, object[] parameterValues = null)
		{
			// get the target element
			var targetNode = (ElementNode) row.Tag;

			// populate the given effect instance with the appropriate target node and times, and wrap it in an effectNode
			effectInstance.TargetNodes = new[] {targetNode};
			effectInstance.TimeSpan = timeSpan;
			effectInstance.StartTime = startTime;
			if (parameterValues != null) effectInstance.ParameterValues = parameterValues;
			if (effectInstance.SupportsMedia)
			{
				effectInstance.Media = Sequence.SequenceData.Media;
			}

			if (effectInstance.SupportsMarks)
			{
				effectInstance.MarkCollections = _sequence.LabeledMarkCollections;
			}
			return new EffectNode(effectInstance, startTime);
	
		}


		/// <summary>
		/// Populates the TimelineControl grid with a new TimedSequenceElement for each of the given EffectNodes in the list.
		/// Uses bulk loading feature of Row
		/// Will add a single TimedSequenceElement to in each row that each targeted element of
		/// the EffectNode references. It will also add callbacks to event handlers for the element.
		/// </summary>
		/// <param name="nodes">The EffectNode to make element(s) in the grid for.</param>
		/// <param name="assignMediaAndMarks">Option to assign media and marks to the effect nodes or not</param>
		private void AddElementsForEffectNodes(IEnumerable<IDataNode> nodes, bool assignMediaAndMarks = true)
		{
			Dictionary<Row, List<Element>> rowMap =
			_elementNodeToRows.SelectMany(x => x.Value).ToDictionary(x => x, x => new List<Element>());
			List<EffectNode> nodesToRemove = null;
			foreach (EffectNode node in nodes)
			{
				if (node.StartTime > _sequence.Length)
				{
					Logging.Warn("Effect start time {0} is beyond the sequence end time {1}. Dropping the effect.", node.StartTime, _sequence.Length);
					if (nodesToRemove == null)
					{
						nodesToRemove = new List<EffectNode>();
					}
					nodesToRemove.Add(node);
					continue;
				}
				if (node.EndTime > _sequence.Length)
				{
					Logging.Warn("Effect end time {0} is beyond the sequence end time {1}. Adjusting the effect length to fit.", node.StartTime, _sequence.Length);
					if (node.Effect != null)
					{
						node.Effect.TimeSpan = _sequence.Length - node.StartTime;
					}
				}

				if (assignMediaAndMarks)
				{
					if (node.Effect.SupportsMedia)
					{
						node.Effect.Media = Sequence.SequenceData.Media;
					}

					if (node.Effect.SupportsMarks)
					{
						node.Effect.MarkCollections = _sequence.LabeledMarkCollections;
					}
				}
				
				TimedSequenceElement element = SetupNewElementFromNode(node);
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
						const string message = "TimedSequenceEditor: <AddElementsForEffectNodes> - No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
						Logging.Error(message);
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm(message, @"", false, false);
						messageBox.ShowDialog();
					}
				}
			}

			if (nodesToRemove != null)
			{
				foreach (var effectNode in nodesToRemove)
				{
					_sequence.RemoveData(effectNode);
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
		private TimedSequenceElement AddElementForEffectNodeTpl(EffectNode node)
		{
			TimedSequenceElement element = SetupNewElementFromNode(node);

			// for the effect, make a single element and add it to every row that represents its target elements
			node.Effect.TargetNodes.AsParallel().WithCancellation(_cancellationTokenSource.Token)
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
									const string message = "TimedSequenceEditor: <AddElementForEffectNodeTpl> - No Timeline.Row is associated with a target ElementNode for this EffectNode. It now exists in the sequence, but not in the GUI.";
									Logging.Error(message);
									//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
									MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
									var messageBox = new MessageBoxForm(message, @"", false, false);
									messageBox.ShowDialog();
								}
							});
			TimelineControl.grid.RenderElement(element);
			return element;
		}

		private TimedSequenceElement SetupNewElementFromNode(EffectNode node)
		{
			TimedSequenceElement element = new TimedSequenceElement(node);
			element.ContentChanged += ElementContentChangedHandler;
			element.TimeChanged += ElementTimeChangedHandler;
			return element;
		}

		/// <summary>
		/// Checks all elements and if they are dirty they are placed in the render queue
		/// </summary>
		private async void CheckAndRenderDirtyElementsAsync()
		{
			Task t = Task.Factory.StartNew(() =>
			{
				var elements = TimelineControl.Rows.SelectMany(row => row).Distinct();

				elements.AsParallel().WithCancellation(_cancellationTokenSource.Token).ForAll(element =>
				{
					if (element.EffectNode.Effect.IsDirty)
					{
						TimelineControl.grid.RenderElement(element);
					}
				});
			});

			await t;
		}

		private async void RenderLipSyncElementsAsync()
		{
			//This is not ideal having the editor look for specific type of effect. Need to find a better way for 
			//the Lipsync effects to know if their mapping changed and mark themselves dirty.
			Task t = Task.Factory.StartNew(() =>
			{
				var elements = TimelineControl.Rows.SelectMany(row => row).Distinct();

				elements.AsParallel().WithCancellation(_cancellationTokenSource.Token).ForAll(element =>
				{
					if (element.EffectNode.Effect is LipSync)
					{
						element.EffectNode.Effect.MarkDirty();
						TimelineControl.grid.RenderElement(element);
					}
				});
			});

			await t;
		}

		/// <summary>
		/// Checks all elements and if they support audio is updates the media property and puts them in the render queue
		/// </summary>
		private void UpdateMediaOnSupportedEffects()
		{
			var elements = TimelineControl.Rows.SelectMany(row => row).Distinct();

			elements.AsParallel().WithCancellation(_cancellationTokenSource.Token).ForAll(element =>
			{
				if (element.EffectNode.Effect.SupportsMedia)
				{
				element.EffectNode.Effect.Media = Sequence.SequenceData.Media;
					TimelineControl.grid.RenderElement(element);
				}
			});
		}

		private void RemoveSelectedElements()
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
			TimelineControl.grid.ClearSelectedElements();
			SequenceModified();
		}


		private int _doEventsCounter;

		/// <summary>
		/// Adds a single given element node as a row in the timeline control. Recursively adds all
		/// child nodes of the given node as children, if needed.
		/// </summary>
		/// <param name="node">The node to generate a row for.</param>
		/// <param name="parentRow">The parent node the row should belong to, if any.</param>
		private void AddNodeAsRow(ElementNode node, Row parentRow)
		{
			// made the new row from the given node and add it to the control.
			TimedSequenceRowLabel label = new TimedSequenceRowLabel {Name = node.Name};
			
			Row newRow = TimelineControl.AddRow(label, parentRow, TimelineControl.rowHeight);
			if (parentRow == null)
			{
				newRow.Visible = true;
			}
			//newRow.ElementRemoved += ElementRemovedFromRowHandler;
			//newRow.ElementAdded += ElementAddedToRowHandler;

			// Tag it with the node it refers to, and take note of which row the given element node will refer to.
			newRow.Tag = node;
			if (_elementNodeToRows.ContainsKey(node))
				_elementNodeToRows[node].Add(newRow);
			else
				_elementNodeToRows[node] = new List<Row> { newRow };

			// This slows the load down just a little, but it
			// allows the update of the load timer on the bottom of the 
			// screen so Vixen doesn't appear to be locked up for very large sequences
			if (_doEventsCounter % 600 == 0)
				Application.DoEvents();
			_doEventsCounter++;

			// iterate through all if its children, adding them as needed
			foreach (ElementNode child in node.Children)
			{
				AddNodeAsRow(child, newRow);

			}
		}

		#endregion

		#region Effect & Preset Library Drag/Drop

		private void EffectDropped(Guid effectGuid, TimeSpan startTime, Row row)
		{
			//Modified 12-3-2014 to allow Control-Drop of effects to replace selected effects
			
			//TimeSpan startTime = Util.Min(TimelineControl.PixelsToTime(location.X), (_sequence.Length - duration)); // Ensure the element is inside the grid.

			if (ModifierKeys.HasFlag(Keys.Control) && TimelineControl.SelectedElements.Any())
			{

				var message = string.Format("This action will replace {0} effects, are you sure ?",
					TimelineControl.SelectedElements.Count());
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, @"Replace existing effects?", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No)
				{
					return;
				}

				var newEffects = (from elem in TimelineControl.SelectedElements let newEffectInstance = ApplicationServices.Get<IEffectModuleInstance>(effectGuid) select CreateEffectNode(newEffectInstance, elem.Row, elem.StartTime, elem.Duration)).ToList();

				RemoveSelectedElements();
				AddEffectNodes(newEffects);
				SelectEffectNodes(newEffects);

				//Add the undo action for the newly created effects
				var act = new EffectsAddedUndoAction(this, newEffects);
				_undoMgr.AddUndoAction(act);
			}
			else
			{
				AddNewEffectById(effectGuid, row, startTime, GetDefaultEffectDuration(startTime), true);
			}
		}

		private void UpdateEffectProperty(PropertyDescriptor descriptor, Element element, Object value)
		{
			descriptor.SetValue(element.EffectNode.Effect, value);
			element.UpdateNotifyContentChanged();
			SequenceModified();
		}

		private bool ShowMultipleEffectDropMessage(string name)
		{
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Multiple type effects selected, this will only apply to effects of the type: " +
									name, @"Multiple Type Effects", false, true);
			messageBox.ShowDialog();
			if (messageBox.DialogResult == DialogResult.Cancel)
			{
				return false;
			}

			return true;
		}

		private FormParameterPicker CreateParameterPicker(List<EffectParameterPickerControl> parameterPickerControls)
		{
			FormParameterPicker parameterPicker = new FormParameterPicker(parameterPickerControls)
			{
				StartPosition = FormStartPosition.Manual,
				Top = _mouseOriginalPoint.Y
			};
			parameterPicker.Left = ((_mouseOriginalPoint.X + parameterPicker.Width) < Screen.FromControl(this).Bounds.Width)
				? _mouseOriginalPoint.X
				: _mouseOriginalPoint.X - parameterPicker.Width;
			return parameterPicker;
		}

		private List<EffectParameterPickerControl> CreateGradientListPickerControls(PropertyData property, List<ColorGradient> gradients)
		{
			var parameterPickerControls = gradients.Select((t, i) => new EffectParameterPickerControl
			{
				Index = i,
				PropertyInfo = property.Descriptor,
				ParameterImage = GetColorGradientBitmap(t),
				DisplayName = string.Format("{0} {1}", property.DisplayName, i + 1)
			}).ToList();
			return parameterPickerControls;
		}


		private List<EffectParameterPickerControl> CreateGradientLevelPairPickerControls(PropertyData property, List<GradientLevelPair> gradientLevelPairs, bool gradient=true)
		{
			var parameterPickerControls = gradientLevelPairs.Select((t, i) => new EffectParameterPickerControl
			{
				Index = i,
				PropertyInfo = property.Descriptor,
				ParameterImage = gradient?GetColorGradientBitmap(t.ColorGradient):GetCurveBitmap(t.Curve),
				DisplayName = string.Format("{0} {1}", property.DisplayName, i + 1)
			}).ToList();
			return parameterPickerControls;
		}

		private IEnumerable<Element> GetElementsForDrop(Element element)
		{
			var elements = element.Selected
				? TimelineControl.SelectedElements.Where(x => x.EffectNode.Effect.GetType() == element.EffectNode.Effect.GetType())
				: new[] { element };
			return elements;
		}

		private bool ValidateMultipleEffects(Element element, IEnumerable<Element> elements)
		{
			if (TimelineControl.SelectedElements.Count() > 1 && elements.Count() != TimelineControl.SelectedElements.Count())
			{
				if (!ShowMultipleEffectDropMessage(element.EffectNode.Effect.EffectName))
				{
					return true;
				}
			}
			return false;
		}

		private void ShowMultiDropMessage()
		{
			UpdateToolStrip4("Choose the property to set, press Escape to cancel.", 8);
		}

		private void CompleteDrop(Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, Element element, string type)
		{
			if (elementValues.Any())
			{
				var undo = new EffectsPropertyModifiedUndoAction(elementValues);
				AddEffectsModifiedToUndo(undo);
				TimelineControl.grid.ClearSelectedElements();
				TimelineControl.SelectElement(element);
				UpdateToolStrip4(
					string.Format("{2} applied to {0} {1} effect(s).", elementValues.Count(), element.EffectNode.Effect.EffectName, type), 30);
			}
		}


		#region Preset Library Color Drop

		private void HandleColorDrop(Element element, Color color)
		{
			_mouseOriginalPoint = new Point(MousePosition.X, MousePosition.Y);

			var elements = GetElementsForDrop(element);

			if (ValidateMultipleEffects(element, elements)) return;	

			HandleColorDropOnElements(elements, color);
		}

		private void HandleColorDropOnElements(IEnumerable<Element> elements, Color color)
		{
			if (elements == null || !elements.Any()) return;

			var element = elements.First();

			var properties = MetadataRepository.GetProperties(element.EffectNode.Effect).Where(x => (x.PropertyType == typeof(Color)) && x.IsBrowsable);

			if (!properties.Any())
			{
				//if we are dropping on effects that do not have color just convert to gradient and use that drop method
				HandleGradientDropOnElements(elements, new ColorGradient(color));
				return;
			}

			properties = MetadataRepository.GetProperties(element.EffectNode.Effect).Where(x => (x.PropertyType == typeof(Color) ||
				x.PropertyType == typeof(ColorGradient) || x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);

			Dictionary<Element, Tuple<Object, PropertyDescriptor>> elementValues = new Dictionary<Element, Tuple<object, PropertyDescriptor>>();

			if (!properties.Any()) return;
			if (properties.Count() == 1)
			{
				var property = properties.First();
				if (property.PropertyType == typeof (Color))
				{
					foreach (var e in elements)
					{
						HandleColorDropOnColor(color, elementValues, e, property.Descriptor);
					}
				}
			}
			else
			{
				//We have more than one color type property
				List<EffectParameterPickerControl> parameterPickerControls = properties.Where(p => p.PropertyType == typeof(Color) || p.PropertyType == typeof(ColorGradient)).Select(propertyData => new EffectParameterPickerControl
				{
					PropertyInfo = propertyData.Descriptor, 
					ParameterImage = propertyData.PropertyType == typeof (Color) ? GetColorBitmap((Color) propertyData.Descriptor.GetValue(element.EffectNode.Effect)) : GetColorGradientBitmap((ColorGradient) propertyData.Descriptor.GetValue(element.EffectNode.Effect))
				}).ToList();

				var gradientList = properties.Where(p => p.PropertyType == typeof (List<ColorGradient>));
				foreach (var propertyData in gradientList)
				{
					List<ColorGradient> gradients = propertyData.Descriptor.GetValue(element.EffectNode.Effect) as List<ColorGradient>;
					if (gradients == null) return;

					parameterPickerControls.AddRange(CreateGradientListPickerControls(propertyData, gradients));
				}

				var gradientLevelList = properties.Where(p => p.PropertyType == typeof(List<GradientLevelPair>));
				foreach (var propertyData in gradientLevelList)
				{
					List<GradientLevelPair> gradients = propertyData.Descriptor.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
					if (gradients == null) return;

					parameterPickerControls.AddRange(CreateGradientLevelPairPickerControls(propertyData, gradients));
				}
				
				FormParameterPicker parameterPicker = CreateParameterPicker(parameterPickerControls);

				ShowMultiDropMessage();
				var dr = parameterPicker.ShowDialog();
				if (dr == DialogResult.OK)
				{
					
					foreach (var e in elements)
					{
						if (parameterPicker.PropertyInfo.PropertyType == typeof (Color))
						{
							HandleColorDropOnColor(color, elementValues, e, parameterPicker.PropertyInfo);
						}
						else if (parameterPicker.PropertyInfo.PropertyType == typeof(ColorGradient))
						{
							HandleGradientDropOnGradient(new ColorGradient(color), elementValues, e, parameterPicker.PropertyInfo);
						}
						else if (parameterPicker.PropertyInfo.PropertyType == typeof(List<ColorGradient>))
						{
							List<ColorGradient> gradients = parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect) as List<ColorGradient>;
							var newGradients = gradients.ToList();
							newGradients[parameterPicker.SelectedControl.Index] = new ColorGradient(color);
							elementValues.Add(element, new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect), parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);		
						}
						else if (parameterPicker.PropertyInfo.PropertyType == typeof(List<GradientLevelPair>))
						{
							List<GradientLevelPair> gradients = parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
							var newGradients = gradients.ToList();
							newGradients[parameterPicker.SelectedControl.Index] = new GradientLevelPair(new ColorGradient(color), gradients[parameterPicker.SelectedControl.Index].Curve);
							elementValues.Add(element, new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect), parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);
						}
					}
				}
				else
				{
					UpdateToolStrip4(String.Empty);
				}

			}
			CompleteDrop(elementValues, element, "Color");
			
		}

		private void HandleColorDropOnColor(Color color, Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, Element e, PropertyDescriptor property)
		{
			elementValues.Add(e,
				new Tuple<object, PropertyDescriptor>(property.GetValue(e.EffectNode.Effect), property));
			UpdateEffectProperty(property, e, color);
		}

		#endregion Preset Library Color Drop

		#region Preset Library Curve Drop
		
		private void HandleCurveDrop(Element element, Curve curve)
		{
			_mouseOriginalPoint = new Point(MousePosition.X, MousePosition.Y);

			var elements = GetElementsForDrop(element);

			if (ValidateMultipleEffects(element, elements)) return;	

			Dictionary<Element, Tuple<Object, PropertyDescriptor>> elementValues = new Dictionary<Element, Tuple<object, PropertyDescriptor>>();

			var properties = MetadataRepository.GetProperties(element.EffectNode.Effect).Where(x => (x.PropertyType == typeof(Curve) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);
			if (!properties.Any()) return;
			
			if (properties.Count() == 1)
			{
				var property = properties.First();

				if (property.PropertyType == typeof (Curve))
				{
					foreach (var e in elements)
					{
						elementValues.Add(e, new Tuple<object, PropertyDescriptor>(property.Descriptor.GetValue(e.EffectNode.Effect), property.Descriptor));
						UpdateEffectProperty(property.Descriptor, e, curve);
					}
				}
				else if (property.PropertyType == typeof(List<GradientLevelPair>))
				{
					foreach (var e in elements)
					{
						HandleCurveDropOnGradientLevelPairList(property, e, elementValues, curve);
					}
				}

			}
			else
			{
				//We have more than one property of the same type
				List<EffectParameterPickerControl> parameterPickerControls = properties.Where(p => p.PropertyType == typeof(Curve)).Select(propertyData => new EffectParameterPickerControl
				{
					PropertyInfo = propertyData.Descriptor,
					ParameterImage = GetCurveBitmap((Curve)propertyData.Descriptor.GetValue(element.EffectNode.Effect))
				}).ToList();

				var gradientLevelList = properties.Where(p => p.PropertyType == typeof(List<GradientLevelPair>));
				foreach (var propertyData in gradientLevelList)
				{
					List<GradientLevelPair> gradients = propertyData.Descriptor.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
					if (gradients == null) return;

					parameterPickerControls.AddRange(CreateGradientLevelPairPickerControls(propertyData, gradients, false));
				}

				FormParameterPicker parameterPicker = CreateParameterPicker(parameterPickerControls);

				ShowMultiDropMessage();
				var dr = parameterPicker.ShowDialog();
				if (dr == DialogResult.OK)
				{
					if (parameterPicker.PropertyInfo.PropertyType == typeof (Curve))
					{
						foreach (var e in elements)
						{
							elementValues.Add(e,
								new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect),
									parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, e, curve);
						}
					}
					else if (parameterPicker.PropertyInfo.PropertyType == typeof(List<GradientLevelPair>))
					{
						List<GradientLevelPair> gradientLevelPairs = parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
						if (gradientLevelPairs != null)
						{
							var newGradientLevelPairs = gradientLevelPairs.ToList();
							newGradientLevelPairs[parameterPicker.SelectedControl.Index] =
								new GradientLevelPair(gradientLevelPairs[parameterPicker.SelectedControl.Index].ColorGradient, curve);
							elementValues.Add(element,
								new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect),
									parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradientLevelPairs);
						}
					}
					
				}
				else
				{
					UpdateToolStrip4(String.Empty);
				}

			}
			CompleteDrop(elementValues, element, "Curve");
			
		}

		private void HandleCurveDropOnGradientLevelPairList(PropertyData property, Element element, Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, Curve curve)
		{
			List<GradientLevelPair> gradientLevelPairs = property.Descriptor.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
			if (gradientLevelPairs == null) return;

			if (gradientLevelPairs.Count == 1)
			{
				var newGradientLevelPairs = gradientLevelPairs.ToList();
				newGradientLevelPairs[0] = new GradientLevelPair(gradientLevelPairs[0].ColorGradient, curve);
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(property.Descriptor.GetValue(element.EffectNode.Effect), property.Descriptor));
				UpdateEffectProperty(property.Descriptor, element, newGradientLevelPairs);
				return;
			}

			var parameterPickerControls = CreateGradientLevelPairPickerControls(property, gradientLevelPairs, false);

			var parameterPicker = CreateParameterPicker(parameterPickerControls);

			ShowMultiDropMessage();
			var dr = parameterPicker.ShowDialog();
			if (dr == DialogResult.OK)
			{
				var newGradientLevelPairs = gradientLevelPairs.ToList();
				newGradientLevelPairs[parameterPicker.SelectedControl.Index] = new GradientLevelPair(gradientLevelPairs[parameterPicker.SelectedControl.Index].ColorGradient, curve);
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect), parameterPicker.PropertyInfo));
				UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradientLevelPairs);
			}
		}


		#endregion Preset Library Curve Drop

		#region Preset Library Color Gradient Drop

		private void HandleGradientDrop(Element element, ColorGradient color)
		{
			_mouseOriginalPoint = new Point(MousePosition.X, MousePosition.Y);

			var elements = GetElementsForDrop(element);

			if (ValidateMultipleEffects(element, elements)) return;	

			HandleGradientDropOnElements(elements, color);
		}

		private void HandleGradientDropOnElements(IEnumerable<Element> elements, ColorGradient gradient)
		{
			if (elements == null || !elements.Any()) return;
			var element = elements.First();

			Dictionary<Element, Tuple<Object, PropertyDescriptor>> elementValues = new Dictionary<Element, Tuple<object, PropertyDescriptor>>();

			var properties = MetadataRepository.GetProperties(element.EffectNode.Effect).Where(x => (x.PropertyType == typeof(ColorGradient) ||
				x.PropertyType == typeof(List<ColorGradient>) || x.PropertyType == typeof(List<GradientLevelPair>)) && x.IsBrowsable);
			if (!properties.Any()) return;
			if (properties.Count() == 1)
			{
				var property = properties.First();
				if (property.PropertyType == typeof (ColorGradient))
				{
					foreach (var e in elements)
					{
						HandleGradientDropOnGradient(gradient, elementValues, e, property.Descriptor);
					}
				}
				else if (property.PropertyType == typeof(List<ColorGradient>))
				{
					foreach (var e in elements)
					{
						HandleGradientDropOnColorGradientList(property, e, elementValues, gradient);
					}
				}
				else if (property.PropertyType == typeof(List<GradientLevelPair>))
				{
					foreach (var e in elements)
					{
						HandleGradientDropOnGradientLevelPairList(property, e, elementValues, gradient);
					}
				}
			}
			else
			{
				//We have more than one color type property
				List<EffectParameterPickerControl> parameterPickerControls = properties.Where(p => p.PropertyType == typeof(ColorGradient)).Select(propertyData => new EffectParameterPickerControl
				{
					PropertyInfo = propertyData.Descriptor, 
					ParameterImage = GetColorGradientBitmap((ColorGradient) propertyData.Descriptor.GetValue(element.EffectNode.Effect))
				}).ToList();

				var gradientList = properties.Where(p => p.PropertyType == typeof (List<ColorGradient>));
				foreach (var propertyData in gradientList)
				{
					List<ColorGradient> gradients = propertyData.Descriptor.GetValue(element.EffectNode.Effect) as List<ColorGradient>;
					if (gradients == null) return;

					parameterPickerControls.AddRange(CreateGradientListPickerControls(propertyData, gradients));
				}

				var gradientLevelList = properties.Where(p => p.PropertyType == typeof(List<GradientLevelPair>));
				foreach (var propertyData in gradientLevelList)
				{
					List<GradientLevelPair> gradients = propertyData.Descriptor.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
					if (gradients == null) return;

					parameterPickerControls.AddRange(CreateGradientLevelPairPickerControls(propertyData, gradients));
				}
				
				FormParameterPicker parameterPicker = CreateParameterPicker(parameterPickerControls);

				ShowMultiDropMessage();
				var dr = parameterPicker.ShowDialog();
				if (dr == DialogResult.OK)
				{

					foreach (var e in elements)
					{

						if (parameterPicker.PropertyInfo.PropertyType == typeof (ColorGradient))
						{
							HandleGradientDropOnGradient(gradient, elementValues, e, parameterPicker.PropertyInfo);
						}
						else if (parameterPicker.PropertyInfo.PropertyType == typeof (List<ColorGradient>))
						{
							List<ColorGradient> gradients =
								parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect) as List<ColorGradient>;
							var newGradients = gradients.ToList();
							newGradients[parameterPicker.SelectedControl.Index] = gradient;
							elementValues.Add(element,
								new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect),
									parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);
						}
						else if (parameterPicker.PropertyInfo.PropertyType == typeof (List<GradientLevelPair>))
						{
							List<GradientLevelPair> gradients =
								parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
							var newGradients = gradients.ToList();
							newGradients[parameterPicker.SelectedControl.Index]=new GradientLevelPair(gradient, gradients[parameterPicker.SelectedControl.Index].Curve);
							elementValues.Add(element,
								new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect),
									parameterPicker.PropertyInfo));
							UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);
						}
					}
				}
				else
				{
					UpdateToolStrip4(String.Empty);
				}


			}
			CompleteDrop(elementValues, element, "Gradient");

		}

		private void HandleGradientDropOnGradient(ColorGradient gradient, Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, Element e, PropertyDescriptor property)
		{
			elementValues.Add(e,
				new Tuple<object, PropertyDescriptor>(property.GetValue(e.EffectNode.Effect), property));
			UpdateEffectProperty(property, e, gradient);
		}


		private void HandleGradientDropOnColorGradientList(PropertyData property, Element element, Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, ColorGradient gradient)
		{
			List<ColorGradient> gradients = property.Descriptor.GetValue(element.EffectNode.Effect) as List<ColorGradient>;
			if (gradients == null) return;

			var parameterPickerControls = CreateGradientListPickerControls(property, gradients);

			var parameterPicker = CreateParameterPicker(parameterPickerControls);

			ShowMultiDropMessage();
			var dr = parameterPicker.ShowDialog();
			if (dr == DialogResult.OK)
			{
				var newGradients = gradients.ToList();
				newGradients[parameterPicker.SelectedControl.Index] = gradient;
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect), parameterPicker.PropertyInfo));
				UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);
			}
		}


		private void HandleGradientDropOnGradientLevelPairList(PropertyData property, Element element, Dictionary<Element, Tuple<object, PropertyDescriptor>> elementValues, ColorGradient gradient)
		{
			List<GradientLevelPair> gradients = property.Descriptor.GetValue(element.EffectNode.Effect) as List<GradientLevelPair>;
			if (gradients == null) return;

			if (gradients.Count == 1)
			{
				var newGradients = gradients.ToList();
				newGradients[0] = new GradientLevelPair(gradient, gradients[0].Curve);
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(property.Descriptor.GetValue(element.EffectNode.Effect), property.Descriptor));
				UpdateEffectProperty(property.Descriptor, element, newGradients);
				return;
			}

			var parameterPickerControls = CreateGradientLevelPairPickerControls(property, gradients);

			var parameterPicker = CreateParameterPicker(parameterPickerControls);

			ShowMultiDropMessage();
			var dr = parameterPicker.ShowDialog();
			if (dr == DialogResult.OK)
			{
				var newGradients = gradients.ToList();
				newGradients[parameterPicker.SelectedControl.Index] = new GradientLevelPair(gradient, gradients[parameterPicker.SelectedControl.Index].Curve);
				elementValues.Add(element, new Tuple<object, PropertyDescriptor>(parameterPicker.PropertyInfo.GetValue(element.EffectNode.Effect), parameterPicker.PropertyInfo));
				UpdateEffectProperty(parameterPicker.PropertyInfo, element, newGradients);
			}
		}
		
		#endregion Preset Library Color Gradient Drop

		#region Bitmap methods for PL item drops

		private Bitmap GetColorBitmap(Color color)
		{
			Bitmap colorBitmap = new Bitmap(48, 48);
			Graphics gfx = Graphics.FromImage(colorBitmap);
			using (SolidBrush brush = new SolidBrush(color))
			{
				gfx.FillRectangle(brush, 0, 0, 64, 64);
			}

			return drawBitmapBorder(colorBitmap);
		}

		private Bitmap GetCurveBitmap(Curve curve)
		{
			var curveBitmap = new Bitmap((curve.GenerateGenericCurveImage(new Size(64, 64))));
			return drawBitmapBorder(curveBitmap);
		}

		private Bitmap GetColorGradientBitmap(ColorGradient colorGradient)
		{
			var gradientBitmap = new Bitmap((colorGradient.GenerateColorGradientImage(new Size(64, 64), false)));
			return drawBitmapBorder(gradientBitmap);
		}

		private Bitmap drawBitmapBorder(Bitmap image)
		{
			Graphics gfx = Graphics.FromImage(image);
			using (Pen p = new Pen(Color.FromArgb(136,136,136), 2))
			{
				gfx.DrawRectangle(p, 0, 0, image.Width, image.Height);	
			}
			return image;	
		}

		#endregion Bitmap methods for PL item drops

		#endregion

		#region Dragging Windows Explorer Files to Timeline.

		private void HandleFileDrop(string[] filePaths)
		{
			_mouseOriginalPoint = new Point(MousePosition.X, MousePosition.Y);
			
			//Holding Alt key down while dragging into timeline will flip the String Orientation from the default Vertical to Horizontal.
			//Holding the Ctrl key down while dragging will add the effects sequentialy onto the timeline.

			bool dragFileHorizontalStringOrientation = ModifierKeys == Keys.Alt || ModifierKeys == (Keys.Alt | Keys.Control);
			bool dragFileSequencialEffectPlacement = ModifierKeys == Keys.Control || ModifierKeys == (Keys.Alt | Keys.Control);


			List<Guid> tempeffectGuid = new List<Guid>();
			int i = 0;

			var supportedEffectDescriptors = ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>().Where(x => x.SupportsFiles);

			//Iterate through each selected file that was dragged.
			foreach (string filePath in filePaths)
			{
				Guid guid = Guid.Empty;
				//Check each Effect to see if it supports any File types
				
				var fileExtension = Path.GetExtension(filePath).ToLower();

				var effectDescriptors =
					supportedEffectDescriptors.Where(x => x.SupportedFileExtensions.Contains(fileExtension)).ToList();

				//Point p = new Point(e.X, e.Y);
				if (effectDescriptors.Any())
				{
					if (effectDescriptors.Count <= 1)
					{
						//Do this if there is only one effect that the file can be used with.
						guid = effectDescriptors[0].TypeId;
					}
					else
					{
						//Checks to see if an effect has already been used for the selected file type and if so then just use that effect agiain.
						//Saves having the user select an effect for multiple files that are the same type.
						foreach (var effectDescriptor in effectDescriptors.Where(effectDescriptor => tempeffectGuid.Contains(effectDescriptor.TypeId)))
						{
							guid = effectDescriptor.TypeId; //effect has already been selected so just use that same effect again.
							break;
						}

						//If effect hasn't been auto selected then get the user to select one. Will bring up the Effect Selection Parameter Form
						if (guid == Guid.Empty) guid = HandleFileDropOnEffectList(effectDescriptors);

						//if guid is still empty then the file is not supported or user hit escape key and didn't select an effect to use in Vixen so go to the next file.
						if (guid == Guid.Empty) continue;

						tempeffectGuid.Add(guid); //Adds Effect Guid's so it can be checked against for the next file. This will save showing the Effect selection form everytime or the same file type.
					}

					//If effect Placement is false then just stack all the new effcts on top of each other, else add them sequentially in the timeline.
					if (!dragFileSequencialEffectPlacement) i = 0;

					IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(guid);

					
					//This is extremely brittle. If anything changes in these effects
					//this could break. I added a qualifier to use the common FileName property name for the file 
					//property, but if any new effects don't use that name they could break. This should probably be 
					//some type of attribute. The intimate knowledge of the Picture source is yet another problem.
					foreach (var propertyData in MetadataRepository.GetProperties(effect).Where(propertyData => propertyData.PropertyType == typeof(PictureSource) || propertyData.PropertyType == typeof(ShapeList) || propertyData.PropertyType == typeof(String) || propertyData.PropertyType == typeof(StringOrientation)))
					{
						if (propertyData.PropertyType == typeof(PictureSource))
						{
							propertyData.Descriptor.SetValue(effect, PictureSource.File);
						}

						if (propertyData.PropertyType == typeof(ShapeList))
						{
							propertyData.Descriptor.SetValue(effect, ShapeList.File);
						}

						if (propertyData.PropertyType == typeof(String) && propertyData.Name == @"FileName")
						{
							propertyData.Descriptor.SetValue(effect, filePath);
						}
						if (dragFileHorizontalStringOrientation)
						{
							if (propertyData.PropertyType == typeof(StringOrientation)) propertyData.Descriptor.SetValue(effect, StringOrientation.Horizontal);
						}
					}

					AddEffectInstance(effect, TimelineControl.grid.RowAtPosition(_mouseOriginalPoint), TimelineControl.grid.TimeAtPosition(_mouseOriginalPoint) + TimeSpan.FromTicks(20000000 * i), GetDefaultEffectDuration(TimelineControl.grid.TimeAtPosition(_mouseOriginalPoint) + TimeSpan.FromTicks(20000000 * i)), true);
				}
				i++;
			}
		}

		//Will add each effect to the Effect Selection Parameter form so the user can select which effect to use with the file type.
		private Guid HandleFileDropOnEffectList(IEnumerable<IEffectModuleDescriptor> effectDescriptors)
		{
			var parameterPickerControls = CreateEffectListPickerControls(effectDescriptors);

			var parameterPicker = CreateParameterPicker(parameterPickerControls);

			UpdateToolStrip4("Choose the Effect to use, press Escape to cancel.", 12);
			var dr = parameterPicker.ShowDialog();
			if (dr == DialogResult.OK)
			{
				return parameterPicker.EffectPropertyInfo.TypeId;
			}
			return Guid.Empty;
		}

		private List<EffectParameterPickerControl> CreateEffectListPickerControls(IEnumerable<IEffectModuleDescriptor> effectDescriptors)
		{
			var effectModuleDescriptors = effectDescriptors as IList<IEffectModuleDescriptor> ?? effectDescriptors.ToList();

			return effectModuleDescriptors.Select((t, i) =>
			{
				return new EffectParameterPickerControl
				{
					Index = i,
					EffectPropertyInfo = t,
					ParameterImage = new Bitmap(t.GetRepresentativeImage(), 64, 64),
					DisplayName = t.EffectName
				};
			}).ToList();
		}
		#endregion

		#region Overridden form functions (On___)

		public bool IgnoreKeyDownEvents { get; set; }

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

			TimelineElementsClipboardData result = new TimelineElementsClipboardData
			{
				FirstVisibleRow = -1,
				EarliestStartTime = TimeSpan.MaxValue,
			};

			int rownum = 0;
			var affectedElements = new List<Element>();
			var layerManager = LayerManager;
			HashSet<string> uniqueStrings = new HashSet<string>();
			foreach (Row row in TimelineControl.VisibleRows)
			{
				//Check that the same Row name has not already been processed and will skip if the same Row is found visible in another group.
				if (uniqueStrings.Contains(row.Name)) continue;
				uniqueStrings.Add(row.Name);

				// Since removals may happen during enumeration, make a copy with ToArray().

				//If we already have the elements becasue the same row is duplicated then skip.
				if (affectedElements.Intersect(row.SelectedElements).Any())
				{
					rownum++;
					continue;
				}

				affectedElements.AddRange(row.SelectedElements);
				foreach (Element elem in row.SelectedElements.ToArray())
				{
					if (result.FirstVisibleRow == -1)
						result.FirstVisibleRow = rownum;

					int relativeVisibleRow = rownum - result.FirstVisibleRow;
					var layer = layerManager.GetLayer(elem.EffectNode);
					EffectModelCandidate modelCandidate =
						new EffectModelCandidate(elem.EffectNode.Effect)
							{
								Duration = elem.Duration,
								StartTime = elem.StartTime,
								LayerId = layer.Id,
								LayerName = layer.LayerName,
								LayerTypeId = layer.FilterTypeId
							};
					result.EffectModelCandidates.Add(modelCandidate, relativeVisibleRow);

					if (elem.StartTime < result.EarliestStartTime)
						result.EarliestStartTime = elem.StartTime;

					if (cutElements)
					{
						RemoveEffectNodeAndElement(elem.EffectNode);
						SequenceModified();
					}
				}
				rownum++;
			}
			if (cutElements)
			{
				var act = new EffectsCutUndoAction(this, affectedElements.Select(x => x.EffectNode));
				_undoMgr.AddUndoAction(act);	
			}
			

			IDataObject dataObject = new DataObject(ClipboardFormatName);
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

			if (dataObject.GetDataPresent(ClipboardFormatName.Name))
			{
				data = dataObject.GetData(ClipboardFormatName.Name) as TimelineElementsClipboardData;
			}

			List<int> index = new List<int>();
			List<TimeSpan> markStartTimes = new List<TimeSpan>();
			List<KeyValuePair<EffectModelCandidate, int>> effects;
			switch (PastingMode)
			{
				case PastingMode.VisibleMarks:
				{
					// We need to order the effects by Start time as they are currently ordered by Row index first which is
					// no good for pasting to Mark Collection or Visible Marks.
					effects = data.EffectModelCandidates.OrderBy(x => (x.Key.StartTime)).ToList();
					for (int i = 0; i < _sequence.LabeledMarkCollections.Count; i++)
					{
						// Only continue process visible Mark collections
						if (_sequence.LabeledMarkCollections[i].IsVisible)
						{
							for (int markIndex = 0;
								markIndex < _sequence.LabeledMarkCollections[i].Marks.Count;
								markIndex++)
							{
								if (pasteTime <= _sequence.LabeledMarkCollections[i].Marks[markIndex].StartTime)
								{
									for (int j = 0; j < effects.Count; j++)
									{
										// Will only add the Mark start times for required number of effects or number of
										// marks available whichever is the lesser.
										markStartTimes.Add(_sequence.LabeledMarkCollections[i].Marks[markIndex]
											.StartTime);
										markIndex++;
										if (markIndex == _sequence.LabeledMarkCollections[i].Marks.Count) break;
									}
									break;
								}
							}
						}
					}
					// If we processed multiple MArk Collections that were visible we need to sort the results so the Times are in ascending order.
					markStartTimes.Sort();
					break;
				}
				case PastingMode.Invert:
					foreach (KeyValuePair<EffectModelCandidate, int> order in data.EffectModelCandidates)
					{
						index.Add(data.EffectModelCandidates.Last().Value - order.Value);
					}
					effects = data.EffectModelCandidates.ToList();
					break;
				default:
					// This is the standard paste
					effects = data.EffectModelCandidates.ToList();
					break;
			}

			if (data == null)
				return result;
			TimeSpan offset = pasteTime == TimeSpan.Zero ? TimeSpan.Zero : data.EarliestStartTime;
			Row targetRow = TimelineControl.SelectedRow ?? TimelineControl.ActiveRow ?? TimelineControl.TopVisibleRow;
			List<Row> visibleRows = new List<Row>(TimelineControl.VisibleRows);
			int topTargetRoxIndex = visibleRows.IndexOf(targetRow);
			List<EffectNode> nodesToAdd = new List<EffectNode>();
			foreach (KeyValuePair<EffectModelCandidate, int> kvp in effects)
			{
				EffectModelCandidate effectModelCandidate = kvp.Key;
				int relativeRow = kvp.Value;
				TimeSpan targetTime = effectModelCandidate.StartTime - offset + pasteTime;
				switch (PastingMode)
				{
					case PastingMode.VisibleMarks:
						// now grab the start time of the next mark.
						if (result >= markStartTimes.Count) break; // will break if there are more effects then there are marks.
						targetTime = markStartTimes[result];
						break;
					case PastingMode.Invert:
						relativeRow = index[result];
						break;
				}
				if (PastingMode == PastingMode.VisibleMarks && result >= markStartTimes.Count) break;

				int targetRowIndex = topTargetRoxIndex + relativeRow;
				if (targetTime > TimelineControl.grid.TotalTime)
				{
					continue;
				}
				if (targetTime + effectModelCandidate.Duration > TimelineControl.grid.TotalTime)
				{
					//Shorten to fit.
					effectModelCandidate.Duration = TimelineControl.grid.TotalTime - targetTime;
				}
				if (targetRowIndex<0 || targetRowIndex >= visibleRows.Count)
					continue;

				//Make a new effect and populate it with the detail data from the clipboard
				var newEffect = ApplicationServices.Get<IEffectModuleInstance>(effectModelCandidate.TypeId);
				newEffect.ModuleData = effectModelCandidate.GetEffectData();
				var node = CreateEffectNode(newEffect, visibleRows[targetRowIndex], targetTime, effectModelCandidate.Duration);

				if(LayerManager.ContainsLayer(effectModelCandidate.LayerId))
				{
					LayerManager.AssignEffectNodeToLayer(node, effectModelCandidate.LayerId);
				}
				else
				{
					//Best efforts to try and find a layer of the same name and type before letting it assign default.
					var layer = LayerManager.GetLayer(effectModelCandidate.LayerName, effectModelCandidate.LayerTypeId);
					if (layer != null)
					{
						LayerManager.AssignEffectNodeToLayer(node, layer.Id);
					}

				}
				nodesToAdd.Add(node);
				
				result++;
			}

			// put it in the sequence and in the timeline display
			List<TimedSequenceElement> elements = AddEffectNodes(nodesToAdd);
			SequenceModified();

			var act = new EffectsPastedUndoAction(this, elements.Select(x => x.EffectNode));
			_undoMgr.AddUndoAction(act);

			return result;
		}

		public PastingMode PastingMode { get; private set; }

		#endregion

		#region Undo

		private void InitUndo()
		{
			_undoMgr = new UndoManager();
			_undoMgr.UndoItemsChanged += _undoMgr_UndoItemsChanged;
			_undoMgr.RedoItemsChanged += _undoMgr_RedoItemsChanged;

			editToolStripButton_Undo.Enabled = false;
			undoToolStripMenuItem.Enabled = false;
			editToolStripButton_Undo.ItemChosen += undoButton_ItemChosen;

			editToolStripButton_Redo.Enabled = false;
			redoToolStripMenuItem.Enabled = false;
			editToolStripButton_Redo.ItemChosen += redoButton_ItemChosen;
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
				editToolStripButton_Undo.Enabled = false;
				undoToolStripMenuItem.Enabled = false;
				return;
			}

			editToolStripButton_Undo.Enabled = true;
			undoToolStripMenuItem.Enabled = true;
			editToolStripButton_Undo.UndoItems.Clear();
			foreach (var act in _undoMgr.UndoActions)
				editToolStripButton_Undo.UndoItems.Add(act.Description);
		}

		private void _undoMgr_RedoItemsChanged(object sender, EventArgs e)
		{
			if (_undoMgr.NumRedoable == 0)
			{
				editToolStripButton_Redo.Enabled = false;
				redoToolStripMenuItem.Enabled = false;
				return;
			}

			editToolStripButton_Redo.Enabled = true;
			redoToolStripMenuItem.Enabled = true;
			editToolStripButton_Redo.UndoItems.Clear();
			foreach (var act in _undoMgr.RedoActions)
				editToolStripButton_Redo.UndoItems.Add(act.Description);
		}


		private void timelineControl_ElementsMovedNew(object sender, ElementsChangedTimesEventArgs e)
		{
			var action = new ElementsTimeChangedUndoAction(this, e.PreviousTimes, e.Type);
			_undoMgr.AddUndoAction(action);
		}

		/// <summary>
		/// Used by the Undo/Redo engine
		/// </summary>
		/// <param name="changedElements"></param>
		public void SwapPlaces(Dictionary<Element, ElementTimeInfo> changedElements)
		{
			TimelineControl.grid.SwapElementPlacement(changedElements);
		}

		/// <summary>
		/// Used by the Undo/Redo engine
		/// </summary>
		/// <param name="changedMarks"></param>
		public void SwapPlaces(Dictionary<IMark, MarkTimeInfo> changedMarks)
		{
			foreach (KeyValuePair<IMark, MarkTimeInfo> e in changedMarks)
			{
				// Key is reference to actual mark. Value is class with its times before move.
				// Swap the mark's times with the saved times from before the move, so we can restore them later in redo.
				MarkTimeInfo.SwapPlaces(e.Key, e.Value);
			}

			_timeLineGlobalEventManager.OnMarksMoving(new MarksMovingEventArgs(changedMarks.Keys.ToList()));
			CheckAndRenderDirtyElementsAsync();
		}

		public void SwapLayers(Dictionary<IEffectNode, ILayer> effectNodes)
		{
			foreach (var node in effectNodes.Keys.ToList())
			{
				var tmpLayer = LayerManager.GetLayer(node);
				LayerManager.AssignEffectNodeToLayer(node, effectNodes[node]);
				effectNodes[node] = tmpLayer;
			}

			SequenceModified();
		}
		//private void SwapEffectData(Dictionary<Element, EffectModelCandidate> changedElements)
		//{
		//	List<Element> keys = new List<Element>(changedElements.Keys);
		//	foreach (var element in keys)
		//	{
		//		EffectModelCandidate modelCandidate =
		//			new EffectModelCandidate(element.EffectNode.Effect)();

		//		element.EffectNode.Effect.ModuleData = changedElements[element].GetEffectData(); ;
		//		changedElements[element] = modelCandidate;
		//		element.UpdateNotifyContentChanged();
		//	}
		//}

		public void AddEffectsModifiedToUndo(Dictionary<Element, EffectModelCandidate> modifiedEffectElements, string labelName="properties")
		{
			_undoMgr.AddUndoAction(new EffectsModifiedUndoAction(modifiedEffectElements, labelName));
		}

		public void AddEffectsModifiedToUndo(EffectsPropertyModifiedUndoAction modifiedElements)
		{
			_undoMgr.AddUndoAction(modifiedElements);
		}

		#endregion

		#region IEditorUserInterface implementation

		public bool IsModified
		{
			get { return _mModified; }
		}

		public bool IsEditorStateModified
		{
			get { return _editorStateModified; }
			set { _editorStateModified = value; }
		}

		public void RefreshSequence()
		{
			Sequence = Sequence;
		}

		/// <summary>
		/// Saves the sequence to the optional given path
		/// </summary>
		/// <param name="filePath"></param>
		public void Save(string filePath = null)
		{
			SaveSequence(filePath);
		}

		public ISelection Selection
		{
			get { throw new NotImplementedException(); }
		}

		public SequenceLayers LayerManager
		{
			get
			{
				return Sequence.GetSequenceLayerManager();
				
			}
		}

		public ISequence Sequence
		{
			get { return _sequence; }
			set
			{
				if (value is TimedSequence)
				{
					SequenceRemoved();
					_sequence = (TimedSequence)value;
					SequenceAdded();
				}
				else
				{
					throw new NotImplementedException("Cannot use sequence type with a Timed Sequence Editor");
				}
				//loadSequence(value); 
			}
		}

		private void SequenceAdded()
		{
			_sequence.LabeledMarkCollections.CollectionChanged += LabeledMarkCollections_CollectionChanged;
			AddMarkCollectionHandlers(_sequence.LabeledMarkCollections);
		}

		private void MarkCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SequenceModified();
		}

		private void LabeledMarkCollections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				RemoveMarkCollectionHandlers(e.OldItems.Cast<MarkCollection>());
			}
			else if (e.Action == NotifyCollectionChangedAction.Add)
			{
				AddMarkCollectionHandlers(e.NewItems.Cast<MarkCollection>());
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				//We don't know what happened, so remove all that may exist and add them back to be sure.
				RemoveMarkCollectionHandlers(_sequence.LabeledMarkCollections);
				AddMarkCollectionHandlers(_sequence.LabeledMarkCollections);
			}
			SequenceModified();
		}

		private void SequenceRemoved()
		{
			if (_sequence != null)
			{
				_sequence.LabeledMarkCollections.CollectionChanged -= LabeledMarkCollections_CollectionChanged;
				RemoveMarkCollectionHandlers(_sequence.LabeledMarkCollections);
			}
		}

		private void RemoveMarkCollectionHandlers(IEnumerable<IMarkCollection> markCollections)
		{
			foreach (var mc in markCollections)
			{
				mc.PropertyChanged -= MarkCollection_PropertyChanged;
				mc.Decorator.PropertyChanged -= MarkCollection_PropertyChanged;
			}
		}

		private void AddMarkCollectionHandlers(IEnumerable<IMarkCollection> markCollections)
		{
			foreach (var mc in markCollections)
			{
				mc.PropertyChanged += MarkCollection_PropertyChanged;
				mc.Decorator.PropertyChanged += MarkCollection_PropertyChanged;
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
			UnregisterClipboardViewer();
			if (WindowState == FormWindowState.Minimized)
			{
				//Don't close with a minimized window.
				WindowState = FormWindowState.Normal;
			}

			dockPanel.SaveAsXml(_settingsPath);
			MarksForm.Close();
			EffectsForm.Close();
			LayerEditor.Close();
			FindEffects.Close();

			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockLeftPortion", Name), (int)dockPanel.DockLeftPortion);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DockRightPortion", Name), (int)dockPanel.DockRightPortion);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AutoSaveEnabled", Name), autoSaveToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/DrawModeSelected", Name), modeToolStripButton_DrawMode.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SelectionModeSelected", Name), modeToolStripButton_SelectionMode.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapToSelected", Name), modeToolStripButton_SnapTo.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name), Size.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name), Size.Width);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), WindowState.ToString());
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SnapStrength", Name), TimelineControl.grid.SnapStrength);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CloseGapThreshold", Name), TimelineControl.grid.CloseGap_Threshold);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlignToThreshold", Name), AlignTo_Threshold);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorEnabled", Name), TimelineControl.grid.ResizeIndicator_Enabled);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CadStyleSelectionBox", Name), cADStyleSelectionBoxToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ResizeIndicatorColor", Name), TimelineControl.grid.ResizeIndicator_Color);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CurveLinkCurves", Name), CurveLibraryForm.LinkCurves);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/GradientLinkGradients", Name), GradientLibraryForm.LinkGradients);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ZoomUnderMousePosition", Name), zoomUnderMousePositionToolStripMenuItem.Checked);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WaveFormHeight", Name), TimelineControl.waveform.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/RulerHeight", Name), TimelineControl.ruler.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SplitterDistance", Name), TimelineControl.splitContainer.SplitterDistance);
			
			Save_ToolsStripItemsFile();

			//This .Close is here because we need to save some of the settings from the form before it is closed.
			ColorLibraryForm.Close();
			GradientLibraryForm.Close();
			CurveLibraryForm.Close();

			//These are only saved in options
			//xml.PutPreference(string.Format("{0}/AutoSaveInterval", Name), _autoSaveTimer.Interval);

			//Clean up any old locations from before we organized the settings.
			xml.RemoveNode("StandardNudge");
			xml.RemoveNode("SuperNudge");
			xml.RemoveNode("NudgeSettings");
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
			get { return TimingSource.Position; }
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

		public bool PositionHasValue
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
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(ex.Message, @"Error parsing time", false, false);
				messageBox.ShowDialog();
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
			//TODO finish the clean up of removing the mark manager
			//MarkManager manager = new MarkManager(new List<MarkCollection>(_sequence.MarkCollections), this, this, this);
			//if (manager.ShowDialog() == DialogResult.OK)
			//{
			//	_sequence.MarkCollections = manager.MarkCollections;
			//	PopulateMarkSnapTimes();
			//	SequenceModified();
			//	MarksForm.PopulateMarkCollectionsList(null);
			//}
		}
	
		private void _SetTimingSpeed(float speed)
		{
			if (speed <= 0.10)
			{
				_timingSpeed = 0.10f;
				return;
			}

			if (speed > 4.0f)
			{
				_timingSpeed = 4.0f;
				return;
			}

			_timingSpeed = speed;

			// If they haven't executed the sequence yet, the timing source member will not yet be set.
			if (TimingSource != null)
			{
				TimingSource.Speed = _timingSpeed;
			}

			_UpdateTimingSpeedDisplay();
			toolStripButton_DecreaseTimingSpeed.Enabled = _timingSpeed > _timingChangeDelta;
			toolStripButton_IncreaseTimingSpeed.Enabled = _timingSpeed < 4.0f;
		}

		private void _UpdateTimingSpeedDisplay()
		{
			audioToolStripLabel_TimingSpeed.Image = SpeedVisualisation(); ;
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
					audioToolStripLabel_TimingSpeed.Enabled =
				   timingSource != null && timingSource.SupportsVariableSpeeds;

			}
		}

		private void _PlaySequence(TimeSpan rangeStart, TimeSpan rangeEnd)
		{
			EffectEditorForm.PreviewStop();
			SetPreviewsTopMost();
			if (_context.IsRunning && _context.IsPaused)
			{
				_context.Resume();
				UpdateButtonStates();
			}
			else
			{
				if (playBackToolStripButton_Loop.Checked)
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

		private void SetPreviewsTopMost(bool activate = true)
		{
			foreach (var preview in VixenSystem.Previews)
			{
				var p = preview.PreviewModule;

				if (activate)
				{
					p.PlayerStarted();
				}
				else
				{
					p.PlayerEnded();
				}
			}
		}

		private ITiming TimingSource
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
							audioToolStripLabel_TimingSpeed.Enabled = true;
					toolStripButton_DecreaseTimingSpeed.Enabled = toolStripButton_DecreaseTimingSpeed.Enabled = _timingSpeed > _timingChangeDelta;

				}
				else
				{
					_UpdateTimingSpeedDisplay();
					toolStripButton_IncreaseTimingSpeed.Enabled =
				   toolStripButton_DecreaseTimingSpeed.Enabled =
				   audioToolStripLabel_TimingSpeed.Enabled = false;
				}
			}
		}

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Task _loadingTask = null;

		private void TimedSequenceEditorForm_Shown(object sender, EventArgs e)
		{
			Enabled = false;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			//loadingTask = Task.Factory.StartNew(() => loadSequence(_sequence), token);
			LoadSequence(_sequence);
		}

		private void menuStrip_MenuActivate(object sender, EventArgs e)
		{
			//Check against the private object because it may not even be created and we don't want opening the menu
			//to create the form if the user does not activate it. 
			effectWindowToolStripMenuItem.Checked = !(_effectsForm == null || _effectsForm.DockState == DockState.Unknown);
			markWindowToolStripMenuItem.Checked = !(_marksForm == null || _marksForm.DockState == DockState.Unknown);
			mixingFilterEditorWindowToolStripMenuItem.Checked =
				!(_layerEditor == null || _layerEditor.DockState == DockState.Unknown);
			toolStripMenuItemFindEffects.Checked = !(_findEffects == null || _findEffects.DockState == DockState.Unknown);
			toolStripMenuItemColorLibrary.Checked = !(_colorLibraryForm == null || _colorLibraryForm.DockState == DockState.Unknown);
			toolStripMenuItemGradientLibrary.Checked = !(_gradientLibraryForm == null || _gradientLibraryForm.DockState == DockState.Unknown);
			toolStripMenuItemCurveLibrary.Checked = !(_curveLibraryForm == null || _curveLibraryForm.DockState == DockState.Unknown);
			gridWindowToolStripMenuItem.Checked = !(GridForm.IsHidden || GridForm.DockState == DockState.Unknown);
			effectEditorWindowToolStripMenuItem.Checked =
				!(_effectEditorForm == null || EffectEditorForm.DockState == DockState.Unknown);
		}

		private void timerPostponePlay_Tick(object sender, EventArgs e)
		{
			timerPostponePlay.Enabled = timerDelayCountdown.Enabled = false;
			PlaySequence();
		}

		private void ClearDelayPlayItemChecks()
		{
			//Make sure Looping is not enabled
			playBackToolStripButton_Loop.Checked = toolStripMenuItem_Loop.Checked = false;
			foreach (ToolStripMenuItem item in playOptionsToolStripMenuItem.DropDownItems)
			{
				item.Checked = false;
			}
		}

		private void timerDelayCountdown_Tick(object sender, EventArgs e)
		{
			_delayCountDown--;
			toolStripStatusLabel_delayPlay.Text = string.Format("{0} Seconds", _delayCountDown);
		}

        private void ResetLipSyncNodes()
        {
            foreach (Row row in TimelineControl.Rows)
            {
                for (int j = 0; j < row.ElementCount; j++)
                {
                    Element elem = row.ElementAt(j);
					IEffectModuleInstance effect = elem.EffectNode.Effect;
					if (effect.GetType() == typeof(LipSync))
					{
						effect.MarkDirty();
					}
                }
            }
            TimelineControl.grid.RenderAllRows();
        }

        private void textConverterHandler(object sender, NewTranslationEventArgs args)
        {
            TimelineElementsClipboardData result = new TimelineElementsClipboardData
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
					((LipSync)effect).StaticPhoneme = (App.LipSyncApp.PhonemeType)Enum.Parse(typeof(App.LipSyncApp.PhonemeType), data.Phoneme.ToString().ToUpper());
                    ((LipSync)effect).LyricData = data.LyricData;
                    EffectModelCandidate modelCandidate =
                          new EffectModelCandidate(effect)
                          {
                              Duration = data.Duration,
                              StartTime = data.StartOffset
                          };
                    result.EffectModelCandidates.Add(modelCandidate, 0);
                    if (data.StartOffset < result.EarliestStartTime)
                        result.EarliestStartTime = data.StartOffset;
                    effect.PreRender();
                }
                IDataObject dataObject = new DataObject(ClipboardFormatName);
                dataObject.SetData(result);
                Clipboard.SetDataObject(dataObject, true);
                _TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);
                int pasted = 0;
                if (args.Placement == TranslatePlacement.Cursor)
                {
                    args.FirstMark += _timeLineGlobalStateManager.CursorPosition;
                }
                if (args.Placement != TranslatePlacement.Clipboard)
				{
					PastingMode = PastingMode.Default;
					pasted = ClipboardPaste(args.FirstMark);
                }
                if (pasted == 0)
                {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Conversion Complete and copied to Clipboard \n Paste at first Mark offset", @"Convert Text", false, false);
					messageBox.ShowDialog();
                }
                SequenceModified();
            }
        }

        private void translateFailureHandler(object sender, TranslateFailureEventArgs args)
        {
            GetUserMappingForFailedWord(args.FailedWord);
        }

		private string GetUserMappingForFailedWord(string failedWord)
		{
			var mappedPhonemes = string.Empty;
			LipSyncTextConvertFailForm failForm = new LipSyncTextConvertFailForm
			{
				errorLabel =
				{
					Text = @"Unable to find mapping for " + failedWord + Environment.NewLine +
					       @"Please map using buttons below"
				}
			};
			DialogResult dr = failForm.ShowDialog(this);
			if (dr == DialogResult.OK)
			{
				LipSyncTextConvert.AddUserMaping(failedWord + " " + failForm.TranslatedString);
				mappedPhonemes = failForm.TranslatedString;
			}

			return mappedPhonemes;
		}

        private void changeMappings_Click(object sender, EventArgs e)
        {

		}

		/// <summary>
		/// Aligns selected elements, or if none, all elements to the closest mark.
		/// alignMethod = Start to align the start of the elements, End to align the end of the effects and Both to align Start and End of elements.
		/// </summary>
		/// <param name="alignMethod"></param>
		private void AlignEffectsToNearestMarks(string alignMethod)
		{
			IEnumerable elements;
			if (!TimelineControl.grid.SelectedElements.Any())
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("No effects have been selected and action will be applied to your entire sequence. This can take a considerable length of time, are you sure ?",
					@"Align effects to marks", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No) return;
				elements = TimelineControl.Rows.SelectMany(row => row); //add all elements within the sequence to elements list
			}
			else
			{
				elements = TimelineControl.SelectedElements;
			}

			var moveElements = new Dictionary<Element, Tuple<TimeSpan, TimeSpan>>();
			
			foreach (Element element in elements)
			{
				var nearestStartMark = element.StartTime;
				var nearestEndMark = element.EndTime;

				switch (alignMethod)
				{
					case "Start":
						nearestStartMark = FindNearestMark(element.StartTime, element.EndTime, alignMethod);
						break;
					case "End":
						nearestEndMark = FindNearestMark(element.EndTime, element.StartTime, alignMethod);
						break;
					case "Both":
						nearestStartMark = FindNearestMark(element.StartTime, element.EndTime, "Start");
						nearestEndMark = FindNearestMark(element.EndTime, nearestStartMark, "End");
						break;
				}
				if (nearestStartMark != TimeSpan.Zero && !moveElements.ContainsKey(element) && nearestEndMark != TimeSpan.Zero &&
				    !moveElements.ContainsKey(element))
				{
					moveElements.Add(element, new Tuple<TimeSpan, TimeSpan>(nearestStartMark, nearestEndMark));
				}
			}

			//Make sure we have elements in the list to move.
			if (moveElements.Any()) TimelineControl.grid.MoveResizeElements(moveElements);
		}

		private void AddMarksToSelectedEffects()
		{
			IEnumerable elements = null;
			if (!TimelineControl.grid.SelectedElements.Any())
			{
				var messageBox = new MessageBoxForm("No effects have been selected and action will be applied to your entire sequence. This can take a considerable length of time, are you sure ?",
					@"Align effects to marks", MessageBoxButtons.YesNo, SystemIcons.Information);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No) return;
				elements = TimelineControl.Rows.SelectMany(row => row); //add all elements within the sequence to elements list
			}
			else
			{
				elements = TimelineControl.SelectedElements;
			}
			var addedMarks = new Dictionary<IMark, IMarkCollection>();
			foreach (Element element in elements)
			{
				var mark = AddMarkAtTime(element.StartTime, true);
				if (mark != null)
				{
					addedMarks.Add(mark, _sequence.LabeledMarkCollections.FirstOrDefault(x => x.IsDefault));
				}
			}
			if (addedMarks.Count > 0)
			{
				var act = new MarksAddedUndoAction(this, addedMarks);
				_undoMgr.AddUndoAction(act);
				CheckAndRenderDirtyElementsAsync();
			}
		}

		private void ResetTimeLineSettings()
		{
			TimelineControl.VisibleTimeStart = TimeSpan.Zero;
			TimelineControl.TimePerPixel = TimeSpan.FromTicks(100000);
		}

		/// <summary>
		/// Returns the TimeSpan location of the nearest mark to the given TimeSpan
		/// Located within the threshhold: AlignTo_Threshold
		/// </summary>
		/// <param name="referenceTimeSpan"></param>
		/// <param name="referenceTimeSpan1"></param>
		/// <param name="alignMethod"></param>
		/// <returns></returns>
		private TimeSpan FindNearestMark(TimeSpan referenceTimeSpan, TimeSpan referenceTimeSpan1, string alignMethod)
		{
			var threshold = TimeSpan.FromSeconds(Convert.ToDouble(AlignTo_Threshold, CultureInfo.InvariantCulture));
			TimeSpan result = TimeSpan.Zero;
			TimeSpan compareResult = TimeSpan.Zero;

			foreach (var markTime in _sequence.LabeledMarkCollections.Where(markCollection => markCollection.ShowGridLines).SelectMany(markCollection => markCollection.Marks))
			{
				if (markTime.StartTime == referenceTimeSpan)
				{
					return markTime.StartTime;
				}

				if (markTime.StartTime > referenceTimeSpan - threshold && markTime.StartTime < referenceTimeSpan + threshold && (alignMethod == "Start" && markTime.StartTime < referenceTimeSpan1 || alignMethod == "End" && markTime.StartTime > referenceTimeSpan1))
				{
					if (markTime.StartTime > referenceTimeSpan && markTime.StartTime - referenceTimeSpan < compareResult.Duration() || result == TimeSpan.Zero)
					{
						compareResult = markTime.StartTime - referenceTimeSpan;
						result = markTime.StartTime;
					}
					else if (markTime.StartTime < referenceTimeSpan && referenceTimeSpan - markTime.StartTime < compareResult.Duration() || result == TimeSpan.Zero)
					{
						compareResult = referenceTimeSpan - markTime.StartTime;
						result = markTime.StartTime;
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Returns the default new effect duration.
		/// If visibileDuration is true it will scale so that the whole effect is visible while zoomed in.
		/// </summary>
		public TimeSpan GetDefaultEffectDuration(TimeSpan startTime,bool visibleDuration = true)
		{
			// The default length of a newly created effect is 2 seconds
			TimeSpan defaultEffectDuration = TimeSpan.FromSeconds(2);
			if (visibleDuration)
			{
				// Adjust the timeSpan to 80% to keep the new effects end visible when zoomed in a lot
				if ((TimelineControl.VisibleTimeEnd.Seconds - startTime.Seconds) <= defaultEffectDuration.Seconds)
				{
					defaultEffectDuration = (TimelineControl.VisibleTimeEnd - startTime).Scale(0.8);
				}
				// Don't make an effect shorter than 250 milliseconds.
				if (defaultEffectDuration.TotalMilliseconds <= 250)
				{
					defaultEffectDuration = TimeSpan.FromMilliseconds(250);
				}
			}
			return defaultEffectDuration;
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
	}
}