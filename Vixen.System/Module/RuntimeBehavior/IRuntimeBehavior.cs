using System;
using Vixen.Sys;

namespace Vixen.Module.RuntimeBehavior {
	public interface IRuntimeBehavior {
		void Startup(ISequence sequence);
		void Shutdown();
		void Handle(IEffectNode effectNode);
		bool Enabled { get; set; }
		Tuple<string, Action>[] BehaviorActions { get; }
	}
}
