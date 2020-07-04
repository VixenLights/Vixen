using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Common.WPFCommon.Controls;
using NLog;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Converters;
using ZedGraph;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public abstract class BaseInlineCurveEditor: Control
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Type ThisType = typeof(BaseInlineCurveEditor);
		private Image _image;

		protected Image Image
		{
			get
			{
				if (_image == null)
				{
					_image = (Image)Template.FindName("CurveImage", this);
				}
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		private Canvas _canvas;
		protected Canvas Canvas {
			get
			{
				if (_canvas == null)
				{
					_canvas = (Canvas)Template.FindName("FaderCanvas", this);
				}
				return _canvas;
			}
			set
			{
				_canvas = value;
			}				
		}



		private SliderPoint _levelPoint;
		private ToolTip _toolTip;

		#region Fields

		private int _dragStartPointIndex;
		private Point _dragStartPoint;
		private static readonly CurveToImageConverter Converter = new CurveToImageConverter();

		private bool _isMouseDown;
		private Curve _holdValue;

		private const double DragTolerance = 2.0;
		private const double DistanceTolerance = 8.0;

		#endregion Fields

		static BaseInlineCurveEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		protected BaseInlineCurveEditor()
		{
			Loaded += InlineCurveEditor_Loaded;
			ShowToolTip = true;
		}

		#region Events

		private void InlineCurveEditor_Loaded(object sender, RoutedEventArgs e)
		{
			var template = Template;
			_image = (Image)template.FindName("CurveImage", this);
			Canvas = (Canvas)Template.FindName("FaderCanvas", this);
			AddLevelSlider();
			OnCurveValueChanged();
		}

		protected void OnCurveValueChanged()
		{
			UpdateLevelSliderPosition(GetCurveValue());
			UpdateImage(GetCurveValue());
		}

		#region PropertyEditingStarted Event

		/// <summary>
		/// Identifies the <see cref="PropertyEditingStarted"/> routed event.
		/// </summary>
		public static readonly RoutedEvent PropertyEditingStartedEvent =
			EffectPropertyEditorGrid.PropertyEditingStartedEvent.AddOwner(ThisType);

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
		public static readonly RoutedEvent PropertyEditingFinishedEvent =
			EffectPropertyEditorGrid.PropertyEditingFinishedEvent.AddOwner(ThisType);

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



		#region Properties

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="IsDragging"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool),
			ThisType, new PropertyMetadata(false, OnIsDraggingChanged));

		public static readonly DependencyProperty SliderStyleProperty = DependencyProperty.Register("SliderStyle", typeof(Style), ThisType);

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

		#endregion Dependency Property Fields

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

		public Style SliderStyle
		{
			get { return (Style)GetValue(SliderStyleProperty); }
			set { SetValue(SliderStyleProperty, value); }
		}

		public bool ShowToolTip { get; set; }

		#endregion Properties

		#region Property Changed Callbacks


		private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineCurveEditor = (BaseInlineCurveEditor)d;
			inlineCurveEditor.OnIsDraggingChanged();
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

		#region Base Class Overrides

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			if ((Keyboard.Modifiers & (ModifierKeys.Shift)) != 0 && !GetCurveValue().IsLibraryReference)
			{
				Canvas.Visibility = Visibility.Visible;
			}
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Canvas.Visibility = Visibility.Collapsed;
		}

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			var curve = GetCurveValue();
			if (curve == null || curve.IsLibraryReference) return;
			_holdValue = curve;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				_dragStartPoint = e.GetPosition(this);
				var point = TranslateMouseLocation(_dragStartPoint);
				_dragStartPointIndex = FindClosestPoint(curve.Points, point);
				if (Keyboard.Modifiers == ModifierKeys.Shift ||
					Keyboard.Modifiers == ModifierKeys.Control ||
					Dist(point, curve.Points[_dragStartPointIndex]) < DistanceTolerance)
				{
					_isMouseDown = true;
					_holdValue = curve;

					Focus();
					CaptureMouse();

					e.Handled = true;
				}
			}
		}

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			try
			{
				var curve = GetCurveValue();
				if (curve == null || curve.IsLibraryReference) return;

				if ((Keyboard.Modifiers & (ModifierKeys.Shift)) != 0)
				{
					Canvas.Visibility = Visibility.Visible;
				}
				else
				{
					Canvas.Visibility = Visibility.Collapsed;
				}

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
						}
					}
					else
					{
						if (Keyboard.Modifiers == ModifierKeys.Shift)
						{
							SetLevel(position);
						}
						else
						{
							MovePoint(position);
						}

						e.Handled = true;
					}
				}
				else
				{
					var point = TranslateMouseLocation(position);
					var index = FindClosestPoint(curve.Points, point);
					if (Keyboard.Modifiers == ModifierKeys.Shift)
					{
						if (IsMouseOverLevelHandle())
						{
							DisableToolTip();
							Cursor = Cursors.SizeWE;
						}
						else
						{
							Cursor = Cursors.SizeNS;
						}
					}
					else if (Dist(point, curve.Points[index]) < DistanceTolerance)
					{
						Cursor = Cursors.Cross;
					}
					else
					{
						DisableToolTip();
						Cursor = Cursors.Arrow;
					}
				}
			}
			catch (Exception ex)
			{
				//There is an object null error that occurs at random times in this method that I have been unable to effectively track down
				//It causes the app to crash and potentially loses data. This method is not doing a critical operation, so we can eat the 
				//exception to avoid crashing the user. Hopefully I can eventually track it down and fix the root cause.
				Logging.Error(ex, "An unexpected error occured during the mouse move in the curve and was swallowed.");
			}

		}

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			var curve = GetCurveValue();
			if (curve == null || curve.IsLibraryReference) return;
			if ( (Keyboard.Modifiers & (ModifierKeys.Shift)) != 0 && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) != 0)
			{
				InvertReverseCurve();
			}
			else if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				AddPoint(_dragStartPoint);
			}
			else if (Keyboard.Modifiers == ModifierKeys.Alt && _isMouseDown)
			{
				RemovePoint(_dragStartPoint);
			}

			if (IsDragging || _isMouseDown || !curve.Equals(_holdValue))
			{
				e.Handled = true;
				IsDragging = false;
				_isMouseDown = false;
				SetCurveValue(_holdValue);
			}

			DisableToolTip();
			ReleaseMouseCapture();

		}

		#endregion Base Class Overrides

		#region Helpers

		private void InvertReverseCurve()
		{
			var points = GetCurveValue().Points.Clone();
			if ((Keyboard.Modifiers & (ModifierKeys.Alt)) != 0)
			{
				//Reverses the Curve
				for (int i = 0; i < points.Count; i++)
				{
					points[i].X = 100 - points[i].X;
				}
			}
			else
			{
				//Inverts teh Curve
				for (int i = 0; i < points.Count; i++)
				{
					points[i].Y = 100 - points[i].Y;
				}
			}

			points.Sort();
			_holdValue = new Curve(points);
			UpdateImage(_holdValue);
		}

		private void AddLevelSlider()
		{
			var points = GetCurveValue().Points;

			if (points.Count == 2 && points[0].Y == points[1].Y)
			{
				_levelPoint = new SliderPoint(GetCurveValue().Points[0].Y / 100.0, Canvas, true);
			}
			else
			{
				_levelPoint = new SliderPoint(0.0, Canvas, true);
			}
			

			if (_levelPoint.Center >= 0 && _levelPoint.Center <= Canvas.Width)
			{
				OnAddItem(_levelPoint);
			}
		}

		private void UpdateLevelSliderPosition(Curve curve)
		{
			
			if (_levelPoint != null)
			{
				var points = curve.Points;
				if (points.Count == 2 && points[0].Y == points[1].Y)
				{
					_levelPoint.NormalizedPosition = points[0].Y / 100.0;
				}
				else
				{
					_levelPoint.NormalizedPosition = 0.0;
				}
			}
		}

		private void OnAddItem(SliderPoint sliderPoint)
		{
			sliderPoint.Parent = Canvas;
			sliderPoint.SliderShape.Style = SliderStyle;
			sliderPoint.PropertyChanged += LevelPoint_PropertyChanged;
			sliderPoint.DragCompleted += SliderPoint_DragCompleted;
			
			try
			{
				Canvas.Children.Add(sliderPoint.SliderShape);
			}
			catch { }
		}

		private void SliderPoint_DragCompleted(object sender, EventArgs e)
		{
			if (_holdValue != null)
			{
				SetCurveValue(_holdValue);
				_holdValue = null;
			}
		}

		private void LevelPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			if (e.PropertyName.Equals("center", StringComparison.OrdinalIgnoreCase))
			{
				if (_holdValue == null)
				{
					_holdValue = new Curve(GetCurveValue());
				}
				SliderPoint point = sender as SliderPoint;
				if (point != null)
				{
					if (point.IsDragging)
					{
						var level = Math.Round(point.NormalizedPosition * 100, MidpointRounding.AwayFromZero);
						
						_holdValue = new Curve(new PointPairList(new[] { 0.0, 100.0}, new[] { level, level }));

						UpdateImage(_holdValue);
					}
				}
			}
		}

		private void MovePoint(Point mousePosition)
		{
			if (mousePosition.X > ActualWidth + 5 || mousePosition.Y > ActualHeight + 5) return;
			var point = TranslateMouseLocation(mousePosition);
			
			var points = GetCurveValue().Points.Clone();
			var boundedPoint = EnforcePointBounds(point);
			points[_dragStartPointIndex] = boundedPoint;
			EnableToolTip();
			SetToolTip(string.Format("{0:0}, {1:0}", boundedPoint.X, boundedPoint.Y));

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
		}

		private void SetLevel(Point mousePosition)
		{
			var point = TranslateMouseLocation(mousePosition);

			var levelPoint = EnforcePointBounds(point);
			var points = new PointPairList(new[] { 0.0, 100.0 }, new[] { levelPoint.Y, levelPoint.Y });

			EnableToolTip();
			SetToolTip(string.Format("{0:0}", levelPoint.Y));

			_holdValue = new Curve(points);

			UpdateLevelSliderPosition(_holdValue);

			UpdateImage(_holdValue);
		}

		private void AddPoint(Point mousePosition)
		{
			var point = TranslateMouseLocation(mousePosition);

			var points = GetCurveValue().Points.Clone();

			points.Add(EnforcePointBounds(point));

			points.Sort();

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
		}

		private void RemovePoint(Point mousePosition)
		{
			var curve = GetCurveValue();

			if (curve.Points.Count < 3) return; //Curve must contain two points to be valid

			var point = TranslateMouseLocation(mousePosition);

			var index = FindClosestPoint(curve.Points, point);

			var points = curve.Points.Clone();
			
			points.Remove(points[index]);

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
			
		}

		private void UpdateImage(Curve curve)
		{
			if (_image != null)
			{
				_image.Source = (BitmapImage)Converter.Convert(curve, null, true, null);
			}
		}

		protected abstract void SetCurveValue(Curve c);

		protected abstract Curve GetCurveValue();

		private bool IsMouseOverLevelHandle()
		{
			return _levelPoint.IsMouseOver;
		}

		private int FindClosestPoint(PointPairList points, PointPair point)
		{
			int closestIndex = -1;
			double minDist = Double.MaxValue;
			for (int i = 0; i < points.Count; i++)
			{
				var dist = Dist(point, points[i]);
				if (dist < minDist)
				{
					minDist = dist;
					closestIndex = i;
				}
			}

			return closestIndex;
		}

		private static double Dist(PointPair origin, PointPair point)
		{
			return Math.Sqrt(Math.Pow((point.X - origin.X), 2) + Math.Pow((point.Y - origin.Y), 2));
		}

		private PointPair TranslateMouseLocation(Point pos)
		{
			var position = TranslatePoint(pos, Image);
			//Console.Out.WriteLine("Mouse Location;{0},{1}", position.X, position.Y);
			var pct = position.X / Image.Width;
			var x = 100 * pct;

			pct = (_image.Height - position.Y) / Image.Height;
			var y = 100 * pct;

			return new PointPair(x, y);
		}

		private PointPair EnforcePointBounds(PointPair p, bool round = true)
		{
			if (round)
			{
				p.X = Math.Round(p.X, MidpointRounding.AwayFromZero);
				p.Y = Math.Round(p.Y, MidpointRounding.AwayFromZero);
			}
			if (p.X > 100)
			{
				p.X = 100;
			}
			else if (p.X < 0)
			{
				p.X = 0;
			}

			if (p.Y > 100)
			{
				p.Y = 100;
			}
			else if (p.Y < 0)
			{
				p.Y = 0;
			}

			return p;
		}

		private void SetToolTip(string formattedValue)
		{
			if (ShowToolTip)
			{
				if (_toolTip == null)
				{
					_toolTip = new ToolTip
					{
						PlacementTarget = this,
						Placement = PlacementMode.Bottom,
						VerticalOffset = -20
					};
					ToolTip = _toolTip;
				}
				else
				{
					_toolTip.Content = formattedValue;
				}
			}
		}

		private void EnableToolTip()
		{
			if (_toolTip != null && ShowToolTip)
			{
				_toolTip.IsEnabled = true;
				_toolTip.IsOpen = true;
			}
		}

		private void DisableToolTip()
		{
			if (_toolTip != null && _toolTip.IsEnabled)
			{
				_toolTip.Content = null;
				_toolTip.IsEnabled = false;
				_toolTip.IsOpen = false;
			}
		}

		#endregion Helpers
	}
}
