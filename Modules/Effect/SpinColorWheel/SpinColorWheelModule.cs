using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Intelligent fixture spin color wheel effect.
	/// </summary>
	public class SpinColorWheelModule : FixtureIndexEffectBase<SpinColorWheelData>
	{			
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SpinColorWheelModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/device-action-effects/intelligent-fixture/Spin-Color-Wheel/")
		{			
			// Create the dictionary of function tags
			_dimmerTags = new Dictionary<IElementNode, string>();
		}

		#endregion

		#region Private Fields

		private bool _canDim;

		/// <summary>
		/// Dictionary that keeps track of the dimmer function tags associated with the fixture.
		/// </summary>
		private Dictionary<IElementNode, string> _dimmerTags;

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets the selected spinning color wheel fixture function.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Function")]
		[ProviderDescription(@"Function")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(SpinColorWheelFunctionNameConverter))]
		[PropertyOrder(1)]
		public string SpinColorWheelFunctionName
		{
			get => Data.SpinColorWheelFunctionName;
			set
			{
				Data.SpinColorWheelFunctionName = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the selected spinning color wheel index value.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"SpinColorWheelSetting")]
		[ProviderDescription(@"SpinColorWheelSetting")]
		[TypeConverter(typeof(SpinColorWheelFixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string SpinColorWheelIndexValue
		{
			get => Data.SpinColorWheelIndexValue;
			set
			{
				// Save off the Spin Color Wheel index value
				Data.SpinColorWheelIndexValue = value;

				// Update whether the index item supports a curve
				UpdateSupportsCurve(value, SpinColorWheelFunctionName, FunctionIdentity.SpinColorWheel);
				
				// Update the effect property visibility status
				UpdateAttributes();

				// Mark the effect dirty
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the prism rotation curve.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"SpinColorWheelRotationSpeed")]
		[ProviderDescription(@"SpinColorWheelRotationRotationSpeed")]
		[PropertyOrder(3)]
		public Curve SpinColorWheelCurve
		{
			get => Data.SpinColorWheelCurve;
			set
			{
				Data.SpinColorWheelCurve = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the prism rotation curve.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Intensity")]
		[ProviderDescription(@"Intensity")]
		[PropertyOrder(4)]
		public Curve Intensity
		{
			get => Data.Intensity;
			set
			{
				Data.Intensity = value;
				IsDirty = true;
			}
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Updates the visibility of spinning color wheel attributes.
		/// </summary>
		protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{ nameof(SpinColorWheelIndexValue), SupportsIndexFunction},
				{ nameof(SpinColorWheelCurve), IndexHasCurve},
				{ nameof(Intensity), _canDim}, 
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override void PreRenderInternal(CancellationTokenSource cancellationToken = null)
		{
			// Render the effect
			PreRenderInternal(
				SpinColorWheelFunctionName,
				SpinColorWheelIndexValue,
				SpinColorWheelCurve,
				FunctionIdentity.SpinColorWheel,
				cancellationToken);
			
			// Render the dimmer intents
			RenderCurve(Intensity, FunctionIdentity.Dim, _dimmerTags, cancellationToken);			
		}
						
		/// <summary>
		/// Determines if the fixture supports the ability to spin a color wheel.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// Determine if the applicable nodes support spinning a color wheel
			string spinColorWheelFunctionName = SpinColorWheelFunctionName;
			string spinColorWheelIndexValue = SpinColorWheelIndexValue;
			UpdateFixtureCapabilities(ref spinColorWheelFunctionName, ref spinColorWheelIndexValue, FunctionIdentity.SpinColorWheel);			
			SpinColorWheelFunctionName = spinColorWheelFunctionName;	
			SpinColorWheelIndexValue = spinColorWheelIndexValue;
			
			// Determine if any of the nodes support the dimmer function
			_canDim = GetRenderNodesForFunctionIdentity(FunctionIdentity.Dim, _dimmerTags).Any();
		}

		/// <InheritDoc/>
		protected override List<FixtureIndexBase> GetCompatibleIndexValues(FixtureFunction function)
		{
			// Only include index values that support curves
			return function.GetIndexDataBase().Where(index => index.UseCurve).ToList();
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
			// Retrieve all leaf nodes that have an intelligent fixture property
			IEnumerable<IElementNode> leaves = GetLeafNodesWithIntelligentFixtureProperty(TargetNodes.First());

			// If there are any nodes that support fixtures then...
			if (leaves.Any())
			{
				// Retrieve the fixture property associated with the leaf node
				IntelligentFixtureModule fixtureProperty = GetIntelligentFixtureProperty(leaves.First());

				// If the color wheel function name is NOT empty then...
				if (!string.IsNullOrEmpty(SpinColorWheelFunctionName))
				{
					// Find the first function associated with the effect 
					FixtureFunction func = fixtureProperty.FixtureSpecification.GetInUseFunction(SpinColorWheelFunctionName, FunctionIdentity.SpinColorWheel);

					// Retrieve the colors associated with the color wheel
					List<Color> supportedColors = func.ColorWheelData.Where(entry => !entry.UseCurve && !entry.HalfStep).Select(colorItem => colorItem.Color1).Distinct().ToList();

					// If the selected color wheel index supports a curve then...
					if (IndexHasCurve)
					{
						// Render the curve on the timeline
						DrawVisualRepresentationWithBackground(graphics, clipRectangle, Color.Black, "Spin Color Wheel", 2, 1, SpinColorWheelCurve, 0, supportedColors);
					}
					else
					{
						// Otherwise just indicate the effect is the spin color wheel effect
						DrawText(graphics, clipRectangle, Color.HotPink, "Spin Color Wheel", 0, clipRectangle.Y, clipRectangle.Height);
					}
				}
			}
		}

		#endregion		
	}
}