namespace Vixen.IO {
	abstract class SystemConfigFilePolicy : IFilePolicy {
		public void Write() {
			WriteContextFlag();
			WriteIdentity();
			WriteAlternateDataDirectory();
			WriteChannels();
			WriteNodes();
			WriteControllers();
			WriteControllerLinks();
			WriteChannelPatching();
			WriteDisabledControllers();
		}

		protected abstract void WriteContextFlag();
		protected abstract void WriteIdentity();
		protected abstract void WriteAlternateDataDirectory();
		protected abstract void WriteChannels();
		protected abstract void WriteNodes();
		protected abstract void WriteControllers();
		protected abstract void WriteControllerLinks();
		protected abstract void WriteChannelPatching();
		protected abstract void WriteDisabledControllers();

		public void Read() {
			ReadContextFlag();
			ReadIdentity();
			//ReadAlternateDataDirectory();
			ReadChannels();
			ReadNodes();
			ReadControllers();
			ReadControllerLinks();
			ReadChannelPatching();
			ReadDisabledControllers();
		}

		protected abstract void ReadContextFlag();
		protected abstract void ReadIdentity();
		//protected abstract void ReadAlternateDataDirectory();
		protected abstract void ReadChannels();
		protected abstract void ReadNodes();
		protected abstract void ReadControllers();
		protected abstract void ReadControllerLinks();
		protected abstract void ReadChannelPatching();
		protected abstract void ReadDisabledControllers();

		public int GetVersion() {
			return 7;
		}
	}
}
