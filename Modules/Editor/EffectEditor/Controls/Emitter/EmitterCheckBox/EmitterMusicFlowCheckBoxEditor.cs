using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	class EmitterMusicFlowCheckBoxEditor : EmitterCheckBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterMusicFlowCheckBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterMusicFlowCheckBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion

		#region Protected Methods

		protected override bool GetIEmitterProperty(IEmitter emitter)
		{
			return emitter.FlowMatchesMusic;
		}

		#endregion
	}
}
