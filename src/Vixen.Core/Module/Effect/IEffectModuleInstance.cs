namespace Vixen.Module.Effect
{
	public interface IEffectModuleInstance : IEffect, IModuleInstance
	{
		 bool ForceGenerateVisualRepresentation { get;   }

		void MarkDirty();
		void Removing();
	}
}