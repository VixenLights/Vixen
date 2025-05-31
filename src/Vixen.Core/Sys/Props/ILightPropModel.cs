using System.Collections.ObjectModel;
using Vixen.Sys.Props.Model;

namespace Vixen.Sys.Props
{
	public interface ILightPropModel: IPropModel
	{
        public ObservableCollection<NodePoint> Nodes { get; set; }
        public int NodeSize { get; set; }
    }
}
