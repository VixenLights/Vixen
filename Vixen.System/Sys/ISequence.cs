using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;

namespace Vixen.Sys {
    public interface ISequence {//: IModuleDataContainer {
        string Name { get; }
		void Save();
		void Save(string fileName);
		long Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		void InsertData(ChannelNode[] targetNodes, long startTime, long timeSpan, Command command);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		InputChannels Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		MediaCollection Media { get; }
		IModuleDataSet ModuleDataSet { get; }
	}
}
