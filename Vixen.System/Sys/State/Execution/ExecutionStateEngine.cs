using System;

namespace Vixen.Sys.State.Execution {
	public class ExecutionStateEngine {
		public event EventHandler StateChanged;

		public ExecutionStateEngine() {
			OpeningState = new OpeningState(this);
			ClosingState = new ClosingState(this);
			OutputTestState = new OutputTestState(this);
			OpenState = new OpenState(this);
			ClosedState = new ClosedState(this);

			CurrentState = ClosedState;
		}

		public State CurrentState { get; private set; }

		public void SetState(State state) {
			// Current state driven entirely by the state objects, which define the
			// state machine.
			if(state != CurrentState) {
				CurrentState.Leave();
				CurrentState = state;
				OnStateChanged(EventArgs.Empty);
				CurrentState.Enter();
			}
		}

		public bool IsRunning {
			get {
				return
					CurrentState == OpeningState ||
					CurrentState == OpenState;
			}
		}

		public void ToOpen() {
			CurrentState.OnOpen();
		}

		public void ToClose() {
			CurrentState.OnClose();
		}

		public void ToOutputTest() {
			CurrentState.OnOutputTest();
		}

		public State OpeningState { get; private set; }
		public State ClosingState { get; private set; }
		public State OutputTestState { get; private set; }
		public State OpenState { get; private set; }
		public State ClosedState { get; private set; }

		protected virtual void OnStateChanged(EventArgs e) {
			if(StateChanged != null) {
				StateChanged(this, e);
			}
		}
	}
}
