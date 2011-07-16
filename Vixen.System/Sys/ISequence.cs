using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Media;

namespace Vixen.Sys {
    public interface ISequence : IModuleDataContainer {
        string Name { get; set; }
		void Save();
		void Save(string fileName);
		long Length { get; set; }
		bool IsUntimed { get; set; }
		string FilePath { get; set; }
		/// <summary>
		/// Data entered here at runtime is handled only by runtime behaviors.
		/// </summary>
		/// <param name="channels"></param>
		/// <param name="startTime"></param>
		/// <param name="timeSpan"></param>
		/// <param name="command"></param>
		void InsertData(ChannelNode[] targetNodes, long startTime, long timeSpan, Command command);
		InsertDataListenerStack InsertDataListener { get; set; }
		TimingProviders TimingProvider { get; }
		InputChannels Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		MediaCollection Media { get; }
	}
}
