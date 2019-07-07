using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterFlowControlComboBoxEditor : EmitterComboBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterFlowControlComboBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterFlowControlComboBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
