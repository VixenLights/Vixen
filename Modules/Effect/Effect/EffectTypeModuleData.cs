using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Effect.Effect
{
	[DataContract]
	public abstract class EffectTypeModuleData: ModuleDataModelBase
	{
		[DataMember]
		public byte Layer { get; set; }
		
	}
}
