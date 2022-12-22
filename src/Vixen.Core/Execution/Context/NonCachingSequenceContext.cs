using Vixen.Execution.DataSource;
using Vixen.Sys.Attribute;

namespace Vixen.Execution.Context
{
	[Context(ContextTargetType.Sequence, ContextCaching.NoCaching)]
	internal class NonCachingSequenceContext : SequenceContext
	{
		private SequenceDataPump _dataSource;

		public NonCachingSequenceContext()
		{
			_dataSource = new SequenceDataPump();
		}

		protected override IDataSource _DataSource
		{
			get { return _dataSource; }
		}

		protected override void OnSequenceStarted(Sys.SequenceStartedEventArgs e)
		{
			_dataSource.Sequence = Sequence;
			_dataSource.Start();
			base.OnSequenceStarted(e);
		}

		protected override void OnSequenceReStarted(Sys.SequenceStartedEventArgs e)
		{
			_dataSource.Stop();
			_dataSource.Start();
			base.OnSequenceReStarted(e);
		}

		protected override void OnSequenceEnded(Sys.SequenceEventArgs e)
		{
			_dataSource.Stop();
			base.OnSequenceEnded(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (_DataSource != null)
					_dataSource.Dispose();

				_dataSource = null;
			}
			base.Dispose(disposing);
		}
	}
}