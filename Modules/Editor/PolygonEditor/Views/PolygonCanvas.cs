using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VixenModules.Editor.PolygonEditor.Views
{
    /// <summary>
    /// Polygon canvas.  This class adds the grid and tick marks.
    /// </summary>
    public class PolygonCanvas : Canvas
	{
        #region Private Methods

        /// <summary>
        /// Draws vertical grid lines on the canvas.
        /// </summary>
        /// <param name="drawingContext">Drawing context</param>
        /// <param name="pen">Pen to draw the grid lines with</param>
        /// <param name="numberOfLines">Number of lines to draw</param>
        /// <param name="gridWidth">Width between grid lines</param>
        private void DrawVerticalGridLines(DrawingContext drawingContext, Pen pen, int numberOfLines, double gridWidth)
        {
            double offset = 0;

            for (int i = 0; i <= numberOfLines; ++i)
            {
                // Draw a vertical line to form the grid
                drawingContext.DrawLine(pen, new Point(offset, 0), new Point(offset, ActualHeight - 1));

                // Advance to the next line
                offset += gridWidth;
            }

            drawingContext.DrawLine(pen, new Point(ActualWidth - 1, 0), new Point(ActualWidth - 1, ActualHeight - 1));
        }

        /// <summary>
        /// Draws horizontal grid lines on the canvas.
        /// </summary>
        /// <param name="drawingContext">Drawing context</param>
        /// <param name="pen">Pen to draw the grid lines with</param>
        /// <param name="numberOfLines">Number of lines to draw</param>
        /// <param name="gridWidth">Width between grid lines</param>
        private void DrawHorizontalGridLines(DrawingContext drawingContext, Pen pen, int numberOfLines, double gridWidth)
        {
            double offset = 0;

            for (int i = 0; i <= numberOfLines; ++i)
            {
                // Draw a horizontal line to form the grid
                drawingContext.DrawLine(pen, new Point(0, offset), new Point(ActualWidth - 1, offset));

                // Advance to the next line
                offset += gridWidth;
            }

            drawingContext.DrawLine(pen, new Point(0, ActualHeight - 1), new Point(ActualWidth - 1, ActualHeight-1));
        }

        /// <summary>
        /// Draws horizontal tick marks on the top and bottom of the editor.
        /// </summary>
        /// <param name="drawingContext">Drawing context</param>
        /// <param name="tickPen">Pen to draw tick marks with</param>
        /// <param name="tickHeight">Height of the tick marks</param>
        /// <param name="tickSpacing">Spacing between ticks</param>
        private void DrawHorizontalTickMarks(DrawingContext drawingContext, Pen tickPen, int tickHeight, double tickSpacing)
        {
            double offset = 0;
            while (offset < ActualWidth)
            {
                drawingContext.DrawLine(tickPen, new Point(offset, 0), new Point(offset, tickHeight));
                drawingContext.DrawLine(tickPen, new Point(offset, ActualHeight - tickHeight), new Point(offset, ActualHeight));
                offset = offset + tickSpacing;
            }
        }

        /// <summary>
        /// Draws vertical tick marks on the left and right of the editor.
        /// </summary>
        /// <param name="drawingContext">Drawing context</param>
        /// <param name="tickPen">Pen to draw tick marks with</param>
        /// <param name="tickHeight">Height of the tick marks</param>
        /// <param name="tickSpacing">Spacing between ticks</param>
        private void DrawVerticalTickMarks(DrawingContext drawingContext, Pen tickPen, int tickHeight, double tickSpacing)
        {
            double offset = 0;
            while (offset < ActualHeight)
            {
                drawingContext.DrawLine(tickPen, new Point(0, offset), new Point(tickHeight, offset));
                drawingContext.DrawLine(tickPen, new Point(ActualWidth - tickHeight, offset), new Point(ActualWidth, offset));
                offset = offset + tickSpacing;
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

            double height = ActualHeight;
            double width = ActualWidth;
            
            // This is the length between the grid lines
            double gridWidth;

            // The larger dimension will have 10 grid lines
            const int MaxGridLines = 10;

            // Check which dimension is larger
            if (height >  width)
            {                
                gridWidth = height / MaxGridLines;                
            }
            else
            {             
                gridWidth = (width / MaxGridLines);                
            }

            // Determine the number of vertical lines to draw
            int linesVertical = (int)(width / gridWidth);

            // Determine the number of horizontal lines to draw
            int linesHorizontal = (int)(height / gridWidth);

            // Determine the spacing between the tick marks
            double tickSpacing = gridWidth / 5;

            // Create a Light Gray Pen for drawing the grid lines 
            Pen pen = new Pen(Brushes.LightGray, 1) { StartLineCap = PenLineCap.Triangle, EndLineCap = PenLineCap.Triangle };            
            
            // Set the pen style to dash 
            pen.DashStyle = DashStyles.Dash;
            
            // Draw the grid lines
            DrawVerticalGridLines(drawingContext, pen, linesVertical, gridWidth);
            DrawHorizontalGridLines(drawingContext, pen, linesHorizontal, gridWidth);
            
            // Create a Light Gray Pen for drawing the tick marks along the outside of the canvas
            Pen tickPen = new Pen(Brushes.LightGray, 1) { StartLineCap = PenLineCap.Triangle, EndLineCap = PenLineCap.Triangle };
            
            const int TickHeight = 10;

            // Draw the ticks around the edge of the editor
            DrawHorizontalTickMarks(drawingContext, tickPen, TickHeight, tickSpacing);
            DrawVerticalTickMarks(drawingContext, tickPen, TickHeight, tickSpacing);
        }

    #endregion
    }
}
