using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Script {
	/// <summary>
	/// Wraps an output channel to enable a channel to be a channel enumerable so fixtures
	/// and single channels can be treated the same.
	/// </summary>
	public class UserScriptChannel : IEnumerable<UserScriptChannel> {
		private OutputChannel _channel;

		public UserScriptChannel(OutputChannel channel) {
			_channel = channel;
		}

		public OutputChannel Channel {
			get { return _channel; }
		}

		public string Name {
			get { return _channel.Name; }
		}

		public bool Masked {
			get { return _channel.Masked; }
			set { _channel.Masked = value; }
		}

		public IEnumerator<UserScriptChannel> GetEnumerator() {
			yield return this;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
