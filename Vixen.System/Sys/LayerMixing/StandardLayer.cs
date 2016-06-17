using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public class StandardLayer: Layer
	{
		public StandardLayer(string layerName)
		{
			Type = LayerType.Standard;
			LayerName = layerName;
		}
	}
}
