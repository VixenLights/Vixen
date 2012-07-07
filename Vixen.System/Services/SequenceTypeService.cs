using System;
using Vixen.Execution;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace Vixen.Services {
	public class SequenceTypeService {
		static private SequenceTypeService _instance;

		private SequenceTypeService() { }

		public static SequenceTypeService Instance {
			get { return _instance ?? (_instance = new SequenceTypeService()); }
		}

		public ISequenceTypeModuleInstance CreateSequenceFactory(string fileType) {
			SequenceTypeModuleManagement manager = Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			return manager.Get(fileType);
		}

		public ISequenceExecutor CreateSequenceExecutor(ISequence sequence) {
			SequenceTypeModuleManagement manager = Modules.GetManager<ISequenceTypeModuleInstance, SequenceTypeModuleManagement>();
			var sequenceTypeFactory = manager.GetFactory(sequence);
			if(sequenceTypeFactory != null) {
				ISequenceExecutor sequenceExecutor = sequenceTypeFactory.CreateExecutor();
				if(sequenceExecutor == null) throw new InvalidOperationException("No executor exists for sequence of type " + sequence.GetType()  + ".");
				return sequenceExecutor;
			}
			return null;
		}
	}
}
