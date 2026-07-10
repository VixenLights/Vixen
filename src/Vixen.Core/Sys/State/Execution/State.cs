namespace Vixen.Sys.State.Execution
{
	public abstract class State
	{
		protected State(ExecutionStateEngine engine)
		{
			Engine = engine;
		}

		public abstract string Name { get; }

		public virtual void Enter()
		{
		}

		public virtual void Leave()
		{
		}

		public virtual void OnOpen()
		{
		}

		public virtual void OnClose()
		{
		}

		public virtual void OnTest()
		{
		}

		protected ExecutionStateEngine Engine { get; private set; }
	}
}