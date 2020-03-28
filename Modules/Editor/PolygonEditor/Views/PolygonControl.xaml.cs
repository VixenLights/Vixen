
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PolygonEditor
{
    /// <summary>
    /// Interaction logic for Polygon.xaml
    /// </summary>
    public partial class PolygonControl 
    {
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PolygonControl()
		{
			InitializeComponent();                       
        }

        #endregion

        #region Fields

        /// <summary>
        /// Rubberband adorner for selecting points.
        /// </summary>
        private RubberbandAdorner _rubberbandAdorner;

        /// <summary>
        /// Resize adorner for resizing, moving, and rotating the points. 
        /// </summary>
        private ResizeAdorner _resizingAdorner;

        /// <summary>
        /// Stores the position of the mouse when the user left clicks.
        /// This field is used to draw the rubber band selection lasso. 
        /// </summary>
        private Point _originMouseStartPoint;

        /// <summary>
        /// True when the user just selected a polygon.
        /// This field inhibits drawing the rubberband adorner when selecting polygon via
        /// the center cross hair.
        /// </summary>
        bool _selectedPolygon;

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to delete a polygon point if one is selected.
        /// </summary>
        /// <returns>True if apoint was deleted.</returns>
        public bool TryDeletePoint()
        {
            bool handled = false;

            // If we can delete a polygon point then...
            if (VM.SelectedPolygon != null &&
                VM.SelectedPoints.Count == 1 &&
                VM.SelectedPolygon.DeletePointCommand.CanExecute(null))
            {
                // Execute the command on the view model
                VM.SelectedPolygon.DeletePointCommand.Execute(null);

                handled = true;                
            }

            return handled;
        }

		#endregion

		#region Public Properties

		private PolygonEditorViewModel _vm;

        /// <summary>
        /// View model for the polygon editor.
        /// </summary>
        public PolygonEditorViewModel VM
        {
            get
            {
                return _vm;
            }
            set
            {
                _vm = value;

                // Register for view model events
                _vm.RefreshResizeAdorner += RefreshResizeAdorner;
                _vm.RemoveResizeAdorner += RemoveResizeAdorner;
                _vm.DisplayResizeAdorner += DisplayResizeAdorner;
                _vm.RepositionResizeAdorner += RepositionResizeAdorner;


                dgSimple.ItemsSource = VM.SelectedPoints;
            }                
        }

		#endregion

		#region Private Methods

        /// <summary>
        /// Updates the mouse cursor for the specified location.
        /// </summary>
        /// <param name="mousePosition">Mouse position</param>
		private void UpdateCursor(Point mousePosition)
        {
            // Default to the normal arrow cursor
            Cursor cursor = Cursors.Arrow;

            PolygonLineSegment lineSegment = null;
            PolygonViewModel polygon = null; 

            // If the editor is in selection mode and
            // the mouse over a moveable item then....
            if (VM.IsSelecting &&
                VM.IsMouseOverMoveableItem(mousePosition))
            {
                // Change to the sizing cursor
                cursor = Cursors.SizeAll;
            }
            else if (!VM.IsSelecting &&                
                     IsMouseOverLine(mousePosition, ref lineSegment, ref polygon))
            {
                if (polygon.PolygonClosed)
                {
                    cursor = //new Cursor("C:\\Users\\JenAndJohn\\source\\repos\\PolygonEditor\\AddPoint.cur");
                        new Cursor(@"E:\SteamLibrary\steamapps\common\Portal 2\portal2_dlc2\materials\puzzlemaker\cursors\cursor_arrow_dragbox.cur");
                }
            }

            // Set cursor for the control
            Cursor = cursor;
        }

        /// <summary>
        /// Updates the rubber band lasso for selecting points.
        /// </summary>
        /// <param name="canvas">Polygon canvas</param>
        /// <param name="mousePosition">Position of the mouse</param>
        private void LassoPoints(Canvas canvas, Point mousePosition)
        {
            // Capture the mouse while lasso'ing points
            canvas.CaptureMouse();

            // Get the adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

            // If the rubber band adorner has not been created then...
            if (adornerLayer != null && _rubberbandAdorner == null)
            {
                // Create the rubber band adorner 
                _rubberbandAdorner = new RubberbandAdorner(canvas, _originMouseStartPoint);
                adornerLayer.Add(_rubberbandAdorner);
            }

            if (_rubberbandAdorner != null)
            {
                // Set the current mouse position as the end of the rubber band area
                _rubberbandAdorner.EndPoint = mousePosition;
            }
        }

        /*
        /// <summary>
        /// Event handler that creates the columns for the DataGridView.
        /// </summary>        
        private void DG1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Determine which columns are visible
            switch (e.Column.Header.ToString())
            {
                case "X":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                case "Y":
                    e.Column.Visibility = Visibility.Visible;
                    break;
                default:
                    e.Column.Visibility = Visibility.Hidden;
                    break;
            }
        }
        */

        /// <summary>
        /// Redraws the resize adorner.
        /// </summary>
        private void RefreshResizeAdorner()
        {
            // Get the adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);

            if (adornerLayer != null)
            {
                // Update the adorners in the layer
                adornerLayer.Update();
            }
        }

        /// <summary>
        /// Updates the resize adorner based on the specified polygon points.
        /// </summary>
        /// <param name="points">Polygon points to include in the resize adorner</param>
        private void UpdateResizeAdorner(ObservableCollection<PolygonPointViewModel> points)
        {
            // Get the bounds of the selected points
            Rect bounds = GetSelectedContentBounds(points);

            // Get the adorner layer from the canvas
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);

            if (adornerLayer != null)
            {
                // Remove the resizing adorner
                RemoveResizeAdorner();

                // Create the resizing adorner
                _resizingAdorner = new ResizeAdorner(this, canvas, VM, bounds);
                
                // Add the resizing adorner to the canvas
                adornerLayer.Add(_resizingAdorner);
            }                            
        }

        #endregion

        #region Private View Model Event Handlers

        /// <summary>
        /// Event handler repositions (re-creates) the resize adorner based  on the selected points.
        /// </summary>        
        private void RepositionResizeAdorner(object sender, EventArgs e)
        {
            // If there are selected points then...
            if (VM.SelectedPoints.Count > 0)
            {
                // Reposition the resizing adorner
                UpdateResizeAdorner(VM.SelectedPoints);
            }
        }

        /// <summary>
        /// Event handler removes the resize adorner.
        /// </summary>        
		private void RemoveResizeAdorner(object sender, EventArgs e)
        {
            // Deselect all polygon points
            VM.DeselectAllPolygons();
            
            // Remove the resize adorner
            RemoveResizeAdorner();            
        }

        /// <summary>
        /// Event handler dipslays the resize adorner for the specified polygon.
        /// </summary>        
        private void DisplayResizeAdorner(object sender, EventArgs e)
        {
            // Update the resize adorner based on the selected polygon
            UpdateResizeAdorner(VM.SelectedPolygon.PointCollection);
        }

        /// <summary>
        /// Event handler that refreshes (redraws) the resize adorner.
        /// </summary>        
        private void RefreshResizeAdorner(object sender, EventArgs e)
        {
            RefreshResizeAdorner();
        }

        /// <summary>
        /// Gets the rectangle bounds of the specified polygon points.
        /// </summary>        
        private Rect GetSelectedContentBounds(ObservableCollection<PolygonPointViewModel> points)
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

        /// <summary>
        /// Removes the resize adorner.
        /// </summary>
        private void RemoveResizeAdorner()
        {
            // If the resize adorner has been created then...                   
            if (_resizingAdorner != null)
            {
                // Retrieve the resize adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);

                // Remove the resize adorner
                adornerLayer.Remove(_resizingAdorner);

                // Release the resize adorner
                _resizingAdorner = null;
            }
        }
        
        #endregion

        #region Private XAML Canvas Events

        /// <summary>
        /// Gives the view model the size of the canvas for limiting move, rotate and resize operations.
        /// </summary>        
        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            // Give the editor view model the boundaries of the canvas
            VM.UpdateEditorSize(canvas.ActualWidth, canvas.ActualHeight);
        }

        /// <summary>
        /// Moves and selects polygon points.
        /// </summary>        
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition((Canvas)sender);

            // Update the cursor based on what the mouse is over
            UpdateCursor(mousePosition);

            // If a point is being moved then...
            if (VM.MovingPoint)
            {
                // Move the currently selected point
                VM.MovePoint(mousePosition);
            }

            // If the editor is in selection mode and
            // the user is NOT moving a point and
            // the left mouse button is down and
            // user did not just select a polygon then...
            if (VM.IsSelecting &&
                !VM.MovingPoint &&                
                e.LeftButton == MouseButtonState.Pressed &&
                !_selectedPolygon)
            {
                // Use the rubber band adorner to select (lasso) points
                LassoPoints(canvas, mousePosition);
            }

            // Indicate the event was handled
            e.Handled = true;
        }
        
        /// <summary>
        /// Canvas left mouse down event handler.
        /// </summary>        
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {        
            // Get the mouse click position
            Point clickPosition = e.GetPosition((Canvas)sender);

            // Store off the mouse click position
            _originMouseStartPoint = clickPosition;

            // Default to not selecting a polygon
            _selectedPolygon = false;

            // If the editor is in polygon draw mode then...
            if (!VM.IsSelecting)
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
                    else
                    {
                        // Add a polygon point at the click position
                        VM.AddPoint(clickPosition);
                    }
                }
                else
                {
                    // Add a polygon point at the click position
                    VM.AddPoint(clickPosition);
                }
            }
            else // Editor is in Select mode
            {                                                
                // Attempt to select either a polygon or polygon points
                _selectedPolygon = VM.SelectPolygonOrPolygonPoint(clickPosition);                        
            }

            // Remove the resize adorner
            RemoveResizeAdorner();

            // If the editor is in selection mode and
            // there is a selected polygon with all points selected then...
            if (VM.IsSelecting &&
                VM.SelectedPolygon != null &&
                VM.SelectedPolygon.AllPointsSelected)                
            {
                // Select all the points on the polygon
                VM.SelectPolygonPoints();

                // Update the resize adorner for the selected polygon
                UpdateResizeAdorner(VM.SelectedPolygon.PointCollection);
            }
                        
            // Indicate that the event was handled
            e.Handled = true;
        }

        /// <summary>
        /// Canvas left mouse up event handler.
        /// </summary>        
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Position of the mouse when the button was released
            Point clickPosition = e.GetPosition(canvas);

            // If the user is moving a polygon point then...
            if (VM.MovingPoint)
            {
                // End the point move
                VM.EndMoveSelectedPoint(clickPosition);              
            }

            // If the user did not select a polygon on the mouse down event then...
            if (!_selectedPolygon)
            {             
                // Get the adorner layer from the canvas
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);

                // If the rubber band adorner was created then...
                if (adornerLayer != null && _rubberbandAdorner != null)
                {
                    // Retrieve the points of the rubbber band
                    Point endPoint = _rubberbandAdorner.EndPoint.Value;
                    Point startPoint = _originMouseStartPoint;

                    // Remove the rubber band adorner
                    adornerLayer.Remove(_rubberbandAdorner);
                    _rubberbandAdorner = null;

                    // Release the mouse
                    canvas.ReleaseMouseCapture();

                    // Clears the selected points
                    VM.ClearSelectedPoints();

                    // Create a rectangle from the rubber band
                    Rect lasso = new Rect(startPoint, endPoint);

                    // Select the polygon points inside the lasso
                    VM.SelectPolygonPoints(lasso);

                    // If more than one polygon point is selected then...
                    if (VM.SelectedPoints.Count > 1)
                    {
                        // If an entire polygon was captured in the lasso then...
                        PolygonViewModel selectedPolygon = null;
                        if (VM.IsEntirePolygonSelected(VM.SelectedPoints, ref selectedPolygon))
                        {
                            // Select the polygon
                            VM.SelectPolygon(selectedPolygon, false);
                        }

                        // Update the resize adorner
                        UpdateResizeAdorner(VM.SelectedPoints);
                    }
                    // If only one point is selected then...
                    else if (VM.SelectedPoints.Count == 1)
                    {
                        // Find the polygon associated with the selected polygon point
                        PolygonViewModel selectedPolygon = VM.FindPolygon(VM.SelectedPoints[0]);

                        // Select the polygon point                        
                        VM.SelectPoint(VM.SelectedPoints[0], selectedPolygon);                                             
                    }
                }
            }
                         
            // Indicate the event was handled
            e.Handled = true;
        }

        #endregion

        private void UpdatePolygonLines()
        {
            foreach (PolygonViewModel polygon in VM.Polygons)
            {
                if (polygon.Dirty)
                {
                    UpdatePolygonLines(polygon);
                    polygon.Dirty = false;
                }
            }
        }

        private void UpdatePolygonLines(PolygonViewModel polygon)
        {
            int previousPoint = 0;

            polygon.LineSegments.Clear();

            for (int index = 1; index < polygon.PointCollection.Count; index++)
            {
                PolygonLineSegment segment = new PolygonLineSegment();
                //Thickness thickness = new Thickness(101, -11, 362, 250);
                //line.Margin = thickness;
                segment.Line.Visibility = System.Windows.Visibility.Visible;
                segment.Line.Opacity = 0.0;
                segment.Line.StrokeThickness = 4;
                segment.Line.Stroke = System.Windows.Media.Brushes.Black;
                segment.Line.X1 = polygon.PointCollection[previousPoint].X;
                segment.Line.Y1 = polygon.PointCollection[previousPoint].Y;
                segment.Line.X2 = polygon.PointCollection[index].X;
                segment.Line.Y2 = polygon.PointCollection[index].Y;
                segment.StartPoint = polygon.PointCollection[previousPoint];
                segment.EndPoint = polygon.PointCollection[index];

                canvas.Children.Add(segment.Line);
                polygon.LineSegments.Add(segment);

                previousPoint++;

                if (index == polygon.PointCollection.Count - 1)
                {
                    PolygonLineSegment segment2 = new PolygonLineSegment();
                    //Thickness thickness = new Thickness(101, -11, 362, 250);
                    //line.Margin = thickness;
                    segment2.Line.Visibility = System.Windows.Visibility.Visible;
                    segment2.Line.Opacity = 0.0;
                    segment2.Line.StrokeThickness = 4;
                    segment2.Line.Stroke = System.Windows.Media.Brushes.Black;
                    segment2.Line.X1 = polygon.PointCollection[index].X;
                    segment2.Line.Y1 = polygon.PointCollection[index].Y;
                    segment2.Line.X2 = polygon.PointCollection[0].X;
                    segment2.Line.Y2 = polygon.PointCollection[0].Y;
                    segment2.StartPoint = polygon.PointCollection[previousPoint];
                    segment2.EndPoint = polygon.PointCollection[index];

                    canvas.Children.Add(segment2.Line);
                    polygon.LineSegments.Add(segment2);
                }
            }
        }

        private bool IsMouseOverLine(Point mousePosition, 
            ref PolygonLineSegment polygonLineSegment,
            ref PolygonViewModel polygonParent)
        {
            bool mouseOverLine = false;

            polygonLineSegment = null;

            UpdatePolygonLines();

            foreach (PolygonViewModel polygon in VM.Polygons)
            {
                foreach (PolygonLineSegment segment in polygon.LineSegments)
                {
                    if (segment.Line.IsMouseOver)
                    {
                        polygonLineSegment = segment;
                        polygonParent = polygon;

                        mouseOverLine = true;
                    }
                }
            }

            return mouseOverLine;
        }        
    }
}
