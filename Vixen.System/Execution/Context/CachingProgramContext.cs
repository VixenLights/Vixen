using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution.Context {
	public abstract class CachingProgramContext : ProgramContext {
		private PreFilteringBufferingDataSource _dataSource;

		public enum CachingLevel {
			Sequence,
			Context
		};

		protected CachingProgramContext(CachingLevel cachingLevel) {
			_dataSource = new PreFilteringBufferingDataSource(cachingLevel == CachingLevel.Context);
		}

		public override IMutableDataSource ContextDataSource {
			get { return _dataSource; }
		}

		protected override IDataSource _DataSource {
			get { return ContextDataSource; }
		}

		public override IProgram Program {
			get { return base.Program; }
			set {
				base.Program = value;
				_dataSource.ContextName = value.Name;
			}
		}
	}
}
