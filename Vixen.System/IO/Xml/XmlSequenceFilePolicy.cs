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

		private const int SEQUENCE_FILE_VERSION = 1;
		private const string ATTR_LENGTH = "length";

		public XmlSequenceFilePolicy() {
			// Used when wanting just the current version of the sequence file.
		}

		public XmlSequenceFilePolicy(Sequence sequence, XElement content) {
			//write: needs sequence to read from and content to write to
			//read: needs sequence to read to and content to read from
			_sequence = sequence;
			_content = content;
		}

		public override int GetVersion() {
			return SEQUENCE_FILE_VERSION;
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
			XmlModuleDataSetSerializer serializer = new XmlModuleDataSetSerializer();
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

		protected override void ReadLength() {
			XAttribute attribute = _content.Attribute(ATTR_LENGTH);

			TimeSpan length = TimeSpan.Zero;
			if(attribute != null) {
				length = TimeSpan.FromTicks(long.Parse(attribute.Value));
			}

			_sequence.Length = length;
		}

		protected override void ReadTimingSource() {
			XmlSelectedTimingProviderSerializer serializer = new XmlSelectedTimingProviderSerializer();
			_sequence.TimingProvider.SelectedTimingProvider = serializer.ReadObject(_content);
		}

		protected override void ReadModuleData() {
			XmlModuleDataSetSerializer serializer = new XmlModuleDataSetSerializer();
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
