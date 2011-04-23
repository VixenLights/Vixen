using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	class MediaTimingProvider : ITimingProvider {
		public string TimingProviderTypeName {
			get { return "Media"; }
		}

		public string[] GetAvailableTimingSources(ISequence sequence) {
			return sequence.Media.Where(x => x.TimingSource != null).Select(x => x.MediaFilePath).ToArray();
		}

		public ITiming GetTimingSource(ISequence sequence, string sourceName) {
			IMediaModuleInstance mediaModule = sequence.Media.FirstOrDefault(x => x.TimingSource != null && x.MediaFilePath == sourceName);
			if(mediaModule != null) {
				return mediaModule.TimingSource;
			}
			return null;
		}
	}
}
