﻿//using CommandStandard;

namespace Vixen.Module.EffectEditor
{
	public interface IEffectEditorModuleDescriptor : IModuleDescriptor
	{
		/// <summary>
		/// Type id of the effect that this control edits.
		/// Guid.Empty if the editor isn't specific to an effect.
		/// </summary>
		Guid EffectTypeId { get; }

		/// <summary>
		/// Signature of the parameter types this control edits.
		/// Null if the editor is specific to an effect.
		/// </summary>
		Type[] ParameterSignature { get; }
	}
}