namespace Vixen.Module.Preview {
	public interface IThreadBehavior {
		void Start();
		void Stop();
		bool IsRunning { get; }
	}
}
