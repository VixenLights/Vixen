using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Service
{
	internal class SequenceHelper
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private static ISequenceContext _context; 

		public static IEnumerable<Sequence> GetSequences()
		{
			var sequenceNames = SequenceService.Instance.GetAllSequenceFileNames().Select(Path.GetFileName);
			var sequences = sequenceNames.Select(sequenceName => new Sequence
			{
				Name = Path.GetFileNameWithoutExtension(sequenceName),
				FileName = sequenceName
			}).ToList();

			return sequences;
		}

		/// <summary>
		/// Play a sequence of a given file name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Status PlaySequence(string name){
			
			var status = new Status();

			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			string fileName = HttpUtility.UrlDecode(name);
			if (_context != null && (_context.IsRunning))
			{
				status.Message = string.Format("Already playing {0}", _context.Sequence.Name);
			}
			else
			{
				ISequence sequence = SequenceService.Instance.Load(fileName);
				if (sequence == null)
				{
					return null;
				}
				Logging.Info(string.Format("Web - Prerendering effects for sequence: {0}", sequence.Name));
				Parallel.ForEach(sequence.SequenceData.EffectData.Cast<IEffectNode>(), effectNode => effectNode.Effect.PreRender());

				_context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);
				_context.ContextEnded += context_ContextEnded;
				_context.Play(TimeSpan.Zero, sequence.Length);
				status.Message = string.Format("Playing sequence {0} of length {1}", sequence.Name, sequence.Length);
			}

			return status;
		}

		private static void context_ContextEnded(object sender, EventArgs e)
		{
			var context = sender as IContext;
			if (context != null)
			{
				context.ContextEnded -= context_ContextEnded;
				VixenSystem.Contexts.ReleaseContext(context);
			}

		}

		/// <summary>
		/// Pause the current playing sequence.
		/// </summary>
		/// <returns>Status</returns>
		public static Status PauseSequence()
		{
			var status = new Status();
			if (_context != null && _context.IsRunning)
			{
				status.Message = string.Format("{0} paused.", _context.Sequence.Name);
				_context.Pause();
			}
			else
			{
				status.Message = "Nothing playing.";
			}
			return status;
		}

		/// <summary>
		/// Stop the current playing sequnce. Does not effect scheduled sequences.
		/// </summary>
		/// <returns></returns>
		public static Status StopSequence()
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
			return status;
		}

		/// <summary>
		/// Retrieve the status of any sequences playing. 
		/// </summary>
		/// <returns>Status</returns>
		public static Status Status()
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

			return status;
		}
	}
}
