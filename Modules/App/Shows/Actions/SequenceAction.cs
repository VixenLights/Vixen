using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.Media;
using Vixen.Sys;
using Vixen.Services;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.Shows
{
	public class SequenceAction : Action
	{
		private ISequenceContext _sequenceContext = null;
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ISequence _sequence;
		
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
				if (_sequenceContext == null || SequenceChanged() || _sequence == null)
				{
					if (_sequenceContext != null)
					{
						DisposeCurrentContext();
					}

					var entry = SequenceManager.GetSequenceAsync(ShowItem.Sequence_FileName);
					entry.Wait();
					var sequenceEntry = entry.Result;

					if (sequenceEntry == null)
					{
						Logging.Error("Failed to preprocess sequence {1} because it could not be loaded.", ShowItem.Name);
						return;
					}

					//Give up to 30 seconds for the sequence to load.
					int retryCount = 0;
					while (sequenceEntry.SequenceLoading && retryCount<30)
					{
						retryCount++;
						Logging.Info("Waiting for sequence to load. {1}", ShowItem.Name);
						Thread.Sleep(1);
					}

					if (sequenceEntry.Sequence == null)
					{
						Logging.Error("Failed to preprocess sequence {1} because it could not be loaded.", ShowItem.Name);
						return;
					}
					_sequence = sequenceEntry.Sequence;
					
					//Initialize the media if we have it so any audio effects can be rendered 
					LoadMedia();
					
					ISequenceContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), _sequence);

					Parallel.ForEach(_sequence.SequenceData.EffectData.Cast<IEffectNode>(), RenderEffect);

					context.SequenceEnded += sequence_Ended;

					_sequenceContext = context;
				}
				PreProcessingCompleted = true;
			}
			catch (Exception ex)
			{
				Logging.Error("Could not pre-render sequence " + ShowItem.Sequence_FileName + "; ",ex);
			}
		}

		private void LoadMedia()
		{
			var sequenceMedia = _sequence.GetAllMedia();
			if (sequenceMedia != null && sequenceMedia.Any())
				foreach (IMediaModuleInstance media in sequenceMedia)
				{
					media.LoadMedia(TimeSpan.Zero);
				}
		}

		private void RenderEffect(IEffectNode node)
		{
			if (node.Effect.IsDirty)
			{
				node.Effect.PreRender();
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
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeCurrentContext();
				
			}
			SequenceManager.ConsumerFinished(ShowItem.Sequence_FileName);
			_sequenceContext = null;
			base.Dispose(disposing);
		}

		
	}
}