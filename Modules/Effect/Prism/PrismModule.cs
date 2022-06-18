using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Prism
{
	/// <summary>
	/// Intelligent fixture prism effect.
	/// </summary>
	public class PrismModule : FixtureIndexEffectBase<PrismData>
	{			
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PrismModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/prism/")
		{
			// Initialize the list of available index values
			IndexValues = new List<string>();
		}

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets the selected prism function name for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 2)]
		[ProviderDisplayName(@"Function")]
		[ProviderDescription(@"Function")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(PrismFunctionNameConverter))]
		[PropertyOrder(1)]
		public string PrismFunctionName
		{
			get => Data.PrismFunctionName;
			set
			{
				Data.PrismFunctionName = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the selected prism index value for the fixture(s).
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 2)]
		[ProviderDisplayName(@"PrismSetting")]
		[ProviderDescription(@"PrismSetting")]
		[TypeConverter(typeof(PrismFixtureIndexCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string PrismIndexValue
		{
			get => Data.PrismIndexValue;
			set
			{
				// Save off the prism index value
				Data.PrismIndexValue = value;

				// Update whether the index item supports a curve
				UpdateSupportsCurve(value, PrismFunctionName, FunctionIdentity.Prism);
				
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
		[ProviderCategory(@"Configuration", 2)]
		[ProviderDisplayName(@"PrismRotationSpeed")]
		[ProviderDescription(@"PrismRotationSpeed")]
		[PropertyOrder(3)]
		public Curve PrismCurve
		{
			get => Data.PrismCurve;
			set
			{
				Data.PrismCurve = value;
				IsDirty = true;
			}
		}

		#endregion
		
		#region Protected Methods

		/// <summary>
		/// Updates the visibility of prism attributes.
		/// </summary>
		protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ nameof(PrismIndexValue), SupportsIndexFunction},
				{ nameof(PrismCurve), IndexHasCurve},
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
			// Render the prism index values
			PreRenderInternal(
				PrismFunctionName,
				PrismIndexValue,
				PrismCurve,
				FunctionIdentity.Prism,
				cancellationToken);
		}
				
		/// <summary>
		/// Determines if the fixture supports a prism.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// Determine if the applicable nodes support a prism
			string prismFunctionName = PrismFunctionName;
			string prismIndexValue = PrismIndexValue;
			UpdateFixtureCapabilities(ref prismFunctionName, ref prismIndexValue, FunctionIdentity.Prism);
			PrismFunctionName = prismFunctionName;
			PrismIndexValue = prismIndexValue;			
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
			// If the selected prism index supports a curve then...
			if (IndexHasCurve)
			{
				// Render the curve on the timeline
				DrawVisualRepresentation(graphics, clipRectangle, Color.Yellow, "Prism", 2, 1, PrismCurve, 0);
			}
			else			
			{
				// Otherwise just indicate the effect is the prism effect
				DrawText(graphics, clipRectangle, Color.Yellow, "Prism", 0, clipRectangle.Y, clipRectangle.Height);
			}
		}

		#endregion		
	}
}