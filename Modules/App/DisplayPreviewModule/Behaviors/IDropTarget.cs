namespace VixenModules.App.DisplayPreview.Behaviors
{
    using System.Windows;

    public interface IDropTarget
    {
        /// <summary>
        /// Drops the specified data object
        /// </summary>
        /// <param name="dataObject">
        /// The data object.
        /// </param>
        /// <param name="point">
        /// The point.
        /// </param>
        void Drop(IDataObject dataObject, Point point);

        /// <summary>
        ///   Gets the effects.
        /// </summary>
        /// <param name = "dataObject">The data object.</param>
        /// <returns>The drag effects to be applied.</returns>
        DragDropEffects GetDropEffects(IDataObject dataObject);
    }
}
