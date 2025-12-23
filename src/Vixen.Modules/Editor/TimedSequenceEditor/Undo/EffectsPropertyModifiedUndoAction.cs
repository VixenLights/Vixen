using System.ComponentModel;
using Common.Controls;
using Common.Controls.Timeline;
using ExCSS;
using Newtonsoft.Json.Linq;
using VixenModules.Effect.Effect;
using Vixen.Module.Effect;

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
			object previousPropertyData;

			foreach (var element in ElementValues.Keys.ToList())
			{
				object effectDetail, parentEffect = null;
				object property;

				Tuple<object, PropertyDescriptor> value = ElementValues[element];
				if (value.Item1 is Tuple<object, object, object> tupleValue)
				{
					Tuple<object, object, object> propertyData = (Tuple<object, object, object>)value.Item1;
					effectDetail = propertyData.Item1;
					parentEffect = propertyData.Item2;
					property = propertyData.Item3;
				}
				else
				{
					property = value.Item1;
					effectDetail = element.EffectNode.Effect;
				}

				PropertyDescriptor propValue = (PropertyDescriptor)value.Item2;

				// Save the current property (i.e. color, gradient, curve)
				var saveProperty = propValue.GetValue(effectDetail);

				// Set the new property (i.e. color, gradient, curve)
				propValue.SetValue(effectDetail, property);
				
				// Update the Effect
				element.UpdateNotifyContentChanged();

				// If this is a Wave or Liquid...
				if (parentEffect != null)
				{
					// Save the previous property data
					previousPropertyData = new Tuple<object, object, object>(effectDetail, parentEffect, saveProperty);
					((EffectModuleInstanceBase)parentEffect).UpdateNotifyContentChanged();
				}

				// Else all other effects
				else
				{
					// Save the previous color
					previousPropertyData = saveProperty;
				}

				// Save off for a Redo.
				ElementValues[element] = new Tuple<object, PropertyDescriptor>(previousPropertyData, value.Item2);
			}


			base.Undo();
		}

		public override void Redo()
		{
			object previousColorData;

			foreach (var element in ElementValues.Keys.ToList())
			{
				object effectDetail, parentEffect = null;
				object gradient;

				Tuple<object, PropertyDescriptor> value = ElementValues[element];
				if (value.Item1 is Tuple<object, object, object> tupleValue)
				{
					Tuple<object, object, object> colorData = (Tuple<object, object, object>)value.Item1;
					effectDetail = colorData.Item1;
					parentEffect = colorData.Item2;
					gradient = colorData.Item3;
				}
				else
				{
					gradient = value.Item1;
					effectDetail = element.EffectNode.Effect;
				}

				PropertyDescriptor propValue = (PropertyDescriptor)value.Item2;

				// Save the current color
				var saveColor = propValue.GetValue(effectDetail);

				// Set the new color
				propValue.SetValue(effectDetail, gradient);
				
				// Update the Effect
				element.UpdateNotifyContentChanged();

				// If this is a Wave or Liquid...
				if (parentEffect != null)
				{
					// Save the previous color data
					previousColorData = new Tuple<object, object, object>(effectDetail, parentEffect, saveColor);
					((EffectModuleInstanceBase)parentEffect).UpdateNotifyContentChanged();
				}

				// Else all other effects
				else
				{
					// Save the previous color
					previousColorData = saveColor;
				}

				// Save off for an Undo.
				ElementValues[element] = new Tuple<object, PropertyDescriptor>(previousColorData, value.Item2);
			}
			
			base.Redo();
		}

public override string Description
		{
			get { return string.Format("{0} Effect(s) {1} modified", ElementValues.Count, DisplayName);; } 
		}
	}
}
