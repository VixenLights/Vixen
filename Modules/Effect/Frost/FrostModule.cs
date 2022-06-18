using System;
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
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Frost
{
    /// <summary>
    /// Intelligent fixture frost effect.
    /// </summary>
    public class FrostModule : FixtureEffectBase<FrostData>
	{
		#region Fields 

		/// <summary>
		/// Flag which determines if the node(s) associated with the effect support a frost lense.
		/// </summary>
		private bool _canFrost;
		
        #endregion

        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public FrostModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/frost/")
		{			
		}

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets the selected frost function name.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Function")]
		[ProviderDescription(@"Function")]
		[PropertyEditor("SelectionEditor")]
		[TypeConverter(typeof(FrostFunctionNameConverter))]
		[PropertyOrder(1)]
		public string FrostFunctionName
		{
			get => Data.FrostFunctionName;
			set
			{
				Data.FrostFunctionName = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets the frost curve for the fixture.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FrostPercentage")]
		[ProviderDescription(@"FrostPercentage")]
		[PropertyOrder(2)]
		public Curve Frost
		{
			get => Data.Frost;
			set
			{
				Data.Frost = value;
				IsDirty = true;
			}
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the visibility of Zoom attributes.
        /// </summary>
        protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{				
				{ nameof(Frost), _canFrost},
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
			// Render the Frost intents
			RenderCurve(Frost, FunctionIdentity.Frost, FrostFunctionName, cancellationToken);			
		}

		/// <summary>
		/// Determines if the fixture supports a frost lense.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// If there are element nodes associated with the effect
			if (TargetNodes.Any())
			{				
				// If the function name is NOT empty and
				// function is no longer associated with the node then...
				if (!string.IsNullOrEmpty(FrostFunctionName) &&
					!GetMatchingFunctionNames(FunctionIdentity.Frost).Contains(FrostFunctionName))
				{
					// Clear out the selected index function name
					FrostFunctionName = String.Empty;
				}

				// If the frost function name is empty and		
				// there are functions that support frost then...
				if (string.IsNullOrEmpty(FrostFunctionName) &&
					GetMatchingFunctionNames(FunctionIdentity.Frost).Any())
				{
					// Initialize the selected function to the first function
					FrostFunctionName = GetMatchingFunctionNames(FunctionIdentity.Frost).First();
				}

				// Determine if any of the nodes support the frost function
				_canFrost = GetRenderNodesForFunctionName(FrostFunctionName, FunctionIdentity.Frost).Any();
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
			// If the associated nodes support a frost lense then...
			if (_canFrost)
			{								
				// Draw the visual on the timeline
				DrawVisualRepresentation(graphics, clipRectangle, Color.LightSkyBlue, "Frost", 2, 1, Frost, 0);
			}						
		}

		#endregion
	}
}