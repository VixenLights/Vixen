using Catel.IoC;
using Orc.Theming;
using Orc.Wizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Extensions;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixtureWizard.Wizard;
using VixenModules.Editor.FixtureWizard.Wizard.Models;
using VixenModules.OutputFilter.CoarseFineBreakdown;
using VixenModules.OutputFilter.ColorBreakdown;
using VixenModules.OutputFilter.ColorWheelFilter;
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.OutputFilter.DimmingFilter;
using VixenModules.OutputFilter.PrismFilter;
using VixenModules.OutputFilter.ShutterFilter;
using VixenModules.OutputFilter.TaggedFilter;
using VixenModules.Property.Color;
using VixenModules.Property.IntelligentFixture;

namespace VixenApplication.Setup.ElementTemplates
{
	/// <summary>
	/// Maintains an intelligent fixture element template.
	/// </summary>
	public class IntelligentFixtureTemplate : IElementTemplate
	{
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public IntelligentFixtureTemplate()
        {
			// Default the nodes to delete to empty list
			_nodesToDelete = new List<ElementNode>();
		}

		#endregion

		#region IElementTemplate

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string TemplateName
		{
			get
			{
				return "Intelligent Fixture";
			}
		}

		#endregion

		#region Fields

		/// <summary>
		/// Flag indicating that the color property has been created for the fixture.
		/// </summary>
		private bool _colorPropertyCreated;

		/// <summary>
		/// Fixture wizard for configuring intelligent fixtures.
		/// </summary>
		private IFixtureWizard _wizard;

		/// <summary>
		/// Fixture nodes created by this template.
		/// When a group is created the individual nodes do not need to appear in the element tree.
		/// </summary>
		private List<ElementNode> _nodesToDelete;

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates a coarse fine breakdown module.
		/// </summary>
		/// <returns>Coarse fine breakdown module</returns>
		private CoarseFineBreakdownModule CreateCoarseFineBreakDownModule()
		{
			// Create course / fine breakdown module
			CoarseFineBreakdownModule courseFineBreakdown =
				ApplicationServices.Get<IOutputFilterModuleInstance>(CoarseFineBreakdownDescriptor.ModuleId) as
					CoarseFineBreakdownModule;

			// Add the coarse / fine breakdown module to Vixen collection of filters
			VixenSystem.Filters.AddFilter(courseFineBreakdown);

			// Return the course fine breakdown module
			return courseFineBreakdown;
		}

		/// <summary>
		/// Creates a color property on the specified element node.
		/// </summary>
		/// <param name="node">Element node to associate the color property with</param>
		/// <returns>New color property</returns>
		private ColorModule CreateColorProperty(ElementNode node)
		{
			// Add the properties descriptor to the node
			node.Properties.Add(ColorDescriptor.ModuleId);

			// Retrieve the color property from the node
			ColorModule colorProperty = (ColorModule)node.Properties.Single(prop => prop is ColorModule);
			
			// Return the color property
			return colorProperty;
		}

		/// <summary>
		/// Adds the specified filter to the node.
		/// </summary>
		/// <param name="node">Node to add the filter to</param>
		/// <param name="filter">Filter to add to the node</param>
		private void AddFilterToNode(ElementNode node, OutputFilterModuleInstanceBase filter)
		{
			// Get the data flow component for the specified node
			IDataFlowComponent dataFlowComponent = VixenSystem.DataFlow.GetComponent(node.Element.Id);

			// Create a data flow
			DataFlowComponentReference dataFlowComponentReference = new DataFlowComponentReference(dataFlowComponent, 0);

			// Associate the filter with data flow
			VixenSystem.DataFlow.SetComponentSource(filter, dataFlowComponentReference);

			// Add the filter to the filter manager
			VixenSystem.Filters.AddFilter(filter);
		}

		/// <summary>
		/// Gets all color wheel colors associated with the fixture.
		/// </summary>		
		/// <param name="fixture">Fixture to retrieve color wheel colors for</param>
		/// <returns>Color wheel colors supported by the fixture</returns>
		/// <remarks>
		/// Retrieving all color wheel colors because some fixtures have more than color wheel.
		/// Had some issues trying to add colors to an existing color set so by retrieving all
		/// colors we avoid that issue.
		/// </remarks>
		private List<Color> GetAllColorWheelColors(FixtureSpecification fixture)
        {
			// Create the return collection of colors
			List <Color> colorWheelColors = new List<Color>();

			// Loop over all the channel definitions
			foreach (FixtureChannel channel in fixture.ChannelDefinitions)
			{
				// Retrieve the function associated with the current channel
				FixtureFunction function = fixture.FunctionDefinitions.Single(fn => fn.Name == channel.Function);

				// If the function is a color wheel then...
				if (function.FunctionType == FixtureFunctionType.ColorWheel)
                {
					// Loop over the color wheel data
					foreach (FixtureColorWheel colorWheelData in function.ColorWheelData)
					{
						// If the color is a unique color then...
						if (!colorWheelData.HalfStep &&
							!colorWheelData.UseCurve &&
							!colorWheelData.ExcludeColorProperty)
						{
							// Add the color to the color set
							colorWheelColors.Add(Color.FromArgb(colorWheelData.Color1.ToArgb()));
						}
					}
				}
			}

			return colorWheelColors;	
		}

