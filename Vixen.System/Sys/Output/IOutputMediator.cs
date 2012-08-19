namespace Vixen.Sys.Output {
	interface IOutputMediator<T> : IHasOutputs<T>
		where T : Output {
		void LockOutputs();
		void UnlockOutputs();
	}
}
