namespace Vixen.Sys.State.Execution {
	public abstract class State {
		protected State(ExecutionStateEngine engine) {
			Engine = engine;
		}

		abstract public string Name { get; }

		virtual public void Enter() { }
		virtual public void Leave() { }

		virtual public void OnOpen() { }
		virtual public void OnClose() { }
		virtual public void OnTest() { }

		protected ExecutionStateEngine Engine { get; private set; }
	}
}