		/// <summary>
		/// Creates an RGB color property and adds it to the specified node.
		/// </summary>
		/// <param name="node">Node to associate the color property with</param>
		private void AddRGBColorProperty(ElementNode node)
        {
			// Remember that the color property has been created
			_colorPropertyCreated = true;

			// Create a color property and associate it with the node
			ColorModule colorProperty = CreateColorProperty(node);

			// Configure full color (RGB Support)
			colorProperty.ColorType = ElementColorType.FullColor;
		}

		/// <summary>
		/// Creates a discrete color property and adds it to the specified node.
		/// </summary>
		/// <param name="node">Node to associate the color property with</param>
		/// <param name="fixture">Fixture profile being processed</param>
		private void AddDiscreteColorProperty(ElementNode node, FixtureSpecification fixture)
        {
			// Remember that the color property has been created
			_colorPropertyCreated = true;

			// Create a color property
			ColorModule colorProperty = CreateColorProperty(node);
			
			// Configure the color property to use discrete colors
			colorProperty.ColorType = ElementColorType.MultipleDiscreteColors;

			// Create a new color set to hold the discrete colors
			ColorSet colorSet = new ColorSet();

			// Loop over all the color wheel colors associated with the fixture
			foreach (Color color in GetAllColorWheelColors(fixture))
			{
				// Add the color to the color set
				colorSet.Add(color);
			}

			// Retrieve the static color data associated with the color property
			ColorStaticData staticData = (ColorStaticData)colorProperty.StaticModuleData;

			// Add / update the color set
			staticData.SetColorSet(fixture.Name, colorSet);

			// Name the color set after the fixture
			colorProperty.ColorSetName = fixture.Name;			
		}

		/// <summary>
		/// Creates a color wheel output filter for the specified color wheel fixture function.
		/// </summary>
		/// <param name="isLampFixture">True when the fixture uses discrete colors</param>
		/// <param name="function">Color wheel fixture function to create filter for</param>
		/// <param name="node">Fixture node to associate the output filter with</param>
		/// <param name="fixtureName">Name of the fixture</param>
		/// <param name="fixture">Fixture profile being processed</param>
		private void ProcessColorWheelFunction(
			bool isLampFixture, 
			FixtureFunction function, 
			ElementNode node, 
			string fixtureName, 
			FixtureSpecification fixture)
		{
			// Create the color wheel output filter
			ColorWheelFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(ColorWheelFilterDescriptor.ModuleId) as
					ColorWheelFilterModule;

			// Give the filter the color wheel data
			filter.ColorWheelData = function.ColorWheelData;
			
			// Configure the filter on whether to convert RGB Color into color wheel index commands
			// Generally for LED fixtures this will be configured to false
			// Generally for lamp fixtures this will configure to true
			filter.ConvertRGBIntoIndexCommands = isLampFixture;
			
			// Give the filter the name of the function as a tag
			filter.Tag = function.Name;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the filter to the display element node
			AddFilterToNode(node, filter);

			// If the color property for the node has not already been created then...
			if (!_colorPropertyCreated)
			{
				// Create a discrete color property for the node
				AddDiscreteColorProperty(node, fixture);			
			}
		}

		/// <summary>
		/// Creates a color break down item for the specified color.		
		/// </summary>
		/// <param name="color">Color to filter on</param>
		/// <param name="colorName">Name of the color</param>
		/// <returns>Color break down for the specified color</returns>
		/// <fixture>This breakdown item is for RGB and RGBW color mixing fixtures.</fixture>
		private ColorBreakdownItem CreateColorBreakdownItem(Color color, string colorName)
		{
			// Create the color break down item
			ColorBreakdownItem colorBreakDownItem = new ColorBreakdownItem();

			// Assign the color to the break down item
			colorBreakDownItem.Color = color;

			// Give the break down item a name
			colorBreakDownItem.Name = colorName;

			return colorBreakDownItem;
		}

		/// <summary>
		/// Adds the specified filter to the last filter added to the node.
		/// </summary>
		/// <param name="node">Intelligent fixture node</param>
		/// <param name="filter">Filter to add</param>
		private void AddFilterToNodesLastFilter(ElementNode node, OutputFilterModuleInstanceBase filter)
        {
			// Find the leafs of the node
			IList<IDataFlowComponentReference> dataFlowNodes = FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(node.Element.Id)).ToList();

			// Add the filter to the last node				
			VixenSystem.DataFlow.SetComponentSource(filter, dataFlowNodes[dataFlowNodes.Count - 1]);

			// Add the filter to the node
			VixenSystem.Filters.AddFilter(filter);
		}

		/// <summary>
		/// Creates a dimming curve module for the specified dimming curve.
		/// </summary>
		/// <param name="dimmingCurve">Dimming curve to apply to the dimming curve module</param>
		/// <returns>Dimming curve module</returns>
		private DimmingCurveModule CreateDimmingCurve(Curve dimmingCurve)
        {
			// Create the dimming curve module
			DimmingCurveModule dimmingCurveModule = ApplicationServices.Get<IOutputFilterModuleInstance>(DimmingCurveDescriptor.ModuleId) as DimmingCurveModule;

			// Apply the curve to the module
			dimmingCurveModule.DimmingCurve = new Curve(dimmingCurve);

			// Return the dimming module
			return dimmingCurveModule;
		}

