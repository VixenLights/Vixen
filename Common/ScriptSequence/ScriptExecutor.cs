using System;
using BaseSequence;
using Vixen.Module.Script;
using Vixen.Sys;

namespace ScriptSequence {
	public class ScriptExecutor : SequenceExecutor {
		private IUserScriptHost _scriptHost;

		protected override void OnSequenceStarted(SequenceStartedEventArgs e) {
			if(!(Sequence is ScriptSequence)) throw new InvalidOperationException("Attempt to compile a non-script sequence.");

			Sequence.SequenceData.EffectData.Clear();

			ScriptCompiler compiler = new ScriptCompiler();
			compiler.Compile((ScriptSequence)Sequence);

			OnMessage(new ExecutorMessageEventArgs(Sequence, string.Join(Environment.NewLine, compiler.Errors)));

			if(!compiler.HasErrors) {
				if(!_CompileOnly(e)) {
					_scriptHost = compiler.ScriptHost;
					_StartScript();
				}
			} else {
				throw new Exception("Script did not compile.");
			}

			base.OnSequenceStarted(e);
		}

		protected override void OnSequenceEnded(SequenceEventArgs e) {
			base.OnSequenceEnded(e);

			// May be the result of a natural end we caused or the user forcing a stop.
			_StopScript();
		}

		protected override bool _DataListener(IEffectNode effectNode) {
			//*** compensation delta, user-configurable
			IEffectNode contextRelativeEffectNode = new EffectNode(effectNode.Effect, effectNode.StartTime + TimingSource.Position);
			Sequence.SequenceData.EffectData.AddData(contextRelativeEffectNode);
			// We don't want any handlers beyond the executor to get live data.
			return true;
		}

		public override void Dispose(bool disposing) {
			_StopScript();
			base.Dispose(disposing);
		}

		private void _StartScript() {
			if(_scriptHost != null) {
				_scriptHost.Sequence = Sequence;
				_scriptHost.TimingSource = TimingSource;
				_scriptHost.Ended += _ScriptEnded;
				_scriptHost.Error += _ScriptError;
				_scriptHost.Start();
			}
		}

		private void _StopScript() {
			if(_scriptHost != null) {
				_scriptHost.Stop();
			}
		}

		private bool _CompileOnly(SequenceStartedEventArgs e) {
			return e.EndTime < e.StartTime;
		}

		private void _ScriptEnded(object sender, EventArgs e) {
			//I'm sure there's an undocumented reason for this that I need to document.
			EndTime = TimeSpan.Zero;
			_scriptHost.Ended -= _ScriptEnded;
			_scriptHost.Error -= _ScriptError;
			_scriptHost = null;
		}

		private void _ScriptError(object sender, ExecutorMessageEventArgs e) {
			OnError(e);
		}
	}
}
