using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Common.Controls.Direct2D;
 

  namespace VixenModules.Preview.VixenPreview.Direct2D
{
    /// <summary>Hosts a <see cref="Scene"/> instance.</summary>
    public sealed class Direct2DControl : FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="Scene"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SceneProperty =
            DependencyProperty.Register("Scene", typeof(Scene), typeof(Direct2DControl), new UIPropertyMetadata(SceneChangedCallback));

        // To prevent lots or resizing, we're going to use a timer to resize the
        // image after a set interval. We can then reset the timer if we recieve
        // another request to resize, helping performance. When resizing, the
        // Image control will scale the old contents for us, which will be blurry
        // but it's only for a little time so should be unnoticeable.
        private static readonly TimeSpan ResizeInterval = TimeSpan.FromMilliseconds(100);
        private DispatcherTimer resizeTimer;

        private Image image;
        private D3D10Image imageSource;
        private bool isDirty;

        /// <summary>
        /// Initializes a new instance of the Direct2DControl class.
        /// </summary>
        public Direct2DControl()
        {
            this.resizeTimer = new DispatcherTimer();
            this.resizeTimer.Interval = ResizeInterval;
            this.resizeTimer.Tick += this.ResizeTimerTick;

            this.imageSource = new  D3D10Image();
            this.imageSource.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            this.image = new Image();
            this.image.Stretch = Stretch.Fill; // We set this because our resizing isn't immediate
            this.image.Source = this.imageSource;
            this.AddVisualChild(this.image);

            // To greatly reduce flickering we're only going to AddDirtyRect
            // when WPF is rendering.
            CompositionTarget.Rendering += this.CompositionTargetRendering;
        }

        /// <summary>
        /// Gets or sets the <see cref="Direct2D.Scene"/> object displayed
        /// by this control.
        /// </summary>
        /// <remarks>
        /// The caller is resposible for the lifetime management of the Scene.
        /// </remarks>
		public AnimatedScene Scene
        {
			get { return (AnimatedScene)this.GetValue(SceneProperty); }
            set { this.SetValue(SceneProperty, value); }
        }


        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <remarks>
        /// This will always return 1 as this control hosts a single
        /// Image control.
        /// </remarks>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>Arranges and sizes the child Image control.</summary>
        /// <param name="finalSize">The size used to arrange the control.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            image.Arrange(new Rect(0, 0, size.Width, size.Height));
            return size;
        }

        /// <summary>Returns the child Image control.</summary>
        /// <param name="index">
        /// The zero-based index of the requested child element in the collection.
        /// </param>
        /// <returns>The child Image control.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero or greater than VisualChildrenCount.
        /// </exception>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return this.image;
        }

        /// <summary>
        /// Updates the UIElement.DesiredSize of the child Image control.
        /// </summary>
        /// <param name="availableSize">The size that the control should not exceed.</param>
        /// <returns>The child Image's desired size.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.image.Measure(availableSize);
            return this.image.DesiredSize;
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="sizeInfo">
        /// The packaged parameters, which includes old and new sizes, and which
        /// dimension actually changes.
        /// </param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (this.Scene != null)
            {
                // Signal to resize
                this.resizeTimer.Start();
            }
        }

        private static void SceneChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (Direct2DControl)d;

            // Unsubscribe from the old scene first
            if (e.OldValue != null)
            {
                ((Scene)e.OldValue).Updated -= instance.SceneUpdated;
            }

            instance.OnSceneChanged();

            // Now subscribe to the events once all the resources have been created
            if (e.NewValue != null)
            {
                ((Scene)e.NewValue).Updated += instance.SceneUpdated;
            }
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.isDirty)
            {
                this.isDirty = false;
                if (this.imageSource.IsFrontBufferAvailable)
                {
                    this.imageSource.Lock();
                    this.imageSource.AddDirtyRect(new Int32Rect(0, 0, this.imageSource.PixelWidth, this.imageSource.PixelHeight));
                    this.imageSource.Unlock();
                }
            }
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Scene != null)
            {
                if (this.imageSource.IsFrontBufferAvailable)
                {
                    this.OnSceneChanged(); // Recreate the resources
                }
                else
                {
                    this.Scene.FreeResources();
                }
            }
        }

        private void OnSceneChanged()
        {
            this.imageSource.Lock();
            try
            {
                if (this.Scene != null)
                {
                    this.Scene.CreateResources();

                    // Resize to the size of this control, if we have a size
                    int width = Math.Max(1, (int)this.ActualWidth);
                    int height = Math.Max(1, (int)this.ActualHeight);
                    this.Scene.Resize(width, height);

                    this.imageSource.SetBackBuffer(this.Scene.Texture);
                    this.Scene.Render();
                }
                else
                {
                    this.imageSource.SetBackBuffer(null);
                }
            }
            finally
            {
                this.imageSource.Unlock();
            }
        }

        private void ResizeTimerTick(object sender, EventArgs e)
        {
            this.resizeTimer.Stop(); // Only call this method once
            if (this.Scene != null)
            {
                // Check we don't resize too small
                int width = Math.Max(1, (int)this.ActualWidth);
                int height = Math.Max(1, (int)this.ActualHeight);
                this.Scene.Resize(width, height);

                this.imageSource.Lock();
                this.imageSource.SetBackBuffer(this.Scene.Texture);
                this.imageSource.Unlock();

                this.Scene.Render();
            }
        }

        private void SceneUpdated(object sender, EventArgs e)
        {
            this.isDirty = true;
        }
    }
}
