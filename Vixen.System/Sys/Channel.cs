using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using CommandStandard;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	abstract public class Channel : IEquatable<Channel> {
		private Patch _patch;

		// Used only by subclasses so they can be generic parameters supporting new().
		protected Channel() { }

		// It would make sense to have the default constructor create an id by default,
		// but creating a GUID can be costly when done thousands of times.  The default
		// constructor is used by ChannelReader as it is a generic taking a Channel<>.
		public Channel(string name) {
			Name = name;
			Id = Guid.NewGuid();
			this.Patch = new Patch();
		}

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
		public string Name { get; set; }
		public Guid Id { get; set; }

		public bool Masked {
			get { return !this.Patch.Enabled; }
			set { this.Patch.Enabled = !value; }
		}

		// Both of these are required for Except(), Distinct(), Union() and Intersect().
		// Equals(<type>) for IEquatable and GetHashCode() because that's needed anytime
		// Equals(object) is overridden (which it really isn't, but this is what is said and
		// it doesn't work otherwise).
		public bool Equals(Channel other) {
			return this.Id == other.Id;
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return Name;
		}

		abstract public void Clear();

		static public XElement WriteXml(Channel channel) {
			XElement element = new XElement("Channel",
				new XAttribute("id", channel.Id),
				new XAttribute("name", channel.Name),
				new XElement("Patch",
					channel.Patch.ControllerReferences.Select(x => ControllerReference.WriteXml(x))));
			return element;
		}

		static public T ReadXml<T>(XElement element)
			where T : Channel, new() {
			T instance = new T() {
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
