using System;
using System.Windows;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterParticleTypeComboBoxEditor : EmitterComboBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterParticleTypeComboBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterParticleTypeComboBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion
	}
}
