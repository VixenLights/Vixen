using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Kayak;
using Kayak.Http;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.WebServer.HTTP;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Actions
{
	public class PlayController:BaseController
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private static ISequenceContext _context; 

		public override void ProcessGet(HttpRequestHead request, IHttpResponseDelegate response)
		{
			if (request.Uri.StartsWith("/api/play"))
			{
				if (request.Uri.StartsWith("/api/play/getSequences"))
				{
					GetSequences(request, response);
					return;
				}
				if (request.Uri.StartsWith("/api/play/playSequence"))
				{
					PlaySequence(request, response);
					return;
				}
				if (request.Uri.StartsWith("/api/play/status"))
				{
					Status(request, response);
					return;
				}
			}
			
			UnsupportedOperation(request, response);
		}

		public override void ProcessPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			//Figure out how to get the post params from within Kayak so some of these operations that
			//should be a post can be. Right now I am limited to just post based on verbs on the url.
			if (request.Uri.StartsWith("/api/play/stopSequence"))
			{
				StopSequence(request, response);
				return;
			}
			UnsupportedOperation(request,response);
		}

		
		private void GetSequences(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var sequenceNames = SequenceService.Instance.GetAllSequenceFileNames().Select(Path.GetFileName);
			var sequences = sequenceNames.Select(sequenceName => new Sequence
			{
				Name = Path.GetFileNameWithoutExtension(sequenceName), FileName = sequenceName
			}).ToList();

			SerializeResponse(sequences,response);
		}

		private void PlaySequence(HttpRequestHead request, IHttpResponseDelegate response)
		{
			HttpResponseHead headers;
			var status = new Status();
			NameValueCollection parms = GetParameters(request);
		
			if(!parms.HasKeys() && parms["name"] != null)
			{
				headers = GetHeaders(0, HttpStatusCode.BadRequest.ToString());
				response.OnResponse(headers,new BufferedProducer(""));
				return;
			}

			string fileName = HttpUtility.UrlDecode(parms["name"]);
			if (_context != null && (_context.IsRunning))
			{
				status.Message = string.Format("Already playing {0}", _context.Sequence.Name);
			}
			else
			{
				ISequence sequence = SequenceService.Instance.Load(fileName);
				if (sequence == null)
				{
					headers = GetOkHeaders(0);
					headers.Status = HttpStatusCode.NotFound.ToString();
					response.OnResponse(headers, new BufferedProducer(""));
					return;
				}
				Logging.Info(string.Format("Web - Prerendering effects for sequence: {0}", sequence.Name));
				Parallel.ForEach(sequence.SequenceData.EffectData.Cast<IEffectNode>(), effectNode  => effectNode.Effect.PreRender());
				
				_context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);
				_context.ContextEnded += context_ContextEnded;
				_context.Play(TimeSpan.Zero, sequence.Length);
				status.Message = string.Format("Playing sequence {0} of length {1}", sequence.Name, sequence.Length);
			}
			
			SerializeResponse(status,response);
		}

		private void context_ContextEnded(object sender, EventArgs e)
		{
			if (_context != null)
			{
				_context.ContextEnded -= context_ContextEnded;
				VixenSystem.Contexts.ReleaseContext(_context);	
			}
			
		}

		private void StopSequence(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var status = new Status();
			if (_context != null && _context.IsRunning)
			{
				status.Message = string.Format("Stopping {0}", _context.Sequence.Name);
				_context.Stop();
			}
			else
			{
				status.Message = "Nothing playing.";
			}
			SerializeResponse(status,response);
		}

		private void Status(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var status = new Status();
			if (_context != null && _context.IsRunning)
			{
				status.Message = string.Format("{0} sequence is playing at position {1}", _context.Sequence.Name,
					_context.GetTimeSnapshot());
			}
			else
			{
				status.Message = "Nothing playing.";
			}

			SerializeResponse(status,response);
		}
	}
}
