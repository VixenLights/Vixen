using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pulse;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.IntelligentFixture;


namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Fixture catch all effect.  Supports fixture functions that are not covered by the other fixture effects.
	/// </summary>
	public class FixtureModule : FixtureEffectBase<FixtureData>
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureModule() :
			// Pass the base class the help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/fixture/")
		{
			// Create the collection of fixture expando objects
			_functionItemCollection = new FixtureFunctionExpandoCollection();

			// Initialize the collection of fixture functions
			_fixtureFunctions = new List<FixtureFunction>();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Collection of fixture functions supported by the nodes.
		/// </summary>
		private List<FixtureFunction> _fixtureFunctions;

		#endregion

		#region Public Static Properties

		/// <summary>
		/// This property supports adding new fixture functions to the collection.
		/// This property allows the child fixture function expando object to configure itself based
		/// on the type of function the user picked from the drop down.
		/// </summary>
		public static FixtureFunction ActiveFunction { get; set; }

		public static List<FixtureFunction> FixtureFunctions { get; set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Backing field for selected function.
		/// </summary>
		private string _selectedFunction;

		/// <summary>
		/// Fixture function drop down to allow user to pick the function to add to the effect.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Functions")]
		[ProviderDescription(@"Functions")]		
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(FunctionCollectionNameConverter))]		
		[PropertyOrder(1)]
		public string SelectedFunction
		{
			get
			{				
				return _selectedFunction;				
			}
			set
			{
				// Store off the selected function name
				_selectedFunction = value;

				// Retrieve the fixture function object corresponding to the selection
				ActiveFunction = _fixtureFunctions.FirstOrDefault(func => func.Name == _selectedFunction);
				
				// Expose the fixture functions
				FixtureFunctions = _fixtureFunctions;
				
				MarkDirty();
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Functions collection backing field.
		/// </summary>
		private FixtureFunctionExpandoCollection _functionItemCollection;

		/// <summary>
		/// Provides a collection of fixture functions that the effect is going to render.
		/// </summary>
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Fixture Functions")]
		[ProviderDescription(@"Functions")]
		[PropertyOrder(2)]
		public FixtureFunctionExpandoCollection Functions
		{
			get
			{
				return _functionItemCollection;
			}
			set
			{
				_functionItemCollection = value;
				MarkDirty();
				OnPropertyChanged();
			}
		}

		#endregion

		#region IModuleInstance

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public override IModuleDataModel ModuleData
		{
			get
			{

				// Ensure the fixture model data is up to date
				UpdateModelData();

				return Data;
			}
			set
			{
				Data = (FixtureData)value;
				UpdateFunctionViewModel(Data);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the list of fixture functions supported by the selected node(s).
		/// </summary>
		/// <returns>Collection of fixture functions supported by the selected node(s)</returns>
		public List<string> GetFunctionNames()
		{
			// Get the list of fixture functions suppored by the node(s)
			List<FixtureFunction> functions = GetFixtureFunctions();

			// Retrieve the name of each fixture function
			return functions.Select(f => f.Name).Distinct().ToList();
		}

		/// <summary>
		/// Generates the visual representation of the effect on the timeline.
		/// </summary>
		/// <param name="graphics">Graphics context</param>
		/// <param name="clipRectangle">Clipping rectangle of the effect on the timeline</param>
		public override void GenerateVisualRepresentation(Graphics graphics, Rectangle clipRectangle)
		{
			// Create a slightly smaller clipping rectangle
			Rectangle clippingRect = new Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, clipRectangle.Height - 4);

			// Retrieve the timeline colors from the function view models
			List<Color> timelineColors = Functions.Select(function => function.TimelineColor).ToList();

			// Draw horizontal bars of color to represent the fixture functions
			Bitmap bars = DrawHorizontalBars(new Size(clipRectangle.Width, clipRectangle.Height), timelineColors);

			// Draw the bars bitmap on the graphics context (timeline)
			graphics.DrawImage(bars, 0, 0);

			// If there is at least one fixture function then...
			if (timelineColors.Count != 0)
			{
				// Draw the effect name in a black font
				DrawText(graphics, clippingRect, Color.Black, "Fixture", 0, 0, clippingRect.Height);
			}
			else
			{
				// Otherwise draw the effect ame in a white font
				DrawText(graphics, clippingRect, Color.White, "Fixture", 0, 0, clippingRect.Height);
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Updates the visibility of the effect attributes.
		/// </summary>
		protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{								
				// No Control to update by default
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Renders fixture functions associated with the effect.
		/// </summary>
		/// <param name="cancellationToken">Indicator if the rendering has been cancelled</param>
		protected override void PreRenderInternal(CancellationTokenSource cancellationToken = null)
		{
			// Loop over the fixture functions associated with the effect
			foreach (FixtureFunctionExpando function in Functions)
			{
				// The function name is blank if the effect is placed on a non-fixture
				if (!string.IsNullOrEmpty(function.FunctionName))
				{
					// Retrieve the fixture function from the fixture specification
					FixtureFunction fixtureFunction = GetFunction(function.FunctionName);

					// If the fixture function was found then...
					if (fixtureFunction != null)
					{
						// Render based on the fixture function type
						switch (function.FunctionType)
						{
							case FixtureFunctionType.Range:
								RenderCurve(function.Range, fixtureFunction, cancellationToken);
								break;
							case FixtureFunctionType.Indexed:
								RenderIndexed(function.IndexValue, function.Range, fixtureFunction, cancellationToken);
								break;
							case FixtureFunctionType.ColorWheel:
								RenderIndexed(function.ColorIndexValue, function.Range, fixtureFunction, cancellationToken);
								break;
							case FixtureFunctionType.RGBWColor:
							case FixtureFunctionType.RGBColor:
								RenderRGB(function.Color, function.Range, fixtureFunction, cancellationToken);
								break;
							default:
								Debug.Assert(false, "Unsupported Function Type");
								break;
						}
					}
				}
			}

			UpdateModelData();
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// Update the fixture functions associated with the node(s)
			_fixtureFunctions = GetFixtureFunctions();

			// Create a collection of fixture functions to remove
			List<FixtureFunctionData> functionsToRemove = new List<FixtureFunctionData>();

			// Loop over the fixture function data objects
			foreach (FixtureFunctionData function in Data.FunctionData)
			{
				// If the function is no longer associated with the node then...
				if (!_fixtureFunctions.Any(fn => fn.Name == function.FunctionName))
				{
					// Add the function to the collection of functions to remove from the effect
					functionsToRemove.Add(function);
				}
			}

			// Loop over the functions to remove from the effect
			foreach (FixtureFunctionData function in functionsToRemove)
			{
				// Remove the function from the data
				Data.FunctionData.Remove(function);
			}

			// Convert the fixture function data into view models
			UpdateFunctionViewModel(Data);

			// If there are fixture functions then...
			if (_fixtureFunctions.Count > 0)
			{
				// Default the selected function to the first function
				SelectedFunction = _fixtureFunctions.First().Name;
			}
		}

		#endregion

		#region Protected Properties

		/// <inheritdoc />
		protected override EffectTypeModuleData EffectModuleData
		{
			get
			{
				// Ensure the fixture model data is up to date
				UpdateModelData();

				return Data;
			}
		}

		#endregion

		#region Private Methods
				
		/// <summary>
		/// Converts from the serialized fixture function data to the model fixture data.
		/// </summary>		
		private void UpdateFunctionViewModel(FixtureData fixtureData)
		{
			// Create a new collection of fixture function expando objects
			FixtureFunctionExpandoCollection functionItemCollection = new FixtureFunctionExpandoCollection();

			// Loop over the ranges in the serialized effect data
			foreach (FixtureFunctionData fixtureFunctionData in fixtureData.FunctionData)
			{
				// Create a new fixture function in the model
				FixtureFunctionExpando fixtureFunctionExpando = new FixtureFunctionExpando();

				// Give the fixture function expando object the collection of fixture data models
				fixtureFunctionExpando.FixtureFunctions = fixtureData.FixtureFunctions;

				// Transfer the properties from the serialized effect data to the fixture function model
				fixtureFunctionExpando.Range = new Curve(fixtureFunctionData.Range);
				fixtureFunctionExpando.FunctionIdentity = fixtureFunctionData.FunctionIdentity;
				fixtureFunctionExpando.FunctionName = fixtureFunctionData.FunctionName;
				fixtureFunctionExpando.FunctionType = fixtureFunctionData.FunctionType;
				fixtureFunctionExpando.IndexValue = fixtureFunctionData.IndexValue;
				fixtureFunctionExpando.ColorIndexValue = fixtureFunctionData.ColorIndexValue;
				fixtureFunctionExpando.TimelineColor = fixtureFunctionData.TimelineColor;

				// Add the fixture function to the collection
				functionItemCollection.Add(fixtureFunctionExpando);
			}

			// Replace the fixture functions exposed by the effect
			Functions = functionItemCollection;
		}

		/// <summary>
		/// Update the fixture function model data.
		/// </summary>
		private void UpdateModelData()
		{
			// Save off the fixture functions
			Data.FixtureFunctions = _fixtureFunctions;

			// Clear the function data collection
			Data.FunctionData.Clear();

			// Loop over the view model functions
			foreach (IFixtureFunctionExpando functionViewModel in Functions)
			{
				// Create a new fixture function model object
				FixtureFunctionData functionModel = new FixtureFunctionData();
				
				// Copy the settings from the view model to the model
				functionModel.FunctionName = functionViewModel.FunctionName;
				functionModel.Range = new Curve(functionViewModel.Range);
				functionModel.FunctionIdentity = functionViewModel.FunctionIdentity;
				functionModel.IndexValue = functionViewModel.IndexValue;
				functionModel.ColorIndexValue = functionViewModel.ColorIndexValue;
				functionModel.FunctionType = functionViewModel.FunctionType;
				functionModel.TimelineColor = functionViewModel.TimelineColor;	

				// Add the fixture function model to the collection
				Data.FunctionData.Add(functionModel);
			}
		}

		/// <summary>
		/// Renders the spedified fixture function range (curve).
		/// </summary>
		/// <param name="curve">Curve to render</param>		
		/// <param name="function">Fixture function to render</param>
		/// <param name="cancellationToken">Indicator if the rendering was cancelled</param>
		private void RenderCurve(Curve curve, FixtureFunction function, CancellationTokenSource cancellationToken)
		{
			// Get the element nodes associated with the specified named function
			IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

			// Render the curve for the specified nodes and fixture function
			RenderCurve(nodes, curve, function.FunctionIdentity, function.Name, null, cancellationToken);
		}
	
		/// <summary>
		/// Retrieves the fixture function with the specified name.
		/// </summary>
		/// <param name="functionName">Name of the function to find</param>
		/// <returns>Fixture function with the specified name</returns>
		private FixtureFunction GetFunction(string functionName)
		{
			// Return the first fixture function that matches the name
			return _fixtureFunctions.FirstOrDefault(fn => fn.Name == functionName);
		}

		/// <summary>
		/// Retrieves the leaf nodes that support the specified function name.
		/// </summary>
		/// <param name="functionName">Function name to search for</param>
		/// <returns></returns>
		private IEnumerable<IElementNode> GetRenderNodesForFunctionType(string functionName)
		{
			// Create the return collection
			List<IElementNode> leavesThatSupportFunction = new List<IElementNode>();

			// Retrieve the first taret node
			IElementNode node = TargetNodes.FirstOrDefault();

			// If the node is NOT null then...
			if (node != null)
			{
				// Loop over the leaves
				foreach (IElementNode leafNode in node.GetLeafEnumerator())
				{
					// Attempt to get the Intelligent Fixture property
					IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)leafNode.Properties.FirstOrDefault(item => item is IntelligentFixtureModule);

					// If a fixture property was found then...
					if (fixtureProperty != null)
					{
						// If the fixture supports the function then...
						if (fixtureProperty.FixtureSpecification.SupportsFunction(functionName))
						{
							// Add the leaf to the collection of nodes to return
							leavesThatSupportFunction.Add(leafNode);
						}
					}
				}
			}

			return leavesThatSupportFunction;							
		}

		/// <summary>
		/// Render the color function.
		/// </summary>
		/// <param name="colorGradient">Color gradient to render</param>
		/// <param name="intensity">Itensity curve for the color</param>
		/// <param name="function">Fixture function to render</param>
		/// <param name="cancellationToken">Indicator if the rendering has been cancelled</param>
		private void RenderRGB(ColorGradient colorGradient, Curve intensity, FixtureFunction function, CancellationTokenSource cancellationToken = null)
		{
			// Retrieve the nodes associated with the function
			IEnumerable<IElementNode> nodes = GetRenderNodesForFunctionType(function.Name);

			// If there are any nodes associated with the function then...
			if (nodes.Any())
			{
				// Loop over the frames that make up the effect
				for (int frameNum = 0; frameNum < GetNumberFrames(); frameNum++)
				{
					// For each node being rendered then...
					foreach (IElementNode node in nodes)
					{
						// Add the color intents to the output of the effect
						EffectIntentCollection.Add(RenderColorPulse(node, intensity, colorGradient));
					}
				}
			}
		}

		/// <summary>
		/// Render a color pulse on the specified node.
		/// </summary>
		/// <param name="node">Node to rendeer intents for</param>
		/// <param name="intensity">Intensity curve to render</param>
		/// <param name="colorGradient">Color gradient to render</param>
		/// <returns></returns>
		private EffectIntents RenderColorPulse(IElementNode node, Curve intensity, ColorGradient colorGradient)
		{
			// Delegate to the pulse effect helper class
			return PulseRenderer.RenderNode(node, intensity, colorGradient, TimeSpan, false);
		}

		/// <summary>
		/// Draws horizontal bars of color on a bitmap for the effect's graphical representation on the timeline.
		/// </summary>
		/// <param name="size">Size of the drawing area on the timeline</param>
		/// <param name="colors">Colors of the bars</param>
		/// <returns>Bitmap filled with horizontal bars of the specified colors</returns>
		public Bitmap DrawHorizontalBars(
			Size size,												
			List<Color> colors)
		{			
			// Create the bitmap to draw into
			Bitmap barBitmap = new Bitmap(size.Width, size.Height);

			// Make a copy of the colors
			List<Color> barColors = new List<Color>(colors);

			// If there are no colors then...
			if (barColors.Count == 0)
			{
				// Default bitmap to one bar of black
				barColors.Add(Color.Black);
			}

			// Create a graphics context using the bitmap
			using (Graphics g = Graphics.FromImage(barBitmap))
			{										
				// Determine the height of each bar
				int heightOfBar = size.Height / barColors.Count;
				
				// Start drawing at the bottom
				int y = 0;

				// Loop over the colors
				for(int colorIndex = 0; colorIndex < barColors.Count; colorIndex++)
				{
					// Create a solid brush using the bar color
					using (Brush barBrush = new SolidBrush(barColors[colorIndex]))
					{
						// If this is the last bar then...
						if (colorIndex == barColors.Count - 1)
						{
							// Extend the bar to fill the area
							g.FillRectangle(barBrush, new Rectangle(0, y, size.Width, size.Height - y));							
						}
						else
						{
							// Draw the color bar
							g.FillRectangle(barBrush, new Rectangle(0, y, size.Width, heightOfBar));
						}

						// Increment the Y coordinate of the bar
						y += heightOfBar;
					}
				}								
			}

			return barBitmap;
		}

		#endregion
	}
}