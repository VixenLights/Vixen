using Common.Controls;
using Common.Controls.Timeline;
using Vixen.Module.Effect;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsPropertyModifiedUndoAction:UndoAction
	{
		public EffectsPropertyModifiedUndoAction(Dictionary<Element, Tuple<Object, PropertyDetail>> effectPropertyValues)
		{
			if (effectPropertyValues == null) throw new ArgumentNullException("effectPropertyValues");
			ElementValues = effectPropertyValues;
			if (effectPropertyValues.Count > 0)
				DisplayName = effectPropertyValues.First().Value.Item2.Name;
		}

		public Dictionary<Element, Tuple<Object, PropertyDetail>> ElementValues { get; private set; }
		public string DisplayName { get; private set; }
		public override void Undo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<object, PropertyDetail> value = ElementValues[element];
				var property = value.Item1;
				var propValue = value.Item2;

				// Save the current property (i.e. color, gradient, curve)
				var saveProperty = propValue.PropertyDescriptor.GetValue(propValue.Effect);

				// Set the new property (i.e. color, gradient, curve)
				element.EffectNode.Effect.UpdateProperty(propValue.PropertyDescriptor, propValue.Effect, property);
				
				// Update the Effect
				element.UpdateNotifyContentChanged();

				// Save off for a Redo.
				ElementValues[element] = new Tuple<object, PropertyDetail>(saveProperty, propValue);
			}

			base.Undo();
		}

		public override void Redo()
		{
			foreach (var element in ElementValues.Keys.ToList())
			{
				Tuple<object, PropertyDetail> value = ElementValues[element];
				var property = value.Item1;
				var propValue = value.Item2;

				// Save the current property
				var saveProperty = propValue.PropertyDescriptor.GetValue(propValue.Effect);

				// Set the new property
				element.EffectNode.Effect.UpdateProperty(propValue.PropertyDescriptor, propValue.Effect, property);

				// Update the Effect
				element.UpdateNotifyContentChanged();

				// Save off for an Undo.
				ElementValues[element] = new Tuple<object, PropertyDetail>(saveProperty, propValue);
			}
			
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("{0} Effect(s) {1} modified", ElementValues.Count, DisplayName);; } 
		}
	}
}
