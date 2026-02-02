using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Common.WPFCommon.Controls
{
	/// <summary>
	/// Interaction logic for SuperSlider.xaml
	/// </summary>
	public partial class SuperSlider : UserControl
    {
		#region Constructors
        public SuperSlider()
        {
            InitializeComponent();

            // This handles the initial sync after XAML parsing is done
            this.Loaded += (s, e) =>
            {
	            if (ValueSlider != null)
	            {
		            ValueSlider.Minimum = this.Minimum;
		            ValueSlider.Maximum = this.Maximum;
		            ValueSlider.Value = this.Value;
	            }
            };
        }
		#endregion

		#region Properties
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
	        nameof(Value), typeof(int), typeof(SuperSlider), new PropertyMetadata(default(int), OnValueChanged));

		public int Value
		{
			get => (int)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
	        nameof(Minimum), typeof(int), typeof(SuperSlider), new PropertyMetadata(default(int), OnMinimumChanged));

		public int Minimum
		{
			get => (int)GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
	        nameof(Maximum), typeof(int), typeof(SuperSlider), new PropertyMetadata(default(int), OnMaximumChanged));

        public int Maximum
        {
	        get => (int)GetValue(MaximumProperty);
	        set => SetValue(MaximumProperty, value);
        }

		public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(
	        nameof(AutoToolTipPlacement),
	        typeof(AutoToolTipPlacement),
	        typeof(SuperSlider),
	        new PropertyMetadata(AutoToolTipPlacement.None, OnAutoToolTipPlacementChanged)); // Link the callback here

        public AutoToolTipPlacement AutoToolTipPlacement
        {
	        get => (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty);
	        set => SetValue(AutoToolTipPlacementProperty, value); // Keep this simple
        }

		#endregion

		#region Callbacks
		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (SuperSlider)d;
			int newValue = (int)e.NewValue;

			if (control.ValueSlider != null && control.ValueSlider.Value != newValue)
			{
				control.ValueSlider.Value = newValue;
			}
		}
		private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (SuperSlider)d;
			int newMinimum = (int)e.NewValue;

			if (control.ValueSlider != null)
			{
				control.ValueSlider.Minimum = newMinimum;
			}

			if (control.Value < newMinimum)
			{
				control.Value = newMinimum;
			}
		}

		private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (SuperSlider)d;
			int newMaximum = (int)e.NewValue;

			if (control.ValueSlider != null)
			{
				control.ValueSlider.Maximum = newMaximum;
			}

			if (control.Value > newMaximum)
			{
				control.Value = newMaximum;
			}
		}

		private static void OnAutoToolTipPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (SuperSlider)d;

			// Check if the internal slider exists yet (important during initialization)
			if (control.ValueSlider != null)
			{
				control.ValueSlider.AutoToolTipPlacement = (AutoToolTipPlacement)e.NewValue;
			}
		}
		#endregion

		#region Event Handlers
		private void DecreaseButton_Click(object sender, RoutedEventArgs e)
		{
			Value--;
		}

		private void IncreaseButton_Click(object sender, RoutedEventArgs e)
		{
			Value++;
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			Value = (int)((Maximum - Minimum) / 2) + Minimum;
		}

		private void ValueSlider_Scroll(object sender, MouseWheelEventArgs e)
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

			e.Handled = true;
		}

		private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int newValue = (int)Math.Round(e.NewValue);

			if (this.Value != newValue)
			{
				this.Value = newValue;
			}
		}		
		#endregion
	}
}
