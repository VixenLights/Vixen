using System;
using System.Collections.Generic;
using System.Windows;
using VixenModules.App.Polygon;
using VixenModules.Editor.PolygonEditor.Converters;
using VixenModules.Editor.PolygonEditor.ViewModels;

namespace VixenModules.Editor.PolygonEditor.Views
{
	/// <summary>
	/// Interaction logic for PolygonEditorView.xaml
	/// </summary>
	public partial class PolygonEditorView
    {
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="polygonContainer">Polygon/line data</param>
		public PolygonEditorView(PolygonContainer polygonContainer)
		{
			InitializeComponent();

			// Save off the polygon/line data 
			_polygonContainer = polygonContainer;							
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Container of polygon/line data that is being edited.
		/// </summary>
		private PolygonContainer _polygonContainer;

		/// <summary>
		/// View model for the editor.
		/// </summary>
		private PolygonEditorViewModel _vm;

		#endregion

		#region Protected Methods

		/// <inheritdoc />
		protected override void Initialize()
		{
			// Call the base class implementation
			base.Initialize();
			
			if (ViewModel is PolygonEditorViewModel vm)
			{
				// Save off the view model
				_vm = vm;

				// Give the view model the editor capabilities
				vm.EditorCapabilities = _polygonContainer.EditorCapabilities;

				// Attempting to size the editor to fill 70% of the primary monitor
				// ReSharper disable once InconsistentNaming
				const double PercentOfScreen = 0.7;
				double wfactor = ((SystemParameters.PrimaryScreenWidth * PercentOfScreen) / _polygonContainer.Width);
				double hfactor = ((SystemParameters.PrimaryScreenHeight * PercentOfScreen) / _polygonContainer.Height);

				// Take the minimum scale factor between the width and height
				double factor = Math.Min(wfactor, hfactor);

				// Calculate the width and height of the canvas using the scale factor and rounding to the next largest pixel
				vm.CanvasWidth = (int)Math.Ceiling(_polygonContainer.Width * factor);
				vm.CanvasHeight = (int)Math.Ceiling(_polygonContainer.Height * factor);

				// Determine the X and Y scale factors
				double xScaleFactor = 1.0; 

				// Need to guard against dividing by zero when working with 1 pixel width
				if (_polygonContainer.Width - 1.0 == 0.0)
				{
					xScaleFactor = (vm.CanvasWidth - 1.0);
				}
				else
				{
					xScaleFactor = (vm.CanvasWidth - 1.0) / (_polygonContainer.Width - 1.0);
				}

				double yScaleFactor = 1.0;

				// Need to guard against dividing by zero when working with 1 pixel height
				if (_polygonContainer.Height - 1.0 == 0.0)
				{
					yScaleFactor = (vm.CanvasHeight - 1.0);
				}
				else
				{
					yScaleFactor = (vm.CanvasHeight - 1.0) / (_polygonContainer.Height - 1.0);
				}

				// Set the size of the actual display element
				vm.DisplayElementWidth = (_polygonContainer.DisplayElementWidth - 1.0) * xScaleFactor;
				vm.DisplayElementHeight = (_polygonContainer.DisplayElementHeight - 1.0) * yScaleFactor;
				vm.DisplayElementXOrigin = (_vm.CanvasWidth - vm.DisplayElementWidth) / 2.0 - 1.0;
				vm.DisplayElementYOrigin = (_vm.CanvasHeight - vm.DisplayElementHeight) / 2.0 - 1.0;
				
				// Configure whether to display the outline of the display element
				vm.ShowDisplayElement = _polygonContainer.ShowDisplayElement;

				// Give the polygon point converter the scale factor
				PolygonPointXConverter.XScaleFactor = xScaleFactor;
				PolygonPointYConverter.YScaleFactor = yScaleFactor;

				// Give the polygon point converter the Y dimension of the drawing canvas
				PolygonPointYConverter.BufferHt = _polygonContainer.Height;

				// If the edit is in time based mode then...
				if (_polygonContainer.EditorCapabilities.ShowTimeBar)
				{
					// Initialize the snapshots with the polygons/lines
					vm.InitializePolygonSnapshots(
						_polygonContainer.Polygons,
						_polygonContainer.PolygonTimes,
						_polygonContainer.Lines,
						_polygonContainer.LineTimes,
						_polygonContainer.Ellipses,
						_polygonContainer.EllipseTimes);
				}
				else
				{
					// Give the view model the model polygons and polygon times
					vm.InitializePolygons(_polygonContainer.Polygons, _polygonContainer.PolygonTimes);

					// Give the view model the model lines and line times, 
					vm.InitializeLines(_polygonContainer.Lines, _polygonContainer.LineTimes);

					// Give the view model the model ellipses and ellipse and times
					vm.InitializeEllipses(_polygonContainer.Ellipses, _polygonContainer.EllipseTimes);
				}
				
				// Initialize the window width to the canvas width
				Width = vm.CanvasWidth; 

				// If the display element is long and tall don't let the window
				// get too skinny
				if (Width < PercentOfScreen * SystemParameters.PrimaryScreenWidth)
				{
					Width = SystemParameters.PrimaryScreenWidth * PercentOfScreen;
				}

				// If the editor is in time based mode then need to adjust the height for the time bar
				int timeBarHeight = 0;
				if (vm.EditorCapabilities.ShowTimeBar)
				{
					timeBarHeight = 190;
				}

				// Set the height of the window
				Height = vm.CanvasHeight + timeBarHeight;

				// If the calculated height is less than the minimum then...
				const double MinimumHeightPercentage = 0.25;
				if (Height < MinimumHeightPercentage * SystemParameters.PrimaryScreenHeight)
				{
					// Set the height to the minimum
					Height = MinimumHeightPercentage * SystemParameters.PrimaryScreenHeight;
				}

				// Give the view model the width of the window
				vm.WindowWidth = (int)this.Width;				
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the polygon models from the view model.
		/// </summary>
		public IList<Polygon> Polygons
		{			
			get
			{
				// Create the return collection
				List<Polygon> polygons = new List<Polygon>();

				// If the editor is in time based mode then...
				if (_polygonContainer.EditorCapabilities.ShowTimeBar)
				{		
					// Loop over the snapshots
					foreach (PolygonSnapshotViewModel snapShot in _vm.PolygonSnapshots)
					{
						// If a polygon view model is populated then...
						if (snapShot.PolygonViewModel != null)
						{
							// Add the polygon to the return collection
							polygons.Add(snapShot.PolygonViewModel.Polygon);
						}						
					}				
				}
				// Otherwise we are not in time based mode
				else
				{
					// Add all the polygon models to the return collection
					polygons.AddRange(_vm.PolygonModels);					
				}

				// Loop over all the polygon models
				foreach(Polygon polygon in polygons)
				{
					// Scale the polygon model points to the display element dimensions
					polygon.ScalePoints(1.0 / PolygonPointXConverter.XScaleFactor, 1.0 / PolygonPointYConverter.YScaleFactor);					
					
					// Round the polygon points to the display element pixels
					polygon.RoundPoints();
				}

				return polygons;
			}			
		}

		/// <summary>
		/// Gets the times associated with the polygons.
		/// </summary>
		public IList<double> PolygonTimes
		{
			get
			{
				return _vm.GetPolygonTimes();
			}
		}

		/// <summary>
		/// Gets the line models from the view model. 
		/// </summary>
		public IList<Line> Lines
		{
			get
			{
				// Create the return collection
				List<Line> lines = new List<Line>();

				// If the editor is in time based mode then...
				if (_polygonContainer.EditorCapabilities.ShowTimeBar)
				{
					// Loop over the snapshots
					foreach (PolygonSnapshotViewModel snapShot in ((PolygonEditorViewModel)ViewModel).PolygonSnapshots)
					{
						// If a polygon view model is populated then...
						if (snapShot.LineViewModel != null)
						{
							// Add the line to the return collection
							lines.Add(snapShot.LineViewModel.Line);
						}
					}
				}
				// Otherwise we are not in time based mode
				else
				{					
					// Add all the line models to the return collection
					lines.AddRange(_vm.LineModels);					
				}

				// Loop over the line models
				foreach (Line line in lines)
				{
					// Scale the polygon model points to the display element dimensions
					line.ScalePoints(1.0 / PolygonPointXConverter.XScaleFactor, 1.0 / PolygonPointYConverter.YScaleFactor);

					// Round the line points to the display element pixels
					line.RoundPoints();
				}

				return lines;
			}			
		}

		/// <summary>
		/// Gets the times associated with the lines.
		/// </summary>
		public IList<double> LineTimes
		{
			get
			{
				return _vm.GetLineTimes();
			}			
		}

		/// <summary>
		/// Gets the ellipse models from the view model. 
		/// </summary>
		public IList<Ellipse> Ellipses
		{
			get
			{
				// Create the return collection
				List<Ellipse> ellipses = new List<Ellipse>();

				// If the editor is in time based mode then...
				if (_polygonContainer.EditorCapabilities.ShowTimeBar)
				{
					// Loop over the snapshots
					foreach (PolygonSnapshotViewModel snapShot in ((PolygonEditorViewModel)ViewModel).PolygonSnapshots)
					{
						// If a ellipse view model is populated then...
						if (snapShot.EllipseViewModel != null)
						{
							// Add the ellipse to the return collection
							ellipses.Add(snapShot.EllipseViewModel.Ellipse);
						}
					}
				}
				// Otherwise we are not in time based mode
				else
				{
					// Add all the ellipse models to the return collection
					ellipses.AddRange(_vm.EllipseModels);
				}

				// Loop over the ellipse models
				foreach (Ellipse ellipse in ellipses)
				{
					// Scale the polygon model points to the display element dimensions
					ellipse.ScalePoints(1.0 / PolygonPointXConverter.XScaleFactor, 1.0 / PolygonPointYConverter.YScaleFactor);
					ellipse.Center.X *= 1.0 / PolygonPointXConverter.XScaleFactor;
					ellipse.Center.Y *= 1.0 / PolygonPointYConverter.YScaleFactor;
					ellipse.Width *= 1.0 / PolygonPointXConverter.XScaleFactor;
					ellipse.Height *= 1.0 / PolygonPointYConverter.YScaleFactor;
				}

				return ellipses;
			}
		}

		/// <summary>
		/// Gets the times associated with the ellipses.
		/// </summary>
		public IList<double> EllipseTimes
		{
			get
			{
				return _vm.GetEllipseTimes();
			}
		}

		#endregion
	}
}
