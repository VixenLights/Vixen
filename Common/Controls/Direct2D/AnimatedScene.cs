using System;
using System.Windows.Threading;

namespace Common.Controls.Direct2D
{
    /// <summary>
    /// Allows drawing of a Scene at a specified Frame Per Second.
    /// </summary>
    public abstract class AnimatedScene : Scene
    {
        private bool isAnimating;
        private DateTime lastUpdate;
        private DispatcherTimer timer;

        /// <summary>
        /// Initializes a new instance of the AnimatedScene class.
        /// </summary>
        /// <param name="desiredFps">
        /// The desired number of frames to render per second.
        /// </param>
        /// <remarks>
        /// The desiredFps parameter must be greater than zero. Setting the value to
        /// <see cref="Int32.MaxValue">int.MaxValue</see> will cause the control to
        /// update as often as possible.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// desiredFps is less than or equal to zero.
        /// </exception>
        protected AnimatedScene(int desiredFps)
        {
            if (desiredFps <= 0)
            {
                throw new ArgumentOutOfRangeException("desiredFps", "Value must be greater than zero.");
            }

            // We'll set the timer to a low priority so everything else is
            // processed before we render.
            this.timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            this.timer.Tick += this.TimerTick;

            // Calculate the number of times per second to render. Any large values
            // will cause the interval to be zero, causing the timer to execute
            // every time the Dispatcher queue is processed.
            this.timer.Interval = TimeSpan.FromMilliseconds(1000 / desiredFps);
        }

        public bool IsAnimating
        {
            get
            {
                return this.isAnimating;
            }
            set
            {
                if (value)
                {
                    if (this.RenderTarget != null)
                    {
                        // Only start timing if we've been set up. If not, we'll
                        // start the timer after we've created the resources.
                        this.timer.Start();
                    }
                }
                else
                {
                    this.timer.Stop(); // Always safe to call
                }
                this.isAnimating = value;
            }
        }

        /// <summary>
        /// Gets the time in seconds since the last update.
        /// </summary>
        /// <remarks>
        /// The accuracy of this field is the same as DateTime so will only be
        /// accurate to approximately 15ms, as the DispatcherTimer used has the
        /// same limitation.
        /// </remarks>
        protected double ElapsedTime { get; private set; }

        /// <summary>
        /// Creates device dependent resources and resumes the animation.
        /// </summary>
        protected override void OnCreateResources()
        {
            if (this.isAnimating)
            {
                this.timer.Start();
            }

            base.OnCreateResources();
        }

        /// <summary>
        /// Releases device dependent resources and pauses the animation.
        /// </summary>
        protected override void OnFreeResources()
        {
            base.OnFreeResources();
            this.timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            // We use DateTime.UtcNow as it's faster than DateTime.Now and all
            // we're interested in is the difference between calls.
            this.ElapsedTime = DateTime.UtcNow.Subtract(this.lastUpdate).TotalSeconds;

            this.Render(); // Force an update
            this.lastUpdate = DateTime.UtcNow;
        }
    }
}
