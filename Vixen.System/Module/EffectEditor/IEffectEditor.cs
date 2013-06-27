using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Commands;

namespace Vixen.Module.EffectEditor
{
	public interface IEffectEditor
	{
		IEffectEditorControl CreateEditorControl();

		/// <summary>
		/// Type id of the effect that this control edits.
		/// Guid.Empty if the editor isn't specific to an effect.
		/// </summary>
		Guid EffectTypeId { get; }

		/// <summary>
		/// Signature of the effect parameters this control edits.
		/// Null if the editor is specific to an effect.
		/// </summary>
		Type[] ParameterSignature { get; }
	}
}