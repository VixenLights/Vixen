using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Abstract place holder for all effect data implementations
	/// This should ensure a consistent version of clone that can be enhanced shoudl some base data types 
	/// be added in this class. Future refactoring could place some common types here.
	/// </summary>
	[DataContract]
	public abstract class EffectTypeModuleData: ModuleDataModelBase
	{
		protected EffectTypeModuleData()
		{
			TargetPositioning = TargetPositioningType.Strings;
		}

		[DataMember]
		public TargetPositioningType TargetPositioning { get; set; }

		public override IModuleDataModel Clone()
		{
			var instance = CreateInstanceForClone();
			instance.TargetPositioning = TargetPositioning;
			return instance;
		}

		protected abstract EffectTypeModuleData CreateInstanceForClone();

	}
}
