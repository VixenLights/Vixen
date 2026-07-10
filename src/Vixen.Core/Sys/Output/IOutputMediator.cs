namespace Vixen.Sys.Output
{
	internal interface IOutputMediator<T> : IHasOutputs<T>
		where T : Output
	{
		void LockOutputs();
		void UnlockOutputs();
	}
}