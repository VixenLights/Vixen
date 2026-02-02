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
		    nameof(Value), typeof(int), typeof(NumericTextSpinBox), new PropertyMetadata(default(int), OnValueChanged));

	    public int Value
	    {
			get
			{
				return (int)GetValue(ValueProperty);
			}
			set
			{
				value = Math.Clamp(value, Minimum, Maximum);
				SetValue(ValueProperty, value);
				ValueBox.Text = value.ToString();
			}
		}

	    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
		    nameof(Minimum), typeof(int), typeof(NumericTextSpinBox), new PropertyMetadata(default(int)));

	    public int Minimum
	    {
		    get
		    {
			    return (int)GetValue(MinimumProperty);
		    }
		    set
		    {
			    SetValue(MinimumProperty, value);
				if (Value < Minimum)
				{
					Value = Minimum;
				}
			}
		}

	    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
		    nameof(Maximum), typeof(int), typeof(NumericTextSpinBox), new PropertyMetadata(default(int)));

	    public int Maximum
	    {
		    get
		    {
			    return (int)GetValue(MaximumProperty);
		    }
		    set
		    {
			    SetValue(MaximumProperty, value);
				if (Value > Maximum)
				{
					Value = Maximum;
				}
			}
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
            Maximum = Int32.MaxValue;
			Minimum = Int32.MinValue;
			Value = 0;
		}
		#endregion

		#region Callbacks
		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (NumericTextSpinBox)d;
			if (control.ValueBox != null)
			{
				control.ValueBox.Text = control.Value.ToString();
			}

			// Fire the Routed Event
			control.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
		}
		#endregion

		#region Event Handlers
		private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
	        TextBox tb = sender as TextBox;

			// Only allow a minus if it is at the start AND there isn't already a minus sign there.
			if (e.Text == "-")
			{
				e.Handled = tb.SelectionStart != 0 || tb.Text.Contains("-");
			}

			// Don't allow anything in front of the minus sign
			else if (tb.SelectionStart == 0 && tb.SelectionLength == 0 && tb.Text.StartsWith("-"))
				e.Handled = true;

			// If the user types anything else, check if it's a digit
			else
			{
				// If it's NOT a digit, block it
				Regex regex = new Regex("[^0-9]");
				e.Handled = regex.IsMatch(e.Text);
			}
        }

		private void ValueBox_LostFocus(object sender, EventArgs e)
		{
			if (int.TryParse(ValueBox.Text, out int result) == false)
			{
				Value = Minimum;
			}
			else
			{
				Value = Convert.ToInt32(ValueBox.Text);
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
					Value++;
				}
			}
			else if (e.Delta < 0)
			{
				// Scroll Down: Decrease value
				if (Value > Minimum)
				{
					Value--;
				}
			}

			ValueBox.SelectionLength = 0;
			ValueBox.CaretIndex = ValueBox.Text.Length;
			e.Handled = true;
		}

		private void ValueBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
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
					Value += (Maximum - Minimum + 1)/10;
					if (Value > Maximum)
					{
						Value = Maximum;
					}
					e.Handled = true;
					break;

				case Key.PageDown:
					Value -= (Maximum - Minimum + 1) / 10;
					if (Value < Minimum)
					{
						Value = Minimum;
					}
					e.Handled = true;
					break;

				case Key.Up:
					if (Value < Maximum)
					{
						Value++;
					}
					e.Handled = true;
					break;

				case Key.Down:
					if (Value > Minimum)
					{
						Value--;
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
				Value--;
			}
			ValueBox.Focus();
        }

		private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
			if (Value < Maximum)
			{
				Value++;
			}
			ValueBox.Focus();
		}
		#endregion
	}
}
