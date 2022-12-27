using System.IO;
using System.Linq;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal class MediaTimingProvider : ITimingProvider
	{
		public string TimingProviderTypeName
		{
			get { return "Media"; }
		}

		public string[] GetAvailableTimingSources(ISequence sequence)
		{
			return sequence.GetAllMedia().Where(x => x.TimingSource != null).Select(x => Path.GetFileName(x.MediaFilePath)).ToArray();
		}

		public ITiming GetTimingSource(ISequence sequence, string sourceName)
		{
			string sourcePath = Path.Combine(MediaService.MediaDirectory, sourceName);
			IMediaModuleInstance mediaModule =
				sequence.GetAllMedia().FirstOrDefault(x => x.TimingSource != null && x.MediaFilePath == sourcePath);
			if (mediaModule != null) {
				return mediaModule.TimingSource;
			}
			return null;
		}
	}
}