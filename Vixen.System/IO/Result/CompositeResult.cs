using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.IO.Result {
	class CompositeResult : IEnumerable<IResult> {
		private List<IResult> _results;

		public CompositeResult() {
			_results = new List<IResult>();
		}

		public void AddResults(IResult operationResults) {
			_results.Add(operationResults);
		}

		public void AddResults(IEnumerable<IResult> operationResults) {
			_results.AddRange(operationResults);
		}

		public bool Success {
			get { return _results.All(x => x.Success); }
		}

		public string Message {
			get { return string.Join(Environment.NewLine, _results); }
		}

		//public object Object {
		//    get { return _results.Select(x => x.Object).Last(); }
		//}

		public IEnumerator<IResult> GetEnumerator() {
			return _results.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
