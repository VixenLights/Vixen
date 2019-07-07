using System;
using System.Windows;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterAnimateCheckBoxEditor : EmitterCheckBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterAnimateCheckBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterAnimateCheckBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
										
		#region Protected Methods

		protected override bool GetIEmitterProperty(IEmitter emitter)
		{
			return emitter.Animate;
		}

		#endregion
	}
}
