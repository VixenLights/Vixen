using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class LiveContext : Context {
		private LiveDataSource _dataSource;

		public LiveContext(string name)
			: base(name) {
			_dataSource = new LiveDataSource();
		}

		public void Execute(EffectNode data) {
			_dataSource.AddData(data);
		}

		public void Execute(EffectNode[] data) {
			_dataSource.AddData(data);
		}

		protected override IDataSource _DataSource {
			get { return _dataSource; }
		}

		protected override ITiming _TimingSource {
			get { return Vixen.Sys.Execution.SystemTime; }
		}
	}
}
