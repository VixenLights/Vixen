namespace System.Windows.Controls.WpfPropertyGrid.Design
{
	public class TabbedLayoutTemplateSelector : DataTemplateSelector
	{
		private readonly ResourceLocator _resourceLocator = new ResourceLocator();

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var category = item as CategoryItem;
			if (category != null)
			{
				var template = FindEditorTemplate(category);
				if (template != null) return template;
			}

			return base.SelectTemplate(item, container);
		}

		protected virtual DataTemplate FindEditorTemplate(CategoryItem category)
		{
			if (category == null) return null;

			var editor = category.Editor;

			if (editor == null) return null;

			var template = editor.InlineTemplate as DataTemplate;
			if (template != null) return template;

			return _resourceLocator.GetResource(editor.InlineTemplate) as DataTemplate;
		}
	}
}