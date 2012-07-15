namespace VixenModules.App.DisplayPreview.WPF
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using VixenModules.App.DisplayPreview.Model;

    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumbDragDelta;
        }

        private void MoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;
            var contentControl = DataContext as ContentControl;

            if (contentControl != null)
            {
                var displayItem = contentControl.Content as DisplayItem;
                if (displayItem != null)
                {
                    if (!displayItem.IsUnlocked)
                    {
                        return;
                    }
                }
            }

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
