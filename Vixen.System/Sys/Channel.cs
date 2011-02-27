using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using CommandStandard;
using System.Collections.Concurrent;

namespace Vixen.Sys {
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	abstract public class Channel : IEquatable<Channel> {
		// It would make sense to have the default constructor create an id by default,
		// but creating a GUID can be costly when done thousands of times.  The default
		// constructor is used by ChannelReader as it is a generic taking a Channel<>.
		public Channel()
			: this(false) {
		}

		public Channel(bool createId) {
			if(createId) Id = Guid.NewGuid();
			this.Patch = new Patch();
			AllowFrameSkip = true;
		}

		public Fixture ParentFixture { get; set; }
		public Patch Patch { get; private set; }
		public bool AllowFrameSkip { get; set; }
		public string Name { get; set; }
		public Guid Id { get; set; }

		public bool Masked {
			get { return !this.Patch.Enabled; }
			set { this.Patch.Enabled = !value; }
		}

		virtual public Channel Clone(Fixture parentFixture) {
			Channel other = this.MemberwiseClone() as Channel;
			other.ParentFixture = parentFixture;
			other.Patch = this.Patch.Clone();
			return other;
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

		abstract public void Clear();
	}
}
