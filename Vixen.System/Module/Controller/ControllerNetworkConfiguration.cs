using System.Collections.Generic;
using System.Net;

namespace Vixen.Module.Controller
{
	public class ControllerNetworkConfiguration
	{
		public bool SupportsUniverses { get; set; }

		public List<UniverseConfiguration> Universes { get; set; }
	}

	public class UniverseConfiguration
	{
		public int Universe { get; set; }

		public bool Active { get; set; }

		public int Start { get; set; }

		public int Size { get; set; }

		public bool IsMultiCast { get; set; }

		public IPEndPoint IpAddress { get; set; }
	}
}
