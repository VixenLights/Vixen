using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Vixen.Execution;
using Vixen.Module.PreFilter;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.Sequence;
using Vixen.Module.RuntimeBehavior;
using ISequence = Vixen.Sys.ISequence;

namespace Vixen.IO.Xml {
	class XmlSequenceReader : XmlReaderBase<Sequence> {
		private const string ELEMENT_SEQUENCE = "Sequence";
		//private const string ELEMENT_TIMING_SOURCE = "TimingSource";
		private const string ELEMENT_MODULE_DATA = "ModuleData";
		//private const string ELEMENT_EFFECT_NODES = "EffectNodes";
		//private const string ELEMENT_EFFECT_NODE = "EffectNode";
		//private const string ELEMENT_START_TIME = "StartTime";
		//private const string ELEMENT_TIME_SPAN = "TimeSpan";
		//private const string ELEMENT_TARGET_NODES = "TargetNodes";
		//private const string ELEMENT_TARGET_NODE = "TargetNode";
		private const string ELEMENT_IMPLEMENTATION_CONTENT = "Implementation";
		//private const string ELEMENT_SELECTED_TIMING = "Selected";
		//private const string ELEMENT_FILTER_NODES = "FilterNodes";
		//private const string ELEMENT_FILTER_NODE = "FilterNode";
		//private const string ATTR_TYPE_ID = "typeId";
		//private const string ATTR_INSTANCE_ID = "instanceId";
		//private const string ATTR_ID = "id";
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

			// Filter nodes, reliant upon module data.
			_ReadFilterNodes(element, obj);

			// Subclass implementation data
			_ReadImplementationContent(element, obj);
		}

		private void _ReadTimingSource(XElement element, ISequence sequence) {
			Debug.Assert(sequence != null);
			XmlSelectedTimingProviderSerializer serializer = new XmlSelectedTimingProviderSerializer();
			sequence.TimingProvider.SelectedTimingProvider = serializer.ReadObject(element);
		}
		//private void _ReadTimingSource(XElement element, Sequence sequence) {
		//    element = element.Element(ELEMENT_TIMING_SOURCE).Element(ELEMENT_SELECTED_TIMING);

		//    string providerType = element.Attribute(ATTR_SELECTED_TIMING_TYPE).Value;
		//    string sourceName = element.Attribute(ATTR_SELECTED_TIMING_SOURCE).Value;

		//    if(providerType.Length == 0) providerType = null;
		//    if(sourceName.Length == 0) sourceName = null;

		//    sequence.TimingProvider.SetSelectedSource(providerType, sourceName);
		//}

		private void _ReadModuleData(XElement element, Sequence sequence) {
			// Doing it this way because Sequence has members that are dependent upon
			// data in the dataset and need it to be populated right away.
			string moduleDataString = element.Element(ELEMENT_MODULE_DATA).InnerXml();
			Module.ModuleLocalDataSet dataSet = new Module.ModuleLocalDataSet();
			dataSet.Deserialize(moduleDataString);
			sequence.ModuleDataSet = dataSet;
		}

		private void _ReadEffectNodes(XElement element, Sequence sequence) {
			XmlEffectNodeCollectionSerializer serializer = new XmlEffectNodeCollectionSerializer();
			IEnumerable<EffectNode> effectNodes = serializer.ReadObject(element);

			//***
			//Make sure adding the effect nodes (with instance id) to the sequence gets the
			//instance data set
			sequence.Data.ClearStream();
			sequence.InsertData(effectNodes);

			////Get the effect module instances from the module data.
			//var effectModules = sequence.ModuleDataSet.GetInstances<IEffectModuleInstance>().ToDictionary(x => x.InstanceId);

			//// Get the effect module instance.
			//IEffectModuleInstance effect = effectModules[instanceId];

			//// Add it to the sequence.
			//sequence.InsertData(effectNode);
		}
		//private void _ReadEffectNodes(XElement element, Sequence sequence) {
		//    //Get the effect module instances from the module data.
		//    var effectModules = sequence.ModuleDataSet.GetInstances<IEffectModuleInstance>().ToDictionary(x => x.InstanceId);

		//    // Create a channel node lookup of channels that are currently valid.
		//    var channelNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

		//    sequence.Data.ClearStream();
	
