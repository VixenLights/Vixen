using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.Shows
{
	public class SequenceAction : Action
	{
		private ISequenceContext _sequenceContext = null;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		ISequence sequence = null;

		public SequenceAction(ShowItem showItem)
			: base(showItem)
		{
		}

		public override void Execute()
		{
			try
			{
				IsRunning = true;
				PreProcess();
				_sequenceContext.Start();
			}
			catch (Exception ex)
			{
			    Logging.Error("Could not execute sequence " + ShowItem.Sequence_FileName + "; " + ex.Message);
			}
		}

		//public override TimeSpan Duration()
		//{
		//	if (_sequenceContext != null) 
		//		return _sequenceContext.Sequence.Length;
		//	else
		//		return TimeSpan.Zero;
		//}

		public override void Stop()
		{
			if (_sequenceContext != null)
				_sequenceContext.Stop();
			base.Stop();
		}

		bool _preProcessingCompleted = false;
		public override bool PreProcessingCompleted
		{
			get
			{
				bool changed = SequenceChanged();
				bool complete = _preProcessingCompleted && !changed;
				//Console.WriteLine("Get PreProcessingCompleted" + ShowItem.Name + "=" + complete + ":" + !changed + ":" + _preProcessingCompleted);
				return complete;
			}
			set
			{
				//Console.WriteLine("Set PreProcessingCompleted" + ShowItem.Name + "=" + value);
				_preProcessingCompleted = value;
			}
		}

		public override void PreProcess(CancellationTokenSource cancellationTokenSource = null)
		{
			if (cancellationTokenSource != null)
				CancellationTknSource = cancellationTokenSource;

			try
			{
				//Console.WriteLine("PreProcess: " + ShowItem.Name + " : " + (_sequenceContext == null));
				if (_sequenceContext == null || SequenceChanged())
				{
					if (_sequenceContext != null)
					{
						DisposeCurrentContext();
					}

					sequence = SequenceService.Instance.Load(ShowItem.Sequence_FileName);
					// Why doesn't this work?
					//IContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.ContextLevelCaching), sequence);
					ISequenceContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);

					// Parallel doesn't work here. Causes multiple sequences to be run at the same time
					//foreach (IEffectNode effectNode in sequence.SequenceData.EffectData.Cast<IEffectNode>())
					Parallel.ForEach(sequence.SequenceData.EffectData.Cast<IEffectNode>(), effectNode => effectNode.Effect.PreRender());

					context.SequenceEnded += sequence_Ended;

					_sequenceContext = context;
				}
				PreProcessingCompleted = true;
			}
			catch (Exception ex)
			{
				Logging.ErrorException("Could not pre-render sequence " + ShowItem.Sequence_FileName + "; ",ex);
			}
		}

		DateTime _lastSequenceDateTime = DateTime.Now;
		private bool SequenceChanged()
		{
			bool datesEqual = false;

			if (System.IO.File.Exists(ShowItem.Sequence_FileName))
			{
				DateTime lastWriteTime = System.IO.File.GetLastWriteTime(ShowItem.Sequence_FileName);
				datesEqual = (_lastSequenceDateTime == lastWriteTime);
				_lastSequenceDateTime = lastWriteTime;
			}
			return !datesEqual;
		}

		private void sequence_Ended(object sender, EventArgs e)
		{
			base.Complete();
		}

		private void DisposeCurrentContext()
		{
			if (_sequenceContext != null)
			{
				_sequenceContext.SequenceEnded -= sequence_Ended;
				VixenSystem.Contexts.ReleaseContext(_sequenceContext);
				var tSequence = (sequence as TimedSequence);
				if (tSequence != null)
				{
					tSequence.Dispose();
				}

			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeCurrentContext();
				
			}
			sequence = null;
			_sequenceContext = null;
			base.Dispose(disposing);
		}

		
	}
}