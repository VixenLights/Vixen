using System.Runtime.Serialization;
using System.Net;
using Vixen.Module;

namespace VixenModules.Output.DDP
{
	[DataContract]
	public class DDPData : ModuleDataModelBase
	{
		/// <summary>
		/// Gets or sets the IP address used to communicate with the controller, in dotted-quad string
		/// form (e.g. "192.168.1.50"). This is always an IP-literal string, never a host name -- if the
		/// user configured the controller using a host name, <see cref="HostName"/> holds that original
		/// entry and this property holds the IP address it most recently resolved to.
		/// </summary>
		[DataMember]
		public string Address { get; set; }

		/// <summary>
		/// Gets or sets the DNS host name the user entered in the setup dialog, or <see langword="null"/>
		/// if the controller is configured with a literal IP address instead. When set, the setup dialog
		/// pre-selects "Host Name" mode and pre-fills this value the next time it is opened.
		/// </summary>
		[DataMember]
		public string HostName { get; set; }

		public DDPData()
		{
			Address = IPAddress.Loopback.ToString();
		}

		public override IModuleDataModel Clone()
		{
			DDPData result = new DDPData();
			result.Address = Address;
			result.HostName = HostName;
			return result;
		}
	}
}