		/// <summary>
		/// Scans the specified fixture specification and returns the number of color channels found.
		/// </summary>
		/// <param name="fixture">Fixture specification to analyze</param>
		/// <returns>Number of color channels found</returns>
		private int GetNumberOfColorChannels(FixtureSpecification fixture)
		{
			// Initialize to zero color channels found
			int colorChannels = 0;

			// Loop over the channels on the fixture
			foreach (FixtureChannel channel in fixture.ChannelDefinitions)
			{
				// Find the function associated with the channel
				FixtureFunction function = fixture.FunctionDefinitions.SingleOrDefault(fnc => fnc.Name == channel.Function);

				// If the function was found then...
				if (function != null)	
				{
					// If the function is a color function then...
					if (function.FunctionType == FixtureFunctionType.RGBColor ||
					    function.FunctionType == FixtureFunctionType.RGBWColor)
					{
						// Increment the color channel counter
						colorChannels++;
					}
				}
			}

			return colorChannels;
		}

		/// <summary>
		/// Processes a color mixing fixture function (RGB, RGBW).
		/// </summary>
		/// <param name="foundColorMixingChannel">Flag indicating if a color mixing channel was already found</param>
		/// <param name="colorMixingChannelCount">The number of color mixing channels found so far</param>
		/// <param name="node">Fixture node</param>
		/// <param name="numberOfColorChannelsFoundOnFixture">Number of color mixing channels found on the fixture (3,4,6,8)</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		/// <param name="numberOfExpectedColors">Number of expected colors</param>
		private void ProcessColorMixingFunction(
			ref bool foundColorMixingChannel,
			ref int colorMixingChannelCount,
			ElementNode node,
			Func<List<ColorBreakdownItem>> getBreakDownItems,
			int numberOfColorChannelsFoundOnFixture,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve,
			int numberOfExpectedColors)			
		{
			// If this is the first color mixing channel encountered then...
			if (!foundColorMixingChannel)
			{
				// If one dimming curve for the fixture was selected then...
				if (dimmingCurveSelection == DimmingCurveSelection.OneDimmingCurvePerFixture)
				{
					// Add the dimming module to the flow
					AddFilterToNode(node, CreateDimmingCurve(dimmingCurve));
				}
								
				// Remember that we encountered a color mixing channel
				foundColorMixingChannel = true;

				// Keep track of the number of color mixing channels encountered
				// Expect to see 3 or 4 consecutive channels
				colorMixingChannelCount++;

				// Create the color breakdown module
				ColorBreakdownModule filter =
					ApplicationServices.Get<IOutputFilterModuleInstance>(ColorBreakdownDescriptor.ModuleId)
						as
						ColorBreakdownModule;

				// Create the list of color breakdown items
				List<ColorBreakdownItem> newBreakdownItems = getBreakDownItems();
				
				// Associate the color breakdown items with the filter
				filter.BreakdownItems = newBreakdownItems;

				// Indicate the colors mix to create the desired color
				filter.MixColors = true;

				// If the number of color channels on the fixture is greater than 4 then
				// we are most likely dealing with 16 bit color
				if (numberOfColorChannelsFoundOnFixture > 4)
				{
					// Configure the color breakdown filter to use 16-bit outputs
					filter._16Bit = true;
				}

				// If a dimming curve is in place then...
				if (dimmingCurveSelection == DimmingCurveSelection.OneDimmingCurvePerFixture)
				{
					// Add the color breakdown filter to the output of the dimming curve output
					AddFilterToNodesLastFilter(node, filter);
				}
				else
				{
					// Otherwise add the color breakdown filter to the node
					AddFilterToNode(node, filter);
				}

				// If adding a dimming curve per color then...
				if (dimmingCurveSelection == DimmingCurveSelection.OneDimmingCurvePerColor)
				{
					// Find the leafs of the node
					IList<IDataFlowComponentReference> dataFlowNodes = FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(node.Element.Id)).ToList();

					// Get the last filter (Color Breakdown Filter)
					IDataFlowComponentReference flow = dataFlowNodes[dataFlowNodes.Count - 1];

					// Loop over the color breakdown filter outputs
					for (int index = 0; index < numberOfExpectedColors; index++)
                    {	
						// Create a dimming curve module
						DimmingCurveModule dimmingCurveFilter = CreateDimmingCurve(dimmingCurve);

						// Add the dimming curve module to Vixen collection of filters
						VixenSystem.Filters.AddFilter(dimmingCurveFilter);

						// Add the dimming curve module to the color breakdown output
						VixenSystem.DataFlow.SetComponentSource(dimmingCurveFilter, flow.Component, index);
                    }

					// If the fixture is using 16 bit color then...
					if (filter._16Bit)
					{
						// Find the leafs of the node
						IList<IDataFlowComponentReference> updatedDataFlowNodes =
							FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(node.Element.Id))
								.ToList();

						// Loop over the colors on the breakdown filter
						for (int index = 0; index < numberOfExpectedColors; index++)
						{
							// Get the output on specified dimming curve
							IDataFlowComponentReference dimmingFlow =
								updatedDataFlowNodes[updatedDataFlowNodes.Count - index - 1];

							// Create course / fine breakdown module
							CoarseFineBreakdownModule courseFineBreakdown = CreateCoarseFineBreakDownModule();

							// Add the coarse / fine breakdown module 
							VixenSystem.DataFlow.SetComponentSource(courseFineBreakdown, dimmingFlow.Component, 0);
						}
					}

				}
				// Otherwise if the filter needs to produce 16-bit outputs then...
				else if (filter._16Bit)
				{
					// Find the leafs of the node
					IList<IDataFlowComponentReference> dataFlowNodes = FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(node.Element.Id)).ToList();

