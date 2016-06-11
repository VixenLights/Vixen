using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public class DefaultLayerMixingDefinition:BaseLayerMixingDefinition
	{
		public DefaultLayerMixingDefinition()
		{
			Type = LayerMixingDefinitionType.Default;
			LayerName = "Default";
			LayerLevel = 0;
		}

	}
}
