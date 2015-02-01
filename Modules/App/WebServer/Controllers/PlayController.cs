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
	public class PlayController : ApiController
	{
		[HttpGet]
		public Status Status()
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
			Status status = SequenceHelper.PlaySequence(sequence.FileName);
			if (status == null)
			{
				var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent(string.Format("No sequence with name = {0}", sequence.FileName)),
					ReasonPhrase = "Sequence Not Found"
				};
				throw new HttpResponseException(resp);
			}

			return status;
		}

		[HttpPost]
		public Status PauseSequence()
		{
			return SequenceHelper.StopSequence();
		}

		[HttpPost]
		public Status StopSequence()
		{
			return SequenceHelper.StopSequence();
		}
	}
}
