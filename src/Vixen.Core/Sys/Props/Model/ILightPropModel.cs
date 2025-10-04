using System.Collections.ObjectModel;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Maintains light prop model.
	/// </summary>
	public interface ILightPropModel : IPropModel
	{
		/// <summary>
		/// Collection of 2-D prop nodes.  The Z component is not utilized.
		/// </summary>
		ObservableCollection<NodePoint> Nodes { get; set; }

		/// <summary>
		/// Collection of 3-D prop nodes.
		/// </summary>
		ObservableCollection<NodePoint> ThreeDNodes { get; set; }
					
		/// <summary>
		/// Updates prop nodes for rotation changes.
		/// </summary>
		void UpdatePropNodes();
	}
}
