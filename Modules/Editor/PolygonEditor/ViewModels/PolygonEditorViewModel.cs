using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VixenModules.App.Polygon;
using Point = System.Windows.Point;

namespace PolygonEditor
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
		public PolygonEditorViewModel(ObservableCollection<Polygon> polygons)
		{
			// Store off the model polygons
			PolygonModels = polygons;

			// Create the collection of view model polygons	 						
			Polygons = new ObservableCollection<PolygonViewModel>();

			// Create the view model polygons from the model
			foreach (Polygon polygon in PolygonModels)
			{
				Polygons.Add(new PolygonViewModel(polygon, this));
			}
						
			// If the editor is being displayed without any polygons then...
			if (Polygons.Count == 0)
			{
				// Default to draw mode where we are creating a polygon
				AddNewPolygon();				
			}
			
			// Create the collection of selected points
			SelectedPoints = new ObservableCollection<PolygonPointViewModel>();
			
			// Create the commands
			CopyCommand = new Command(Copy, IsPolygonSelected);
			PasteCommand = new Command(Paste, CanExecutePaste);
			DeleteCommand = new Command(Delete, IsPolygonSelected);			
			CutCommand = new Command(Cut, IsPolygonSelected);
			SnapToGridCommand = new Command(SnapToGrid);			
		}

		#endregion

		#region Private Constants

		/// <summary>
		/// Clipboard format identifier for a polygon.
		/// </summary>
		const string PolygonClipboardFormat = "PolygonClipboardFormat";

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// Gets or sets the view model's selected polygon.
		/// </summary>
		public PolygonViewModel SelectedPolygon
		{
			get { return GetValue<PolygonViewModel>(SelectedPolygonProperty); }
			private set { SetValue(SelectedPolygonProperty, value); }			
		}

		/// <summary>
		/// SelectedPolygon property data.
		/// </summary>
		public static readonly PropertyData SelectedPolygonProperty = RegisterProperty("SelectedPolygon", typeof(PolygonViewModel));

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
		public static readonly PropertyData PreviousPointMovingProperty = RegisterProperty("PreviousPointMoving", typeof(PolygonPointViewModel));

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
		public static readonly PropertyData PointMovingProperty = RegisterProperty("PointMoving", typeof(PolygonPointViewModel));

		/// <summary>
		/// Gets or sets the next point during a move operation.
		/// </summary>
		public PolygonPointViewModel NextPointMoving
		{
			get { return GetValue<PolygonPointViewModel>(NextPointMovingProperty); }
			private set { SetValue(NextPointMovingProperty, value); }
		}

		/// <summary>
		/// PointMoving property data.
		/// </summary>
		public static readonly PropertyData NextPointMovingProperty = RegisterProperty("NextPointMoving", typeof(PolygonPointViewModel));

		/// <summary>
		/// True when the moving ghost point is visible.
		/// </summary>
		public bool MovingPointVisibility
		{
			get { return GetValue<bool>(MovingPointVisibilityProperty); }
			private set { SetValue(MovingPointVisibilityProperty, value); }
		}

		/// <summary>
		/// PointMovingVisibility property data.
		/// </summary>
		public static readonly PropertyData MovingPointVisibilityProperty = RegisterProperty("MovingPointVisibility", typeof(bool));

		public bool IsSelecting
		{
			get { return GetValue<bool>(IsSelectingProperty); }
			set 
			{ 
				SetValue(IsSelectingProperty, value);

				if (!value)
				{
					if (NewPolygon == null)
					{
						AddNewPolygon();
						SelectedPolygon = NewPolygon;
					}

					DeselectAllPolygons();
					RaiseRemoveResizeAdorner();					
				}
				else
				{
					if (NewPolygon != null)
					{
						Polygons.Remove(NewPolygon);
						PolygonModels.Remove(NewPolygon.Polygon);
						NewPolygon = null;
						SelectedPolygon = null;
					}
				}

				((Command)CopyCommand).RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// PointMovingVisibility property data.
		/// </summary>
		public static readonly PropertyData IsSelectingProperty = RegisterProperty("IsSelectingProperty", typeof(bool));

		#endregion

		#region Public Properties

		/// <summary>
		/// Width of canvas.
		/// </summary>
		public double ActualWidth { get; set; }

		/// <summary>
		/// Height of canvas.
		/// </summary>
		public double ActualHeight { get; set; }

		/// <summary>
		/// Gets or sets the model polygons.
		/// </summary>
		public ObservableCollection<Polygon> PolygonModels
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the view model polygons.
		/// </summary>
		public ObservableCollection<PolygonViewModel> Polygons
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

		#endregion

		#region Public Mouse Over Methods

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
				if (IsMouseOverPoint(position, ref selectedPolygonPoint, ref selectedPolygon) ||
					IsMouseOverCenterCrossHash(position, ref selectedPolygon))
				{
					// Indicate the mouse is over a moveable item and break out of loop
					mouseOverMoveableItem = true;
					break;
				}
			}

			return mouseOverMoveableItem;
		}

		#endregion

		#region Private Mouse Over Methods		

		/// <summary>
		/// Returns true when the mouse is over one of the polygon's center cross hash.		
		/// </summary>
		/// <param name="mousePosition">Position of the mouse</param>
		/// <param name="polygon">Polygon the mouse is over or NULL</param>
		/// <returns></returns>
		private bool IsMouseOverCenterCrossHash(Point mousePosition, ref PolygonViewModel crossHashPolygon)
		{
			// Default to not being over the center of a polygon
			crossHashPolygon = null;

			// Default to not being of the center cross hash
			bool mouseOverCenterCrossHash = false;

			// Loop over the polygon view models
			foreach (PolygonViewModel polygon in Polygons)
			{
				// If the mouse is over the center cross hash then...
				if (polygon.IsOverCenterCrossHash(mousePosition))
				{
					// Set polygon the mouse is over
					crossHashPolygon = polygon;

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
		/// <returns></returns>
		private bool IsMouseOverPoint(Point mousePosition, ref PolygonPointViewModel pointUnderMouse, ref PolygonViewModel polygonUnderMouse)
		{
			// Default to the mouse not being over a polygon point
			bool mouseOverPoint = false;

			// Loop over the polygon view models
			foreach (PolygonViewModel polygon in Polygons)
			{				
				// Loop over the points on the polygon
				foreach (PolygonPointViewModel point in polygon.PointCollection)
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

						// Return the point and the polygon the mouse is over
						pointUnderMouse = point;
						polygonUnderMouse = polygon;

						break;
					}			
				}

				// If the mouse is over a polygon point then...
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
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Refreshes all the polygon's point collections.
		/// </summary>
		private void RefreshAllPolygons()
		{
			// Loop over the polygon view models
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Fire the property changed event so that the converters run
				polygon.NotifyPointCollectionChanged();
			}
		}
		
		/// <summary>
		/// Selects the specified polygon.
		/// </summary>
		/// <param name="polygon"></param>
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
		/// <param name="polygon">Polygon to select</param>
		public void SelectPolygon(PolygonViewModel polygon, bool updateSelectedPoints)
		{
			// Select the polygon and all its points
			SelectAllPointsOnPolygon(polygon);

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
		}

		/// <summary>
		/// Creates a new polygon.
		/// </summary>
		private void AddNewPolygon()
		{
			// Create a new model polygon
			Polygon polygon = new Polygon();

			// Create a new view model polygon
			NewPolygon = new PolygonViewModel(polygon, this);

			// Initialize the new polygon as the SelectedPolygon
			SelectedPolygon = NewPolygon;

			// Save off the model polygon
			PolygonModels.Add(polygon);

			// Save off the view model polygon
			Polygons.Add(NewPolygon);
		}
	
		/// <summary>
		/// Deselects the current selected polygon.
		/// </summary>
		private void DeselectPolygon()
		{
			// If a polygon was previously selected then...
			if (SelectedPolygon != null)
			{
				// Deselect all points on the polygon
				SelectedPolygon.DeselectAllPoints();
			}

			// Clear out the selected polygon
			SelectedPolygon = null;

			// Since there is no longer a selected polygon update the commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Fires an event to remove the resize adorner.
		/// </summary>
		private void RaiseRemoveResizeAdorner()
		{
			EventHandler handler = RemoveResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		#endregion

		#region Private Command Methods

		/// <summary>
		/// Snaps the polygon points to pixel positions.  The expectation with this editor is that it is 10 times
		/// larger than the actual display element.
		/// </summary>
		private void SnapToGrid()
		{
			// Loop over the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Loop over the points on the polygon
				foreach (PolygonPointViewModel point in polygon.PointCollection)
				{
					// Round to the nearest 10
					int x = (int)Math.Round(point.X / 10.0);
					int y = (int)Math.Round(point.Y / 10.0);
					
					point.X = x * 10.0;
					point.Y = y * 10.0;
				}

				// Raise the property change events for the points to the converters run
				polygon.NotifyPointCollectionChanged();
			}
		}

		/// <summary>
		/// Deletes the selected polygon.
		/// </summary>
		private void Delete()
		{
			// Remove the specified polygon
			Polygons.Remove(SelectedPolygon);
			PolygonModels.Remove(SelectedPolygon.Polygon);

			// Remove the resize adorner
			RaiseRemoveResizeAdorner();

			// Update the enable status of the copy, cut and delete commands
			((Command)CopyCommand).RaiseCanExecuteChanged();
			((Command)DeleteCommand).RaiseCanExecuteChanged();
			((Command)CutCommand).RaiseCanExecuteChanged();
		}
	
		/// <summary>
		/// Cuts the selected polygon.
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
			// If the clipboard contains polygon data then...
			if (Clipboard.ContainsData(PolygonClipboardFormat))
			{
				// Retrieve the polygon model object from the clipboard
				Polygon copiedPolygon = (Polygon)Clipboard.GetData(PolygonClipboardFormat);
				
				// Create a polygon view model object
				PolygonViewModel copiedPolygonVM = new PolygonViewModel(copiedPolygon, this);
				
				// Calculate the center point of the editor
				double centerX = ActualWidth / 2.0;
				double centerY = ActualHeight / 2.0;

				// Retrieve the center point of the polygon
				PolygonPointViewModel centerPoint = copiedPolygonVM.CenterPoint;

				// Determine how far from center the clipboard polygon is positioned
				// This is the distance required to move the polygon to the center of the editor.
				double moveX = centerX - centerPoint.X;
				double moveY = centerY - centerPoint.Y;

				// Loop over the points on the polygon
				foreach (PolygonPointViewModel pt in copiedPolygonVM.PointCollection)
				{
					// Move the points so the polygon is centered in the middle of the editor
					pt.X += moveX;
					pt.Y += moveY;

					// Make sure all the point fits on the editor
					if (pt.X >= ActualWidth)
					{
						pt.X = ActualWidth - 1;
					}

					if (pt.Y >= ActualHeight)
					{
						pt.Y = ActualHeight - 1;
					}
				}

				// Add the polygon view model to the collection
				Polygons.Add(copiedPolygonVM);

				// Fire the Property Changed event to ensure the converters run
				copiedPolygonVM.NotifyPointCollectionChanged();

				// Deselect any polygons or polygon points
				DeselectAllPolygons();
				DeselectPolygon();

				// Remove the resize adorner
				RaiseRemoveResizeAdorner();

				// Select the pasted polygon
				SelectPolygon(copiedPolygonVM, true);

				// Display the resize adorner
				EventHandler handler = DisplayResizeAdorner;
				handler?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// Copies the selected polygon.
		/// </summary>
		private void Copy()
		{			
			// Copy the polygon data to the clipboard
			Clipboard.SetData(PolygonClipboardFormat, SelectedPolygon.Polygon);
			
			// Update the execute status of the paste command
			((Command)PasteCommand).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Returns true when the Paste command can execute.
		/// </summary>
		/// <returns>True when the Paste command can execute</returns>
		private bool CanExecutePaste()
		{
			// Return whether polygon data is on the clipboard
			return Clipboard.ContainsData(PolygonClipboardFormat);
		}

		/// <summary>
		/// Returns true if a polygon is selected.
		/// </summary>
		/// <returns>True if a polygon is selected</returns>
		private bool IsPolygonSelected()
		{
			return IsSelecting &&
				SelectedPolygon != null &&
				SelectedPolygon.AllPointsSelected;
		}

		#endregion

		#region Public Methods

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
		/// Selects a polygon or polygon point based on the click position.
		/// </summary>
		/// <param name="clickPosition">Click position</param>
		public bool SelectPolygonOrPolygonPoint(Point clickPosition)
		{
			bool polygonSelected = false;
			
			// Deselect the currently selected polygon
			DeselectPolygon();

			PolygonViewModel selectedPolygon = null;
			PolygonPointViewModel selectedPolygonPoint = null;

			// If the mouse is over a polygon point then...
			if (IsMouseOverPoint(clickPosition, ref selectedPolygonPoint, ref selectedPolygon))
			{
				// Select the polygon point
				SelectPoint(selectedPolygonPoint, selectedPolygon);
				
				// Start a move operation on the point
				MovingPoint = true;

				// Create a new ghost Point
				PointMoving = new PolygonPointViewModel(new PolygonPoint(), null);

				// Set the position of the ghost point
				PointMoving.X = SelectedPolygon.SelectedVertex.X;
				PointMoving.Y = SelectedPolygon.SelectedVertex.Y;
			}
			// If the mouse is over the center cross hash then...
			else if (IsMouseOverCenterCrossHash(clickPosition, ref selectedPolygon))
			{
				// Select the polygon and all its points
				SelectPolygon(selectedPolygon, true);
				
				// Return that the entire polygon was selected
				polygonSelected = true;
			}
			// Otherwise the mouse isn't over a polygon or a point
			else
			{
				// Deselect all polygons
				DeselectAllPolygons();
			}
			
			// Return whether a polygon was selected
			return polygonSelected;
		}

		/// <summary>
		/// Adds a point to the current polygon.
		/// </summary>
		/// <param name="position">Position of new point</param>
		public void AddPoint(Point position)
		{			
			// Forward the operation to the polygon view model
			NewPolygon.AddPoint(position);
				
			// If the polygon was closed then...
			if (NewPolygon.PolygonClosed)
			{
				// Prepare to create a new polygon
				AddNewPolygon();
			}			
		}
		
		/// <summary>
		/// 
		/// Moves the selected point.
		/// </summary>
		/// <param name="position">New position of the point</param>
		public void EndMoveSelectedPoint(Point position)
		{							
			// Forward the operation to the selected polygon
			SelectedPolygon.MoveSelectedPoint(position);

			// End the point move
			MovingPoint = false;

			// Hide the ghost point
			MovingPointVisibility = false;
		}

		/// <summary>
		/// Moves the currently selected point to the specified position.
		/// </summary>
		/// <param name="position">New position of the point</param>
		public void MovePoint(Point position)
		{
			// Update the Next and Previous point so that the ghost point and associated line segments
			// can be drawn
			PreviousPointMoving = SelectedPolygon.GetPreviousPoint();
			NextPointMoving = SelectedPolygon.GetNextPoint();

			// Update the ghost point position
			PointMoving.X = position.X;
			PointMoving.Y = position.Y;

			// Make the ghost point visible
			MovingPointVisibility = true;
		}

		/// <summary>
		/// Selects the specified point on the specified polygon.
		/// </summary>
		/// <param name="selectedPoint">Point to select</param>
		/// <param name="selectedPolygon">Polygon which contains the point</param>
		public void SelectPoint(PolygonPointViewModel selectedPoint, PolygonViewModel selectedPolygon)
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
		/// Selects polygon points in the specified lasso rectangle.
		/// </summary>
		/// <param name="lasso">Rectangle to select points from</param>
		public void SelectPolygonPoints(Rect lasso)
		{
			// Loop over the polygons
			foreach (PolygonViewModel polygon in Polygons)
			{
				// Loop over the points on the polygon
				foreach (PolygonPointViewModel point in polygon.PointCollection)
				{
					// If the point is inside the rectangle then...
					if (lasso.Contains(point.GetPoint()))
					{
						// Select the point
						point.Selected = true;
						SelectedPoints.Add(point);
					}
				}
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
		/// Event displays the resize adorner after operations like a paste.
		/// </summary>
		public event EventHandler DisplayResizeAdorner;

		/// <summary>
		/// Repositions the resize adorner after operations like rotate.
		/// </summary>
		public event EventHandler RepositionResizeAdorner;

		#endregion

		#region IResizeable Methods

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void RotateSelectedItems(double angle, Point center)
		{
			RotateTransform rt = new RotateTransform(angle, center.X, center.Y);

			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				Point tempPoint = point.GetPoint();
				Point transformedPoint = rt.Transform(tempPoint);

				point.X = transformedPoint.X;				
				point.Y = transformedPoint.Y;
			}
		
			RefreshAllPolygons();

			EventHandler handler = RefreshResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		public void DoneRotating()
		{
			//RaiseRemoveResizeAdorner();

			EventHandler handler = RepositionResizeAdorner;
			handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public bool IsRotateable(double angle, Point center)
		{
			bool rotateable = true;

			RotateTransform rt = new RotateTransform(angle, center.X, center.Y);

			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				Point tempPoint = point.GetPoint();
				Point transformedPoint = rt.Transform(tempPoint);

				point.X = transformedPoint.X;
				point.Y = transformedPoint.Y;

				if (point.X < 0 ||
					point.Y < 0 ||
					point.X > ActualWidth ||
					point.Y > ActualHeight)
				{
					RaiseRemoveResizeAdorner();					

					rotateable = false;
					break;
				}
			}

			return rotateable;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void TransformSelectedItems(TransformGroup t)
		{
			foreach (Transform tc in t.Children)
			{
				if (tc is ScaleTransform)
				{
					ScaleTransform st = (ScaleTransform)tc;
					
					foreach (PolygonPointViewModel point in SelectedPoints)
					{
						Point tempPoint = point.GetPoint();
						Point transformedPoint = st.Transform(tempPoint);

						point.X = transformedPoint.X;						
						point.Y = transformedPoint.Y;						
					}
				}
			}

			RefreshAllPolygons();

			//EventHandler handler = RepositionResizeAdorner;
			//handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void MoveSelectedItems(Transform transform)
		{
			TranslateTransform translateTransform = (TranslateTransform)transform;
			
			foreach (PolygonPointViewModel point in SelectedPoints)
			{
				Point tempPoint = point.GetPoint();
				Point transformedPoint = translateTransform.Transform(tempPoint);

				point.X = transformedPoint.X;
				point.Y = transformedPoint.Y;
			}
			
			RefreshAllPolygons();

			//EventHandler handler = RepositionResizeAdorner;
			//handler?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		//public object FindResource(object resourceKey)
		//{
		//	return null;
		//}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public double GetWidth()
		{
			return ActualWidth;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		/// <returns></returns>
		public double GetHeight()
		{
			return ActualHeight;
		}

		#endregion					
	}
}
