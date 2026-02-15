using Common.Controls;
using Common.Controls.Timeline;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsPropertyModifiedUndoAction:UndoAction
	{
		public EffectsPropertyModifiedUndoAction(Dictionary<Element, Tuple<Object, PropertyMetaData>> effectPropertyValues)
		{
			if (effectPropertyValues == null) throw new ArgumentNullException(nameof(effectPropertyValues));
			ElementValues = effectPropertyValues;
			if (effectPropertyValues.Count > 0)
				DisplayName = effectPropertyValues.First().Value.Item2.DisplayName;
		}

		public Dictionary<Element, Tuple<Object, PropertyMetaData>> ElementValues { get; private set; }
		public string DisplayName { get; init; }
		public override void Undo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<Object, PropertyMetaData> value = ElementValues[element];
				object temp = value.Item2.Descriptor.GetValue(value.Item2.Owner);
				value.Item2.Descriptor.SetValue(value.Item2.Owner, ElementValues[element].Item1);
				ElementValues[element] = new Tuple<object, PropertyMetaData>(temp,value.Item2);
				element.UpdateNotifyContentChanged();
			}
			

			base.Undo();
		}

		public override void Redo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<Object, PropertyMetaData> value = ElementValues[element];
				object temp = value.Item2.Descriptor.GetValue(value.Item2.Owner);
				value.Item2.Descriptor.SetValue(value.Item2.Owner, ElementValues[element].Item1);
				ElementValues[element] = new Tuple<object, PropertyMetaData>(temp, value.Item2);
				element.UpdateNotifyContentChanged();
			}
			
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("{0} Effect(s) {1} modified", ElementValues.Count, DisplayName);; } 
		}
	}
}
