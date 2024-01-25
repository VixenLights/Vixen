using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.SetPosition
{
	/// <summary>
	/// Intelligent fixture pan and tilt effect.
	/// </summary>
	public class SetPositionModule : FixtureEffectBase<SetPositionData>
	{
		#region Fields

		/// <summary>
		/// Flag which determines if the node(s) associated with the effect support tilting.
		/// </summary>
		private bool _canTilt;

		/// <summary>
		/// Flag which determines if the node(s) associated with the effect support panning.
		/// </summary>
		private bool _canPan;

		/// <summary>
		/// Dictionary that keeps track of the tilt function tags associated with the fixture.
		/// </summary>
		private Dictionary<IElementNode, string> _tiltTags;

		/// <summary>
		/// Dictionary that keeps track of the pan function tags associated with the fixture.
		/// </summary>
		private Dictionary<IElementNode, string> _panTags;

        #endregion

        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public SetPositionModule() :
			// Give the base class the online help URL
			base("http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/intelligent-fixture/set-position/")
		{
			// Create the dictionary of tilt function tag names indexed by element node
			_tiltTags = new Dictionary<IElementNode, string>();

			// Create the dictionary of pan function tag names indexed by element node
			_panTags = new Dictionary<IElementNode, string>();
		}

		#endregion

		#region Public Effect Properties

		/// <summary>
		/// Gets or sets whether this effect controls the pan of the fixture.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"SetPositionEnablePan")]
		[ProviderDescription(@"SetPositionEnablePan")]
		[PropertyOrder(1)]
		public bool EnablePan
		{
			get => Data.EnablePan;
			set
			{
				Data.EnablePan = value;
				IsDirty = true;

				UpdateAttributes(true);
			}
		}

		/// <summary>
		/// Gets or sets the pan of the fixture.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Pan")]
		[ProviderDescription(@"Pan")]
		[PropertyOrder(2)]
		public Curve Pan
		{
			get => Data.Pan;
			set
			{
				Data.Pan = value;
				IsDirty = true;
			}
		}

		/// <summary>
		/// Gets or sets whether this effect controls the tilt of the fixture.
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"SetPositionEnableTilt")]
		[ProviderDescription(@"SetPositionEnableTilt")]
		[PropertyOrder(3)]
		public bool EnableTilt
		{
			get => Data.EnableTilt;
			set
			{
				Data.EnableTilt = value;
				IsDirty = true;

				UpdateAttributes(true);
			}
		}

		/// <summary>
		/// Gets or sets the tilt of the fixture. 
		/// </summary>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Tilt")]
		[ProviderDescription(@"Tilt")]
		[PropertyOrder(4)]
		public Curve Tilt
		{
			get => Data.Tilt;
			set
			{
				Data.Tilt = value;
				IsDirty = true;
			}
		}

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates the visibility of Pan / Tilt attributes.
        /// </summary>
        protected override void UpdateAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ nameof(Pan), _canPan && EnablePan},
				{ nameof(Tilt), _canTilt && EnableTilt},
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
			// If any of the nodes can pan and
			// the user has enabled Pan control then...
			if (_canPan && EnablePan)
			{
				// Render the pan intents
				RenderCurve(Pan, FunctionIdentity.Pan, _panTags, cancellationToken);
			}

			// If any of the nodes can tilt and
			// the user has enabled Tilt control then...
			if (_canTilt && EnableTilt)
			{
				// Render the tilt intents
				RenderCurve(Tilt, FunctionIdentity.Tilt, _tiltTags, cancellationToken);
			}
		}

		/// <summary>
		/// Determines if the fixture supports the ability to pan or tilt.
		/// </summary>
		protected override void UpdateFixtureCapabilities()
		{	
			// Determine if any of the nodes can pan
			_canPan = GetRenderNodesForFunctionIdentity(FunctionIdentity.Pan, _panTags).Any();

			// Determine if any of the nodes can tilt
			_canTilt = GetRenderNodesForFunctionIdentity(FunctionIdentity.Tilt, _tiltTags).Any();					
		}

		#endregion

		#region Public Overrides of BaseEffect

		/// <summary>
		/// Generates the visual representation of the effect on the timeline.
		/// </summary>
		/// <param name="graphics">Graphics context</param>
		/// <param name="clipRectangle">Clipping rectangle of the effect on the timeline</param>
		public override void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		{
			// Set a flag that indicates if both the pan and tilt are supported by the element nodes
			// and enabled by the user for control
			bool showBoth = _canPan && EnablePan && _canTilt && EnableTilt;
			
			// Create the drawing rectangle for the curves
			// If both curves are displayed the drawing area is halved.  Top half for one curve and bottom half for the other curve.
			Rectangle rect = new Rectangle(clipRectangle.X, clipRectangle.Y, clipRectangle.Width, showBoth ? (clipRectangle.Height-4) / 2 : clipRectangle.Height-4);
			
			// If the node supports panning and
			// the user selected to control the Pan then...
			if (_canPan && EnablePan)
			{
				// Draw the pan curve on the timeline
				DrawVisualRepresentation(g, rect, Color.Green, "Pan", 2, showBoth ? -2 : 1, Pan, 0 );
			}

			// If the node supports tilting and
			// the user selected to control the Tilt then...
			if (_canTilt && EnableTilt)
			{
				// Defint the tilt color
				Color tiltColor = Color.FromArgb(0, 128, 255);

				const string CurveText = "Tilt";
								
				// If showing both pan and tilt curves then...
				if (showBoth)
				{					
					// Draw the tilt curve in the bottom of the timeline
					DrawVisualRepresentation(g, rect, tiltColor, CurveText, rect.Height+3, rect.Height, Tilt, rect.Y);
				}
				else
				{
					// Draw the tilt curve on the timeline
					DrawVisualRepresentation(g, rect, tiltColor, CurveText, 2, 1, Tilt, rect.Y);
				}			
			}
		}

		#endregion
	}
}