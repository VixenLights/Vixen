using System;
using System.Collections.Generic;
using Vixen.Module.Timing;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.Sys {
	public enum SequenceType {
		None,
		Standard,
		Script
	};
	public interface ISequence : IHasMedia, IHasPreFilterNodes {
        string Name { get; }
		void Save();
		void Save(string fileName);
		TimeSpan Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		void InsertData(IEffectNode effectNode);
		void InsertData(IEnumerable<IEffectNode> effectNodes);
		bool RemoveData(IEffectNode effectNode);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		DataStreams Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		ModuleLocalDataSet ModuleDataSet { get; }
		IEnumerable<IEffectNode> GetData();
		ITiming GetTiming();
		SequenceType SequenceType { get; }
	}
}
