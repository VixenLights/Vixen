using Catel.MVVM;
using GongSolutions.Wpf.DragDrop;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace VixenApplication.DisplaySetup.ViewModels
{
    public class PropTreeViewModel : ViewModelBase, IDropTarget, IDragSource, IDisposable
	{
        public void OnDragEnter(DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnDragLeave(EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnDragDrop(DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnDragOver(DragEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            throw new NotImplementedException();
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            throw new NotImplementedException();
        }

        public void Dropped(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            throw new NotImplementedException();
        }

        public void DragCancelled()
        {
            throw new NotImplementedException();
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}