using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	class EmitterOscStartAngleSliderEditor : EmitterSliderEditorBase
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(EmitterOscStartAngleSliderEditor);

		#endregion

		#region Static Constructor

		static EmitterOscStartAngleSliderEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
