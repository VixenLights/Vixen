using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Maintains a collection of props.
	/// </summary>
	public class PropGroup : IPropGroup
	{
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		public PropGroup()
		{
			// Initialize the prop collection
			Props = new List<IProp>();
		}
		
		#endregion

		#region IPropGroup

		///<inheritdoc/>
		public bool CreateGroup { get; set; }

		///<inheritdoc/>
		public string GroupName { get; set; }

		///<inheritdoc/>
		public IList<IProp> Props { get; private set; } 

		#endregion
	}
}
