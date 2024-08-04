using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Antlr4.Runtime;

using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.IntelligentFixture;

using ZedGraph;

using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VixenModules.Effect.Fan
{
	/// <summary>
	/// Creates an effect that draws a brick pattern and animates it.
	/// </summary>
	public class FanModule : FixtureEffectBase<FanData>
	{
		#region Private Fields

		/// <summary>
		/// This frame buffer contains a tile of the weave.
		/// This tile can be repeated to fill the display element.
		/// </summary>
		private IPixelFrameBuffer _tileFrameBuffer;

		/// <summary>
		/// Scale factor used on all the tile pattern settings.
		/// </summary>
		private int _scaleValue;

		/// <summary>
		/// Height of the frame buffer.  When in string mode with a rotation this height has been increased to support
		/// rotating the original frame buffer.
		/// </summary>
		private int _bufferHt;

		/// <summary>
		/// Width of the frame buffer.  When in string mode with a rotation this width has been increased to support
		/// rotating the original frame buffer.
		/// </summary>
		private int _bufferWi;

		/// <summary>
		/// This field represents both the width and height of the frame buffer when a rotation is being applied.
		/// This field is only used in string mode.
		/// </summary>
		private int _length;

		/// <summary>
		/// Height of the repeating tile.
		/// </summary>
		private int _heightOfTile;

		/// <summary>
		/// Width of the repeating tile.
		/// </summary>
		private int _widthOfTile;

		/// <summary>
		/// Effect data (settings) associated with the effect.
		/// </summary>
		private FanData _data;

		/// <summary>
		/// Weave horizontal thickness in pixels.
		/// </summary>
		private int _weaveHorizontalThickness;

		/// <summary>
		/// Weave vertical thickness in pixels.
		/// </summary>
		private int _weaveVerticalThickness;

		/// <summary>
		/// Horizontal spacing between the weave in pixels.
		/// </summary>
		private int _weaveHorizontalSpacing;

		/// <summary>
		/// Vertical spacing between the weave in pixels.
		/// </summary>
		private int _weaveVerticalSpacing;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FanModule() : base("https://webtest.vixenlights.com/docs/usage/sequencer/effects/pixel/fan/")
		{
			// Create the data (settings) associated with the effect
			_data = new FanData();

			// Create the dictionary of pan function tag names indexed by element node
			_panTags = new Dictionary<IElementNode, string>();

			// Initialize the enabled status of the controls
			InitAllAttributes();
		}

		#endregion

		#region Public (Override) Methods

		/*
		///<inheritdoc/>
		public override bool IsDirty
		{
			get
			{
				// If any linked color gradients have changed then...
				if (HorizontalColors.Any(x => !x.CheckLibraryReference()) ||
					VerticalColors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set
			{
				base.IsDirty = value;
			}
		}
		*/

		#endregion

		#region Public Properties

		///<inheritdoc/>
		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as FanData;
				
				// Re-initialize all the controls after getting new module data
				InitAllAttributes();
				
				IsDirty = true;
			}
		}

		#endregion
		
		#region Public Config Properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Center Handling")]
		[ProviderDescription(@"CenterHandling")]
		[PropertyOrder(1)]
		public CenterOptions CenterHandling
		{
			get { return _data.CenterHandling; }
			set
			{
				_data.CenterHandling = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Increment Angle")]
		[ProviderDescription(@"FanIncrementAngle")]
		[PropertyOrder(2)]
		public Curve IncrementAngle
		{
			get { return _data.IncrementAngle; }
			set
			{
				_data.IncrementAngle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
				
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Start Pan Angle")]
		[ProviderDescription(@"FanPanAngle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(5)]
		public int PanStartAngle
		{
			get { return _data.PanAngle; }
			set
			{
				_data.PanAngle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
						
		#endregion
						
		#region Protected Properties

		/// <inheritdoc/>
		protected override EffectTypeModuleData EffectModuleData => _data;

		#endregion

		#region Private Methods
		
		/*
		/// <summary>
		/// Gets the spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(BrickWidth / 100.0 * scaleValue);
		}

		private int GetBrickHeight(int scaleValue)
		{
			return (int)(BrickHeight / 100.0 * scaleValue);
		}

		private int GetBrickWidth(int scaleValue)
		{
			return (int)(BrickWidth / 100.0 * scaleValue);
		}

		private int GetMorterHeight(int scaleValue)
		{
			return (int)(MorterHeight / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(BrickHeight / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;	
		}

		/// <summary>
		/// Gets the horizontal spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetHorizontalWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(WeaveHorizontalSpacing / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the horizontal thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetHorizontalWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(WeaveHorizontalThickness / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;
		}

		/// <summary>
		/// Gets the vertical spacing in between the weave bars.
		/// </summary>    
		/// <param name="scaleValue">Value used to scale the spacing</param>
		/// <returns>Returns the spacing between the weave bars</returns>
		private int GetVerticalWeaveSpacing(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			return (int)(WeaveVerticalSpacing / 100.0 * scaleValue);
		}

		/// <summary>
		/// Gets the vertical thickness of the weave bars.
		/// </summary>
		/// <param name="scaleValue">Value used to scale the thickness</param>
		/// <returns></returns>
		private int GetVerticalWeaveThickness(int scaleValue)
		{
			// Calculate the spacing between the weave bars
			int thickness = (int)(WeaveVerticalThickness / 100.0 * scaleValue);

			// Ensure the thickness is always at least one pixel
			if (thickness == 0)
			{
				thickness = 1;
			}

			return thickness;
		}
		*/

		/// <summary>
		/// Initializes the visibility of the attributes.
		/// </summary>
		private void InitAllAttributes()
		{						
			//UpdateSizingAttributes(false);
			TypeDescriptor.Refresh(this);
		}
		
		/*
		private void UpdateSizingAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(6)
			{
				// Highlight percentage is only visible when Highlight is selected and we are in Location mode
				{ nameof(WeaveHorizontalSpacing), AdvancedSizing },
				{ nameof(WeaveVerticalSpacing), AdvancedSizing },
				{ nameof(WeaveHorizontalThickness), AdvancedSizing },
				{ nameof(WeaveVerticalThickness), AdvancedSizing },
				{ nameof(BrickWidth), !AdvancedSizing },
				{ nameof(BrickHeight), !AdvancedSizing },
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		*/
		
		/// <summary>
		/// Gets the highlight color for the weave color index.
		/// </summary>
		/// <returns>Highlight color for the specified color index</returns>
		private Color GetHighlightColor(int colorIndex, List<ColorGradient> colors)
		{
			// The color gradients run perpendicular to the bars so the highlight always
			// impacts the beginning of the gradient
			Color highlightColor = colors[colorIndex].GetColorAt(0);

			// Convert from RGB to HSV color format 
			HSV hsv = HSV.FromRGB(highlightColor);

			// Highlight the weave bar
			hsv.S = 0.0f;

			// Convert the HSB color back to RGB
			highlightColor = hsv.ToRGB();

			return highlightColor;
		}

		#endregion

		#region Protected Methods



		protected override void UpdateAttributes(bool refresh = true)
		{
			
		}

		protected override void UpdateFixtureCapabilities()
		{			
		}

		protected override void PreRenderInternal(CancellationTokenSource tokenSource = null)
		{						
			if (TargetNodes.Length == 1)
			{
				var renderNodes = GetNodesToRenderOn(TargetNodes.First());
				DoRendering(renderNodes, tokenSource);
			}
			else
			{
				DoRendering(TargetNodes.ToList(), tokenSource);
			}			
		}

		private int DepthOfEffect { get; set; }

		private List<IElementNode> GetNodesToRenderOn(IElementNode node)
		{
			IEnumerable<IElementNode> renderNodes = null;

			if (DepthOfEffect == 0)
			{
				renderNodes = node.GetLeafEnumerator().ToList();
			}
			else
			{
				renderNodes = new[] { node };
				for (int i = 0; i < DepthOfEffect; i++)
				{
					renderNodes = renderNodes.SelectMany(x => x.Children);
				}
			}

			// If the given DepthOfEffect results in no nodes (because it goes "too deep" and misses all nodes), 
			// then we'll default to the LeafElements, which will at least return 1 element (the TargetNode)
			if (!renderNodes.Any())
				renderNodes = node.GetLeafEnumerator();

			return renderNodes.ToList();
		}

		/// <summary>
		/// Retrieves the leaf nodes that support the specified function identity type.
		/// </summary>
		/// <param name="type">Function identity type to search for</param>
		/// <param name="tags">Dictionary of function tags to populate</param>
		/// <returns>Nodes that contain the specified function identity type along with the optional label associated with the function</returns>
		protected IEnumerable<Tuple<IElementNode, string>> GetRenderNodesForFunctionIdentity2(
			FunctionIdentity type, 
			Dictionary<IElementNode, string> tags, 
			IElementNode node)
		{
			// Create the return collection
			List<Tuple<IElementNode, string>> leavesThatSupportFunction = new List<Tuple<IElementNode, string>>();
			
			// If the node is NOT null then...
			if (node != null)
			{
				// Retrieve all leaf nodes that have an intelligent fixture property
				IEnumerable<IElementNode> leaves = GetLeafNodesWithIntelligentFixtureProperty(node);

				// Clear the function tags associated with the nodes
				tags.Clear();

				// Loop over the leaves
				foreach (IElementNode leafNode in leaves)
				{
					// Retrieve the fixture property associated with the node
					IntelligentFixtureModule fixtureProperty = (IntelligentFixtureModule)leafNode.Properties.Single(x => x is IntelligentFixtureModule);

					// If the fixture supports the specified function identity then...
					if (fixtureProperty.FixtureSpecification.SupportsFunction(type))
					{
						// Find the function associated with the effect based on function identity enumeration
						FixtureFunction func = fixtureProperty.FixtureSpecification.FunctionDefinitions.SingleOrDefault(function => function.FunctionIdentity == type);

						// Add the function name to the collection of tags
						tags.Add(leafNode, func.Name);

						// Add the leaf to the collection of nodes to return
						leavesThatSupportFunction.Add(new Tuple<IElementNode, string>(leafNode, func.Label));
					}
				}
			}

			return leavesThatSupportFunction;
		}

		/// <summary>
		/// Dictionary that keeps track of the pan function tags associated with the fixture.
		/// </summary>
		private Dictionary<IElementNode, string> _panTags;

		/// <summary>
		/// Creates fixture intents from the specified curve.
		/// This method also populates a dictionary of tags so that the intents are properly tagged.
		/// </summary>
		/// <param name="nodes">Nodes associated with the curve</param>
		/// <param name="curve">Curve to create intents from</param>
		/// <param name="functionType">Function associated with the intents</param>
		/// <param name="tag">Default tag to use when a dictionary of node tags is not provided</param>
		/// <param name="tags">Named tags associated with the function</param>
		/// <param name="cancellationToken">Whether or not the rendering has been cancelled</param>
		protected void RenderCurve2(
			IEnumerable<Tuple<IElementNode, string>> nodes,
			Curve curve, FunctionIdentity functionType,
			string tag,
			Dictionary<IElementNode, string> tags,
			CancellationTokenSource cancellationToken,
			double initialValue,
			double offset)
		{
			// If any nodes were found then...
			if (nodes.Any())
			{
				// Sort the points on the curve
				curve.Points.Sort();

				// Create a hashset of point values
				HashSet<double> points = new HashSet<double> { 0.0 };

				// Transfer the point to the hashset
				foreach (PointPair point in curve.Points)
				{
					points.Add(point.X);
				}
				points.Add(100.0);

				// Convert the hash set into a list
				List<double> pointList = points.ToList();

				// Initialize the start time
				TimeSpan startTime = TimeSpan.Zero;

				// Loop over the curve points
				for (int i = 1; i < points.Count; i++)
				{
					// Determine the time span between two points
					TimeSpan timeSpan = TimeSpan.FromMilliseconds(TimeSpan.TotalMilliseconds * ((pointList[i] - pointList[i - 1]) / 100));

					// Loop over the leaf nodes associated with the effect
					foreach (Tuple<IElementNode, string> node in nodes)
					{
						// Default the node tag 
						string nodeTag = tag;

						// If a dictionary of tags was specified then...
						if (tags != null)
						{
							// Retrieve the function tag from the dictionary based on the node being processed
							nodeTag = tags[node.Item1];
						}

						// Get the previous point
						RangeValue<FunctionIdentity> startValue = new RangeValue<FunctionIdentity>(functionType, nodeTag, curve.GetValue(pointList[i - 1]) / 100d, node.Item2);
						startValue.Value = initialValue + startValue.Value * offset;
						
						if (startValue.Value < 0.0)
						{
							startValue.Value = 0.0;
						}
						else if (startValue.Value > 1.0)
						{
							startValue.Value = 1.0;
						}

						// Get the current point
						RangeValue<FunctionIdentity> endValue = new RangeValue<FunctionIdentity>(functionType, nodeTag, curve.GetValue(pointList[i]) / 100d, node.Item2);
						endValue.Value = initialValue + endValue.Value * offset;

						if (endValue.Value < 0.0)
						{
							endValue.Value = 0.0;
						}
						else if (endValue.Value > 1.0)
						{
							endValue.Value = 1.0;
						}

						// Extrapolate an intent from the two points
						RangeIntent intent = new RangeIntent(startValue, endValue, timeSpan);
						
						// If the rendering has been cancelled then...
						if (cancellationToken != null && cancellationToken.IsCancellationRequested)
						{
							// Exit the function
							return;
						}

						// Add the intent to the output collection
						EffectIntentCollection.AddIntentForElement(node.Item1.Element.Id, intent, startTime);
					}

					// Move on to the next point
					startTime = startTime + timeSpan;
				}
			}
		}

		private void DoRendering(List<IElementNode> renderNodes, CancellationTokenSource tokenSource = null)
		{
			int count = renderNodes.Count;
			int middleIndex = count / 2 - 1;

			// TODO: Handle Center Moving Head

			Dictionary<IElementNode, string> tags = new Dictionary<IElementNode, string>();

			double offset = -2;
			for (int leftIndex = middleIndex; leftIndex >= 0; leftIndex --)
			{
				IEnumerable<Tuple<IElementNode, string>> nodes = GetRenderNodesForFunctionIdentity2(FunctionIdentity.Pan, tags, renderNodes[leftIndex]);
				RenderCurve2(nodes, IncrementAngle, FunctionIdentity.Pan, tags.First().Value, tags, tokenSource, PanStartAngle / 100.0, offset);				
				offset -= 1;	
			}

			offset = 2;
			for (int rightIndex = middleIndex + 1; rightIndex < count; rightIndex++)
			{
				IEnumerable<Tuple<IElementNode, string>> nodes = GetRenderNodesForFunctionIdentity2(FunctionIdentity.Pan, tags, renderNodes[rightIndex]);
				RenderCurve2(nodes, IncrementAngle, FunctionIdentity.Pan, tags.First().Value, tags, tokenSource, PanStartAngle / 100.0, offset);
				offset += 1;
			}

			// Render the tilt intents
			
		}

		#endregion
	}
}
