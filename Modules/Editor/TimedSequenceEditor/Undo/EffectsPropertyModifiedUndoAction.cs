using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Controls;
using Common.Controls.Timeline;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsPropertyModifiedUndoAction:UndoAction
	{
		public EffectsPropertyModifiedUndoAction(Dictionary<Element, Object> objValues, PropertyDescriptor property)
		{
			ElementValues = objValues;
			Property = property;
			
		}

		public Dictionary<Element, Object> ElementValues { get; private set; }
		public PropertyDescriptor Property { get; private set; }

		public override void Undo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				object temp = Property.GetValue(element.EffectNode.Effect);
				Property.SetValue(element.EffectNode.Effect, ElementValues[element]);
				ElementValues[element] = temp;
				element.UpdateNotifyContentChanged();
			}
			

			base.Undo();
		}

		public override void Redo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Property.SetValue(element.EffectNode.Effect, ElementValues[element]);
				element.UpdateNotifyContentChanged();
			}
			
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("{0} Effect(s) {1} modified", ElementValues.Count, Property.DisplayName);; } 
		}
	}
}
