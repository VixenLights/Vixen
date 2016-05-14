using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common.Controls;
using Common.Controls.Timeline;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsPropertyModifiedUndoAction:UndoAction
	{
		public EffectsPropertyModifiedUndoAction(Dictionary<Element, Tuple<Object, PropertyDescriptor>> effectPropertyValues)
		{
			if (effectPropertyValues == null) throw new ArgumentNullException("effectPropertyValues");
			ElementValues = effectPropertyValues;
			if (effectPropertyValues.Count > 0)
				DisplayName = effectPropertyValues.First().Value.Item2.DisplayName;
		}

		public Dictionary<Element, Tuple<Object, PropertyDescriptor>> ElementValues { get; private set; }
		public string DisplayName { get; private set; }
		public override void Undo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<Object, PropertyDescriptor> value = ElementValues[element];
				object temp = value.Item2.GetValue(element.EffectNode.Effect);
				value.Item2.SetValue(element.EffectNode.Effect, ElementValues[element].Item1);
				ElementValues[element] = new Tuple<object, PropertyDescriptor>(temp,value.Item2);
				element.UpdateNotifyContentChanged();
			}
			

			base.Undo();
		}

		public override void Redo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<Object, PropertyDescriptor> value = ElementValues[element];
				object temp = value.Item2.GetValue(element.EffectNode.Effect);
				value.Item2.SetValue(element.EffectNode.Effect, ElementValues[element].Item1);
				ElementValues[element] = new Tuple<object, PropertyDescriptor>(temp, value.Item2);
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
