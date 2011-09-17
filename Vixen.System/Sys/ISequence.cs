using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;

namespace Vixen.Sys {
    public interface ISequence {
        string Name { get; }
		void Save();
		void Save(string fileName);
		TimeSpan Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		void InsertData(EffectNode commandNode);
		EffectNode InsertData(IEffectModuleInstance effect, TimeSpan startTime);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		EffectStreams Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		MediaCollection Media { get; }
		IModuleDataSet ModuleDataSet { get; }
	}
}
