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
			WriteChannelPatching();
			WriteDisabledControllers();
			WritePreviews();
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
		protected abstract void WriteChannelPatching();
		protected abstract void WriteDisabledControllers();
		protected abstract void WritePreviews();

		public void Read() {
			ReadContextFlag();
			ReadIdentity();
			//ReadAlternateDataDirectory();
			ReadFilterEvaluationAllowance();
			ReadChannels();
			ReadNodes();
			ReadControllers();
			ReadControllerLinks();
			ReadSmartControllers();
			ReadChannelPatching();
			ReadDisabledControllers();
			ReadPreviews();
		}

		protected abstract void ReadContextFlag();
		protected abstract void ReadIdentity();
		//protected abstract void ReadAlternateDataDirectory();
		protected abstract void ReadFilterEvaluationAllowance();
		protected abstract void ReadChannels();
		protected abstract void ReadNodes();
		protected abstract void ReadControllers();
		protected abstract void ReadControllerLinks();
		protected abstract void ReadSmartControllers();
		protected abstract void ReadChannelPatching();
		protected abstract void ReadDisabledControllers();
		protected abstract void ReadPreviews();

		public int GetVersion() {
			return 11;
		}
	}
}
