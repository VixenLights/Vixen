using System;

namespace Vixen.Sys.State.Execution {
	public class ExecutionStateEngine {
		public event EventHandler StateChanged;

		public ExecutionStateEngine() {
			OpeningState = new OpeningState(this);
			ClosingState = new ClosingState(this);
			TestOpeningState = new TestOpeningState(this);
			OpenState = new OpenState(this);
			ClosedState = new ClosedState(this);
			TestOpenState = new TestOpenState(this);

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
					CurrentState != ClosingState &&
					CurrentState != ClosedState;
			}
		}

		public void ToOpen() {
			CurrentState.OnOpen();
		}

		public void ToClosed() {
			CurrentState.OnClose();
		}

		public void ToTest() {
			CurrentState.OnTest();
		}

		public State OpeningState { get; private set; }
		public State ClosingState { get; private set; }
		public State TestOpeningState { get; private set; }
		public State OpenState { get; private set; }
		public State ClosedState { get; private set; }
		public State TestOpenState { get; private set; }

		protected virtual void OnStateChanged(EventArgs e) {
			if(StateChanged != null) {
				StateChanged(this, e);
			}
		}
	}
}