					// Get the last filter (Color Breakdown Filter)
					IDataFlowComponentReference flow = dataFlowNodes[dataFlowNodes.Count - 1];

					// Loop over the color breakdown filter outputs
					for (int index = 0; index < numberOfExpectedColors; index++)
					{
						// Create course / fine breakdown module
						CoarseFineBreakdownModule courseFineBreakdown = CreateCoarseFineBreakDownModule();
						
						// Add the coarse / fine breakdown module 
						VixenSystem.DataFlow.SetComponentSource(courseFineBreakdown, flow.Component, index);
					}
				}
			}
			// Otherwise this is the 2nd, 3rd, 4th...8th RGBW channel
			else
			{
				// Increment the number of color mixing channels encountered
				colorMixingChannelCount++;

				// If the 4th RGBW channel was processed then...
				if (colorMixingChannelCount == numberOfColorChannelsFoundOnFixture)
				{
					// Reset the color mixing channel flags
					colorMixingChannelCount = 0;
					foundColorMixingChannel = false;
				}
			}
		}

		/// <summary>
		/// Gets the RGB color break down items.
		/// </summary>
		/// <returns>RGB color break down items</returns>
		private List<ColorBreakdownItem> GetRGBColorBreakdownItems()
        {
			// Create the list of color breakdown items
			List<ColorBreakdownItem> breakdownItems = new List<ColorBreakdownItem>();

			// Create the color break down items for RGBW
			breakdownItems.Add(CreateColorBreakdownItem(Color.Red, "Red"));
			breakdownItems.Add(CreateColorBreakdownItem(Color.Lime, "Green"));
			breakdownItems.Add(CreateColorBreakdownItem(Color.Blue, "Blue"));

			return breakdownItems;
		}

		/// <summary>
		/// Gets the RGBW color break down items.
		/// </summary>
		/// <returns>RGBW color break down items</returns>
		private List<ColorBreakdownItem> GetRGBWColorBreakdownItems()
		{
			// Get the RGB break down items
			List<ColorBreakdownItem> breakdownItems = GetRGBColorBreakdownItems();	
			
			// Add the white break down item
			breakdownItems.Add(CreateColorBreakdownItem(Color.White, "White"));

			return breakdownItems;
		}

		/// <summary>
		/// Processes an RGB fixture function.
		/// </summary>
		/// <param name="foundRGBChannel">Flag indicating if an RGB channel was already found</param>
		/// <param name="rgbChannelCount">The number of color mixing channels found so far</param>
		/// <param name="node">Fixture node</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		/// <param name="numberOfColorChannels">Number of color channels found on the fixture</param>
		private void ProcessRGBFunction(
			ref bool foundRGBChannel,
			ref int rgbChannelCount,
			ElementNode node,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve,
			int numberOfColorChannels)
		{
			// Process the color mixing function
			ProcessColorMixingFunction(
				ref foundRGBChannel,
				ref rgbChannelCount,
				node,
				GetRGBColorBreakdownItems,
				numberOfColorChannels,
				dimmingCurveSelection,
				dimmingCurve,
				3); 				
		}

		/// <summary>
		/// Processes an RGBW fixture function.
		/// </summary>
		/// <param name="foundRGBWChannel">Flag indicating if an RGBW channel was already found</param>
		/// <param name="rgbwChannelCount">The number of color mixing channels found so far</param>
		/// <param name="node">Fixture node</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		/// <param name="numberOfColorChannels">Number of color channels found on the fixture</param>
		private void ProcessRGBWFunction(
			ref bool foundRGBWChannel,
			ref int rgbwChannelCount,
			ElementNode node,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve,
			int numberOfColorChannels)
		{
			// Process the color mixing function
			ProcessColorMixingFunction(
				ref foundRGBWChannel,
				ref rgbwChannelCount,
				node,
				GetRGBWColorBreakdownItems,
				numberOfColorChannels,
				dimmingCurveSelection,
				dimmingCurve,
				4);
		}

		/// <summary>
		/// Processes a dimmer fixture function.
		/// </summary>
		/// <param name="node">Fixture node</param>
		/// <param name="function">Fixture function</param>
		/// <param name="isColorMixing">Indicates if the fixture is color mixing</param>		
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		private void ProcessDimmerFunction(
			ElementNode node, 
			FixtureFunction function, 
			bool isColorMixing, 			
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve)
		{
			// Create the dimming filter
			DimmingFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(DimmingFilterDescriptor.ModuleId) as
					DimmingFilterModule;

			// Assign the filter the function name as the tag
			filter.Tag = function.Name;

			// Configure whether the filter should convert RGB intents into dimming intents
			// Generally this only applies to lamp fixtures
			filter.ConvertColorIntoDimmingIntents = !isColorMixing;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the dimming filter to the node
			AddFilterToNode(node, filter);

			// If the fixture was configured with a dimming curve and
			// it is NOT a color mixing fixture then...
			if (dimmingCurveSelection != DimmingCurveSelection.NoDimmingCurve &&
				!isColorMixing)
			{
				// Add the dimming curve to the flow
				AddFilterToNodesLastFilter(node, CreateDimmingCurve(dimmingCurve));
			}
		}
		
		/// <summary>
		/// Process Open Close Prism function.
		/// </summary>
		/// <param name="node">Fixture node</param>
		/// <param name="function">Fixture function</param>
		private void ProcessOpenClosePrismFunction(ElementNode node, FixtureFunction function)
		{
			// Create the prism filter
			PrismFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(PrismFilterDescriptor.ModuleId) as
					PrismFilterModule;

			// Create a variable to hold the open prism command DMX value
			byte openPrism = 0;

			// Create a variable to hold the close prism command DMX value
			byte closePrism = 0;

			// Loop over the enumerations associated with the function
			foreach (FixtureIndex fixtureIndex in function.IndexData)
			{
				// If an open shutter index is found then...
				if (fixtureIndex.IndexType == FixtureIndexType.PrismOpen &&
					openPrism == 0)
				{
					// Save off the numeric value of the Open Prism
					openPrism = (byte)fixtureIndex.StartValue;
				}

				// If a close shutter index is found then...
				if (fixtureIndex.IndexType == FixtureIndexType.PrismClose &&
					closePrism == 0)
				{
					// Save off the numeric value of the Close Prism 
					closePrism= (byte)fixtureIndex.StartValue;
				}
			}

			// Assign the function name as the filter tag
			filter.Tag = function.Name;

			// Configure whether to generate open prism intents when a prism index intent is encountered			
			filter.ConvertPrismIntentsIntoOpenPrismIntents = true;

			// Assign the open shutter index value
			filter.OpenPrismIndexValue = openPrism;

			// Assign the close prism index value
			filter.ClosePrismIndexValue = closePrism;

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the prism filter to the node
			AddFilterToNode(node, filter);
		}


		/// <summary>
		/// Process shutter function.
		/// </summary>
		/// <param name="node">Fixture node</param>
		/// <param name="function">Fixture function</param>
		private void ProcessShutterFunction(ElementNode node, FixtureFunction function)
		{
			// Create the shutter filter
			ShutterFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(ShutterFilterDescriptor.ModuleId) as
					ShutterFilterModule;

			// Create a variable to hold the open shutter command DMX value
			byte openShutter = 0;

			// Create a variable to hold the close shutter command DMX value
			byte closeShutter = 0;

			// Loop over the enumerations associated with the function
			foreach (FixtureIndex fixtureIndex in function.IndexData)
			{
				// If an open shutter index is found then...
				if (fixtureIndex.IndexType == FixtureIndexType.ShutterOpen &&
					openShutter == 0)
				{
					// Save off the numeric value of the Open Shutter
					openShutter = (byte)fixtureIndex.StartValue;					
				}

				// If a close shutter index is found then...
				if (fixtureIndex.IndexType == FixtureIndexType.ShutterClosed &&
					closeShutter == 0)
				{
					// Save off the numeric value of the Close Shutter
					closeShutter = (byte)fixtureIndex.StartValue;					
				}
			}

			// Assign the function name as the filter tag
			filter.Tag = function.Name;
									
			// Configure whether to generate shutter intents when a color intent is encountered
			// This setting applies to both LED and Lamp fixtures
			filter.ConvertColorIntoShutterIntents = true;
			
			// Assign the open shutter index value
			filter.OpenShutterIndexValue = openShutter;

			// Assign the close shutter index value
			filter.CloseShutterIndexValue = closeShutter;
			
			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the shutter filter to the node
			AddFilterToNode(node, filter);
		}

		/// <summary>
		/// Processes the None functions.
		/// </summary>
		/// <param name="node">Fixture node</param>
		private void ProcessNoneFunction(ElementNode node)
		{
			// Create the tagged filter
			TaggedFilterModule filter =
				ApplicationServices.Get<IOutputFilterModuleInstance>(TaggedFilterDescriptor.ModuleId) as
					TaggedFilterModule;

			// Assign the tag of Ignore so that no intents match
			filter.Tag = "Ignore";

			// Create the output associated with the filter
			filter.CreateOutput();

			// Add the tagged filter to the node
			AddFilterToNode(node, filter);
		}

		/// <summary>
		/// Processes tagged functions.
		/// </summary>
		/// <param name="node">Fixture node</param>
		/// <param name="function">Fixture function being processed</param>
		/// <param name="colorMixing">Whether the fixture supports color mixing</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		private void ProcessTaggedFunction(
			ElementNode node,
			FixtureFunction function,
			bool colorMixing,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve,
			bool automaticallyControlDimmer)
		{
			// If the function is a dimming function then...
			if (function.FunctionIdentity == FunctionIdentity.Dim &&
			    automaticallyControlDimmer)
			{
				// Process the dimmer function
				ProcessDimmerFunction(node, function, colorMixing, dimmingCurveSelection, dimmingCurve);
			}
			else
			{
				// Create the tagged filter
				TaggedFilterModule filter =
					ApplicationServices.Get<IOutputFilterModuleInstance>(TaggedFilterDescriptor.ModuleId) as
						TaggedFilterModule;

				// Assign the function name as the tag
				filter.Tag = function.Name;

				// Create the output associated with the filter
				filter.CreateOutput();

				// Add the tagged filter to the node
				AddFilterToNode(node, filter);
			}
		}

		/// <summary>
		/// Add a courase / fine break down filter.
		/// </summary>
		/// <param name="node">Fixture node</param>
		private void AddCourseFineBreakdown(ElementNode node)
		{
			// Create course / fine breakdown module
			CoarseFineBreakdownModule courseFineBreakdown = CreateCoarseFineBreakDownModule();
			
			// Find the leafs of the node
			IList<IDataFlowComponentReference> nodes = FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(node.Element.Id)).ToList();

			// Add the break down module to the last node
			// (This node is usually a tagged filter)
			VixenSystem.DataFlow.SetComponentSource(courseFineBreakdown, nodes[nodes.Count - 1]);
		}
		
		/// <summary>
		/// Shows the intelligent fixture wizard.
		/// </summary>		
		/// <returns>Intelligent fixture wizard task</returns>
		private async Task<bool?> ShowWizardAsync()
		{
			// Get the Catel type factory
			ITypeFactory typeFactory = this.GetTypeFactory();

			// Use the type factory to create the intelligent fixture wizard
			_wizard = typeFactory.CreateInstance(typeof(IntelligentFixtureWizard)) as IFixtureWizard;

			// Configure the wizard window to show up in the Windows task bar
			_wizard.ShowInTaskbarWrapper = true;
			
			// Enable the help button
			_wizard.ShowHelpWrapper = true;

			// Configure the wizard to allow the user to jump between already visited pages
			_wizard.AllowQuickNavigationWrapper = true;

			// Allow Catel to help determine when it is safe to transition to the next wizard page
			_wizard.HandleNavigationStatesWrapper = true;

			// Configure the wizard to NOT cache views
			_wizard.CacheViewsWrapper = false;

			// Configure the wizard with a navigation controller														
			_wizard.NavigationControllerWrapper = typeFactory.CreateInstanceWithParametersAndAutoCompletion<FixtureWizardNavigationController>(_wizard);
					
			// Create the wizard service
			IDependencyResolver dependencyResolver = this.GetDependencyResolver();
			IWizardService wizardService = (IWizardService)dependencyResolver.Resolve(typeof(IWizardService));

			// Display the intelligent fixture wizard
			bool? result = await wizardService.ShowWizardAsync(_wizard);

			// Return whether the wizard was completed or cancelled
			return result;
		}

		/// <summary>
		/// Processes the specified index function.
		/// </summary>
		/// <param name="function">Fixture function being processed</param>
		/// <param name="node">Fixture node</param>
		/// <param name="colorMixing">Whether the fixture supports color mixing</param>
		/// <param name="automaticallyOpenAndCloseShutter">Whether to configure the filters to automatically open and close the shutter</param>
		/// <param name="automaticallyOpenAndClosePrism">Whether to configure the filters to automatically open and close the prism</param>
		private void ProcessIndexedFunction(
			FixtureFunction function,
			ElementNode node,
			bool colorMixing,
			bool automaticallyOpenAndCloseShutter,
			bool automaticallyOpenAndClosePrism)			
        {			
			// If the function is a shutter function and 
			// shutter is being automated then...
			if (function.FunctionIdentity == FunctionIdentity.Shutter &&
			    automaticallyOpenAndCloseShutter)
			{
				// Add a special shutter filter that responds to color intents
				ProcessShutterFunction(node, function);
			}
			// If the function is a open/close prism function then...
			else if (function.FunctionIdentity == FunctionIdentity.OpenClosePrism)
			{
				// Add a special shutter filter that responds to prism index intents
				ProcessOpenClosePrismFunction(node, function);
			}
			// Otherwise this is a generic tagged function
			else
			{
				ProcessTaggedFunction(node, function, colorMixing, DimmingCurveSelection.NoDimmingCurve, null, false);
			}
		}

		/// <summary>
		/// Processes the specified function.
		/// </summary>
		/// <param name="function">Fixture function being processed</param>
		/// <param name="node">Intelligent fixture node</param>
		/// <param name="fixture">Fixture profile / specification</param>
		/// <param name="colorMixing">Whether the fixture supports color mixing</param>		
		/// <param name="foundColorMixingChannel">Flag that indicates a color mixing channel has been found</param>
		/// <param name="colorMixingChannelCount">Keeps track of the number of color mixing channels processed</param>
		/// <param name="automaticallyOpenAndCloseShutter">Whether to configure the filters to automatically open and close the shutter</param>
		/// <param name="automaticallyOpenAndClosePrism">Whether to configure the filters to automatically open and close the prism</param>
		/// <param name="nextChannelFunction">The next fixture function in the profile/specification</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		/// <param name="automaticallyControlDimmer">Whether to configure the filters to convert color intensity into a dim command</param>
		/// <param name="automaticallyControlColorWheel">Whether to configure the filters to convert colors into color wheel commands</param>
		private void ProcessFunction(
			FixtureFunction function, 
			ElementNode node, 
			FixtureSpecification fixture, 
			bool colorMixing,
			ref bool foundColorMixingChannel,
			ref int colorMixingChannelCount,
			bool automaticallyOpenAndCloseShutter,
			bool automaticallyOpenAndClosePrism,
			FixtureFunction nextChannelFunction,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve,
			bool automaticallyControlDimmer,
			bool automaticallyControlColorWheel)
        {
			// Process the function based on its type
			switch(function.FunctionType)
            {
				case FixtureFunctionType.ColorWheel:
					// If Vixen is to automatically convert colors into color wheel commands then...
					if (automaticallyControlColorWheel)
					{
						ProcessColorWheelFunction(!colorMixing, function, node, fixture.Name, fixture);
					}
					else
					{
						ProcessIndexedFunction(function, node, colorMixing, automaticallyOpenAndCloseShutter, automaticallyOpenAndClosePrism);
					}
					break;
				case FixtureFunctionType.RGBWColor:
					ProcessRGBWFunction(ref foundColorMixingChannel, ref colorMixingChannelCount, node, dimmingCurveSelection, dimmingCurve, GetNumberOfColorChannels(fixture));
					break;
				case FixtureFunctionType.RGBColor:
					ProcessRGBFunction(ref foundColorMixingChannel, ref colorMixingChannelCount, node, dimmingCurveSelection, dimmingCurve, GetNumberOfColorChannels(fixture));
					break;
				case FixtureFunctionType.Indexed:
					ProcessIndexedFunction(function, node, colorMixing, automaticallyOpenAndCloseShutter, automaticallyOpenAndClosePrism);
					break;
				case FixtureFunctionType.Range:
					ProcessTaggedFunction(node, function, colorMixing, dimmingCurveSelection, dimmingCurve, automaticallyControlDimmer);
					break;
				case FixtureFunctionType.None:
					ProcessNoneFunction(node);
					break;
				default:
					Debug.Assert(false, "Unsupported Function Type!");
					break;
			}
												
			// If the next two channels are range functions with the same function name then...
			if (function.FunctionType == FixtureFunctionType.Range &&
				nextChannelFunction != null &&
				nextChannelFunction.FunctionType == FixtureFunctionType.Range &&
				function.Name == nextChannelFunction.Name)
			{
				// Add a course / fine breakdown module
				AddCourseFineBreakdown(node);
			}
		}

		/// <summary>
		/// Creates a intelligent fixture node.
		/// </summary>
		/// <param name="fixtureName">Name of the fixture</param>
		/// <param name="fixtureSpecification">Fixture profile associated with the fixture</param>
		/// <param name="colorMixing">Whether the fixture supports color mixing</param>
		/// <param name="automaticallyOpenAndCloseShutter">Whether to configure the filters to automatically open and close the shutter</param>
		/// <param name="automaticallyControlColorWheel">Whether to configure the filters to convert colors into color wheel commands</param>
		/// <param name="automaticallyControlDimmer">Whether to configure the filters to convert color intensity into a dim command</param>
		/// <param name="automaticallyOpenAndClosePrism">Whether to configure the filters to automatically open and close the prism</param>
		/// <param name="dimmingCurveSelection">Dimming curve filter selection</param>
		/// <param name="dimmingCurve">Dimming curve to insert</param>
		/// <returns>Configured intelligent fixture node</returns>
		private ElementNode CreateFixtureNode(
			string fixtureName,
			FixtureSpecification fixtureSpecification,
			bool colorMixing,
			bool automaticallyOpenAndCloseShutter,
			bool automaticallyControlColorWheel,
			bool automaticallyControlDimmer,
			bool automaticallyOpenAndClosePrism,
			DimmingCurveSelection dimmingCurveSelection,
			Curve dimmingCurve)
		{
			// Reset the flag indicating a color property has been created
			// Fixtures can have color wheels and individual color channels.  This flag makes sure only one color property is created.
			_colorPropertyCreated = false;

			// Create the new display element node
			ElementNode node = ElementNodeService.Instance.CreateSingle(null, fixtureName, true, true);

			// Add the intelligent fixture property to the new node
			node.Properties.Add(IntelligentFixtureDescriptor._typeId);

			// Retrieve the intelligent 
			IntelligentFixtureModule intelligentFixtureProperty =
				(IntelligentFixtureModule)node.Properties.Single(prop => prop is IntelligentFixtureModule);

			// Set the fixture profile on the fixture property
			intelligentFixtureProperty.FixtureSpecification = fixtureSpecification;
			FixtureSpecification fixture = intelligentFixtureProperty.FixtureSpecification;			

			// Local variable to keep track of the last function processed
			string lastFunction = string.Empty;

			// Local variables to keep track of if we have encountered an RGBW channel
			bool foundColorMixingChannel = false;
			int colorMixingChannelCount = 0;
			
			// If the fixture is a color mixing fixture then...
			if (colorMixing)
			{
				// Create a color mixing color property
				AddRGBColorProperty(node);
			}

			// Loop over the channels on the fixture
			int channelIndex = 0;
			foreach (FixtureChannel channel in fixture.ChannelDefinitions)
			{
				// Declare variables for the next 
				FixtureChannel nextChannel = null;
				FixtureFunction nextChannelFunction = null;

				// If there are additional channels in the definitions then...
				if (channelIndex + 1 < fixture.ChannelDefinitions.Count)
				{
					// Retrieve the next channel
					nextChannel = fixture.ChannelDefinitions[channelIndex + 1];

					// Retrieve the next channel's function
					nextChannelFunction = fixture.FunctionDefinitions.Single(fn => fn.Name == nextChannel.Function);
				}

				// Retrieve the function associated with the current channel
				FixtureFunction function = fixture.FunctionDefinitions.Single(fn => fn.Name == channel.Function);

				// If the channel's function is different from the last function processed then...
				if (channel.Function != lastFunction ||
				    channel.Function == FixtureFunctionType.None.GetEnumDescription())
				{
					// Update the last function processed
					lastFunction = channel.Function;
					
					// Process the fixture function
					ProcessFunction(
						function,
						node,
						fixture,
						colorMixing,
						ref foundColorMixingChannel,
						ref colorMixingChannelCount,
						automaticallyOpenAndCloseShutter,
						automaticallyOpenAndClosePrism,
						nextChannelFunction,
						dimmingCurveSelection,
						dimmingCurve,
						automaticallyControlDimmer,
						automaticallyControlColorWheel);
				}

				// Increment the channel counter
				channelIndex++;
			}

			return node;
		}
		
		/// <summary>
		/// This utility method was copied from ColorSetupHelper.cs.
		/// </summary>
		/// <param name="component">Data flow to find leaves from</param>
		/// <returns>Collection of data flow leaves</returns>
		private IEnumerable<IDataFlowComponentReference> FindLeafOutputsOrBreakdownFilters(IDataFlowComponent component)
		{
			if (component == null)
			{
				yield break;
			}

			if (component is ColorBreakdownModule)
			{
				yield return new DataFlowComponentReference(component, -1);
				// this is a bit iffy -- -1 as a component output index -- but hey.
			}

			if (component.Outputs == null || component.OutputDataType == DataFlowType.None)
			{
				yield break;
			}

			for (int i = 0; i < component.Outputs.Length; i++)
			{
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				if (!children.Any())
				{
					yield return new DataFlowComponentReference(component, i);
				}
				else
				{
					foreach (IDataFlowComponent child in children)
					{
						foreach (IDataFlowComponentReference result in FindLeafOutputsOrBreakdownFilters(child))
						{
							yield return result;
						}
					}
				}
			}
		}

		#endregion

		#region IElementTemplate
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			// Without this call the wizard will produce the following exception:
			// 'Theming is not yet initialized, make sure to initialize a theme via ThemeManager first'
			ThemeManager.Current.SynchronizeTheme();

			// Create the Catel dependency resolver
			IDependencyResolver dependencyResolver = this.GetDependencyResolver();

			// Retrieve the color scheme service
			IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

			// Select the dark color scheme
			baseColorService.SetBaseColorScheme("Dark");

			// Retrieve the accent color service
			IAccentColorService accentColorServer = (IAccentColorService)dependencyResolver.Resolve(typeof(IAccentColorService));

			// Configure the page bubbles on the left to be blue to look better with the dark theme
			accentColorServer.SetAccentColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("DodgerBlue"));

			// Show the fixture wizard
			bool? cancelled = await ShowWizardAsync();

			// Determine if the wizard was cancelled 
			Cancelled = (cancelled.HasValue && !cancelled.Value);

			// Create a collection for the fixture nodes we are creating
			List<ElementNode> nodes = new List<ElementNode>();

			// If the wizard was NOT cancelled then...
			if (!Cancelled)
			{
				// Retrieve the pages from the wizard so that the selected state can be extracted
				SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)_wizard.Pages.Single(page => page is SelectProfileWizardPage);
				GroupingWizardPage groupingPage = (GroupingWizardPage)_wizard.Pages.Single(page => page is GroupingWizardPage);
				AutomationWizardPage automationPage = (AutomationWizardPage)_wizard.Pages.Single(page => page is AutomationWizardPage);
				ColorSupportWizardPage colorSupportPage = (ColorSupportWizardPage)_wizard.Pages.Single(page => page is ColorSupportWizardPage);
				DimmingCurveWizardPage dimmingCurvePage = (DimmingCurveWizardPage)_wizard.Pages.Single(page => page is DimmingCurveWizardPage);

				// Save the fixture
				FixtureSpecificationManager.Instance().Save(selectProfilePage.Fixture);
				
				// Loop over the number of fixture nodes to create
				for (int index = 0; index < groupingPage.NumberOfFixtures; index++)
				{
					// Base the fixture name on the index of the node
					string fixtureName = groupingPage.ElementPrefix + (index + 1).ToString();

					// Create the fixture node
					nodes.Add(CreateFixtureNode(
						fixtureName,
						selectProfilePage.Fixture,
						colorSupportPage.ColorMixing,
						automationPage.AutomaticallyOpenAndCloseShutter,
						automationPage.AutomaticallyControlColorWheel,
						automationPage.AutomaticallyControlDimmer,
						automationPage.AutomaticallyOpenAndClosePrism,
						dimmingCurvePage.GetDimmingCurveSelection(),
						dimmingCurvePage.DimmingCurve));
				}
				
				// If the user selected to create a group of fixtures then...
				if (groupingPage.CreateGroup)
				{
					// Create the group with the specified name
					ElementNode groupNode = ElementNodeService.Instance.CreateSingle(null, groupingPage.GroupName, false);

					// Loop over the fixtures
					foreach (ElementNode node in nodes)
					{
						// Add the specified fixture to the group
						groupNode.AddChild(node);
					}

					// Save off the individual fixture nodes so they can be removed from the tree
					_nodesToDelete = nodes;

					// Return the group node
					nodes = new List<ElementNode>() { groupNode };
				}
			}
			
			return nodes;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			// Nothing to setup
			return true;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureColor
		{
			get
			{
				// A color property is configured as part of the wizard so additional dialogs are not necessary
				return false;
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureDimming
		{
			get 
			{ 
				// Not supporting dimming curves for fixtures
				return false; 
			}				
		}
		
		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool Cancelled
        {
			get;
			private set;
        }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IEnumerable<ElementNode> GetElementsToDelete()
		{
			return _nodesToDelete;
		}

		#endregion
	}
}
