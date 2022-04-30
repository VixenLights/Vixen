using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Maintains one index (enumeration) name value pair.
    /// </summary>
    public class IndexedItemViewModel : ItemViewModel, IFixtureSaveable
	{
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public IndexedItemViewModel()
		{			
		}

        #endregion

        #region Public Catel Properties

		/// <summary>
		/// Start value of the index.
		/// </summary>
        public string StartValue
		{
			get { return GetValue<string>(StartValueProperty); }
			set 
			{ 
				SetValue(StartValueProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Start value property data.
		/// </summary>
		public static readonly PropertyData StartValueProperty = RegisterProperty(nameof(StartValue), typeof(string), null);

		/// <summary>
		/// End value of the index.
		/// </summary>
		public string EndValue
		{
			get { return GetValue<string>(EndValueProperty); }
			set 
			{ 
				SetValue(EndValueProperty, value);
				
				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// End value property data.
		/// </summary>
		public static readonly PropertyData EndValueProperty = RegisterProperty(nameof(EndValue), typeof(string), null);

		/// <summary>
		/// Indicator if the index is a ranage and should be represented by a curve.
		/// </summary>
		public bool UseCurve
		{
			get { return GetValue<bool>(UseCurveProperty); }
			set 
			{ 
				SetValue(UseCurveProperty, value);
				
				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Use curve property data.
		/// </summary>
		public static readonly PropertyData UseCurveProperty = RegisterProperty(nameof(UseCurve), typeof(bool), null);

		/// <summary>
		/// Type of the index for use by the preview.
		/// </summary>
		public FixtureIndexType IndexType
		{
			get { return GetValue<FixtureIndexType>(IndexTypeProperty); }
			set 
			{ 
				SetValue(IndexTypeProperty, value);

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Index type property data.
		/// </summary>
		public static readonly PropertyData IndexTypeProperty = RegisterProperty(nameof(IndexType), typeof(FixtureIndexType), null);

		#endregion

		#region Public Properties

		/// <summary>
		/// Collectin of index types.
		/// This collection excludes the ColorWheel index type as it should not be user selectable.		
		/// </summary>
		public IList<FixtureIndexType> IndexTypes
		{
			get
			{
				// Create the return value
				List<FixtureIndexType> types = new List<FixtureIndexType>();

				// Loop over the enumerated values in the enumeration
				foreach (FixtureIndexType enumValue in Enum.GetValues(typeof(FixtureIndexType)))
				{
					// Add them to the collection
					types.Add(enumValue);
				}

				// Remove ColorWheel because it is intended for internal use and it should not be displayed
				// on the fixture editor
				types.Remove(FixtureIndexType.ColorWheel);

				return types;
			}
		}

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

			// Validate the index start value
			ValidateDMXNumber(validationResults, StartValueProperty, "Start Value", StartValue);
		
			// If the index uses a curve then...
			if (UseCurve)
			{
				// Validate the index end value
				ValidateDMXNumber(validationResults, EndValueProperty, "End Value", EndValue);
			}

			// Ensure the validation bar is displayed;
			DisplayValidationBar(validationResults);			
		}

		#endregion
	}
}
