using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Common;

namespace Vixen.Sys {
	public class InputChannel : Channel , IEnumerable<CommandNode> {
		private List<CommandNode> _data = new List<CommandNode>();

		public InputChannel(string name)
			: base(name) {
		}

		private InputChannel() { }

		public IEnumerator<CommandNode> GetEnumerator() {
			// We need an enumerator that is live and does not operate upon a snapshot
			// of the data.
			return new LiveListEnumerator<CommandNode>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void AddData(CommandNode data) {
			_data.Add(data);
		}

		public void AddData(IEnumerable<CommandNode> data) {
			_data.AddRange(data);
		}

		public override void Clear() {
			_data.Clear();
		}


		// Not actually used, but kept around in case there's a later need.
		//static public XElement WriteXml(Channel channel) {
		//    XElement element = new XElement("Channel",
		//        new XAttribute("id", channel.Id),
		//        new XAttribute("name", channel.Name));
		//    return element;
		//}

		//static public InputChannel ReadXml(XElement element) {
		//    InputChannel instance = new InputChannel() {
		//        Id = new Guid(element.Attribute("id").Value),
		//        Name = element.Attribute("name").Value
		//    };
		//    return instance;
		//}
	}
}
