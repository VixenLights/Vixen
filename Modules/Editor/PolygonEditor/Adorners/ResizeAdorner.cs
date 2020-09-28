using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using Size = System.Windows.Size;

namespace VixenModules.Editor.PolygonEditor.Adorners
{	
	public class ResizeAdorner : Adorner
	{
		// Resizing adorner uses Thumbs for visual elements.  
		// The Thumbs have built-in mouse input handling.
		private readonly Thumb _topLeft;
		private readonly Thumb _topRight;
		private readonly Thumb _bottomLeft;
		private readonly Thumb _bottomRight;

		private readonly Thumb _middleLeft;
		private readonly Thumb _middleRight;
		private readonly Thumb _middleBottom;
		private readonly Thumb _middleTop;

		private readonly Thumb _centerDrag;

		private readonly Thumb _rotate;

		private readonly RotateTransform _rotateTransform;
		private readonly RotateTransform _reverseRotateTransform;

		private Point _rotationCenter;

		/// <summary>
		/// Gets or sets the vertex for rotating points.
		/// </summary>
		public Point RotationCenter 
		{
			get
			{
				return _rotationCenter;
			}
			set
			{
				_rotationCenter = value;
				UpdateRotationTransform();
			}
		}

		// To store and manage the adorner’s visual children.
		private readonly VisualCollection _visualChildren;
		private Rect _bounds;
		private double _rotationAngle;
		private Point _dragStart;
		FrameworkElement _frameworkElement;

		private readonly IResizeable _vm;
		// Initialize the ResizingAdorner.
		public ResizeAdorner(double rotationAngle, FrameworkElement frameworkElement, Canvas adornedElement, IResizeable vm, Rect bounds)
			: base(adornedElement)
		{
			_vm = vm;
			_bounds = bounds;

			_rotationAngle = rotationAngle;

			_frameworkElement = frameworkElement;

			_visualChildren = new VisualCollection(this);

			// Call a helper method to initialize the Thumbs
			// with a customized cursors.
			BuildAdornerCorner(ref _topLeft, Cursors.SizeNWSE);
			BuildAdornerCorner(ref _topRight, Cursors.SizeNESW);
			BuildAdornerCorner(ref _bottomLeft, Cursors.SizeNESW);
			BuildAdornerCorner(ref _bottomRight, Cursors.SizeNWSE);

			BuildAdornerCorner(ref _middleTop, Cursors.SizeNS);
			BuildAdornerCorner(ref _middleRight, Cursors.SizeWE);
			BuildAdornerCorner(ref _middleBottom, Cursors.SizeNS);
			BuildAdornerCorner(ref _middleLeft, Cursors.SizeWE);

			BuildAdornerCorner(ref _centerDrag, Cursors.SizeAll);

			Style s = (Style)_frameworkElement.FindResource("RotateThumbStyle");
			BuildAdornerCorner(ref _rotate, Cursors.Hand, s);

			// Add handlers for resizing.
			_bottomLeft.DragDelta += HandleBottomLeft;
			_bottomRight.DragDelta += HandleBottomRight;
			_topLeft.DragDelta += HandleTopLeft;
			_topRight.DragDelta += HandleTopRight;

			_bottomLeft.DragCompleted += DragCompleted;
			_bottomRight.DragCompleted += DragCompleted;
			_topLeft.DragCompleted += DragCompleted;
			_topRight.DragCompleted += DragCompleted;

			_middleTop.DragDelta += HandleMiddleTop;
			_middleRight.DragDelta += HandleMiddleRight;
			_middleTop.DragCompleted += DragCompleted;
			_middleRight.DragCompleted += DragCompleted;

			_middleBottom.DragDelta += HandleMiddleBottom;
			_middleLeft.DragDelta += HandleMiddleLeft;
			_middleLeft.DragCompleted += DragCompleted;
			_middleBottom.DragCompleted += DragCompleted;

			_centerDrag.DragDelta += HandleCenterDrag;
			_centerDrag.DragStarted += _centerDrag_DragStarted;
			
			_rotate.DragDelta += HandleRotate;
			_rotate.DragCompleted += DragCompleted;

			_rotationCenter = Center(Bounds);
			_rotateTransform = new RotateTransform(_rotationAngle, _rotationCenter.X, _rotationCenter.Y);
			_reverseRotateTransform = new RotateTransform(-_rotationAngle, _rotationCenter.X, _rotationCenter.Y);
		}

