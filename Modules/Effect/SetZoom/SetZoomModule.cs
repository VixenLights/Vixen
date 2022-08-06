using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.SetZoom
{
    /// <summary>
    /// Intelligent fixture zoom effect.
    /// </summary>
    public class SetZoomModule : FixtureEffectBase<SetZoomData>
	{
        #region Fields 

		/// <summary>
		/// Flag which determines if the node(s) associated with the effect support zooming.
		/// </summary>
        private bool _canZoom;

		/// <summary>
		/// Dictionary that keeps track of the zoom function tags associated with the fixture.
		/// </summary>
		private Dictionary<IElementNode, string> _zoomTags;

        #endregion

        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public SetZoomModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/set-zoom/")
		{
			// Create the dictionary of zoom function tag names indexed by element node
			_zoomTags = new Dictionary<IElementNode, string>();
		}

        #endregion

        #region Public Effect Properties

		/// <summary>
		/// Gets or sets the zoom curve for the fixture.
		/// </summary>
        [Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Zoom")]
		[ProviderDescription(@"Zoom")]
		public Curve Zoom
		{
			get => Data.Zoom;
			set
			{
				Data.Zoom = value;
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
				{ nameof(Zoom), _canZoom},
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
			// Render the zoom intents
			RenderCurve(Zoom, FunctionIdentity.Zoom, _zoomTags, cancellationToken);
		}

		/// <summary>
		/// Determines if the fixture supports the ability to zoom.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{
			// Determine if any of the nodes support the zoom function
			_canZoom = GetRenderNodesForFunctionIdentity(FunctionIdentity.Zoom, _zoomTags).Any();					
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
			// If the associated nodes support zooming then...
			if (_canZoom)
			{								
				// Draw the visual on the timeline
				DrawVisualRepresentation(graphics, clipRectangle, Color.Red, "Zoom", 2, 1, Zoom, 0);
			}						
		}

		#endregion
	}
}