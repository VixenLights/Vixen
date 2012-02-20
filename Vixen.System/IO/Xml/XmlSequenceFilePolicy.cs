using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Vixen.Module;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	class XmlSequenceFilePolicy : SequenceFilePolicy {
		private Sequence _sequence;
		private XElement _content;

		private const string ATTR_TYPE = "type";
		private const string ATTR_LENGTH = "length";

		public XmlSequenceFilePolicy() {
			// Used when wanting just the current version of the sequence file.
		}

		public XmlSequenceFilePolicy(Sequence sequence, XElement content) {
			_sequence = sequence;
			_content = content;
		}

		protected override void WriteType() {
			_content.Add(new XAttribute(ATTR_TYPE, _sequence.Type));
		}

		protected override void WriteLength() {
			_content.Add(new XAttribute(ATTR_LENGTH, _sequence.Length.Ticks));
		}

		protected override void WriteTimingSource() {
			XmlSelectedTimingProviderSerializer serializer = new XmlSelectedTimingProviderSerializer();
			XElement element = serializer.WriteObject(_sequence.TimingProvider.SelectedTimingProvider);
			_content.Add(element);
		}

		protected override void WriteModuleData() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			XElement element = serializer.WriteObject(_sequence.ModuleDataSet);
			_content.Add(element);
		}

		protected override void WriteEffectNodes() {
			XmlEffectNodeCollectionSerializer serializer = new XmlEffectNodeCollectionSerializer();
			XElement element = serializer.WriteObject(_sequence.Data.GetMainStreamData().Cast<EffectNode>());
			_content.Add(element);
		}

		protected override void WriteFilterNodes() {
			XmlPreFilterNodeCollectionSerializer serializer = new XmlPreFilterNodeCollectionSerializer();
			XElement element = serializer.WriteObject(_sequence.GetPreFilters());
			_content.Add(element);
		}

		protected override void ReadType() {
			string type = XmlHelper.GetAttribute(_content, ATTR_TYPE);

			SequenceType value = SequenceType.Standard;
			if(type != null) {
				value = (SequenceType)Enum.Parse(typeof(SequenceType), type);
			}

			_sequence.SequenceType = value;
		}

		protected override void ReadLength() {
			string length = XmlHelper.GetAttribute(_content, ATTR_LENGTH);

			TimeSpan value = TimeSpan.Zero;
			if(length != null) {
				value = TimeSpan.FromTicks(long.Parse(length));
			}

			_sequence.Length = value;
		}

		protected override void ReadTimingSource() {
			XmlSelectedTimingProviderSerializer serializer = new XmlSelectedTimingProviderSerializer();
			_sequence.TimingProvider.SelectedTimingProvider = serializer.ReadObject(_content);
		}

		protected override void ReadModuleData() {
			XmlModuleLocalDataSetSerializer serializer = new XmlModuleLocalDataSetSerializer();
			_sequence.ModuleDataSet = serializer.ReadObject(_content);
			// Side effect: With the module dataset now being available, get the sequence's runtime
			// behaviors' data.
			_GetBehaviorData(_sequence);
		}

		protected override void ReadEffectNodes() {
			XmlEffectNodeCollectionSerializer serializer = new XmlEffectNodeCollectionSerializer();
			IEnumerable<EffectNode> effectNodes = serializer.ReadObject(_content);
			_sequence.Data.ClearStream();
			_sequence.InsertData(effectNodes);
		}

		protected override void ReadFilterNodes() {
			XmlPreFilterNodeCollectionSerializer serializer = new XmlPreFilterNodeCollectionSerializer();
			IEnumerable<PreFilterNode> preFilterNodes = serializer.ReadObject(_content);
			_sequence.ClearPreFilters();
			_sequence.AddPreFilters(preFilterNodes);
		}

		protected override void ReadMediaData() {
			_sequence.Media = new MediaCollection(_sequence.ModuleDataSet);
		}

		private void _GetBehaviorData(Sequence sequence) {
			foreach(IModuleInstance runtimeBehavior in sequence.RuntimeBehaviors) {
				sequence.ModuleDataSet.GetModuleTypeData(runtimeBehavior);
			}
		}
	}
}
