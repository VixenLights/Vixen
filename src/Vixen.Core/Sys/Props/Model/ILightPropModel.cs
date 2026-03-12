using System.Collections.ObjectModel;

namespace Vixen.Sys.Props.Model
{
	/// <summary>
	/// Maintains light prop model.
	/// </summary>
	public interface ILightPropModel : IPropModel
	{
		/// <summary>
		/// Collection of 3-D prop nodes.
		/// </summary>
		ObservableCollection<NodePoint> Nodes { get; set; }

		public int LightSize { get; set;  }

		public ObservableCollection<AxisRotationModel> AxisRotationModel { get; set; }

		/// <summary>
		/// Updates prop nodes for rotation changes.
		/// </summary>
		void UpdatePropNodes();
	}
}
