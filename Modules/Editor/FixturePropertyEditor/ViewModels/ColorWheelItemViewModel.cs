using Catel.Data;
using Catel.MVVM;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains a color wheel item view model.
	/// </summary>
	public class ColorWheelItemViewModel : ItemViewModel, IFixtureSaveable
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public ColorWheelItemViewModel()
		{
			// Create the command to edit the color
			EditColor1Command = new Command(EditColor1);

			// Text for the color buttons
			ButtonText = "...";			
		}

		#endregion

		#region Public Properties
		
		/// <summary>
		/// Delegate to update the half step colors based on the items before and after.
		/// </summary>
		public Action UpdateHalfSteps { get; set; }

		/// <summary>
		/// Command to edit the color.
		/// </summary>
		public ICommand EditColor1Command { get; set; }

		#endregion

		#region Public Catel Properties

		/// <summary>
		/// Start DMX value of the color item.
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
		/// End DMX value of the color item.
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
		/// Indicates if the color wheel entry uses a curve beteween the start and stop values.
		/// </summary>
		public bool UseCurve
		{
			get { return GetValue<bool>(UseCurveProperty); }
			set { SetValue(UseCurveProperty, value); }
		}

		/// <summary>
		/// Use Curve value property data.
		/// </summary>
		public static readonly PropertyData UseCurveProperty = RegisterProperty(nameof(UseCurve), typeof(bool), null);


		/// <summary>
		/// First color associated with the color wheel item.
		/// </summary>
		public System.Drawing.Color Color1
		{
			get { return GetValue<System.Drawing.Color>(Color1Property); }
			set { SetValue(Color1Property, value); }
		}

		/// <summary>
		/// Color 1 property data.
		/// </summary>
		public static readonly PropertyData Color1Property = RegisterProperty(nameof(Color1), typeof(System.Drawing.Color), null);

		/// <summary>
		/// Second color associated with the color wheel item.
		/// </summary>
		public System.Drawing.Color Color2
		{
			get { return GetValue<System.Drawing.Color>(Color2Property); }
			set { SetValue(Color2Property, value); }
		}

		/// <summary>
		/// Color 2 property data.
		/// </summary>
		public static readonly PropertyData Color2Property = RegisterProperty(nameof(Color2), typeof(System.Drawing.Color), null);
		
		/// <summary>
		/// Color item represents a half step and contains two colors.
		/// </summary>
		public bool HalfStep
		{
			get { return GetValue<bool>(HalfStepProperty); }
			set
			{
				SetValue(HalfStepProperty, value);

				// If the color item is a half step then...
				if (value)
				{
					// Clear out the button text
					ButtonText = String.Empty;

					// Update all the half step colors
					UpdateHalfSteps();
				}
				// Otherwise the entry is NOT a half step
				else
				{
					// Show the ellipses on the button
					ButtonText = "...";
				}

				// Refresh command status
				RaiseCanExecuteChangedInternal();
			}
		}

		/// <summary>
		/// Half step property data.
		/// </summary>
		public static readonly PropertyData HalfStepProperty = RegisterProperty(nameof(HalfStep), typeof(bool), null);

		/// <summary>
		/// Button text on the color panel.
		/// </summary>
		public string ButtonText
		{
			get { return GetValue<string>(ButtonTextProperty); }
			set { SetValue(ButtonTextProperty, value); }
		}

		/// <summary>
		/// Button text property data.
		/// </summary>
		public static readonly PropertyData ButtonTextProperty = RegisterProperty(nameof(ButtonText), typeof(string), null);

		/// <summary>
		/// Indicates if the color wheel entry should be excluded from the color property of the element.
		/// </summary>
		public bool ExcludeColorProperty
		{
			get { return GetValue<bool>(ExcludeColorPropertyProperty); }
			set { SetValue(ExcludeColorPropertyProperty, value); }
		}

		/// <summary>
		/// ExcludeColor value property data.
		/// </summary>
		public static readonly PropertyData ExcludeColorPropertyProperty = RegisterProperty(nameof(ExcludeColorProperty), typeof(bool), null);

		#endregion

		#region Private Methods

		/// <summary>
		/// Displays a dialog to edit the item's color.
		/// </summary>
		private void EditColor1()
		{
			// Half step colors are not editable
			if (!HalfStep)
			{
				// Create the color picker dialog
				using (ColorPicker cp = new ColorPicker())
				{
					// Display the color picker dialog
					cp.LockValue_V = false;
					cp.Color = XYZ.FromRGB(Color1);
					DialogResult result = cp.ShowDialog();

					// If the user selected OK then...
					if (result == DialogResult.OK)
					{
						// Update the color wheel item color based on the user's selection
						Color1 = cp.Color.ToRGB();
					}
				}

				// Update all the half steps
				UpdateHalfSteps();
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
			// Validate the Name property
			ValidateName(validationResults);	

			// Validate the StartValue property
			ValidateDMXNumber(validationResults, StartValueProperty, "Start Value", StartValue);

			// If the color wheel item uses a curve then...
			if (UseCurve)
			{
				// Validate the EndValue property
				ValidateDMXNumber(validationResults, StartValueProperty, "End Value", StartValue);
			}			

			// Display the validation bar
			DisplayValidationBar(validationResults);			
		}

        #endregion      				        
    }
}
