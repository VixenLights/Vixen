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

			foreach (ElementNode node in TargetNodes)
			{
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
			foreach (Element element in node)
			{
				LightingValue lightingValue = new LightingValue(Color, (float)IntensityLevel);
				IIntent intent = new LightingIntent(lightingValue, lightingValue, TimeSpan);
				_elementData.AddIntentForElement(element.Id, intent, TimeSpan.Zero);
			}
			//// if this node is an RGB node, then it will know what to do with it (might render directly,
			//// might be broken down into sub-elements, etc.) So just pass it off to that instead.
			//if (node.Properties.Contains(SetLevelDescriptor._RGBPropertyId)) {
			//    RenderRGB(node);
			//} else {
			//    if (node.IsLeaf) {
			//        RenderMonochrome(node);
			//    } else {
			//        foreach (ElementNode child in node.Children)
			//            RenderNode(child);
			//    }
			//}
		}

		//// renders a single command for each element in the given node with the specified monochrome level.
		//private void RenderMonochrome(ElementNode node)
		//{
		//    // this is probably always going to be a single element for the given node, as
		//    // we have iterated down to leaf nodes in RenderNode() above. May as well do
		//    // it this way, though, in case something changes in future.
		//    foreach (Element element in node.GetElementEnumerator()) {
		//        //Command setLevelCommand = new Lighting.Monochrome.SetLevel(Level);
		//        //CommandNode data = new CommandNode(setLevelCommand, TimeSpan.Zero, TimeSpan);

		//        //IIntentModuleInstance intent = ApplicationServices.Get<IIntentModuleInstance>(_levelIntentId);
		//        //intent.TimeSpan = TimeSpan;
		//        //IntentNode data = new IntentNode(intent, TimeSpan.Zero);
		//        if(element != null) {
		//            //_elementData[element.Id] = new[] {data};
		//            LightingValue lightingValue = new LightingValue(Color.White, IntensityLevel);
		//            IIntent intent = new LightingLinearIntent(lightingValue, lightingValue, TimeSpan);
		//            _elementData.AddIntentForElement(element.Id, intent, TimeSpan.Zero);
		//        }
		//    }
		//}

		//private void RenderRGB(ElementNode node)
		//{
		//    //// get the RGB property for the element, and get it to render the data for us
		//    //RGBModule rgbProperty = node.Properties.Get(SetLevelDescriptor._RGBPropertyId) as RGBModule;
		//    //ElementCommands rgbData = rgbProperty.RenderColorToCommands(Color, Level);

		//    //// iterate through the rendered commands, adjust them to fit our times, and add them to our rendered data
		//    //foreach (KeyValuePair<Guid, Command[]> kvp in rgbData) {
		//    //    foreach (Command c in kvp.Value) {
		//    //        //TODO
		//    //        //CommandNode newCommandNode = new CommandNode(c, TimeSpan.Zero, TimeSpan);
		//    //        //_elementData.AddCommandNodeForElement(kvp.Key, newCommandNode);
		//    //    }
		//    //}
		//    foreach(Element element in node.GetElementEnumerator()) {
		//        LightingValue lightingValue = new LightingValue(Color, IntensityLevel);
		//        IIntent intent = new LightingLinearIntent(lightingValue, lightingValue, TimeSpan);
		//        _elementData.AddIntentForElement(element.Id, intent, TimeSpan.Zero);
		//    }
		//}
	}
}
