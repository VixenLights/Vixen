using System.Runtime.Serialization;
using System.Net;
using Vixen.Module;

namespace VixenModules.Output.DDP
{
	[DataContract]
	public class DDPData : ModuleDataModelBase
	{
		[DataMember]
		public IPAddress Address { get; set; }

		public DDPData()
		{
			Address = new IPAddress(new byte[] {127, 0, 0, 1});
		}

		public override IModuleDataModel Clone()
		{
			DDPData result = new DDPData();
			result.Address = new IPAddress(Address.GetAddressBytes());
			return result;
		}
	}
}