using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Common.WPFCommon.Controls
{
    /// <summary>
    /// Interaction logic for NumericTextSpinBox.xaml
    /// </summary>
    public partial class NumericTextSpinBox : UserControl
    {
		#region Properties
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
		    nameof(Value), typeof(double), typeof(NumericTextSpinBox), new PropertyMetadata((double)0, OnValueChanged, ValidateValue));

	    public double Value
		{
			get => (double)GetValue(ValueProperty);
			set
			{
				var newValue = Math.Round(value, DecimalPlaces);
				SetValue(ValueProperty, newValue);
			}
		}

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
		    nameof(Minimum), typeof(double), typeof(NumericTextSpinBox), new PropertyMetadata((double)0, ValidateValue_Callback));

	    public double Minimum
	    {
		    get => (double)GetValue(MinimumProperty);
		    set => SetValue(MinimumProperty, value);
		}

	    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
		    nameof(Maximum), typeof(double), typeof(NumericTextSpinBox), new PropertyMetadata((double)100, ValidateValue_Callback));

	    public double Maximum
	    {
		    get => (double)GetValue(MaximumProperty);
		    set => SetValue(MaximumProperty, value);
		}

	    public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register(
		    nameof(DecimalPlaces), typeof(int), typeof(NumericTextSpinBox), new PropertyMetadata(0));

	    public int DecimalPlaces
	    {
		    get => (int)GetValue(DecimalPlacesProperty);
		    set => SetValue(DecimalPlacesProperty, value);
	    }
		#endregion

		#region Events
		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
			nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericTextSpinBox));

		public event RoutedEventHandler ValueChanged
		{
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}
		#endregion

		#region Constructors
		public NumericTextSpinBox()
        {
			InitializeComponent();
		}
		#endregion

		#region Callbacks
		/// <summary>
		/// Set the text box to the entered value
		/// </summary>
		/// <param name="d"></param>
		/// <param name="e"></param>
		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (NumericTextSpinBox)d;

			control.ValueBox.Text = control.Value.ToString($"F{control.DecimalPlaces}");
			control.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
		}

		private static object ValidateValue(DependencyObject d, object baseValue)
		{
			var control = (NumericTextSpinBox)d;
			var value = (double)baseValue;
			return Math.Clamp(value, control.Minimum, control.Maximum);
		}

		private static void ValidateValue_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CoerceValue(ValueProperty);
		}		
		#endregion

		#region Event Handlers
		/// <summary>
		/// Validate if the proposed number is valid
		/// </summary>
		/// <param name="sender">Specifies the Control that is being validated</param>
		/// <param name="e">Specifies Text that is being validated</param>
		/// <remarks>e.Handled is set to false if the character is to be ignored</remarks>
		private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
	        TextBox tb = sender as TextBox;
			string newText;

			// What would the resulting value be if the character entered replaced some existing text
	        if (tb.SelectionLength > 0)
	        {
		        newText = tb.Text.Remove(tb.SelectionStart, tb.SelectionLength).Insert(tb.SelectionStart, e.Text);
	        }

	        // or inserted some new text
	        else
	        {
		        newText = tb.Text.Insert(tb.CaretIndex, e.Text);
	        }

			// Now let's validate that new value.
			// First, a special case that JUST a decimal or negative sign is entered
			if (newText.Length == 1 && (e.Text == "." || e.Text == "-"))
			{
				e.Handled = false;
			}

			// Else verify it's a valid number and within range
	        else if (double.TryParse(newText, out double result))
	        {
		        int decimalPoint = newText.LastIndexOf('.');

				// If there's a decimal point, verify the value is within the specified precision
				if (decimalPoint != -1 && newText.Length - decimalPoint > DecimalPlaces + 1)
				{
					e.Handled = true;
				}
				else
				{
					e.Handled = false;
				}
	        }

			// Or finally the value entered is not acceptable.
	        else
	        {
		        e.Handled = true;
	        }
		}

		private void ValueBox_LostFocus(object sender, EventArgs e)
		{
			if (double.TryParse(ValueBox.Text, out double result))
			{
				Value = result;
			}
			else
			{
				Value = Minimum;
			}
		}

		private void ValueBox_GotFocus(object sender, RoutedEventArgs e)
		{
			ValueBox.SelectionChanged += ValueBox_HandleInitialSelection;
		}

		private void ValueBox_HandleInitialSelection(object sender, RoutedEventArgs e)
		{
			// Immediately detach so the user can still highlight text manually later
			ValueBox.SelectionChanged -= ValueBox_HandleInitialSelection;

			// If Windows tried to select everything, undo it
			if (ValueBox.SelectionLength > 0)
			{
				ValueBox.SelectionLength = 0;
				ValueBox.CaretIndex = ValueBox.Text.Length;
			}
		}

		private void ValueBox_Scroll(object sender, MouseWheelEventArgs e)
		{
			// Check if the wheel was moved up or down
			if (e.Delta > 0)
			{
				// Scroll Up: Increase value
				if (Value < Maximum)
				{
					Value += StepValue(DecimalPlaces);
				}
			}
			else if (e.Delta < 0)
			{
				// Scroll Down: Decrease value
				if (Value > Minimum)
				{
					Value -= StepValue(DecimalPlaces);
				}
			}

			ValueBox.SelectionLength = 0;
			ValueBox.CaretIndex = ValueBox.Text.Length;
			e.Handled = true;
		}

		private void ValueBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (double.TryParse(tb.Text, out double result) == false)
			{
				result = Value;
			}

			switch (e.Key)
			{
				case Key.Home:
					Value = Minimum;
					e.Handled = true;
					break;

				case Key.End:
					Value = Maximum;
					e.Handled = true;
					break;

				case Key.PageUp:
					Value = result + (Maximum - Minimum)/10.0;
					if (Value > Maximum)
					{
						Value = Maximum;
					}
					e.Handled = true;
					break;

				case Key.PageDown:
					Value = result - (Maximum - Minimum) / 10.0;
					if (Value < Minimum)
					{
						Value = Minimum;
					}
					e.Handled = true;
					break;

				case Key.Up:
					if (Value < Maximum)
					{
						Value = result + StepValue(DecimalPlaces);
					}
					e.Handled = true;
					break;

				case Key.Down:
					if (Value > Minimum)
					{
						Value = result - StepValue(DecimalPlaces);
					}
					e.Handled = true;
					break;
			}

			if (e.Handled == true)
			{
				ValueBox.SelectionLength = 0;
				ValueBox.CaretIndex = ValueBox.Text.Length;
			}
		}

		private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
			if (Value > Minimum)
			{
				Value -= StepValue(DecimalPlaces);
			}
			ValueBox.Focus();
        }

		private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
			if (Value < Maximum)
			{
				Value += StepValue(DecimalPlaces);
			}
			ValueBox.Focus();
		}
		#endregion

		#region Private methods
		private double StepValue(int decimalPrecision)
		{
			return Math.Pow(10, -decimalPrecision);
		}
		#endregion
	}
}
