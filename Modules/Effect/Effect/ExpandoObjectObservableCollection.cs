using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Vixen.Marks;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Maintains a collection of ExpandoObjects and provides properties and methods
	/// for sharing mark collections with these objects.
	/// </summary>
	/// <typeparam name="T">Collection Item Type</typeparam>
	public abstract class ExpandoObjectObservableCollection<T> : NotifyPropertyObservableCollection<T>
		where T : IMarkCollectionExpandoObject
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName">Name of the collection for child property change events</param>
		public ExpandoObjectObservableCollection(string propertyName) : base(propertyName)
		{
		}

		#endregion

		#region Public Properties

		private BaseEffect _parent;

		/// <summary>
		/// Parent effect.
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

				// 
				foreach (IMarkCollectionExpandoObject expandoObject in this)
				{
					// Give each Expando object a reference to the parent effect
					expandoObject.Parent = Parent;
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

				
				foreach (IMarkCollectionExpandoObject expandoObject in this)
				{
					// Give each Expando object the IMarkCollection collection
					expandoObject.MarkCollections = value;
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Updates the selected mark collection on the Expando objects when it no longer exists.		
		/// </summary>
		public void UpdateSelectedMarkCollectionNames()
		{
			// Loop over the Expando objects
			foreach (IMarkCollectionExpandoObject expandoObject in this)
			{
				// Update the selected mark collection on the Expando object
				expandoObject.UpdateSelectedMarkCollectionName();
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

			// Loop over the expando objects
			foreach (IMarkCollectionExpandoObject expandoObject in this)
			{
				if (Parent != null)
				{
					// Give the Expando object a reference to the parent effect
					expandoObject.Parent = Parent;
				}

				if (MarkCollections != null)
				{
					// Give the expando object the mark collection for the effect
					expandoObject.MarkCollections = MarkCollections;
				}
			}
		}

		#endregion
	}
}
