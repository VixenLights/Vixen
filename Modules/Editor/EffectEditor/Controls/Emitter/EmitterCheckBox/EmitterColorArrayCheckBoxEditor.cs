using System;
using System.Windows;
using VixenModules.Effect.Liquid;

namespace VixenModules.Editor.EffectEditor.Controls
{
	class EmitterColorArrayCheckBoxEditor : EmitterCheckBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterColorArrayCheckBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterColorArrayCheckBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
					
		#region Protected Methods

		protected override bool GetIEmitterProperty(IEmitter emitter)
		{
			return emitter.UseColorArray;
		}

		#endregion
	}
}
