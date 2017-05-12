using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common.Controls;
using Common.Controls.ColorManagement.ColorPicker;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.Editor.EffectEditor.Converters;
using VixenModules.Effect.Effect;
using Color = System.Drawing.Color;
using Control = System.Windows.Controls.Control;
using Cursors = System.Windows.Input.Cursors;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class InlineGradientEditor : BaseInlineGradientEditor
	{
		private static readonly Type ThisType = typeof(InlineGradientEditor);

		#region Fields

		private ColorGradient _holdValue;

		
		//private Canvas _canvas;
		
		#endregion Fields

		static InlineGradientEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		public InlineGradientEditor()
		{
			//Loaded += InlineGradientEditor_Loaded;
		}

		#region Events

		private void InlineGradientEditor_Loaded(object sender, RoutedEventArgs e)
		{
			//_canvas = (Canvas)Template.FindName("FaderCanvas", this);
			//_canvas.MouseMove += _canvas_MouseMove; ;
			_canvas.MouseDown += CanvasOnMouseDown;
			_canvas.MouseLeftButtonDown += CanvasOnMouseLeftButtonDown;
			OnComponentChanged();
			ReloadPoints();
			UpdateImage(Value);
		}

		private void _canvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsMouseOverAnyHandle())
			{
				Cursor = Cursors.SizeWE;
			}
			else
			{
				Cursor = Cursors.Arrow;
			}
			
		}

		private void CanvasOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				if (IsMouseOverAnyHandle())
				{
					//launch the color editor.
					ShowColorPicker();
				}
				e.Handled = true;
			}
		}

		private void CanvasOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			var mousePoint = Mouse.GetPosition(sender as IInputElement);
			if (mousePoint.X < 0 || mousePoint.X > _canvas.Width || mousePoint.Y < 0 || mousePoint.Y > _canvas.Height) return;
			if (IsMouseOverAnyHandle()) return;
			AddColorPoint(mousePoint.X / _canvas.Width);
		}

		//private bool IsMouseOverAnyHandle()
		//{
		//	return _points.Any(q => q.IsMouseOver);
		//}

		private void OnAddItem(SliderPoint sliderPoint)
		{
			sliderPoint.Parent = _canvas;
			sliderPoint.SliderShape.Style = SliderStyle;
			sliderPoint.SliderShape.KeyDown += GradientPoint_KeyDown; ;
			sliderPoint.PropertyChanged += GradientPoint_PropertyChanged;
			sliderPoint.DragCompleted += SliderPoint_DragCompleted;
			try
			{
				_canvas.Children.Add(sliderPoint.SliderShape);
			}
			catch { }
		}

		private void SliderPoint_DragCompleted(object sender, EventArgs e)
		{
			_holdValue = new ColorGradient(_holdValue);
			SetValue();
		}

		private void GradientPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("center", StringComparison.OrdinalIgnoreCase))
			{
				if (_holdValue == null)
				{
					_holdValue = new ColorGradient(Value);

				}
				SliderPoint point = sender as SliderPoint;
				if (point != null)
				{
					var pointIndexes = point.Tag as List<int>;
					var colorPoints = _holdValue.Colors;
					if (pointIndexes != null)
					{
						foreach (var index in pointIndexes)
						{
							colorPoints[index].Position = point.NormalizedPosition;
						}
					}
					
					UpdateImage(_holdValue);
				}
			}
		}

		private void GradientPoint_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				DeletePoint();
				e.Handled = true;
			}
		}

		private void OnRemoveItem(SliderPoint sliderPoint)
		{
			sliderPoint.SliderShape.KeyDown -= GradientPoint_KeyDown;
			sliderPoint.PropertyChanged -= GradientPoint_PropertyChanged;
			sliderPoint.DragCompleted += SliderPoint_DragCompleted;
			try
			{
				_canvas.Children.Remove(sliderPoint.SliderShape);
			}
			catch { }
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

		#region Dependency Property Fields

		/// <summary>
		/// Identifies the <see cref="Value"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ColorGradient),
			ThisType, new PropertyMetadata(new ColorGradient(), ValueChanged));

		

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

		public int SelectedIndex
		{
			get
			{
				var selectedPoint = _points.FirstOrDefault(x => x.SliderShape.IsFocused);
				return selectedPoint != null ? _points.IndexOf(selectedPoint) : -1;
			}
		}

		/// <summary>
		/// Gets or sets the value. This is a dependency property.
		/// </summary>
		/// <value>The value.</value>
		public ColorGradient Value
		{
			get { return (ColorGradient)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		


		#endregion Properties

		#region Property Changed Callbacks

		private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var inlineGradientEditor = (InlineGradientEditor)d;
			if (!inlineGradientEditor.IsInitialized)
				return;
			inlineGradientEditor.Value = (ColorGradient) e.NewValue;
			inlineGradientEditor.UpdateImage(inlineGradientEditor.Value);
			inlineGradientEditor.ReloadPoints();
		}

		

		#endregion Property Changed Callbacks

		#region Base Class Overrides



		#endregion Base Class Overrides

		#region Helpers

		private void ShowColorPicker()
		{
			var selectedIndex = SelectedIndex;
			if (selectedIndex < 0) return;
			var handle = _points[selectedIndex];
			var colorPointIndexes = (List<int>)handle.Tag;
			if (IsDiscrete)
			{
				_holdValue = new ColorGradient(Value);
				List<Color> selectedColors = new List<Color>();
				List<ColorPoint> colorPoints = new List<ColorPoint>();
				foreach (int index in colorPointIndexes)
				{
					ColorPoint pt = _holdValue.Colors[index];
					if (pt == null)
						continue;
					colorPoints.Add(pt);
					selectedColors.Add(pt.Color.ToRGB().ToArgb());
				}

				using (DiscreteColorPicker picker = new DiscreteColorPicker())
				{
					picker.ValidColors = ValidColors;
					picker.SelectedColors = selectedColors;
					if (picker.ShowDialog() == DialogResult.OK)
					{
						if (!picker.SelectedColors.Any())
						{
							DeletePoint();
						}
						else
						{
							double position = colorPoints.First().Position;
							foreach (var colorPoint in colorPoints)
							{
								_holdValue.Colors.Remove(colorPoint);
							}

							foreach (Color selectedColor in picker.SelectedColors)
							{
								ColorPoint newPoint = new ColorPoint(selectedColor, position);
								_holdValue.Colors.Add(newPoint);
							}
							SetValue();
						}
					}
				}
			}
			else
			{
				if (colorPointIndexes.Count > 1)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Non-discrete color gradient, >1 selected point. oops! please report it.", "Delete library gradient?", false, false);
					messageBox.ShowDialog();
				}
				_holdValue = new ColorGradient(Value);
				ColorPoint pt = _holdValue.Colors[colorPointIndexes.FirstOrDefault()];
				if (pt == null)
					return;
				using (ColorPicker frm = new ColorPicker(Mode, Fader))
				{
					frm.LockValue_V = false;
					frm.Color = pt.Color;
					if (frm.ShowDialog() == DialogResult.OK)
					{
						pt.Color = frm.Color;
						Mode = frm.SecondaryMode;
						Fader = frm.PrimaryFader;
						SetValue();
					}
				}
			}
		}

		

		protected void SetValue()
		{
			Value = _holdValue;
			_holdValue = null;
		}

		private void ClearPoints()
		{
			_points.ToList().ForEach(OnRemoveItem);
			_points.Clear();
		}

		private SliderPoint AddPoint(double position, bool isSelected = false)
		{
			var point = new SliderPoint(position, _canvas);

			//point.SliderShape.MouseDown += (s, a) => SelectedIndex = _points.IndexOf(point);
			if (point.Center >= 0 && point.Center <= _canvas.Width)
			{
				_points.Add(point);
				_points.Sort(new SliderPointPositionComparer());
				OnAddItem(point);
				if (isSelected)
				{
					point.SliderShape.Focus();
				}
				//OnValueAdded(e);
			}

			return point;
		}

		private void DeletePoint()
		{
			var selectedIndex = SelectedIndex;
			if (selectedIndex >= 0)
			{
				var selectedPoint = _points[selectedIndex];
				List<int> colorPointIndexes = (List<int>)selectedPoint.Tag;
				_holdValue = new ColorGradient(Value);

				List<ColorPoint> colorPoints = new List<ColorPoint>();
				foreach (int index in colorPointIndexes)
				{
					ColorPoint pt = _holdValue.Colors[index];
					if (pt == null)
						continue;
					colorPoints.Add(pt);
				}

				foreach (var colorpoint in colorPoints)
				{
					_holdValue.Colors.Remove(colorpoint);
				}
			}

			SetValue();

		}

		
		private void ReloadPoints()
		{
			if (Value != null)
			{
				ClearPoints();
				List<ColorPoint> sortedColors = new List<ColorPoint>(Value.Colors.SortedArray());
				// we'll assume they're sorted, so any colorpoints at the same position are contiguous in the array
				for (int i = 0; i < sortedColors.Count; i++)
				{
					ColorPoint currentPoint = sortedColors[i];
					double currentPos = currentPoint.Position;
					List<int> indexes = new List<int>();

					indexes.Add(Value.Colors.IndexOf(currentPoint));
					while (i + 1 < sortedColors.Count && sortedColors[i + 1].Position == currentPos)
					{
						indexes.Add(Value.Colors.IndexOf(sortedColors[i + 1]));
						i++;
					}

					var point = AddPoint(currentPos);
					point.Tag = indexes;
				}
			}
		}

		private void AddColorPoint(double pos)
		{
			List<ColorPoint> newColorPoints;
			if (IsDiscrete)
			{
				newColorPoints = new List<ColorPoint>();
				
				if (ValidColors.Any())
				{
					newColorPoints.Add(new ColorPoint(ValidColors.FirstOrDefault(), pos));
				}

			}
			else
			{
				newColorPoints = new List<ColorPoint> { new ColorPoint(Value.GetColorAt((float)pos), pos) };
			}

			_holdValue = new ColorGradient(Value);
			foreach (ColorPoint newColorPoint in newColorPoints)
			{
				_holdValue.Colors.Add(newColorPoint);
			}

			SetValue();
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

		protected override ColorGradient GetColorGradientValue()
		{
			return Value;
		}
	}
}
