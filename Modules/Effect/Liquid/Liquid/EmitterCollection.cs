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
	public class EmitterCollection : MarkCollectionExpandoObjectCollection<IEmitter, Emitter>
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

		#endregion
		
		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>		
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
