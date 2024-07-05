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
			Address = "127, 0, 0, 1";
		}

		public override IModuleDataModel Clone()
		{
			DDPData result = new DDPData();
			result.Address = Address;
			return result;
		}
	}
}