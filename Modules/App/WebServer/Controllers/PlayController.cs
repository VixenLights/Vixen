using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VixenModules.App.WebServer.Filter;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class PlayController : BaseApiController
	{
		[HttpGet]
		public IEnumerable<Status> Status()
		{
			return SequenceHelper.Status();
		}

		public IEnumerable<Sequence> GetSequences()
		{
			return SequenceHelper.GetSequences();
		}

		[HttpPost]
		public Status PlaySequence(Sequence sequence)
		{
			return SequenceHelper.PlaySequence(sequence);
		}

		[HttpPost]
		public Status PauseSequence(Sequence sequence)
		{
			return SequenceHelper.PauseSequence(sequence);
		}

		[HttpPost]
		public Status StopSequence(Sequence sequence)
		{
			return SequenceHelper.StopSequence(sequence);
		}
	}
}
