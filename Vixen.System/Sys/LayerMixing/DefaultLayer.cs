using System.Runtime.Serialization;

namespace Vixen.Sys.LayerMixing
{
	[DataContract]
	public class DefaultLayer:Layer
	{
		public DefaultLayer()
		{
			Type = LayerType.Default;
			LayerName = "Default";
			LayerLevel = 0;
		}

	}
}
