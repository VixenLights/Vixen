using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterNozzleAngleComboBoxEditor : EmitterComboBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterNozzleAngleComboBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterNozzleAngleComboBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
