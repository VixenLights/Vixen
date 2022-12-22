using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Maintains the collection of morph polygons.
	/// </summary>	
	public class MorphPolygonsObservableCollection : NotifyPropertyObservableCollection<IMorphPolygon>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MorphPolygonsObservableCollection() : base("MorphPolygons")
		{
		}

		#endregion
	}
}
