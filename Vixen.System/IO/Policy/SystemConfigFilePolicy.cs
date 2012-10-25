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
			WritePreviews();
			WriteFilters();
			WriteDataFlowPatching();
			WriteDisabledDevices();
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
		protected abstract void WritePreviews();
		protected abstract void WriteFilters();
		protected abstract void WriteDataFlowPatching();
		protected abstract void WriteDisabledDevices();

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
			ReadPreviews();
			ReadFilters();
			ReadDataFlowPatching();
			ReadDisabledDevices();
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
		protected abstract void ReadPreviews();
		protected abstract void ReadFilters();
		protected abstract void ReadDataFlowPatching();
		protected abstract void ReadDisabledDevices();
	}
}
