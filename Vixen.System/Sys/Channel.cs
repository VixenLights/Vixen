using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	abstract public class Channel : IEquatable<Channel> {
		protected Channel() { }

		// It would make sense to have the default constructor create an id by default,
		// but creating a GUID can be costly when done thousands of times.  The default
		// constructor is used by ChannelReader as it is a generic taking a Channel<>.
		public Channel(string name) {
			Name = name;
			Id = Guid.NewGuid();
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

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
	}
}
