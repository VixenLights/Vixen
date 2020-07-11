using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Vixen.Marks;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Liquid;

namespace Liquid
{
    /// <summary>
    /// Maintains the collection of emitters.
    /// </summary>	
	public class EmitterCollection : NotifyPropertyObservableCollection<IEmitter>
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public EmitterCollection() : base("Emitters")
		{
		}

		#endregion

		#region Public Properties

		private BaseEffect _parent;

        /// <summary>
        /// Parent Liquid effect.
        /// </summary>
		public BaseEffect Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;

				// Give each of the emitters a reference to the parent effect
				foreach (IEmitter emitter in this)
				{					
					emitter.Parent = Parent;
				}
			}
		}

		private ObservableCollection<string> _markNameCollection;
		
        /// <summary>
        /// Mark name collection.
        /// </summary>
		public ObservableCollection<string> MarkNameCollection
		{
			get
			{
				return _markNameCollection;
			}
			set
			{
				_markNameCollection = value;

                // Give each emitter the mark name collection
				foreach (IEmitter emitter in this)
				{
					emitter.MarkNameCollection = _markNameCollection;
				}							
			}
		}

		private ObservableCollection<IMarkCollection> _markCollections;

        /// <summary>
        /// IMarkCollection collection.
        /// </summary>
		public ObservableCollection<IMarkCollection> MarkCollections
		{
			get
			{
				return _markCollections;
			}
			set
			{
				_markCollections = value;

                // Give each emitter the IMarkCollection collection
				foreach (IEmitter emitter in this)
				{
					emitter.MarkCollections = value;
				}
			}
		}

		#endregion

		#region Public Methods

	    /// <summary>
        /// Updates the selected mark collection names on the emitters.
        /// This method helps handle when a IMarkCollection name is changed.
        /// </summary>
		public void UpdateSelectedMarkCollectionNames()
		{
			foreach (IEmitter emitter in this)
			{
				emitter.UpdateSelectedMarkCollectionName();
			}           
		}

		#endregion

		#region Protected Methods
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) 
		{
			// Call the base class implementation
			base.OnCollectionChanged(e);

			foreach(Emitter emitter in this)
			{				
				if (Parent != null)
				{
					emitter.Parent = Parent;
				}

				if (MarkNameCollection != null)
				{
					emitter.MarkNameCollection = MarkNameCollection;
				}

				if (MarkCollections != null)
				{
					emitter.MarkCollections = MarkCollections;
				}
			}
		}
	
		#endregion
	}
}
