using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VixenModules.App.Curves;
using VixenModules.Editor.EffectEditor.Converters;
using ZedGraph;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineCurveEditor : Control
	{
		private static readonly Type ThisType = typeof(InlineCurveEditor);
		private Image _image;

		#region Fields

		private int _dragStartPointIndex;
		private Point _dragStartPoint;
		private static readonly CurveToImageConverter Converter = new CurveToImageConverter();

		private bool _isMouseDown;
		private Curve _holdValue;

		private const double DragTolerance = 2.0;
		private const double DistanceTolerance = 8.0;

		#endregion Fields

		static InlineCurveEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		public InlineCurveEditor()
		{
			Loaded += InlineCurveEditor_Loaded;
		}

		private void InlineCurveEditor_Loaded(object sender, RoutedEventArgs e)
		{
			var template = Template;
			_image = (Image)template.FindName("CurveImage", this);
		}

		#region Events

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

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Curve),
			ThisType, new PropertyMetadata(new Curve(CurveType.Flat100), ValueChanged));

		/// <summary>
		/// Identifies the <see cref="IsDragging"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool),
			ThisType, new PropertyMetadata(false, OnIsDraggingChanged));

		#endregion Dependency Property Fields

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
		//[TypeConverter(typeof(LengthConverter))]
		public Curve Value
		{
			get { return (Curve)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
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
			var inlineCurveEditor = (InlineCurveEditor)d;
			if (!inlineCurveEditor.IsInitialized)
				return;
			inlineCurveEditor.Value = (Curve) e.NewValue;
			inlineCurveEditor.UpdateImage(inlineCurveEditor.Value);

		}

		private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineCurveEditor = (InlineCurveEditor)d;
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

		#region Base Class Overrides

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (Value == null) return;
			if (e.LeftButton == MouseButtonState.Pressed) 
			{
				_dragStartPoint = e.GetPosition(this);
				var point = TranslateMouseLocation(_dragStartPoint);
				_dragStartPointIndex = FindClosestPoint(Value.Points, point);
				if (Keyboard.Modifiers == ModifierKeys.Shift || 
					Keyboard.Modifiers == ModifierKeys.Control || 
					Dist(point, Value.Points[_dragStartPointIndex]) < DistanceTolerance)
				{
					_holdValue = Value;
					_isMouseDown = true;

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
			if (Value==null) return;
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
				var index = FindClosestPoint(Value.Points, point);
				if (Dist(point, Value.Points[index]) < DistanceTolerance)
				{
					Cursor = Cursors.Cross;
				}
				else
				{

					Cursor = Cursors.Arrow;
				}
			}
			
		}

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (Value == null) return;
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				AddPoint(_dragStartPoint);
			}
			else if (Keyboard.Modifiers == ModifierKeys.Alt && _isMouseDown)
			{
				RemovePoint(_dragStartPoint);
			}
			
			if (IsDragging || _isMouseDown)
			{
				e.Handled = true;
				IsDragging = false;
				_isMouseDown = false;
				SetValue();
			}

			ReleaseMouseCapture();

		}

		#endregion Base Class Overrides

		#region Helpers

		private void MovePoint(Point mousePosition)
		{
			if (mousePosition.X > ActualWidth+5 || mousePosition.Y > ActualHeight+5) return;
			var point = TranslateMouseLocation(mousePosition);

			var points = Value.Points.Clone();

			points[_dragStartPointIndex] = EnforcePointBounds(point);

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
		}

		private void SetLevel(Point mousePosition)
		{
			var point = TranslateMouseLocation(mousePosition);

			var levelPoint = EnforcePointBounds(point);
			var points = new PointPairList(new[] { 0.0, 100.0}, new[] { levelPoint.Y, levelPoint.Y });

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
		}

		private void AddPoint(Point mousePosition)
		{
			var point = TranslateMouseLocation(mousePosition);

			var points = Value.Points.Clone();

			points.Add(EnforcePointBounds(point));

			points.Sort();

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);		
		}

		private void RemovePoint(Point mousePosition)
		{
			var point = TranslateMouseLocation(mousePosition);

			var index = FindClosestPoint(Value.Points, point);

			var points = Value.Points.Clone();
			
			points.Remove(points[index]);

			_holdValue = new Curve(points);

			UpdateImage(_holdValue);
		}

		private void UpdateImage(Curve curve)
		{
			_image.Source = (BitmapImage)Converter.Convert(curve, null, true, null);
		}

		protected void SetValue()
		{
			Value = _holdValue;
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
			var position = TranslatePoint(pos, _image);
			//Console.Out.WriteLine("Mouse Location;{0},{1}", position.X, position.Y);
			var pct = position.X / _image.Width;
			var x = 100 * pct;

			pct = (_image.Height - position.Y) / _image.Height;
			var y = 100 * pct;

			return new PointPair(x, y);
		}

		private PointPair EnforcePointBounds(PointPair p)
		{
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

	}
}
