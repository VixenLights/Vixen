using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Common.WPFCommon.Command;
using VixenModules.App.CustomPropEditor.Controls;
using VixenModules.App.CustomPropEditor.ViewModel;
using Size = System.Windows.Size;

namespace VixenModules.App.CustomPropEditor.Adorners
{
    public class ResizeAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        readonly Thumb _topLeft;
        readonly Thumb _topRight;
        readonly Thumb _bottomLeft;
        readonly Thumb _bottomRight;

        readonly Thumb _middleLeft;
        readonly Thumb _middleRight;
        readonly Thumb _middleBottom;
        readonly Thumb _middleTop;

        readonly Thumb _rotate;

        private RotateTransform rt;

        // To store and manage the adorner’s visual children.
        readonly VisualCollection _visualChildren;
        private Rect _bounds;
        private double _rotationAngle = 0;

        private bool _done;
        
        private readonly PropDesigner vm;
        // Initialize the ResizingAdorner.
        public ResizeAdorner(Canvas adornedElement, PropDesigner vm)
            : base(adornedElement)
        {
            this.vm = vm;
            Bounds = new Rect(0,0,adornedElement.ActualWidth, adornedElement.ActualHeight);
           
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

            Style s = (Style) vm.FindResource("RotateThumbStyle");
            BuildAdornerCorner(ref _rotate, Cursors.Hand, s);

            // Add handlers for resizing.
            _bottomLeft.DragDelta += HandleBottomLeft;
            _bottomRight.DragDelta += HandleBottomRight;
            _topLeft.DragDelta += HandleTopLeft;
            _topRight.DragDelta += HandleTopRight;

            _middleTop.DragDelta += _middleTop_DragDelta;
            _middleRight.DragDelta += _middleRight_DragDelta;
            _middleBottom.DragDelta += _middleBottom_DragDelta;
            _middleLeft.DragDelta += _middleLeft_DragDelta;

            _rotate.DragDelta += _rotate_DragDelta;

            rt = new RotateTransform(_rotationAngle, Center(Bounds).Y, Center(Bounds).Y);

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
                if (_rotate!= null && !_rotate.IsDragging)
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

        // Handler for resizing from the bottom-right.
        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            var scaleY = args.VerticalChange / Bounds.Height;
            var scaleX = args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);

            scaleY += 1;
            scaleX += 1;

