using System;
using System.Collections.Generic;
using Vixen.Module.PreFilter;
using Vixen.Module.Timing;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.RuntimeBehavior;

namespace Vixen.Sys {
    public interface ISequence {
        string Name { get; }
		void Save();
		void Save(string fileName);
		TimeSpan Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		void InsertData(EffectNode effectNode);
		void InsertData(IEnumerable<EffectNode> effectNodes);
		EffectNode InsertData(IEffectModuleInstance effect, TimeSpan startTime);
		bool RemoveData(EffectNode effectNode);
		PreFilterNode AddPreFilter(IPreFilterModuleInstance preFilter, TimeSpan startTime);
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
	}
}
