using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VixenModules.App.CustomPropEditor.Controls
{
    /// <summary>
    /// Interaction logic for SliderMenuItem.xaml
    /// </summary>
    public partial class SliderMenuItem : MenuItem
    {
        private const double ThumbHeight = 9.0d;
        private Slider _slider;
        private readonly SortedDictionary<double, double> _tickValueMap;

        
        /// <summary>
        /// Instance Constructor.
        /// </summary>
        public SliderMenuItem()
        {
            _tickValueMap = new SortedDictionary<double, double>();
            InitializeComponent();
        }

        #region Attached Property Steps

        
        /// <summary>
        /// Xaml accessor function for Getting Steps property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int GetSteps(DependencyObject obj)
        {
            return (int)obj.GetValue(StepsProperty);
        }

        
        /// <summary>
        /// Xaml accessor function for Setting Steps property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSteps(DependencyObject obj, int value)
        {
            obj.SetValue(StepsProperty, value);
        }

        
        /// <summary>
        /// The number of steps, or tick placements between a menu item
        /// and the previous one.
        /// </summary>
        public static readonly DependencyProperty StepsProperty =
            DependencyProperty.RegisterAttached(
            "Steps",
            typeof(int),
            typeof(SliderMenuItem),
            new UIPropertyMetadata(1));

        #endregion

        #region Attached Property Value

        
        /// <summary>
        /// Gets or Sets the value for this slider.
        /// </summary>
        public double ItemValue
        {
            get { return (double)GetValue(ItemValueProperty); }
            set { SetValue(ItemValueProperty, value); }
        }

        
        /// <summary>
        /// Xaml accessor function for Value property
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double GetItemValue(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemValueProperty);
        }

        
        /// <summary>
        /// Xaml accessor function for Value property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetItemValue(DependencyObject obj, double value)
        {
            obj.SetValue(ItemValueProperty, value);
        }

        
        /// <summary>
        /// The real value for the slider.  
        /// </summary>
        public static readonly DependencyProperty ItemValueProperty =
            DependencyProperty.RegisterAttached(
            "ItemValue",
            typeof(double),
            typeof(SliderMenuItem),
            new UIPropertyMetadata(1.0d, Value_ValueChanged));

        #endregion

        #region Attached Property Skip

        
        /// <summary>
        /// Skip property for sub menu items.  Allows a menu item to 
        /// skip a tick mark.  Especially useful for separators.
        /// </summary>
        public static readonly DependencyProperty SkipProperty =
            DependencyProperty.RegisterAttached(
            "Skip",
            typeof(bool),
            typeof(SliderMenuItem),
            new UIPropertyMetadata(false));

        
        /// <summary>
        /// Xaml accessor function for Skip property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetSkip(DependencyObject obj)
        {
            return (bool)obj.GetValue(SkipProperty);
        }

        
        /// <summary>
        /// Xaml accessor function for Skip property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSkip(DependencyObject obj, bool value)
        {
            obj.SetValue(SkipProperty, value);
        }

        #endregion

        
        /// <summary>
        /// Listen for change to Value property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private static void Value_ValueChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            // find appropriate tick spot, and set slider value
            SliderMenuItem item = obj as SliderMenuItem;

            if (item != null)
            {
                SetTickToValue(item, (double)e.NewValue);
            }
        }

        
        /// <summary>
        /// Listen for change to slider value.  Enables binding where
        /// SliderMenuItem is the target, and a framework element is the source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            // find appropriate tick spot, and set slider value
            SetValueToTick(this, (double)e.NewValue);
        }

        
        /// <summary>
        /// Sets the slider thumb to the closest match after the value changes.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newValue"></param>
        private static void SetTickToValue(SliderMenuItem item, double newValue)
        {
            // find tick spot where   
            double[] ticks = new double[item._tickValueMap.Keys.Count];
            item._tickValueMap.Keys.CopyTo(ticks, 0);

            // Find exact match
            if (item._tickValueMap.ContainsValue(newValue))
            {
                foreach (double tick in item._tickValueMap.Keys)
                {
                    if (item._tickValueMap[tick] == newValue)
                    {
                        item._slider.Value = tick;
                        return;
                    }
                }
            }

            // Find closest match
            for (int i = 1; i < item._tickValueMap.Count; i++)
            {

                double lowTick = ticks[i - 1];
                double highTick = ticks[i];

                double lowValue = item._tickValueMap[lowTick];
                double highValue = item._tickValueMap[highTick];

                //double newValue = (double)e.NewValue;

                if (newValue > lowValue &&
                    newValue < highValue)
                {
                    double valueScale = highValue - lowValue;
                    double tickScale = highTick - lowTick;

                    // set slider to closest tick match
                    double newTick = (newValue - lowValue) / (valueScale) * tickScale + lowTick;

                    item._slider.Value = newTick;
                }
            }
        }

        
        /// <summary>
        /// After the slider value has changed, update the Value property to 
        /// hold the scaled value.
        /// </summary>
        private static void SetValueToTick(SliderMenuItem item, double tickValue)
        {
            if (item._tickValueMap.ContainsKey(tickValue))
            {
                item.ItemValue = item._tickValueMap[tickValue];
                return;
            }

            double[] keys = new double[item._tickValueMap.Keys.Count];
            item._tickValueMap.Keys.CopyTo(keys, 0);

            int index = Array.BinarySearch<double>(keys, tickValue);

            Debug.Assert(index < 0, "What? How come I didn't find the key already?");

            index = ~index;

            Debug.Assert(index < item.Items.Count, "How did tick value go above 1000?");
            Debug.Assert(index != 0, "Insert location was before element 0.");

            double lowTick = keys[index - 1];
            double highTick = keys[index];

            double lowValue = item._tickValueMap[lowTick];
            double highValue = item._tickValueMap[highTick];

            double valueScale = highValue - lowValue;
            double sourceScale = highTick - lowTick;

            double newValue = (tickValue - lowTick) * valueScale / sourceScale + lowValue;
            item.ItemValue = newValue;
        }

        
        /// <summary>
        /// After template is applied, save reference to slider object.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._slider = this.Template.FindName("PART_Slider", this) as Slider;

            if (_slider == null)
                throw new InvalidOperationException("Control template is missing part Slider_PART");

            _slider.ValueChanged += Slider_ValueChanged;
        }

        
        /// <summary>
        /// Arrange pass.  Call base method to figure out menu item placement,
        /// then place tick marks at the centers of the menu items.
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size returnSize = base.ArrangeOverride(arrangeBounds);

            FrameworkElement topElement = null;
            FrameworkElement bottomElement = null;

            for (int i = 0; i < Items.Count; i++)
            {
                FrameworkElement elem = Items[i] as FrameworkElement;

                Debug.Assert(elem != null, "Added an object that wasn't a FrameworkElement??");

                // Find the bottom element.  It must have a value greater or equal to the
                // top element
                if (topElement != null)
                {
                    if ((double)elem.GetValue(SliderMenuItem.ItemValueProperty) >=
                        (double)topElement.GetValue(SliderMenuItem.ItemValueProperty))
                    {
                        bottomElement = elem;
                    }
                }

                // Move along.  Nothing to see here.
                if ((bool)(elem.GetValue(SliderMenuItem.SkipProperty)))
                    continue;

                // set the first element
                if (topElement == null)
                    topElement = elem;
            }

            // Single element.  Not much of a slider, but don't crash
            if (bottomElement == null && topElement != null)
                bottomElement = topElement;

            // No elements.  Nothing to do.
            if (bottomElement == null && topElement == null)
                return returnSize;

            // Calculate top, bottom margins.
            // This margin enables the thumb stop at 0 and 100 to line up with
            // the center of the top and bottom menu items.
            Rect bound = LayoutInformation.GetLayoutSlot(topElement);
            
            var track = (Track)_slider.Template.FindName("PART_Track", _slider);
            var thumb = track.Thumb;
            
            double topMargin = bound.Top + thumb.ActualHeight / 2;
            double pointZero = bound.Top + bound.Height / 2.0;

            bound = LayoutInformation.GetLayoutSlot(bottomElement);

            double bottomMargin = returnSize.Height - bound.Bottom + thumb.ActualHeight / 2.0d;
            double pointOneHundred = bound.Top + bound.Height / 2.0d;

            // Set the margin.
            _slider.Margin = new Thickness(0, topMargin, 0, bottomMargin);


            for (int i = 0; i < Items.Count; i++)
            {
                FrameworkElement elem = Items[i] as FrameworkElement;

                if (elem is MenuItem)
                    ((MenuItem)elem).Click += new RoutedEventHandler(SliderMenuItem_Click);

                // Move along.  Nothing to see here.
                if ((bool)(elem.GetValue(SliderMenuItem.SkipProperty)))
                    continue;

                // Grab the coordinates of the child menu item
                bound = LayoutInformation.GetLayoutSlot(elem);

                // Get the number of steps, or tick spots between this child menu item
                //  and the previous one.
                int steps = (int)elem.GetValue(SliderMenuItem.StepsProperty);

                // A value of 0 for Steps is like setting Skip = true
                if (steps < 1)
                    continue;

                // Calculate tick spot.
                double thisTickSpot = _slider.Maximum * (bound.Top - pointZero + bound.Height / 2.0d)
                    / (pointOneHundred - pointZero);

                // Calculate continuous tick spots.  Only allow continuous after the first element.
                if (_slider.Ticks.Count > 0)
                {
                    double lastTickSpot = _slider.Ticks[_slider.Ticks.Count - 1];
                    double division = (thisTickSpot - lastTickSpot) / steps;

                    for (int current_step = 1; current_step < steps; current_step++)
                    {
                        double intermediateTickSpot = lastTickSpot + current_step * division;
                        _slider.Ticks.Add(intermediateTickSpot);
                    }
                }

                _slider.Ticks.Add(thisTickSpot);
                double sliderValue = (double)elem.GetValue(SliderMenuItem.ItemValueProperty);
                _tickValueMap[thisTickSpot] = sliderValue;

            }

            // At end of arrange pass, set the tick to the inital value
            SetTickToValue(this, ItemValue);

            return returnSize;
        }

        
        /// <summary>
        /// A child menu item was clicked.  Set the value automatically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ItemValue = (double)((DependencyObject)sender).GetValue(SliderMenuItem.ItemValueProperty);
        }
    }
}
