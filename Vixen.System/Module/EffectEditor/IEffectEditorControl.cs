using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Effect;

namespace Vixen.Module.EffectEditor
{
	public interface IEffectEditorControl
	{
		object[] EffectParameterValues { get; set; }
		IEffect TargetEffect { get; set; }
	}
}