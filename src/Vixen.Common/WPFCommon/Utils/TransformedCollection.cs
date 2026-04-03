using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Vixen.Extensions;

namespace Common.WPFCommon.Utils
{
	public class TransformedCollection<TSource, TTarget> : ReadOnlyCollection<TTarget>, INotifyCollectionChanged, IDisposable
    {
        #region Fields
        private bool _disposed;
        private readonly IEnumerable<TSource> _sourceCollection;
        private readonly Func<TSource, TTarget> _setup;
        private readonly Action<TTarget> _teardown;
        #endregion

        #region Constructors
        public TransformedCollection(IEnumerable<TSource> sourceCollection, Func<TSource, TTarget> setup, Action<TTarget> teardown = null) :
            base(new List<TTarget>(sourceCollection.Select(setup)))
        {
            _setup = setup;
            _teardown = teardown;

            _sourceCollection = sourceCollection;
            var notifyCollectionChanged = _sourceCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Public Methods
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #region Protected Methods
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                    var notifyCollectionChanged = _sourceCollection as INotifyCollectionChanged;
                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged -= OnSourceCollectionChanged;
                    }
                }

                // Cleanup unmanaged resources

                // Mark the object as disposed
                _disposed = true;
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
        #endregion

        #region Private Methods
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (_teardown != null)
                {
                    foreach (TTarget target in Items)
                    {
                        _teardown(target);
                    }
                }

                Items.Clear();
                Items.AddRange(_sourceCollection.Select(_setup));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                //When we move, we don't need to recreate the VM, just move it
                List<object> newItems = null;
                if (e.OldItems != null)
                {
                    newItems = new List<object>();
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        TTarget target = Items[e.OldStartingIndex];
                        newItems.Add(target);
                        Items.RemoveAt(e.OldStartingIndex);
                        Items.Insert(i + e.NewStartingIndex, target);
                    }
                }

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewStartingIndex, e.OldStartingIndex));
            }
            else
            {
                List<object> oldItems = null;
                if (e.OldItems != null)
                {
                    oldItems = new List<object>();
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        TTarget target = Items[e.OldStartingIndex];
                        oldItems.Add(target);
                        if (_teardown != null)
                        {
                            _teardown(target);
                        }
                        Items.RemoveAt(e.OldStartingIndex);
                    }
                }

                List<object> newItems = null;
                if (e.NewItems != null)
                {
                    newItems = new List<object>();
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        TTarget target = _setup((TSource)e.NewItems[i]);
                        newItems.Add(target);
                        Items.Insert(i + e.NewStartingIndex, target);
                    }
                }

                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, e.OldStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, e.NewStartingIndex));
                }
            }
        }
        #endregion
    }

    public static class CollectionTransforms
    {
        public static TransformedCollection<TSource, TTarget> Transform<TSource, TTarget>(this IEnumerable<TSource> sourceCollection, Func<TSource, TTarget> setup, Action<TTarget> teardown = null)
        {
            return new TransformedCollection<TSource, TTarget>(sourceCollection, setup, teardown);
        }
    }
}
