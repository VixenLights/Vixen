﻿using Vixen.Module.Timing;
using Vixen.Execution;

namespace Vixen.Sys
{
	public interface ISequence : IHasMedia, IHasSequenceFilterNodes, IHasLayers, IHasMarks
	{
		string Name { get; }
		void Save();
		void Save(string fileName);
		TimeSpan Length { get; set; }
		string FileExtension { get; }
		string FilePath { get; set; }
		void InsertData(IEffectNode effectNode);
		void InsertData(IEnumerable<IEffectNode> effectNodes);
		bool RemoveData(IEffectNode effectNode);
		InsertDataListenerStack InsertDataListener { get; set; }
		SelectedTimingProvider SelectedTimingProvider { get; set; }
		ITiming GetTiming();
		ISequenceTypeDataModel SequenceData { get; set; }
	}
}