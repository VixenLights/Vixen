using System;
using System.Collections.Generic;
using System.Linq;
using Common.Controls;
using Common.Controls.Timeline;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class ElementsTimeChangedUndoAction : UndoAction
	{
		private Dictionary<Element, ElementTimeInfo> m_changedElements;
		private ElementMoveType m_moveType;
		private TimedSequenceEditorForm m_form;

		public ElementsTimeChangedUndoAction(TimedSequenceEditorForm form, Dictionary<Element, ElementTimeInfo> changedElements, ElementMoveType moveType)
			: base()
		{
			m_changedElements = changedElements;
			m_moveType = moveType;
			m_form = form;
		}


		public override void Undo()
		{
			m_form.SwapPlaces(m_changedElements);

			base.Undo();
		}

		public override void Redo()
		{

			//Check to see if our element still exists, or has been replaced because of a delete and add manuver
			Dictionary <Element, ElementTimeInfo> validatedElements = new Dictionary<Element, ElementTimeInfo>();
			foreach (var elementTimeInfo in m_changedElements)
			{
				if (!elementTimeInfo.Key.Row.ContainsElement(elementTimeInfo.Key))
				{
					var el = elementTimeInfo.Key.Row.FirstOrDefault(e => e.EffectNode.Equals(elementTimeInfo.Key.EffectNode));
					if (el != null)
					{
						validatedElements.Add(el, elementTimeInfo.Value);
					}
				}
				else
				{
					validatedElements.Add(elementTimeInfo.Key, elementTimeInfo.Value);
				}
			}

			m_changedElements = validatedElements;

			m_form.SwapPlaces(m_changedElements);

			base.Redo();
		}

		public override string Description
		{
			get
			{
				string s = (m_changedElements.Count == 1 ? string.Empty : "s");
				switch (m_moveType) {
					case ElementMoveType.Move:
						return string.Format("Moved {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.Resize:
						return string.Format("Resize {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignStart:
						return string.Format("Start Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignEnd:
						return string.Format("End Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignBoth:
						return string.Format("Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignDurations:
						return string.Format("Duration Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignStartToEnd:
						return string.Format("Start to End Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignEndToStart:
						return string.Format("End to Start Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.AlignCenters:
						return string.Format("Center Align {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.Distribute:
						return string.Format("Distributed {0} effect{1}", m_changedElements.Count, s);
					default:
						throw new Exception("Unknown ElementMoveType!");
				}
			}
		}
	}
}