		private void DragCompleted(object sender, DragCompletedEventArgs e)
		{
			_vm.DoneRotating();
		}
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			e.Handled = false;
		}

		public Rect Bounds
		{
			get { return _bounds; }
			set
			{
				if (_rotate != null && !_rotate.IsDragging)
				{
					_bounds = value;
					InvalidateArrange();
					InvalidateMeasure();
					InvalidateVisual();
				}
			}
		}

		public static Point Center(Rect rect)
		{
			return new Point(rect.Left + rect.Width / 2,
				rect.Top + rect.Height / 2);
		}

		private void _centerDrag_DragStarted(object sender, DragStartedEventArgs e)
		{
			Point pos = Mouse.GetPosition(AdornedElement);
			_dragStart = pos;
		}

		private void HandleCenterDrag(object sender, DragDeltaEventArgs e)
		{
			FrameworkElement el = AdornedElement as FrameworkElement;
			Point pos = Mouse.GetPosition(AdornedElement);
			
			var offset = new Point(pos.X - _dragStart.X, pos.Y - _dragStart.Y);
			
			// If the move operation is valid then...
			if (_vm.IsMoveable(offset))
			{
				_dragStart = pos;
				var transform = new TranslateTransform(offset.X, offset.Y);
				MoveTransformItems(transform);
			}
		}

		/// <summary>
		/// Returns the height of the bounds while
		/// guarding against scaling with a zero denominator.
		/// </summary>
		/// <returns>Y-Axis scale factor denominator</returns>
		private double GetYScaleDenominator()
		{
			double yDenominator = Bounds.Height;
			if (yDenominator == 0.0)
			{
				yDenominator = 1.0;
			}

			return yDenominator;
		}

		/// <summary>
		/// Returns the width of the bounds while
		/// guarding against scaling with a zero denominator.
		/// </summary>
		/// <returns>x-Axis scale factor denominator</returns>
		private double GetXScaleDenominator()
		{
			double xDenominator = Bounds.Width;
			if (xDenominator == 0.0)
			{
				xDenominator = 1.0;
			}

			return xDenominator;
		}

		// Handler for resizing from the bottom-right.
		private void HandleBottomRight(object sender, DragDeltaEventArgs args)
		{
			var scaleY = args.VerticalChange / GetYScaleDenominator();
			var scaleX = args.HorizontalChange / GetXScaleDenominator(); 

			scaleY += 1;
			scaleX += 1;

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				scaleX = scaleY = Math.Max(scaleX, scaleY);
			}

