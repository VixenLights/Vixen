using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterOffTimeSliderEditor : EmitterSliderEditorBase
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(EmitterOffTimeSliderEditor);

		#endregion

		#region Static Constructor

		static EmitterOffTimeSliderEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
