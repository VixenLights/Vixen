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

        // To store and manage the adorner’s visual children.
        readonly VisualCollection _visualChildren;
        private Rect _bounds;
        private readonly RelayCommand<Transform> _transformCommand;

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

            // Add handlers for resizing.
            _bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            _bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            _topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            _topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);

           
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
                _bounds = value;
                InvalidateVisual();
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
            
            vm.ScaleSelectedItems(scaleX, scaleY, center);

            //FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            ////Thumb hitThumb = sender as Thumb;

            ////if (adornedElement == null || hitThumb == null) return;
            //FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            //// Ensure that the Width and Height are properly initialized after the resize.
            //EnforceSize(adornedElement);

            //// Change the size by the amount the user drags the mouse, as long as it’s larger 
            //// than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {

            var scaleY = -args.VerticalChange / Bounds.Height;
            var scaleX = args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);


            scaleY += 1;

            scaleX += 1;

            vm.ScaleSelectedItems(scaleX, scaleY, center);
            //FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            //Thumb hitThumb = sender as Thumb;

            //if (adornedElement == null || hitThumb == null) return;
            //FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            //// Ensure that the Width and Height are properly initialized after the resize.
            //EnforceSize(adornedElement);

            //// Change the size by the amount the user drags the mouse, as long as it’s larger 
            //// than the width or height of an adorner, respectively.
            //adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            ////adornedElement.Height = Math.Max(adornedElement.Height – args.VerticalChange, hitThumb.DesiredSize.Height);

            //double height_old = adornedElement.Height;
            //double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            //double top_old = Canvas.GetTop(adornedElement);
            //adornedElement.Height = height_new;
            //Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the top-left.
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {

            var scaleY = -args.VerticalChange / Bounds.Height;
            var scaleX = -args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);


            scaleY += 1;

            scaleX += 1;

            vm.ScaleSelectedItems(scaleX, scaleY, center);
            //FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            //Thumb hitThumb = sender as Thumb;

            //if (adornedElement == null || hitThumb == null) return;

            //// Ensure that the Width and Height are properly initialized after the resize.
            //EnforceSize(adornedElement);

            //// Change the size by the amount the user drags the mouse, as long as it’s larger 
            //// than the width or height of an adorner, respectively.
            ////adornedElement.Width = Math.Max(adornedElement.Width – args.HorizontalChange, hitThumb.DesiredSize.Width);
            ////adornedElement.Height = Math.Max(adornedElement.Height – args.VerticalChange, hitThumb.DesiredSize.Height);

            //double width_old = adornedElement.Width;
            //double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //double left_old = Canvas.GetLeft(adornedElement);
            //adornedElement.Width = width_new;
            //Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
            //double height_old = adornedElement.Height;
            //double height_new = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
            //double top_old = Canvas.GetTop(adornedElement);
            //adornedElement.Height = height_new;
            //Canvas.SetTop(adornedElement, top_old - (height_new - height_old));
        }

        // Handler for resizing from the bottom-left.
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {

            var scaleY = args.VerticalChange / Bounds.Height;
            var scaleX = -args.HorizontalChange / Bounds.Width;
            var center = Center(Bounds);


            scaleY += 1;

            scaleX += 1;

            vm.ScaleSelectedItems(scaleX, scaleY, center);
            //FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            //Thumb hitThumb = sender as Thumb;

            //if (adornedElement == null || hitThumb == null) return;

            //// Ensure that the Width and Height are properly initialized after the resize.
            //EnforceSize(adornedElement);

            //// Change the size by the amount the user drags the mouse, as long as it’s larger 
            //// than the width or height of an adorner, respectively.
            ////adornedElement.Width = Math.Max(adornedElement.Width – args.HorizontalChange, hitThumb.DesiredSize.Width);
            //adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);

            //double width_old = adornedElement.Width;
            //double width_new = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            //double left_old = Canvas.GetLeft(adornedElement);
            //adornedElement.Width = width_new;
            //Canvas.SetLeft(adornedElement, left_old - (width_new - width_old));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdornedElement.Measure(constraint);
            return new Size(Bounds.Width, Bounds.Height); ;
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {

            _topLeft.Arrange(new Rect(Bounds.X-_topLeft.Width, Bounds.Y-_topLeft.Height, _topLeft.Width, _topLeft.Height));
            _topRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y-_topRight.Height , _bottomRight.Width, _topRight.Height));
            _bottomLeft.Arrange(new Rect(Bounds.X-_bottomLeft.Width, Bounds.Y + Bounds.Height, _bottomLeft.Width, _bottomLeft.Height));
            _bottomRight.Arrange(new Rect(Bounds.X + Bounds.Width, Bounds.Y+Bounds.Height, _bottomRight.Width,_bottomRight.Height));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

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
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner’s visual collection.
        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
    }
}
