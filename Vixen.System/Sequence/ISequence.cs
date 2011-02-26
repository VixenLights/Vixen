using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Input;

namespace Vixen.Sequence {
	public enum UpdateBehavior { DeltaOnly, FullUpdate };

    public interface ISequence : IFixtureContainer, IModuleDataContainer {
        string Name { get; set; }
		void Load(string fileName);
		void Save();
		void Save(string fileName);
		int Length { get; set; }
		bool IsUntimed { get; set; }
		string FileName { get; set; }

		//*** not yet used
		bool Masked { get; set; }
		void InsertData(OutputChannel[] channels, int startTime, int timeSpan, Command command);
		UpdateBehavior UpdateBehavior { get; }
		InsertDataListenerStack InsertDataListener { get; set; }
		Guid TimingSourceId { get; set; }
		CommandNodeIntervalSync Data { get; }
		IRuntimeBehaviorModuleInstance[] RuntimeBehaviors { get; }
		IEnumerable<OutputChannel> OutputChannels { get; }
	}
}
