using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Maintains a collection of props.
	/// </summary>
	public interface IPropGroup
	{
		/// <summary>
		/// True when collections of props are to be contained within a group.
		/// </summary>
		bool CreateGroup { get; set; }

		/// <summary>
		/// Name of the group node.
		/// </summary>
		string GroupName { get; set; }
		
		/// <summary>
		/// Gets a Collection of props.
		/// </summary>
		IList<IProp> Props { get; }
	}
}
