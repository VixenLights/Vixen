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
		void InsertData(EffectNode effectNode);
		void InsertData(IEnumerable<EffectNode> effectNodes);
		bool RemoveData(EffectNode effectNode);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		DataStreams Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		//MediaCollection Media { get; }
		ModuleLocalDataSet ModuleDataSet { get; }
		IEnumerable<EffectNode> GetData();
		ITiming GetTiming();
		SequenceType SequenceType { get; }
	}
}
