using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
            this._setup = setup;
            this._teardown = teardown;

            this._sourceCollection = sourceCollection;
            var notifyCollectionChanged = this._sourceCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += this.OnSourceCollectionChanged;
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Public Methods
        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion

        #region Protected Methods
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                    var notifyCollectionChanged = this._sourceCollection as INotifyCollectionChanged;
                    if (notifyCollectionChanged != null)
                    {
                        notifyCollectionChanged.CollectionChanged -= this.OnSourceCollectionChanged;
                    }
                }

                // Cleanup unmanaged resources

                // Mark the object as disposed
                this._disposed = true;
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }
        #endregion

        #region Private Methods
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this._teardown != null)
                {
                    foreach (TTarget target in this.Items)
                    {
                        this._teardown(target);
                    }
                }

                this.Items.Clear();
                this.Items.AddRange(this._sourceCollection.Select(this._setup));
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
                        TTarget target = this.Items[e.OldStartingIndex];
                        newItems.Add(target);
                        this.Items.RemoveAt(e.OldStartingIndex);
                        this.Items.Insert(i + e.NewStartingIndex, target);
                    }
                }

                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewStartingIndex, e.OldStartingIndex));
            }
            else
            {
                List<object> oldItems = null;
                if (e.OldItems != null)
                {
                    oldItems = new List<object>();
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        TTarget target = this.Items[e.OldStartingIndex];
                        oldItems.Add(target);
                        if (this._teardown != null)
                        {
                            this._teardown(target);
                        }
                        this.Items.RemoveAt(e.OldStartingIndex);
                    }
                }

                List<object> newItems = null;
                if (e.NewItems != null)
                {
                    newItems = new List<object>();
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        TTarget target = this._setup((TSource)e.NewItems[i]);
                        newItems.Add(target);
                        this.Items.Insert(i + e.NewStartingIndex, target);
                    }
                }

                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, e.OldStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewStartingIndex));
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems, e.NewStartingIndex));
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
