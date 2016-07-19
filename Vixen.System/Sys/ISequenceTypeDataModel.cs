using System;
using System.Collections.Generic;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.Media;
using Vixen.Sys.LayerMixing;

namespace Vixen.Sys
{
	public interface ISequenceTypeDataModel
	{
		//int Version { get; set; }
		TimeSpan Length { get; set; }
		SelectedTimingProvider SelectedTimingProvider { get; set; }
		ModuleLocalDataSet LocalDataSet { get; set; }
		List<IMediaModuleInstance> Media { get; set; }
		DataStream EffectData { get; set; }
		DataStream SequenceFilterData { get; set; }
		DataStreams DataStreams { get; }
		SequenceLayers SequenceLayers { get; set; }

	}
}