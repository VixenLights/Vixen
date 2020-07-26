using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VixenModules.App.Polygon;
using VixenModules.Editor.PolygonEditor.Adorners;
using VixenModules.Editor.PolygonEditor.Converters;
using Point = System.Windows.Point;

namespace VixenModules.Editor.PolygonEditor.ViewModels
{
	/// <summary>
	/// View model for the polygon editor.
	/// </summary>
	public class PolygonEditorViewModel : ViewModelBase, IResizeable
	{
		#region Constructor 
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="polygons">Collection of polygon model objects</param>
		public PolygonEditorViewModel()
		{
			// Create the collection of model polygons
			PolygonModels = new ObservableCollection<Polygon>();

			// Create the collection of model lines
			LineModels = new ObservableCollection<Line>();

			// Create the collection of view model polygons	 						
			Polygons = new ObservableCollection<PolygonViewModel>();

			// Create the collection of view model lines
			Lines = new ObservableCollection<LineViewModel>();

			// Create the collection of polygon snapshots
			PolygonSnapshots = new ObservableCollection<PolygonSnapShotViewModel>();
			
			// If the editor is being displayed without any polygons then...
			if (Polygons.Count == 0)
			{
				// Default to draw mode where we are creating a polygon
				AddNewPolygon();
			}

			// Create the collection of selected points
			SelectedPoints = new ObservableCollection<PolygonPointViewModel>();

			// Create the commands
			CopyCommand = new Command(Copy, IsShapeSelected);
			PasteCommand = new Command(Paste, CanExecutePaste);
			DeleteCommand = new Command(Delete, CanExecuteDelete);
			CutCommand = new Command(Cut, CanExecuteDelete);
			SnapToGridCommand = new Command(SnapToGrid);
			OkCommand = new Command(OK);
			CancelCommand = new Command(Cancel);
			DeletePointCommand = new Command(DeletePoint);
			ToggleStartSideCommand = new Command(ToggleStartSide, IsPolygonSelected);
			ToggleStartPointCommand = new Command(ToggleStartPoint, IsLineSelected);
			PolygonToLineCommand = new Command(TogglePolygonToLine, IsPolygon);
			LineToPolygonCommand = new Command(ToggleLineToPolygon, IsLine);
			AddPolygonSnapShotCommand = new Command(AddPolygonSnapShot);
			DeletePolygonSnapShotCommand = new Command(DeletePolygonSnapShot, IsDeleteSnapshotPolygonEnabled);
			NextPolygonSnapShotCommand = new Command(NextPolygonSnapShot, IsNextPolygonSnapShotEnabled);
			PreviousPolygonSnapShotCommand = new Command(PreviousPolygonSnapShot, IsPreviousPolygonSnapShotEnabled);
			AddPointCommand = new Command(AddPolygonPoint, IsAddPolygonPointEnabled);
			DrawPolygonCommand = new Command(DrawPolygonAction, IsDrawShapeEnabled);
			DrawLineCommand = new Command(DrawLineAction, IsDrawShapeEnabled);			
			MoveSnapshotCommand = new Command<MouseEventArgs>(MoveMouseSnapshot);
			MouseLeftButtonDownTimeBarCommand = new Command<MouseEventArgs>(MouseLeftButtonDownTimeBar);
			MouseLeftButtonUpTimeBarCommand = new Command<MouseEventArgs>(MouseLeftButtonUpTimeBar);
			MouseLeaveTimeBarCommand = new Command<MouseEventArgs>(MouseLeaveTimeBar);
			ToggleLabelsCommand = new Command(ToggleLabels);
			CanvasMouseMoveCommand = new Command<MouseEventArgs>(CanvasMouseMove);
			CanvasMouseLeftButtonDownCommand = new Command<MouseEventArgs>(CanvasMouseLeftButtonDown);
			CanvasMouseLeftButtonUpCommand = new Command<MouseEventArgs>(CanvasMouseLeftButtonUp);
		}
		
		#endregion

		#region Private Constants

		/// <summary>
		/// Clipboard format identifier for a polygon.
		/// </summary>
		const string PolygonClipboardFormat = "PolygonClipboardFormat";

		/// <summary>
		/// Clipboard format identifier for a line.
		/// </summary>
		const string LineClipboardFormat = "LineClipboardFormat";

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// Cursor applicable to the time bar.
		/// </summary>
		public Cursor TimeBarCusor
		{
			get { return GetValue<Cursor>(TimeBarCursorProperty); }
			set { SetValue(TimeBarCursorProperty, value); }
		}

		/// <summary>
		/// TimeBar property data.
		/// </summary>
		public static readonly PropertyData TimeBarCursorProperty = RegisterProperty(nameof(TimeBarCusor), typeof(Cursor));

		/// <summary>
		/// Cursor for the polygon/line drawing canvas.
		/// </summary>
		public Cursor CanvasCursor
		{
			get { return GetValue<Cursor>(CanvasCursorProperty); }
			set { SetValue(CanvasCursorProperty, value); }
		}

		/// <summary>
		/// TimeBar property data.
		/// </summary>
		public static readonly PropertyData CanvasCursorProperty = RegisterProperty(nameof(CanvasCursor), typeof(Cursor));

		/// <summary>
		/// Desired width of control.
		/// </summary>
		public int ControlWidth
		{
			get { return GetValue<int>(ControlWidthProperty); }
			set { SetValue(ControlWidthProperty, value); }
		}

		/// <summary>
		/// ControlWidth property data.
		/// </summary>
		public static readonly PropertyData ControlWidthProperty = RegisterProperty(nameof(ControlWidth), typeof(int));

		/// <summary>
		/// Desired height of control.
		/// </summary>
		public int ControlHeight
		{
			get { return GetValue<int>(ControlHeightProperty); }
			set { SetValue(ControlHeightProperty, value); }
		}

		/// <summary>
		/// WindowHeight property data.
		/// </summary>
		public static readonly PropertyData ControlHeightProperty = RegisterProperty(nameof(ControlHeight), typeof(int));

		/// <summary>
		/// Desired width of window.
		/// </summary>
		public int WindowWidth
		{
			get { return GetValue<int>(WindowWidthProperty); }
			set { SetValue(WindowWidthProperty, value); }
		}

		/// <summary>
		/// WindowWidth property data.
		/// </summary>
		public static readonly PropertyData WindowWidthProperty = RegisterProperty(nameof(WindowWidth), typeof(int));

		/// <summary>
		/// Desired height of window.
		/// </summary>
		public int WindowHeight
		{
			get { return GetValue<int>(WindowHeightProperty); }
			set { SetValue(WindowHeightProperty, value); }
		}

		/// <summary>
		/// WindowHeight property data.
		/// </summary>
		public static readonly PropertyData WindowHeightProperty = RegisterProperty(nameof(WindowHeight), typeof(int));

		/// <summary>
		/// Desired width of canvas.
		/// </summary>
		public int CanvasWidth
		{
			get { return GetValue<int>(CanvasWidthProperty); }
			set { SetValue(CanvasWidthProperty, value); }
		}

		/// <summary>
		/// CanvasWidth property data.
		/// </summary>
		public static readonly PropertyData CanvasWidthProperty = RegisterProperty(nameof(CanvasWidth), typeof(int));

		/// <summary>
		/// Desired height of canvas.
		/// </summary>
		public int CanvasHeight
		{
			get { return GetValue<int>(CanvasHeightProperty); }
			set { SetValue(CanvasHeightProperty, value); }
		}

		/// <summary>
		/// CanvaswHeight property data.
		/// </summary>
		public static readonly PropertyData CanvasHeightProperty = RegisterProperty(nameof(CanvasHeight), typeof(int));

		/// <summary>
		/// Gets or sets the view model's selected polygon.
		/// </summary>
		public PolygonViewModel SelectedPolygon
		{
			get { return GetValue<PolygonViewModel>(SelectedPolygonProperty); }
			private set
			{
				SetValue(SelectedPolygonProperty, value);
				SelectedShape = value;
			}
		}

		//  Why is the SelectedPolygon Catel?
		public PolygonSnapShotViewModel SelectedSnapShot { get; set; }

		/// <summary>
		/// SelectedPolygon property data.
		/// </summary>
		public static readonly PropertyData SelectedPolygonProperty = RegisterProperty(nameof(SelectedPolygon), typeof(PolygonViewModel));

		/// <summary>
		/// Gets or sets the view model's selected line.
		/// </summary>
		public LineViewModel SelectedLine
		{
			get { return GetValue<LineViewModel>(SelectedLineProperty); }
			private set
			{
				SetValue(SelectedLineProperty, value);
				SelectedShape = value;
			}
		}

		/// <summary>
		/// SelectedLine property data.
		/// </summary>
		public static readonly PropertyData SelectedLineProperty = RegisterProperty(nameof(SelectedLine), typeof(LineViewModel));

		public ShapeViewModel SelectedShape
		{
			get { return GetValue<ShapeViewModel>(SelectedShapeProperty); }
			private set { SetValue(SelectedShapeProperty, value); }
		}

		/// <summary>
		/// SelectedLine property data.
		/// </summary>
		public static readonly PropertyData SelectedShapeProperty = RegisterProperty(nameof(SelectedShape), typeof(ShapeViewModel));

		/// <summary>
		/// Gets or sets the previous point during a point move operation.
		/// </summary>
		public PolygonPointViewModel PreviousPointMoving
		{
			get { return GetValue<PolygonPointViewModel>(PreviousPointMovingProperty); }
			private set { SetValue(PreviousPointMovingProperty, value); }
		}

		/// <summary>
		/// PreviousPointMoving property data.
		/// </summary>
		public static readonly PropertyData PreviousPointMovingProperty = RegisterProperty(nameof(PreviousPointMoving), typeof(PolygonPointViewModel));

		/// <summary>
		/// Gets or sets the point being moved.
		/// </summary>
		public PolygonPointViewModel PointMoving
		{
			get { return GetValue<PolygonPointViewModel>(PointMovingProperty); }
			private set { SetValue(PointMovingProperty, value); }
		}

		/// <summary>
		/// PointMoving property data.
		/// </summary>
		public static readonly PropertyData PointMovingProperty = RegisterProperty(nameof(PointMoving), typeof(PolygonPointViewModel));

		/// <summary>
		/// Gets or sets the next point during a move operation.
		/// </summary>
		public PolygonPointViewModel NextPointMoving
		{
			get { return GetValue<PolygonPointViewModel>(NextPointMovingProperty); }
			private set { SetValue(NextPointMovingProperty, value); }
		}

		/// <summary>
		/// NextPointMoving property data.
		/// </summary>
		public static readonly PropertyData NextPointMovingProperty = RegisterProperty(nameof(NextPointMoving), typeof(PolygonPointViewModel));

		/// <summary>
		/// True when the moving ghost point is visible.
		/// </summary>
		public bool MovingPointVisibilityPrevious
		{
			get { return GetValue<bool>(MovingPointVisibilityPreviousProperty); }
			private set { SetValue(MovingPointVisibilityPreviousProperty, value); }
		}

		/// <summary>
		/// MovingPointVisibilityPrevious property data.
		/// </summary>
		public static readonly PropertyData MovingPointVisibilityPreviousProperty = RegisterProperty(nameof(MovingPointVisibilityPrevious), typeof(bool));

		/// <summary>
		/// True when the moving ghost point is visible.
		/// </summary>
		public bool MovingPointVisibilityNext
		{
			get { return GetValue<bool>(MovingPointVisibilityNextProperty); }
			private set { SetValue(MovingPointVisibilityNextProperty, value); }
		}

		/// <summary>
		/// MovingPointVisibilityNext property data.
		/// </summary>
		public static readonly PropertyData MovingPointVisibilityNextProperty = RegisterProperty(nameof(MovingPointVisibilityNext), typeof(bool));

		/// <summary>
		/// Collection of polygon snapshots.
		/// </summary>
		public ObservableCollection<PolygonSnapShotViewModel> PolygonSnapshots
		{
			get { return GetValue<ObservableCollection<PolygonSnapShotViewModel>>(PolygonSnapShotsProperty); }
			private set { SetValue(PolygonSnapShotsProperty, value); }
		}

		/// <summary>		
		/// PolygonSnapshots property data.
		/// </summary>
		public static readonly PropertyData PolygonSnapShotsProperty = RegisterProperty(nameof(PolygonSnapshots), typeof(ObservableCollection<PolygonSnapShotViewModel>));
		
		/// <summary>
		/// Draw polygon mode flag.
		/// </summary>
		public bool DrawPolygon
		{
			get { return GetValue<bool>(DrawPolygonProperty); }
			set 
			{
				if (value)
				{
					// Clear the other mode flags
					DrawLine = false;
					AddPoint = false;
					IsSelecting = false;

					// If there is not a new polygon then...
					if (NewPolygon == null)
					{
						// Create a new polygon
						AddNewPolygon();
						SelectedPolygon = NewPolygon;
					}

					// Deselect all the other polygons
					DeselectAllPolygons();

					// Remove the resize adorner
					RaiseRemoveResizeAdorner();
				}
				else
				{
					// If we were in the middle of creating a new polygon then...
					if (NewPolygon != null)
					{
						// Remove the partial polygon
						Polygons.Remove(NewPolygon);
						PolygonModels.Remove(NewPolygon.Polygon);
						NewPolygon = null;
						SelectedPolygon = null;
					}
				}
				SetValue(DrawPolygonProperty, value); 
			}
		}