            if (EnforceSize(scaleX, scaleY))
            {
                vm.ScaleSelectedItems(scaleX, scaleY, center);
            }
            
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            var scaleY = -args.VerticalChange / Bounds.Height;
            var scaleX = args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);

            scaleY += 1;
            scaleX += 1;

            if (EnforceSize(scaleX, scaleY))
            {
                vm.ScaleSelectedItems(scaleX, scaleY, center);
            }
            
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {

            var scaleY = -args.VerticalChange / Bounds.Height;
            var scaleX = -args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);
            
            scaleY += 1;
            scaleX += 1;

            if (EnforceSize(scaleX, scaleY))
            {
                vm.ScaleSelectedItems(scaleX, scaleY, center);
            }
            
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            var scaleY = args.VerticalChange / Bounds.Height;
            var scaleX = -args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);

            scaleY += 1;
            scaleX += 1;

            if (EnforceSize(scaleX, scaleY))
            {
                vm.ScaleSelectedItems(scaleX, scaleY, center);
            }
        }

        private void _middleLeft_DragDelta(object sender, DragDeltaEventArgs args)
        {
            var scaleX = -args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);

            scaleX += 1;

            if (EnforceSize(scaleX, 1))
            {
                vm.ScaleSelectedItems(scaleX, 1, center);
            }
        }

        private void _middleBottom_DragDelta(object sender, DragDeltaEventArgs args)
        {
            var scaleY = args.VerticalChange / Bounds.Height;
            var center = Center(Bounds);

            scaleY += 1;

            if (EnforceSize(1, scaleY))
            {
                vm.ScaleSelectedItems(1, scaleY, center);
            }
        }

        private void _middleRight_DragDelta(object sender, DragDeltaEventArgs args)
        {
            var scaleX = args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);

            scaleX += 1;

            if (EnforceSize(scaleX, 1))
            {
                vm.ScaleSelectedItems(scaleX, 1, center);
            }
        }

        private void _middleTop_DragDelta(object sender, DragDeltaEventArgs args)
        {
            var scaleY = -args.VerticalChange / Bounds.Height;
            var center = Center(Bounds);

            scaleY += 1;

            if (EnforceSize(1, scaleY))
            {
                vm.ScaleSelectedItems(1, scaleY, center);
            }
            
        }
        private void _rotate_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);

            var center = Center(Bounds);

            //double deltaX = pos.X - center.X;
            //double deltaY = pos.Y - center.Y;

            double angle = GetAngleDegree(center, pos.X, pos.Y);

            angle = -Vector.AngleBetween(new Vector(center.X, center.Y), new Vector(pos.X, pos.Y));
            Console.Out.WriteLine(angle);
            _rotationAngle += angle;
            _rotationAngle = _rotationAngle % 360;

            rt.Angle = _rotationAngle;
            vm.RotateSelectedItems(angle, center);
            
        }

        protected static double GetAngleDegree(Point origin, double x, double y)
        {
            var n = 270 - (Math.Atan2(origin.Y - y, origin.X - x)) * 180 / Math.PI;
            return n % 360;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdornedElement.Measure(constraint);
            InvalidateVisual();
            return new Size(Bounds.Width, Bounds.Height); ;
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            var center = Center(Bounds);
            Console.Out.WriteLine(_rotationAngle);

            rt.Angle = _rotationAngle;
            rt.CenterX = center.X;
            rt.CenterY = center.Y;
            _topLeft.Arrange(new Rect(Bounds.X-_topLeft.Width, Bounds.Y-_topLeft.Height, _topLeft.Width, _topLeft.Height));

           
            _topRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y-_topRight.Height , _bottomRight.Width, _topRight.Height));
            
            _bottomLeft.Arrange(new Rect(Bounds.X-_bottomLeft.Width, Bounds.Y + Bounds.Height, _bottomLeft.Width, _bottomLeft.Height));
            _bottomRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y+Bounds.Height, _bottomRight.Width,_bottomRight.Height));

            _middleTop.Arrange(new Rect(Bounds.X +Bounds.Width /2 - _middleTop.Width/2, Bounds.Y - _middleTop.Height, _middleTop.Width, _middleTop.Height));
            _middleRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height/2 - _middleRight.Height/2, _bottomRight.Width, _topRight.Height));
            _middleBottom.Arrange(new Rect(Bounds.X + Bounds.Width / 2 - _middleBottom.Width / 2, Bounds.Y + Bounds.Height, _bottomLeft.Width, _bottomLeft.Height));
            _middleLeft.Arrange(new Rect(Bounds.X - _topLeft.Width, Bounds.Y + Bounds.Height/2 - _middleLeft.Width/2, _middleBottom.Width, _middleBottom.Height));

            _rotate.Arrange(new Rect(Bounds.X + Bounds.Width / 2 - _middleTop.Width / 2, Bounds.Y - 3 * _middleTop.Height , _middleTop.Width, _middleTop.Height));
            //_rotate.RenderTransform = new RotateTransform(_rotationAngle, center.X, center.Y);
            //_rotate.RenderTransformOrigin = center;
            // Return the final size.

            

            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor, Style s = null)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            if (s != null)
            {
                cornerThumb.Style = s;
            }

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.MediumBlue);

            _visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        private bool EnforceSize(double scaleX, double scaleY)
        {

            var el = AdornedElement as FrameworkElement;
            if (el != null)
            {
                Rect r = new Rect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
                r.Scale(scaleX, scaleY);
                if (r.X >= 0 && r.Y >= 0 && r.Width <= r.X + el.Width && r.Y + r.Height <= el.Height)
                {
                    return true;
                }
            }

            return false;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            Console.Out.WriteLine("Get Transform");
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(transform);
            result.Children.Add(rt);
            return result;

        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner’s visual collection.
        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
    }
}
