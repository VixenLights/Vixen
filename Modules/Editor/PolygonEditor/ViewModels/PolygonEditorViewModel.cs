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
		public PolygonEditorViewModel()
		{
			// Create the convert to commands
			ConvertToLineCommand = new Command(ConvertToLine, ConvertToLineEnabled);
			ConvertToPolygonCommand = new Command(ConvertToPolygon, ConvertToPolygonEnabled);
			ConvertToEllipseCommand = new Command(ConvertToEllipse, ConvertToEllipseEnabled);

			// Create the collection of model polygons
			PolygonModels = new ObservableCollection<Polygon>();

			// Create the collection of model lines
			LineModels = new ObservableCollection<Line>();

			// Create the collection of model ellipses
			EllipseModels = new ObservableCollection<Ellipse>();

			// Create the collection of view model polygons	 						
			Polygons = new ObservableCollection<PolygonViewModel>();

			// Create the collection of view model lines
			Lines = new ObservableCollection<LineViewModel>();

			// Create the collection of view model ellipses
			Ellipses = new ObservableCollection<EllipseViewModel>();

			// Create the collection of polygon snapshots
			PolygonSnapshots = new ObservableCollection<PolygonSnapshotViewModel>();
			
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
			DeleteShapeOrPointsCommand = new Command(DeleteShapeOrPoints);
			ToggleStartSideCommand = new Command(ToggleStartSide, IsToggleableShapeSelected);
			ToggleStartPointCommand = new Command(ToggleStartPoint, IsLineSelected);
			AddPolygonSnapshotCommand = new Command(AddPolygonSnapshot);
			DeletePolygonSnapshotCommand = new Command(DeletePolygonSnapshot, IsDeleteSnapshotPolygonEnabled);
			NextPolygonSnapshotCommand = new Command(NextPolygonSnapshot, IsNextPolygonSnapshotEnabled);
			PreviousPolygonSnapshotCommand = new Command(PreviousPolygonSnapshot, IsPreviousPolygonSnapshotEnabled);
			AddPointCommand = new Command(AddPolygonPoint, IsAddPolygonPointEnabled);
			DrawPolygonCommand = new Command(DrawPolygonAction, IsDrawShapeEnabled);
			DrawLineCommand = new Command(DrawLineAction, IsDrawShapeEnabled);			
			DrawEllipseCommand = new Command(DrawEllipseAction, IsDrawShapeEnabled);
			MoveSnapshotCommand = new Command<MouseEventArgs>(MoveMouseSnapshot);
			MouseLeftButtonDownTimeBarCommand = new Command<MouseEventArgs>(MouseLeftButtonDownTimeBar);
			MouseLeftButtonUpTimeBarCommand = new Command<MouseEventArgs>(MouseLeftButtonUpTimeBar);
			MouseLeaveTimeBarCommand = new Command<MouseEventArgs>(MouseLeaveTimeBar);
			ToggleLabelsCommand = new Command(ToggleLabels);
			CanvasMouseMoveCommand = new Command<MouseEventArgs>(CanvasMouseMove);
			CanvasMouseLeftButtonDownCommand = new Command<MouseEventArgs>(CanvasMouseLeftButtonDown);
			CanvasMouseLeftButtonUpCommand = new Command<MouseEventArgs>(CanvasMouseLeftButtonUp);
			CellEditEndingCommand = new Command(CellEditEnding);
			CurrentCellChangedCommand = new Command(CurrentCellChanged);

			// Ellipse points are not editable by default
			SelectedPointsReadOnly = true;
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

		/// <summary>
		/// Clipboard format identifier for an ellipse.
		/// </summary>
		const string EllipseClipboardFormat = "EllipseClipboardFormat";

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// Determines if the selected points grid is read-only.
		/// </summary>
		public bool SelectedPointsReadOnly
		{
			get { return GetValue<bool>(SelectedPointsReadOnlyProperty); }
			set { SetValue(SelectedPointsReadOnlyProperty, value); }
		}

		/// <summary>
		/// SelectedPointsReadOnly property data.
		/// </summary>
		public static readonly PropertyData SelectedPointsReadOnlyProperty = RegisterProperty(nameof(SelectedPointsReadOnly), typeof(bool));

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
		/// Display element width excluding any margin.
		/// </summary>
		public double DisplayElementWidth
		{
			get { return GetValue<double>(DisplayElementWidthProperty); }
			set { SetValue(DisplayElementWidthProperty, value); }
		}

		/// <summary>
		/// DisplayElementWidth property data.
		/// </summary>
		public static readonly PropertyData DisplayElementWidthProperty = RegisterProperty(nameof(DisplayElementWidth), typeof(double));

		/// <summary>
		/// Display element height excluding any margin.
		/// </summary>
		public double DisplayElementHeight
		{
			get { return GetValue<double>(DisplayElementHeightProperty); }
			set { SetValue(DisplayElementHeightProperty, value); }
		}

		/// <summary>
		/// DisplayElementHeight property data.
		/// </summary>
		public static readonly PropertyData DisplayElementHeightProperty = RegisterProperty(nameof(DisplayElementHeight), typeof(double));

		/// <summary>
		/// Display element X axis origin.  This is an offset into the virtual display element.
		/// </summary>
		public double DisplayElementXOrigin
		{
			get { return GetValue<double>(DisplayElementXOriginProperty); }
			set { SetValue(DisplayElementXOriginProperty, value); }
		}

		/// <summary>
		/// Display element X axis origin.  
		/// </summary>
		public static readonly PropertyData DisplayElementXOriginProperty = RegisterProperty(nameof(DisplayElementXOrigin), typeof(double));

		/// <summary>
		/// Display element Y axis origin.  This is an offset into the virtual display element.
		/// </summary>
		public double DisplayElementYOrigin
		{
			get { return GetValue<double>(DisplayElementYOriginProperty); }
			set { SetValue(DisplayElementYOriginProperty, value); }
		}

		/// <summary>
		/// DisplayElementYOrigin property data.
		/// </summary>
		public static readonly PropertyData DisplayElementYOriginProperty = RegisterProperty(nameof(DisplayElementYOrigin), typeof(double));

		/// <summary>
		/// Controls whether the display element outline should be displayed.
		/// </summary>
		public bool ShowDisplayElement
		{
			get { return GetValue<bool>(ShowDisplayElementProperty); }
			set { SetValue(ShowDisplayElementProperty, value); }
		}

		/// <summary>
		/// ShowDisplayElement property data.
		/// </summary>
		public static readonly PropertyData ShowDisplayElementProperty = RegisterProperty(nameof(ShowDisplayElement), typeof(bool));

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

		/// <summary>
		/// Gets or sets the view model's selected ellipse.
		/// </summary>
		public EllipseViewModel SelectedEllipse
		{
			get { return GetValue<EllipseViewModel>(SelectedEllipseProperty); }
			private set
			{
				SetValue(SelectedEllipseProperty, value);
				SelectedShape = value;
			}
		}

		/// <summary>
		/// SelectedLine property data.
		/// </summary>
		public static readonly PropertyData SelectedEllipseProperty = RegisterProperty(nameof(SelectedEllipse), typeof(EllipseViewModel));

		/// <summary>
		/// Currently selected shape.
		/// </summary>
		public ShapeViewModel SelectedShape
		{
			get { return GetValue<ShapeViewModel>(SelectedShapeProperty); }
			private set
			{
				SetValue(SelectedShapeProperty, value);

				// Refresh the convert commands since the selected shape changed
				RefreshToolbarConvertCommands();
			}
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
		public ObservableCollection<PolygonSnapshotViewModel> PolygonSnapshots
		{
			get { return GetValue<ObservableCollection<PolygonSnapshotViewModel>>(PolygonSnapshotsProperty); }
			private set { SetValue(PolygonSnapshotsProperty, value); }
		}

		/// <summary>		
		/// PolygonSnapshots property data.
		/// </summary>
		public static readonly PropertyData PolygonSnapshotsProperty = RegisterProperty(nameof(PolygonSnapshots), typeof(ObservableCollection<PolygonSnapshotViewModel>));
		
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
					DrawEllipse = false;
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
					// If we were in the middle of creating a new polygon 
					// then remove the partial polygon
					RemovePartialPolygon();
				}
				SetValue(DrawPolygonProperty, value); 
			}
		}

		/// <summary>
		/// DrawPolygon property data.
		/// </summary>
		public static readonly PropertyData DrawPolygonProperty = RegisterProperty(nameof(DrawPolygon), typeof(bool));

		/// <summary>
		/// Draw Ellipse mode flag.
		/// </summary>
		public bool DrawEllipse
		{
			get { return GetValue<bool>(DrawEllipseProperty); }
			set
			{
				// If drawing an ellipse
				if (value)
				{
					// Clear the other mode flags
					DrawPolygon = false;
					DrawLine = false;
					AddPoint = false;
					IsSelecting = false;

					// Deselect all the other shapes
					DeselectAllShapes();

					// Remove the resize adorner
					RaiseRemoveResizeAdorner();
				}
				
				SetValue(DrawEllipseProperty, value);
			}
		}

		/// <summary>
		/// DrawEllipse property data.
		/// </summary>
		public static readonly PropertyData DrawEllipseProperty = RegisterProperty(nameof(DrawEllipse), typeof(bool));

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
					DrawEllipse = false;
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
					// If we were in the middle of creating a new line 
					// then remove the partial line
					RemovePartialLine();
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
					DrawEllipse = false;
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
					DrawEllipse = false;
				}
				SetValue(IsSelectingProperty, value);

				// Update the copy command availability
				((Command)CopyCommand).RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// IsSelecting property data.
		/// </summary>
		public static readonly PropertyData IsSelectingProperty = RegisterProperty(nameof(IsSelecting), typeof(bool));

		/// <summary>
		/// 
		/// </summary>
		public bool TimeBarVisible
		{
			get
			{
				return GetValue<bool>(TimeBarVisibleProperty);
			}
			set
			{
				SetValue(TimeBarVisibleProperty, value);
			}
		}

		/// <summary>
		/// TimeBarVisible property data.
		/// </summary>
		public static readonly PropertyData TimeBarVisibleProperty = RegisterProperty(nameof(TimeBarVisible), typeof(bool));

		/// <summary>
		/// Width of time bar.
		/// </summary>
		public double TimeBarActualWidth
		{
			get { return GetValue<double>(TimeBarActualWidthProperty); }
			set
			{
				SetValue(TimeBarActualWidthProperty, value);
				UpdatePolygonSnapshots();
			}
		}

		/// <summary>
		/// TimeBarActualWidth property data.
		/// </summary>
		public static readonly PropertyData TimeBarActualWidthProperty = RegisterProperty(nameof(TimeBarActualWidth), typeof(double));

		/// <summary>
		/// Width of the canvas.
		/// </summary>
		public double CanvasActualWidth
		{
			get { return GetValue<double>(CanvasActualWidthProperty); }
			set
			{
				SetValue(CanvasActualWidthProperty, value);

				// Give the grid validation rule the canvas width
				XValidationRule.Width = (int)value;

				ActualWidth = (int)value;
			}
		}

		/// <summary>
		/// CanvasActualWidth property data.
		/// </summary>
		public static readonly PropertyData CanvasActualWidthProperty = RegisterProperty(nameof(CanvasActualWidth), typeof(double));

		/// <summary>
		/// Height of the canvas.
		/// </summary>
		public double CanvasActualHeight
		{
			get { return GetValue<double>(CanvasActualHeightProperty); }
			set
			{
				SetValue(CanvasActualHeightProperty, value);

					// Give the grid validation rule the canvas height
				YValidationRule.Height = (int)value;

				ActualHeight = (int)value;
			}
		}

		/// <summary>
		/// CanvasActualHeight property data.
		/// </summary>
		public static readonly PropertyData CanvasActualHeightProperty = RegisterProperty(nameof(CanvasActualHeight), typeof(double));

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
		/// Gets or sets the selected snapshot.
		/// </summary>
		public PolygonSnapshotViewModel SelectedSnapshot { get; set; }

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
			// The selection toolbar icon should be visible when
			// the draw toolbars are visible or the add point toolbar is visible (pattern mode)
			get { return ShowSelectDraw || ShowAddPoint; }
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
		/// Gets or sets the ellipse models.
		/// </summary>
		public ObservableCollection<Ellipse> EllipseModels { get; set; }

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
		/// Gets or sets the view model ellipses.
		/// </summary>
		public ObservableCollection<EllipseViewModel> Ellipses
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
		public bool MovingSnapshot { get; set; }

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
		/// Delete the selected shape or points command.
		/// </summary>
		public ICommand DeleteShapeOrPointsCommand { get; private set; }

		/// <summary>
		/// Toggle polygon start side command.
		/// </summary>
		public ICommand ToggleStartSideCommand { get; private set; }

		/// <summary>
		/// Toggle line start point command.
		/// </summary>
		public ICommand ToggleStartPointCommand { get; private set; }

		/// <summary>
		/// Converts the shape to a line command.
		/// </summary>
		public ICommand ConvertToLineCommand { get; private set; }

		/// <summary>
		/// Converts the shape to an ellipse.
		/// </summary>
		public ICommand ConvertToEllipseCommand { get; private set; }

		/// <summary>
		/// Converts the shape to a polygon command.
		/// </summary>
		public ICommand ConvertToPolygonCommand { get; private set; }

		/// <summary>
		/// Adds a polygon snapshot command.
		/// </summary>
		public ICommand AddPolygonSnapshotCommand { get; private set; }

		/// <summary>
		/// Delete polygon snapshot command.
		/// </summary>
		public ICommand DeletePolygonSnapshotCommand { get; private set; }

		/// <summary>
		/// Selects the next polygon snapshot command.
		/// </summary>
		public ICommand NextPolygonSnapshotCommand { get; private set; }

		/// <summary>
		/// Selects the previous polygon snapshot command.
		/// </summary>
		public ICommand PreviousPolygonSnapshotCommand { get; private set; }
		
		/// <summary>
		/// Add a polygon point command.
		/// </summary>
		public ICommand AddPointCommand { get; private set; }

		/// <summary>
		/// Draw a new polygon command.
		/// </summary>
		public ICommand DrawPolygonCommand { get; private set; }

		/// <summary>
		/// Draw a new ellipse command.
		/// </summary>
		public ICommand DrawEllipseCommand { get; private set; }

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

		/// <summary>
		/// DataGrid command when the current cell changes.
		/// </summary>
		public ICommand CurrentCellChangedCommand { get; private set; }

		/// <summary>
		/// DataGrid command when a cell is ending edit.
		/// </summary>
		public ICommand CellEditEndingCommand { get; private set; }

		#endregion

		#region Public Mouse Over Methods

		/// <summary>
		/// Returns true if the mouse is over a moveable item.
		/// </summary>
		/// <param name="position">Position of mouse</param>
		/// <returns>True if the mouse is over a moveable item</returns>
		/// <param name="snapshot">Snapshot the mouse is over</param>
		/// <returns>True if the mouse is over a moveable item</returns>
		public bool IsMouseOverTimeBar(Point position, ref PolygonSnapshotViewModel snapshot)
		{
			// Default to NOT over a snapshot
			bool overTimeBar = false;
			
			// Loop over the polygon snapshots
			foreach (PolygonSnapshotViewModel polygonSnapshot in PolygonSnapshots)
			{
				// If the mouse is over the polygon snapshot then...
				if (polygonSnapshot.IsMouseOverTimeBar(position))
				{
					// Save off the snapshot
					snapshot = polygonSnapshot;

					// Indicate we are over a snapshot
					overTimeBar = true;

					// Break out of foreach loop
					break;
				}
			}

			return overTimeBar;
		}

		/// <summary>
		/// Returns true if the mouse is over the center of a shape.
		/// </summary>
		/// <param name="position">Position of the mouse</param>
		/// <returns>true if the mouse is over the center of a shape</returns>
		private bool IsMouseOverCenterOfShape(Point position)
		{
			PolygonViewModel selectedPolygon = null;
			EllipseViewModel selectedEllipse = null;
			LineViewModel selectedLine = null;

			// Default to not being over the center of a shape
			bool mouseOverMoveableItem = false;

			// If the mouse is over a polygon then...
			if (IsMouseOverPolygonCenterCrossHash(position, ref selectedPolygon))
			{
				mouseOverMoveableItem = true;
			}
			// Check to see if the mouse is over a line
			else if (IsMouseOverLineCenterCrossHash(position, ref selectedLine))
			{
				mouseOverMoveableItem = true;
			}
			// Otherwise check to see if the mouse is over an ellipse
			else if (IsMouseOverEllipseCenterCrossHash(position, ref selectedEllipse))
			{
				mouseOverMoveableItem = true;
			}

			return mouseOverMoveableItem;
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

			PolygonViewModel selectedPolygon = null;
			PolygonPointViewModel selectedPolygonPoint = null;
			
			// If the mouse is over a polygon point or
			// it is over the center cross hash
			if (IsMouseOverPolygonPoint(position, ref selectedPolygonPoint, ref selectedPolygon) ||
				IsMouseOverPolygonCenterCrossHash(position, ref selectedPolygon))
			{
				// Indicate the mouse is over a moveable item and break out of loop
				mouseOverMoveableItem = true;
			}
			
			// If the mouse is not over a moveable item then check the lines
			if (!mouseOverMoveableItem)
			{
				LineViewModel selectedLine = null;
				PolygonPointViewModel selectedPoint = null;

				if (IsMouseOverLinePoint(position, ref selectedPoint, ref selectedLine) ||
					IsMouseOverLineCenterCrossHash(position, ref selectedLine))
				{
					// Indicate the mouse is over a moveable item and break out of loop
					mouseOverMoveableItem = true;
				}
			}

			// If the mouse is not over a moveable item then check the ellipses
			if (!mouseOverMoveableItem)
			{
				EllipseViewModel selectedEllipse = null;
				
				// If the mouse is over the center of the ellipse then...
				if (IsMouseOverEllipseCenterCrossHash(position, ref selectedEllipse))
				{
					// Indicate the mouse is over a moveable item and break out of loop
					mouseOverMoveableItem = true;
				}
			}

			return mouseOverMoveableItem;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ensures the specified point is on the editor canvas.
		/// </summary>
		/// <param name="position">Position of point</param>
		/// <returns>Limited point</returns>
		public Point LimitPointToCanvas(Point position)
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
		/// Selects the specified polygon snapshot.
		/// </summary>
		/// <param name="snapshot">Polygon snapshot to select</param>
		public void SelectPolygonSnapshot(PolygonSnapshotViewModel snapshot)
		{
			// If a selected snapshot exists then...
			if (SelectedSnapshot != null)
			{
				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Mark the previous snapshot as NOT selected
				SelectedSnapshot.Selected = false;
			}
			
			// Mark the specified polygon as selected
			snapshot.Selected = true;
			SelectedSnapshot = snapshot;

			// Clear all the containers the editor binds to
			Polygons.Clear();
			PolygonModels.Clear();
			Lines.Clear();
			LineModels.Clear();
			Ellipses.Clear();
			EllipseModels.Clear();

			// If the selected snapshot has a polygon then...
			if (SelectedSnapshot.PolygonViewModel != null)
			{
				// Add the polygon to the editor collections
				PolygonModels.Add(SelectedSnapshot.PolygonViewModel.Polygon);
				Polygons.Add(SelectedSnapshot.PolygonViewModel);

				// Select the polygon
				SelectPolygon(SelectedSnapshot.PolygonViewModel, true);

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			// Otherwise if the selected snapshot has a line then...
			else if (SelectedSnapshot.LineViewModel != null)
			{
				// Add the line to the editor collections
				LineModels.Add(SelectedSnapshot.LineViewModel.Line);
				Lines.Add(SelectedSnapshot.LineViewModel);

				// Select the line
				SelectLine(SelectedSnapshot.LineViewModel, true);

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			// Otherwise if the selected snapshot has an ellipse then...
			else if (SelectedSnapshot.EllipseViewModel != null)
			{
				// Add the ellipse to the editor collection
				EllipseModels.Add(SelectedSnapshot.EllipseViewModel.Ellipse);
				Ellipses.Add(SelectedSnapshot.EllipseViewModel);

				// Select the ellipse
				SelectEllipse(SelectedSnapshot.EllipseViewModel, true);

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			else
			{
				// Clear out the selected line, polygon, and ellipse
				SelectedLine = null;
				SelectedPolygon = null;
				SelectedEllipse = null;
			}

			// Force the editor to refresh
			RaisePropertyChanged(nameof(Polygons));
			RaisePropertyChanged(nameof(Lines));
			RaisePropertyChanged(nameof(Ellipses));

			// Force the toolbar commands to refresh
			((Command)DeletePolygonSnapshotCommand).RaiseCanExecuteChanged();
			((Command)AddPointCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="polygon">Polygon to select</param>
		/// <param name="updateSelectedPoints">Determines whether the selected points are updated</param>
		public void SelectPolygon(PolygonViewModel polygon, bool updateSelectedPoints)
		{
			// Select the polygon and all its points
			SelectAllPointsOnPolygon(polygon);

			// Clear out the selected line
			SelectedLine = null;

			// Clear out the selected ellipse
			SelectedEllipse = null;

			// Store off the selected polygon
			SelectedPolygon = polygon;

			// If the selected points need to be updated then...
			if (updateSelectedPoints)
			{
				// Clear out the selected points
				SelectedPoints.Clear();

				// Select all the points on the polygon
				SelectedPoints.AddRange(SelectedPolygon.PointCollection);

				// Polygon points are editable in the grid
				SelectedPointsReadOnly = false;
			}

			// Since the selected shape changed update the commands
			RefreshToolbarSelectedShapeCommands();
		}

		/// <summary>
		/// Selects the specified line.
		/// </summary>
		/// <param name="line">Line to select</param>
		/// <param name="updateSelectedPoints">Determines whether the selected points are updated</param>
		public void SelectLine(LineViewModel line, bool updateSelectedPoints)
		{
			// Select the line and all its points
			SelectAllPointsOnLine(line);

			// Clear out the selected polygon
			SelectedPolygon = null;

			// Clear out the selected ellipse
			SelectedEllipse = null;

			// Store off the selected line
			SelectedLine = line;

			// If the selected points need to be updated then...
			if (updateSelectedPoints)
			{
				// Clear out the selected points
				SelectedPoints.Clear();

				// Select all the points on the line
				SelectedPoints.AddRange(SelectedLine.PointCollection);

				// Line points are editable in the grid
				SelectedPointsReadOnly = false;
			}

			// Since the selected shape changed update the commands
			RefreshToolbarSelectedShapeCommands();
		}

		/// <summary>
		/// Selects the specified ellipse.
		/// </summary>
		/// <param name="ellipse">Ellipse to select</param>
		/// <param name="updateSelectedPoints">Determines whether the selected points are updated</param>
		public void SelectEllipse(EllipseViewModel ellipse, bool updateSelectedPoints)
		{
			// Select the ellipse and all its points
			SelectAllPointsOnEllipse(ellipse);

			// Clear out the selected polygon
			SelectedPolygon = null;

			// Clear out the selected line
			SelectedLine = null;

			// Store off the selected ellipse
			SelectedEllipse = ellipse;

			// If the selected points need to be updated then...
			if (updateSelectedPoints)
			{
				// Clear out the selected points
				SelectedPoints.Clear();

				// Select all the points on the line
				SelectedPoints.AddRange(SelectedEllipse.PointCollection);

				// Ellipse points are not editable in the grid because we need to maintain 90 degree
				// angles between the sides of the rectangle
				SelectedPointsReadOnly = true;
			}

			// Since the selected shape changed update the commands
			RefreshToolbarSelectedShapeCommands();
		}

		/// <summary>
		/// Displays the resize adorner for the selected shape.
		/// </summary>
		public void DisplayResizeAdornerForSelectedShape()
		{
			// If there is a selected shape then...
			if (SelectedShape != null)
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
		/// <param name="crossHashPolygon">Polygon the mouse is over or NULL</param>
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
		/// <returns>True when the mouse is over the line's center cross hash.</returns>
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
		/// Returns true when the mouse is over the ellipse's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="crossHashEllipse">Ellipse the mouse is over or NULL</param>
		/// <returns>True when the mouse is over the ellipse's center cross hash.</returns>
		private bool IsMouseOverEllipseCenterCrossHash(Point mousePosition, ref EllipseViewModel crossHashEllipse)
		{
			// Default to not being over the center of an ellipse
			ShapeViewModel crossHashShape = null;

			// Default to not being over the center cross hash
			bool mouseOverCenterCrossHash = IsMouseOverShapeCenterCrossHash(
				mousePosition,
				ref crossHashShape,
				Ellipses.Cast<ShapeViewModel>().ToList());

			// If the mouse was over an ellipse cross hash then return the ellipse reference
			crossHashEllipse = crossHashShape as EllipseViewModel;

			return mouseOverCenterCrossHash;
		}

		/// <summary>
		/// Returns true when the mouse is over one of the shape's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="crossHashShape">Shape the mouse is over or NULL</param>
		/// <param name="shapes">Shapes to inspect</param>
		/// <returns>True if the mouse is over a shape's cross hash</returns>
		private bool IsMouseOverShapeCenterCrossHash(
			Point mousePosition, 
			ref ShapeViewModel crossHashShape,
			List<ShapeViewModel> shapes)
		{
			// Default to not being over the center cross hash
			crossHashShape = null;

			// Default to not being over the center cross hash
			bool mouseOverCenterCrossHash = false;

			// Loop over the shape view models
			foreach (ShapeViewModel shape in shapes)
			{
				// If the mouse is over the center cross hash then...
				if (shape.IsOverCenterCrossHash(mousePosition))
				{
					// Set shape the mouse is over
					crossHashShape = shape;

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
		/// Once a WPF radio button in a radio button group is set a radio button in that
		/// group has to be set.  Since ShowLabels is a group of one we need extra logic to
		/// maintain the desired state.
		/// </summary>
		private bool PreviousShowLabels { get; set; }

		/// <summary>
		/// Flag that indicates a change was made in the point grid.
		/// </summary>
		private bool GridEditMade { get; set; }

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
		/// Returns true if the specified point is off the drawing canvas.
		/// </summary>
		/// <param name="point">Point to test</param>
		/// <returns>True if the point is off the drawing canvas</returns>
		private bool PointIsOffCanvas(Point point)
		{
			// Returns true if the point is off the drawing canvas
			return (point.X < 0 ||
			        point.Y < 0 ||
			        point.X > ActualWidth - 1 ||
			        point.Y > ActualHeight - 1);
		}

		/// <summary>
		/// Aborts an incomplete line.
		/// </summary>
		private void RemovePartialLine()
		{
			// If creating a new line is in progress then...
			if (NewLine != null)
			{
				// Remove the partial line
				Lines.Remove(NewLine);
				LineModels.Remove(NewLine.Line);
				NewLine = null;
				SelectedLine = null;
			}
		}

		/// <summary>
		/// Aborts a new incomplete polygon.
		/// </summary>
		private void RemovePartialPolygon()
		{
			// If creating a new polygon is in progress then...
			if (NewPolygon != null)
			{
				// Remove the partial polygon
				Polygons.Remove(NewPolygon);
				PolygonModels.Remove(NewPolygon.Polygon);
				NewPolygon = null;
				SelectedPolygon = null;
			}
		}

		/// <summary>
		/// Event handler for when the current cell in the point grid has changed.
		/// </summary>
		private void CurrentCellChanged()
		{
			// If an edit was made in the point grid then...
			if (GridEditMade)
			{
				// If there are more than one selected point then...
				if (SelectedPoints.Count > 1)
				{
					// Remove the resize adorner
					RaiseRemoveResizeAdorner(false);

					// Display the resize adorner
					RaiseDisplayResizeAdornerForSelectedPoints();
				}
			}

			// Clear the grid edit flag
			GridEditMade = false;
		}

		/// <summary>
		/// Event handler for when an edit is made in the point grid
		/// </summary>
		private void CellEditEnding()
		{
			// Remember that an edit was made
			GridEditMade = true;
		}

		/// <summary>
		/// Refreshes toolbar commands impacted by the selected shape.
		/// </summary>
		private void RefreshToolbarSelectedShapeCommands()
		{
			((Command)PasteCommand).RaiseCanExecuteChanged();
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)CutCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartSideCommand).RaiseCanExecuteChanged();
			((Command)ToggleStartPointCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Refreshes toolbar commands impacted by the selection vs draw mode.
		/// </summary>
		private void RefreshToolbarSelectionModeCommands()
		{
			((Command) AddPointCommand).RaiseCanExecuteChanged();
			((Command) DrawPolygonCommand).RaiseCanExecuteChanged();
			((Command) DrawLineCommand).RaiseCanExecuteChanged();
			((Command) DrawEllipseCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Refreshes toolbar command for converting shapes to different types.
		/// </summary>
		private void RefreshToolbarConvertCommands()
		{
			((Command) ConvertToLineCommand).RaiseCanExecuteChanged();
			((Command) ConvertToPolygonCommand).RaiseCanExecuteChanged();
			((Command) ConvertToEllipseCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Handles the polygon/line canvas left button down command.
		/// </summary>
		/// <param name="args"></param>
		private void CanvasMouseLeftButtonDown(MouseEventArgs args)
		{
			// Get the mouse click position
			Point clickPosition = args.GetPosition(Canvas);

			// These mouse move commands seem to get called when the mouse is outside the canvas area.
			if (clickPosition.X < ActualWidth)
			{
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

								// Select the polygon so all the points show up in the point grid
								SelectPolygon(polygon, true);

								// If the number of points is greater than four then...
								if (polygon.PointCollection.Count > 4)
								{
									// A wipe is no longer applicable so hide the green line
									polygon.SegmentsVisible = false;
								}
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
						// Add a line point at the click position
						AddLinePoint(clickPosition);
					}
					// Otherwise if we are in draw ellipse mode then...
					else if (DrawEllipse)
					{
						// Add a default ellipse at the click position
						AddEllipse(clickPosition);
					}
				}
				else // Editor is in Select mode
				{
					Canvas.CaptureMouse();

					// Attempt to select either a polygon, line, ellipse or point
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
				else if (IsSelecting &&
				         SelectedEllipse != null &&
				         SelectedEllipse.AllPointsSelected)
				{
					// Display the resize adorner for the selected ellipse
					RaiseDisplayResizeAdorner();
				}
			}
		}

		/// <summary>
		/// Adds an ellipse at the specified point.
		/// </summary>
		/// <param name="clickPosition">Position of mouse</param>
		private void AddEllipse(Point clickPosition)
		{
			// Create a new ellipse model
			Ellipse ellipseModel = new Ellipse();

			// Create a new ellipse view model
			EllipseViewModel ellipseViewModel = new EllipseViewModel(ellipseModel, ShowLabels);
			
			// Default the smallest dimension of the display element to be the height
			double smallestDimension = ActualHeight;

			// Check to see if the width is smaller than the height
			if (ActualWidth < smallestDimension)
			{
				// Set the smallest dimension to the width
				smallestDimension = ActualWidth;
			}

			// Create dimensions 20 percent the size of the smallest dimension of the display element
			int left = (int)(clickPosition.X - smallestDimension * 0.1);
			int top = (int)(clickPosition.Y - smallestDimension * 0.1);
			int right = (int)(clickPosition.X + smallestDimension * 0.1);
			int bottom = (int)(clickPosition.Y + smallestDimension * 0.1);

			// Create points based on the 20% factor
			Point topLeft = new Point(left, top);
			topLeft = LimitPointToCanvas(topLeft);

			Point topRight = new Point(right, top);
			topRight = LimitPointToCanvas(topRight);

			Point bottomRight = new Point(right, bottom);
			bottomRight = LimitPointToCanvas(bottomRight);

			Point bottomLeft = new Point(left, bottom);
			bottomLeft = LimitPointToCanvas(bottomLeft);

			// Add the points to the ellipse view model
			ellipseViewModel.AddPoint(topLeft);
			ellipseViewModel.AddPoint(topRight);
			ellipseViewModel.AddPoint(bottomRight);
			ellipseViewModel.AddPoint(bottomLeft);

			// Add the ellipse model to the collection of models
			EllipseModels.Add(ellipseModel);

			// Add the ellipse view model to the collection of view models
			Ellipses.Add(ellipseViewModel);

			// Initialize the green wipe line on the ellipse
			ellipseViewModel.InitializeGreenLine();

			// If the editor is time frame mode then...
			if (TimeBarVisible)
			{
				// Associate the ellipse with the snapshot
				SelectedSnapshot.EllipseViewModel = ellipseViewModel;

				// Set the fill type to solid
				ellipseModel.FillType = PolygonFillType.Solid;

				// Green line is only necessary when ;the ellipse is a wipe ellipse
				ellipseViewModel.SegmentsVisible = false;

				// Re-selecting the snapshot to make sure editor is bound to the correct polygon
				SelectPolygonSnapshot(SelectedSnapshot);

				// Switch to selection mode
				IsSelecting = true;

				// Refresh the toolbar commands
				RefreshToolbarSelectionModeCommands();
			}

			// Force the view to refresh the converters
			ellipseViewModel.NotifyPointCollectionChanged();
		}

		/// <summary>
		/// Canvas left mouse up event handler.
		/// </summary>        
		private void CanvasMouseLeftButtonUp(MouseEventArgs args)
		{
			// Position of the mouse when the button was released
			Point clickPosition = args.GetPosition(Canvas);

			// The mouse commands are being called when the mouse is not over the canvas
			// If a move point or lasso operation in progress we still need to handle the event/command
			if (clickPosition.X < ActualWidth || MovingPoint || LassoingPoints)
			{
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
		}

		/// <summary>
		/// Creates a new polygon snapshot at the specified normalized time and the specified index.
		/// </summary>
		/// <param name="normalizedTime">Normalized time of the snapshot</param>
		/// <param name="index">Index into the collection of snapshots</param>
		/// <returns>Reference to the new snapshot</returns>
		private PolygonSnapshotViewModel CreatePolygonSnapshot(double normalizedTime, int index)
		{
			// Create the new snapshot
			PolygonSnapshotViewModel snapshot = new PolygonSnapshotViewModel();
			
			// Set the normalized time of the snapshot
			snapshot.NormalizedTime = normalizedTime;
			
			// Convert the normalized time into a pixel based position
			snapshot.Initialize((int)(normalizedTime * AdjustedTimeBarActualWidth) + PolygonSnapshotViewModel.HalfWidth);
			
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

			// Loop over the ellipse view models
			foreach (EllipseViewModel ellipse in Ellipses)
			{
				// Fire the property changed event so that the converters run
				ellipse.NotifyPointCollectionChanged();
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
			SelectedPolygon.SelectShape();
		}

		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="line">Line to select</param>
		private void SelectAllPointsOnLine(LineViewModel line)
		{
			// Store off the selected line
			SelectedLine = line;

			// Select all the points on the line and the center hash
			SelectedLine.SelectShape();
		}

		/// <summary>
		/// Selects the specified ellipse.
		/// </summary>
		/// <param name="ellipse">Ellipse to select</param>
		private void SelectAllPointsOnEllipse(EllipseViewModel ellipse)
		{
			// Store off the selected ellipse
			SelectedEllipse = ellipse;

			// Select all the points on the ellipse and the center hash
			SelectedEllipse.SelectShape();
		}

		/// <summary>
		/// Creates a new polygon.
		/// </summary>
		private void AddNewPolygon()
		{
			// Create a new model polygon
			Polygon polygon = new Polygon();

			// Create a new view model polygon
			NewPolygon = new PolygonViewModel(polygon, ShowLabels);

			// Initialize the new polygon as the SelectedPolygon
			SelectedPolygon = NewPolygon;

			// Save off the model polygon
			PolygonModels.Add(polygon);

			// Save off the view model polygon
			Polygons.Add(NewPolygon);

			// If the effect is in time mode then...
			if (TimeBarVisible)
			{
				// Set the fill type to solid
				polygon.FillType = PolygonFillType.Solid;
			}
		}

		/// <summary>
		/// Creates a new line.
		/// </summary>
		private void AddNewLine()
		{
			// Create a new Line model
			Line line = new Line();

			// Create a new Line view model
			NewLine = new LineViewModel(line, ShowLabels);

			// Initialize the new line as the SelectedLine
			SelectedLine = NewLine;
			
			// Save off the model line
			LineModels.Add(line);

			// Save off the view model polygon
			Lines.Add(NewLine);

			// If the effect is in time mode then...
			if (TimeBarVisible)
			{
				// Set the fill type to solid
				line.FillType = PolygonFillType.Solid;
			}
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
		/// Puts the editor into draw ellipse mode.
		/// </summary>
		private void DrawEllipseAction()
		{
			DrawEllipse = true;
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
				isEnabled = SelectedSnapshot != null &&
				            SelectedSnapshot.PolygonViewModel == null &&
				            SelectedSnapshot.LineViewModel == null &&
				            SelectedSnapshot.EllipseViewModel == null;
			}

			return isEnabled;
		}

		/// <summary>
		/// Returns true when the add polygon point is enabled.
		/// </summary>
		/// <returns>Returns true when the add polygon point is enabled</returns>
		private bool IsAddPolygonPointEnabled()
		{
			// Only enable the add points capability when a polygon exists
			return Polygons.Count > 0;
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
		/// Deletes the selected shape.
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

			// If there is a selected ellipse then...
			if (SelectedEllipse != null)
			{
				// Remove the specified ellipse
				Ellipses.Remove(SelectedEllipse);
				EllipseModels.Remove(SelectedEllipse.Ellipse);

				// Clear out the selected ellipse
				SelectedEllipse = null;
			}

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();
			
			// If we are in Time-Based mode then...
			if (TimeBarVisible)
			{
				// Remove the shape from the snap-shot
				SelectedSnapshot.PolygonViewModel = null;
				SelectedSnapshot.LineViewModel = null;
				SelectedSnapshot.EllipseViewModel = null;
			}

			// Update the enable status of the paste, copy, cut and delete commands
			RefreshToolbarSelectedShapeCommands();
			RefreshToolbarSelectionModeCommands();
		}

		/// <summary>
		/// Deletes the selected shape or selected polygon points.
		/// </summary>
		private void DeleteShapeOrPoints()
		{
			// If a shape can be deleted then...
			if (CanExecuteDelete())
			{
				// Delete the currently selected shape
				Delete();
			}
			// Otherwise try to delete the selected points
			else
			{
				// Attempt to delete all the points
				int index = 0;
				while (index < SelectedPoints.Count && SelectedPoints.Count > 0)
				{
					// Retrieve the specified point
					PolygonPointViewModel pt = SelectedPoints[index];

					// Find the point's parent polygon
					// Note that deleting a point from a line or ellipse is not supported
					PolygonViewModel polygon = FindPolygon(pt);

					// If the parent polygon was found and the polygon has more than three points then...
					if (polygon != null && polygon.PointCollection.Count > 3)
					{
						// Delete the specified point
						polygon.DeletePoint(pt);

						// If the time bar is NOT visible and
						// the polygon was previously a wipe polygon and
						// the point count is 3 or 4 then...
						if (!TimeBarVisible &&
							polygon.Polygon.FillType == PolygonFillType.Wipe &&
						    (polygon.PointCollection.Count == 4 ||
						     polygon.PointCollection.Count == 3))
						{
							// Display the green line
							polygon.SegmentsVisible = true;
						}
					}
					else
					{
						// Otherwise move onto the next point
						index++;
					}
				}

				// Remove the resize adorner since it is possible points were removed
				RaiseRemoveResizeAdorner();
			}
		}

		/// <summary>
		/// Turns/on off the polygon/line labels.
		/// </summary>
		private void ToggleLabels()
		{
			// If the ShowLabels was previously true then...
			if (ShowLabels && PreviousShowLabels)
			{
				// Turn off the labels
				ShowLabels = false;
			}

			// Loop over all the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Toggle the visibility of the labels
				polygon.LabelVisible = ShowLabels;
			}

			// Loop over all the lines
			foreach (LineViewModel line in Lines)
			{
				// Toggle the visibility of the labels
				line.LabelVisible = ShowLabels;
			}

			// Loop over all the ellipses
			foreach (EllipseViewModel ellipse in Ellipses)
			{
				// Toggle the visibility of the labels
				ellipse.LabelVisible = ShowLabels;
			}

			// Save off the current state of the labels
			PreviousShowLabels = ShowLabels;
		}

		/// <summary>
		/// Toggles the start point of a line.
		/// </summary>
		private void ToggleStartPoint()
		{
			// Forward the request on to the line
			SelectedLine.ToggleStartPoint();

			// Deselect all shapes
			DeselectAllShapes();

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();
		}

		/// <summary>
		/// Converts the selected shape into a polygon.
		/// </summary>
		private void ConvertToPolygon()
		{
			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Remove the selected shape if it is a line
				if (SelectedShape is LineViewModel)
				{
					LineModels.Remove(SelectedLine.Line);
					Lines.Remove(SelectedLine);
				}
				// Otherwise remove the selected ellipse
				else if (SelectedShape is EllipseViewModel)
				{
					EllipseModels.Remove(SelectedEllipse.Ellipse);
					Ellipses.Remove(SelectedEllipse);
				}
			}
			// If the editor only allows one shape then clear the lines and ellipses
			else
			{
				// Clear the line collections
				Lines.Clear();
				LineModels.Clear();

				// Clear the ellipse collections
				Ellipses.Clear();
				EllipseModels.Clear();
			}

			// Create a new polygon
			Polygon polygon = new Polygon();
			PolygonModels.Add(polygon);
			Polygons.Add(new PolygonViewModel(polygon, ShowLabels));

			// Configure the polygon to be the size of the display element
			Polygons[Polygons.Count - 1].AddPoint(new Point(0, 0));
			Polygons[Polygons.Count - 1].AddPoint(new Point(ActualWidth - 1, 0));
			Polygons[Polygons.Count - 1].AddPoint(new Point(ActualWidth - 1 , ActualHeight - 1));
			Polygons[Polygons.Count - 1].AddPoint(new Point(0, ActualHeight - 1));
			Polygons[Polygons.Count - 1].AddPoint(new Point(0, 0));

			// If the time bar is visible then...
			if (TimeBarVisible)
			{
				// Set the fill type to solid
				polygon.FillType = PolygonFillType.Solid;

				// Associate the polygon with the snapshot
				SelectedSnapshot.PolygonViewModel = Polygons[Polygons.Count - 1];
				SelectedSnapshot.LineViewModel = null;
				SelectedSnapshot.EllipseViewModel = null;

				// Select the snapshot
				SelectPolygonSnapshot(SelectedSnapshot);
			}

			// Deselect all shapes and remove the resize adorner
			DeselectAllShapes();
			RaiseRemoveResizeAdorner();

			// Refresh the toolbar buttons
			RefreshToolbarConvertCommands();
		}

		/// <summary>
		/// Converts the selected shape into a line.
		/// </summary>
		private void ConvertToLine()
		{
			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Remove the selected shape if it is a polygon
				if (SelectedShape is PolygonViewModel)
				{
					PolygonModels.Remove(SelectedPolygon.Polygon);
					Polygons.Remove(SelectedPolygon);
				}
				// Otherwise remove the selected ellipse
				else if (SelectedShape is EllipseViewModel)
				{
					EllipseModels.Remove(SelectedEllipse.Ellipse);
					Ellipses.Remove(SelectedEllipse);
				}
			}
			// If the editor only allows one shape then clear the polygons and ellipses
			else
			{
				// Clear the polygon collections
				Polygons.Clear();
				PolygonModels.Clear();

				// Clear the ellipse collections
				Ellipses.Clear();
				EllipseModels.Clear();
			}

			// Create a new line
			Line line = new Line();
			LineModels.Add(line);
			LineViewModel lineViewModel = new LineViewModel(line, ShowLabels);
			Lines.Add(lineViewModel);

			// Initialize the line to be on the left of the display element extending the length of the display element
			PolygonPoint pt1 = new PolygonPoint();
			PolygonPoint pt2 = new PolygonPoint();
			line.Points.Add(pt1);
			line.Points.Add(pt2);
			Lines[0].StartPoint = new PolygonPointViewModel(pt1, lineViewModel);
			Lines[0].EndPoint = new PolygonPointViewModel(pt2, lineViewModel);
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
				// Set the fill type to solid
				line.FillType = PolygonFillType.Solid;

				// Associate the line with the snapshot
				SelectedSnapshot.PolygonViewModel = null;
				SelectedSnapshot.EllipseViewModel = null;
				SelectedSnapshot.LineViewModel = Lines[0];

				SelectPolygonSnapshot(SelectedSnapshot);
			}

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();

			// Select the new line
			SelectLine(lineViewModel, true);

			// Display the resize adorner
			RaiseDisplayResizeAdorner();

			// Refresh the toolbar buttons
			RefreshToolbarConvertCommands();
		}

		/// <summary>
		/// Converts the selected shape into an ellipse.
		/// </summary>
		void ConvertToEllipse()
		{
			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Remove the selected shape if it is a polygon
				if (SelectedShape is PolygonViewModel)
				{
					PolygonModels.Remove(SelectedPolygon.Polygon);
					Polygons.Remove(SelectedPolygon);
				}
				// Otherwise remove the selected line
				else if (SelectedShape is LineViewModel)
				{
					LineModels.Remove(SelectedLine.Line);
					Lines.Remove(SelectedLine);
				}
			}
			// If the editor only allows one shape then clear the polygons and lines
			else
			{
				// Clear the polygon collections
				Polygons.Clear();
				PolygonModels.Clear();

				// Clear the line collections
				Lines.Clear();
				LineModels.Clear();
			}

			// If the time bar is visible then...
			if (TimeBarVisible)
			{
				SelectedSnapshot.LineViewModel = null;
				SelectedSnapshot.PolygonViewModel = null;
			}

			// Add the ellipse in the center of the editor
			Point clickPosition = new Point(ActualWidth / 2.0, ActualHeight / 2.0);
			AddEllipse(clickPosition);

			// Deselect all shapes and remove the resize adorner
			DeselectAllShapes();
			RaiseRemoveResizeAdorner();

			// Refresh the toolbar buttons
			RefreshToolbarConvertCommands();
		}

		/// <summary>
		/// Toggles the start side of a polygon or ellipse.
		/// </summary>
		private void ToggleStartSide()
		{
			if (SelectedPolygon != null)
			{
				// Forward the request to the selected polygon
				SelectedPolygon.ToggleStartSide();
			}
			else if (SelectedEllipse != null)
			{
				// Forward the request to the selected ellipse
				SelectedEllipse.ToggleStartSide();
			}
		}

		/// <summary>
		/// Returns true if a polygon or ellipse is selected.
		/// </summary>
		/// <returns>true if a polygon or ellipse is selected</returns>
		private bool IsToggleableShapeSelected()
		{
			return IsPolygonSelected() || IsEllipseSelected();
		}

		/// <summary>
		/// Adds a new polygon snapshot.
		/// </summary>
		private void AddPolygonSnapshot()
		{
			// Retrieve the index of the currently selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapshot);

			// Calculate the time of the new polygon; attempt to move it to the right
			double time = (SelectedSnapshot.Time + 20) / AdjustedTimeBarActualWidth;

			// If the time is off the scale
			if (time > 1.0)
			{
				// Set the time to the maximum
				time = 1.0;
			}

			// Create a new polyogn snapshot at the specified time and index
			PolygonSnapshotViewModel snapshot = CreatePolygonSnapshot(time, index + 1);

			// If a polygon is associated with the snapshot then...
			if (SelectedSnapshot.PolygonViewModel != null)				
			{
				// Clone the polygon of the selected snapshot
				PolygonViewModel newPolygonViewModel = new PolygonViewModel(
					SelectedSnapshot.PolygonViewModel.Polygon.Clone(), ShowLabels);
				
				// Associate the cloned polygon with the snapshot
				snapshot.PolygonViewModel = newPolygonViewModel;
			}
			// If a line is associated with the snapshot then...
			else if (SelectedSnapshot.LineViewModel != null)
			{
				// Associate the cloned line with the snapshot
				snapshot.LineViewModel = new LineViewModel(SelectedSnapshot.LineViewModel.Line.Clone(), ShowLabels);
			}
			// If an ellipse is associated with the snapshot then...
			else if (SelectedSnapshot.EllipseViewModel != null)
			{
				// Clone the ellipse of the selected snapshot
				EllipseViewModel newEllipseViewModel = new EllipseViewModel(SelectedSnapshot.EllipseViewModel.Ellipse.Clone(), ShowLabels);

				// Associate the cloned line with the snapshot
				snapshot.EllipseViewModel = newEllipseViewModel;
			}

			// Select the newly created snapshot
			SelectPolygonSnapshot(snapshot);

			// Update the toolbar commands
			((Command)DeletePolygonSnapshotCommand).RaiseCanExecuteChanged();
		}
	
		/// <summary>
		/// Deletes the selected snapshot.
		/// </summary>
		private void DeletePolygonSnapshot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapshot);

			// Remove the selected snapshot
			PolygonSnapshots.Remove(SelectedSnapshot);

			// If the index is not the first index then...
			if (index > 0)
			{
				// Select the previous snapshot
				SelectPolygonSnapshot(PolygonSnapshots[index - 1]);
			}
			// Otherwise
			else
			{
				// Select the new first snapshot
				SelectPolygonSnapshot(PolygonSnapshots[0]);
			}
						
			// Update the toolbar commands
			((Command)DeletePolygonSnapshotCommand).RaiseCanExecuteChanged();			
		}

		/// <summary>
		/// Selects the next polygon snapshot.
		/// </summary>
		private void NextPolygonSnapshot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapshot);

			// Select the next polygon snapshot to the right
			SelectPolygonSnapshot(PolygonSnapshots[index + 1]);
			
			// Update the toolbar commands
			((Command)NextPolygonSnapshotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapshotCommand).RaiseCanExecuteChanged();
		}
		
		/// <summary>
		/// Selects the previous snapshot.
		/// </summary>
		private void PreviousPolygonSnapshot()
		{
			// Get the index of the selected snapshot
			int index = PolygonSnapshots.IndexOf(SelectedSnapshot);

			// Select the next polygon snapshot to the left
			SelectPolygonSnapshot(PolygonSnapshots[index - 1]);

			// Update the toolbar commands
			((Command)NextPolygonSnapshotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapshotCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Returns true if the Next Polygon Snapshot command is enabled.
		/// </summary>
		/// <returns>True if the Next Polygon Snapshot command is enabled</returns>
		private bool IsNextPolygonSnapshotEnabled()
		{
			// Command is enabled if the current snapshot is not the last snapshot
			return PolygonSnapshots.IndexOf(SelectedSnapshot) < PolygonSnapshots.Count - 1;
		}

		/// <summary>
		/// Returns true if the Previous Polygon Snapshot command is enabled.
		/// </summary>
		/// <returns>True if the Previous Polygon Snapshot command is enabled</returns>
		private bool IsPreviousPolygonSnapshotEnabled()
		{
			// Command is enabled if the current snapshot is not the first snapshot
			return PolygonSnapshots.IndexOf(SelectedSnapshot) != 0;
		}

		/// <summary>
		/// Updates the mouse cursor for the specified location.
		/// </summary>
		/// <param name="mousePosition">Mouse position</param>
		private void UpdateCursor(Point mousePosition)
		{
			// Default to the normal arrow cursor
			Cursor cursor = Cursors.Arrow;

			PolygonSnapshotViewModel snapshot = null;
			if (IsMouseOverTimeBar(mousePosition, ref snapshot))
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
			// Indicate the event has been handled
			eventArgs.Handled = true;

			// Get the position of the mouse
			Point mousePosition = eventArgs.GetPosition((IInputElement)eventArgs.OriginalSource);

			// Update the cursor based on what the mouse is over
			UpdateCursor(mousePosition);

			// If a snapshot is being moved then...
			if (MovingSnapshot)
			{				
				// Move the snapshot
				MovePolygonSnapshot(SelectedSnapshot, (int)mousePosition.X);
			}
		}

		/// <summary>
		/// Handles the time bar left mouse down command.
		/// </summary>
		/// <param name="eventArgs">Mouse event arguments</param>
		private void MouseLeftButtonDownTimeBar(MouseEventArgs eventArgs)
		{
			// Indicate the event has been handled
			eventArgs.Handled = true;

			// Get the position of the mouse
			Point clickPosition = eventArgs.GetPosition((Canvas)eventArgs.OriginalSource);
			
			PolygonSnapshotViewModel polygonSnapshot = null;

			// If the mouse is over a snapshot then...
			if (IsMouseOverTimeBar(clickPosition, ref polygonSnapshot))
			{
				// Indicate we are moving a snapshot
				MovingSnapshot = true;

				// Select the snapshot under the mouse
				SelectPolygonSnapshot(polygonSnapshot);
			}
		}

		/// <summary>
		/// Handles the time bar mouse left button up command.
		/// </summary>
		/// <param name="eventArgs"></param>
		private void MouseLeftButtonUpTimeBar(MouseEventArgs eventArgs)
		{
			// Indicate the event has been handled
			eventArgs.Handled = true;

			// Get the position of the mouse
			Point clickPosition = eventArgs.GetPosition((Canvas)eventArgs.OriginalSource);

			// If a snapshot is being moved then...
			if (MovingSnapshot)
			{				
				// Move the selected snapshot to the specified position
				MovePolygonSnapshot(SelectedSnapshot, (int)clickPosition.X);
			}

			// Clear the flag that indicates a snapshot move is in progress
			MovingSnapshot = false;
		}

		/// <summary>
		/// Handles the time bar mouse leave command.
		/// </summary>
		private void MouseLeaveTimeBar()
		{
			// If the mouse leaves the time bar it ends the snapshot move
			MovingSnapshot = false;
		}
				
		/// <summary>
		/// Moves the specified polygon to the specified position.
		/// </summary>
		/// <param name="polygonSnapshot">Polygon to move</param>
		/// <param name="position">Position to move the snapshot to</param>
		public void MovePolygonSnapshot(PolygonSnapshotViewModel polygonSnapshot, int position)
		{			
			// If the pointer has gone off the scale to the left then...
			if (position < PolygonSnapshotViewModel.HalfWidth)
			{
				// Reset the position to the start of the scale
				position = PolygonSnapshotViewModel.HalfWidth;
			}

			// If the pointer has gone off the scale to the right then...
			if (position > (AdjustedTimeBarActualWidth + PolygonSnapshotViewModel.HalfWidth))
			{
				// Reset the position to the end of the scale
				position = (int)(AdjustedTimeBarActualWidth + PolygonSnapshotViewModel.HalfWidth);
			}

			// Forward the command to the polygon snapshot
			polygonSnapshot.Move(position);

			// Calculate the normalized time (0.0 - 1.0)
			polygonSnapshot.NormalizedTime = (position - PolygonSnapshotViewModel.HalfWidth) / AdjustedTimeBarActualWidth;
			Debug.Assert(polygonSnapshot.NormalizedTime >= 0.0);
			Debug.Assert(polygonSnapshot.NormalizedTime <= 1.0);

			// Sort the snapshots by the associated time
			List<PolygonSnapshotViewModel> sortedSnapshots = PolygonSnapshots.OrderBy(snapShot => snapShot.Time).ToList();
			
			// Clear the collection of snapshots
			PolygonSnapshots.Clear();

			// Add the snapshots back in increasing time order
			PolygonSnapshots.AddRange(sortedSnapshots);

			// Force the view to refresh
			RaisePropertyChanged(nameof(PolygonSnapshots));

			// Update the button enabled status
			((Command)NextPolygonSnapshotCommand).RaiseCanExecuteChanged();
			((Command)PreviousPolygonSnapshotCommand).RaiseCanExecuteChanged();
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
				LineViewModel copiedLineVM = new LineViewModel(copiedLine, ShowLabels);

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
				DeselectAllShapes();

				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted line
				SelectLine(copiedLineVM, true);

				// If we are in time based mode then...
				if (EditorCapabilities.ShowTimeBar)
				{
					// Update the snapshot polygon with the copied view model polygon
					SelectedSnapshot.LineViewModel = copiedLineVM;

					// Reselect the polygon snapshot to reconfigure the bindings
					SelectPolygonSnapshot(SelectedSnapshot);
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
				PolygonViewModel copiedPolygonVM = new PolygonViewModel(copiedPolygon, ShowLabels);

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
				DeselectAllShapes();

				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted polygon
				SelectPolygon(copiedPolygonVM, true);

				// If we are in time based mode then...
				if (EditorCapabilities.ShowTimeBar)
				{
					// Update the snapshot polygon with the copied view model polygon
					SelectedSnapshot.PolygonViewModel = copiedPolygonVM;

					// Reselect the polygon snapshot to reconfigure the bindings
					SelectPolygonSnapshot(SelectedSnapshot);
				}

				// Display the resize adorner
				RaiseDisplayResizeAdorner();
			}
			// If the clipboard contains ellipse data then...
			else if (Clipboard.ContainsData(EllipseClipboardFormat))
			{
				// Retrieve the ellipse model object from the clipboard
				Ellipse copiedEllipse = ((Ellipse)Clipboard.GetData(EllipseClipboardFormat)).Clone();

				// Add the model ellipse to the collection
				EllipseModels.Add(copiedEllipse);

				// Create an ellipse view model object
				EllipseViewModel copiedEllipseVM = new EllipseViewModel(copiedEllipse, ShowLabels);

				// Retrieve the center point of the ellipse
				PolygonPointViewModel centerPoint = copiedEllipseVM.CenterPoint;

				// Determine how far from center the clipboard ellipse is positioned
				// This is the distance required to move the ellipse to the center of the editor.
				double moveX = centerX - centerPoint.X;
				double moveY = centerY - centerPoint.Y;

				// Move the points so the ellipse is centered in the middle of the editor
				copiedEllipseVM.MovePoints(moveX, moveY, LimitPointToCanvas);

				// Add the ellipse view model to the collection
				Ellipses.Add(copiedEllipseVM);

				// Fire the Property Changed event to ensure the converters run
				copiedEllipseVM.NotifyPointCollectionChanged();

				// Deselect all shapes
				DeselectAllShapes();
				
				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted ellipse
				SelectEllipse(copiedEllipseVM, true);

				// If we are in time based mode then...
				if (EditorCapabilities.ShowTimeBar)
				{
					// Update the snapshot polygon with the copied view model ellipse
					SelectedSnapshot.EllipseViewModel = copiedEllipseVM;
					
					// Reselect the polygon snapshot to reconfigure the bindings
					SelectPolygonSnapshot(SelectedSnapshot);
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
			else if (IsLineSelected())
			{
				// Copy the line data to the clipboard
				Clipboard.SetData(LineClipboardFormat, SelectedLine.Line.Clone());
			}
			else
			{
				Clipboard.SetData(EllipseClipboardFormat, SelectedEllipse.Ellipse.Clone());
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
			// Return whether polygon, line, or ellipse data is on the clipboard and 
			// the editor is configured to allow pasting new shapes
			bool canExecutePaste =
				(Clipboard.ContainsData(PolygonClipboardFormat) ||
				 Clipboard.ContainsData(LineClipboardFormat) ||
				 Clipboard.ContainsData(EllipseClipboardFormat)) &&
				   EditorCapabilities != null &&
				   EditorCapabilities.AddPolygons;

			// If the editor is in time bar mode and the paste command would be enabled then...
			if (_editorCapabilities.ShowTimeBar && canExecutePaste)
			{
				// Only enable paste when the snapshot doesn't already have a shape
				canExecutePaste = SelectedSnapshot.PolygonViewModel == null &&
				                  SelectedSnapshot.EllipseViewModel == null &&
				                  SelectedSnapshot.LineViewModel == null;
			}

			// Return whether it is valid to enable paste
			return canExecutePaste;
		}

		/// <summary>
		/// Returns true when the Delete command can execute.
		/// </summary>
		/// <returns>True when the Delete command can execute</returns>
		private bool CanExecuteDelete()
		{
			// Return whether polygon, line, or ellipse is selected and 
			// the editor is configured to allow deleting shapes
			return (IsPolygonSelected() || IsLineSelected() || IsEllipseSelected()) &&
				EditorCapabilities != null &&
				EditorCapabilities.DeletePolygons;
		}

		/// <summary>
		/// Returns true when a shaped is selected.
		/// </summary>
		/// <returns>True when a shape is selected</returns>
		private bool IsShapeSelected()
		{
			return IsPolygonSelected() || IsLineSelected() || IsEllipseSelected();
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
		/// Returns true if an ellipse is selected.
		/// </summary>
		/// <returns>True if an ellipse is selected</returns>
		private bool IsEllipseSelected()
		{
			return IsSelecting &&
			       SelectedEllipse != null;
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
		/// Returns true if a shape can be converted to an ellipse.
		/// </summary>
		/// <returns>True if the shape can be converted</returns>
		private bool ConvertToEllipseEnabled()
		{
			// Default to NOT enabling the command
			bool convertToEllipseEnabled = false;

			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Return true if the selected shape is not an ellipse
				convertToEllipseEnabled =
					SelectedShape != null &&
					!(SelectedShape is EllipseViewModel) &&
					IsSelecting;
			}
			// Otherwise only allow the conversion if the current shape is NOT an ellipse
			else
			{
				convertToEllipseEnabled = (Ellipses.Count == 0) && IsSelecting;
			}

			return convertToEllipseEnabled;
		}

		/// <summary>
		/// Returns true if a shape can be converted to a polygon.
		/// </summary>
		/// <returns>True if the shape can be converted</returns>
		private bool ConvertToPolygonEnabled()
		{
			// Default to NOT enabling the command
			bool convertToPolygonEnabled = false;

			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Return true if the selected shape is not a polygon
				convertToPolygonEnabled =
					SelectedShape != null &&
					!(SelectedShape is PolygonViewModel) &&
					IsSelecting;
			}
			// Otherwise only allow the conversion if the current shape is NOT a polygon
			else
			{
				convertToPolygonEnabled = (Polygons.Count == 0) && IsSelecting;
			}

			return convertToPolygonEnabled;
		}

		/// <summary>
		/// Returns true if a shape can be converted to a line.
		/// </summary>
		/// <returns>True if the shape can be converted</returns>
		private bool ConvertToLineEnabled()
		{
			// Default to NOT enabling the command
			bool convertToLineEnabled = false;

			// If the editor is configured to allow multiple shapes then...
			if (EditorCapabilities.AllowMultipleShapes)
			{
				// Return true if the selected shape is not a line
				convertToLineEnabled =
					IsSelecting &&
					SelectedShape != null && 
				    !(SelectedShape is LineViewModel);
			}
			// Otherwise only allow the conversion if the current shape is NOT a polygon
			else
			{
				convertToLineEnabled = (Lines.Count == 0) && IsSelecting;
			}

			return convertToLineEnabled;
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

			// If a new polygon is being created and
			// the polygon contains 2 points and the 
			// mouse is over the first point then...
			if (NewPolygon != null &&
			    NewPolygon.PointCollection.Count >= 2 &&
			    NewPolygon.IsMouseOverFirstPolygonPoint(mousePosition))
			{
				// Use the cross cursor to indicate the polygon can be closed
				cursor = Cursors.Cross;
			}
			// If the mouse is over the center of a shape then...
			else if (IsSelecting && 
			         IsMouseOverCenterOfShape(mousePosition))
			{
				// Use special cross cursor to indicate the shape can be selected
				cursor = Cursors.Cross;
			}
			// If the editor is in selection mode and
			// the mouse over a moveable item then....
			else if (IsSelecting && 
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
		/// <param name="mouseEventArgs">Mouse event arguments</param>
		private void CanvasMouseMove(MouseEventArgs mouseEventArgs)
		{
			// Get the position of the mouse click
			Point mousePosition = mouseEventArgs.GetPosition(Canvas);
			
			// Update the cursor based on what the mouse is over
			UpdateCanvasCursor(mousePosition);

			// If the mouse if off the canvas and we are creating
			// a new polygon or line then...
			if (PointIsOffCanvas(mousePosition) &&
			    (DrawPolygon || DrawLine))
			{
				// Hide the ghost line
				MovingPointVisibilityPrevious = false;

				// If we are in draw polygon then...
				if (DrawPolygon)
				{
					// Remove the partial polygon
					RemovePartialPolygon();
					AddNewPolygon();
				}
				// Otherwise we are in draw line mode then...
				else if (DrawLine)
				{
					// Remove the partial line
					RemovePartialLine();
					AddNewLine();
				}
			}

			// If a point is being moved then...
			if (MovingPoint)
			{
				// Move the currently selected point
				MovePoint(mousePosition);
			}
			// If a new polygon is being created and 
			// the polygon is NOT closed
			// and there is at least one point then...
			else if (NewPolygon != null &&
			         !NewPolygon.IsClosed &&
			         NewPolygon.PointCollection.Count > 0)
			{
				// Update the position of the ghost point 
				PointMoving = new PolygonPointViewModel(new PolygonPoint(), null);
				PointMoving.X = mousePosition.X;
				PointMoving.Y = mousePosition.Y;

				// Set the starting point for the ghost line
				PreviousPointMoving = NewPolygon.PointCollection[NewPolygon.PointCollection.Count - 1];
				
				// Make the ghost point and associated line visible
				MovingPointVisibilityPrevious = true;
			}
			// Otherwise if a new line is being created and
			// the first point has already been created then...
			else if (NewLine != null &&
			         NewLine.PointCollection.Count == 1)
			{
				// Update the position of the ghost point 
				PointMoving = new PolygonPointViewModel(new PolygonPoint(), null);
				PointMoving.X = mousePosition.X;
				PointMoving.Y = mousePosition.Y;

				// Set the starting point for the ghost line
				PreviousPointMoving = NewLine.PointCollection[NewLine.PointCollection.Count - 1];

				// Make the ghost point and associated line visible
				MovingPointVisibilityPrevious = true;
			}

			// If the editor is in selection mode and
			// the user is NOT moving a point and
			// the left mouse button is down and
			// user did not just select a shape and
			// the mouse is over the canvas then...
			if (IsSelecting &&
			    !MovingPoint &&
			    mouseEventArgs.LeftButton == MouseButtonState.Pressed &&
			    !SelectedShapeFlag &&
			    mousePosition.X < ActualWidth)
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

			// Deselect all the ellipses
			DeselectAllEllipses();

			// Since there is no longer a selected shape update the commands
			RefreshToolbarSelectedShapeCommands();
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

			// Clear out the selected polygon
			SelectedPolygon = null;

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

			// Clear out the selected line
			SelectedLine = null;

			// Clear all selected points
			SelectedPoints.Clear();
		}

		/// <summary>
		/// Deselect all ellipses.
		/// </summary>
		public void DeselectAllEllipses()
		{
			// Loop over all the ellipses
			foreach (EllipseViewModel ellipse in Ellipses)
			{
				// Deselect all points on the ellipse
				ellipse.DeselectAllPoints();
			}

			// Clear out the selected ellipse
			SelectedEllipse = null;

			// Clear all selected points
			SelectedPoints.Clear();
		}

		/// <summary>
		/// Selects a polygon, line, ellipse or point based on the click position.
		/// </summary>
		/// <param name="clickPosition">Click position</param>
		public bool SelectPolygonLineOrPoint(Point clickPosition)
		{
			// Default to the mouse not being over a shape
			bool shapeSelected = false;
			
			// Deselect the currently selected polygon or line
			DeselectAllShapes();

			PolygonViewModel selectedPolygon = null;
			LineViewModel selectedLine = null;
			EllipseViewModel selectedEllipse = null;
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
			// If the mouse is over a polygon center cross hash then...
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
			// If the mouse is over the line center cross hash then...
			else if (IsMouseOverLineCenterCrossHash(clickPosition, ref selectedLine))
			{
				// Select the line and all its points
				SelectLine(selectedLine, true);

				// Return that the entire polygon was selected
				shapeSelected = true;
			}
			// If the mouse is over the ellipse center cross hash then...
			else if (IsMouseOverEllipseCenterCrossHash(clickPosition, ref selectedEllipse))
			{
				// Select the ellipse and all its points
				SelectEllipse(selectedEllipse, true);

				// Return that the entire polygon was selected
				shapeSelected = true;
			}
			// Otherwise the mouse isn't over a polygon or line point 
			else
			{
				// Deselect all shapes
				DeselectAllShapes();
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
				// Hide the ghost point and associated line
				MovingPointVisibilityPrevious = false;

				// If the editor is time frame mode then...
				if (TimeBarVisible)
				{
					// Associate the polygon with the snapshot
					SelectedSnapshot.PolygonViewModel = NewPolygon;

					// Re-selecting the snapshot to make sure editor is bound to the correct polygon
					SelectPolygonSnapshot(SelectedSnapshot);

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
			RefreshToolbarSelectionModeCommands();
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
				// Hide the ghost point and associated line
				MovingPointVisibilityPrevious = false;

				// Save off the start point
				NewLine.StartPoint = NewLine.PointCollection[0];

				// Save off the end point
				NewLine.EndPoint = NewLine.PointCollection[1];
				
				// If the editor is time frame mode then...
				if (TimeBarVisible)
				{
					// Associate the polygon with the snapshot
					SelectedSnapshot.LineViewModel = NewLine;

					// Re-selecting the snapshot to make sure editor is bound to the correct line
					SelectPolygonSnapshot(SelectedSnapshot);

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
		public void InitializePolygonSnapshots(
			IList<Polygon> polygons, 
			IList<double> polygonTimes,
			IList<Line> lines,
			IList<double> lineTimes,
			IList<Ellipse> ellipses,
			IList<double> ellipseTimes)
		{			
			// Loop over the polygon models
			int index = 0;
			foreach (Polygon polygon in polygons)
			{
				// Scale the points for the editor canvas size
				polygon.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);

				// Create the snapshot 
				PolygonSnapshotViewModel snapshot = CreatePolygonSnapshot(polygonTimes[index], PolygonSnapshots.Count);
					
				// Associate the polygon with the snapshot
				snapshot.PolygonViewModel = new PolygonViewModel(polygon, ShowLabels);
				index++;
			}
								
			// Loop over the model lines
			index = 0;
			foreach (Line line in lines)
			{
				// Scale the points for the editor canvas size
				line.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);

				// Create the snapshot 
				PolygonSnapshotViewModel snapshot = CreatePolygonSnapshot(lineTimes[index], PolygonSnapshots.Count);
					
				// Associate the line with the snapshot
				snapshot.LineViewModel = new LineViewModel(line, ShowLabels);
				index++;
			}

			// Loop over the model ellipses
			index = 0;
			foreach (Ellipse ellipse in ellipses)
			{
				// Scale the points for the editor canvas size
				ellipse.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);

				// Create the snapshot 
				PolygonSnapshotViewModel snapshot = CreatePolygonSnapshot(ellipseTimes[index], PolygonSnapshots.Count);

				// Associate the ellipse with the snapshot
				snapshot.EllipseViewModel = new EllipseViewModel(ellipse, ShowLabels);
				index++;
			}

			// Select the first snapshot
			SelectPolygonSnapshot(PolygonSnapshots[0]);				
		}

		/// <summary>
		/// Update the position of the snapshot sliders when the view is resized.
		/// </summary>
		public void UpdatePolygonSnapshots()
		{
			// Loop over the snapshots
			foreach(PolygonSnapshotViewModel snapShot in PolygonSnapshots)
			{
				// Update the snapshots position based on the normalized time
				snapShot.Move((int)((snapShot.NormalizedTime * AdjustedTimeBarActualWidth) + PolygonSnapshotViewModel.HalfWidth));
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

			// Clear the polygon view models and models		
			Polygons.Clear();
			PolygonModels.Clear();

			// If the editor is NOT in time based mode then...
			if (!EditorCapabilities.ShowTimeBar)			
			{
				// Loop over the polygon models
				foreach (Polygon polygon in polygons)
				{
					// Add the polygon model
					PolygonModels.Add(polygon);

					// Add the polygon view model
					Polygons.Add(new PolygonViewModel(polygon, ShowLabels));
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

			// Clear the line view models and models
			Lines.Clear();
			LineModels.Clear();

			// If the editor is NOT in time mode then...
			if (!EditorCapabilities.ShowTimeBar)
			{
				// Loop over the line models
				foreach (Line line in lines)
				{
					// Add line model
					LineModels.Add(line);

					// Add the line view model
					Lines.Add(new LineViewModel(line, ShowLabels));
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
		/// Initializes the editor with the ellipse models.
		/// </summary>
		/// <param name="ellipses">Model ellipses</param>
		/// <param name="times">Model ellipse times</param>		
		public void InitializeEllipses(IList<Ellipse> ellipses, IList<double> times)
		{
			// Loop over the ellipses
			foreach (Ellipse ellipse in ellipses)
			{
				// Scale the points for the editor canvas size
				ellipse.ScalePoints(PolygonPointXConverter.XScaleFactor, PolygonPointYConverter.YScaleFactor);
			}

			// Clear the line view models and models
			Ellipses.Clear();
			EllipseModels.Clear();

			// If the editor is NOT in time mode then...
			if (!EditorCapabilities.ShowTimeBar)
			{
				// Loop over the ellipse models
				foreach (Ellipse ellipse in ellipses)
				{
					// Create ellipse model
					EllipseModels.Add(ellipse);

					// Create ellipse view model
					Ellipses.Add(new EllipseViewModel(ellipse, ShowLabels));
				}

				// If the editor is configured to be in select mode and 
				// there is only one ellipse then...
				if (_editorCapabilities.DefaultToSelect && Ellipses.Count == 1)
				{
					// Select the ellipse
					SelectEllipse(Ellipses[0], true);
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
			foreach(PolygonSnapshotViewModel snapShot in PolygonSnapshots)
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
		/// Gets the polygon snapshot times.
		/// </summary>
		/// <returns>Collection of normalized (0-1) polygons times</returns>
		public IList<double> GetPolygonTimes()
		{
			// Create the return collection
			List<double> times = new List<double>();

			// Loop over the snapshots
			foreach (PolygonSnapshotViewModel snapShot in PolygonSnapshots)
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
		/// Gets the ellipse snapshot times.
		/// </summary>
		/// <returns>Collection of normalized (0-1) ellipse times</returns>
		public IList<double> GetEllipseTimes()
		{
			// Create the return collection
			List<double> times = new List<double>();

			// Loop over the snapshots
			foreach (PolygonSnapshotViewModel snapShot in PolygonSnapshots)
			{
				// If the snapshot contains an ellipse then...
				if (snapShot.EllipseViewModel != null)
				{
					// Add the time to the collection
					times.Add(snapShot.NormalizedTime);
				}
			}

			// Return the ellipse times
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

			// Line points are editable in the grid
			SelectedPointsReadOnly = false;

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

			// Polygon and line points are editable in the grid
			// Note ellipse points are not supported in this method because
			// of the potential to distort the rectangle angles.
			SelectedPointsReadOnly = false;
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
		/// Returns true if the specified offset is valid for the selected points.
		/// </summary>
		/// <param name="offset">Offset to move the points</param>
		/// <returns>true if the specified offset is valid</returns>
		public bool IsMoveable(Point offset)
		{
			// Default to the move being valid
			bool isMoveable = true;

			// Loop over the selected points
			foreach (PolygonPointViewModel selectedPoint in SelectedPoints)
			{
				// Check that the point is not off the canvas to the left
				if (selectedPoint.X + offset.X < 0)
				{
					isMoveable = false;
				}

				// Check that the point is not off the canvas to the top
				if (selectedPoint.Y + offset.Y < 0)
				{
					isMoveable = false;
				}

				// Check that the point is not off the canvas to the bottom
				if (selectedPoint.Y + offset.Y > ActualHeight - 1)
				{
					isMoveable = false;
				}

				// Check that the point is not off the canvas to the right
				if (selectedPoint.X + offset.X > ActualWidth - 1)
				{
					isMoveable = false;
				}
			}

			return isMoveable;
		}

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

			// If the selected shaped is an ellipse then...
			if (SelectedEllipse != null)
			{
				// Save off the rotation angle
				SelectedEllipse.Angle += angle;
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
				if (PointIsOffCanvas(transformedPoint))
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
		public bool IsTransformValid(TransformGroup scaleTransform)
		{
			// Default to the transform being valid
			bool isTransformValid = true;
			
			// Loop over the selected points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				// Transform the point
				Point transformedPoint = scaleTransform.Transform(point.GetPoint());

				// Round the point to ten fractional digits to account for floating point errors
				// when scaling the points
				double x = Math.Round(transformedPoint.X, 10);
				double y = Math.Round(transformedPoint.Y, 10);

				// If the point is off the canvas to the left
				if (x < 0)
				{
					isTransformValid = false;
				}

				// If the point is off the canvas to the top
				if (y < 0)
				{
					isTransformValid = false;
				}

				// If the point is off the canvas to the right
				if (x > ActualWidth - 1)
				{
					isTransformValid = false;
				}

				// If the point is off the canvas to the bottom
				if (y > ActualHeight - 1)
				{
					isTransformValid = false;
				}
			}

			return isTransformValid;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void TransformSelectedItems(TransformGroup t)
		{
			// Loop over the selected view model points
			foreach (PolygonPointViewModel point in SelectedPoints)
			{						
				// Transform the point
				Point transformedPoint = t.Transform(point.GetPoint());

				// Update the view model point
				point.SuppressChangeEvents = true;
				point.X = transformedPoint.X;						
				point.Y = transformedPoint.Y;
				point.SuppressChangeEvents = false;

				// Make sure the point is still on the drawing canvas
				LimitPointToCanvas(point);											
			}
	
			// Refresh all shapes for the transform
			RefreshAllShapes();

			// Update the resize adorner
			EventHandler handler = RefreshResizeAdorner;
			handler?.Invoke(this, new EventArgs());
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

			// Update the resize adorner
			EventHandler handler = RefreshResizeAdorner;
			handler?.Invoke(this, new EventArgs());
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
