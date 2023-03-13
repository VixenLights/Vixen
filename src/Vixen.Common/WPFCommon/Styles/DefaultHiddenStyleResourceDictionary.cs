using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Common.WPFCommon.Styles
{
	public class DefaultHiddenStyleResourceDictionary : ResourceDictionary
	{
		public DefaultHiddenStyleResourceDictionary()
		{
			// Run OnResourceDictionaryLoaded asynchronously to ensure other ResourceDictionary are already loaded before adding new entries
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(OnResourceDictionaryLoaded));
		}

		private void OnResourceDictionaryLoaded()
		{
			var presentationFrameworkAssembly = typeof(Application).Assembly;

			AddEditorContextMenuDefaultStyle(presentationFrameworkAssembly);
			AddEditorMenuItemDefaultStyle(presentationFrameworkAssembly);
		}

		private void AddEditorContextMenuDefaultStyle(Assembly presentationFrameworkAssembly)
		{
			var contextMenuStyle = Application.Current.FindResource(typeof(ContextMenu)) as Style;
			var editorContextMenuType = Type.GetType("System.Windows.Documents.TextEditorContextMenu+EditorContextMenu, " + presentationFrameworkAssembly);

			if (editorContextMenuType != null)
			{
				var editorContextMenuStyle = new Style(editorContextMenuType, contextMenuStyle);
				Add(editorContextMenuType, editorContextMenuStyle);
			}
		}

		private void AddEditorMenuItemDefaultStyle(Assembly presentationFrameworkAssembly)
		{
			var menuItemStyle = Application.Current.FindResource(typeof(MenuItem)) as Style;
			var editorMenuItemType = Type.GetType("System.Windows.Documents.TextEditorContextMenu+EditorMenuItem, " + presentationFrameworkAssembly);

			if (editorMenuItemType != null)
			{
				var editorContextMenuStyle = new Style(editorMenuItemType, menuItemStyle);
				Add(editorMenuItemType, editorContextMenuStyle);
			}
		}
	}
}
