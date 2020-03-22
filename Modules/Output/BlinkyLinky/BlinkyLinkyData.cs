using System;
using System.Collections.Generic;
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
			Address = new IPAddress(new byte[] {1, 1, 1, 1});
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