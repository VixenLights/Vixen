using Catel.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Vixen.Data.Value;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains a fixture function item view model.
    /// </summary>
    public class FunctionItemViewModel : ItemViewModel, IFixtureSaveable
	{
		#region Constructors 

		/// <summary>
		/// Constructor
		/// </summary>
		public FunctionItemViewModel()
		{						
			// Default the function type to Range
			FunctionTypeEnum = FixtureFunctionType.Range;

			// Default the preview identity to Custom
			FunctionIdentity = FunctionIdentity.Custom;
			
			// Initialize the index data
			IndexData = new List<FixtureIndex>();

			// Initialize the color wheel data
			ColorWheelData = new List<FixtureColorWheel>();

			// Create the list of valid function identities for range functions
			_rangeFunctionIndentities = new List<FunctionIdentity>();
			_rangeFunctionIndentities.Add(FunctionIdentity.Pan);
			_rangeFunctionIndentities.Add(FunctionIdentity.Tilt);
			_rangeFunctionIndentities.Add(FunctionIdentity.Zoom);
			_rangeFunctionIndentities.Add(FunctionIdentity.Dim);
			_rangeFunctionIndentities.Add(FunctionIdentity.Frost);
			_rangeFunctionIndentities.Add(FunctionIdentity.Custom);

			// Create the list of valid function identities for index functions
			_indexFunctionIndentities = new List<FunctionIdentity>();
			_indexFunctionIndentities.Add(FunctionIdentity.Shutter);
			_indexFunctionIndentities.Add(FunctionIdentity.Gobo);			
			_indexFunctionIndentities.Add(FunctionIdentity.Prism);
			_indexFunctionIndentities.Add(FunctionIdentity.OpenClosePrism);			
			_indexFunctionIndentities.Add(FunctionIdentity.Custom);

			// Create the list of valid function identities for color wheel functions
			_colorWheelIdentities = new List<FunctionIdentity>();
			_colorWheelIdentities.Add(FunctionIdentity.SpinColorWheel);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="indexData">Index data associated with the function</param>
		/// <param name="colorWheelData">Color wheel data associated with the function</param>
		/// <param name="rotationLimits">Rotation limits associated with the function</param>
		/// <param name="zoomNarrowToWide">Indicates whether the fixture zooms from narrow to wide</param>
		public FunctionItemViewModel(
			List<FixtureIndex> indexData,
			List<FixtureColorWheel> colorWheelData,
			FixtureRotationLimits rotationLimits,
			bool zoomNarrowToWide) : this()
		{
			// Store off the index data
			IndexData = indexData;

			// Store off the color wheel data
			ColorWheelData = colorWheelData;

			// Store off the rotation limit data
			RotationLimits = rotationLimits;

			// Store off whether this fixture zooms narrow to wide
			ZoomNarrowToWide = zoomNarrowToWide;
		}

		#endregion

		#region Fields

		/// <summary>
		/// Collection of function identities applicable to range functions.
		/// </summary>
		private List<FunctionIdentity> _rangeFunctionIndentities;

		/// <summary>
		/// Collection of function identities applicable to index functions.
		/// </summary>
		private List<FunctionIdentity> _indexFunctionIndentities;

		/// <summary>
		/// Collection of function identities applicable to color wheel functions.
		/// </summary>
		private List<FunctionIdentity> _colorWheelIdentities;

		#endregion

		#region Public Properties 

		/// <summary>
		/// Index data associated with the function.
		/// </summary>
		public List<FixtureIndex> IndexData { get; set; }

		/// <summary>
		/// Color wheel data associated with the function.
		/// </summary>
		public List<FixtureColorWheel> ColorWheelData { get; set; }

		/// <summary>
		/// Rotation limits associated with the function.
		/// </summary>
		public FixtureRotationLimits RotationLimits { get; set; }

		/// <summary>
		/// Whether the fixtures zooms narrow to wide.
		/// </summary>
		public bool ZoomNarrowToWide { get; set; }

		/// <summary>
		/// Color of the function on the timeline for certain effects.
		/// </summary>
		public Color TimelineColor { get; set; }

		/// <summary>
		/// List of avalailable tags (function identities) for the function item.
		/// </summary>
		public IList<FunctionIdentity> FunctionIdentities
		{
			get
			{
				// Create the return value
				List<FunctionIdentity> types = new List<FunctionIdentity>();

				// Determine the function identities based on function type
				switch(FunctionTypeEnum)
				{					
					case FixtureFunctionType.Range:
						types = _rangeFunctionIndentities;
						break;					
					case FixtureFunctionType.Indexed:
						types = _indexFunctionIndentities;
						break;
					case FixtureFunctionType.ColorWheel:
						types = _colorWheelIdentities;
						break;
					case FixtureFunctionType.RGBColor:
					case FixtureFunctionType.RGBWColor:
					case FixtureFunctionType.None:
						break;
						default:
						Debug.Assert(false, "Unsupported function type!");
						break;
				}

				// Return the applicable function identities
				return types;
			}
		}

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Type of function (Ranged, Indexed, Color Wheel etc).
		/// </summary>
		public FixtureFunctionType FunctionTypeEnum
		{
			get 
			{ 
				return GetValue<FixtureFunctionType>(FunctionTypeEnumProperty); 
			}
			set 
			{ 
				SetValue(FunctionTypeEnumProperty, value);

				// Fire the Function Type Changed event
				EventHandler handler = FunctionTypeChanged;
				handler?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Function type property data.
		/// </summary>
		public static readonly PropertyData FunctionTypeEnumProperty = RegisterProperty(nameof(FunctionTypeEnum), typeof(FixtureFunctionType), null);

		/// <summary>
		/// Function identity.
		/// </summary>
		public FunctionIdentity FunctionIdentity
		{
			get
			{
				return GetValue<FunctionIdentity>(FunctionIdentityProperty);
			}
			set
			{
				SetValue(FunctionIdentityProperty, value);

				// Force Catel to re-validate
				Validate(true);

				// Refresh the command button enabled status
				RefreshCanExecuteChanged();

				// Fire the Function Type Changed event to ensure the correct details control is displayed
				EventHandler handler = FunctionTypeChanged;
				handler?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Function property data.
		/// </summary>
		public static readonly PropertyData FunctionIdentityProperty = RegisterProperty(nameof(FunctionIdentity), typeof(FunctionIdentity), null);
		
		/// <summary>
		/// Preview legend associated with the function.
		/// </summary>
		public string Legend
		{
			get { return GetValue<string>(LegendProperty); }
			set 
			{ 
				SetValue(LegendProperty, value);				
			}
		}

		/// <summary>
		/// Legend property data.
		/// </summary>
		public static readonly PropertyData LegendProperty = RegisterProperty(nameof(Legend), typeof(string), null);

		/// <summary>
		/// Maximum rotation associated with the function.  Only applies to Pan and tilt functions.
		/// </summary>
		public string MaximumRotation
		{
			get { return GetValue<string>(MaximumRotationProperty); }
			set
			{
				SetValue(MaximumRotationProperty, value);
			}
		}

		/// <summary>
		/// Maximum rotation property data.
		/// </summary>
		public static readonly PropertyData MaximumRotationProperty = RegisterProperty(nameof(MaximumRotation), typeof(string), null);

		#endregion

		#region Public Events

		/// <summary>
		/// Event for when the function type changed.
		/// This event ensures the correct user control is displayed in the detail area.
		/// </summary>
		public event EventHandler FunctionTypeChanged;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Validates the editable fields.
		/// </summary>
		/// <param name="validationResults">Results of the validation</param>
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			// Validate the name field
			ValidateName(validationResults);
			
			// Ensure the validation bar is displayed
			DisplayValidationBar(validationResults);
		}
		
		#endregion
	}
}
