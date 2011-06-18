using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Xml.Linq;
using Vixen.Common;

namespace Vixen.Sys {
	public class OutputChannel : Channel, IEnumerable<CommandData>, IEqualityComparer<OutputChannel> {
		private Patch _patch;
		private ConcurrentQueue<CommandData> _data = new ConcurrentQueue<CommandData>();

		public OutputChannel(string name)
			: base(name) {
			this.Patch = new Patch();
		}

		private OutputChannel() { }

		public Patch Patch {
			get { return _patch; }
			set {
				// Want any controller references to be properly removed.
				if(_patch != null) {
					_patch.Clear();
				}
				_patch = value;
			}
		}

		public bool Masked {
			get { return !this.Patch.Enabled; }
			set { this.Patch.Enabled = !value; }
		}
		
		public IEnumerator<CommandData> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new ConcurrentQueueLiveEnumerator<CommandData>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(IEnumerable<CommandData> data) {
			foreach(CommandData dataElement in data) {
				_data.Enqueue(dataElement);
			}
		}

		public void AddData(CommandData data) {
			_data.Enqueue(data);
		}

		public override void Clear() {
			_data = new ConcurrentQueue<CommandData>();
		}

		public bool Equals(OutputChannel x, OutputChannel y) {
			return x.Id == y.Id;
		}

		public int GetHashCode(OutputChannel obj) {
			return obj.Id.GetHashCode();
		}


		static public XElement WriteXml(OutputChannel channel) {
			XElement element = new XElement("Channel",
				new XAttribute("id", channel.Id),
				new XAttribute("name", channel.Name),
				new XElement("Patch",
					channel.Patch.ControllerReferences.Select(x => ControllerReference.WriteXml(x))));
			return element;
		}

		static public OutputChannel ReadXml(XElement element) {
			OutputChannel instance = new OutputChannel() {
				Id = new Guid(element.Attribute("id").Value),
				Name = element.Attribute("name").Value,
				Patch = new Patch(
					element.Element("Patch").Elements()
						.Select(x => ControllerReference.ReadXml(x))
					),
			};
			return instance;
		}
	}
}
