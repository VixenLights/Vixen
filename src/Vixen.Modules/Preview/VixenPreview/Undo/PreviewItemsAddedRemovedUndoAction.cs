using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsAddedRemovedUndoAction : Common.Controls.UndoAction
	{
		private VixenPreviewControl m_form;
		public IEnumerable<DisplayItem> m_elements;
		private int m_count;

		public PreviewItemsAddedRemovedUndoAction(VixenPreviewControl form, IEnumerable<DisplayItem> items)
		{
			m_form = form;
			m_elements = items;
			m_count = m_elements.Count();
		}

		protected void removeEffects()
		{
			foreach (var items in m_elements)
				m_form.RemoveDisplayItem(items);
			m_form.Refresh();
		}

		protected void addEffects()
		{
			foreach (var items in m_elements)
				m_form.AddDisplayItem(items);
			m_form.Refresh();
		}

		protected int Count
		{
			get { return m_count; }
		}
	}
}