namespace VixenModules.App.DisplayPreview.WPF
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumbDragDelta;
        }

        private void MoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem != null)
            {
                var left = Canvas.GetLeft(designerItem);
                var top = Canvas.GetTop(designerItem);

                Canvas.SetLeft(designerItem, left + e.HorizontalChange);
                Canvas.SetTop(designerItem, top + e.VerticalChange);
            }
        }
    }
}
