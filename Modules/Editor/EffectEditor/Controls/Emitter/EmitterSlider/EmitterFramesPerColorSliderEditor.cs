using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterFramesPerColorSliderEditor : EmitterSliderEditorBase
	{
		#region Static Fields

		private static readonly Type ThisType = typeof(EmitterFramesPerColorSliderEditor);

		#endregion

		#region Static Constructor

		static EmitterFramesPerColorSliderEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
