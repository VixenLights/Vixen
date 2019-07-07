using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterOnTimeSliderEditor : EmitterSliderEditorBase
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(EmitterOnTimeSliderEditor);

		#endregion

		#region Static Constructor

		static EmitterOnTimeSliderEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