		/// <summary>
		/// DrawPolygon property data.
		/// </summary>
		public static readonly PropertyData DrawPolygonProperty = RegisterProperty(nameof(DrawPolygon), typeof(bool));

		/// <summary>
		/// Draw line mode flag.
		/// </summary>
		public bool DrawLine
		{
			get { return GetValue<bool>(DrawLineProperty); }
			set 
			{
				if (value)
				{
					// Clear the other mode flags
					DrawPolygon = false;
					IsSelecting = false;
					AddPoint = false;

					// If there is not a new line then...
					if (NewLine == null)
					{
						// Create a new line
						AddNewLine();
						SelectedLine = NewLine;
					}
				}
				else
				{
					// If we were in the middle of creating a new line then...
					if (NewLine != null)
					{
						// Remove the partial line
						Lines.Remove(NewLine);
						LineModels.Remove(NewLine.Line);
						NewLine = null;
						SelectedLine = null;
					}					
				}

				SetValue(DrawLineProperty, value); 
			}
		}

		/// <summary>
		/// DrawLine property data.
		/// </summary>
		public static readonly PropertyData DrawLineProperty = RegisterProperty(nameof(DrawLine), typeof(bool));
		
		/// <summary>
		/// Add polygon point mode flag.
		/// </summary>
		public bool AddPoint
		{
			get { return GetValue<bool>(AddPointProperty); }
			set
			{
				if (value)
				{
					// Clear the other mode flags
					DrawLine = false;
					DrawPolygon = false;
					IsSelecting = false;					
				}				
				SetValue(AddPointProperty, value);
			}
		}

		/// <summary>
		/// AddPoint property data.
		/// </summary>
		public static readonly PropertyData AddPointProperty = RegisterProperty(nameof(AddPoint), typeof(bool));