		//    foreach(XElement effectNodeElement in element.Element(ELEMENT_EFFECT_NODES).Elements(ELEMENT_EFFECT_NODE)) {
		//        Guid typeId = Guid.Parse(effectNodeElement.Attribute(ATTR_TYPE_ID).Value);
		//        Guid instanceId = Guid.Parse(effectNodeElement.Attribute(ATTR_INSTANCE_ID).Value);
		//        TimeSpan startTime = TimeSpan.FromTicks(long.Parse(effectNodeElement.Element(ELEMENT_START_TIME).Value));
		//        TimeSpan timeSpan = TimeSpan.FromTicks(long.Parse(effectNodeElement.Element(ELEMENT_TIME_SPAN).Value));
		//        IEnumerable<Guid> targetNodeIds = effectNodeElement
		//            .Element(ELEMENT_TARGET_NODES)
		//            .Elements(ELEMENT_TARGET_NODE)
		//            .Select(x => Guid.Parse(x.Attribute(ATTR_ID).Value));

		//        // Get the effect module instance.
		//        IEffectModuleInstance effect = effectModules[instanceId];
		//        // Set effect members.
		//        // (Target nodes may or may not exist.)
		//        effect.TimeSpan = timeSpan;
		//        IEnumerable<Guid> validChannelIds = targetNodeIds.Intersect(channelNodes.Keys);
		//        effect.TargetNodes = validChannelIds.Select(x => channelNodes[x]).ToArray();
		//        // Wipe out the reference to the default data object so that the sequence
		//        // can assign its data object.
		//        effect.ModuleData = null;
		//        // Wrap the effect in an effect node.
		//        EffectNode effectNode = new EffectNode(effect, startTime);
		//        // Add it to the sequence.
		//        sequence.InsertData(effectNode);
		//    }
		//}

		private void _ReadFilterNodes(XElement element, Sequence sequence) {
			XmlPreFilterNodeCollectionSerializer serializer = new XmlPreFilterNodeCollectionSerializer();
			IEnumerable<PreFilterNode> preFilterNodes =  serializer.ReadObject(element);
			//*** this needs to be done when the filter is added to the sequence
			//// Set the module's data.
			//sequence.ModuleDataSet.GetModuleInstanceData(filter);
			//// Add it to the sequence.
			//sequence.AddPreFilter(filter, timeSpan);
			sequence.ClearPreFilters();
			sequence.AddPreFilters(preFilterNodes);
		}
		//private void _ReadFilterNodes(XElement element, Sequence sequence) {
		//    // Create a channel node lookup of channels that are currently valid.
		//    var channelNodes = VixenSystem.Nodes.Distinct().ToDictionary(x => x.Id);

		//    sequence.ClearPreFilters();

		//    foreach(XElement filterNodeElement in element.Element(ELEMENT_FILTER_NODES).Elements(ELEMENT_FILTER_NODE)) {
		//        Guid typeId = Guid.Parse(filterNodeElement.Attribute(ATTR_TYPE_ID).Value);
		//        Guid instanceId = Guid.Parse(filterNodeElement.Attribute(ATTR_INSTANCE_ID).Value);
		//        TimeSpan startTime = TimeSpan.FromTicks(long.Parse(filterNodeElement.Element(ELEMENT_START_TIME).Value));
		//        TimeSpan timeSpan = TimeSpan.FromTicks(long.Parse(filterNodeElement.Element(ELEMENT_TIME_SPAN).Value));
		//        IEnumerable<Guid> targetNodeIds = filterNodeElement
		//            .Element(ELEMENT_TARGET_NODES)
		//            .Elements(ELEMENT_TARGET_NODE)
		//            .Select(x => Guid.Parse(x.Attribute(ATTR_ID).Value));

		//        // Get the effect module instance.
		//        IPreFilterModuleInstance filter = Modules.ModuleManagement.GetPreFilter(typeId, instanceId);
		//        // Set the module's data.
		//        sequence.ModuleDataSet.GetModuleInstanceData(filter);
				
		//        // Set filter members.
		//        IEnumerable<Guid> validChannelIds = targetNodeIds.Intersect(channelNodes.Keys);
		//        filter.TargetNodes = validChannelIds.Select(x => channelNodes[x]).ToArray();
		//        // Add it to the sequence.
		//        sequence.AddPreFilter(filter, timeSpan);
		//    }
		//}

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
			if(versionAt < 2 && targetVersion >= 2) yield return _Version_1_to_2;
		}

		private XElement _Version_1_to_2(XElement element) {
			element.Add(new XElement(ELEMENT_FILTER_NODES));
			return element;
		}
	}
}
