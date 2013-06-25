using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;
using System.Drawing;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevel : EffectModuleInstanceBase
	{
		private SetLevelData _data;
		private EffectIntents _elementData = null;

		public SetLevel()
		{
			_data = new SetLevelData();
		}

		protected override void _PreRender()
		{
			_elementData = new EffectIntents();

			foreach (ElementNode node in TargetNodes) {
				if (node != null)
					RenderNode(node);
			}
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as SetLevelData; }
		}

		[Value]
		public double IntensityLevel
		{
			get { return _data.level; }
			set { _data.level = value; IsDirty = true; }
		}

		[Value]
		public Color Color
		{
			get { return _data.color; }
			set { _data.color = value; IsDirty = true; }
		}

		// renders the given node to the internal ElementData dictionary. If the given node is
		// not a element, will recursively descend until we render its elements.
		private void RenderNode(ElementNode node)
		{
			foreach(ElementNode elementNode in node.GetLeafEnumerator()) {
				LightingValue lightingValue = new LightingValue(Color, (float)IntensityLevel);
				IIntent intent = new LightingIntent(lightingValue, lightingValue, TimeSpan);
				_elementData.AddIntentForElement(elementNode.Element.Id, intent, TimeSpan.Zero);
			}
		}
	}
}
