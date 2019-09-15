using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VixenModules.Editor.EffectEditor.Controls
{
    public class SliderPoint : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
    {
        private const double DefaultWidth = 10;
        private const double DefaultHeight = 10;
	    private double _normalizedPosition;

		private Point _dragStartPoint;
		private const double DragTolerance = 2.0;
		private bool _mouseDown;
	    private ToolTip _toolTip;

		internal Polygon SliderShape { get; private set; }
        internal double Left
        {
            get { return (double)SliderShape.GetValue(Canvas.LeftProperty); }
            set
            {
	            if (double.IsInfinity(value)) return;
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
				if (double.IsInfinity(value)) return;
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
		    get { return _normalizedPosition; }
		    set
		    {
			    if (value >= 0 && value <= 1)
			    {
					_normalizedPosition = value;
					if (Parent != null)
					{
						Center = value * Parent.Width;
					}
					
					SetToolTip();
				}
		    }
	    }

	    public bool ShowToolTip { get; set; }

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
	    /// <param name="showToolTip"></param>
	    public SliderPoint(double position, Panel parent, bool showToolTip=false)
        {
            Parent = parent;
	        ShowToolTip = showToolTip;
	        SliderShape = new Polygon { Width = DefaultWidth, Height = DefaultHeight };
	        _normalizedPosition = position;
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
           // SliderShape.MouseDown += Handle_MouseDown;
			SliderShape.MouseLeftButtonDown += Handle_MouseLeftButtonDown;
            SliderShape.Focusable = true;
			SetToolTip();
        }

		

		private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//if (e.PreviousSize.Width == 0.0) return;
			//var relativePosition = Center / e.PreviousSize.Width;
			Center = _normalizedPosition * Parent.Width;
		}

        private void Handle_MouseMove(object sender, MouseEventArgs e)
        {
	        if (_mouseDown)
	        {
				var point = Mouse.GetPosition(Parent);
				Vector vector = point - _dragStartPoint;

				if (!IsDragging)
				{
					if (vector.Length > DragTolerance)
					{
						IsDragging = true;
						SliderShape.Focus();
						e.Handled = true;
					}
				}
				else
				{
					var center = Math.Round(point.X);
					if (center >= 0 && center <= Parent.Width)
					{
						Center = center;
						_normalizedPosition = center / Parent.Width;
						SliderShape.Focus();
						SetToolTip();
					}
				}
			}
			
        }

		private void Handle_MouseUp(object sender, MouseEventArgs e)
        {
	        if (IsDragging || _mouseDown)
	        {
				var poly = sender as Polygon;
				poly.ReleaseMouseCapture();
		        poly.Focus();
		        _mouseDown = false;
		        if (IsDragging)
		        {
			        IsDragging = false;
					e.Handled = true;
					OnDragCompleted(EventArgs.Empty);
		        }
		        else if(Keyboard.Modifiers == ModifierKeys.Alt)
		        {
					e.Handled = true;
					OnAltClick(EventArgs.Empty);
		        }

				DisableToolTip();
			}
            
		}

	    private void Handle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	    {
			if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount < 2)
			{
				var poly = sender as Polygon;
				poly.CaptureMouse();
				poly.Focus();
				_mouseDown = true;
				_dragStartPoint = e.GetPosition(Parent);
				EnableToolTip();
				e.Handled = true;
			}
		}

		public delegate void DragCompletedEventHandler(object sender, EventArgs e);

		public event DragCompletedEventHandler DragCompleted;
		protected virtual void OnDragCompleted(EventArgs e)
		{
			if (DragCompleted != null)
				DragCompleted(this, e);
		}

		public delegate void AltClickEventHandler(object sender, EventArgs e);
		public event AltClickEventHandler AltClick;
		protected virtual void OnAltClick(EventArgs e)
		{
			if (AltClick != null)
				AltClick(this, e);
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

	    private void SetToolTip()
	    {
		    if (ShowToolTip)
		    {
			    if (_toolTip == null)
			    {
				    _toolTip = new ToolTip
				    {
					    PlacementTarget = SliderShape,
					    Placement = PlacementMode.Bottom
				    };
				    SliderShape.ToolTip = _toolTip;
			    }

			    _toolTip.Content = string.Format("{0:0}", _normalizedPosition * 100.0);
				//This hack is to get the tooltip position to update.
			    _toolTip.HorizontalOffset += 1;
			    _toolTip.HorizontalOffset -= 1;
		    }
	    }

	    private void EnableToolTip()
	    {
		    if (_toolTip != null && ShowToolTip)
		    {
			    _toolTip.IsOpen = true;
		    }
	    }

	    private void DisableToolTip()
	    {
		    if (_toolTip != null)
		    {
			    _toolTip.IsOpen = false;
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
			    Parent.Children.Remove(SliderShape);
				SliderShape.MouseMove -= Handle_MouseMove;
				SliderShape.MouseUp -= Handle_MouseUp;
				//SliderShape.MouseDown -= Handle_MouseDown;
			    SliderShape.MouseLeftButtonDown -= Handle_MouseLeftButtonDown;
				Parent.SizeChanged -= Parent_SizeChanged;
			}
			catch { }
			ReleaseUnmanagedResources();
		    GC.SuppressFinalize(this);
	    }

	    ~SliderPoint()
	    {
		    ReleaseUnmanagedResources();
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
