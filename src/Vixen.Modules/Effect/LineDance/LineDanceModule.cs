using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Sys.Attribute;

using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.IntelligentFixture;

using ZedGraph;

using Color = System.Drawing.Color;

namespace VixenModules.Effect.LineDance
{
	/// <summary>
	/// Creates an effect that make a collection of moving heads dance.
	/// </summary>
	public class LineDanceModule : FixtureEffectBase<LineDanceData>
	{
		#region Fields

		/// <summary>
		/// Flag which determines if the node(s) associated with the effect support panning.
		/// </summary>
		private bool _canPan;

		/// <summary>
		/// Dictionary that keeps track of the pan function tags associated with the fixture.
		/// </summary>
		private IDictionary<IElementNode, (string label, string tag)> _panFunctions;

		/// <summary>
		/// Collection of nodes to render the fan on.
		/// </summary>
		private List<IElementNode> _renderNodes;
		
		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LineDanceModule() : 
			base("https://www.vixenlights.com/vixen-3-documentation/docs/usage/sequencer/effects/intelligent-fixture/line-dance/")
		{			
			// Create the dictionary of node to function information
			_panFunctions = new Dictionary<IElementNode, (string label, string tag)>();		
		}

		#endregion

		#region Public Config Properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceMode")]
		[ProviderDescription(@"LineDanceMode")]
		[PropertyOrder(1)]
		public LineDanceModes Mode
		{
			get { return Data.Mode; }
			set
			{
				Data.Mode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanMode")]
		[ProviderDescription(@"LineDanceFanMode")]
		[PropertyOrder(2)]
		public FanModes FanMode
		{
			get { return Data.FanMode; }
			set
			{
				Data.FanMode = value;
				IsDirty = true;
				OnPropertyChanged();

				UpdateAttributes();	
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanPanIncrement")]
		[ProviderDescription(@"LineDanceFanPanIncrement")]
		[PropertyOrder(3)]
		public Curve IncrementAngle
		{
			get { return Data.IncrementAngle; }
			set
			{
				Data.IncrementAngle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanPanIncrement")]
		[ProviderDescription(@"LineDanceFanPanIncrement")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(4)]
		public int PanIncrement
		{
			get { return Data.PanIncrement; }
			set
			{
				Data.PanIncrement = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanInvertPan")]
		[ProviderDescription(@"LineDanceFanInvertPan")]
		[PropertyOrder(5)]
		public bool InvertPan
		{
			get { return Data.InvertPan; }
			set
			{
				Data.InvertPan = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanCenterHandling")]
		[ProviderDescription(@"LineDanceFanCenterHandling")]
		[PropertyOrder(6)]
		public FanCenterOptions CenterHandling
		{
			get { return Data.CenterHandling; }
			set
			{
				Data.CenterHandling = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanAdvancedOverrides")]
		[ProviderDescription(@"LineDanceFanAdvancedOverrides")]
		[PropertyOrder(7)]
		public bool AdvancedOverrides
		{
			get { return Data.AdvancedOverrides; }
			set
			{
				Data.AdvancedOverrides = value;
				IsDirty = true;
				OnPropertyChanged();
				UpdateAttributes();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"LineDanceFanPanStart")]
		[ProviderDescription(@"LineDanceFanPanStart")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(8)]
		public int PanStartAngle
		{
			get { return Data.PanStartAngle; }
			set
			{
				Data.PanStartAngle = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		#endregion

		#region Private Methods

		/// <summary>
		/// Limits the specified angle to 0 - 1.
		/// </summary>
		/// <param name="angle">Angle to limit</param>
		/// <returns>Limited angle to a range of 0 - 1</returns>
		private double LimitAngle(double angle)
		{
			// If the angle has gone negative then...
			if (angle < 0.0)
			{
				// Reset the angle to the minimum
				angle = 0.0;
			}
			// If the angle is greater than one then...
			else if (angle > 1.0)
			{
				// Reset the angle to the maximum
				angle = 1.0;
			}

			return angle;
		}

		/// <summary>
		/// Creates fan pan intents from the specified curve.		
		/// </summary>
		/// <param name="node">Node associated with the curve</param>
		/// <param name="curve">Curve to create intents from</param>
		/// <param name="functionType">Function associated with the intents</param>
		/// <param name="functionNameTag">Tag associated with the fixture function</param>
		/// <param name="functionLabel">Label associated with the fixture function</param>
		/// <param name="cancellationToken">Whether or not the rendering has been cancelled</param>
		/// <param name="initialValue">Initial angle to pan the moving head</param>
		/// <param name="offset">Moving head offset multiplier</param>
		private void RenderFanCurve(
			IElementNode node,
			Curve curve, 
			FunctionIdentity functionType,
			string functionNameTag,			
			string functionLabel,
			CancellationTokenSource cancellationToken,
			double maxIncrementPan,
			double initialValue,
			double offset)			
		{
			// If a node was specified then...
			if (node != null)
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
										
					// The middle of the curve represents zero
					const double Zero = 0.5;
											
					// Get the previous point
					RangeValue<FunctionIdentity> startValue = new RangeValue<FunctionIdentity>(functionType, functionNameTag, curve.GetValue(pointList[i - 1]) / 100.0, functionLabel);
					startValue.Value = initialValue + (((startValue.Value - Zero) / Zero) * maxIncrementPan) * offset;

					// Limit the angle to 0 - 1
					startValue.Value = LimitAngle(startValue.Value);
						
					// Get the current point
					RangeValue<FunctionIdentity> endValue = new RangeValue<FunctionIdentity>(functionType, functionNameTag, curve.GetValue(pointList[i]) / 100.0, functionLabel);
					endValue.Value = initialValue + (((endValue.Value - Zero) / Zero) * maxIncrementPan) * offset;

					// Limit the angle to 0 - 1
					endValue.Value = LimitAngle(endValue.Value);

					// Extrapolate an intent from the two points
					RangeIntent intent = new RangeIntent(startValue, endValue, timeSpan);

					// If the rendering has been cancelled then...
					if (cancellationToken != null && cancellationToken.IsCancellationRequested)
					{
						// Exit the function
						return;
					}

					// Add the intent to the output collection
					EffectIntentCollection.AddIntentForElement(node.Element.Id, intent, startTime);

					// Move on to the next point
					startTime = startTime + timeSpan;
				}
			}
		}

		/// <summary>
		/// Render the left nodes for a staggered fan.
		/// </summary>
		/// <param name="leftMiddleIndex">Start index of the left side of the fan</param>
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderStaggerLeftNodes(
			int leftMiddleIndex, 
			List<IElementNode> renderNodes, 
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			int offset = 1;
						
			// Loop over the left fan nodes
			for (int leftIndex = leftMiddleIndex; leftIndex >= 0; leftIndex--)
			{
				// Calculate the distance between stagger points
				// Only using 95% of the effect's duration so that there is slight pause at the end of the effect
				int width = 95 / (renderNodes.Count / 2);

				// Create a curve for the node
				Curve c = CreateStaggerCurve(PanIncrement, width * offset, width, leftIndex == 0);
				
				// Render the node
				RenderFanCurve(
					renderNodes[leftIndex],
					c,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[leftIndex]].tag,
					_panFunctions[renderNodes[leftIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);
				
				// Increment the pan offset
				offset += 1;				
			}
		}

		/// <summary>
		/// Render the left nodes of a concurrent fan.
		/// </summary>
		/// <param name="leftMiddleIndex">Start index of the left side of the fan</param>
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderConcurrentLeftNodes(
			int leftMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			int offset = 1;
			
			// Loop over the left fan nodes
			for (int leftIndex = leftMiddleIndex; leftIndex >= 0; leftIndex--)
			{
				// Create a flat curve at the pan increment
				Curve c = CreateConcurrentFlatCurve(PanIncrement);

				// Render the node
				RenderFanCurve(
					renderNodes[leftIndex],
					c,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[leftIndex]].tag,
					_panFunctions[renderNodes[leftIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);

				// Increment the pan offset
				offset += 1;
			}
		}

		/// <summary>
		/// Render the left nodes of the synchronized fan.
		/// </summary>
		/// <param name="leftMiddleIndex">Start index of the left side of the fan</param>
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderSynchronizedLeftNodes(
			int leftMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			double offset = 1;
			
			// Loop over the left fan nodes
			for (int leftIndex = leftMiddleIndex; leftIndex >= 0; leftIndex--)
			{				
				// Render the node
				RenderFanCurve(
					renderNodes[leftIndex],
					IncrementAngle,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[leftIndex]].tag,
					_panFunctions[renderNodes[leftIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);
				
				// Increment the pan offset
				offset += 1;				
			}
		}

		/// <summary>
		/// Creates a curve for staggered fan node.
		/// </summary>
		/// <param name="panIncrement">Pan angle increment of the node pairs</param>
		/// <param name="xPosition">Position within the effect's duration that the node should start to pan</param>
		/// <param name="width">Stagger distance between node points to start their pan</param>
		/// <param name="last">True when this is the last node in the fan</param>
		/// <returns>Curve for staggered fan node</returns>
		private Curve CreateStaggerCurve(int panIncrement, int xPosition, int width, bool last)
		{
			// Define the middle of the curve (zero)
			const double Middle = 50.0;

			// Define the top full pan
			double Top = Middle + panIncrement;
			
			// If this is the first node in the stagger fan then...
			if (xPosition == width)
			{
				// Define 3 point curve
				return new Curve(new PointPairList(new[] { 0.0, width, 100.0 }, new[] { Middle, Top, Top }));
			}
			// Otherwise if this is last node in the stagger fan then...
			else if (last)
			{
				// Define 4 point curve allowing for small (5%) pause at end of effect
				return new Curve(new PointPairList(new[] { 0.0, xPosition - width, 95, 100.0 }, new[] { Middle, Middle, Top, Top }));
			}
			else
			{
				// Define 4 point curve with pan transition in the middle
				return new Curve(new PointPairList(new[] { 0.0, xPosition - width, xPosition, 100.0 }, new[] { Middle, Middle, Top, Top }));
			}
		}

		/// <summary>
		/// Creates a flat curve for concurrent fan.
		/// </summary>
		/// <param name="panIncrement">Pan angle increment</param>
		/// <returns>Flat curve</returns>
		private Curve CreateConcurrentFlatCurve(int panIncrement)
		{
			// Define the middle of the curve (zero)
			const double Middle = 50.0;

			// Define the top full pan
			double Top = Middle + panIncrement;

			return new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { Top, Top }));			
		}

		/// <summary>
		/// Render the right nodes of a synchronized fan.
		/// </summary>
		/// <param name="rightMiddleIndex">Start index of the right side of the fan</param>		
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderSynchronizedRightNodes(
			int rightMiddleIndex, 
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			double offset = -1;

			// Loop over the right fan nodes
			for (int rightIndex = rightMiddleIndex; rightIndex < renderNodes.Count; rightIndex++)
			{				
				// Render the node
				RenderFanCurve(
					renderNodes[rightIndex],
					IncrementAngle,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[rightIndex]].tag,
					_panFunctions[renderNodes[rightIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);

				// Increment the pan offset
				offset -= 1;
			}
		}

		/// <summary>
		/// Render the right nodes of the stagger fan.
		/// </summary>
		/// <param name="rightMiddleIndex">Start index of the right side of the fan</param>		
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderStaggerRightNodes(
			int rightMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			double offset = -1;
			
			// Loop over the right fan nodes
			for (int rightIndex = rightMiddleIndex; rightIndex < renderNodes.Count; rightIndex++)
			{
				// Calculate the distance between stagger points
				// Only using 95% of the effect's duration so that there is slight pause at the end of the effect
				int width = 95 / (renderNodes.Count / 2);

				// Create a curve for the node
				Curve c = CreateStaggerCurve(PanIncrement, width * (int)(Math.Abs(offset)), width, rightIndex == renderNodes.Count - 1);

				// Render the node
				RenderFanCurve(
					renderNodes[rightIndex],
					c,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[rightIndex]].tag,
					_panFunctions[renderNodes[rightIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);

				// Increment the pan offset
				offset -= 1;				
			}
		}

		/// <summary>
		/// Render the right nodes of the fan.
		/// </summary>
		/// <param name="rightMiddleIndex">Start index of the right side of the fan</param>		
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderConcurrentRightNodes(
			int rightMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Initialize the pan offset
			double offset = -1;
			
			// Loop over the right fan nodes
			for (int rightIndex = rightMiddleIndex; rightIndex < renderNodes.Count; rightIndex++)
			{
				// Create a flat curve at the pan increment
				Curve c = CreateConcurrentFlatCurve(PanIncrement);

				// Render the node
				RenderFanCurve(
					renderNodes[rightIndex],
					c,
					FunctionIdentity.Pan,
					_panFunctions[renderNodes[rightIndex]].tag,
					_panFunctions[renderNodes[rightIndex]].label,
					tokenSource,
					maxIncrementPan,
					panStart,
					InvertPan ? -offset : offset);
								
				// Increment the pan offset
				offset -= 1;
			}
		}

		/// <summary>
		/// Render the center node of the fan.
		/// </summary>
		/// <param name="centerIndex">Start index of the right side of the fan</param>		
		/// <param name="renderNodes">All the render nodes of the fan</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void RenderCenterNode(
			int centerIndex, 
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			CancellationTokenSource tokenSource = null)
		{
			// Render the center node
			RenderFanCurve(
				renderNodes[centerIndex],
				IncrementAngle,
				FunctionIdentity.Pan,
				_panFunctions[renderNodes[centerIndex]].tag,
				_panFunctions[renderNodes[centerIndex]].label,
				tokenSource,
				maxIncrementPan,
				panStart,
				0.0);
		}

		/// <summary>
		/// Renders the fan on the specified nodes.
		/// </summary>
		/// <param name="renderNodes">Nodes to render on</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		/// <param name="tokenSource">Cancellation token</param>
		private void DoRendering(
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart,
			Action<bool, int, int, int, List<IElementNode>, double, double> renderAction,
			CancellationTokenSource tokenSource = null)
		{
			// Determine the number of render nodes to render on
			int count = renderNodes.Count;

			// Determine the index of the node to render the left half of the fan
			int leftMiddleIndex = count / 2 - 1;

			// Determine the index of the node to render the right half of the fan
			int rightMiddleIndex = leftMiddleIndex + 1;

			// Determine the index of the center node
			int centerIndex = count / 2;

			// Default to not displaying a center beam
			bool displayCenter = false;

			// If the number of nodes is odd then then we need to dispay a center beam
			if (count % 2 == 1)
			{			
				// If the beam is to displayed to the left then...
				if (CenterHandling == FanCenterOptions.Left)
				{
					// Increment the left index so the center node is included on the left side logic
					leftMiddleIndex += 1;

					// Increment the right side to exclude this center node
					rightMiddleIndex += 1;
				}	
				// Otherwise if the beam is to be displayed in the center
				else if (CenterHandling == FanCenterOptions.Centered)									
				{
					// Increment the right side to exclude this center node
					rightMiddleIndex += 1;

					// Set a flag to indicate a center beam needs to be rendered
					displayCenter = true;
				}				
			}

			// Render the fan
			renderAction(
				displayCenter,
				leftMiddleIndex,
				centerIndex,
				rightMiddleIndex,
				renderNodes,
				maxIncrementPan,
				panStart);
		}

		/// <summary>
		/// Renders the synchronized fan on the specified nodes.
		/// </summary>
		/// <param name="displayCenter">True when a center node is displayed</param>
		/// <param name="leftMiddleIndex">Index of the left middle node</param>
		/// <param name="centerIndex">Index of the center node</param>
		/// <param name="rightMiddleIndex">Index of the right middle node</param>
		/// <param name="renderNodes">Nodes to render on</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		private void RenderSynchronizedFan(
			bool displayCenter, 
			int leftMiddleIndex, 
			int centerIndex, 
			int rightMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart)
		{
			// Render the left nodes
			RenderSynchronizedLeftNodes(leftMiddleIndex, renderNodes, maxIncrementPan, panStart);

			// If displaying a center beam then...
			if (displayCenter)
			{
				// Render the center node
				RenderCenterNode(centerIndex, renderNodes, maxIncrementPan, panStart);
			}

			// Render the right nodes
			RenderSynchronizedRightNodes(rightMiddleIndex, renderNodes, maxIncrementPan, panStart);
		}

		/// <summary>
		/// Renders the stagger fan on the specified nodes.
		/// </summary>
		/// <param name="displayCenter">True when a center node is displayed</param>
		/// <param name="leftMiddleIndex">Index of the left middle node</param>
		/// <param name="centerIndex">Index of the center node</param>
		/// <param name="rightMiddleIndex">Index of the right middle node</param>
		/// <param name="renderNodes">Nodes to render on</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		private void RenderStaggerFan(
			bool displayCenter, 
			int leftMiddleIndex, 
			int centerIndex, 
			int rightMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart)
		{
			// Render the left nodes
			RenderStaggerLeftNodes(leftMiddleIndex, renderNodes, maxIncrementPan, panStart);

			// If displaying a center beam then...
			if (displayCenter)
			{
				// Render the center node
				RenderCenterNode(centerIndex, renderNodes, maxIncrementPan, panStart);
			}

			// Render the right nodes
			RenderStaggerRightNodes(rightMiddleIndex, renderNodes, maxIncrementPan, panStart);
		}

		/// <summary>
		/// Renders the concurrent fan on the specified nodes.
		/// </summary>
		/// <param name="displayCenter">True when a center node is displayed</param>
		/// <param name="leftMiddleIndex">Index of the left middle node</param>
		/// <param name="centerIndex">Index of the center node</param>
		/// <param name="rightMiddleIndex">Index of the right middle node</param>
		/// <param name="renderNodes">Nodes to render on</param>
		/// <param name="maxIncrementPan">Maximum increment pan angle (0-1) to scale the curve</param>
		/// <param name="panStart">Pan start angle (0-1)</param>
		private void RenderConcurrentFan(
			bool displayCenter,
			int leftMiddleIndex,
			int centerIndex,
			int rightMiddleIndex,
			List<IElementNode> renderNodes,
			double maxIncrementPan,
			double panStart)
		{
			// Render the left nodes
			RenderConcurrentLeftNodes(leftMiddleIndex, renderNodes, maxIncrementPan, panStart);

			// If displaying a center beam then...
			if (displayCenter)
			{
				// Render the center node
				RenderCenterNode(centerIndex, renderNodes, maxIncrementPan, panStart);
			}

			// Render the right nodes
			RenderConcurrentRightNodes(rightMiddleIndex, renderNodes, maxIncrementPan, panStart);
		}

		/// <summary>
		/// Calculate the start angle (0-1) of the moving heads.  The angle calculated is 360 degrees.		
		/// </summary>
		/// <returns>Calculate the start angle of the moving heads</returns>
		private double CalculateStartAngle()
		{
			// Retrieve the intelligent fixture associated with the node
			IntelligentFixtureModule fixtureProperty = GetIntelligentFixtureProperty(_renderNodes[0]);

			// Retrieve the pan function from the fixture
			FixtureFunction panFunction = fixtureProperty.FixtureSpecification.FunctionDefinitions.FirstOrDefault(fn => fn.FunctionIdentity == Vixen.Data.Value.FunctionIdentity.Pan);

			// Retrieve the pan limits
			double panStart = panFunction.RotationLimits.StartPosition;
			double panStop = panFunction.RotationLimits.StopPosition;

			// Determine total pan range of the fixture
			double totalDegrees = (panStop - panStart);

			double startAngle = 0.0;

			// If the fixtures can pan over 360 degrees then...
			if (totalDegrees > 360)
			{
				// Determine the value to pan 360 degrees
				startAngle = 360.0 / totalDegrees;
			}
			
			return startAngle;
		}

		/// <summary>
		/// Calculates the maximum pan increment angle (0-1).  This value is used to scale the Pan Increment angle.
		/// The calculation results in the last moving head in the fan to be at the maximum pan of the fixture.
		/// </summary>
		/// <returns>Maximum pan increment (0-1)</returns>
		private double CalculateMaxPanIncrement()
		{
			// Calculate the maximum pan increment with the goal of the last moving head to be at the maximum pan angle
			return (1.0 - PanStartAngle / 100.0) / (_renderNodes.Count / 2);			
		}

		#endregion

		#region Protected Methods

		/// <inheritdoc/>		
		protected override void UpdateAttributes(bool refresh = true)
		{
			// If the render nodes have been determined then...
			if (_renderNodes != null)
			{ 
				Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4);

				// Display Pan Curve for Synchronized Fan Mode
				propertyStates.Add(nameof(IncrementAngle), FanMode == FanModes.Synchronized);

				// Display Pan Slider for Stagger and Concurrent Fan Mode
				propertyStates.Add(nameof(PanIncrement), FanMode != FanModes.Synchronized);

				// Display the Center Handling if there are an odd number of nodes
				propertyStates.Add(nameof(CenterHandling), (_renderNodes.Count % 2 == 1));

				// Only display Pan Start Angle if the user selected to see Advanced Overrides
				propertyStates.Add(nameof(PanStartAngle), AdvancedOverrides);

				SetBrowsable(propertyStates);
				if (refresh)
				{
					TypeDescriptor.Refresh(this);
				}
			}
		}

		/// <inheritdoc/>		
		protected override void UpdateFixtureCapabilities()
		{
			// Clear the dictionary of fixture functions
			_panFunctions.Clear();

			// Determine if any of the nodes can pan
			_renderNodes = GetRenderNodesForFunctionIdentity(FunctionIdentity.Pan, _panFunctions);

			// If there are any nodes that support the pan function
			_canPan = _renderNodes.Any();
		}

		/// <inheritdoc/>		
		protected override void PreRenderInternal(CancellationTokenSource tokenSource = null)
		{
			// Only need to render if the effect has been targeted on at least one node that can pan
			if (_canPan)
			{
				// Determine what type of fan to render
				switch (FanMode)
				{
					case FanModes.Synchronized:
						// Render on the specified nodes
						DoRendering(_renderNodes, CalculateMaxPanIncrement(), PanStartAngle / 100.0, RenderSynchronizedFan, tokenSource);
						break;
					case FanModes.Stagger:
						DoRendering(_renderNodes, CalculateMaxPanIncrement(), PanStartAngle / 100.0, RenderStaggerFan, tokenSource);
						break;
					case FanModes.Concurrent:
						DoRendering(_renderNodes, CalculateMaxPanIncrement(), PanStartAngle / 100.0, RenderConcurrentFan, tokenSource);
						break;
					default:
						Debug.Assert(false, "Unsupported Fan Mode!");
						break;
				}				
			}
		}
		
		/// <inheritdoc/>		
		protected override void TargetNodesChanged()
		{
			// Call the base class implementation
			base.TargetNodesChanged();

			// Determine if any target nodes support the pan function
			UpdateFixtureCapabilities();
			
			// If there are nodes to render on then...
			if (_canPan)
			{			
				// If the Pan Start angle has not been initialized then...
				if (PanStartAngle == -1)
				{					
					// Calculate the pan start angle (0-100)
					PanStartAngle = (int)(CalculateStartAngle() * 100);					
				}			
			}
		}
				
		#endregion
				
		#region Public Overrides of BaseEffect		

		/// <summary>
		/// Generates the visual representation of the effect on the timeline.
		/// </summary>
		/// <param name="graphics">Graphics context</param>
		/// <param name="clipRectangle">Clipping rectangle of the effect on the timeline</param>
		public override void GenerateVisualRepresentation(Graphics graphics, Rectangle clipRectangle)
		{
			// If the associated nodes support pan then...
			if (_canPan)
			{
				// Draw the visual on the timeline
				DrawVisualRepresentation(graphics, clipRectangle, Color.Green, "Fan", 2, 1, IncrementAngle, 0);
			}
		}

		#endregion				
	}
}
