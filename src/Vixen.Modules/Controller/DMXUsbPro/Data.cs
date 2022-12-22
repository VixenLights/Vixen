namespace VixenModules.Output.DmxUsbPro
{
	using System.IO.Ports;
	using System.Runtime.Serialization;
	using Vixen.Module;

	[DataContract]
	public class Data : ModuleDataModelBase
	{
		[DataMember]
		public int BaudRate { get; set; }

		[DataMember]
		public int DataBits { get; set; }

		[DataMember]
		public Parity Partity { get; set; }

		[DataMember]
		public string PortName { get; set; }

		[DataMember]
		public StopBits StopBits { get; set; }

		public override IModuleDataModel Clone()
		{
			var result = new Data
			             	{
			             		BaudRate = this.BaudRate,
			             		DataBits = this.DataBits,
			             		Partity = this.Partity,
			             		PortName = this.PortName,
			             		StopBits = this.StopBits
			             	};
			return result;
		}
	}
}