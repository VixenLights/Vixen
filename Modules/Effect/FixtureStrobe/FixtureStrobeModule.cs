using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Intelligent fixture strobe effect.
	/// </summary>
	public class FixtureStrobeModule : FixtureIndexEffectBase<FixtureStrobeData>
	{			
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public FixtureStrobeModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/fixture-strobe/")
		{
			// Initialize the list of available index values
			IndexValues = new List<string>();
		}

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets the selected strobe function name for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Function")]
		[ProviderDescription(@"Function")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(FixtureStrobeFunctionNameConverter))]
		[PropertyOrder(1)]
		public string FixtureStrobeFunctionName
		{
			get => Data.FixtureStrobeFunctionName;
			set
			{
				Data.FixtureStrobeFunctionName = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the selected strobe index value for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"FixtureStrobeSetting")]
		[ProviderDescription(@"FixtureStrobeSetting")]
		[TypeConverter(typeof(FixtureStrobeFixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string FixtureStrobeIndexValue
		{
			get => Data.FixtureStrobeIndexValue;
			set
			{
				// Save off the strobe index value
				Data.FixtureStrobeIndexValue = value;

				// Update whether the index item supports a curve
				UpdateSupportsCurve(value, FixtureStrobeFunctionName, FunctionIdentity.Shutter);
				
				// Update the effect property visibility status
				UpdateAttributes();

				// Mark the effect dirty
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the strobe curve.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"FixtureStrobeSpeed")]
		[ProviderDescription(@"FixtureStrobeSpeed")]
		[PropertyOrder(3)]
		public Curve FixtureStrobeCurve
		{
			get => Data.FixtureStrobeCurve;
			set
			{
				Data.FixtureStrobeCurve = value;
				IsDirty = true;
			}
		}

		#endregion
		
		#region Protected Methods

		/// <summary>
		/// Updates the visibility of strobe attributes.
		/// </summary>
		protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ nameof(FixtureStrobeIndexValue), SupportsIndexFunction},
				{ nameof(FixtureStrobeCurve), IndexHasCurve},
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
			// Render the shutter curve
			PreRenderInternal(
				FixtureStrobeFunctionName,
				FixtureStrobeIndexValue,
				FixtureStrobeCurve,
				FunctionIdentity.Shutter,
				cancellationToken);
		}
				
		/// <summary>
		/// Determines if the fixture supports strobing.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// Determine if the applicable nodes support strobing
			string functionName = FixtureStrobeFunctionName;
			string indexValue = FixtureStrobeIndexValue;
			UpdateFixtureCapabilities(ref functionName, ref indexValue, FunctionIdentity.Shutter);
			FixtureStrobeFunctionName = functionName;
			FixtureStrobeIndexValue = indexValue;			
		}
		
		/// <inheritdoc/>
		protected override List<FixtureIndexBase> GetCompatibleIndexValues(FixtureFunction function)
		{
			// Only return index values that are marked as strobe items
			return function.GetIndexDataBase().Where(index => index.IndexType == Vixen.Commands.FixtureIndexType.Strobe).ToList();
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
			// If the selected strobe index supports a curve then...
			if (IndexHasCurve)
			{
				// Render the curve on the timeline
				DrawVisualRepresentation(graphics, clipRectangle, Color.Yellow, "Strobe", 2, 1, FixtureStrobeCurve, 0);
			}
			else			
			{
				// Otherwise just indicate the effect is the strobe effect
				DrawText(graphics, clipRectangle, Color.Yellow, "Strobe", 0, clipRectangle.Y, clipRectangle.Height);
			}
		}

		#endregion		
	}
}