using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VixenModules.Editor.EffectEditor.Controls
{
    public class SliderPoint : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private const double DefaultWidth = 10;
        private const double DefaultHeight = 10;
		
        internal Polygon SliderShape { get; private set; }
        internal double Left
        {
            get { return (double)SliderShape.GetValue(Canvas.LeftProperty); }
            set
            {
                var changing = value == Left;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Left"));
                SliderShape.SetValue(Canvas.LeftProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Left"));
            }
        }
        internal double Center
        {
            get { return Left + Width / 2; }
            set
            {
                var changing = value == Center;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Center"));
                Left = value - Width / 2;
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Center"));
            }
        }
        internal double Width
        {
            get
            {
                return (double)SliderShape.GetValue(FrameworkElement.WidthProperty);
            }
            set
            {
                var changing = value == SliderShape.Width;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Width"));
                SliderShape.SetValue(FrameworkElement.WidthProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Width"));
            }
        }
        internal double Height
        {
            get
            {
                return (double)SliderShape.GetValue(FrameworkElement.HeightProperty);
            }
            set
            {
                var changing = value == Width;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Height"));
                SliderShape.SetValue(FrameworkElement.HeightProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Height"));
            }
        }

	    public bool IsDragging { get; set; }
		
	    public Object Tag { get; set; }

	    public double NormalizedPosition
	    {
		    get { return Center / Parent.Width; }
	    }

        internal Panel Parent { get; set; }
        internal bool IsMouseCaptured { get { return (bool)SliderShape.GetValue(UIElement.IsMouseCapturedProperty); } }
        internal bool IsMouseOver { get { return (bool)SliderShape.GetValue(UIElement.IsMouseOverProperty); } }
        internal Style Style
        {
            get
            {
                return (Style)SliderShape.GetValue(FrameworkElement.StyleProperty);
            }
            set
            {
                var changing = value == Style;
                if (changing)
                    OnPropertyChanging(new PropertyChangingEventArgs("Style"));
                SliderShape.SetValue(FrameworkElement.StyleProperty, value);
                if (changing)
                    OnPropertyChanged(new PropertyChangedEventArgs("Style"));
            }
        }

		/// <summary>
		/// Creates a slider handle at the relative position within the parent panel
		/// </summary>
		/// <param name="position"></param>
		/// <param name="parent"></param>
        public SliderPoint(double position, Panel parent)
        {
            Parent = parent;
	        SliderShape = new Polygon { Width = DefaultWidth, Height = DefaultHeight };
	        if (parent != null)
	        {
		        Center = position * parent.Width;
				parent.SizeChanged += Parent_SizeChanged;
	        }
            SliderShape.Points.Add(new Point(0, SliderShape.Height));
			SliderShape.Points.Add(new Point(0, SliderShape.Height/2));
			SliderShape.Points.Add(new Point(SliderShape.Width/2, 0));
            SliderShape.Points.Add(new Point(SliderShape.Width, SliderShape.Height/2));
            SliderShape.Points.Add(new Point(SliderShape.Width, SliderShape.Height));
            SliderShape.MouseMove += Handle_MouseMove;
            SliderShape.MouseUp += Handle_MouseUp;
            SliderShape.MouseDown += Handle_MouseDown;
            SliderShape.Focusable = true;
        }

		private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var relativePosition = Center / e.PreviousSize.Width;
			Center = relativePosition * Parent.Width;
		}

		~SliderPoint()
        {
            try { Parent.Children.Remove(SliderShape); }
            catch { }
        }

        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
	        if (!IsDragging)
	        {
				IsDragging = true;
			}
            var point = Mouse.GetPosition(Parent);
            var center = point.X;
	        if (center >= 0 && center <= Parent.Width)
	        {
		        Center = center;
		        SliderShape.Focus();
	        }
        }

        private void Handle_MouseUp(object sender, MouseEventArgs e)
        {
            var poly = sender as Polygon;
	        poly.ReleaseMouseCapture();
			poly.Focus();
	        if (IsDragging)
	        {
				IsDragging = false;
				OnDragCompleted(EventArgs.Empty);
			}
		}

        private void Handle_MouseDown(object sender, MouseEventArgs e)
        {
            var poly = sender as Polygon;
            poly.CaptureMouse();
            poly.Focus();
        }

		public delegate void DragCompletedEventHandler(object sender, EventArgs e);

	    public event DragCompletedEventHandler DragCompleted;
		protected virtual void OnDragCompleted(EventArgs e)
		{
			if (DragCompleted != null)
				DragCompleted(this, e);
		}

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
    }

	public class SliderPointPositionComparer : IComparer<SliderPoint>
	{
		public int Compare(SliderPoint x, SliderPoint y)
		{
			if (x.Center > y.Center)
			{
				return 1;
			}

			if (x.Center < y.Center)
			{
				return -1;
			}

			return 0;
		}
	}
}
