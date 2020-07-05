
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VixenModules.Editor.PolygonEditor.Adorners;
using VixenModules.Editor.PolygonEditor.ViewModels;

namespace VixenModules.Editor.PolygonEditor.Views
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
            Loaded += PolygonControl_Loaded;
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
        /// True when the user just selected a shape (polygon or line).
        /// This field inhibits drawing the rubberband adorner when selecting a shape via
        /// the center cross hair.
        /// </summary>
        bool _selectedShape;

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
           
                // Give the data grid the SelectedPoints for its ItemsSource
                pointDataGrid.ItemsSource = VM.SelectedPoints;
            }                
        }

        #endregion
        
        #region Private View Model Event Handlers

        /// <summary>
        /// Event handler repositions (re-creates) the resize adorner based on the selected points.
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

            // Deselect all line points
            VM.DeselectAllLines();
            
            // Remove the resize adorner
            RemoveResizeAdorner();            
        }

        /// <summary>
        /// Event handler dipslays the resize adorner for the specified polygon.
        /// </summary>        
        private void DisplayResizeAdorner(object sender, EventArgs e)
        {
            if (VM.SelectedPolygon != null)
            {
                // Update the resize adorner based on the selected polygon
                UpdateResizeAdorner(VM.SelectedPolygon.PointCollection);
            }
            else if (VM.SelectedLine != null)
            {
                // Update the resize adorner based on the selected line
                UpdateResizeAdorner(VM.SelectedLine.PointCollection);
            }
        }

        /// <summary>
        /// Event handler that refreshes (redraws) the resize adorner.
        /// </summary>        
        private void RefreshResizeAdorner(object sender, EventArgs e)
        {
            RefreshResizeAdorner();
        }

        /// <summary>
        /// Gets the rectangle bounds of the specified polygon/line points.
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
            // Get the position of the mouse click
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
            // user did not just select a shape then...
            if (VM.IsSelecting &&
                !VM.MovingPoint &&                
                e.LeftButton == MouseButtonState.Pressed &&
                !_selectedShape)
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

            // Default to not selecting a shape
            _selectedShape = false;

            // If the editor is not in selection mode then...
            if (!VM.IsSelecting)
            {
                // If we are adding points to existing polygons then...
                if (VM.AddPoint)
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
                else if (VM.DrawPolygon)
                {
                    // Add a polygon point at the click position
                    VM.AddPolygonPoint(clickPosition);                    
                }
                // Otherwise if we are draw line mode then...
                else if (VM.DrawLine)
                {
                    VM.AddLinePoint(clickPosition);
                }
            }
            else // Editor is in Select mode
            {
                ((Canvas)sender).CaptureMouse();

                // Attempt to select either a polygon, line or point
                _selectedShape = VM.SelectPolygonLineOrPoint(clickPosition);                        
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
            // If the editor is in selection mode and
            // there is a selected line with all points selected then...
            else if (VM.IsSelecting &&
                     VM.SelectedLine != null &&
                     VM.SelectedLine.AllPointsSelected)
            {
                // Update the resize adorner for the selected line
                UpdateResizeAdorner(VM.SelectedLine.PointCollection);
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

            // Release the mouse
            ((Canvas)sender).ReleaseMouseCapture();

            // If the user is moving a polygon point then...
            if (VM.MovingPoint)
            {
                // End the point move
                VM.EndMoveSelectedPoint(clickPosition);                
            }

            // If the user did not select a shape on the mouse down event then...
            if (!_selectedShape)
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
                    VM.SelectShapePoints(lasso);

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

                        if (selectedPolygon != null)
                        {
                            // Select the polygon point                        
                            VM.SelectPolygonPoint(VM.SelectedPoints[0], selectedPolygon);
                        }
                        else
                        {
                            // Find the line associated with the selected point
                            LineViewModel selectedLine = VM.FindLine(VM.SelectedPoints[0]);

                            if (selectedLine != null)
                            {
                                // Select the line point
                                VM.SelectLinePoint(VM.SelectedPoints[0], selectedLine);
                            }
                        }
                    }
                }
            }
                         
            // Indicate the event was handled
            e.Handled = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Control loaded event handler.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void PolygonControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Hack this to fit the existing property but this is not ideal. This control should have its own view model and then
            //the control can inherit from Catel:UserControl and Catel can inject that VM behind this control.
            //That VM will have a parent of the VM being used here and then you can walk the ladder if needed.
            //However VM should be mostly self contained and not need to rely on their parents. They can expose functions that the
            //parents can interact with when needed.
            if (DataContext is PolygonEditorViewModel vm)
            {
                VM = vm;
            }

            // Display the resize adorner if a selected polygon or line exists
            VM.DisplayResizeAdornerForSelectedPolygonOrLine();
        }

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
            // If the editor is add point mode and
            // the mouse is over a polygon line then...
            else if (VM.AddPoint &&
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
            Cursor = cursor;
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

        /// <summary>
        /// Updates all the polygon line segments.
        /// The line segments are used to perform hit testing when in aded polygon point mode.
        /// </summary>
		private void UpdatePolygonLines()
        {
            // Loop over all the polygons
            foreach (PolygonViewModel polygon in VM.Polygons)
            {
                // If the polygon line segments need to be updated then...
                if (polygon.Dirty)
                {
                    // Update the polygon line segments
                    UpdatePolygonLines(polygon);

                    // Reset the dirty flag
                    polygon.Dirty = false;
                }
            }
        }

        /// <summary>
        /// Creates a polygon line segment.
        /// </summary>
        /// <param name="polygon">Polygon to create the line segment for</param>
        /// <param name="previousPoint">Previous polygon point</param>
        /// <param name="nextPoint">Next polygon point</param>
        private void CreatePolygonLineSegment(PolygonViewModel polygon, PolygonPointViewModel previousPoint, PolygonPointViewModel nextPoint)
        {
            PolygonLineSegment segment = new PolygonLineSegment();                        
            segment.Line.Visibility = Visibility.Visible;
            segment.Line.Opacity = 0.0;
            segment.Line.StrokeThickness = 4;
            segment.Line.Stroke = Brushes.Black;
            segment.Line.X1 =  previousPoint.X;
            segment.Line.Y1 = previousPoint.Y;
            segment.Line.X2 = nextPoint.X;
            segment.Line.Y2 = nextPoint.Y;
            segment.StartPoint = previousPoint;
            segment.EndPoint = nextPoint;

            canvas.Children.Add(segment.Line);
            polygon.LineSegments.Add(segment);            
        }

        /// <summary>
        /// Updates hidden polygon segment lines to aid when knowing the mouse is over a polygon line.
        /// </summary>
        /// <param name="polygon">Polygon to update the line segments for</param>
        private void UpdatePolygonLines(PolygonViewModel polygon)
        {
            // Clear the previous line segments
            polygon.LineSegments.Clear();

            // Loop over the points
            int previousPoint = 0;
            for (int index = 1; index < polygon.PointCollection.Count; index++, previousPoint++)
            {                
                // Create a polygon line segment from the previous point to the next point
                CreatePolygonLineSegment(
                    polygon,
                    polygon.PointCollection[previousPoint],
                    polygon.PointCollection[index]);
                
                // If this is the last point then... 
                if (index == polygon.PointCollection.Count - 1)
                {
                    // Create a segment back to the first point
                    CreatePolygonLineSegment(
                        polygon,
                        polygon.PointCollection[index],
                        polygon.PointCollection[0]);                                       
                }
            }
        }

        /// <summary>
        /// Returns true if the mouse is over a polygon line.
        /// </summary>
        /// <param name="mousePosition">Mouse position</param>
        /// <param name="polygonLineSegment">Polygon line segment the mouse is over</param>
        /// <param name="polygonParent">Polygon the line segment belongs to</param>
        /// <returns></returns>
        private bool IsMouseOverLine(Point mousePosition, 
            ref PolygonLineSegment polygonLineSegment,
            ref PolygonViewModel polygonParent)
        {
            // Default to NOT being over a polygon line
            bool mouseOverLine = false;            
            polygonLineSegment = null;
            polygonParent = null;

            // No need to do this processing if the editor is not in
            // add polygon point mode
            if (VM.AddPoint)
            {
                // Update all the polygon lines to make sure they are accurate
                UpdatePolygonLines();

                // Loop over all the polygons
                foreach (PolygonViewModel polygon in VM.Polygons)
                {
                    // Loop over all the line segments
                    foreach (PolygonLineSegment segment in polygon.LineSegments)
                    {
                        // If the mouse is over a line then...
                        if (segment.Line.IsMouseOver)
                        {
                            // Return the line segment
                            polygonLineSegment = segment;

                            // Return the polygon which contains the line segment
                            polygonParent = polygon;

                            // Indicate that the mouse is over a polygon line segment
                            mouseOverLine = true;
                        }
                    }
                }
            }

            // Return whether the mouse is over a polygon line segment
            return mouseOverLine;
        }

        #endregion
    }
}
