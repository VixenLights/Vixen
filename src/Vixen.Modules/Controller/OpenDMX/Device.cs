using System.Runtime.Serialization;

namespace VixenModules.Controller.OpenDMX
{
	[DataContract]
	public class Device
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int Index { get; set; }

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string SerialNumber { get; set; }

		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Description} - {SerialNumber} - {Id}";
		}

		#endregion
	}
}
