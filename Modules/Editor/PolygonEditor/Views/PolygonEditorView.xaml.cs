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
				const double PercentOfScreen = 0.7;
				double wfactor = ((SystemParameters.PrimaryScreenWidth * PercentOfScreen) / _polygonContainer.Width);
				double hfactor = ((SystemParameters.PrimaryScreenHeight * PercentOfScreen) / _polygonContainer.Height);

				// Take the minimum scale factor between the width and height
				double factor = Math.Min(wfactor, hfactor);

				// Determine the X and Y scale factors
				double xScaleFactor = (_polygonContainer.Width * factor - 1.0) / (_polygonContainer.Width - 1.0);
				double yScaleFactor = (_polygonContainer.Height * factor - 1.0) / (_polygonContainer.Height - 1.0);

				// Give the view model the drawing canvas width and height
				vm.CanvasWidth = (int)(_polygonContainer.Width * factor);
				vm.CanvasHeight = (int)(_polygonContainer.Height * factor);

				// Give the polygon point converter the scale factor
				PolygonPointXConverter.XScaleFactor = xScaleFactor;
				PolygonPointYConverter.YScaleFactor = yScaleFactor;

				// Give the view model the model polygons, polygon times
				vm.InitializePolygons(_polygonContainer.Polygons, _polygonContainer.PolygonTimes);

				// Give the view model the model lines, line times, 
				vm.InitializeLines(_polygonContainer.Lines, _polygonContainer.LineTimes);

				// If the edit is in time based mode then...
				if (_polygonContainer.EditorCapabilities.ShowTimeBar)
				{
					// Initialize the snapshots with the polygons/lines
					vm.InitializePolygonSnapShots(
						_polygonContainer.Polygons,
						_polygonContainer.PolygonTimes,
						_polygonContainer.Lines,
						_polygonContainer.LineTimes);
				}
				
				// Initialize the window width to the canvas width
				Width = vm.CanvasWidth; 

				// If the display element is long and tall don't let the window
				// get too skiny
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
					foreach (PolygonSnapShotViewModel snapShot in _vm.PolygonSnapshots)
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
					foreach (PolygonSnapShotViewModel snapShot in ((PolygonEditorViewModel)ViewModel).PolygonSnapshots)
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

		#endregion
	}
}
