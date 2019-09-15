using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.Media;
using Vixen.Sys;
using Vixen.Sys.State.Execution;

namespace VixenModules.App.Shows
{
	public class SequenceAction : Action
	{
		private ISequenceContext _sequenceContext = null;
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ISequence _sequence;
		private bool _canExecute = true;
		private bool _preProcessingCompleted;
		
		public SequenceAction(ShowItem showItem)
			: base(showItem)
		{
		}

		public override void Execute()
		{
			IsRunning = true;
			if (_canExecute)
			{
				try
				{
					if (!_preProcessingCompleted)
					{
						PreProcess();
					}
					_sequenceContext?.Start();
				}
				catch (Exception ex)
				{
					Logging.Error("Could not execute sequence. " + ShowItem.SequencePath + "; " + ex.Message);
					_canExecute = false;
					Complete();
				}
			}
			else
			{
				Logging.Error($"Could not execute sequence. {ShowItem.SequencePath}");
				Complete();
			}
			
		}

		public override void Stop()
		{
			_sequenceContext?.Stop();
			base.Stop();
		}

		public override bool PreProcessingCompleted
		{
			get
			{
				if (SequenceManager.IsSequenceStale(ShowItem.SequencePath))
				{
					_preProcessingCompleted = false;
					_canExecute = true;
				}
				return _preProcessingCompleted;
			}
			set => _preProcessingCompleted = value;
		}

		public override void PreProcess(CancellationTokenSource cancellationTokenSource = null)
		{
			if (cancellationTokenSource != null)
				CancellationTknSource = cancellationTokenSource;

			try
			{
				if (!_preProcessingCompleted || _sequenceContext == null || _sequence == null)
				{
					if (_sequenceContext != null)
					{
						DisposeCurrentContext();
					}

					var entry = SequenceManager.GetSequenceAsync(ShowItem.SequencePath, Id);
					entry.Wait();
					var sequenceEntry = entry.Result;

					if (sequenceEntry == null)
					{
						Logging.Error("Failed to preprocess sequence {1} because it could not be loaded.",
							ShowItem.Name);
						return;
					}

					//Give up to 30 seconds for the sequence to load.
					int retryCount = 0;
					while (sequenceEntry.SequenceLoading && retryCount < 30)
					{
						retryCount++;
						Logging.Info("Waiting for sequence to load. {1}", ShowItem.Name);
						Thread.Sleep(1);
					}

					if (sequenceEntry.Sequence == null)
					{
						Logging.Error("Failed to preprocess sequence {1} because it could not be loaded.",
							ShowItem.Name);
						return;
					}

					_sequence = sequenceEntry.Sequence;

					//Initialize the media if we have it so any audio effects can be rendered 
					LoadMedia();
					
					Parallel.ForEach(_sequence.SequenceData.EffectData.Cast<IEffectNode>(), RenderEffect);

					if (_canExecute)
					{
						ISequenceContext context =
							VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching),
								_sequence);
						context.SequenceEnded += sequence_Ended;
						_sequenceContext = context;
					}
				}

			}
			catch (Exception ex)
			{
				_canExecute = false;
				Logging.Error(ex, "Could not pre-render sequence. Sequence will not play until the issue is corrected." + ShowItem.SequencePath + "; ");
			}
			finally
			{
				PreProcessingCompleted = true;
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
				var success = node.Effect.PreRender();
				if (!success)
				{
					_canExecute = false;
				}
			}
		}

		private void sequence_Ended(object sender, EventArgs e)
		{
			Complete();
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
			SequenceManager.ConsumerFinished(ShowItem.SequencePath, Id);
			_sequenceContext = null;
			base.Dispose(disposing);
		}

		
	}
}