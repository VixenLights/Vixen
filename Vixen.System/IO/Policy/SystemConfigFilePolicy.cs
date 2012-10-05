namespace Vixen.IO.Policy {
	abstract class SystemConfigFilePolicy : IFilePolicy {
		public void Write() {
			WriteContextFlag();
			WriteIdentity();
			WriteAlternateDataDirectory();
			WriteFilterEvaluationAllowance();
			WriteChannels();
			WriteNodes();
			WriteControllers();
			WriteControllerLinks();
			WriteSmartControllers();
			WriteDisabledControllers();
			WritePreviews();
			WriteFilters();
			WriteDataFlowPatching();
		}

		protected abstract void WriteContextFlag();
		protected abstract void WriteIdentity();
		protected abstract void WriteAlternateDataDirectory();
		protected abstract void WriteFilterEvaluationAllowance();
		protected abstract void WriteChannels();
		protected abstract void WriteNodes();
		protected abstract void WriteControllers();
		protected abstract void WriteControllerLinks();
		protected abstract void WriteSmartControllers();
		protected abstract void WriteDisabledControllers();
		protected abstract void WritePreviews();
		protected abstract void WriteFilters();
		protected abstract void WriteDataFlowPatching();

		public void Read() {
			ReadContextFlag();
			ReadIdentity();
			ReadAlternateDataDirectory();
			ReadFilterEvaluationAllowance();
			ReadChannels();
			ReadNodes();
			ReadControllers();
			ReadControllerLinks();
			ReadSmartControllers();
			ReadDisabledControllers();
			ReadPreviews();
			ReadFilters();
			ReadDataFlowPatching();
		}

		protected abstract void ReadContextFlag();
		protected abstract void ReadIdentity();
		protected abstract void ReadAlternateDataDirectory();
		protected abstract void ReadFilterEvaluationAllowance();
		protected abstract void ReadChannels();
		protected abstract void ReadNodes();
		protected abstract void ReadControllers();
		protected abstract void ReadControllerLinks();
		protected abstract void ReadSmartControllers();
		protected abstract void ReadDisabledControllers();
		protected abstract void ReadPreviews();
		protected abstract void ReadFilters();
		protected abstract void ReadDataFlowPatching();
	}
}
