using System.Runtime.Serialization;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	[DataContract]
	public class StateData: EffectTypeModuleData
	{
		[DataMember] 
		public Guid MarkCollectionId { get; set; } = Guid.Empty;

		[DataMember] 
		public TimingMode TimingMode { get; set; } = TimingMode.State;
		
		[DataMember(EmitDefaultValue = false)]
		public bool AllowMarkGaps { get; set; }
		
		#region Overrides of EffectTypeModuleData

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			StateData result = new StateData
			{
				MarkCollectionId = MarkCollectionId,
				TimingMode = TimingMode,
				AllowMarkGaps = AllowMarkGaps
			};
			return result;
		}

		#endregion
	}
}