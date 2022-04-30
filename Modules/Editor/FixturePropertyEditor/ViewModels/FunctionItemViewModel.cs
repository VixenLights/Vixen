using Catel.Data;
using System;
using System.Collections.Generic;
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
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="indexData">Index data associated with the function</param>
		/// <param name="colorWheelData">Color wheel data associated with the function</param>
		/// <param name="rotationLimits">Rotation limits associated with the function</param>
		public FunctionItemViewModel(
			List<FixtureIndex> indexData,
			List<FixtureColorWheel> colorWheelData,
			FixtureRotationLimits rotationLimits) : this()
		{
			// Store off the index data
			IndexData = indexData;

			// Store off the color wheel data
			ColorWheelData = colorWheelData;

			// Store off the rotation limit data
			RotationLimits = rotationLimits;
		}

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
