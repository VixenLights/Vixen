using System;
using System.Windows;
using System.Windows.Controls;

namespace VixenModules.Editor.EffectEditor.Controls
{
	public class EmitterMarkCollectionComboBoxEditor : EmitterComboBoxEditorBase
	{
		#region Static Properties

		private static readonly Type ThisType = typeof(EmitterMarkCollectionComboBoxEditor);

		#endregion

		#region Static Constructor

		static EmitterMarkCollectionComboBoxEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(ThisType, new FrameworkPropertyMetadata(ThisType));
		}

		#endregion

		#region Protected Methods

		protected override void EmitterDropDownClosed(object sender, EventArgs e)
		{
			if (Value != null)
			{
				SelectionChangedEventArgs selectionChanged = (SelectionChangedEventArgs)e;

				if ((selectionChanged.AddedItems.Count == 1 && selectionChanged.RemovedItems.Count == 1) ||
 					  selectionChanged.AddedItems.Count == 1 && string.IsNullOrEmpty(Value.MarkCollectionName))
				{
					Value = EmitterValue.CreateInstanceForClone();
					Value.InEdit = EmitterValue;
				}
			}
		}

		#endregion
	}
}
