using System;

namespace Vixen.Sys.Output {
	public interface IOutputDevice : ISetup {
		void Start();
		void Stop();
		void Pause();
		void Resume();
		Guid Id { get; }
		string Name { get; set; }
		int UpdateInterval { get; set; }
		bool IsRunning { get; }
		bool IsPaused { get; }
		void Update();
	}
}
