using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Output.Hill320
{
	[DataContract]
	internal class Data : ModuleDataModelBase
	{
		private ushort _controlPort;
		private ushort _dataPort;
		private ushort _statusPort;

		[DataMember]
		public ushort PortAddress
		{
			get { return _dataPort; }
			set { _dataPort = value; }
		}

		[DataMember]
		public ushort ControlPort
		{
			get { return _controlPort; }
			set { _controlPort = value; }
		}

		[DataMember]
		public ushort StatusPort
		{
			get { return _statusPort; }
			set { _statusPort = value; }
		}


		public override IModuleDataModel Clone()
		{
			return new Data() {PortAddress = PortAddress, ControlPort = ControlPort, StatusPort = StatusPort};
		}
	}
}