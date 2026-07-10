namespace Vixen.IO.Policy {
	abstract class SequenceFilePolicy : IFilePolicy {
		virtual public void Write() {
			WriteLength();
			WriteTimingSource();
			WriteMedia();
			WriteModuleData();
			WriteEffectNodes();
			WriteFilterNodes();
		}

		protected abstract void WriteLength();
		protected abstract void WriteTimingSource();
		protected abstract void WriteMedia();
		protected abstract void WriteModuleData();
		protected abstract void WriteEffectNodes();
		protected abstract void WriteFilterNodes();

		virtual public void Read() {
			ReadLength();
			ReadTimingSource();
			ReadMedia();
			ReadModuleData();
			ReadEffectNodes();
			ReadFilterNodes();
		}

		protected abstract void ReadLength();
		protected abstract void ReadTimingSource();
		protected abstract void ReadMedia();
		protected abstract void ReadModuleData();
		protected abstract void ReadEffectNodes();
		protected abstract void ReadFilterNodes();

		public int GetVersion() {
			return 2;
		}
	}
}
