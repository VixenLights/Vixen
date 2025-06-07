using System.Collections.ObjectModel;

namespace Vixen.Sys.Props.Model
{
	public interface ILightPropModel: IPropModel
	{
        public ObservableCollection<NodePoint> Nodes { get; set; }
    }
}
