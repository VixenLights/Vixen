using System;
using System.Collections.Generic;
using Common.Controls;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class ElementsResizingUndoAction : UndoAction
	{
		private Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> m_changedElements;
		public VixenPreviewControl m_form;
		public Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> m_elements;
		public static VixenPreviewControl.DisplayMoveType m_type;

		public ElementsResizingUndoAction(VixenPreviewControl form, Dictionary<DisplayItem, VixenPreviewControl.ElementPositionInfo> changedElements, VixenPreviewControl.DisplayMoveType type)
		{
			m_changedElements = changedElements;
			m_form = form;
			m_type = type;
		}

		public override void Undo()
		{
	//		m_form.SelectedDisplayItems.Clear();
			m_form.resizeUndoElement(m_changedElements);
			m_form.ResizeSwapPlaces(m_changedElements);
			base.Undo();
		}

		public override void Redo()
		{
	//		m_form.SelectedDisplayItems.Clear();
			m_form.resizeUndoElement(m_changedElements);
			m_form.ResizeSwapPlaces(m_changedElements);
			base.Redo();
		}

		public override string Description
		{
			get
			{
				return string.Format("Resize {0}", m_changedElements.Count);
			}
		}
	}
}