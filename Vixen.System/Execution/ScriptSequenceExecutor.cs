using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Vixen.Common;
using Vixen.Sys;
using Vixen.IO;
using Vixen.Script;
using System.Reflection;
using Vixen.Sequence;
using Vixen.Module.Sequence;

namespace Vixen.Execution {
    class ScriptSequenceExecutor : SequenceExecutor {
		private IUserScriptHost _scriptHost;

		protected override void OnPlaying(int startTime, int endTime) {
			base.OnPlaying(startTime, endTime);

			ScriptHostGenerator hostGenerator = new ScriptHostGenerator();
			_scriptHost = hostGenerator.GenerateScript(this.Sequence as ScriptSequenceBase);
			string[] errors = hostGenerator.Errors.ToArray();
			OnMessage(new ExecutorMessageEventArgs(string.Join(Environment.NewLine, errors)));
			// An end time < start time means compile only.
			if(endTime >= startTime) {
				if(errors.Length == 0) {
					// Under the current implementation, this results in a thread being spawned.
					// That thread is the actual execution path of their code.
					_scriptHost.Ended += (sender, e) => {
						EndTime = 0;
					};
					_scriptHost.Error += (sender, e) => {
						OnError(e);
					};
					_scriptHost.Start();
				} else {
					throw new Exception("Script did not compile.");
				}
			} else {
				throw new Exception("Compile done.");
			}
        }

        protected override void OnStopping() {
            base.OnStopping();
			// May be the result of a natural end we caused or the user forcing a stop.
			_StopScript();
			// The script's job was to generate data for the channels.  That data remains
			// in the channel and needs to be purged.
			foreach(Fixture fixture in Sequence.Fixtures) {
				foreach(Channel channel in fixture.Channels) {
					channel.Clear();
				}
			}
        }

        public override void Dispose(bool disposing) {
			_StopScript();
			base.Dispose(disposing);
        }

		private void _StopScript() {
			if(_scriptHost != null) {
				_scriptHost.Stop();
				_scriptHost = null;
			}
		}
    }
}
