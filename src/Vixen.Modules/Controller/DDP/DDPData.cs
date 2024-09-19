using System.Runtime.Serialization;
using System.Net;
using Vixen.Module;

namespace VixenModules.Output.DDP
{
	[DataContract]
	public class DDPData : ModuleDataModelBase
	{
		[DataMember]
		public string Address { get; set; }

		public DDPData()
		{
			Address = IPAddress.Loopback.ToString();
		}

		public override IModuleDataModel Clone()
		{
			DDPData result = new DDPData();
			result.Address = Address;
			return result;
		}
	}
}