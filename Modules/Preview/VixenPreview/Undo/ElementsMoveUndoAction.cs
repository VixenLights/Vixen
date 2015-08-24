using System.Collections.Generic;
using Common.Controls;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class ElementsMoveUndoAction : UndoAction
	{
		private Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> m_changedElements;
		public Preview.VixenPreview.VixenPreviewSetup3 m_form;
		public Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> m_elements;
		public VixenPreviewControl.DisplayMoveType m_type;

		public ElementsMoveUndoAction(Preview.VixenPreview.VixenPreviewSetup3 form, Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> changedElements, VixenPreviewControl.DisplayMoveType type)
		{
			m_changedElements = changedElements;
			m_form = form;
	//		m_elements = item;
			m_type = type;
		}

		public override void Undo()
		{
			//foreach (var items in m_elements)
			//	m_form.RemoveDisplayItem(items);
			//Move Elements back
			m_form.SwapPlaces(m_changedElements);

			base.Undo();
		}

		public override void Redo()
		{
			//foreach (var items in m_elements)
			//	m_form.AddDisplayItem(items);
			//Redo Element Move
			m_form.SwapPlaces(m_changedElements);
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Move {0} ", m_changedElements.Count); }
		}
	}
}