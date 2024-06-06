using System.Net;
using System.Runtime.Serialization;

using Vixen.Module;

namespace VixenModules.Output.ElexolEtherIO
{

	[DataContract]
	public class ElexolEtherIOData : ModuleDataModelBase
	{

		override public IModuleDataModel Clone()
		{
			return MemberwiseClone() as IModuleDataModel;
		}

		#region IP Setup

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int MinimumIntensity { get; set; }

		[DataMember]
		public int Port { get; set; }

		#endregion
	}
}
