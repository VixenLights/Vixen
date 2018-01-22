using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Controls
{
    public class LightShape : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
    {
       
        public Path PathShape { get; private set; }

        internal Point Center
        {
            get { return (Point)PathShape.Data.GetValue(EllipseGeometry.CenterProperty); }
            set
            {
                if (value.X < 0 || value.X > Parent.Width || value.Y < 0 || value.Y > Parent.Height)
                {
                    return;
                }
                
                var changing = value.Equals(Center);
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Center"));
                PathShape.Data.SetValue(EllipseGeometry.CenterProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Center"));
            }
        }

        internal double Radius
        {
            get { return (double)PathShape.Data.GetValue(EllipseGeometry.RadiusXProperty); }
            set
            {
                if (double.IsInfinity(value)) return;
                var changing = value == Radius;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Radius"));
                PathShape.Data.SetValue(EllipseGeometry.RadiusXProperty, value);
                PathShape.Data.SetValue(EllipseGeometry.RadiusYProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Radius"));
            }
        }

        /// <summary>
        /// Creates a Light at the position within the parent panel
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="parent"></param>
        public LightShape(Point position, double radius, Panel parent)
        {
            Parent = parent;
         
            PathShape = new Path() { Data = new EllipseGeometry(position, radius, radius), StrokeThickness = 1, Fill = Brushes.White};
            
            if (parent != null)
            {
                
                parent.SizeChanged += Parent_SizeChanged;
            }

            PathShape.MouseMove += Handle_MouseMove;
            PathShape.MouseUp += Handle_MouseUp;
            // SliderShape.MouseDown += Handle_MouseDown;
            PathShape.MouseLeftButtonDown += Handle_MouseLeftButtonDown;
            PathShape.Focusable = true;
           
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Check to verify we are on the panel
        }

        internal Panel Parent { get; set; }
        internal bool IsMouseCaptured { get { return (bool)PathShape.GetValue(UIElement.IsMouseCapturedProperty); } }
        internal bool IsMouseOver { get { return (bool)PathShape.GetValue(UIElement.IsMouseOverProperty); } }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }

        #endregion  

        public delegate void DragCompletedEventHandler(object sender, EventArgs e);

        public event DragCompletedEventHandler DragCompleted;
        protected virtual void OnDragCompleted(EventArgs e)
        {
            if (DragCompleted != null)
                DragCompleted(this, e);
        }

        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
           
            //if (_mouseDown)
            //{
            //    var point = Mouse.GetPosition(Parent);
            //    Vector vector = point - _dragStartPoint;

            //    if (!IsDragging)
            //    {
            //        if (vector.Length > DragTolerance)
            //        {
            //            IsDragging = true;
            //            SliderShape.Focus();
            //            e.Handled = true;
            //        }
            //    }
            //    else
            //    {
            //        var center = Math.Round(point.X);
            //        if (center >= 0 && center <= Parent.Width)
            //        {
            //            Center = center;
            //            _normalizedPosition = center / Parent.Width;
            //            SliderShape.Focus();
            //            SetToolTip();
            //        }
            //    }
            //}

        }

        private void Handle_MouseUp(object sender, MouseEventArgs e)
        {
            //if (IsDragging || _mouseDown)
            //{
            //    var poly = sender as Polygon;
            //    poly.ReleaseMouseCapture();
            //    poly.Focus();
            //    _mouseDown = false;
            //    if (IsDragging)
            //    {
            //        IsDragging = false;
            //        e.Handled = true;
            //        OnDragCompleted(EventArgs.Empty);
            //    }
            //    else if (Keyboard.Modifiers == ModifierKeys.Alt)
            //    {
            //        e.Handled = true;
            //        OnAltClick(EventArgs.Empty);
            //    }

 
            //}

        }

        private void Handle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount < 2)
            {
                //var poly = sender as Ellipse;
                //poly.CaptureMouse();
                //poly.Focus();
                //_mouseDown = true;
                //_dragStartPoint = e.GetPosition(Parent);
                //EnableToolTip();
                //e.Handled = true;
            }
        }


        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        public void Dispose()
        {

            try
            {
                //Parent.Children.Remove(SliderShape);
                //SliderShape.MouseMove -= Handle_MouseMove;
                //SliderShape.MouseUp -= Handle_MouseUp;
                ////SliderShape.MouseDown -= Handle_MouseDown;
                //SliderShape.MouseLeftButtonDown -= Handle_MouseLeftButtonDown;
                //Parent.SizeChanged -= Parent_SizeChanged;
            }
            catch { }
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~LightShape()
        {
            ReleaseUnmanagedResources();
        }

     
    }
}
