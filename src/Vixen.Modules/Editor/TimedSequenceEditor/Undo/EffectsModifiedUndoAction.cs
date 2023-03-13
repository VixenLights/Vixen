using Common.Controls;
using Common.Controls.Timeline;
using NLog;

using Vixen.Module;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsModifiedUndoAction : UndoAction
	{
		//private readonly TimedSequenceEditorForm _form;
		private readonly Dictionary<Element, EffectModelCandidate> _changedElements;
		private int _count;
		private readonly string _labelName;
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		public EffectsModifiedUndoAction(Dictionary<Element, EffectModelCandidate> changedElements, string labelName="properties")
		{
			_changedElements = changedElements;
			_labelName = labelName;
			_count = _changedElements.Count();
		}

		public override void Undo()
		{
			SwapEffectData();
			base.Undo();
		}

		public override void Redo()
		{
			SwapEffectData();
			base.Redo();
		}

		protected int Count
		{
			get { return _count; }
		}

		private void SwapEffectData()
		{
			List<Element> keys = new List<Element>(_changedElements.Keys);
			foreach (var element in keys)
			{
				EffectModelCandidate modelCandidate =
						new EffectModelCandidate(element.EffectNode.Effect)
						{
							Duration = element.Duration,
							StartTime = element.StartTime,
							LayerId = _changedElements[element].LayerId
						};
				IModuleDataModel model = _changedElements[element].GetEffectData();

				if (model != null)
				{
					element.EffectNode.Effect.ModuleData = model;
					_changedElements[element] = modelCandidate;
					element.UpdateNotifyContentChanged();
				}
				else
				{
					Logging.Error("EffectModelCandidate.cs - GetEffectData returned null.");

					MessageBoxForm.msgIcon = SystemIcons.Error;
					var messageBox = new MessageBoxForm("Incomplete effect data encountered.  Please close Vixen and restart it.", "Error", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		public override string Description
		{
			get
			{
				return string.Format("{0} Effect(s) {1} modified", Count, _labelName);
			}
		}
	}
}
