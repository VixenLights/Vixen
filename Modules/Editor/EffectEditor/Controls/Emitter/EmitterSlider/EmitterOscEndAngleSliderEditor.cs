using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	class EmitterOscEndAngleSliderEditor : EmitterSliderEditorBase
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(EmitterOscEndAngleSliderEditor);

		#endregion

		#region Static Constructor

		static EmitterOscEndAngleSliderEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