			ScaleTransform t = new ScaleTransform(scaleX, scaleY, Bounds.TopLeft.X, Bounds.TopLeft.Y);

			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				TransformItems(t);
			}
		}

		// Handler for resizing from the top-right.
		private void HandleTopRight(object sender, DragDeltaEventArgs args)
		{
			var scaleY = -args.VerticalChange / GetYScaleDenominator();
			var scaleX = args.HorizontalChange / GetXScaleDenominator();

			scaleY += 1;
			scaleX += 1;

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				scaleX = scaleY = Math.Max(scaleX, scaleY);
			}
			ScaleTransform t = new ScaleTransform(scaleX, scaleY, Bounds.BottomLeft.X, Bounds.BottomLeft.Y);

			// If the transform is valid then...
			if (IsTransformValid(t))
			{ 
				TransformItems(t);				
			}
		}

		// Handler for resizing from the top-left.
		private void HandleTopLeft(object sender, DragDeltaEventArgs args)
		{
			var scaleY = -args.VerticalChange / GetYScaleDenominator();
			var scaleX = -args.HorizontalChange / GetXScaleDenominator();

			scaleY += 1;
			scaleX += 1;

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				scaleX = scaleY = Math.Max(scaleX, scaleY);
			}
			
			ScaleTransform t = new ScaleTransform(scaleX, scaleY, Bounds.BottomRight.X, Bounds.BottomRight.Y);
			
			// If the transform is valid then...
			if (IsTransformValid(t))
			{				
				TransformItems(t);
			}
		}

		// Handler for resizing from the bottom-left.
		private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
		{
			var scaleY = args.VerticalChange / GetYScaleDenominator();
			var scaleX = -args.HorizontalChange / GetXScaleDenominator();

			scaleY += 1;
			scaleX += 1;

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				scaleX = scaleY = Math.Max(scaleX, scaleY);
			}
			ScaleTransform t = new ScaleTransform(scaleX, scaleY, Bounds.TopRight.X, Bounds.TopRight.Y);
			
			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				
				TransformItems(t);
			}
		}

		private void HandleMiddleLeft(object sender, DragDeltaEventArgs args)
		{
			var scaleX = -args.HorizontalChange / GetXScaleDenominator();
			scaleX += 1;

			var centerPoint = new Point(Bounds.Right, (Bounds.Bottom - Bounds.Top) / 2);
			ScaleTransform t = new ScaleTransform(scaleX, 1, centerPoint.X, centerPoint.Y);
			
			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				TransformItems(t);
			}
		}

		private void HandleMiddleBottom(object sender, DragDeltaEventArgs args)
		{
			double scaleY = args.VerticalChange / GetYScaleDenominator();

			scaleY += 1;

			var centerPoint = new Point((Bounds.Right - Bounds.Left) / 2, Bounds.Top);
			ScaleTransform t = new ScaleTransform(1, scaleY, centerPoint.X, centerPoint.Y);
			
			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				TransformItems(t);
			}
		}

		private void HandleMiddleRight(object sender, DragDeltaEventArgs args)
		{
			var scaleX = args.HorizontalChange / GetXScaleDenominator();

			scaleX += 1;

			var centerPoint = new Point(Bounds.Left, (Bounds.Bottom - Bounds.Top) / 2);
			var t = new ScaleTransform(scaleX, 1, centerPoint.X, centerPoint.Y);

			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				TransformItems(t);
			}
		}

		private void HandleMiddleTop(object sender, DragDeltaEventArgs args)
		{
			double scaleY = -args.VerticalChange / GetYScaleDenominator();
			scaleY += 1;
			
			var centerPoint = new Point((Bounds.Right - Bounds.Left) / 2, Bounds.Bottom);
			ScaleTransform t = new ScaleTransform(1, scaleY, centerPoint.X, centerPoint.Y);
				
			// If the transform is valid then...
			if (IsTransformValid(t))
			{
				TransformItems(t);
			}
		}

		private void HandleRotate(object sender, DragDeltaEventArgs e)
		{
			Point pos = Mouse.GetPosition(AdornedElement);

			double rotationAngle = GetAngle(_rotationCenter, pos);
			
			var difference = rotationAngle - _rotateTransform.Angle;
		
			if (_vm.IsRotateable(difference, _rotationCenter))
			{
				_rotationAngle = GetAngle(_rotationCenter, pos);
				_rotateTransform.Angle = _rotationAngle;
				_reverseRotateTransform.Angle = -_rotationAngle;

				_vm.RotateSelectedItems(difference, _rotationCenter);
			}		
			else
			{
				_rotate.CancelDrag();
			}
		}

		private Rect TransformItems(ScaleTransform scaleTransform, Rect bounds)
		{
			bounds = scaleTransform.TransformBounds(bounds);
			
			return bounds;
		}

		private bool IsTransformValid(ScaleTransform scaleTransform)
		{
			TransformGroup tg = new TransformGroup();
			tg.Children.Add(_reverseRotateTransform);
			tg.Children.Add(scaleTransform);
			tg.Children.Add(_rotateTransform);

			return _vm.IsTransformValid(tg);
		}

		private void TransformItems(ScaleTransform scaleTransform)
		{
			Bounds = scaleTransform.TransformBounds(Bounds);

			TransformGroup tg = new TransformGroup();
			tg.Children.Add(_reverseRotateTransform);
			tg.Children.Add(scaleTransform);
			tg.Children.Add(_rotateTransform); 
			_vm.TransformSelectedItems(tg);

			// If we haven't rotated yet, then it is safe to update the rotation center
			if (_rotationAngle == 0)
			{
				RotationCenter = Center(Bounds);
			}
		}

		private void MoveTransformItems(Transform transform)
		{
			_rotationCenter = transform.Transform(_rotationCenter);
			UpdateRotationTransform();
			Bounds = transform.TransformBounds(Bounds);
			_vm.MoveSelectedItems(transform);
		}

		private void UpdateRotationTransform()
		{
			_rotateTransform.CenterX = _reverseRotateTransform.CenterX = _rotationCenter.X;
			_rotateTransform.CenterY = _reverseRotateTransform.CenterY = _rotationCenter.Y;
		}

		/// <summary>
		/// Fetches angle relative to screen center point
		/// </summary>
		/// <param name="screenPoint"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		private static double GetAngle(Point screenPoint, Point center)
		{
			double dx = screenPoint.X - center.X;
			double dy = screenPoint.Y - center.Y;

			double inRads = Math.Atan2(dy, dx);

			// Convert from radians and adjust the angle from the 0 at 9:00 positon.
			return (inRads * (180 / Math.PI) + 270) % 360;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			AdornedElement.Measure(constraint);
			InvalidateVisual();
			return new Size(Bounds.Width + _topLeft.Width + _topRight.Width, Bounds.Height + _bottomLeft.Height + (3 * _rotate.Height)); ;
		}

		// Arrange the Adorners.
		protected override Size ArrangeOverride(Size finalSize)
		{
			if (_rotate.IsDragging) return finalSize;
			_topLeft.Arrange(new Rect(Bounds.X - _topLeft.Width, Bounds.Y - _topLeft.Height, _topLeft.Width, _topLeft.Height));
			_topRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y - _topRight.Height, _topRight.Width, _topRight.Height));

			_bottomLeft.Arrange(new Rect(Bounds.X - _bottomLeft.Width, Bounds.Y + Bounds.Height, _bottomLeft.Width, _bottomLeft.Height));
			_bottomRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height, _bottomRight.Width, _bottomRight.Height));

			_middleTop.Arrange(new Rect(Bounds.X + Bounds.Width / 2 - _middleTop.Width / 2, Bounds.Y - _middleTop.Height, _middleTop.Width, _middleTop.Height));
			_middleRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height / 2 - _middleRight.Height / 2, _middleRight.Width, _middleRight.Height));
			_middleBottom.Arrange(new Rect(Bounds.X + Bounds.Width / 2 - _middleBottom.Width / 2, Bounds.Y + Bounds.Height, _middleBottom.Width, _middleBottom.Height));
			_middleLeft.Arrange(new Rect(Bounds.X - _middleLeft.Width, Bounds.Y + Bounds.Height / 2 - _middleLeft.Width / 2, _middleLeft.Width, _middleLeft.Height));

			_centerDrag.Arrange(new Rect(Bounds.X + Bounds.Width/2 - _centerDrag.Width / 2, Bounds.Y + Bounds.Height/2 - _centerDrag.Height / 2, _centerDrag.Width, _centerDrag.Height));
			//_centerDrag.Arrange(new Rect(_rotationCenter.X - _centerDrag.Width / 2, _rotationCenter.Y - _centerDrag.Height / 2, _centerDrag.Width, _centerDrag.Height));

			_rotate.Arrange(new Rect(_rotationCenter.X - _rotate.Width / 2, Bounds.Y - 3 * _rotate.Height, _rotate.Width, _rotate.Height));

			return finalSize;
		}

		// Helper method to instantiate the corner Thumbs, set the Cursor property, 
		// set some appearance properties, and add the elements to the visual tree.
		private void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor, Style s = null)
		{
			if (cornerThumb != null) return;

			cornerThumb = new Thumb();

			if (s != null)
			{
				cornerThumb.Style = s;
			}
			else
			{
				cornerThumb.Style = (Style)_frameworkElement.FindResource("ResizeThumbStyle");
			}

			// Set some arbitrary visual characteristics.
			cornerThumb.Cursor = customizedCursor;
			cornerThumb.Height = cornerThumb.Width = 10;
			
			_visualChildren.Add(cornerThumb);
		}

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			GeneralTransformGroup result = new GeneralTransformGroup();
			result.Children.Add(_rotateTransform);
			result.Children.Add(transform);

			return result;
		}

		// Override the VisualChildrenCount and GetVisualChild properties to interface with 
		// the adorner’s visual collection.
		protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
		protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
	}
}
