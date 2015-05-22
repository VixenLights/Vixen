namespace System.Windows.Controls.WpfPropertyGrid.Editors
{
	public class SelectionEditor : PropertyEditor
	{
		public SelectionEditor()
		{
			InlineTemplate = EditorKeys.ComboBoxEditorKey;
		}

		public SelectionEditor(Type declaringType, string propertyName):base(declaringType,propertyName, EditorKeys.ComboBoxEditorKey)
		{
			
		}
	}
}