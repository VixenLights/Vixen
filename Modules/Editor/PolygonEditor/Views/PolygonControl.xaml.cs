
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
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

        #endregion
        
		#region Public Properties

		private PolygonEditorViewModel _vm;

        /// <summary>
        /// View model for the polygon editor.
        /// </summary>
        // ReSharper disable once InconsistentNaming
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
                _vm.DisplayResizeAdornerForSelectedPoints += DisplayResizeAdornerForSelectedPoints;
                _vm.RepositionResizeAdorner += RepositionResizeAdorner;
                _vm.RemoveRubberBandAdorner += RemoveRubberBandAdorner;
                _vm.LassoPoints += LassoPoints;
                _vm.IsMouseOverLine = IsMouseOverLine;
                _vm.Canvas = canvas;

                // Give the data grid the SelectedPoints for its ItemsSource
                pointDataGrid.ItemsSource = VM.SelectedPoints;
            }                
        }

        #endregion

        #region Private View Model Event Handlers

        /// <summary>
        /// Lassos (selects) the points under the rubber band.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void LassoPoints(object sender, EventArgs e)
        {
	        // Use the rubber band adorner to select (lasso) points
	        LassoPoints((MouseEventArgs)e);
        }

        /// <summary>
        /// Event handler repositions (re-creates) the resize adorner based on the selected points.
        /// </summary>        
        private void RepositionResizeAdorner(object sender, EventArgs e)
        {
	        // Reposition the resizing adorner
            UpdateResizeAdorner(VM.SelectedPoints);
        }

        /// <summary>
        /// Removes the rubber band (lasso) adorner.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void RemoveRubberBandAdorner(object sender, EventArgs e)
        {
	        // Get the adorner layer from the canvas
	        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
	        Debug.Assert(adornerLayer != null, nameof(adornerLayer) + " != null");

	        // Remove the rubber band adorner
	        adornerLayer.Remove(_rubberbandAdorner);
	        _rubberbandAdorner = null;
        }

        /// <summary>
        /// Event handler removes the resize adorner.
        /// </summary>        
		private void RemoveResizeAdorner(object sender, EventArgs e)
        {
	        // Remove the resize adorner
            RemoveResizeAdorner();            
        }

        /// <summary>
        /// Event handler displays the resize adorner for the specified shape.
        /// </summary>        
        private void DisplayResizeAdorner(object sender, EventArgs e)
        {
	        // Update the resize adorner based on the selected shape
		    UpdateResizeAdorner(VM.SelectedShape.PointCollection);
        }

        /// <summary>
        /// Event handler displays the resize adorner for the selected points.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void DisplayResizeAdornerForSelectedPoints(object sender, EventArgs e)
        {
            // Update the resize adorner based on the selected points
	        UpdateResizeAdorner(VM.SelectedPoints);
        }

        /// <summary>
        /// Event handler that refreshes (redraws) the resize adorner.
        /// </summary>        
        private void RefreshResizeAdorner(object sender, EventArgs e)
        {
            // Redraw the resize adorner
            RefreshResizeAdorner();
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
                Debug.Assert(adornerLayer != null, nameof(adornerLayer) + " != null");

                // Remove the resize adorner
                adornerLayer.Remove(_resizingAdorner);

                // Release the resize adorner
                _resizingAdorner = null;
            }
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

            // Display the resize adorner if a selected shape exists
            VM.DisplayResizeAdornerForSelectedShape();
        }

        /// <summary>
        /// Updates the rubber band lasso for selecting points.
        /// </summary>
        /// <param name="mouseEventArgs">Position of the mouse</param>
        private void LassoPoints(MouseEventArgs mouseEventArgs)
        {
	        // Get the position of the mouse click
	        Point mousePosition = mouseEventArgs.GetPosition(canvas);

            // Capture the mouse while lasso'ing points
            canvas.CaptureMouse();

            // Get the adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);

            // If the rubber band adorner has not been created then...
            if (adornerLayer != null && _rubberbandAdorner == null)
            {
                // Create the rubber band adorner 
                _rubberbandAdorner = new RubberbandAdorner(canvas, VM.LassoMouseStartPoint);

                // Set a flag indicating a lasso operation is in progress
                VM.LassoingPoints = true;

                // Add the rubber band adorner to the display
                adornerLayer.Add(_rubberbandAdorner);
            }

            // If the rubber band adorner has been created then...
            if (_rubberbandAdorner != null)
            {
                // Set the current mouse position as the end of the rubber band area
                _rubberbandAdorner.EndPoint = VM.LimitPointToCanvas(mousePosition);
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
            Rect bounds = VM.GetSelectedContentBounds(points);

            // Get the adorner layer from the canvas
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(canvas);

            // If the resize adorner is NOT null then...
            if (adornerLayer != null)
            {
                // Remove the resizing adorner
                RemoveResizeAdorner();

                // If the selected shape is an ellipse and
                if (VM.SelectedEllipse != null)
                {
	                // If the ellipse has a resize adorner associated with it then...
	                if (VM.SelectedEllipse.ResizeAdorner != null)
	                {
		                // Retrieve the resize adorner from the ellipse view model
		                _resizingAdorner = VM.SelectedEllipse.ResizeAdorner;
	                }
	                else
	                {
		                // Create a collection of point view models
		                ObservableCollection<PolygonPointViewModel> copyPoints =
			                new ObservableCollection<PolygonPointViewModel>();

		                // Create a reverse transform for the ellipse
		                RotateTransform reverseRotateTransform = new RotateTransform(-VM.SelectedEllipse.Angle,
			                VM.SelectedEllipse.CenterPoint.X, VM.SelectedEllipse.CenterPoint.Y);

		                // Loop over the points on the rectangle that surrounds the ellipse
		                foreach (PolygonPointViewModel pt in points)
		                {
			                // Clone the ellipse (rectangle) point
			                PolygonPointViewModel clonedPoint =
				                new PolygonPointViewModel(pt.PolygonPoint.Clone(), null);

			                // Transform the point
			                Point transformedPoint = reverseRotateTransform.Transform(pt.GetPoint());

			                // Update the cloned point
			                clonedPoint.X = transformedPoint.X;
			                clonedPoint.Y = transformedPoint.Y;

			                // Add the point to the collection
			                copyPoints.Add(clonedPoint);
		                }

		                // Determine the bounds of the resize adorner based on the transformed points
		                bounds = VM.GetSelectedContentBounds(copyPoints);

		                // Create the resizing adorner for the ellipse with the specified angle
		                _resizingAdorner = new ResizeAdorner(VM.SelectedEllipse.Angle, this, canvas, VM, bounds);

		                // Store off the resize adorner with the ellipse
		                VM.SelectedEllipse.ResizeAdorner = _resizingAdorner;
                    }
                }
                else
                {
	                // Create the resizing adorner
	                _resizingAdorner = new ResizeAdorner(0.0, this, canvas, VM, bounds);
                }
                
                // Add the resizing adorner to the canvas
	            adornerLayer.Add(_resizingAdorner);
            }
        }

        #endregion

        #region Private Mouse Over Polygon Line Methods

        /// <summary>
        /// Updates all the polygon line segments.
        /// The line segments are used to perform hit testing when in add polygon point mode.
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
            segment.Line.Stroke = Brushes.DodgerBlue;
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
	        // ReSharper disable once RedundantAssignment
	        ref PolygonLineSegment polygonLineSegment,
	        // ReSharper disable once RedundantAssignment
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
