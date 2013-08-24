using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Services;

namespace VixenModules.App.Shows
{
	public class SequenceAction : Action
	{
		private ISequenceContext _sequenceContext = null;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public SequenceAction(ShowItem showItem)
			: base(showItem)
		{
		}

		public override void Execute()
		{
			base.Execute();

			try
			{
				if (_sequenceContext == null)
					PreProcess();

				_sequenceContext.ContextEnded += sequence_Ended;
				_sequenceContext.Play(TimeSpan.Zero, _sequenceContext.Sequence.Length);
			}
			catch (Exception ex)
			{
			    Logging.Error("Could not execute sequence " + ShowItem.Sequence_FileName + "; " + ex.Message);
			}
		}

		public override TimeSpan Duration()
		{
			if (_sequenceContext != null) 
				return _sequenceContext.Sequence.Length;
			else
				return TimeSpan.Zero;
		}

		public override void Stop()
		{
			_sequenceContext.Stop();
			base.Stop();
		}

		public override void PreProcess()
		{
			try
			{
				if (_sequenceContext == null)
				{
					ISequence sequence = SequenceService.Instance.Load(ShowItem.Sequence_FileName);
					ISequenceContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);

					foreach (IEffectNode effectNode in sequence.SequenceData.EffectData.Cast<IEffectNode>())
					{
						effectNode.Effect.PreRender();
					}
					_sequenceContext = context;
				}
				PreProcessingCompleted = true;
			}
			catch (Exception ex)
			{
				Logging.Error("Could not pre-render sequence " + ShowItem.Sequence_FileName + "; " + ex.Message);
			}
		}

		private void sequence_Ended(object sender, EventArgs e)
		{
			ISequenceContext context = sender as ISequenceContext;
			context.ContextEnded -= sequence_Ended;
			//context.Stop();
			//context.Play(TimeSpan.Zero, TimeSpan.Zero);
			//context.T
			base.Complete();
		}

	}
}