using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controls;
using Common.Controls.Timeline;
using Vixen.Sys;
using Element = Common.Controls.Timeline.Element;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public class ElementsTimeChangedUndoAction : Common.Controls.UndoAction
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
					case ElementMoveType.Align:
						return string.Format("Aligned {0} effect{1}", m_changedElements.Count, s);
					case ElementMoveType.Distribute:
						return string.Format("Distributed {0} effect{1}", m_changedElements.Count, s);
					default:
						throw new Exception("Unknown ElementMoveType!");
				}
			}
		}
	}


	public class EffectsAddedRemovedUndoAction : Common.Controls.UndoAction
	{
		private TimedSequenceEditorForm m_form;
		private IEnumerable<EffectNode> m_effectNodes;
		private int m_count;

		public EffectsAddedRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
		{
			m_form = form;
			m_effectNodes = nodes;
			m_count = m_effectNodes.Count();
		}

		protected void removeEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.RemoveEffectNodeAndElement(node);
		}

		protected void addEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.AddEffectNode(node);
		}

		protected int Count
		{
			get { return m_count; }
		}
	}

	public class EffectsAddedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsAddedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			removeEffects();
			base.Undo();
		}

		public override void Redo()
		{
			addEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Added {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}

	public class EffectsPastedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsPastedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			removeEffects();
			base.Undo();
		}

		public override void Redo()
		{
			addEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Paste {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}

	public class EffectsRemovedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			addEffects();
			base.Undo();
		}

		public override void Redo()
		{
			removeEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Removed {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}

	public class EffectsCutUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsCutUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			addEffects();
			base.Undo();
		}

		public override void Redo()
		{
			removeEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Cut {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}