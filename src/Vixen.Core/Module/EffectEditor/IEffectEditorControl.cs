using Vixen.Module.Effect;

namespace Vixen.Module.EffectEditor
{
	public interface IEffectEditorControl
	{
		object[] EffectParameterValues { get; set; }
		IEffect TargetEffect { get; set; }
	}
}