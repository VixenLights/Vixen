using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Vixen.Sys;

namespace Vixen.Script {
	/// <summary>
	/// Wraps a fixture in a dynamic object to enable fixture.channel syntax.
	/// </summary>
	public class UserScriptFixture : DynamicObject, IEnumerable<UserScriptChannel> {
		private Fixture _fixture;
		// Channel name : UserScriptChannel
		private Dictionary<string, UserScriptChannel> _channels;

		public UserScriptFixture(Fixture fixture) {
			_fixture = fixture;
			_channels =
				fixture.Channels.ToDictionary(x => x.Name, x => new UserScriptChannel(x));
		}

		public string Name {
			get { return _fixture.Name; }
		}

		public bool Masked {
			get { return _fixture.Masked; }
			set { _fixture.Masked = value; }
		}

		public IEnumerator<UserScriptChannel> GetEnumerator() {
			return _channels.Values.GetEnumerator() as IEnumerator<UserScriptChannel>;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _channels.Values.GetEnumerator();
		}

		// Dynamic readonly fields for contained channels.
		public override IEnumerable<string> GetDynamicMemberNames() {
			return _channels.Keys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			UserScriptChannel channel = null;
			result = null;
			if(_channels.TryGetValue(binder.Name, out channel)) {
				result = channel;
				return true;
			}
			return false;
		}
	}
}
