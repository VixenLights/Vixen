using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public class LayerMixingDefinition: BaseLayerMixingDefinition
	{
		public LayerMixingDefinition(string layerName)
		{
			Type = LayerMixingDefinitionType.Standard;
			LayerName = layerName;
		}
	}
}
