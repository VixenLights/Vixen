using System;
using System.Collections.Generic;

namespace Vixen.Sys.Output {
	class OutputDataPolicyCache : IOutputDataPolicyProvider {
		private IDataPolicyFactory _dataPolicyFactory;
		private Dictionary<Guid, IDataPolicy> _outputDataPolicies;

		public OutputDataPolicyCache() {
			_outputDataPolicies = new Dictionary<Guid, IDataPolicy>();
		}

		public void UseFactory(IDataPolicyFactory dataPolicyFactory) {
			if(dataPolicyFactory == null) throw new ArgumentNullException("dataPolicyFactory");

			lock(_outputDataPolicies) {
				_ClearDataPolicyPool();
				_dataPolicyFactory = dataPolicyFactory;
			}
		}

		public IDataPolicy GetDataPolicyForOutput(CommandOutput output) {
			return _GetDataPolicy(output) ?? _CreateDataPolicy(output);
		}

		private void _ClearDataPolicyPool() {
			lock(_outputDataPolicies) {
				_outputDataPolicies.Clear();
			}
		}

		private IDataPolicy _GetDataPolicy(CommandOutput output) {
			lock(_outputDataPolicies) {
				IDataPolicy dataPolicy;
				_outputDataPolicies.TryGetValue(output.Id, out dataPolicy);
				return dataPolicy;
			}
		}

		private IDataPolicy _CreateDataPolicy(CommandOutput output) {
			IDataPolicy dataPolicy = _dataPolicyFactory.CreateDataPolicy();
			_AddDataPolicy(output, dataPolicy);
			return dataPolicy;
		}

		private void _AddDataPolicy(CommandOutput output, IDataPolicy dataPolicy) {
			lock(_outputDataPolicies) {
				_outputDataPolicies[output.Id] = dataPolicy;
			}
		}
	}
}
