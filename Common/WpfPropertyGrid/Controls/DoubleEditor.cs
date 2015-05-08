/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ComponentModel;
using System.Windows.Input;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
  /// <summary>
  /// Simple Expression Blend like double editor.
  /// </summary>
  public class DoubleEditor : Control
  {
    private static readonly Type ThisType = typeof(DoubleEditor);

    #region Fields

    private Point _dragStartPoint;
    private Point _lastDragPoint;

    private double _changeValue;
    private double _changeOffset;
    private bool _isMouseDown;

    private const double DragTolerance = 2.0;

    #endregion Fields

    #region Events

    #region PropertyEditingStarted Event

    /// <summary>
    /// Identifies the <see cref="PropertyEditingStarted"/> routed event.
    /// </summary>
    public static readonly RoutedEvent PropertyEditingStartedEvent = PropertyGrid.PropertyEditingStartedEvent.AddOwner(ThisType);

    /// <summary>
    /// Occurs when property editing is started.
    /// </summary>
    public event PropertyEditingEventHandler PropertyEditingStarted
    {
      add { AddHandler(PropertyEditingStartedEvent, value); }
      remove { RemoveHandler(PropertyEditingFinishedEvent, value); }
    }

    #endregion PropertyEditingStarted Event

    #region PropertyEditingFinished Event

    /// <summary>
    /// Identifies the <see cref="PropertyEditingFinished"/> routed event.
    /// </summary>
    public static readonly RoutedEvent PropertyEditingFinishedEvent = PropertyGrid.PropertyEditingFinishedEvent.AddOwner(ThisType);

    /// <summary>
    /// Occurs when property editing is finished.
    /// </summary>
    public event PropertyEditingEventHandler PropertyEditingFinished
    {
      add { AddHandler(PropertyEditingFinishedEvent, value); }
      remove { RemoveHandler(PropertyEditingFinishedEvent, value); }
    }

    #endregion PropertyEditingFinished Event
    
    #endregion Events

    #region Constructors

    static DoubleEditor()
    {
      DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));

      CommandManager.RegisterClassCommandBinding(ThisType, new CommandBinding(Increase, OnIncrease));
      CommandManager.RegisterClassCommandBinding(ThisType, new CommandBinding(Decrease, OnDecrease));
    }

    #endregion Constructors

    #region Dependency Property Fields

    /// <summary>
    /// Identifies the <see cref="Value"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), ThisType, new PropertyMetadata(0.0, ValueChanged));

    /// <summary>
    /// Identifies the <see cref="SmallChange"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), ThisType, new PropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="LargeChange"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), ThisType, new PropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="DefaultChange"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultChangeProperty = DependencyProperty.Register("DefaultChange", typeof(double), ThisType, new PropertyMetadata(1.0));

    /// <summary>
    /// Identifies the <see cref="Minimum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), ThisType, new PropertyMetadata(0.0, OnMinimumChanged));

    /// <summary>
    /// Identifies the <see cref="Maximum"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), ThisType, new PropertyMetadata(double.MaxValue, OnMaximumChanged));

    /// <summary>
    /// Identifies the <see cref="MaxPrecision"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxPrecisionProperty = DependencyProperty.Register("MaxPrecision", typeof(int), ThisType, new PropertyMetadata(0));

    /// <summary>
    /// Identifies the <see cref="IsDragging"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), ThisType, new PropertyMetadata(false, OnIsDraggingChanged));

    #endregion Dependency Property Fields

    #region RoutedCommand Fields

    /// <summary>
    /// Specifies a command to increase value.
    /// </summary>
    public static RoutedCommand Increase = new RoutedCommand("Increase", ThisType);

    /// <summary>
    /// Specifies a command to decrease value.
    /// </summary>
    public static RoutedCommand Decrease = new RoutedCommand("Decrease", ThisType);

    #endregion RoutedCommand Fields

    #region RoutedEvent Handlers

    private static void OnIncrease(object sender, ExecutedRoutedEventArgs e)
    {
      var ve = (DoubleEditor)sender;

      ve._changeValue = ve.Value;
      ve._changeOffset = 0;
      ve.CalculateValue(ve.DefaultChange);
    }

    private static void OnDecrease(object sender, ExecutedRoutedEventArgs e)
    {
      var ve = (DoubleEditor)sender;

      ve._changeValue = ve.Value;
      ve._changeOffset = 0;
      ve.CalculateValue(-ve.DefaultChange);
    }

    #endregion RoutedEvent Handlers

    #region Properties

    #region PropertyDescriptor Property

    /// <summary>
    /// Identifies the <see cref="PropertyDescriptor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PropertyDescriptorProperty =
      DependencyProperty.Register("PropertyDescriptor", typeof(PropertyDescriptor), ThisType);

    /// <summary>
    /// Underlying property descriptor. This is a dependency property.
    /// </summary>
    public PropertyDescriptor PropertyDescriptor
    {
      get { return (PropertyDescriptor)GetValue(PropertyDescriptorProperty); }
      set { SetValue(PropertyDescriptorProperty, value); }
    }

    #endregion PropertyDescriptor Property

    /// <summary>
    /// Gets or sets the value. This is a dependency property.
    /// </summary>
    /// <value>The value.</value>
    [TypeConverter(typeof(LengthConverter))]
    public double Value
    {
      get { return (double)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    /// <summary>
    /// Gets or sets the small change value. This is a dependency property.
    /// </summary>
    /// <value>The small change value.</value>
    public double SmallChange
    {
      get { return (double)GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    /// <summary>
    /// Gets or sets the default change value. This is a dependency property.
    /// </summary>
    /// <value>The default change value.</value>
    public double DefaultChange
    {
      get { return (double)GetValue(DefaultChangeProperty); }
      set { SetValue(DefaultChangeProperty, value); }
    }

    /// <summary>
    /// Gets or sets the large change value. This is a dependency property.
    /// </summary>
    /// <value>The large change value.</value>
    public double LargeChange
    {
      get { return (double)GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }

    /// <summary>
    /// Gets or sets the minimum. This is a dependency property.
    /// </summary>
    /// <value>The minimum.</value>
    public double Minimum
    {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    /// <summary>
    /// Gets or sets the maximum. This is a dependency property.
    /// </summary>
    /// <value>The maximum.</value>
    public double Maximum
    {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    /// <summary>
    /// Gets or sets the maximum precision. This is a dependency property.
    /// </summary>
    /// <value>The maximum precision.</value>
    public int MaxPrecision
    {
      get { return (int)GetValue(MaxPrecisionProperty); }
      set { SetValue(MaxPrecisionProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is dragging.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is dragging; otherwise, <c>false</c>.
    /// </value>
    [Browsable(false)]
    public bool IsDragging
    {
      get { return (bool)GetValue(IsDraggingProperty); }
      set { SetValue(IsDraggingProperty, value); }
    }

    #endregion Properties

    #region Property Changed Callbacks

    private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var doubleEditor = (DoubleEditor)d;
      if (!doubleEditor.IsInitialized)
        return;

      doubleEditor.Value = doubleEditor.EnforceLimitsAndPrecision((double)e.NewValue);
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var doubleEditor = (DoubleEditor)d;
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var doubleEditor = (DoubleEditor)d;
    }

    private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var doubleEditor = (DoubleEditor)d;
      doubleEditor.OnIsDraggingChanged();
    }

    protected virtual void OnIsDraggingChanged()
    {
      if (IsDragging)
      {
        OnPropertyEditingStarted();
      }
      else
      {
        OnPropertyEditingFinished();
      }
    }

    #endregion Property Changed Callbacks

    #region Base Class Overrides

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      base.OnMouseDown(e);
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        _isMouseDown = true;
        _dragStartPoint = e.GetPosition(this);

        Focus();
        CaptureMouse();

        e.Handled = true;
      }
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      Point position = e.GetPosition(this);
      Vector vector = position - _dragStartPoint;

      if (_isMouseDown)
      {
        if (!IsDragging)
        {
          if (vector.Length > DragTolerance)
          {
            IsDragging = true;
            e.Handled = true;

            _dragStartPoint = position;

            _lastDragPoint = _dragStartPoint;
            _changeValue = Value;
            _changeOffset = 0;
          }
        }
        else
        {
          Vector offset = position - _lastDragPoint;
          double offsetLength = Math.Round(offset.Length);

          if (offsetLength != 0)
          {
            CalculateValue((offset.X > offset.Y) ? offsetLength : -offsetLength);
            _lastDragPoint = position;
          }
        }
        e.Handled = true;
      }
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      base.OnMouseUp(e);

      if (IsDragging || _isMouseDown)
      {
        e.Handled = true;
        IsDragging = false;
        _isMouseDown = false;
      }

      ReleaseMouseCapture();
    }

    #endregion Base Class Overrides

    #region Helpers

    private void CalculateValue(double chageValue)
    {
      //
      // Calculate the base ammount of chage based on...
      //
      // On Mouse Click & Control Key Press
      if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
        chageValue *= SmallChange;
      // On Mouse Click & Shift Key Press
      else if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        chageValue *= LargeChange;
      else
        chageValue *= DefaultChange;

      _changeOffset += chageValue;
      double newValue = _changeValue + _changeOffset;
      //
      // Make sure the change is line up with Max/Min Limits and set the precission as specified.
      Value = EnforceLimitsAndPrecision(newValue);

      return;
    }

    private double EnforceLimitsAndPrecision(double value)
    {
      return Math.Round(Math.Max(Minimum, Math.Min(Maximum, value)), MaxPrecision);
    }

    #endregion Helpers

    /// <summary>
    /// Raises the <see cref="PropertyEditingStarted"/> event.
    /// </summary>
    protected virtual void OnPropertyEditingStarted()
    {
      RaiseEvent(new PropertyEditingEventArgs(PropertyEditingStartedEvent, this, PropertyDescriptor));
    }

    /// <summary>
    /// Raises the <see cref="PropertyEditingFinished"/> event.
    /// </summary>
    protected virtual void OnPropertyEditingFinished()
    {
      RaiseEvent(new PropertyEditingEventArgs(PropertyEditingFinishedEvent, this, PropertyDescriptor));
    }
  }
}