using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	class EmitterEdgeHandlingComboBoxEditor : EmitterComboBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterEdgeHandlingComboBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterEdgeHandlingComboBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
