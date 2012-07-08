using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using Vixen.Module;

namespace VixenModules.Output.BlinkyLinky
{
	[DataContract]
	public class BlinkyLinkyData : ModuleDataModelBase
	{
		[DataMember]
		public IPAddress Address { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public int Stream { get; set; }

		public BlinkyLinkyData()
		{
			Address = new IPAddress(new byte[] { 10, 0, 0, 1 });
			Port = 747;
			Stream = 0;
		}

		public override IModuleDataModel Clone()
		{
			BlinkyLinkyData result = new BlinkyLinkyData();
			result.Address = new IPAddress(Address.GetAddressBytes());
			result.Port = Port;
			result.Stream = Stream;
			return result;
		}
	}
}
