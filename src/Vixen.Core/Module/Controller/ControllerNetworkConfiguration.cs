using System.Net;

namespace Vixen.Module.Controller
{
	public class ControllerNetworkConfiguration
	{
		public ProtocolTypes ProtocolType { get; set; }

		public TransmissionMethods TransmissionMethod { get; set; }

		public IPAddress IpAddress { get; set; }

		//public string HostName { get; set; }
		public bool SupportsUniverses { get; set; }

		public List<UniverseConfiguration> Universes { get; set; }
	}

	public class UniverseConfiguration
	{
		public int UniverseNumber { get; set; }

		public bool Active { get; set; }

		public int Start { get; set; }

		public int Size { get; set; }
	}
}
