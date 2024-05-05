using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains a collection of whirls.
	/// </summary>
	public class WhirlCollection : ExpandoObjectObservableCollection<IWhirl, Whirl>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WhirlCollection() :
			base("Whirls")
		{
		}

		#endregion
	}
}
