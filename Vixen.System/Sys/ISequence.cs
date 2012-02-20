using System;
using System.Collections.Generic;
using Vixen.Module.Timing;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.Sys {
	public enum SequenceType {
		Standard,
		Script
	};
	public interface ISequence {
        string Name { get; }
		void Save();
		void Save(string fileName);
		TimeSpan Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		void InsertData(EffectNode effectNode);
		void InsertData(IEnumerable<EffectNode> effectNodes);
		bool RemoveData(EffectNode effectNode);
		void AddPreFilter(PreFilterNode preFilterNode);
		bool RemovePreFilter(PreFilterNode preFilterNode);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		DataStreams Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		MediaCollection Media { get; }
		IModuleDataSet ModuleDataSet { get; }
		IEnumerable<EffectNode> GetData();
		IEnumerable<PreFilterNode> GetPreFilters();
		ITiming GetTiming();
		SequenceType SequenceType { get; }
	}
}
