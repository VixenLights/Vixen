using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VixenModules.Editor.PolygonEditor.Views
{
    /// <summary>
    /// Draws the background for the polygon editor time bar.
    /// </summary>
	public class PolygonTimeBarCanvas : Canvas
	{
        #region Private Constants

        /// <summary>
        /// Margin on the left and right side of the time bar scale.
        /// </summary>
        const int RulerMargin = 5;
        #endregion

        #region Private Methods

        /// <summary>
        /// Draws horizontal tick marks on the bottom of the time bar.
        /// </summary>
        /// <param name="drawingContext">Drawing context</param>
        /// <param name="tickPen">Pen to draw tick marks with</param>
        /// <param name="smallTickHeight">Height of the small tick marks</param>
        /// <param name="tickSpacing">Spacing between ticks</param>
        private void DrawHorizontalTickMarks(DrawingContext drawingContext, Pen tickPen, int smallTickHeight, int largeTickHeight, double tickSpacing)
        {                        
            // Keep track of how many ticks have been drawn                        
            int tickCount = 0;

            // Start at the left margin
            double offset = RulerMargin;

            // Draw ticks along the time bar until we get to the end
            while (offset < (ActualWidth - RulerMargin))
            {
                // Draw a large tick every ten ticks
                const int LargeTickSpacing = 10;

                // Every ten ticks draw a large tick
                if (tickCount % LargeTickSpacing == 0)
                { 
                    // Draw a large tick
                    drawingContext.DrawLine(tickPen, new Point(offset, ActualHeight - largeTickHeight), new Point(offset, ActualHeight));
                }
                else
                {
                    // Draw a small tick
                    drawingContext.DrawLine(tickPen, new Point(offset, ActualHeight - smallTickHeight), new Point(offset, ActualHeight));
                }
                
                // Move to the right to the next tick
                offset = offset + tickSpacing;
                
                // Keep track of how many ticks have been drawn
                tickCount++;
            }
        }
        
        #endregion

        #region Protected Methods

        /// <summary>
        /// Refer to MSDN documentation.
        /// </summary>		
        protected override void OnRender(DrawingContext drawingContext)
        {
            // Call the base class implementation
            base.OnRender(drawingContext);
                        
            // The number of long tick marks
            const int NumberOfLongTickMarks = 10;
            const int NumberOfSmallTickMarks = 10;

            // Determine the spacing between the large tick marks           
            double largeTickSpacing = ((ActualWidth - 2 * RulerMargin) / NumberOfLongTickMarks);

            // Determine the spacing between the small tick marks
            double smallTickSpacing = largeTickSpacing / NumberOfSmallTickMarks;

            // Create a Light Gray Pen for drawing the tick marks 
            Pen tickPen = new Pen(Brushes.Black, 1) { StartLineCap = PenLineCap.Triangle, EndLineCap = PenLineCap.Triangle };

            const int SmallTickHeight = 10;
            const int LargeTickHeight = 15;
            
            // Draw the ruler at the bottom
            DrawHorizontalTickMarks(drawingContext, tickPen, SmallTickHeight, LargeTickHeight, smallTickSpacing);           
        }

        #endregion
    }
}