		/// <summary>
		/// Selection mode flag.
		/// </summary>
		public bool IsSelecting
		{
			get { return GetValue<bool>(IsSelectingProperty); }
			set
			{
				if (value)
				{
					// Clear the other mode flags
					DrawPolygon = false;
					DrawLine = false;
					AddPoint = false;
				}
				SetValue(IsSelectingProperty, value);

				// Update the copy command availability
				((Command)CopyCommand).RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// IsSelecting property data.
		/// </summary>
		public static readonly PropertyData IsSelectingProperty = RegisterProperty(nameof(IsSelectingProperty), typeof(bool));

		/// <summary>
		/// 
		/// </summary>
		public bool TimeBarVisible
		{
			get { return GetValue<bool>(TimeBarVisibleProperty); }
			set { SetValue(TimeBarVisibleProperty, value); }
		}

		/// <summary>
		/// TimeBarVisible property data.
		/// </summary>
		public static readonly PropertyData TimeBarVisibleProperty = RegisterProperty(nameof(TimeBarVisible), typeof(bool), null);

		/// <summary>
		/// Width of time bar.
		/// </summary>
		public double TimeBarActualWidth
		{
			get { return GetValue<double>(TimeBarActualWidthProperty); }
			set
			{
				SetValue(TimeBarActualWidthProperty, value);
				UpdatePolygonSnapShots();
			}
		}

		/// <summary>
		/// TimeBarWidth property data.
		/// </summary>
		public static readonly PropertyData TimeBarActualWidthProperty = RegisterProperty(nameof(TimeBarActualWidth), typeof(double));

		/// <summary>
		/// Gets or sets whether the polygon/line labels should be shown.
		/// </summary>
		public bool ShowLabels
		{
			get { return GetValue<bool>(ShowLabelsProperty); }
			set { SetValue(ShowLabelsProperty, value); }
		}

		/// <summary>
		/// ShowLabels property data.
		/// </summary>
		public static readonly PropertyData ShowLabelsProperty = RegisterProperty(nameof(ShowLabels), typeof(bool));

		#endregion

		#region Public Properties

		/// <summary>
		/// Flag that indicates the user lassoing points.
		/// </summary>
		public bool LassoingPoints { get; set; }

		/// <summary>
		/// Stores the position of the mouse when the user left clicks.
		/// This field is used to draw the rubber band selection lasso. 
		/// </summary>
		public Point LassoMouseStartPoint { get; set; }

		/// <summary>
		/// Canvas for drawing polygons and lines.
		/// This property is necessary to retrieve the location of the mouse relative to this canvas.
		/// </summary>
		public IInputElement Canvas { get; set; }

		/// <summary>
		/// Defines a delegate type to determine if the mouse is over a polygon line.
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="polygonLineSegment">Polygon line segement the mouse is over</param>
		/// <param name="polygonParent">Polygon that owns the line</param>
		/// <returns></returns>
		public delegate bool IsMouseOverLineDelegate(Point mousePosition, ref PolygonLineSegment polygonLineSegment, ref PolygonViewModel polygonParent);

		/// <summary>
		/// Gets or sets the delegate to determine if the mouse is over a line.
		/// </summary>
		public IsMouseOverLineDelegate IsMouseOverLine { get; set; }
		
		/// <summary>
		/// Subtracts off a margin from the polygon snapshot time bar.
		/// </summary>
		public double AdjustedTimeBarActualWidth
		{
			get { return TimeBarActualWidth - 10.0; }
		}

		/// <summary>
		/// Returns true when the draw polygon and draw line toolbar buttons are visible.
		/// </summary>
		public bool ShowSelectDraw
		{
			get { return EditorCapabilities.AddPolygons; }
		}

		/// <summary>
		/// Returns true when the polygon add point toolbar button is visible.
		/// </summary>
		public bool ShowAddPoint
		{
			get { return EditorCapabilities.AddPoint; }
		}

		/// <summary>
		/// Returns true when the selection toolbar button is visible.
		/// </summary>
		public bool ShowSelect
		{
			get { return ShowSelectDraw; }
		}

		/// <summary>
		/// Returns true when the paste toolbar button is visible.
		/// </summary>
		public bool ShowPaste
		{
			get { return EditorCapabilities.AddPolygons; }
		}

		/// <summary>
		/// Returns true when the cut toolbar button is visible.
		/// </summary>
		public bool ShowCut
		{
			get { return EditorCapabilities.CutPolygons; }
		}

		/// <summary>
		/// Returns true when the copy toolbar button is visible.
		/// </summary>
		public bool ShowCopy
		{
			get { return EditorCapabilities.CopyPolygons; }
		}

		/// <summary>
		/// Returns true when the delete toolbar button is visible.
		/// </summary>
		public bool ShowDelete
		{
			get { return EditorCapabilities.DeletePolygons; }
		}

		/// <summary>
		/// Returns true when the toggle polygon start side toolbar button is visible.
		/// </summary>
		public bool ShowToggleStartSide
		{
			get { return EditorCapabilities.ToggleStartSide; }
		}

		/// <summary>
		/// Returns true when the toggle line start point toolbar button is visible.
		/// </summary>
		public bool ShowToggleStartPoint
		{
			get { return EditorCapabilities.ToggleStartPoint; }
		}

		private double _actualWidth;
		
		/// <summary>
		/// Width of canvas.
		/// </summary>
		public double ActualWidth 
		{ 
			get
			{
				return _actualWidth;
			}
			set
			{
				_actualWidth = value;

				// Give the grid validation rule the canvas width
				XValidationRule.Width = (int)value;
			}
		}

		private double _actualHeight;

		/// <summary>
		/// Height of canvas.
		/// </summary>
		public double ActualHeight 
		{ 
			get
			{
				return _actualHeight;
			}
			set
			{
				_actualHeight = value;

				// Give the grid validation rule the canvas height
				YValidationRule.Height = (int)value;
			}
		}
						
		/// <summary>
		/// Gets or sets the model polygons.
		/// </summary>
		public ObservableCollection<Polygon> PolygonModels { get; set; }
		
		/// <summary>
		/// Gets or sets the model lines.
		/// </summary>
		public ObservableCollection<Line> LineModels { get; set; }
		
		/// <summary>
		/// Gets or sets the view model polygons.
		/// </summary>
		public ObservableCollection<PolygonViewModel> Polygons
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the view model lines.
		/// </summary>
		public ObservableCollection<LineViewModel> Lines
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SelectedPoints.  Note the points could be on one or more polygons.
		/// </summary>
		public ObservableCollection<PolygonPointViewModel> SelectedPoints { get; private set; }

		/// <summary>
		/// Gets or sets whether a point is being moved.
		/// </summary>
		public bool MovingPoint { get; set; }

		/// <summary>
		/// Gets or sets whether a polygon snapshot is being moved.
		/// </summary>
		public bool MovingSnapShot { get; set; }

		private PolygonEditorCapabilities _editorCapabilities;

		/// <summary>
		/// Gets or sets the editor configuration settings.
		/// </summary>
		public PolygonEditorCapabilities EditorCapabilities
		{
			get
			{
				return _editorCapabilities;
			}
			set
			{
				// Store off the editor capabilities
				_editorCapabilities = value;

				// Configure whether the polygons should color the start side
				PolygonViewModel.ColorStartSide = !value.ShowTimeBar;

				// Initialze whether the snapshot bar is visible
				TimeBarVisible = _editorCapabilities.ShowTimeBar;

				// If the editor should start out in selection mode or
				// the time bar is visible then...
				if (_editorCapabilities.DefaultToSelect || TimeBarVisible)
				{
					// Put the editor in selection mode
					IsSelecting = true;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether the user selected a shape on the mouse down event.
		/// </summary>
		public bool SelectedShapeFlag { get; set; }

		#endregion

		#region Public Commands

		/// <summary>
		/// Copy polygon command.
		/// </summary>
		public ICommand CopyCommand { get; private set; }

		/// <summary>
		/// Paste polygon command.
		/// </summary>
		public ICommand PasteCommand { get; private set; }

		/// <summary>
		/// Delete polygon command.
		/// </summary>
		public ICommand DeleteCommand { get; private set; }
		
		/// <summary>
		/// Cut polygon command.
		/// </summary>
		public ICommand CutCommand { get; private set; }
		
		/// <summary>
		/// SnapToGrid command.  Snaps the polygon points to integer positions.
		/// </summary>
		public ICommand SnapToGridCommand { get; private set; }

		/// <summary>
		/// Ok button command.
		/// </summary>
		public ICommand OkCommand { get; private set; }

		/// <summary>
		/// Cancel button command.
		/// </summary>
		public ICommand CancelCommand { get; private set; }

		/// <summary>
		/// Delete polygon point command.
		/// </summary>
		public ICommand DeletePointCommand { get; private set; }

		/// <summary>
		/// Toggle polygon start side command.
		/// </summary>
		public ICommand ToggleStartSideCommand { get; private set; }

		/// <summary>
		/// Toggle line start point command.
		/// </summary>
		public ICommand ToggleStartPointCommand { get; private set; }

		/// <summary>
		/// Converts the polygon to a line command.
		/// </summary>
		public ICommand PolygonToLineCommand { get; private set; }

		/// <summary>
		/// Converts the line to a polygon command.
		/// </summary>
		public ICommand LineToPolygonCommand { get; private set; }

		/// <summary>
		/// Adds a polygon snapshot command.
		/// </summary>
		public ICommand AddPolygonSnapShotCommand { get; private set; }

		/// <summary>
		/// Delete polygon snapshot command.
		/// </summary>
		public ICommand DeletePolygonSnapShotCommand { get; private set; }

		/// <summary>
		/// Selects the next polygon snapshot command.
		/// </summary>
		public ICommand NextPolygonSnapShotCommand { get; private set; }

		/// <summary>
		/// Selects the previous polygon snapshot command.
		/// </summary>
		public ICommand PreviousPolygonSnapShotCommand { get; private set; }
		
		/// <summary>
		/// Add a polygon point command.
		/// </summary>
		public ICommand AddPointCommand { get; private set; }

		/// <summary>
		/// Draw a new polygon command.
		/// </summary>
		public ICommand DrawPolygonCommand { get; private set; }

		/// <summary>
		/// Draw a new line command.
		/// </summary>
		public ICommand DrawLineCommand { get; private set; }
		
		/// <summary>
		/// Move snapshot command.
		/// </summary>
		public ICommand MoveSnapshotCommand { get; private set; }

		/// <summary>
		/// Time bar mouse left button down command.
		/// </summary>
		public ICommand MouseLeftButtonDownTimeBarCommand { get; private set; }

		/// <summary>
		/// Time bar mouse left button up command.
		/// </summary>
		public ICommand MouseLeftButtonUpTimeBarCommand { get; private set; }

		/// <summary>
		/// Time bar mouse leave command.
		/// </summary>
		public ICommand MouseLeaveTimeBarCommand { get; private set; }

		/// <summary>
		/// Toggles Polygon/Line Labels On/Off.
		/// </summary>
		public ICommand ToggleLabelsCommand { get; private set; }

		/// <summary>
		/// Mouse move over the polygon/line canvas command.
		/// </summary>
		public ICommand CanvasMouseMoveCommand { get; private set; }

		/// <summary>
		/// Mouse left button down polygon/line canvas command.
		/// </summary>
		public ICommand CanvasMouseLeftButtonDownCommand { get; private set; }

		/// <summary>
		/// Mouse left button up polygon/line canvas command.
		/// </summary>
		public ICommand CanvasMouseLeftButtonUpCommand { get; private set; }

		#endregion

		#region Public Mouse Over Methods

		/// <summary>
		/// Returns true if the mouse is over a moveable item.
		/// </summary>
		/// <param name="position">Position of mouse</param>
		/// <returns>True if the mouse is over a moveable item</returns>
		public bool IsMouseOverTimeBar(Point position, ref PolygonSnapShotViewModel snapShot)
		{
			// Default to NOT over a snapshot
			bool overTimeBar = false;
			
			// Loop over the polygon snapshots
			foreach (PolygonSnapShotViewModel polygonSnapShot in PolygonSnapshots)
			{
				// If the mouse is over the polygon snapshot then...
				if (polygonSnapShot.IsMouseOverTimeBar(position))
				{
					// Save off the snapshot
					snapShot = polygonSnapShot;

					// Indicate we are over a snapshot
					overTimeBar = true;

					// Break out of foreach loop
					break;
				}
			}

			return overTimeBar;
		}

		/// <summary>
		/// Returns true if the mouse is over a moveable item.
		/// </summary>
		/// <param name="position">Position of mouse</param>
		/// <returns>True if the mouse is over a moveable item</returns>
		public bool IsMouseOverMoveableItem(Point position)
		{
			// Default to not being over a moveable item
			bool mouseOverMoveableItem = false;

			// Loop over all the polygons
			foreach (PolygonViewModel poly in Polygons)
			{				
				PolygonViewModel selectedPolygon = null;
				PolygonPointViewModel selectedPolygonPoint = null;
				
				// If the mouse is over a polygon point or
				// it is over the center cross hash
				if (IsMouseOverPolygonPoint(position, ref selectedPolygonPoint, ref selectedPolygon) ||
					IsMouseOverPolygonCenterCrossHash(position, ref selectedPolygon))
				{
					// Indicate the mouse is over a moveable item and break out of loop
					mouseOverMoveableItem = true;
					break;
				}
			}

			// If the mouse is not over a moveable item then check the lines
			if (!mouseOverMoveableItem)
			{
				// Loop over all the lines
				foreach (LineViewModel line in Lines)
				{
					LineViewModel selectedLine = null;
					PolygonPointViewModel selectedPoint = null;

					if (IsMouseOverLinePoint(position, ref selectedPoint, ref selectedLine) ||
						IsMouseOverLineCenterCrossHash(position, ref selectedLine))
					{
						// Indicate the mouse is over a moveable item and break out of loop
						mouseOverMoveableItem = true;
						break;
					}
				}
			}

			return mouseOverMoveableItem;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Selects the specified polygon snapshot.
		/// </summary>
		/// <param name="snapShot">Polygon snapshot to select</param>
		public void SelectPolygonSnapShot(PolygonSnapShotViewModel snapShot)
		{
			// If a selected snapshot exists then...
			if (SelectedSnapShot != null)
			{
				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Mark the previous snapshot as NOT selected
				SelectedSnapShot.Selected = false;
			}
			
			// Mark the specified polygon as selected
			snapShot.Selected = true;
			SelectedSnapShot = snapShot;

			// Clear all the containers the editor binds to
			Polygons.Clear();
			PolygonModels.Clear();
			Lines.Clear();
			LineModels.Clear();

			// If the selected snapshot has a polygon then...
			if (SelectedSnapShot.PolygonViewModel != null)
			{
				// Add the polygon to the editor collections
				PolygonModels.Add(SelectedSnapShot.PolygonViewModel.Polygon);
				Polygons.Add(SelectedSnapShot.PolygonViewModel);

				// Select the polygon
				SelectPolygon(SelectedSnapShot.PolygonViewModel, true);

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			// Otherwise if the selected snapshot has a line then...
			else if (SelectedSnapShot.LineViewModel != null)
			{
				// Add the line to the editor collections
				LineModels.Add(SelectedSnapShot.LineViewModel.Line);
				Lines.Add(SelectedSnapShot.LineViewModel);

				// Select the line
				SelectLine(SelectedSnapShot.LineViewModel, true);

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			else
			{
				// Clear out the selected line and polygon
				SelectedLine = null;
				SelectedPolygon = null;
			}

			// Force the editor to refresh
			RaisePropertyChanged("Polygons");
			RaisePropertyChanged("Lines");

			// Force the toolbar commands to refresh
			((Command)DeletePolygonSnapShotCommand).RaiseCanExecuteChanged();
			((Command)AddPointCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="polygon">Polygon to select</param>
		public void SelectPolygon(PolygonViewModel polygon, bool updateSelectedPoints)
		{
			// Select the polygon and all its points
			SelectAllPointsOnPolygon(polygon);

			// Clear out the selected line
			SelectedLine = null;

			// Store off the selected polygon
			SelectedPolygon = polygon;

			// If the selected points need to be updated then...
			if (updateSelectedPoints)
			{
				// Clear out the selected points
				SelectedPoints.Clear();

				// Select all the points on the polygon
				SelectedPoints.AddRange(SelectedPolygon.PointCollection);
			}

			// Since the selected polygon changed update the commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)CutCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Selects the specified line.
		/// </summary>
		/// <param name="line">Line to select</param>
		public void SelectLine(LineViewModel line, bool updateSelectedPoints)
		{
			// Select the line and all its points
			SelectAllPointsOnLine(line);

			// Clear out the selected polygon
			SelectedPolygon = null;

			// Store off the selected line
			SelectedLine = line;

			// If the selected points need to be updated then...
			if (updateSelectedPoints)
			{
				// Clear out the selected points
				SelectedPoints.Clear();

				// Select all the points on the line
				SelectedPoints.AddRange(SelectedLine.PointCollection);
			}

			// Since the selected polygon changed update the commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)CutCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartPointCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Displays the resize adorner for the selected shape.
		/// </summary>
		public void DisplayResizeAdornerForSelectedShape()
		{
			// If there is selected polygon with points then...
			if (SelectedPolygon != null &&
				SelectedPolygon.PointCollection.Count > 0)
			{
				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			else if (SelectedLine != null &&
					 SelectedLine.PointCollection.Count == 2)
			{
				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
		}

		#endregion

		#region Private Mouse Over Methods		

		/// <summary>
		/// Returns true when the mouse is over the polygon's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="polygon">Polygon the mouse is over or NULL</param>
		/// <returns>True when the mouse is over the polygon's center cross hash</returns>
		private bool IsMouseOverPolygonCenterCrossHash(Point mousePosition, ref PolygonViewModel crossHashPolygon)
		{
			// Default to not being over the center of a polygon
			ShapeViewModel crossHashShape = null;

			// Default to not being over the center cross hash
			bool mouseOverCenterCrossHash = IsMouseOverShapeCenterCrossHash(
				mousePosition,
				ref crossHashShape,
				Polygons.Cast<ShapeViewModel>().ToList());

			// If the mouse was over a polygon return the polygon reference
			crossHashPolygon = crossHashShape as PolygonViewModel;

			return mouseOverCenterCrossHash;			
		}

		/// <summary>
		/// Returns true when the mouse is over the line's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="polygon">Polygon the mouse is over or NULL</param>
		/// <returns>True when the mouse is over he line's center cross hash.</returns>
		private bool IsMouseOverLineCenterCrossHash(Point mousePosition, ref LineViewModel crossHashLine)
		{
			// Default to not being over the center of a line
			ShapeViewModel crossHashShape = null;

			// Default to not being over the center cross hash
			bool mouseOverCenterCrossHash = IsMouseOverShapeCenterCrossHash(
				mousePosition,
				ref crossHashShape,
				Lines.Cast<ShapeViewModel>().ToList());

			// If the mouse was over a line return the line reference
			crossHashLine = crossHashShape as LineViewModel;
			
			return mouseOverCenterCrossHash;
		}

		/// <summary>
		/// Returns true when the mouse is over one of the shape's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="shape">Shape the mouse is over or NULL</param>
		/// <returns>True if the mouse is over a shape's point</returns>
		private bool IsMouseOverShapeCenterCrossHash(
			Point mousePosition, 
			ref ShapeViewModel crossHashLine,
			List<ShapeViewModel> shapes)
		{
			// Default to not being over the center cross hash
			crossHashLine = null;

			// Default to not being over the center cross hash
			bool mouseOverCenterCrossHash = false;

			// Loop over the shape view models
			foreach (ShapeViewModel shape in shapes)
			{
				// If the mouse is over the center cross hash then...
				if (shape.IsOverCenterCrossHash(mousePosition))
				{
					// Set shape the mouse is over
					crossHashLine = shape;

					// Indicate that the mouse is over the cross hash
					mouseOverCenterCrossHash = true;

					break;
				}
			}

			return mouseOverCenterCrossHash;
		}


		/// <summary>
		/// Returns true if the mouse is over polygon point.
		/// </summary>
		/// <param name="mousePosition">Position of mouse</param>
		/// <param name="pointUnderMouse"></param>
		/// <param name="polygonUnderMouse"></param>
		/// <returns>True if the mouse is over a polygon point</returns>
		private bool IsMouseOverPolygonPoint(
			Point mousePosition, 
			ref PolygonPointViewModel pointUnderMouse, 
			ref PolygonViewModel polygonUnderMouse)
		{
			ShapeViewModel shapeUnderMouse = null;			
			bool isMouseOverLinePoint = IsMouseOverShapePoint(
				mousePosition,
				ref pointUnderMouse,
				ref shapeUnderMouse,
				Polygons.Cast<ShapeViewModel>().ToList());

			// If the mouse was over a polygon return the polygon reference
			polygonUnderMouse = shapeUnderMouse as PolygonViewModel;

			return isMouseOverLinePoint;
		}

		/// <summary>
		/// Returns true if the mouse is over a line end point.
		/// </summary>
		/// <param name="mousePosition">Position of mouse</param>
		/// <param name="pointUnderMouse">Point under mouse</param>
		/// <param name="polygonUnderMouse">Line the point belongs too</param>
		/// <returns>True if the mouse over a line end point</returns>
		private bool IsMouseOverLinePoint(
			Point mousePosition,
			ref PolygonPointViewModel pointUnderMouse,
			ref LineViewModel lineUnderMouse)
		{			
			// Determine if the mouse is over a shape
			ShapeViewModel shapeUnderMouse = null;
			bool isMouseOverLinePoint = IsMouseOverShapePoint(
				mousePosition,
				ref pointUnderMouse,
				ref shapeUnderMouse,
				Lines.Cast<ShapeViewModel>().ToList());

			// If the mouse was over a line return the line reference
			lineUnderMouse = shapeUnderMouse as LineViewModel;

			return isMouseOverLinePoint;
		}

		/// <summary>
		/// Returns true if the mouse is over a shape's point.
		/// </summary>
		/// <param name="mousePosition">Position of mouse</param>
		/// <param name="pointUnderMouse">Point under mouse</param>
		/// <param name="shapeUnderMouse">Shape the point belongs too</param>
		/// /// <param name="shapes">Shapes to test</param>
		/// <returns>True if the mouse over a line</returns>		
		private bool IsMouseOverShapePoint(
			Point mousePosition,
			ref PolygonPointViewModel pointUnderMouse,
			ref ShapeViewModel shapeUnderMouse,
			List<ShapeViewModel> shapes)
		{
			// Default to the mouse not being over a shape point
			bool mouseOverPoint = false;

			// Loop over the shape view models
			foreach (ShapeViewModel line in shapes)
			{
				// Loop over the points on the shape
				foreach (PolygonPointViewModel point in line.PointCollection)
				{
					// Calculate the distance from the point
					double deltaX = Math.Abs(point.X - mousePosition.X);
					double deltaY = Math.Abs(point.Y - mousePosition.Y);

					const int MouseTolerance = 5;

					// If the mouse is within the tolerance of the point then...
					if (deltaX <= MouseTolerance && deltaY <= MouseTolerance)
					{
						// Indicate the mouse is over a point
						mouseOverPoint = true;

						// Return the point and the shape the mouse is over
						pointUnderMouse = point;
						shapeUnderMouse = line;

						// Break out of loop
						break;
					}
				}

				// If the mouse is over a line point then...
				if (mouseOverPoint)
				{
					// Break out of loop
					break;
				}
			}

			return mouseOverPoint;
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Polygon that is in the process of getting created.
		/// </summary>
		private PolygonViewModel NewPolygon { get; set; }
		
		/// <summary>
		/// Line that is in the process of getting created.
		/// </summary>
		private LineViewModel NewLine { get; set; }

		#endregion

		#region Private Methods
		
		/// <summary>
		/// Handles the polygon/line canvas left button down command.
		/// </summary>
		/// <param name="args"></param>
		private void CanvasMouseLeftButtonDown(MouseEventArgs args)
		{
			// Get the mouse click position
			Point clickPosition = args.GetPosition(Canvas);

			// Store off the mouse click position
			LassoMouseStartPoint = clickPosition;

			// Default to not selecting a shape
			SelectedShapeFlag = false;

			// If the editor is not in selection mode then...
			if (!IsSelecting)
			{
				// If we are adding points to existing polygons then...
				if (AddPoint)
				{
					PolygonLineSegment lineSegment = null;
					PolygonViewModel polygon = null;

					// If the mouse is over a polygon line segment then...
					if (IsMouseOverLine(clickPosition, ref lineSegment, ref polygon))
					{
						// If the polygon is closed then the user must want to add a new point along the line segment
						if (polygon.PolygonClosed)
						{
							// Find the index of the start point of the line segment
							int index = polygon.PointCollection.IndexOf(lineSegment.StartPoint);

							// Insert the point into the point collection
							polygon.InsertPoint(clickPosition, index + 1);
						}
					}
				}
				// If the editor is in draw polygon mode then...
				else if (DrawPolygon)
				{
					// Add a polygon point at the click position
					AddPolygonPoint(clickPosition);
				}
				// Otherwise if we are draw line mode then...
				else if (DrawLine)
				{
					AddLinePoint(clickPosition);
				}
			}
			else // Editor is in Select mode
			{
				Canvas.CaptureMouse();

				// Attempt to select either a polygon, line or point
				SelectedShapeFlag = SelectPolygonLineOrPoint(clickPosition);
			}

			// Remove the resize adorner
			RaiseRemoveResizeAdorner(false);
			
			// If the editor is in selection mode and
			// there is a selected polygon with all points selected then...
			if (IsSelecting &&
				SelectedPolygon != null &&
				SelectedPolygon.AllPointsSelected)
			{
				// Select all the points on the polygon
				SelectPolygonPoints();

				// Display the resize adorner for the selected polygon
				RaiseDisplayResizeAdorner();
			}
			// If the editor is in selection mode and
			// there is a selected line with all points selected then...
			else if (IsSelecting &&
					 SelectedLine != null &&
					 SelectedLine.AllPointsSelected)
			{
				// Display the resize adorner for the selected line
				RaiseDisplayResizeAdorner();
			}
		}

		/// <summary>
		/// Canvas left mouse up event handler.
		/// </summary>        
		private void CanvasMouseLeftButtonUp(MouseEventArgs args)
		{
			// Position of the mouse when the button was released
			Point clickPosition = args.GetPosition(Canvas);

			// Release the mouse
			Canvas.ReleaseMouseCapture();

			// If the user is moving a polygon point then...
			if (MovingPoint)
			{
				// End the point move
				EndMoveSelectedPoint(clickPosition);
			}

			// If the user did not select a shape on the mouse down event then...
			if (!SelectedShapeFlag)
			{
				// If the rubber band adorner was created then...
				if (LassoingPoints)
				{
					// Retrieve the points of the rubbber band
					Point endPoint = clickPosition; 
					Point startPoint = LassoMouseStartPoint;

					// Remove the rubber band adorner
					RaiseRemoveRubberBandAdorner();

					// Release the mouse
					Canvas.ReleaseMouseCapture();

					// Clears the selected points
					ClearSelectedPoints();

					// Create a rectangle from the rubber band
					Rect lasso = new Rect(startPoint, endPoint);

					// Select the polygon points inside the lasso
					SelectShapePoints(lasso);

					// If more than one polygon point is selected then...
					if (SelectedPoints.Count > 1)
					{
						// If an entire polygon was captured in the lasso then...
						PolygonViewModel selectedPolygon = null;
						if (IsEntirePolygonSelected(SelectedPoints, ref selectedPolygon))
						{
							// Select the polygon
							SelectPolygon(selectedPolygon, false);
						}

						// Update the resize adorner for the selected points
						RaiseDisplayResizeAdornerForSelectedPoints();
					}
					// If only one point is selected then...
					else if (SelectedPoints.Count == 1)
					{
						// Find the polygon associated with the selected polygon point
						PolygonViewModel selectedPolygon = FindPolygon(SelectedPoints[0]);

						if (selectedPolygon != null)
						{
							// Select the polygon point                        
							SelectPolygonPoint(SelectedPoints[0], selectedPolygon);
						}
						else
						{
							// Find the line associated with the selected point
							LineViewModel selectedLine = FindLine(SelectedPoints[0]);

							if (selectedLine != null)
							{
								// Select the line point
								SelectLinePoint(SelectedPoints[0], selectedLine);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Creates a new polygon snapshot at the specified normalized time and the specified index.
		/// </summary>
		/// <param name="normalizedTime">Normalized time of the snapshot</param>
		/// <param name="index">Index into the collection of snapshots</param>
		/// <returns>Reference to the new snapshot</returns>
		private PolygonSnapShotViewModel CreatePolygonSnapShot(double normalizedTime, int index)
		{
			// Create the new snapshot
			PolygonSnapShotViewModel snapshot = new PolygonSnapShotViewModel();
			
			// Set the normalized time of the snapshot
			snapshot.NormalizedTime = normalizedTime;
			
			// Convert the normalized time into a pixel based position
			snapshot.Initialize((int)(normalizedTime * AdjustedTimeBarActualWidth) + PolygonSnapShotViewModel.HalfWidth);
			
			// Insert the new snapshot into the collection
			PolygonSnapshots.Insert(index, snapshot);
			
			// Return the newly created snapshot
			return snapshot;
		}

		/// <summary>
		/// Refreshes all the shape point collections.
		/// </summary>
		private void RefreshAllShapes()
		{
			// Loop over the polygon view models
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Fire the property changed event so that the converters run
				polygon.NotifyPointCollectionChanged();
			}

			// Loop over the line view models
			foreach (LineViewModel line in Lines)
			{
				// Fire the property changed event so that the converters run
				line.NotifyPointCollectionChanged();
			}
		}
		
		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="polygon">Polygon to select points on</param>
		private void SelectAllPointsOnPolygon(PolygonViewModel polygon)
		{
			// Store off the selected polygon
			SelectedPolygon = polygon;
			
			// Select all the points on the polygon and the center hash
			SelectedPolygon.SelectPolygon();
		}

		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="polygon"></param>
		private void SelectAllPointsOnLine(LineViewModel line)
		{
			// Store off the selected line
			SelectedLine = line;

			// Select all the points on the polygon and the center hash
			SelectedLine.SelectLine();
		}
				
		/// <summary>
		/// Creates a new polygon.
		/// </summary>
		private void AddNewPolygon()
		{
			// Create a new model polygon
			Polygon polygon = new Polygon();

			// Create a new view model polygon
			NewPolygon = new PolygonViewModel(polygon);

			// Initialize the new polygon as the SelectedPolygon
			SelectedPolygon = NewPolygon;

			// Save off the model polygon
			PolygonModels.Add(polygon);

			// Save off the view model polygon
			Polygons.Add(NewPolygon);
		}

		/// <summary>
		/// Creates a new line.
		/// </summary>
		private void AddNewLine()
		{
			// Create a new Line model
			Line line = new Line();

			// Create a new Line view model
			NewLine = new LineViewModel(line);

			// Initialize the new line as the SelectedLine
			SelectedLine = NewLine;
			
			// Save off the model line
			LineModels.Add(line);

			// Save off the view model polygon
			Lines.Add(NewLine);
		}
	
		/// <summary>
		/// Deselects the current selected polygon.
		/// </summary>
		private void DeselectShapes()
		{
			// If a polygon was previously selected then...
			if (SelectedPolygon != null)
			{
				// Deselect all points on the polygon
				SelectedPolygon.DeselectAllPoints();
			}

			// Clear out the selected polygon
			SelectedPolygon = null;

			// If a line was previously selected then...
			if (SelectedLine != null)
			{
				// Deselect all points on the line
				SelectedLine.DeselectAllPoints();
			}

			// Clear out the selected line
			SelectedLine = null;
			
			// Since there is no longer a selected polygon or line update the commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartPointCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Fires an event to remove the resize adorner.
		/// </summary>
		private void RaiseRemoveResizeAdorner(bool deselectAllShapes = true)
		{
			// If the caller wants to deselect all shapes then...
			if (deselectAllShapes)
			{
				// Deselect all shapes
				DeselectAllShapes();
			}

			EventHandler handler = RemoveResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Fires an event to display the resize adorner.
		/// </summary>
		private void RaiseDisplayResizeAdorner()
		{
			EventHandler handler = DisplayResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Fires an event to display the resize adorner for the selected points.
		/// Note these points could be from more than one shape.
		/// </summary>
		private void RaiseDisplayResizeAdornerForSelectedPoints()
		{
			EventHandler handler = DisplayResizeAdornerForSelectedPoints;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Fires an eent to display the rubber band (lasso) adorner.
		/// </summary>
		private void RaiseRemoveRubberBandAdorner()
		{
			// Remember that we are no longer lassoing points
			LassoingPoints = false;

			EventHandler handler = RemoveRubberBandAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Sorts the snapshots by the associated time.
		/// </summary>
		/// <typeparam name="T">Type of shape</typeparam>
		/// <param name="shapes">Colletion of shapes to sort</param>
		/// <param name="times">Times associated with the shapes</param>
		private void SortSnapshots<T>(IList<T> shapes, IList<double> times)
				where T : Shape
		{
			// Create a data structure to associate the shape with its normalized time
			List<Tuple<T, double>> timedLines = new List<Tuple<T, double>>();

			// Loop over the shapes
			for (int index = 0; index < shapes.Count; index++)
			{
				// Create a shape/time tuple
				timedLines.Add(new Tuple<T, double>(shapes[index], times[index]));
			}

			// Sort the tuples by the time
			List<Tuple<T, double>> sortedShapes = timedLines.OrderBy(shapeTuple => shapeTuple.Item2).ToList();

			// Break down the tuples into the shapes and the times
			shapes = sortedShapes.Select(tuple => tuple.Item1).ToList();
			times = sortedShapes.Select(tuple => tuple.Item2).ToList();
		}
		/// <summary>
		/// Ensures the specified point is on the editor canvas.
		/// </summary>
		/// <param name="position">Position of point</param>
		/// <returns>Limited point</returns>
		private Point LimitPointToCanvas(Point position)
		{
			// If the X coordinate is off the canvas to the right then...
			if (position.X > ActualWidth - 1)
			{
				// Reset the X coordinate to the edge of the canvas
				position.X = ActualWidth - 1;
			}

			// If the X coordinate is off the canvas to the left then...
			if (position.X < 0)
			{
				// Reset the X coordinate to the edge of the canvas
				position.X = 0;
			}

			// If the Y coordinate is off the canvas then...
			if (position.Y > ActualHeight - 1)
			{
				// Reest the Y coordinate to the edge of the canvas
				position.Y = ActualHeight - 1;
			}

			// If the Y coordinates is off the canvas then...	
			if (position.Y < 0)
			{
				// Reest the Y coordinate to the edge of the canvas
				position.Y = 0;
			}

			// Return the limited point
			return position;
		}

		/// <summary>
		/// Ensure the point is on the drawing canvas.
		/// </summary>
		/// <param name="position">Polygon point to update</param>
		private void LimitPointToCanvas(PolygonPointViewModel position)
		{
			// Get the limited .NET point
			Point limitedPoint = LimitPointToCanvas(position.GetPoint());

			// Update the view model point
			position.X = limitedPoint.X;
			position.Y = limitedPoint.Y;			
		}

		#endregion

		#region Private Command Methods

		/// <summary>
		/// Puts the editor into draw line mode.
		/// </summary>
		private void DrawLineAction()
		{
			// Set flag indicating we are drawing a line
			DrawLine = true;
		}

		/// <summary>
		/// Puts the editor into draw polygon mode.
		/// </summary>
		private void DrawPolygonAction()
		{
			// Set a flag indicating we are drawing a polygon
			DrawPolygon = true;
		}

		/// <summary>
		/// Puts the editor into add polygon point mode.
		/// </summary>
		private void AddPolygonPoint()
		{
			// Set a flag indicating we are in add polygon point mode
			AddPoint = true;
		}

		/// <summary>
		/// Returns true when the draw a new shape is enabled.
		/// </summary>
		/// <returns>True when the draw a new shape is enabled.</returns>
		private bool IsDrawShapeEnabled()
		{
			// Default to the draw shape being available
			bool isEnabled = true;

			// If in time based mode then...
			if (TimeBarVisible)
			{
				// Can only draw a new shape if the previous shape has been deleted
				isEnabled = SelectedSnapShot != null &&
							SelectedSnapShot.PolygonViewModel == null &&
							SelectedSnapShot.LineViewModel == null;
			}

			return isEnabled;
		}

		/// <summary>
		/// Returns true when the add polygon point is enabled.
		/// </summary>
		/// <returns>Returns true when the add polygon point is enabled</returns>
		private bool IsAddPolygonPointEnabled()
		{
			// Adding polygon points is only valid after the selected polygon is completed (closed).
			return SelectedPolygon != null &&
				   SelectedPolygon.PolygonClosed;
		}

		/// <summary>		
		/// Method to invoke when the Ok command is executed.
		/// </summary>
		private void OK()
		{
			// Round the points the nearest pixel
			SnapToGrid();

			// If there are any polygons then...
			if (Polygons.Any())
			{
				// If the last polygon is not complete then...
				if (!Polygons.Last().PolygonClosed)
				{
					// Remove the incomplete polygon
					Polygons.Remove(Polygons.Last());
					PolygonModels.Remove(PolygonModels.Last());
				}
			}

			// If there are any lines then...
			if (Lines.Any())
			{
				// If the last line is not complete then...
				if (Lines.Last().PointCollection.Count != 2)
				{
					// Remove the incomplete line
					Lines.Remove(Lines.Last());
					LineModels.Remove(LineModels.Last());
				}
			}

			// Call Catel processing
			this.SaveAndCloseViewModelAsync();			
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private void Cancel()
		{
			// Call Catel processing
			this.CancelAndCloseViewModelAsync();
		}

		/// <summary>
		/// Snaps the polygon points to pixel positions.  The editor resolution is not the same as the display element.  
		/// </summary>
		private void SnapToGrid()
		{
			// If the width and height have been intialized then...
			// TODO: Find a better way to do this than mouse enter
			if (ActualWidth != 0 && ActualHeight != 0)
			{
				// Loop over the polygons
				foreach (PolygonViewModel polygon in Polygons)
				{
					// Snap the polygon to the display element
					polygon.SnapToGrid(ActualWidth, ActualHeight);

					// Raise the property change events for the points to the converters run
					polygon.NotifyPointCollectionChanged();
				}

				// Loop over the lines
				foreach (LineViewModel line in Lines)
				{
					// Snap the polygon to the display element
					line.SnapToGrid(ActualWidth, ActualHeight);

					// Raise the property change events for the points to the converters run
					line.NotifyPointCollectionChanged();
				}
			}
		}

		/// <summary>
		/// Deletes the selected polygon/line.
		/// </summary>
		private void Delete()
		{
			// If there is a selected polygon then...
			if (SelectedPolygon != null)
			{
				// Remove the specified polygon
				Polygons.Remove(SelectedPolygon);
				PolygonModels.Remove(SelectedPolygon.Polygon);

				// Clear out the selected polygon
				SelectedPolygon = null;
			}

			// If there is a selected line then...
			if (SelectedLine != null)
			{
				// Remove the specified line
				Lines.Remove(SelectedLine);
				LineModels.Remove(SelectedLine.Line);

				// Clear out the selected line
				SelectedPolygon = null;
			}

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();
			
			// If we are in Time-Based mode then...
			if (TimeBarVisible)
			{
				// Remove the polygon from the snap-shot
				SelectedSnapShot.PolygonViewModel = null;				
			}

			// Update the enable status of the copy, cut and delete commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)CutCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartPointCommand).RaiseCanExecuteChanged();
			((Command)AddPointCommand).RaiseCanExecuteChanged();
			((Command)DrawPolygonCommand).RaiseCanExecuteChanged();
			((Command)DrawLineCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Deletes the selected polygon point.
		/// </summary>
		private void DeletePoint()
		{			
			// If we can delete a polygon point then...
			if (SelectedPolygon != null &&
				SelectedPoints.Count == 1 &&
				SelectedPolygon.DeletePointCommand.CanExecute(null))
			{
				// Execute the command on the view model
				SelectedPolygon.DeletePointCommand.Execute(null);		
			}			
		}

		/// <summary>
		/// Turns/on off the polygon/line labels.
		/// </summary>
		private void ToggleLabels()
		{
			// Toggle the toolbar button
			ShowLabels = !ShowLabels;

			// Loop over all the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Toggle the visibility of the labels
				polygon.LabelVisible = !polygon.LabelVisible;
			}

			// Loop over all the lines
			foreach (LineViewModel line in Lines)
			{
				// Toggle the visibility of the labels
				line.LabelVisible = !line.LabelVisible;
			}
		}

		/// <summary>
		/// Toggles the start point of a line.
		/// </summary>
		private void ToggleStartPoint()
		{
			// Forward the request on to the line
			SelectedLine.ToggleStartPoint();

			// Deselect all shapes
			DeselectShapes();

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();
		}

		/// <summary>
		/// Toggles the line into a polygon.
		/// </summary>
		private void ToggleLineToPolygon()
		{
			// Clear the line collections
			Lines.Clear();
			LineModels.Clear();

			// Create a new polygon
			Polygon polygon = new Polygon();
			PolygonModels.Add(polygon);
			Polygons.Add(new PolygonViewModel(polygon));

			// Configure the polygon to be the size of the display element
			Polygons[0].AddPoint(new Point(0, 0));
			Polygons[0].AddPoint(new Point(ActualWidth - 1, 0));
			Polygons[0].AddPoint(new Point(ActualWidth - 1 , ActualHeight - 1));
			Polygons[0].AddPoint(new Point(0, ActualHeight - 1));
			Polygons[0].AddPoint(new Point(0, 0));

			// If the time bar is visible then...
			if (TimeBarVisible)
			{
				// Associate the polygon with the snapshot
				SelectedSnapShot.PolygonViewModel = Polygons[0];
				SelectedSnapShot.LineViewModel = null;

				// Select the snapshot
				SelectPolygonSnapShot(SelectedSnapShot);
			}

			// Deselect all shapes and remove the resize adorner
			DeselectShapes();
			RaiseRemoveResizeAdorner();

			// Refresh the toolbar buttons
			((Command)PolygonToLineCommand).RaiseCanExecuteChanged();
			((Command)LineToPolygonCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Toggles the polygon into a line.
		/// </summary>
		private void TogglePolygonToLine()
		{
			// Clear the polygon collections
			Polygons.Clear();
			PolygonModels.Clear();

			// Create a new line
			Line line = new Line();
			LineModels.Add(line);
			Lines.Add(new LineViewModel(line));

			// Initialize the line to be on the left of the display element extending the length of the display element
			PolygonPoint pt1 = new PolygonPoint();
			PolygonPoint pt2 = new PolygonPoint();
			line.Points.Add(pt1);
			line.Points.Add(pt2);
			Lines[0].StartPoint = new PolygonPointViewModel(pt1, null);
			Lines[0].EndPoint = new PolygonPointViewModel(pt2, null);
			Lines[0].StartPoint.X = ActualWidth / 2.0;
			Lines[0].StartPoint.Y = ActualHeight * 0.2 - 1;
			Lines[0].EndPoint.X = ActualWidth / 2.0;
			Lines[0].EndPoint.Y = ActualHeight * 0.8 - 1;
			Lines[0].PointCollection.Add(Lines[0].StartPoint);
			Lines[0].PointCollection.Add(Lines[0].EndPoint);

			// Make sure the first point is green
			Lines[0].StartPoint.DeselectedColor = Colors.Green;
			Lines[0].StartPoint.Color = Lines[0].StartPoint.DeselectedColor;
			Lines[0].NotifyPointCollectionChanged();
			Lines[0].Visibility = true;

			// If the time bar is visible then...
			if (TimeBarVisible)
			{
				// Associate the line with the snapshot
				SelectedSnapShot.PolygonViewModel = null;
				SelectedSnapShot.LineViewModel = Lines[0];

				SelectPolygonSnapShot(SelectedSnapShot);
			}

			// Deselect all shapes and remove the resize adorner
			DeselectShapes();
			RaiseRemoveResizeAdorner();

			// Refresh the toolbar buttons
			((Command)PolygonToLineCommand).RaiseCanExecuteChanged();
			((Command)LineToPolygonCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Toggles the start side of a polygon.
		/// </summary>
		private void ToggleStartSide()
		{
			// Forward the request to the selected polygon
			SelectedPolygon.ToggleStartSide();
		}

		/// <summary>
		/// Adds a new polygon snapshot.
		/// </summary>
		private void AddPolygonSnapShot()
		{
			// Retrieve the index of the currently selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapShot);

			// Calculate the time of the new polygon; attempt to move it to the right
			double time = (SelectedSnapShot.Time + 20) / AdjustedTimeBarActualWidth;

			// If the time is off the scale
			if (time > 1.0)
			{
				// Set the time to the maximum
				time = 1.0;
			}

			// Create a new polyogn snapshot at the specified time and index
			PolygonSnapShotViewModel snapShot = CreatePolygonSnapShot(time, index + 1);

			// If a polygon is associated with the snapshot then...
			if (SelectedSnapShot.PolygonViewModel != null)				
			{
				// Clone the polygon of the selected snapshot
				PolygonViewModel newPolygonViewModel = new PolygonViewModel(
					SelectedSnapShot.PolygonViewModel.Polygon.Clone());
				
				// Associate the cloned polygon with the snapshot
				snapShot.PolygonViewModel = newPolygonViewModel;
			}
			// If a line is associated with the snapshot then...
			else if (SelectedSnapShot.LineViewModel != null)
			{
				// Clone the line of the selected snapshot
				LineViewModel newLineViewModel = new LineViewModel(SelectedSnapShot.LineViewModel.Line.Clone());

				// Associate the cloned line with the snapshot
				snapShot.LineViewModel = newLineViewModel;
			}
			
			// Select the newly created snapshot
			SelectPolygonSnapShot(snapShot);

			// Update the toolbar commands
			((Command)DeletePolygonSnapShotCommand).RaiseCanExecuteChanged();
		}
	
		/// <summary>
		/// Deletes the selected snapshot.
		/// </summary>
		private void DeletePolygonSnapShot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapShot);

			// Remove the selected snapshot
			PolygonSnapshots.Remove(SelectedSnapShot);

			// If the index is not the first index then...
			if (index > 0)
			{
				// Select the previous snapshot
				SelectPolygonSnapShot(PolygonSnapshots[index - 1]);
			}
			// Otherwise
			else
			{
				// Select the new first snapshot
				SelectPolygonSnapShot(PolygonSnapshots[0]);
			}
						
			// Update the toolbar commands
			((Command)DeletePolygonSnapShotCommand).RaiseCanExecuteChanged();			
		}

		/// <summary>
		/// Selects the next polygon snapshot.
		/// </summary>
		private void NextPolygonSnapShot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapShot);

			// Select the next polygon snapshot to the right
			SelectPolygonSnapShot(PolygonSnapshots[index + 1]);
			
			// Update the toolbar commands
			((Command)NextPolygonSnapShotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapShotCommand).RaiseCanExecuteChanged();
		}
		
		/// <summary>
		/// Selects the previous snapshot.
		/// </summary>
		private void PreviousPolygonSnapShot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapShot);

			// Select the next polygon snapshot to the left
			SelectPolygonSnapShot(PolygonSnapshots[index - 1]);

			// Update the toolbar commands
			((Command)NextPolygonSnapShotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapShotCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Returns true if the Next Polygon Snapshot command is enabled.
		/// </summary>
		/// <returns>True if the Next Polygon Snapshot command is enabled</returns>
		private bool IsNextPolygonSnapShotEnabled()
		{
			// Command is enabled if the current snapshot is not the last snapshot
			return PolygonSnapshots.IndexOf(SelectedSnapShot) < PolygonSnapshots.Count - 1;
		}

		/// <summary>
		/// Returns true if the Previous Polygon Snapshot command is enabled.
		/// </summary>
		/// <returns>True if the Previous Polygon Snapshot command is enabled</returns>
		private bool IsPreviousPolygonSnapShotEnabled()
		{
			// Command is enabled if the current snapshot is not the first snapshot
			return PolygonSnapshots.IndexOf(SelectedSnapShot) != 0;
		}

		/// <summary>
		/// Updates the mouse cursor for the specified location.
		/// </summary>
		/// <param name="mousePosition">Mouse position</param>
		private void UpdateCursor(Point mousePosition)
		{
			// Default to the normal arrow cursor
			Cursor cursor = Cursors.Arrow;

			PolygonSnapShotViewModel snapShot = null;
			if (IsMouseOverTimeBar(mousePosition, ref snapShot))
			{
				// Change to the sizing cursor
				cursor = Cursors.SizeWE;
			}

			// Set cursor for the control
			TimeBarCusor = cursor;
		}

		/// <summary>
		/// Handles time bar snapshot move mouse command.
		/// </summary>
		/// <param name="eventArgs">Mouse move event arguments</param>
		private void MoveMouseSnapshot(MouseEventArgs eventArgs)
		{
			// Get the position of the mouse
			Point mousePosition = eventArgs.GetPosition((Canvas)eventArgs.OriginalSource);

			// Update the cursor based on what the mouse is over
			UpdateCursor(mousePosition);

			// If a snapshot is being moved then...
			if (MovingSnapShot)
			{				
				// Move the snapshot
				MovePolygonSnapshot(SelectedSnapShot, (int)mousePosition.X);
			}
		}

		/// <summary>
		/// Handles the time bar left mouse down command.
		/// </summary>
		/// <param name="eventArgs">Mouse event arguments</param>
		private void MouseLeftButtonDownTimeBar(MouseEventArgs eventArgs)
		{
			// Get the position of the mouse
			Point clickPosition = eventArgs.GetPosition((Canvas)eventArgs.OriginalSource);
			
			PolygonSnapShotViewModel polygonSnapShot = null;

			// If the mouse is over a snapshot then...
			if (IsMouseOverTimeBar(clickPosition, ref polygonSnapShot))
			{
				// Indicate we are moving a snapshot
				MovingSnapShot = true;

				// Select the snapshot under the mouse
				SelectPolygonSnapShot(polygonSnapShot);
			}
		}

		/// <summary>
		/// Handles the time bar mouse left button up command.
		/// </summary>
		/// <param name="eventArgs"></param>
		private void MouseLeftButtonUpTimeBar(MouseEventArgs eventArgs)
		{
			// Get the position of the mouse
			Point clickPosition = eventArgs.GetPosition((Canvas)eventArgs.OriginalSource);

			// If a snapshot is being moved then...
			if (MovingSnapShot)
			{				
				// Move the selected snapshot to the specified position
				MovePolygonSnapshot(SelectedSnapShot, (int)clickPosition.X);
			}

			// Clear the flag that indicates a snapshot move is in progress
			MovingSnapShot = false;
		}

		/// <summary>
		/// Handles the time bar mouse leave command.
		/// </summary>
		private void MouseLeaveTimeBar()
		{			
			// If the mouse leaves the time bar it ends the snapshot move
			MovingSnapShot = false;
		}
				
		/// <summary>
		/// Moves the specified polygon to the specified position.
		/// </summary>
		/// <param name="polygonSnapShot">Polygon to move</param>
		/// <param name="position">Position to move the snapshot to</param>
		public void MovePolygonSnapshot(PolygonSnapShotViewModel polygonSnapShot, int position)
		{			
			// If the pointer has gone off the scale to the left then...
			if (position < PolygonSnapShotViewModel.HalfWidth)
			{
				// Reset the position to the start of the scale
				position = PolygonSnapShotViewModel.HalfWidth;
			}

			// If the pointer has gone off the scale to the right then...
			if (position > (AdjustedTimeBarActualWidth + PolygonSnapShotViewModel.HalfWidth))
			{
				// Reset the position to the end of the scale
				position = (int)(AdjustedTimeBarActualWidth + PolygonSnapShotViewModel.HalfWidth);
			}

			// Forward the command to the polygon snapshot
			polygonSnapShot.Move(position);

			// Calculate the normalized time (0.0 - 1.0)
			polygonSnapShot.NormalizedTime = (position - PolygonSnapShotViewModel.HalfWidth) / AdjustedTimeBarActualWidth;
			Debug.Assert(polygonSnapShot.NormalizedTime >= 0.0);
			Debug.Assert(polygonSnapShot.NormalizedTime <= 1.0);

			// Sort the snapshots by the associated time
			List<PolygonSnapShotViewModel> sortedSnapshots = PolygonSnapshots.OrderBy(snapShot => snapShot.Time).ToList();
			
			// Clear teh collection of snapshots
			PolygonSnapshots.Clear();

			// Add the snapshots back in increasing time order
			PolygonSnapshots.AddRange(sortedSnapshots);

			// Update the toolbar commands
			((Command)NextPolygonSnapShotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapShotCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Cuts the selected polygon/line.
		/// </summary>
		private void Cut()
		{
			Copy();
			Delete();
		}
		
		/// <summary>
		/// Paste the polygon on the clipboard into the center of the editor.
		/// </summary>
		private void Paste()
		{
			// Calculate the center point of the editor
			double centerX = ActualWidth / 2.0;
			double centerY = ActualHeight / 2.0;

			// If the clipboard contains polygon data then...
			if (Clipboard.ContainsData(LineClipboardFormat))
			{
				// Retrieve the line model object from the clipboard
				Line copiedLine = ((Line)Clipboard.GetData(LineClipboardFormat)).Clone();

				// Add the model line to the collection
				LineModels.Add(copiedLine);

				// Create a line view model object
				LineViewModel copiedLineVM = new LineViewModel(copiedLine);

				// Retrieve the center point of the line
				PolygonPointViewModel centerPoint = copiedLineVM.CenterPoint;

				// Determine how far from center the clipboard line is positioned
				// This is the distance required to move the line to the center of the editor.
				double moveX = centerX - centerPoint.X;
				double moveY = centerY - centerPoint.Y;

				// Move the points so the polygon is centered in the middle of the editor
				copiedLineVM.MovePoints(moveX, moveY, LimitPointToCanvas);

				// Add the line view model to the collection
				Lines.Add(copiedLineVM);

				// Fire the Property Changed event to ensure the converters run
				copiedLineVM.NotifyPointCollectionChanged();
				
				// Deselect any line or line points
				DeselectAllLines();
				DeselectShapes();

				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted line
				SelectLine(copiedLineVM, true);

				// If we are in time based mode then...
				if (EditorCapabilities.ShowTimeBar)
				{
					// Update the snapshot polygon with the copied view model polygon
					SelectedSnapShot.LineViewModel = copiedLineVM;

					// Reselect the polygon snapshot to reconfigure the bindings
					SelectPolygonSnapShot(SelectedSnapShot);
				}

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			// If the clipboard contains polygon data then...
			else if (Clipboard.ContainsData(PolygonClipboardFormat))
			{
				// Retrieve the polygon model object from the clipboard
				Polygon copiedPolygon = ((Polygon)Clipboard.GetData(PolygonClipboardFormat)).Clone();

				// Add the model polygon to the collection
				PolygonModels.Add(copiedPolygon);

				// Create a polygon view model object
				PolygonViewModel copiedPolygonVM = new PolygonViewModel(copiedPolygon);
				
				// Retrieve the center point of the polygon
				PolygonPointViewModel centerPoint = copiedPolygonVM.CenterPoint;
				
				// Determine how far from center the clipboard polygon is positioned
				// This is the distance required to move the polygon to the center of the editor.
				double moveX = centerX - centerPoint.X;
				double moveY = centerY - centerPoint.Y;

				// Move the points so the polygon is centered in the middle of the editor
				copiedPolygonVM.MovePoints(moveX, moveY, LimitPointToCanvas);
				
				// Add the polygon view model to the collection
				Polygons.Add(copiedPolygonVM);

				// Fire the Property Changed event to ensure the converters run
				copiedPolygonVM.NotifyPointCollectionChanged();

				// Deselect any polygons or polygon points
				DeselectAllPolygons();
				DeselectShapes();

				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted polygon
				SelectPolygon(copiedPolygonVM, true);

				// If we are in time based mode then...
				if (EditorCapabilities.ShowTimeBar)
				{
					// Update the snapshot polygon with the copied view model polygon
					SelectedSnapShot.PolygonViewModel = copiedPolygonVM;

					// Reselect the polygon snapshot to reconfigure the bindings
					SelectPolygonSnapShot(SelectedSnapShot);
				}

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
		}

		/// <summary>
		/// Copies the selected polygon.
		/// </summary>
		private void Copy()
		{
			// If a polygon is selected then...
			if (IsPolygonSelected())
			{
				// Copy the polygon data to the clipboard
				Clipboard.SetData(PolygonClipboardFormat, SelectedPolygon.Polygon.Clone());
			}
			else
			{
				// Copy the line data to the clipboard
				Clipboard.SetData(LineClipboardFormat, SelectedLine.Line.Clone());
			}
			
			// Update the execute status of the paste command
			((Command)PasteCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Returns true when the Paste command can execute.
		/// </summary>
		/// <returns>True when the Paste command can execute</returns>
		private bool CanExecutePaste()
		{
			// Return whether polygon or line data is on the clipboard and 
			// the editor is configured to allow pasting new shapes
			return (Clipboard.ContainsData(PolygonClipboardFormat) ||
					Clipboard.ContainsData(LineClipboardFormat)) &&
				   EditorCapabilities != null &&
				   EditorCapabilities.AddPolygons;
		}

		/// <summary>
		/// Returns true when the Delete command can execute.
		/// </summary>
		/// <returns>True when the Delete command can execute</returns>
		private bool CanExecuteDelete()
		{
			// Return whether polygon or line is selected and 
			// the editor is configured to allow deleting shapes
			return (IsPolygonSelected() || IsLineSelected()) &&
				EditorCapabilities != null &&
				EditorCapabilities.DeletePolygons;
		}

		/// <summary>
		/// Returns true when the Toggle Start Side command can execute.
		/// </summary>
		/// <returns>True when the Toggle Start Side command can execute</returns>
		private bool CanToggleStartSide()
		{
			// Return whether a polygon is selected and
			// the editor is configured to allow toggling the start side
			return IsPolygonSelected() &&
				EditorCapabilities != null &&
				EditorCapabilities.ToggleStartSide;
		}

		/// <summary>
		/// Returns true when the Toggle Start Side command can execute.
		/// </summary>
		/// <returns>True when the Toggle Start Side command can execute</returns>
		private bool IsShapeSelected()
		{
			return IsPolygonSelected() || IsLineSelected();
		}

		/// <summary>
		/// Returns true if a polygon is selected.
		/// </summary>
		/// <returns>True if a polygon is selected</returns>
		private bool IsPolygonSelected()
		{
			// Return true if the editor is selection mode and
			// a polygon is selected and 
			// all points on the polygon are selected
			return IsSelecting &&
				SelectedPolygon != null &&
				SelectedPolygon.AllPointsSelected;
		}

		/// <summary>
		/// Returns true if a line is selected.
		/// </summary>
		/// <returns>True if a line is selected</returns>
		private bool IsLineSelected()
		{
			// Return true if the editor is selection mode and
			// a line is selected and 
			return IsSelecting &&
				   SelectedLine != null;				
		}

		/// <summary>
		/// Returns true when the Delete Snapshot command can execute.
		/// </summary>
		/// <returns>True when the Delete Snapshot command can execute</returns>
		private bool IsDeleteSnapshotPolygonEnabled()
		{
			// Delete is enabled when there is more than one snapshot
			return PolygonSnapshots.Count() > 1;
		}

		/// <summary>
		/// Returns true if there is at least one polygon.
		/// </summary>
		/// <returns>True if there is at least one polygon</returns>
		private bool IsPolygon()
		{
			return (Polygons.Count > 0);
		}

		/// <summary>
		/// Returns true if there is at least one line.
		/// </summary>
		/// <returns>True if there is at least one line</returns>
		private bool IsLine()
		{
			return (Lines.Count > 0);
		}

		/// <summary>
		/// Updates the mouse cursor for the specified location.
		/// </summary>
		/// <param name="mousePosition">Mouse position</param>
		private void UpdateCanvasCursor(Point mousePosition)
		{
			// Default to the normal arrow cursor
			Cursor cursor = Cursors.Arrow;

			PolygonLineSegment lineSegment = null;
			PolygonViewModel polygon = null;

			// If the editor is in selection mode and
			// the mouse over a moveable item then....
			if (IsSelecting &&
			    IsMouseOverMoveableItem(mousePosition))
			{
				// Change to the sizing cursor
				cursor = Cursors.SizeAll;
			}
			// If the editor is add point mode and
			// the mouse is over a polygon line then...
			else if (AddPoint &&
			         IsMouseOverLine(mousePosition, ref lineSegment, ref polygon))
			{
				// The polygon that is associated with the line is closed then...
				if (polygon.PolygonClosed)
				{
					// Load special add point cursor
					using (Stream stream = GetAssemblyResourceStream(Assembly.GetExecutingAssembly(), "Cursors/cursor_arrow_dragbox.cur"))
					{
						cursor = new Cursor(stream);
					}
				}
			}

			// Set cursor for the control
			CanvasCursor = cursor;
		}

		/// <summary>
		/// Loads the resource at the specified path.
		/// The path separator is '/'.  The path should not start with '/'.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		private Stream GetAssemblyResourceStream(Assembly asm, string path)
		{
			// Just to be sure
			if (path[0] == '/')
			{
				path = path.Substring(1);
			}

			// Just to be sure
			if (path.IndexOf('\\') == -1)
			{
				path = path.Replace('\\', '/');
			}

			Stream resStream = null;

			string resName = asm.GetName().Name + ".g.resources"; // Ref: Thomas Levesque Answer at:
																  // http://stackoverflow.com/questions/2517407/enumerating-net-assembly-resources-at-runtime

			using (var stream = asm.GetManifestResourceStream(resName))
			{
				using (var resReader = new System.Resources.ResourceReader(stream))
				{
					string dataType = null;
					byte[] data = null;

					resReader.GetResourceData(path.ToLower(), out dataType, out data);

					if (data != null)
					{
						switch (dataType) // COde from 
						{
							// Handle internally serialized string data (ResourceTypeCode members).
							case "ResourceTypeCode.String":
								BinaryReader reader = new BinaryReader(new MemoryStream(data));
								string binData = reader.ReadString();
								Console.WriteLine("   Recreated Value: {0}", binData);
								break;
							case "ResourceTypeCode.Int32":
								Console.WriteLine("   Recreated Value: {0}", BitConverter.ToInt32(data, 0));
								break;
							case "ResourceTypeCode.Boolean":
								Console.WriteLine("   Recreated Value: {0}", BitConverter.ToBoolean(data, 0));
								break;
							// .jpeg image stored as a stream.
							case "ResourceTypeCode.Stream":
								////const int OFFSET = 4;
								////int size = BitConverter.ToInt32(data, 0);
								////Bitmap value1 = new Bitmap(new MemoryStream(data, OFFSET, size));
								////Console.WriteLine("   Recreated Value: {0}", value1);

								const int OFFSET = 4;
								resStream = new MemoryStream(data, OFFSET, data.Length - OFFSET);

								break;
							// Our only other type is DateTimeTZI.
							default:
								Debug.Assert(false, "Unsupported Resource Type");
								break;
						}
					}
				}
			}

			return resStream;
		}

		/// <summary>
		/// Handles polygon/line canvas mouse move command.
		/// </summary>
		/// <param name="mouseEventArgs"></param>
		private void CanvasMouseMove(MouseEventArgs mouseEventArgs)
		{
			// Get the position of the mouse click
			Point mousePosition = mouseEventArgs.GetPosition(Canvas);
			
			// Update the cursor based on what the mouse is over
			UpdateCanvasCursor(mousePosition);

			// If a point is being moved then...
			if (MovingPoint)
			{
				// Move the currently selected point
				MovePoint(mousePosition);
			}

			// If the editor is in selection mode and
			// the user is NOT moving a point and
			// the left mouse button is down and
			// user did not just select a shape then...
			if (IsSelecting &&
			    !MovingPoint &&
			    mouseEventArgs.LeftButton == MouseButtonState.Pressed &&
			    !SelectedShapeFlag)
			{
				// Use the rubber band adorner to select (lasso) points
				EventHandler handler = LassoPoints;
				handler?.Invoke(this, mouseEventArgs);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Deselects all shapes.
		/// </summary>
		public void DeselectAllShapes()
		{
			// Deselect all the polygons
			DeselectAllPolygons();

			// Deselect all the lines
			DeselectAllLines();
		}

		/// <summary>
		/// Deselects all polygons.
		/// </summary>
		public void DeselectAllPolygons()
		{
			// Loop over all the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Deselect all points on the polygon
				polygon.DeselectAllPoints();
			}

			// Clear all selected points
			SelectedPoints.Clear();
		}

		/// <summary>
		/// Deselect all lines.
		/// </summary>
		public void DeselectAllLines()
		{
			// Loop over all the lines
			foreach (LineViewModel line in Lines)
			{
				// Deselect all points on the line
				line.DeselectAllPoints();
			}

			// Clear all selected points
			SelectedPoints.Clear();
		}

		/// <summary>
		/// Gives the view model the bounds of the editor.
		/// </summary>
		/// <param name="actualWidth">Width of editor</param>
		/// <param name="actualHeight">Height of editor</param>
		public void UpdateEditorSize(double actualWidth, double actualHeight)
		{
			// Store off the dimensions of the editor
			ActualWidth = actualWidth;
			ActualHeight = actualHeight;			
		}
				
		/// <summary>
		/// Selects a polygon, line or point based on the click position.
		/// </summary>
		/// <param name="clickPosition">Click position</param>
		public bool SelectPolygonLineOrPoint(Point clickPosition)
		{
			// Default to the mouse not being over a shape
			bool shapeSelected = false;
			
			// Deselect the currently selected polygon or line
			DeselectShapes();

			PolygonViewModel selectedPolygon = null;
			LineViewModel selectedLine = null;
			PolygonPointViewModel selectedPoint = null;

			// If the mouse is over a polygon/line point then...
			if (IsMouseOverPolygonPoint(clickPosition, ref selectedPoint, ref selectedPolygon))
			{
				// Select the polygon point
				SelectPolygonPoint(selectedPoint, selectedPolygon);
				
				// Start a move operation on the point
				MovingPoint = true;

				// Create a new ghost Point
				PointMoving = new PolygonPointViewModel(new PolygonPoint(), null);

				// Set the position of the ghost point
				PointMoving.X = SelectedPolygon.SelectedVertex.X;
				PointMoving.Y = SelectedPolygon.SelectedVertex.Y;
			}
			// If the mouse is over the center cross hash then...
			else if (IsMouseOverPolygonCenterCrossHash(clickPosition, ref selectedPolygon))
			{
				// Select the polygon and all its points
				SelectPolygon(selectedPolygon, true);
				
				// Return that the entire polygon was selected
				shapeSelected = true;
			}
			// If the mouse is over a line point then...
			else if (IsMouseOverLinePoint(clickPosition, ref selectedPoint, ref selectedLine))
			{
				// Select the polygon point
				SelectLinePoint(selectedPoint, selectedLine);
				
				// Start a move operation on the point
				MovingPoint = true;

				// Create a new ghost Point
				PointMoving = new PolygonPointViewModel(new PolygonPoint(), null);

				// Set the position of the ghost point
				PointMoving.X = SelectedLine.SelectedVertex.X;
				PointMoving.Y = SelectedLine.SelectedVertex.Y;
			}
			// If the mouse is over the center cross hash then...
			else if (IsMouseOverLineCenterCrossHash(clickPosition, ref selectedLine))
			{
				// Select the line and all its points
				SelectLine(selectedLine, true);

				// Return that the entire polygon was selected
				shapeSelected = true;
			}
			// Otherwise the mouse isn't over a polygon or line point 
			else					
			{
				// Deselect all polygons
				DeselectAllPolygons();

				// Deselect all lines
				DeselectAllLines();
			}
			
			// Return whether a shape was selected
			return shapeSelected;
		}

		/// <summary>
		/// Adds a point to the current polygon.
		/// </summary>
		/// <param name="position">Position of new point</param>
		public void AddPolygonPoint(Point position)
		{			
			// Forward the operation to the polygon view model
			NewPolygon.AddPoint(position);
				
			// If the polygon was closed then...
			if (NewPolygon.PolygonClosed)
			{
				// If the editor is time frame mode then...
				if (TimeBarVisible)
				{
					// Associate the polygon with the snapshot
					SelectedSnapShot.PolygonViewModel = NewPolygon;

					// Re-selecting the snapshot to make sure editor is bound to the correct polygon
					SelectPolygonSnapShot(SelectedSnapShot);

					// Clear out the new polygon
					NewPolygon = null;
					
					// Switch to selection mode
					IsSelecting = true;

					// Refresh the shape points
					RefreshAllShapes();
				}
				else
				{
					// Prepare to create a new polygon
					AddNewPolygon();
				}				
			}

			// If the polygon was closed then we need to evaluate what toolbar commands are valid
			((Command)AddPointCommand).RaiseCanExecuteChanged();
			((Command)DrawPolygonCommand).RaiseCanExecuteChanged();
			((Command)DrawLineCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Adds a point to the current line.
		/// </summary>
		/// <param name="position">Position of new point</param>
		public void AddLinePoint(Point position)
		{
			// Forward the operation to the line view model
			NewLine.AddPoint(position);

			// If the line has two points then..
			if (NewLine.PointCollection.Count == 2)
			{
				// Save off the start point
				NewLine.StartPoint = NewLine.PointCollection[0];

				// Save off the end piont
				NewLine.EndPoint = NewLine.PointCollection[1];
				
				// If the editor is time frame mode then...
				if (TimeBarVisible)
				{
					// Associate the polygon with the snapshot
					SelectedSnapShot.LineViewModel = NewLine;

					// Re-selecting the snapshot to make sure editor is bound to the correct line
					SelectPolygonSnapShot(SelectedSnapShot);

					// Clear out the new line
					NewLine = null;

					// Switch to selection mode
					IsSelecting = true;

					// Refresh the shape points
					RefreshAllShapes();
				}
				else
				{
					// Prepare to create a new line
					AddNewLine();
				}
			}
		}

		/// <summary>
		/// Initializes the polygon snapshots.
		/// </summary>
		public void InitializePolygonSnapShots(
			IList<Polygon> polygons, 
			IList<double> polygonTimes,
			IList<Line> lines,
			IList<double> lineTimes)
		{			
			// Loop over the polygon models
			int index = 0;
			foreach (Polygon polygon in polygons)
			{	
				// Create the snapshot 
				PolygonSnapShotViewModel snapShot = CreatePolygonSnapShot(polygonTimes[index], PolygonSnapshots.Count);
					
				// Associate the polygon with the snapshot
				snapShot.PolygonViewModel = new PolygonViewModel(polygon);
				index++;
			}
								
			// Loop over the model lines
			index = 0;
			foreach (Line line in lines)
			{
				// Create the snapshot 
				PolygonSnapShotViewModel snapShot = CreatePolygonSnapShot(lineTimes[index], PolygonSnapshots.Count);
					
				// Associate the line with the snapshot
				snapShot.LineViewModel = new LineViewModel(line);
				index++;
			}
				
			// Select the first snapshot
			SelectPolygonSnapShot(PolygonSnapshots[0]);				
		}

		/// <summary>
		/// Update the position of the snapshot sliders when the view is resized.
		/// </summary>
		public void UpdatePolygonSnapShots()
		{
			// Loop over the snapshots
			foreach(PolygonSnapShotViewModel snapShot in PolygonSnapshots)
			{
				// Update the snapshots position based on the normalized time
				snapShot.Move((int)((snapShot.NormalizedTime * AdjustedTimeBarActualWidth) + PolygonSnapShotViewModel.HalfWidth));
			}
		}

		/// <summary>
		/// Initializes the editor with the polygon models.
		/// </summary>
		/// <param name="polygons">Model polygons</param>
		/// <param name="times">Model polygon times</param>		
		public void InitializePolygons(IList<Polygon> polygons, IList<double> times)
		{			
			// Loop over all the polygons
			foreach(Polygon polygon in polygons)
			{
				// Scale the points for the editor canvas size
				polygon.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);				
			}

			// Sort the polygons by the associated times
			SortSnapshots(polygons, times);
						
			// Clear the polygon view models and models		
			Polygons.Clear();
			PolygonModels.Clear();

			// If the editor is NOT in time based mode then...
			if (!EditorCapabilities.ShowTimeBar)			
			{
				// Loop over the polygon models
				foreach (Polygon polygon in polygons)
				{
					// Create polygon view models
					PolygonModels.Add(polygon);
					Polygons.Add(new PolygonViewModel(polygon));					
				}

				// If the editor is configured to be in select mode and 
				// there is only one polygon then...
				if (_editorCapabilities.DefaultToSelect && Polygons.Count == 1)
				{	
					// Select the polygon
					SelectPolygon(Polygons[0], true);
				}				
			}
		}
		
		/// <summary>
		/// Initializes the editor with the line models.
		/// </summary>
		/// <param name="lines">Model polygons</param>
		/// <param name="times">Model polygon times</param>		
		public void InitializeLines(IList<Line> lines, IList<double> times)
		{			
			// Loop over the lines
			foreach (Line line in lines)
			{
				// Scale the points for the editor canvas size
				line.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);
			}

			// Sort the polygons by the associated times
			SortSnapshots(lines, times);

			// Clear the line view models and models
			Lines.Clear();
			LineModels.Clear();

			// If the editor is NOT in time mode then...
			if (!EditorCapabilities.ShowTimeBar)
			{
				// Loop over the line models
				foreach (Line line in lines)
				{
					// Create line view models
					LineModels.Add(line);
					Lines.Add(new LineViewModel(line));
				}

				// If the editor is configured to be in select mode and 
				// there is only one line then...
				if (_editorCapabilities.DefaultToSelect && Lines.Count == 1)
				{
					// Select the line
					SelectLine(Lines[0], true);
				}				
			}
		}

		/// <summary>
		/// Gets the line snapshot times.
		/// </summary>
		/// <returns>Collection of normalized (0-1) line times</returns>
		public IList<double> GetLineTimes()
		{
			// Create the return collection
			List<double> times = new List<double>();

			// Loop over the snapshots
			foreach(PolygonSnapShotViewModel snapShot in PolygonSnapshots)
			{
				// If the snapshot contains a line then...
				if (snapShot.LineViewModel != null)
				{
					// Add the time to the collection
					times.Add(snapShot.NormalizedTime);
				}
			}

			// Return the line times
			return times;
		}

		/// <summary>
		/// Gets the polyogn snapshot times.
		/// </summary>
		/// <returns>Collection of normalized (0-1) polygons times</returns>
		public IList<double> GetPolygonTimes()
		{
			// Create the return collection
			List<double> times = new List<double>();

			// Loop over the snapshots
			foreach (PolygonSnapShotViewModel snapShot in PolygonSnapshots)
			{
				// If the snapshot contains a polygon then...
				if (snapShot.PolygonViewModel != null)
				{
					Debug.Assert(snapShot.NormalizedTime >= 0.0);
					Debug.Assert(snapShot.NormalizedTime <= 1.0);

					// Add the time to the collection
					times.Add(snapShot.NormalizedTime);
				}
			}

			// Return the polygon times
			return times;
		}
		
		/// <summary>		
		/// Moves the selected point.
		/// </summary>
		/// <param name="position">New position of the point</param>
		public void EndMoveSelectedPoint(Point position)
		{
			// Make sure the point is on the editor canvas
			position = LimitPointToCanvas(position);
			
			// If there is a selected polygon then...
			if (SelectedPolygon != null)
			{
				// Forward the operation to the selected polygon
				SelectedPolygon.MoveSelectedPoint(position);
			}
			else
			{
				// Forward the operation to the selected line
				SelectedLine.MoveSelectedPoint(position);
			}

			// End the point move
			MovingPoint = false;

			// Hide the ghost lines
			MovingPointVisibilityNext = false;
			MovingPointVisibilityPrevious = false;
		}

		/// <summary>
		/// Moves the currently selected point to the specified position.
		/// </summary>
		/// <param name="position">New position of the point</param>
		public void MovePoint(Point position)
		{
			// If there is a selected polygon then...
			if (SelectedPolygon != null)
			{
				// Update the Next and Previous point so that the ghost point and associated line segments
				// can be drawn
				PreviousPointMoving = SelectedPolygon.GetPreviousPoint();
				NextPointMoving = SelectedPolygon.GetNextPoint();

				// Make the ghost point visible
				MovingPointVisibilityNext = true;
				MovingPointVisibilityPrevious = true;
			}
			// Otherwise a line is being moved
			else
			{
				// If the selected is the start point then...
				if (SelectedLine.StartPoint == SelectedLine.SelectedVertex)
				{
					// Set the other point for the ghost line to the the end point
					PreviousPointMoving = SelectedLine.EndPoint;
				}
				else
				{
					// Otherwise the other point is the start point of the line
					PreviousPointMoving = SelectedLine.StartPoint;
				}

				// Make one of the ghost line visible
				MovingPointVisibilityPrevious = true;
			}
			
			// Make sure the point did not go beyond the editor canvas
			Point limitedPoint = LimitPointToCanvas(position);

			// Update the ghost point position
			PointMoving.X = limitedPoint.X;
			PointMoving.Y = limitedPoint.Y;			
		}

		/// <summary>
		/// Selects the specified point on the specified polygon.
		/// </summary>
		/// <param name="selectedPoint">Point to select</param>
		/// <param name="selectedPolygon">Polygon which contains the point</param>
		public void SelectPolygonPoint(PolygonPointViewModel selectedPoint, PolygonViewModel selectedPolygon)
		{						
			// If a polygon was previously selected then...
			if (SelectedPolygon != null)
			{
				// Deselect all points on the previously selected polygon
				SelectedPolygon.DeselectAllPoints();
			}

			// Store off the selected polygon
			SelectedPolygon = selectedPolygon;

			// Select the specified point
			SelectedPolygon.SelectPoint(selectedPoint);

			// Add the point to the selected points collection
			SelectedPoints.Clear();
			SelectedPoints.Add(SelectedPolygon.SelectedVertex);
		}

		/// <summary>
		/// Selects the specified point on the specified line.
		/// </summary>
		/// <param name="selectedPoint">Point to select</param>
		/// <param name="selectedLine">Line which contains the point</param>
		public void SelectLinePoint(PolygonPointViewModel selectedPoint, LineViewModel selectedLine)
		{
			// If a line was previously selected then...
			if (SelectedLine != null)
			{
				// Deselect all points on the previously selected line
				SelectedLine.DeselectAllPoints();
			} 

			// Store off the selected line
			SelectedLine = selectedLine;

			// Select the specified point
			SelectedLine.SelectPoint(selectedPoint);

			// Add the point to the selected points collection
			SelectedPoints.Clear();
			SelectedPoints.Add(selectedPoint);
		}

		/// <summary>
		/// Returns true if all the points on a polygon have been selected.
		/// </summary>
		/// <param name="selectedPoints">Collection of selected points</param>
		/// <param name="selectedPolygon"></param>
		/// <returns>True if all the points on a polygon were selected</returns>
		public bool IsEntirePolygonSelected(ObservableCollection<PolygonPointViewModel> selectedPoints, ref PolygonViewModel selectedPolygon)
		{
			// Initialize the selected polygon to null
			selectedPolygon = null;
			
			// Loop over the polygon view models
			foreach (PolygonViewModel polygon in Polygons)
			{
				// If all the points on the specified polygon are selected then...
				if (polygon.AreAllPointsSelected())
				{
					// Check to see if this is the only polygon with points selected
					if (polygon.PointCollection.Count == selectedPoints.Count)
					{
						// Indicate the selected polygon was found
						selectedPolygon = polygon;
						break;
					}
				}
			}

			// Return true if an entire polygon was selected
			return (selectedPolygon != null);
		}

		/// <summary>
		/// Selects all the polygon points on the currently selected polygon.
		/// </summary>
		public void SelectPolygonPoints()
		{
			// Clear the selected points                
			SelectedPoints.Clear();

			// Loop over all the points on the selected polygon
			foreach (PolygonPointViewModel point in SelectedPolygon.PointCollection)
			{
				// Add the polygon points to the selected points collection
				SelectedPoints.Add(point);
			}
		}

		/// <summary>
		/// Clears the selected points.
		/// </summary>
		public void ClearSelectedPoints()
		{
			// Loop over the selected points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Deselect the point
				point.Selected = false;
			}

			// Clear the selected points
			SelectedPoints.Clear();
		}

		/// <summary>
		/// Selects polygon points and line points in the specified lasso rectangle.
		/// </summary>
		/// <param name="lasso">Rectangle to select points from</param>
		public void SelectShapePoints(Rect lasso)
		{		
			// Loop over the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Add the points of the polygon included in the lasso rectangle
				SelectedPoints.AddRange(polygon.SelectShapePoints(lasso));				
			}

			// Loop over the lines
			foreach (LineViewModel line in Lines)
			{
				// Add the points of the lines included in the lasso rectangle
				SelectedPoints.AddRange(line.SelectShapePoints(lasso));				
			}
		}

		/// <summary>
		/// Finds the polygon parent of the specified polygon point.
		/// </summary>
		/// <param name="point">Polygon point to find the parent</param>
		/// <returns>Polygon that contains the specifed point</returns>
		public PolygonViewModel FindPolygon(PolygonPointViewModel point)
		{
			PolygonViewModel selectedPolygon = null;

			// Loop over all the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// If the polygon contains the specified point then...
				if (polygon.PointCollection.Contains(point))
				{ 
					// Store off that we found the polygon
					selectedPolygon = polygon;

					// Break out of loop
					break;
				}
			}

			return selectedPolygon;
		}

		/// <summary>
		/// Finds the polygon parent of the specified polygon point.
		/// </summary>
		/// <param name="point">Polygon point to find the parent</param>
		/// <returns>Polygon that contains the specifed point</returns>
		public LineViewModel FindLine(PolygonPointViewModel point)
		{
			LineViewModel selectedLine = null;

			// Loop over all the lines
			foreach (LineViewModel line in Lines)
			{
				// If the line contains the specified point then...
				if (line.PointCollection.Contains(point))
				{
					// Store off that we found the line
					selectedLine = line;

					// Break out of loop
					break;
				}
			}

			return selectedLine;
		}

		/// <summary>
		/// Gets the rectangle bounds of the specified polygon/line points.
		/// </summary>        
		public Rect GetSelectedContentBounds(ObservableCollection<PolygonPointViewModel> points)
		{
			// Initialize the rectangle to the first point
			double left = points[0].X;
			double top = points[0].Y;
			double right = points[0].X;
			double bottom = points[0].Y;

			// Loop through the polygon points
			foreach (PolygonPointViewModel point in points)
			{
				// If the point is less than the current left point then...
				if (point.X < left)
				{
					// Update the left point
					left = point.X;
				}

				// If the point is less than the current top point then...
				if (point.Y < top)
				{
					// Update the top point
					top = point.Y;
				}

				// If the point is greater than the current right point then...
				if (point.X > right)
				{
					// Update the right point
					right = point.X;
				}

				// if the point is greater than the current bottom point then...
				if (point.Y > bottom)
				{
					// Update the bottom point
					bottom = point.Y;
				}
			}

			// Create a rectangle from the four points
			Rect bounds = new Rect(left, top, right - left, bottom - top);

			// Return the bounds of the points
			return bounds;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Event refreshes the resize adorner after a rotation.
		/// </summary>
		public event EventHandler RefreshResizeAdorner;

		/// <summary>
		/// Event removes the resize adorner.
		/// </summary>
		public event EventHandler RemoveResizeAdorner;

		/// <summary>
		/// Event displays the resize adorner for the selected shape.
		/// </summary>
		public event EventHandler DisplayResizeAdorner;

		/// <summary>
		/// Event displays the resize adorner for the selected points.
		/// </summary>
		public event EventHandler DisplayResizeAdornerForSelectedPoints;

		/// <summary>
		/// Event repositions the resize adorner after operations like rotate.
		/// </summary>
		public event EventHandler RepositionResizeAdorner;

		/// <summary>
		/// Event lasso's the points within the rubber band area.
		/// </summary>
		public event EventHandler LassoPoints;

		/// <summary>
		/// Event removes the rubber band adorner.
		/// </summary>
		public event EventHandler RemoveRubberBandAdorner;

		#endregion

		#region IResizeable Methods

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void RotateSelectedItems(double angle, Point center)
		{
			// Create the rotation transform
			RotateTransform rt = new RotateTransform(angle, center.X, center.Y);

			// Process each selected polygon/line points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Convert the view model point to a .NET point
				Point tempPoint = point.GetPoint();
				
				// Transform the point
				Point transformedPoint = rt.Transform(tempPoint);

				// Update the view model point
				point.X = transformedPoint.X;				
				point.Y = transformedPoint.Y;
			}
		
			// Refresh all the shapes for the transform
			RefreshAllShapes();

			// Update the resize adorner
			EventHandler handler = RefreshResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void DoneRotating()
		{
			// If there are selected points then...
			if (SelectedPoints.Count > 0)
			{
				// Fire event to reposition the resize adorner
				EventHandler handler = RepositionResizeAdorner;
				handler?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public bool IsRotateable(double angle, Point center)
		{
			// Default to being a valid rotation
			bool rotateable = true;

			// Create the rotation transform
			RotateTransform rt = new RotateTransform(angle, center.X, center.Y);

			// Process each selected polygon/line points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Convert the view model point to a .NET point
				Point tempPoint = point.GetPoint();

				// Transform the point
				Point transformedPoint = rt.Transform(tempPoint);

				// If the point is outside of the drawing canvas then...				
				if (transformedPoint.X < 0 ||
					transformedPoint.Y < 0 ||
					transformedPoint.X > ActualWidth - 1 ||
					transformedPoint.Y > ActualHeight - 1)
				{
					// Remove the resize adorner 
					RaiseRemoveResizeAdorner();					

					// Indicate this is an invalid rotation
					rotateable = false;
					break;
				}
			}

			// Return whether this is a valid rotation
			return rotateable;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void ClipSelectedPoints()
		{
			// Loop over the selected view model points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Ensure the point is on the canvas
				LimitPointToCanvas(point);				
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void TransformSelectedItems(TransformGroup t)
		{
			// Loop over the transforms
			foreach (Transform tc in t.Children)
			{
				// If the transform is a scale transform then...
				if (tc is ScaleTransform)
				{
					// Cast to the scale transform
					ScaleTransform st = (ScaleTransform)tc;
					
					// Loop over the selected view model points
					foreach (PolygonPointViewModel point in SelectedPoints)
					{						
						// Transform the point
						Point transformedPoint = st.Transform(point.GetPoint());

						// Update the view model point
						point.SuppressChangeEvents = true;
						point.X = transformedPoint.X;						
						point.Y = transformedPoint.Y;
						point.SuppressChangeEvents = false;

						// Make sure the point is still on the drawing canvas
						LimitPointToCanvas(point);											
					}
				}
			}

			// Refresh all shapes for the transform
			RefreshAllShapes();			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void MoveSelectedItems(Transform transform)
		{
			// Cast to a translate transform
			TranslateTransform translateTransform = (TranslateTransform)transform;
			
			// Loop over the selected view model points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Transform the point
				Point transformedPoint = translateTransform.Transform(point.GetPoint());

				// Update the view model point
				point.SuppressChangeEvents = true;
				point.X = transformedPoint.X;
				point.Y = transformedPoint.Y;
				point.SuppressChangeEvents = false;
			}

			// Refresh all shapes for the transform
			RefreshAllShapes();			
		}
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public double GetWidth()
		{
			// Return the width of the drawing canvas
			return ActualWidth;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		/// <returns></returns>
		public double GetHeight()
		{
			// Return the height of the drawing canvas
			return ActualHeight;
		}
		
		#endregion
	}
}
