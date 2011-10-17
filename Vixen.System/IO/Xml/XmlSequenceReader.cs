using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.Sequence;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.IO.Xml {
	class XmlSequenceReader : XmlReaderBase<Sequence> {
		private const string ELEMENT_SEQUENCE = "Sequence";
		private const string ELEMENT_TIMING_SOURCE = "TimingSource";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		private const string ELEMENT_EFFECT_NODES = "EffectNodes";
		private const string ELEMENT_EFFECT_NODE = "EffectNode";
		private const string ELEMENT_START_TIME = "StartTime";
		private const string ELEMENT_TIME_SPAN = "TimeSpan";
		private const string ELEMENT_TARGET_NODES = "TargetNodes";
		private const string ELEMENT_TARGET_NODE = "TargetNode";
		private const string ELEMENT_IMPLEMENTATION_CONTENT = "Implementation";
		private const string ELEMENT_SELECTED_TIMING = "Selected";
		private const string ATTR_EFFECT_TYPE_ID = "typeId";
		private const string ATTR_EFFECT_INSTANCE_ID = "instanceId";
		private const string ATTR_ID = "id";
		private const string ATTR_LENGTH = "length";
		private const string ATTR_SELECTED_TIMING_TYPE = "type";
		private const string ATTR_SELECTED_TIMING_SOURCE = "source";

		override protected Sequence _CreateObject(XElement element, string filePath) {
			// Get the specific sequence module manager.
			SequenceModuleManagement manager = Modules.GetManager<ISequenceModuleInstance, SequenceModuleManagement>();
			// Get an instance of the appropriate sequence module.
			Sequence sequence = manager.Get(filePath) as Sequence;
			if(sequence == null) throw new InvalidOperationException("No sequence type defined for file " + filePath);

			sequence.FilePath = filePath;

			return sequence;
		}

		protected override void _PopulateObject(Sequence obj, XElement element) {
			//Already referencing the doc element.
			obj.Length = TimeSpan.FromTicks(long.Parse(element.Attribute(ATTR_LENGTH).Value));

			// Timing
			_ReadTimingSource(element, obj);

			// Module data
			_ReadModuleData(element, obj);

			// Things that need to wait for other sequence data:

			// Runtime behavior module data
			_ReadBehaviorData(element, obj);

			// Media module data
			_ReadMedia(element, obj);

			// Effect nodes, reliant upon module data.
			_ReadEffectNodes(element, obj);

			// Subclass implementation data
			_ReadImplementationContent(element, obj);
		}

		private void _ReadTimingSource(XElement element, Sequence sequence) {
			element = element.Element(ELEMENT_TIMING_SOURCE).Element(ELEMENT_SELECTED_TIMING);

			string providerType = element.Attribute(ATTR_SELECTED_TIMING_TYPE).Value;
			string sourceName = element.Attribute(ATTR_SELECTED_TIMING_SOURCE).Value;

			if(providerType.Length == 0) providerType = null;
			if(sourceName.Length == 0) sourceName = null;

			sequence.TimingProvider.SetSelectedSource(providerType, sourceName);
		}

		private void _ReadModuleData(XElement element, Sequence sequence) {
			string moduleDataString = element.Element(ELEMENT_MODULE_DATA).InnerXml();
			sequence.ModuleDataSet.Deserialize(moduleDataString);
		}

		private void _ReadEffectNodes(XElement element, Sequence sequence) {
			//Get the effect module instances from the module data.
			var effectModules = sequence.ModuleDataSet.GetInstances<IEffectModuleInstance>().ToDictionary(x => x.InstanceId);

			// Create a channel node lookup.
			var channelNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

			sequence.Data.ClearEffectStream();
	
			foreach(XElement effectNodeElement in element.Element(ELEMENT_EFFECT_NODES).Elements(ELEMENT_EFFECT_NODE)) {
				Guid typeId = Guid.Parse(effectNodeElement.Attribute(ATTR_EFFECT_TYPE_ID).Value);
				Guid instanceId = Guid.Parse(effectNodeElement.Attribute(ATTR_EFFECT_INSTANCE_ID).Value);
				TimeSpan startTime = TimeSpan.FromTicks(long.Parse(effectNodeElement.Element(ELEMENT_START_TIME).Value));
				TimeSpan timeSpan = TimeSpan.FromTicks(long.Parse(effectNodeElement.Element(ELEMENT_TIME_SPAN).Value));
				IEnumerable<Guid> targetNodeIds = effectNodeElement
					.Element(ELEMENT_TARGET_NODES)
					.Elements(ELEMENT_TARGET_NODE)
					.Select(x => Guid.Parse(x.Attribute(ATTR_ID).Value));

				// Get the effect module instance.
				IEffectModuleInstance effect = effectModules[instanceId];
				// Set effect members.
				// (Target nodes may or may not exist.)
				effect.TimeSpan = timeSpan;
				IEnumerable<Guid> validChannelIds = targetNodeIds.Intersect(channelNodes.Keys);
				effect.TargetNodes = validChannelIds.Select(x => channelNodes[x]).ToArray();
				// Wipe out the reference to the default data object so that the sequence
				// can assign its data object.
				effect.ModuleData = null;
				// Wrap the effect in an effect node.
				EffectNode effectNode = new EffectNode(effect, startTime);
				// Add it to the sequence.
				sequence.InsertData(effectNode);
			}
		}

		private  void _ReadBehaviorData(XElement element, Sequence sequence) {
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in sequence.RuntimeBehaviors) {
				sequence.ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}

		private  void _ReadMedia(XElement element, Sequence sequence) {
			sequence.Media = new MediaCollection(sequence.ModuleDataSet);
		}

		private void _ReadImplementationContent(XElement element, Sequence sequence) {
			element = element.Element(ELEMENT_IMPLEMENTATION_CONTENT);
			_ReadContent(element, sequence);
		}

		virtual protected void _ReadContent(XElement element, Sequence sequence) { }

		protected override IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}
	}
}
