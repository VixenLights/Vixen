using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Common;
using System.IO;
using Vixen.Sequence;

namespace Vixen.Sys {
	// Output only for now.  May make generic for input and output fixtures.
    public class Fixture : IEquatable<Fixture> {
        // Going with a linked list because List<> accells at random access while
        // a LinkedList<> accells at sequential access. 
		private LinkedList<OutputChannel> _channels = new LinkedList<OutputChannel>();
		private string _fixtureDefinitionName;

		public void InsertChannel(OutputChannel channel) {
            _channels.AddLast(channel);
            channel.ParentFixture = this;
			// If there is no name for this fixture and it's the first channel
			// being added, give it a default name.
			if(string.IsNullOrWhiteSpace(Name) && _channels.Count == 1) {
				Name = channel.Name;
			}
        }

		public IEnumerable<OutputChannel> Channels {
            get { return _channels; }
        }

        public int ChannelCount {
            get { return _channels.Count; }
        }

		public string Name { get; set; }

        public ISequence ParentSequence { get; set; }

        public bool AllowFrameSkip {
            get { return _channels.All(x => x.AllowFrameSkip); }
            set {
                foreach(Channel channel in _channels) {
                    channel.AllowFrameSkip = value;
                }
            }
        }

        public bool Masked {
            get { return Channels.All(x => x.Masked); }
            set {
                foreach(Channel channel in Channels) {
                    channel.Masked = value;
                }
            }
        }

		public FixtureDefinition FixtureDefinition {
			set {
				if(value != null) {
					// Setting template reference.
					if(_fixtureDefinitionName != value.DefinitionName) {
						_fixtureDefinitionName = value.DefinitionName;
						// This must be done in two steps so that we have the original
						// instances (appropriate ones, that is) that belong to this fixture
						// and clones of the missing ones from the definition.
						_channels = new LinkedList<OutputChannel>(_channels.Intersect(value.Channels));
						// Add any channels in the definition but not in the fixture.
						foreach(Channel channel in value.Channels.Except(_channels.ToArray())) {
							InsertChannel(channel.Clone(this) as OutputChannel);
						}
					}
				} else if(!string.IsNullOrWhiteSpace(_fixtureDefinitionName)) {
					// Removing any template reference.
					_fixtureDefinitionName = "";
					_channels.Clear();
				}
			}
		}

		public string FixtureDefinitionName {
			get { return _fixtureDefinitionName; }
			set {
				if(!value.Equals(_fixtureDefinitionName, StringComparison.OrdinalIgnoreCase)) {
					this.FixtureDefinition = FixtureDefinition.Get(value);
				}
			}
		}

		// Both of these are required for Except(), Distinct(), Union() and Intersect().
		// Equals(<type>) for IEquatable and GetHashCode() because that's needed anytime
		// Equals(object) is overridden (which it really isn't, but this is what is said and
		// it doesn't work otherwise).
		public bool Equals(Fixture other) {
			// Do in the order that will most likely cause an early out.
			return
				this.Name == other.Name &&
				this.ChannelCount == other.ChannelCount &&
				this.Channels.All(x => other.Channels.Contains(x));
		}
		
		public override int GetHashCode() {
			return (Name + ChannelCount + string.Join<Guid>(string.Empty, Channels.Select(x => x.Id))).GetHashCode();
		}
	}
}
