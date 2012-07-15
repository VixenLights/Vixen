using Vixen.Execution.DataSource;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace Vixen.Execution.Context {
	[Context(ContextTargetType.Sequence, ContextCaching.ContextLevelCaching)]
	class CachingSequenceContext : SequenceContext {
		private PreFilteringBufferingDataSource _dataSource;

		public CachingSequenceContext() {
			_dataSource = new PreFilteringBufferingDataSource(true);
		}

		protected override IDataSource _DataSource {
			get { return _dataSource; }
		}

		protected override void OnSequenceStarted(SequenceStartedEventArgs e) {
			_dataSource.ContextName = Name;
			_dataSource.SetSequence(Sequence);
			_dataSource.Start();
			base.OnSequenceStarted(e);
		}

		protected override void OnSequenceEnded(SequenceEventArgs e) {
			base.OnSequenceEnded(e);
			_dataSource.Stop();
		}
	}
}
