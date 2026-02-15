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
		}
		#endregion

		#region Properties
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
			nameof(Value), typeof(double), typeof(SuperSlider),
			new FrameworkPropertyMetadata(
				1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnValueChanged));

		public double Value
		{
			get => (double)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
			"ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(SuperSlider));

		public event RoutedPropertyChangedEventHandler<double> ValueChanged
		{
			add { AddHandler(ValueChangedEvent, value); }
			remove { RemoveHandler(ValueChangedEvent, value); }
		}

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
			nameof(Minimum), typeof(double), typeof(SuperSlider), new PropertyMetadata(1.0));

		public double Minimum
		{
			get => (double)GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
	        nameof(Maximum), typeof(double), typeof(SuperSlider), new PropertyMetadata(100.0));

        public double Maximum
        {
	        get => (double)GetValue(MaximumProperty);
	        set => SetValue(MaximumProperty, value);
        }

		public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(
	        nameof(AutoToolTipPlacement),typeof(AutoToolTipPlacement), typeof(SuperSlider),
	        new PropertyMetadata(AutoToolTipPlacement.None));

        public AutoToolTipPlacement AutoToolTipPlacement
        {
	        get => (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty);
	        set => SetValue(AutoToolTipPlacementProperty, value);
        }

        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
	        nameof(AutoToolTipPrecision), typeof(int), typeof(SuperSlider), new PropertyMetadata(0));

        public int AutoToolTipPrecision
        {
	        get => (int)GetValue(AutoToolTipPrecisionProperty);
	        set => SetValue(AutoToolTipPrecisionProperty, value);
        }

		public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
	        nameof(TickFrequency), typeof(double), typeof(SuperSlider), new PropertyMetadata(1.0));

        public double TickFrequency
		{
	        get => (double)GetValue(TickFrequencyProperty);
	        set => SetValue(TickFrequencyProperty, value);
        }

        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
	        nameof(SmallChange), typeof(double), typeof(SuperSlider), new PropertyMetadata(1.0));

        public double SmallChange
		{
	        get => (double)GetValue(SmallChangeProperty);
	        set => SetValue(SmallChangeProperty, value);
        }

        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
	        nameof(LargeChange), typeof(double), typeof(SuperSlider), new PropertyMetadata(1.0));

        public double LargeChange
		{
	        get => (double)GetValue(LargeChangeProperty);
	        set => SetValue(LargeChangeProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
	        var control = (SuperSlider)d;
	        var newValue = (double)e.NewValue;

			var args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue)
	        {
		        RoutedEvent = ValueChangedEvent
	        };
	        control.RaiseEvent(args);
        }
		#endregion

		#region Event Handlers
		private void DecreaseButton_Click(object sender, RoutedEventArgs e)
		{
			Value -= SmallChange;
		}

		private void IncreaseButton_Click(object sender, RoutedEventArgs e)
		{
			Value += SmallChange;
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			Value = ((Maximum - Minimum) / 2) + Minimum;
		}

		private void ValueSlider_Scroll(object sender, MouseWheelEventArgs e)
		{
			// Check if the wheel was moved up or down
			if (e.Delta > 0)
			{
				Value += SmallChange;
			}
			else if (e.Delta < 0)
			{
				Value -= SmallChange;
			}

			e.Handled = true;
		}

		private void ValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!Equals(this.Value, e.NewValue))
			{
				SetCurrentValue(ValueProperty, e.NewValue);
			}
		}		
		#endregion
	}
}
