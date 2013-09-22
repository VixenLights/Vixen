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
	public class SequenceAction : Action, IDisposable
	{
		private IContext _sequenceContext = null;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

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

				//_sequenceContext.Play(TimeSpan.Zero, _sequenceContext.Sequence.Length);
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
			_sequenceContext.Stop();
			base.Stop();
		}

		public override bool PreProcessingCompleted
		{
			get
			{
				return SequenceChanged();
			}
			set
			{
				base.PreProcessingCompleted = value;
			}
		}

		public override void PreProcess()
		{
			try
			{
				if (_sequenceContext == null || SequenceChanged())
				{
					if (_sequenceContext != null)
					{
						_sequenceContext.Dispose();
						_sequenceContext = null;
					}

					ISequence sequence = SequenceService.Instance.Load(ShowItem.Sequence_FileName);
					// Why doesn't this work?
					//IContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.ContextLevelCaching), sequence);
					IContext context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);

					foreach (IEffectNode effectNode in sequence.SequenceData.EffectData.Cast<IEffectNode>())
					{
						effectNode.Effect.PreRender();
					}

					context.ContextEnded += sequence_Ended;

					_sequenceContext = context;
				}
				PreProcessingCompleted = true;
			}
			catch (Exception ex)
			{
				Logging.Error("Could not pre-render sequence " + ShowItem.Sequence_FileName + "; " + ex.Message);
			}
		}

		DateTime _lastSequenceDateTime = DateTime.Now;
		private bool SequenceChanged()
		{
			//Console.WriteLine("SequenceFileUpdated");
			bool _datesEqual = false;

			if (System.IO.File.Exists(ShowItem.Sequence_FileName))
			{
				DateTime _lastWriteTime = System.IO.File.GetLastWriteTime(ShowItem.Sequence_FileName);
				_datesEqual = (_lastSequenceDateTime.CompareTo(_lastWriteTime) != 0);
				_lastSequenceDateTime = _lastWriteTime;
			}
			return !_datesEqual;
		}

		private void sequence_Ended(object sender, EventArgs e)
		{
			IContext context = sender as IContext;
			base.Complete();
		}

		~SequenceAction()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_sequenceContext != null)
				_sequenceContext.Dispose();

			_sequenceContext = null;

			GC.SuppressFinalize(this);
		}
	}
}